using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace OExm
{
    public partial class ExamPortal : System.Web.UI.Page
    {
        private DataTable Questions
        {
            get { return Session["Questions"] as DataTable; }
            set { Session["Questions"] = value; }
        }

        private Dictionary<int, string> ResponsesStatus
        {
            get { return Session["ResponsesStatus"] as Dictionary<int, string>; }
            set { Session["ResponsesStatus"] = value; }
        }

        private int StudentExamId
        {
            get { return Convert.ToInt32(Session["StudentExamId"]); }
        }

        private int CurrentIndex
        {
            get { return Convert.ToInt32(hfQuestionIndex.Value); }
            set { hfQuestionIndex.Value = value.ToString(); }
        }

        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(object sender, EventArgs e)
        {
            StopBrowserCaching();

            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (Session["StudentExamId"] == null || Request.QueryString["ExamId"] == null)
            {
                Response.Redirect("Instructions.aspx");
                return;
            }

            int examId = Convert.ToInt32(Request.QueryString["ExamId"]);

            if (Request["__EVENTTARGET"] == "TabSwitch")
            {
                SaveViolation("Tab Switch");
            }

            int secondsLeft = GetSecondsLeft(examId);
            if (secondsLeft <= 0)
            {
                SubmitExam();
                return;
            }

            hfRemainingSeconds.Value = secondsLeft.ToString();
            lblTimer.Text = TimeSpan.FromSeconds(secondsLeft).ToString(@"mm\:ss");

            if (!IsPostBack)
            {
                LoadQuestionsForAttempt(examId);
                CurrentIndex = 0;
                MarkCurrentQuestionAsVisited();
                ShowQuestion(0);
            }
        }

        private void StopBrowserCaching()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
        }

        // =========================================================
        // TIMER
        // =========================================================

        private int GetSecondsLeft(int examId)
        {
            object startTimeValue = DatabaseHelper.ExecuteScalar(
                "SELECT StartTime FROM StudentExams WHERE StudentExamId=@id",
                new SqlParameter[] { new SqlParameter("@id", StudentExamId) });

            object durationValue = DatabaseHelper.ExecuteScalar(
                "SELECT DurationMinutes FROM Exams WHERE ExamId=@id",
                new SqlParameter[] { new SqlParameter("@id", examId) });

            if (startTimeValue == null || startTimeValue == DBNull.Value ||
                durationValue == null || durationValue == DBNull.Value)
            {
                Response.Redirect("Instructions.aspx");
                return 0;
            }

            DateTime startTime = Convert.ToDateTime(startTimeValue);
            int durationMinutes = Convert.ToInt32(durationValue);

            double secondsPassed = (DateTime.Now - startTime).TotalSeconds;
            double totalSeconds = durationMinutes * 60;

            return (int)(totalSeconds - secondsPassed);
        }

        // =========================================================
        // LOADING THE QUESTIONS FOR THIS ATTEMPT (+ header info)
        // =========================================================

        private void LoadQuestionsForAttempt(int examId)
        {
            DataTable examInfo = DatabaseHelper.ExecuteQuery(
                @"SELECT e.QuestionBankId, e.QuestionCount, e.QuestionMode, e.ExamName,
                         e.PositiveMarksPerQuestion, e.NegativeMarksPerQuestion, qb.BankName
                  FROM Exams e
                  LEFT JOIN QuestionBanks qb ON e.QuestionBankId = qb.BankId
                  WHERE e.ExamId=@id",
                new SqlParameter[] { new SqlParameter("@id", examId) });

            if (examInfo.Rows.Count == 0)
            {
                Response.Redirect("Instructions.aspx");
                return;
            }

            // Header labels for the exam portal top bar
            lblExamNameHeader.Text = examInfo.Rows[0]["ExamName"].ToString();
            lblSubjectHeader.Text = examInfo.Rows[0]["BankName"] == DBNull.Value ? "General" : examInfo.Rows[0]["BankName"].ToString();
            lblPositiveMark.Text = examInfo.Rows[0]["PositiveMarksPerQuestion"] == DBNull.Value ? "0" : examInfo.Rows[0]["PositiveMarksPerQuestion"].ToString();
            lblNegativeMark.Text = examInfo.Rows[0]["NegativeMarksPerQuestion"] == DBNull.Value ? "0" : examInfo.Rows[0]["NegativeMarksPerQuestion"].ToString();

            int questionCount = examInfo.Rows[0]["QuestionCount"] == DBNull.Value
                ? 0
                : Convert.ToInt32(examInfo.Rows[0]["QuestionCount"]);

            string bankName = examInfo.Rows[0]["BankName"] == DBNull.Value ? "" : examInfo.Rows[0]["BankName"].ToString();
            string tableName = BankHelper.ToTableName(bankName);

            // Load random questions from the bank's own physical table
            // e.g. SELECT TOP 20 * FROM [Java] ORDER BY NEWID()
            // For Manual mode: only fetch the SlNos the admin hand-picked
            DataTable rawRows;
            string mode = examInfo.Rows[0]["QuestionMode"].ToString();

            if (mode == "Manual")
            {
                // Get the SlNos saved in ExamQuestions for this exam
                DataTable selectedSlNos = DatabaseHelper.ExecuteQuery(
                    "SELECT QuestionId FROM ExamQuestions WHERE ExamId=@id",
                    new SqlParameter[] { new SqlParameter("@id", examId) });

                if (selectedSlNos.Rows.Count == 0)
                {
                    Questions = new DataTable();
                    return;
                }

                // Build a comma-separated list of SlNos for IN clause
                System.Text.StringBuilder slNoList = new System.Text.StringBuilder();
                foreach (DataRow slRow in selectedSlNos.Rows)
                {
                    if (slNoList.Length > 0) slNoList.Append(",");
                    // Safe: these are integers saved by our own code
                    slNoList.Append(Convert.ToInt32(slRow["QuestionId"]));
                }

                rawRows = DatabaseHelper.ExecuteQuery(
                    "SELECT * FROM [" + tableName + "] WHERE SlNo IN (" + slNoList + ") ORDER BY NEWID()");
            }
            else
            {
                // Random mode: just pull N random rows from the bank table
                rawRows = BankHelper.GetRandomQuestions(tableName, questionCount);
            }

            // Add CorrectOption column if not already present (Manual mode raw query doesn't have it)
            if (!rawRows.Columns.Contains("CorrectOption"))
            {
                rawRows.Columns.Add("CorrectOption", typeof(string));
                foreach (DataRow row in rawRows.Rows)
                {
                    row["CorrectOption"] = BankHelper.DetermineCorrectOption(
                        row["Ans"].ToString(), row["M1"].ToString(),
                        row["M2"].ToString(), row["M3"].ToString(), row["M4"].ToString());
                }
            }
            // (QuestionId, QuestionText, OptionA..D, CorrectOption)
            DataTable questions = new DataTable();
            questions.Columns.Add("QuestionId", typeof(int));
            questions.Columns.Add("QuestionText", typeof(string));
            questions.Columns.Add("OptionA", typeof(string));
            questions.Columns.Add("OptionB", typeof(string));
            questions.Columns.Add("OptionC", typeof(string));
            questions.Columns.Add("OptionD", typeof(string));
            questions.Columns.Add("CorrectOption", typeof(string));

            foreach (DataRow raw in rawRows.Rows)
            {
                DataRow q = questions.NewRow();
                // Use SlNo as a temporary QuestionId for this session
                q["QuestionId"] = Convert.ToInt32(raw["SlNo"]);
                q["QuestionText"] = raw["Question"].ToString();
                q["OptionA"] = raw["M1"].ToString();
                q["OptionB"] = raw["M2"].ToString();
                q["OptionC"] = raw["M3"].ToString();
                q["OptionD"] = raw["M4"].ToString();
                q["CorrectOption"] = raw["CorrectOption"].ToString();
                questions.Rows.Add(q);
            }

            // Also store the bank table name so SaveAnswer can use it
            Session["ExamBankTable"] = tableName;

            Questions = questions;
        }

        // =========================================================
        // SHOWING A QUESTION ON SCREEN
        // =========================================================

        private void ShowQuestion(int index)
        {
            DataTable questions = Questions;
            if (questions == null || questions.Rows.Count == 0 || index < 0 || index >= questions.Rows.Count)
                return;

            DataRow question = questions.Rows[index];
            int questionId = Convert.ToInt32(question["QuestionId"]);

            lblQuestion.Text = question["QuestionText"].ToString();

            rblOptions.Items[0].Text = "A. " + question["OptionA"].ToString();
            rblOptions.Items[1].Text = "B. " + question["OptionB"].ToString();
            rblOptions.Items[2].Text = "C. " + question["OptionC"].ToString();
            rblOptions.Items[3].Text = "D. " + question["OptionD"].ToString();

            // Restore whatever answer was saved before (if any), safely.
            // FindByValue guards against a value that doesn't match A/B/C/D.
            rblOptions.ClearSelection();
            string savedAnswer = GetSavedAnswer(questionId);
            if (rblOptions.Items.FindByValue(savedAnswer) != null)
            {
                rblOptions.SelectedValue = savedAnswer;
            }

            RefreshResponsesStatus();
            UpdateOverviewCounts(questions.Rows.Count);
            rpPalette.DataSource = questions;
            rpPalette.DataBind();
        }

        private string GetSavedAnswer(int questionId)
        {
            object result = DatabaseHelper.ExecuteScalar(
                "SELECT SelectedOption FROM StudentResponses WHERE StudentExamId=@se AND QuestionId=@q",
                new SqlParameter[]
                {
                    new SqlParameter("@se", StudentExamId),
                    new SqlParameter("@q", questionId)
                });

            return (result == null || result == DBNull.Value) ? "" : result.ToString().Trim();
        }

        // Loads the Answered / NotAnswered / MarkedForReview status of every
        // question once per page view, so the palette buttons don't need
        // one database call per button.
        private void RefreshResponsesStatus()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT QuestionId, Status FROM StudentResponses WHERE StudentExamId=@se",
                new SqlParameter[] { new SqlParameter("@se", StudentExamId) });

            Dictionary<int, string> statuses = new Dictionary<int, string>();
            foreach (DataRow row in dt.Rows)
            {
                statuses[Convert.ToInt32(row["QuestionId"])] = row["Status"].ToString();
            }

            ResponsesStatus = statuses;
        }

        // Updates the "Exam Overview" counts in the sidebar
        // (Attempted / To Review / Not Attempted).
        private void UpdateOverviewCounts(int totalQuestions)
        {
            Dictionary<int, string> statuses = ResponsesStatus;

            int answered = 0;
            int review = 0;

            if (statuses != null)
            {
                foreach (string status in statuses.Values)
                {
                    if (status == "Answered") answered++;
                    else if (status == "MarkedForReview") review++;
                }
            }

            lblAttemptedCount.Text = answered.ToString();
            lblReviewCount.Text = review.ToString();
            lblNotAttemptedCount.Text = (totalQuestions - answered - review).ToString();
        }

        // =========================================================
        // SAVING ANSWERS
        // =========================================================

        private void MarkCurrentQuestionAsVisited()
        {
            DataTable questions = Questions;
            int index = CurrentIndex;
            if (questions == null || index >= questions.Rows.Count) return;

            int questionId = Convert.ToInt32(questions.Rows[index]["QuestionId"]);

            if (!ResponseExists(questionId))
            {
                SaveAnswer(questionId, "", "NotAnswered");
            }
        }

        private void SaveCurrentAnswer(string status)
        {
            DataTable questions = Questions;
            int index = CurrentIndex;
            if (questions == null || index >= questions.Rows.Count) return;

            int questionId = Convert.ToInt32(questions.Rows[index]["QuestionId"]);
            string selectedOption = rblOptions.SelectedValue;

            // Nothing picked and not being marked for review = nothing new to save.
            if (string.IsNullOrEmpty(selectedOption) && status != "MarkedForReview") return;

            SaveAnswer(questionId, selectedOption, status);
        }

        private bool ResponseExists(int questionId)
        {
            object count = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM StudentResponses WHERE StudentExamId=@se AND QuestionId=@q",
                new SqlParameter[]
                {
                    new SqlParameter("@se", StudentExamId),
                    new SqlParameter("@q", questionId)
                });

            return Convert.ToInt32(count) > 0;
        }

        private void SaveAnswer(int questionId, string selectedOption, string status)
        {
            string query = ResponseExists(questionId)
                ? "UPDATE StudentResponses SET SelectedOption=@s, Status=@st WHERE StudentExamId=@se AND QuestionId=@q"
                : "INSERT INTO StudentResponses (StudentExamId, QuestionId, SelectedOption, Status) VALUES (@se, @q, @s, @st)";

            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[]
            {
                new SqlParameter("@s", selectedOption),
                new SqlParameter("@st", status),
                new SqlParameter("@se", StudentExamId),
                new SqlParameter("@q", questionId)
            });
        }

        // =========================================================
        // NAVIGATION BUTTONS
        // =========================================================

        protected void btnNext_Click(object sender, EventArgs e)
        {
            SaveCurrentAnswer("Answered");
            GoToQuestion(CurrentIndex + 1);
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            SaveCurrentAnswer("Answered");
            GoToQuestion(CurrentIndex - 1);
        }

        protected void btnMarkForReview_Click(object sender, EventArgs e)
        {
            SaveCurrentAnswer("MarkedForReview");
            GoToQuestion(CurrentIndex + 1);
        }

        protected void btnClearResponse_Click(object sender, EventArgs e)
        {
            rblOptions.ClearSelection();
        }

        protected void rpPalette_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "JumpTo") return;

            SaveCurrentAnswer("Answered");
            GoToQuestion(Convert.ToInt32(e.CommandArgument));
        }

        private void GoToQuestion(int newIndex)
        {
            DataTable questions = Questions;
            if (questions == null || questions.Rows.Count == 0) return;

            if (newIndex < 0) newIndex = 0;
            if (newIndex > questions.Rows.Count - 1) newIndex = questions.Rows.Count - 1;

            CurrentIndex = newIndex;
            MarkCurrentQuestionAsVisited();
            ShowQuestion(newIndex);
        }

        // =========================================================
        // SUBMITTING THE EXAM
        // =========================================================

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SaveCurrentAnswer("Answered");
            SubmitExam();
        }

        private void SubmitExam()
        {
            object currentStatus = DatabaseHelper.ExecuteScalar(
                "SELECT Status FROM StudentExams WHERE StudentExamId=@id",
                new SqlParameter[] { new SqlParameter("@id", StudentExamId) });

            if (currentStatus != null && currentStatus.ToString() == "Completed")
            {
                Response.Redirect("Result.aspx");
                return;
            }

            DatabaseHelper.ExecuteNonQuery(
                "UPDATE StudentExams SET EndTime=GETDATE(), Status='Completed' WHERE StudentExamId=@se",
                new SqlParameter[] { new SqlParameter("@se", StudentExamId) });

            DatabaseHelper.ExecuteNonQuery(
                "INSERT INTO ActivityLogs (UserId, ActionName) VALUES (@u, 'Exam Submitted')",
                new SqlParameter[] { new SqlParameter("@u", Session["UserId"]) });

            Response.Redirect("Result.aspx");
        }

        // =========================================================
        // TAB-SWITCH VIOLATIONS
        // =========================================================

        private void SaveViolation(string type)
        {
            DateTime? lastViolation = Session["LastViolation"] as DateTime?;
            if (lastViolation != null && (DateTime.Now - lastViolation.Value).TotalSeconds < 10) return;

            DatabaseHelper.ExecuteNonQuery(
                "INSERT INTO ExamViolations (StudentExamId, ViolationType) VALUES (@se, @t)",
                new SqlParameter[]
                {
                    new SqlParameter("@se", StudentExamId),
                    new SqlParameter("@t", type)
                });

            Session["LastViolation"] = DateTime.Now;
        }

        // =========================================================
        // QUESTION NAVIGATOR COLORS (right-hand number buttons)
        // =========================================================

        public string GetPaletteClass(object questionIdObj)
        {
            if (questionIdObj == null || questionIdObj == DBNull.Value) return "nav-btn not-visited";

            int questionId = Convert.ToInt32(questionIdObj);
            DataTable questions = Questions;
            int currentIndex = CurrentIndex;

            bool isCurrentQuestion = questions != null
                && currentIndex < questions.Rows.Count
                && Convert.ToInt32(questions.Rows[currentIndex]["QuestionId"]) == questionId;

            if (isCurrentQuestion) return "nav-btn current";

            return "nav-btn " + ColorForStatus(questionId);
        }

        private string ColorForStatus(int questionId)
        {
            Dictionary<int, string> statuses = ResponsesStatus;
            string status = (statuses != null && statuses.ContainsKey(questionId)) ? statuses[questionId] : "";

            switch (status)
            {
                case "Answered": return "answered";
                case "MarkedForReview": return "review";
                case "NotAnswered": return "not-answered";
                default: return "not-visited";
            }
        }
    }
}

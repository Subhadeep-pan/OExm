using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace OExm
{
    public partial class ExamPortal : System.Web.UI.Page
    {
        private DataTable questions;

        protected void Page_Load(object sender, EventArgs e)
        {


            // LOGIN CHECK
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (Session["StudentExamId"] == null)
            {
                Response.Redirect("Instructions.aspx");
                return;
            }
            
            int studentExamId = Convert.ToInt32(Session["StudentExamId"]);
            int examId = Convert.ToInt32(Request.QueryString["ExamId"]);
            if (Request["__EVENTTARGET"] == "TabSwitch")
            {
                if (Session["LastViolation"] == null ||
                    (DateTime.Now - Convert.ToDateTime(Session["LastViolation"])).TotalSeconds > 10)
                {
                    SaveViolation("Tab Switch");
                    Session["LastViolation"] = DateTime.Now;
                }
            }

            // Calculate remaining seconds securely from Database StartTime and Exam Duration
            string attemptQuery = "SELECT StartTime FROM StudentExams WHERE StudentExamId=@se";
            SqlParameter[] attemptParams = { new SqlParameter("@se", studentExamId) };
            DataTable dtAttempt = DatabaseHelper.ExecuteQuery(attemptQuery, attemptParams);

            string examQuery = "SELECT DurationMinutes FROM Exams WHERE ExamId=@id";
            SqlParameter[] examParams = { new SqlParameter("@id", examId) };
            DataTable dtExam = DatabaseHelper.ExecuteQuery(examQuery, examParams);

            if (dtAttempt.Rows.Count > 0 && dtExam.Rows.Count > 0)
            {
                DateTime startTime = Convert.ToDateTime(dtAttempt.Rows[0]["StartTime"]);
                int durationMins = Convert.ToInt32(dtExam.Rows[0]["DurationMinutes"]);
                double elapsedSecs = (DateTime.Now - startTime).TotalSeconds;
                double remainingSecs = (durationMins * 60) - elapsedSecs;

                if (remainingSecs <= 0)
                {
                    AutoSubmitExam();
                    return;
                }

                hfRemainingSeconds.Value = ((int)remainingSecs).ToString();
                lblTimer.Text =
    TimeSpan.FromSeconds(
        (int)remainingSecs)
    .ToString(@"mm\:ss");
            }
            else
            {
                Response.Redirect("Default.aspx");
                return;
            }
            if (Request["__EVENTTARGET"] == "FullscreenExit")
            {
                if (Session["LastViolation"] == null ||
                    (DateTime.Now -
                     Convert.ToDateTime(
                         Session["LastViolation"]))
                    .TotalSeconds > 10)
                {
                    SaveViolation(
                        "Fullscreen Exit");

                    Session["LastViolation"] =
                        DateTime.Now;
                }
            }


            if (!IsPostBack)
            {
                LoadQuestions();
                
                // Initialize the first question as Visited (NotAnswered)
                MarkAsVisited(0);

                hfQuestionIndex.Value = "0";
                ShowQuestion(0);
            }
        }

        private void LoadQuestions()
        {
            if (Request.QueryString["ExamId"] == null)
            {
                Response.Redirect("Instructions.aspx");
                return;
            }
            int examId = Convert.ToInt32(Request.QueryString["ExamId"]);

            string settingsQuery =
@"SELECT
    QuestionBankId,
    QuestionCount,
    QuestionMode
FROM Exams
WHERE ExamId=@id";

            SqlParameter[] settingsParams =
            {
    new SqlParameter("@id", examId)
};

            DataTable settings =
                DatabaseHelper.ExecuteQuery(
                    settingsQuery,
                    settingsParams);

            int bankId =
                Convert.ToInt32(
                    settings.Rows[0]["QuestionBankId"]);

            int count =
                Convert.ToInt32(
                    settings.Rows[0]["QuestionCount"]);
            string mode =
    settings.Rows[0]["QuestionMode"]
    .ToString();

            string query;
            SqlParameter[] parameters = null;

            if (mode == "Random")
            {
                query = @"
    SELECT TOP (@count) *
    FROM Questions
    WHERE BankId=@bank
    AND IsActive=1
    ORDER BY NEWID()";

                parameters = new SqlParameter[]
 {
    new SqlParameter("@count", count),
    new SqlParameter("@bank", bankId)
 };
            }
            else
            {
                query = @"
    SELECT TOP (@count) q.*
    FROM Questions q
    INNER JOIN ExamQuestions eq
    ON q.QuestionId = eq.QuestionId
    WHERE eq.ExamId=@id
    AND q.IsActive=1
    ORDER BY NEWID()";

                parameters = new SqlParameter[]
{
    new SqlParameter("@count", count),
    new SqlParameter("@id", examId)
};
            }

            questions =
                DatabaseHelper.ExecuteQuery(
                    query,
                    parameters);
            Session["Questions"] = questions;
           
        }

        private void ShowQuestion(int index)
        {
            questions = (DataTable)Session["Questions"];

            if (questions == null || questions.Rows.Count == 0)
                return;

            DataRow row = questions.Rows[index];

            lblQuestion.Text = row["QuestionText"].ToString();
            lblSectionName.Text =
    "Question Bank Exam";

            rblOptions.ClearSelection();
            rblOptions.Items[0].Text = row["OptionA"].ToString();
            rblOptions.Items[1].Text = row["OptionB"].ToString();
            rblOptions.Items[2].Text = row["OptionC"].ToString();
            rblOptions.Items[3].Text = row["OptionD"].ToString();

            // RESUME EXAM: Pre-populate previously recorded answer
            int qId = Convert.ToInt32(row["QuestionId"]);
            int studentExamId = Convert.ToInt32(Session["StudentExamId"]);
            
            string query = "SELECT SelectedOption FROM StudentResponses WHERE StudentExamId=@se AND QuestionId=@q";
            SqlParameter[] parameters = {
                new SqlParameter("@se", studentExamId),
                new SqlParameter("@q", qId)
            };
            
            object selected = DatabaseHelper.ExecuteScalar(query, parameters);
            if (selected != null && selected != DBNull.Value && !string.IsNullOrEmpty(selected.ToString()))
            {
                rblOptions.SelectedValue = selected.ToString();
            }

            // Sync and save the statuses list for Palette CSS bindings
            Session["ResponsesStatus"] = GetResponsesStatus();
            rpPalette.DataSource = questions;
            rpPalette.DataBind();
        }
       
        private Dictionary<int, string> GetResponsesStatus()
        {
            int studentExamId = Convert.ToInt32(Session["StudentExamId"]);
            string query = "SELECT QuestionId, Status FROM StudentResponses WHERE StudentExamId=@se";
            SqlParameter[] parameters = { new SqlParameter("@se", studentExamId) };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

            Dictionary<int, string> statusDict = new Dictionary<int, string>();
            foreach (DataRow r in dt.Rows)
            {
                int qId = Convert.ToInt32(r["QuestionId"]);
                statusDict[qId] = r["Status"].ToString();
            }
            return statusDict;
        }

        private void SaveViolation(string type)
        {
            if (Session["StudentExamId"] == null)
                return;

            string query = @"
    INSERT INTO ExamViolations
    (
        StudentExamId,
        ViolationType
    )
    VALUES
    (
        @se,
        @t
    )";

            System.Data.SqlClient.SqlParameter[] p =
            {
        new System.Data.SqlClient.SqlParameter(
            "@se",
            Session["StudentExamId"]),

        new System.Data.SqlClient.SqlParameter(
            "@t",
            type)
    };

            DatabaseHelper.ExecuteNonQuery(query, p);
        }
        public string GetPaletteClass(object questionIdObj)
        {
            if (questionIdObj == null || questionIdObj == DBNull.Value) return "palette-btn gray";
            int qId = Convert.ToInt32(questionIdObj);

            // Active displayed question detection
            int currentIndex = Convert.ToInt32(hfQuestionIndex.Value);
            questions = (DataTable)Session["Questions"];
            bool isActive = false;
            
            if (questions != null && currentIndex < questions.Rows.Count)
            {
                if (Convert.ToInt32(questions.Rows[currentIndex]["QuestionId"]) == qId)
                    isActive = true;
            }

            Dictionary<int, string> statuses = (Dictionary<int, string>)Session["ResponsesStatus"];
            string cssClass = "palette-btn";

            if (statuses != null && statuses.ContainsKey(qId))
            {
                string status = statuses[qId];
                if (status == "Answered") cssClass += " green";
                else if (status == "MarkedForReview") cssClass += " yellow";
                else if (status == "NotAnswered") cssClass += " red";
                else cssClass += " gray";
            }
            else
            {
                cssClass += " gray";
            }

            if (isActive) cssClass += " active-q";
            return cssClass;
        }

        private void MarkAsVisited(int index)
        {
            questions = (DataTable)Session["Questions"];
            if (questions == null || index >= questions.Rows.Count) return;

            int questionId = Convert.ToInt32(questions.Rows[index]["QuestionId"]);
            ViewState["QuestionId"] =
questionId;
            int studentExamId = Convert.ToInt32(Session["StudentExamId"]);

            // If no response row exists for this question, insert a 'NotAnswered' row to mark it Visited (Red)
            string checkQuery = "SELECT COUNT(*) FROM StudentResponses WHERE StudentExamId=@se AND QuestionId=@q";
            SqlParameter[] checkParams = {
                new SqlParameter("@se", studentExamId),
                new SqlParameter("@q", questionId)
            };
            object count = DatabaseHelper.ExecuteScalar(checkQuery, checkParams);

            if (Convert.ToInt32(count) == 0)
            {
                string insertQuery = "INSERT INTO StudentResponses (StudentExamId, QuestionId, SelectedOption, Status) VALUES (@se, @q, '', 'NotAnswered')";
                SqlParameter[] insertParams = {
                    new SqlParameter("@se", studentExamId),
                    new SqlParameter("@q", questionId)
                };
                DatabaseHelper.ExecuteNonQuery(insertQuery, insertParams);
            }
        }

        private void SaveAnswer(string status = "Answered")
        {
            questions = (DataTable)Session["Questions"];
            if (questions == null) return;

            int index = Convert.ToInt32(hfQuestionIndex.Value);
            if (index >= questions.Rows.Count) return;

            int questionId = Convert.ToInt32(questions.Rows[index]["QuestionId"]);
            string selected = rblOptions.SelectedValue;
            int studentExamId = Convert.ToInt32(Session["StudentExamId"]);

            // If they haven't chosen an option, but marked for review
            if (string.IsNullOrEmpty(selected))
            {
                if (status == "MarkedForReview")
                {
                    UpsertAnswer(studentExamId, questionId, "", "MarkedForReview");
                }
                return;
            }

            UpsertAnswer(studentExamId, questionId, selected, status);
        }

        private void UpsertAnswer(int studentExamId, int questionId, string selectedOption, string status)
        {
            string checkQuery = "SELECT COUNT(*) FROM StudentResponses WHERE StudentExamId=@se AND QuestionId=@q";
            SqlParameter[] checkParams = {
                new SqlParameter("@se", studentExamId),
                new SqlParameter("@q", questionId)
            };
            object count = DatabaseHelper.ExecuteScalar(checkQuery, checkParams);

            if (Convert.ToInt32(count) > 0)
            {
                string updateQuery = "UPDATE StudentResponses SET SelectedOption=@s, Status=@st WHERE StudentExamId=@se AND QuestionId=@q";
                SqlParameter[] updateParams = {
                    new SqlParameter("@s", selectedOption),
                    new SqlParameter("@st", status),
                    new SqlParameter("@se", studentExamId),
                    new SqlParameter("@q", questionId)
                };
                DatabaseHelper.ExecuteNonQuery(updateQuery, updateParams);
            }
            else
            {
                string insertQuery = "INSERT INTO StudentResponses (StudentExamId, QuestionId, SelectedOption, Status) VALUES (@se, @q, @s, @st)";
                SqlParameter[] insertParams = {
                    new SqlParameter("@se", studentExamId),
                    new SqlParameter("@q", questionId),
                    new SqlParameter("@s", selectedOption),
                    new SqlParameter("@st", status)
                };
                DatabaseHelper.ExecuteNonQuery(insertQuery, insertParams);
            }
        }

        private void AdvanceToNext()
        {
            int index = Convert.ToInt32(hfQuestionIndex.Value);
            questions = (DataTable)Session["Questions"];
            
            if (questions != null && index < questions.Rows.Count - 1)
            {
                index++;
                hfQuestionIndex.Value = index.ToString();
                
                // Mark the newly loaded question as Visited (Red)
                MarkAsVisited(index);
                
                ShowQuestion(index);
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            SaveAnswer("Answered");
            AdvanceToNext();
        }

        protected void btnMarkForReview_Click(object sender, EventArgs e)
        {
            SaveAnswer("MarkedForReview");
            AdvanceToNext();
        }
        
        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            // Auto Save current selection on back click
            SaveAnswer();

            int index = Convert.ToInt32(hfQuestionIndex.Value);
            if (index > 0)
            {
                index--;
                hfQuestionIndex.Value = index.ToString();
                ShowQuestion(index);
            }
        }

        protected void rpPalette_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "JumpTo")
            {
                // Auto Save current selection before jumping!
                SaveAnswer();

                int newIndex = Convert.ToInt32(e.CommandArgument);
                hfQuestionIndex.Value = newIndex.ToString();

                // Mark the jumped-to question as Visited
                MarkAsVisited(newIndex);
                
                ShowQuestion(newIndex);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SaveAnswer("Answered");


            SubmitExamAttempt();
        }

        private void AutoSubmitExam()
        {
            // Submit with current selection
            SaveAnswer();
            SubmitExamAttempt();
        }


        protected void btnClearResponse_Click(
    object sender,
    EventArgs e)
        {
            rblOptions.ClearSelection();
        }


        private void SubmitExamAttempt()
        {
            int studentExamId =
                Convert.ToInt32(
                    Session["StudentExamId"]);

            // PREVENT DUPLICATE SUBMISSION

            string checkQuery =
            @"SELECT Status
      FROM StudentExams
      WHERE StudentExamId=@id";

            SqlParameter[] checkParams =
            {
        new SqlParameter("@id",
            studentExamId)
    };

            object status =
                DatabaseHelper.ExecuteScalar(
                    checkQuery,
                    checkParams);

            if (status != null &&
                status.ToString() == "Completed")
            {
                Response.Redirect("Result.aspx");
                return;
            }

            // COMPLETE EXAM

            string query =
            @"UPDATE StudentExams
      SET EndTime = GETDATE(),
          Status = 'Completed'
      WHERE StudentExamId = @se";

            SqlParameter[] parameters =
            {
        new SqlParameter("@se",
            studentExamId)
    };

            DatabaseHelper.ExecuteNonQuery(
                query,
                parameters);

            // ACTIVITY LOG

            string logQuery =
            @"INSERT INTO ActivityLogs
      (UserId, ActionName)
      VALUES(@u,'Exam Submitted')";

            SqlParameter[] logParams =
            {
        new SqlParameter("@u",
            Session["UserId"])
    };

            DatabaseHelper.ExecuteNonQuery(
                logQuery,
                logParams);

            Response.Redirect("Result.aspx");
        }
    }
}
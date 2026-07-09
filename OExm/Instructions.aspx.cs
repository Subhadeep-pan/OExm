using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace OExm
{
    public partial class Instructions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DisableCache();

            if (Session["Role"]?.ToString() != "Student")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadExams();
            }
        }

        private void DisableCache()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        }

        // =========================================================
        // STEP 1: WHICH EXAMS CAN THIS STUDENT SEE?
        // =========================================================

        private void LoadExams()
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            // Only show exams that are Published, currently active, within
            // their date window, AND not already completed by this student.
            string query = @"
                SELECT ExamId, ExamName
                FROM Exams
                WHERE ExamStatus='Published'
                AND IsActive=1
                AND GETDATE() BETWEEN StartDate AND EndDate
                AND ExamId NOT IN (
                    SELECT ExamId FROM StudentExams
                    WHERE UserId=@u AND Status='Completed'
                )
                ORDER BY ExamName";

            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[]
            {
                new SqlParameter("@u", userId)
            });

            ddlExams.DataSource = dt;
            ddlExams.DataTextField = "ExamName";
            ddlExams.DataValueField = "ExamId";
            ddlExams.DataBind();

            if (dt.Rows.Count == 0)
            {
                ClearExamDetails();
                lblExamName.Text = "No exams available";
                lblDescription.Text = "There's nothing published for you right now, or you've already completed every exam available to you.";
                btnStart.Enabled = false;
                return;
            }

            btnStart.Enabled = true;
            LoadExamDetails();
        }

        // =========================================================
        // STEP 2: SHOW EVERYTHING ABOUT THE SELECTED EXAM
        // =========================================================

        protected void ddlExams_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadExamDetails();
        }

        private void LoadExamDetails()
        {
            if (ddlExams.SelectedValue == "") return;

            string query = @"
                SELECT e.ExamName, e.Description, e.DurationMinutes, e.QuestionCount,
                       e.PassingMarks, e.PositiveMarksPerQuestion, e.NegativeMarksPerQuestion,
                       e.QuestionMode, e.StartDate, e.EndDate, qb.BankName
                FROM Exams e
                LEFT JOIN QuestionBanks qb ON e.QuestionBankId = qb.BankId
                WHERE e.ExamId=@id";

            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[]
            {
                new SqlParameter("@id", ddlExams.SelectedValue)
            });

            if (dt.Rows.Count == 0)
            {
                ClearExamDetails();
                return;
            }

            DataRow exam = dt.Rows[0];

            int questionCount = exam["QuestionCount"] == DBNull.Value ? 0 : Convert.ToInt32(exam["QuestionCount"]);
            decimal positiveMarks = exam["PositiveMarksPerQuestion"] == DBNull.Value ? 0 : Convert.ToDecimal(exam["PositiveMarksPerQuestion"]);
            decimal negativeMarks = exam["NegativeMarksPerQuestion"] == DBNull.Value ? 0 : Convert.ToDecimal(exam["NegativeMarksPerQuestion"]);

            lblExamName.Text = exam["ExamName"].ToString();
            lblDescription.Text = exam["Description"].ToString();
            lblDuration.Text = exam["DurationMinutes"].ToString();
            lblQuestionCount.Text = questionCount.ToString();
            lblPassingMarks.Text = exam["PassingMarks"] == DBNull.Value ? "-" : exam["PassingMarks"].ToString();
            lblNegativeMarks.Text = negativeMarks + " per wrong answer";
            lblMode.Text = exam["QuestionMode"].ToString();
            lblTotalMarks.Text = (questionCount * positiveMarks).ToString();
            lblBankName.Text = exam["BankName"] == DBNull.Value ? "-" : exam["BankName"].ToString();

            lblAvailableFrom.Text = exam["StartDate"] == DBNull.Value ? "-" : Convert.ToDateTime(exam["StartDate"]).ToString("dd MMM yyyy, hh:mm tt");
            lblAvailableUntil.Text = exam["EndDate"] == DBNull.Value ? "-" : Convert.ToDateTime(exam["EndDate"]).ToString("dd MMM yyyy, hh:mm tt");
        }

        private void ClearExamDetails()
        {
            lblExamName.Text = "-";
            lblDescription.Text = "-";
            lblDuration.Text = "0";
            lblQuestionCount.Text = "0";
            lblPassingMarks.Text = "-";
            lblNegativeMarks.Text = "-";
            lblMode.Text = "-";
            lblTotalMarks.Text = "-";
            lblBankName.Text = "-";
            lblAvailableFrom.Text = "-";
            lblAvailableUntil.Text = "-";
        }

        // =========================================================
        // START BUTTON
        // =========================================================

        protected void btnStart_Click(object sender, EventArgs e)
        {
            if (ddlExams.SelectedValue == "")
            {
                ShowMessage("Please select an exam.", true);
                return;
            }

            int examId = Convert.ToInt32(ddlExams.SelectedValue);
            int userId = Convert.ToInt32(Session["UserId"]);

            if (AlreadyCompleted(userId, examId))
            {
                ShowMessage("You have already completed this exam.", true);
                return;
            }

            // If there's already an in-progress attempt, resume it instead of starting a new one.
            object runningExam = DatabaseHelper.ExecuteScalar(
                @"SELECT TOP 1 StudentExamId FROM StudentExams
                  WHERE UserId=@u AND ExamId=@e AND Status='InProgress'",
                new SqlParameter[]
                {
                    new SqlParameter("@u", userId),
                    new SqlParameter("@e", examId)
                });

            if (runningExam != null)
            {
                Session["StudentExamId"] = runningExam;
                Response.Redirect("ExamPortal.aspx?ExamId=" + examId);
                return;
            }

            object newExam = DatabaseHelper.ExecuteScalar(
                @"INSERT INTO StudentExams (UserId, ExamId, StartTime, Status)
                  VALUES (@u, @e, GETDATE(), 'InProgress');
                  SELECT SCOPE_IDENTITY();",
                new SqlParameter[]
                {
                    new SqlParameter("@u", userId),
                    new SqlParameter("@e", examId)
                });

            Session["StudentExamId"] = newExam;
            LogActivity(userId, "Exam Started");

            Response.Redirect("ExamPortal.aspx?ExamId=" + examId);
        }

        private bool AlreadyCompleted(int userId, int examId)
        {
            object result = DatabaseHelper.ExecuteScalar(
                @"SELECT COUNT(*) FROM StudentExams
                  WHERE UserId=@u AND ExamId=@e AND Status='Completed'",
                new SqlParameter[]
                {
                    new SqlParameter("@u", userId),
                    new SqlParameter("@e", examId)
                });

            return Convert.ToInt32(result) > 0;
        }

        private void LogActivity(int userId, string action)
        {
            DatabaseHelper.ExecuteNonQuery(
                "INSERT INTO ActivityLogs (UserId, ActionName) VALUES (@u, @a)",
                new SqlParameter[]
                {
                    new SqlParameter("@u", userId),
                    new SqlParameter("@a", action)
                });
        }

        private void ShowMessage(string message, bool isError)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;
            lblMessage.CssClass = isError ? "error-message" : "success-message";
        }
    }
}

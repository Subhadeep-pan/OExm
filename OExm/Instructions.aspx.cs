using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace OExm
{
    public partial class Instructions : System.Web.UI.Page
    {
        // ─────────────────────────────────────────────
        //  PAGE LOAD
        // ─────────────────────────────────────────────
        protected void Page_Load(object sender, EventArgs e)
        {
            // Prevent browser from caching this page
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

            // ── Auth guard ───────────────────────────
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // ── Student-only page ────────────────────
            if (Session["Role"] == null || Session["Role"].ToString() != "Student")
            {
                Response.Redirect("ManageExams.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadExams();
            }
        }

        // ─────────────────────────────────────────────
        //  LOAD EXAM DROPDOWN
        // ─────────────────────────────────────────────
        private void LoadExams()
        {
            // Select only the columns needed — avoid SELECT *
            string query = @"

SELECT
    E.ExamId,
    E.ExamName

FROM Exams E

WHERE
    E.ExamStatus = 'Published'
    AND E.IsActive = 1

    AND GETDATE() >= E.StartDate
    AND GETDATE() <= E.EndDate

    AND E.ExamId NOT IN
    (
        SELECT ExamId
        FROM StudentExams
        WHERE UserId = @u
        AND Status = 'Completed'
    )

ORDER BY E.ExamName

";

            DataTable dt =
    DatabaseHelper.ExecuteQuery(
        query,
        new SqlParameter[]
        {
            new SqlParameter(
                "@u",
                Session["UserId"])
        });

            ddlExams.DataSource = dt;
            ddlExams.DataTextField = "ExamName";
            ddlExams.DataValueField = "ExamId";
            ddlExams.DataBind();

            if (ddlExams.Items.Count == 0)
            {
                lblDuration.Text = "0";
                lblDescription.Text = "No active exams are currently available.";
                btnStart.Enabled = false;
                return;
            }

            LoadExamDetails();
        }

        // ─────────────────────────────────────────────
        //  LOAD EXAM DETAILS FOR SELECTED EXAM
        // ─────────────────────────────────────────────
        private void LoadExamDetails()
        {
            // Guard: ensure dropdown has a valid integer value
            int examId;
            if (!int.TryParse(ddlExams.SelectedValue, out examId) || examId <= 0)
                return;

            // Select only needed columns — avoid SELECT *
            string query =
                @"SELECT DurationMinutes, Description
                  FROM Exams
                  WHERE ExamId     = @id
                    AND ExamStatus = 'Published'
                    AND IsActive   = 1";

            SqlParameter[] parameters = { new SqlParameter("@id", examId) };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                lblDuration.Text = dt.Rows[0]["DurationMinutes"].ToString();
                lblDescription.Text = dt.Rows[0]["Description"].ToString();
            }
        }

        // ─────────────────────────────────────────────
        //  DROPDOWN CHANGE
        // ─────────────────────────────────────────────
        protected void ddlExams_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadExamDetails();
        }

        // ─────────────────────────────────────────────
        //  START EXAM
        // ─────────────────────────────────────────────
        protected void btnStart_Click(object sender, EventArgs e)
        {
            // Validate dropdown selection
            int examId;
            if (!int.TryParse(ddlExams.SelectedValue, out examId) || examId <= 0)
            {
                ShowMessage("Please select a valid examination.", isError: true);
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            // ── Verify exam is still Published & Active at start time ──
            //    Prevents starting an exam that was unpublished after the page loaded
            string examValidQuery =
                @"SELECT COUNT(*)
                  FROM Exams
                  WHERE ExamId     = @e
                    AND ExamStatus = 'Published'
                    AND IsActive   = 1";

            object examValid = DatabaseHelper.ExecuteScalar(
                examValidQuery,
                new SqlParameter[] { new SqlParameter("@e", examId) });

            if (Convert.ToInt32(examValid) == 0)
            {
                ShowMessage("This examination is no longer available.", isError: true);
                return;
            }

            // ── Check if student already completed this exam ──────────
            string checkCompletedQuery =
                @"SELECT TOP 1 StudentExamId
                  FROM StudentExams
                  WHERE UserId = @u
                    AND ExamId = @e
                    AND Status = 'Completed'";

            SqlParameter[] checkParams =
            {
                new SqlParameter("@u", userId),
                new SqlParameter("@e", examId)
            };

            object completedExam = DatabaseHelper.ExecuteScalar(checkCompletedQuery, checkParams);
            if (completedExam != null)
            {
                ShowMessage("You have already attempted this exam. Please check your results.", isError: true);
                return;
            }

            // ── For fixed-question exams, verify questions are mapped ─
            string modeQuery =
                "SELECT QuestionMode FROM Exams WHERE ExamId = @e";

            object mode = DatabaseHelper.ExecuteScalar(
                modeQuery,
                new SqlParameter[] { new SqlParameter("@e", examId) });

            if (mode != null && mode.ToString() != "Random")
            {
                string mapQuery =
                    "SELECT COUNT(*) FROM ExamQuestions WHERE ExamId = @e";

                object mapped = DatabaseHelper.ExecuteScalar(
                    mapQuery,
                    new SqlParameter[] { new SqlParameter("@e", examId) });

                if (Convert.ToInt32(mapped) == 0)
                {
                    ShowMessage("This exam has no questions mapped yet. Please contact your administrator.", isError: true);
                    return;
                }
            }

            // ── Resume an in-progress attempt if one exists ───────────
            string runningQuery =
                @"SELECT TOP 1 StudentExamId
                  FROM StudentExams
                  WHERE UserId = @u
                    AND ExamId = @e
                    AND Status = 'InProgress'";

            SqlParameter[] runningParams =
            {
                new SqlParameter("@u", userId),
                new SqlParameter("@e", examId)
            };

            object runningExam = DatabaseHelper.ExecuteScalar(runningQuery, runningParams);
            if (runningExam != null)
            {
                // Resume: set session and redirect (clear stale email flag too)
                Session["StudentExamId"] = runningExam;
                Session.Remove("ResultEmailSent");
                Response.Redirect("ExamPortal.aspx?ExamId=" + examId);
                return;
            }

            // ── Create a new exam attempt ─────────────────────────────
            string insertQuery =
                @"INSERT INTO StudentExams (UserId, ExamId, StartTime, Status)
                  VALUES (@u, @e, GETDATE(), 'InProgress');
                  SELECT SCOPE_IDENTITY();";

            SqlParameter[] insertParams =
            {
                new SqlParameter("@u", userId),
                new SqlParameter("@e", examId)
            };

            object result = DatabaseHelper.ExecuteScalar(insertQuery, insertParams);

            if (result == null)
            {
                ShowMessage("Failed to start the exam. Please try again.", isError: true);
                return;
            }

            Session["StudentExamId"] = result;

            // ── Activity log (includes ExamId for audit trail) ────────
            string logQuery =
                @"INSERT INTO ActivityLogs (UserId, ActionName)
                  VALUES (@u, @action)";

            DatabaseHelper.ExecuteNonQuery(
                logQuery,
                new SqlParameter[]
                {
                    new SqlParameter("@u",      userId),
                    new SqlParameter("@action", "Exam Started: ExamId=" + examId)
                });

            // Clear stale result email flag for this new attempt
            Session.Remove("ResultEmailSent");

            Response.Redirect("ExamPortal.aspx?ExamId=" + examId);
        }

        // ─────────────────────────────────────────────
        //  HELPER: SHOW INLINE MESSAGE
        // ─────────────────────────────────────────────
        private void ShowMessage(string message, bool isError)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;

            // Style differs for error vs info
            lblMessage.Style["background-color"] = isError
                ? "rgba(239,68,68,0.08)"
                : "rgba(34,197,94,0.08)";

            lblMessage.Style["border"] = isError
                ? "1px solid var(--danger)"
                : "1px solid var(--success)";

            lblMessage.Style["color"] = isError
                ? "var(--danger)"
                : "var(--success)";
        }
    }
}

using System;
using System.Data;
using System.Web;

namespace OExm
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DisableCache();

            if (!IsAdmin())
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        private void LoadPageData()
        {
            LoadDashboardStats();
            LoadActivityLogs();
            LoadTopPerformers();
            LoadViolationStats();
            LoadViolations();
        }

        private void DisableCache()
        {
            Response.Cache.SetCacheability(
                HttpCacheability.NoCache);

            Response.Cache.SetNoStore();

            Response.Cache.SetExpires(
                DateTime.Now.AddDays(-1));

            Response.Cache.SetRevalidation(
                HttpCacheRevalidation.AllCaches);
        }

        private bool IsAdmin()
        {
            return Session["UserId"] != null &&
                   Session["Role"] != null &&
                   Session["Role"].ToString() == "Admin";
        }

        private void LoadDashboardStats()
        {
            try
            {
                lblStudents.Text =
                    GetCount(
                        "SELECT COUNT(*) FROM Users WHERE Role='Student'");

                lblExams.Text =
                    GetCount(
                        "SELECT COUNT(*) FROM Exams");

                lblAttempts.Text =
                    GetCount(
                        "SELECT COUNT(*) FROM StudentExams");

                lblPublishedExams.Text =
                    GetCount(
                        "SELECT COUNT(*) FROM Exams WHERE IsActive = 1");

                LoadPassRate();
                LoadAverageScore();
            }
            catch
            {
                SetDefaultStatistics();
            }
        }

        private void LoadPassRate()
        {
            object passed =
                DatabaseHelper.ExecuteScalar(@"
                    SELECT COUNT(*)
                    FROM StudentExams SE
                    INNER JOIN Exams E
                        ON SE.ExamId = E.ExamId
                    WHERE SE.Status='Completed'
                    AND SE.Score >= E.PassingMarks");

            object total =
                DatabaseHelper.ExecuteScalar(@"
                    SELECT COUNT(*)
                    FROM StudentExams
                    WHERE Status='Completed'");

            double passRate = 0;

            if (Convert.ToInt32(total) > 0)
            {
                passRate =
                    (Convert.ToDouble(passed)
                    /
                    Convert.ToDouble(total))
                    * 100;
            }

            lblPassRate.Text =
                passRate.ToString("0.0") + "%";
        }

        private void LoadAverageScore()
        {
            object avgScore =
                DatabaseHelper.ExecuteScalar(@"
                    SELECT ISNULL(AVG(Score),0)
                    FROM StudentExams
                    WHERE Status='Completed'");

            lblAverageScore.Text =
                Convert.ToDecimal(avgScore)
                .ToString("0.00");
        }

        private void SetDefaultStatistics()
        {
            lblStudents.Text = "0";
            lblExams.Text = "0";
            lblAttempts.Text = "0";
            lblPublishedExams.Text = "0";
            lblPassRate.Text = "0%";
            lblAverageScore.Text = "0";
        }


        private void LoadActivityLogs()
        {
            try
            {
                string query = @"
                    SELECT TOP 10
                    U.FullName AS [User],
                    A.ActionName AS [Activity],
                    A.CreatedDate AS [Date Time]
                    FROM ActivityLogs A
                    INNER JOIN Users U
                        ON A.UserId = U.UserId
                    ORDER BY A.CreatedDate DESC";

                gvLogs.DataSource =
                    DatabaseHelper.ExecuteQuery(query);

                gvLogs.DataBind();
            }
            catch
            {
            }
        }

        private void LoadTopPerformers()
        {
            try
            {
                string query = @"
                    SELECT TOP 5
                    U.FullName AS [Student],
                    AVG(SE.Score) AS [Average Score]
                    FROM StudentExams SE
                    INNER JOIN Users U
                        ON SE.UserId = U.UserId
                    WHERE SE.Status='Completed'
                    GROUP BY U.FullName
                    ORDER BY AVG(SE.Score) DESC";

                gvTopPerformers.DataSource =
                    DatabaseHelper.ExecuteQuery(query);

                gvTopPerformers.DataBind();
            }
            catch
            {
            }
        }

        private string GetCount(string query)
        {
            return DatabaseHelper
                .ExecuteScalar(query)
                .ToString();
        }

        // =========================================================
        // EXAM VIOLATIONS (merged in from the old AdminViolations.aspx page)
        // =========================================================

        private void LoadViolationStats()
        {
            try
            {
                lblTotalViolations.Text = GetCount("SELECT COUNT(*) FROM ExamViolations");

                lblStudentsFlagged.Text = GetCount(@"
                    SELECT COUNT(DISTINCT SE.UserId)
                    FROM ExamViolations V
                    INNER JOIN StudentExams SE ON V.StudentExamId = SE.StudentExamId");
            }
            catch
            {
                lblTotalViolations.Text = "0";
                lblStudentsFlagged.Text = "0";
            }
        }

        private void LoadViolations()
        {
            try
            {
                string query = @"
                    SELECT
                        U.FullName AS [Student Name],
                        E.ExamName AS [Exam],
                        V.ViolationType AS [Violation],
                        CONVERT(VARCHAR, V.ViolationTime, 113) AS [Violation Time]
                    FROM ExamViolations V
                    INNER JOIN StudentExams SE ON V.StudentExamId = SE.StudentExamId
                    INNER JOIN Users U ON SE.UserId = U.UserId
                    INNER JOIN Exams E ON SE.ExamId = E.ExamId
                    ORDER BY V.ViolationTime DESC";

                gvViolations.DataSource = DatabaseHelper.ExecuteQuery(query);
                gvViolations.DataBind();
            }
            catch
            {
            }
        }
    }
}
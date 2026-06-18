using System;
using System.Data;
using System.Web;

namespace OExm
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // CACHE PROTECTION

            Response.Cache.SetCacheability(
                HttpCacheability.NoCache);

            Response.Cache.SetNoStore();

            Response.Cache.SetExpires(
                DateTime.Now.AddDays(-1));

            Response.Cache.SetRevalidation(
                HttpCacheRevalidation.AllCaches);

            // LOGIN CHECK

            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // ADMIN CHECK

            if (Session["Role"] == null ||
                Session["Role"].ToString() != "Admin")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadDashboard();
                LoadLogs();
                LoadViolations();
                LoadTopPerformers();
            }
        }

        private void LoadDashboard()
        {
            try
            {
                lblStudents.Text =
                    DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Users WHERE Role='Student'")
                    .ToString();

                lblExams.Text =
                    DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Exams")
                    .ToString();

                lblQuestions.Text =
                    DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Questions")
                    .ToString();

                lblAttempts.Text =
                    DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM StudentExams")
                    .ToString();

                // PUBLISHED EXAMS

                lblPublishedExams.Text =
                    DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Exams WHERE ExamStatus='Published'")
                    .ToString();

                object passCount =
    DatabaseHelper.ExecuteScalar(@"
    SELECT COUNT(*)
    FROM StudentExams
    WHERE Status='Completed'
    AND Score >= 4");

                object totalCompleted =
                    DatabaseHelper.ExecuteScalar(@"
    SELECT COUNT(*)
    FROM StudentExams
    WHERE Status='Completed'");

                double passRate = 0;

                if (Convert.ToInt32(totalCompleted) > 0)
                {
                    passRate =
                        (Convert.ToDouble(passCount)
                        /
                        Convert.ToDouble(totalCompleted))
                        * 100;
                }

                lblPassRate.Text =
                    passRate.ToString("0.0") + "%";

                object avgScore =
    DatabaseHelper.ExecuteScalar(@"
    SELECT ISNULL(AVG(Score),0)
    FROM StudentExams
    WHERE Status='Completed'");

                lblAverageScore.Text =
                    Convert.ToDecimal(avgScore)
                    .ToString("0.00");
            }
            catch
            {
                lblStudents.Text = "0";
                lblExams.Text = "0";
                lblQuestions.Text = "0";
                lblAttempts.Text = "0";
            }
        }

        private void LoadViolations()
        {
            try
            {
                object count =
                    DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM ExamViolations");

                lblViolations.Text =
                    count.ToString();

                string query = @"

                SELECT TOP 10

                U.FullName AS [Student],

                E.ExamName AS [Exam],

                V.ViolationType AS [Violation],

                CONVERT
                (
                    VARCHAR,
                    V.ViolationTime,
                    113
                ) AS [Violation Time]

                FROM ExamViolations V

                INNER JOIN StudentExams SE
                    ON V.StudentExamId =
                       SE.StudentExamId

                INNER JOIN Users U
                    ON SE.UserId =
                       U.UserId

                INNER JOIN Exams E
                    ON SE.ExamId =
                       E.ExamId

                ORDER BY
                    V.ViolationTime DESC

                ";

                gvViolations.DataSource =
                    DatabaseHelper.ExecuteQuery(query);

                gvViolations.DataBind();
            }
            catch
            {
                lblViolations.Text = "0";
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

        ORDER BY AVG(SE.Score) DESC

        ";

                gvTopPerformers.DataSource =
                    DatabaseHelper.ExecuteQuery(query);

                gvTopPerformers.DataBind();
            }
            catch
            {
            }
        }

        private void LoadLogs()
        {
            try
            {
                string query = @"

                SELECT TOP 10

                U.FullName AS [User],

                A.ActionName AS [Activity],

                A.LogTime AS [Date Time]

                FROM ActivityLogs A

                INNER JOIN Users U
                    ON A.UserId =
                       U.UserId

                ORDER BY
                    A.LogTime DESC

                ";

                DataTable dt =
                    DatabaseHelper.ExecuteQuery(query);

                gvLogs.DataSource = dt;
                gvLogs.DataBind();
            }
            catch
            {
                // Ignore errors
            }
        }
    }
}
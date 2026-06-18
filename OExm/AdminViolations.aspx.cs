using System;
using System.Data;
using System.Web;

namespace OExm
{
    public partial class AdminViolations : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // SECURITY

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
                LoadStatistics();
                LoadViolations();
            }
        }

        private void LoadStatistics()
        {
            try
            {
                lblTotalViolations.Text =
                    DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM ExamViolations")
                    .ToString();

                lblStudents.Text =
                    DatabaseHelper.ExecuteScalar(
                    @"SELECT COUNT(DISTINCT SE.UserId)
                      FROM ExamViolations V
                      INNER JOIN StudentExams SE
                      ON V.StudentExamId = SE.StudentExamId")
                    .ToString();

                lblToday.Text =
                    DatabaseHelper.ExecuteScalar(
                    @"SELECT COUNT(*)
                      FROM ExamViolations
                      WHERE CAST(ViolationTime AS DATE)
                      =
                      CAST(GETDATE() AS DATE)")
                    .ToString();
            }
            catch
            {
                lblTotalViolations.Text = "0";
                lblStudents.Text = "0";
                lblToday.Text = "0";
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

                DataTable dt =
                    DatabaseHelper.ExecuteQuery(query);

                gvViolations.DataSource = dt;
                gvViolations.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write(
                    "<script>alert('"
                    + ex.Message.Replace("'", "")
                    + "');</script>");
            }
        }
    }
}
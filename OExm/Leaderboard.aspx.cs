using System;
using System.Data;

namespace OExm
{
    public partial class Leaderboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadStatistics();
                LoadLeaderboard();
            }
        }

        private void LoadStatistics()
        {
            object students =
                DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM Users WHERE Role='Student'");

            lblStudents.Text =
                students.ToString();

            object attempts =
                DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM StudentExams WHERE Status='Completed'");

            lblAttempts.Text =
                attempts.ToString();

            object topScore =
                DatabaseHelper.ExecuteScalar(
                "SELECT ISNULL(MAX(Score),0) FROM StudentExams");

            lblTopScore.Text =
                topScore.ToString();
        }

        private void LoadLeaderboard()
        {
            string query = @"

            SELECT

            ROW_NUMBER() OVER
            (
                ORDER BY MAX(se.Score) DESC
            ) AS Rank,

            u.FullName,

            MAX(se.Score) AS Score,

            COUNT(*) AS ExamCount

            FROM StudentExams se

            INNER JOIN Users u
                ON se.UserId = u.UserId

            WHERE se.Status='Completed'

            GROUP BY
                u.FullName

            ORDER BY
                Score DESC

            ";

            DataTable dt =
                DatabaseHelper.ExecuteQuery(query);

            gvLeaderboard.DataSource = dt;
            gvLeaderboard.DataBind();
        }
    }
}
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace OExm
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DisableCache();

            if (!IsPostBack)
            {
                HandleLogout();
                CheckExistingSession();
            }
        }

        #region Login

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            pnlError.Visible = false;
            lblError.Text = "";

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // VALIDATION

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter Username and Password.");
                return;
            }

            // NORMAL LOGIN

            if (TryLogin(username, password))
            {
                return;
            }

            // LEGACY MD5 LOGIN

            if (TryLegacyLogin(username, password))
            {
                return;
            }

            ShowError("Invalid Username or Password.");
        }

        #endregion

        #region Authentication

        private bool TryLogin(string username, string password)
        {
            string query = @"
                SELECT UserId,
                       FullName,
                       Role
                FROM Users
                WHERE Username = @u
                  AND PasswordHash = @p
                  AND IsActive = 1";

            SqlParameter[] parameters =
            {
                new SqlParameter("@u", username),
                new SqlParameter("@p", password)
            };

            DataTable dt =
                DatabaseHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count == 0)
                return false;

            CreateUserSession(dt.Rows[0]);

            return true;
        }

        private bool TryLegacyLogin(string username, string password)
        {
            string query = @"
                SELECT UserId,
                       FullName,
                       Role
                FROM Users
                WHERE Username = @u
                  AND PasswordHash = @p
                  AND IsActive = 1";

            SqlParameter[] parameters =
            {
                new SqlParameter("@u", username),
                new SqlParameter("@p", password)
               
            };

            DataTable dt =
                DatabaseHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count == 0)
                return false;

            int userId =
                Convert.ToInt32(dt.Rows[0]["UserId"]);

            string updateQuery =
                "UPDATE Users SET PasswordHash=@hash WHERE UserId=@id";

            SqlParameter[] updateParams =
            {
                new SqlParameter("@hash",
                    password),

                new SqlParameter("@id", userId)
            };

            DatabaseHelper.ExecuteNonQuery(
                updateQuery,
                updateParams);

            CreateUserSession(dt.Rows[0]);

            return true;
        }

        #endregion

        #region Session

        private void CreateUserSession(DataRow row)
        {
            Session["UserId"] = row["UserId"];
            Session["FullName"] = row["FullName"];
            Session["Role"] = row["Role"];

            UpdateVisitorCount();

            LogActivity(
                Convert.ToInt32(row["UserId"]),
                "User Logged In");

            RedirectUser();
        }

        private void CheckExistingSession()
        {
            if (Session["UserId"] == null)
                return;

            RedirectUser();
        }

        private void RedirectUser()
        {
            string role =
                Session["Role"].ToString();

            if (role == "Admin")
            {
                Response.Redirect("AdminDashboard.aspx");
            }
            else
            {
                Response.Redirect("Instructions.aspx");
            }
        }

        #endregion

        #region Logout

        private void HandleLogout()
        {
            if (Request.QueryString["logout"] == null)
                return;

            if (Session["UserId"] != null)
            {
                LogActivity(
                    Convert.ToInt32(Session["UserId"]),
                    "User Logged Out");
            }

            Session.Clear();
            Session.Abandon();

            Response.Redirect("Login.aspx");
        }

        #endregion

        #region Activity Log

        private void LogActivity(
            int userId,
            string action)
        {
            string query =
                @"INSERT INTO ActivityLogs
                  (
                      UserId,
                      ActionName
                  )
                  VALUES
                  (
                      @u,
                      @a
                  )";

            SqlParameter[] parameters =
            {
                new SqlParameter("@u", userId),
                new SqlParameter("@a", action)
            };

            DatabaseHelper.ExecuteNonQuery(
                query,
                parameters);
        }

        #endregion

        #region Helpers

        private void DisableCache()
        {
            Response.Cache.SetCacheability(
                HttpCacheability.NoCache);

            Response.Cache.SetNoStore();

            Response.Cache.SetExpires(
                DateTime.UtcNow.AddMinutes(-1));

            Response.Cache.SetRevalidation(
                HttpCacheRevalidation.AllCaches);
        }

        private void UpdateVisitorCount()
        {
            Application.Lock();

            if (Application["TotalVisitors"] == null)
            {
                Application["TotalVisitors"] = 0;
            }

            Application["TotalVisitors"] =
                Convert.ToInt32(
                    Application["TotalVisitors"]) + 1;

            Application.UnLock();
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            lblError.Text = message;
        }

        #endregion
    }
}
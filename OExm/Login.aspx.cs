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
            // PREVENT BACK BUTTON CACHE
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            Response.AppendHeader("Pragma", "no-cache");
            Response.AppendHeader("Cache-Control", "no-cache");
            Response.AppendHeader("Cache-Control", "no-store");
            Response.AppendHeader("Expires", "-1");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

            // LOGOUT

            if (Request.QueryString["logout"] != null)
            {
                if (Session["UserId"] != null)
                {
                    string logQuery =
                    @"INSERT INTO ActivityLogs
        (
            UserId,
            ActionName
        )
        VALUES
        (
            @u,
            'User Logged Out'
        )";

                    DatabaseHelper.ExecuteNonQuery(
                        logQuery,
                        new SqlParameter[]
                        {
                new SqlParameter(
                    "@u",
                    Session["UserId"])
                        });
                }

                Session.Clear();
                Session.RemoveAll();
                Session.Abandon();

                Response.Redirect("Login.aspx");
                return;
            }

            // ALREADY LOGGED IN

            if (Session["UserId"] != null)
            {
                if (Session["Role"].ToString() == "Admin")
                {
                    Response.Redirect("ManageExams.aspx");
                }
                else
                {
                    Response.Redirect("Instructions.aspx");
                }

                return;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            string query =
                "SELECT * FROM Users WHERE Username=@u AND PasswordHash=@p";

            SqlParameter[] parameters =
            {
                new SqlParameter("@u", username),
                new SqlParameter("@p", DatabaseHelper.HashPassword(password))
            };

            DataTable dt =
                DatabaseHelper.ExecuteQuery(query, parameters);

            // SHA256 LOGIN
            if (dt.Rows.Count > 0)
            {
                Session["UserId"] = dt.Rows[0]["UserId"];
                Session["FullName"] = dt.Rows[0]["FullName"];
                Session["Role"] = dt.Rows[0]["Role"];
                Application.Lock();

                Application["TotalVisitors"] =
                Convert.ToInt32(Application["TotalVisitors"]) + 1;

                Application.UnLock();
                // ACTIVITY LOG

                string logQuery =
                "INSERT INTO ActivityLogs(UserId,ActionName) VALUES(@u,'User Logged In')";

                DatabaseHelper.ExecuteNonQuery(
                    logQuery,
                    new SqlParameter[]
                    {
                        new SqlParameter("@u", Session["UserId"])
                    });

                // ROLE BASED REDIRECT

                if (Session["Role"].ToString() == "Admin")
                {
                    Response.Redirect("ManageExams.aspx");
                }
                else
                {
                    Response.Redirect("Instructions.aspx");
                }
            }
            else
            {
                // LEGACY MD5 LOGIN

                SqlParameter[] legacyParams =
                {
                    new SqlParameter("@u", username),
                    new SqlParameter("@p", DatabaseHelper.LegacyHashMD5(password))
                };

                DataTable dtLegacy =
                    DatabaseHelper.ExecuteQuery(query, legacyParams);

                if (dtLegacy.Rows.Count > 0)
                {
                    int userId =
                        Convert.ToInt32(dtLegacy.Rows[0]["UserId"]);

                    string updateQuery =
                    "UPDATE Users SET PasswordHash=@newHash WHERE UserId=@id";

                    SqlParameter[] updateParams =
                    {
                        new SqlParameter("@newHash",
                            DatabaseHelper.HashPassword(password)),

                        new SqlParameter("@id", userId)
                    };

                    DatabaseHelper.ExecuteNonQuery(
                        updateQuery,
                        updateParams);

                    Session["UserId"] =
                        dtLegacy.Rows[0]["UserId"];

                    Session["FullName"] =
                        dtLegacy.Rows[0]["FullName"];

                    Session["Role"] =
                        dtLegacy.Rows[0]["Role"];

                    // ACTIVITY LOG

                    string logQuery =
                    "INSERT INTO ActivityLogs(UserId,ActionName) VALUES(@u,'User Logged In')";

                    DatabaseHelper.ExecuteNonQuery(
                        logQuery,
                        new SqlParameter[]
                        {
                            new SqlParameter("@u", Session["UserId"])
                        });

                    // ROLE BASED REDIRECT

                    if (Session["Role"].ToString() == "Admin")
                    {
                        Response.Redirect("ManageExams.aspx");
                    }
                    else
                    {
                        Response.Redirect("Instructions.aspx");
                    }
                }
                else
                {
                    pnlError.Visible = true;
                    lblError.Text = "Invalid Username or Password";
                }
            }
        }
    }
}
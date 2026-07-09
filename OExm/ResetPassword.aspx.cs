using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace OExm
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // PREVENT BACK BUTTON CACHE

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

            // VALID FLOW CHECK

            if (Session["ResetEmail"] == null)
            {
                Response.Redirect("ForgotPassword.aspx");
                return;
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            string password =
                txtPassword.Text.Trim();

            string confirm =
                txtConfirmPassword.Text.Trim();

            // EMPTY CHECK

            if (string.IsNullOrWhiteSpace(password))
            {
                lblMessage.Text =
                    "Password is required.";

                return;
            }

            // PASSWORD LENGTH

            if (password.Length < 6)
            {
                lblMessage.Text =
                    "Password must be at least 6 characters.";

                return;
            }

            // MATCH CHECK

            if (password != confirm)
            {
                lblMessage.Text =
                    "Passwords do not match.";

                return;
            }

            // SESSION CHECK

            if (Session["ResetEmail"] == null)
            {
                Response.Redirect("ForgotPassword.aspx");
                return;
            }

            string email =
                Session["ResetEmail"].ToString();

            string query = @"
            UPDATE Users
            SET PasswordHash=@p
            WHERE Email=@e";

            SqlParameter[] parameters =
            {
                new SqlParameter("@p", password),

                new SqlParameter(
                    "@e",
                    email
                )
            };

            DatabaseHelper.ExecuteNonQuery(
                query,
                parameters);

            // CLEANUP

            Session.Remove("OTP");
            Session.Remove("OTPTime");
            Session.Remove("ResetEmail");

            // SUCCESS MESSAGE

            Session["SuccessMessage"] =
                "Password reset successfully. Please login.";

            Response.Redirect("Login.aspx");
        }
    }
}
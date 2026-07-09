using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace OExm
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        }

        protected void btnSendOtp_Click(object sender, EventArgs e)
        {
            string email =
                txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                lblMessage.Text =
                    "Please enter your email.";

                return;
            }

            string checkQuery =
                "SELECT COUNT(*) FROM Users WHERE Email=@e";

            object count =
                DatabaseHelper.ExecuteScalar(
                checkQuery,
                new SqlParameter[]
                {
            new SqlParameter("@e", email)
                });

            if (Convert.ToInt32(count) == 0)
            {
                lblMessage.Text =
                    "Email not registered.";

                return;
            }

            Random random =
                new Random();

            string otp =
                random.Next(100000, 999999).ToString();

            Session["OTP"] = otp;
            Session["OTPTime"] = DateTime.Now;
            Session["ResetEmail"] = email;

            EmailHelper.SendEmail(
                email,
                "Password Reset OTP",
                "<h2>Your OTP is: " + otp + "</h2>"
            );

            Response.Redirect("VerifyOtp.aspx");
        }
    }
}
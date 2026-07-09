using System;
using System.Web;
using System.Web.UI;

namespace OExm
{
    public partial class VerifyOtp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));

            if (!IsPostBack)
            {
                lblMessage.Text = "";
            }
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            // OTP EXISTS?

            if (Session["OTP"] == null)
            {
                lblMessage.Text =
                    "OTP has expired. Please request a new OTP.";

                return;
            }

            // OTP EXPIRY CHECK

            if (Session["OTPTime"] != null)
            {
                DateTime otpTime =
                    Convert.ToDateTime(Session["OTPTime"]);

                if ((DateTime.Now - otpTime).TotalMinutes > 5)
                {
                    Session.Remove("OTP");
                    Session.Remove("OTPTime");

                    lblMessage.Text =
                        "OTP expired. Please request a new OTP.";

                    return;
                }
            }

            string enteredOtp =
                txtOtp.Text.Trim();

            string sessionOtp =
                Session["OTP"].ToString();

            if (enteredOtp == sessionOtp)
            {
                // REMOVE OTP AFTER SUCCESS

                Session.Remove("OTP");
                Session.Remove("OTPTime");

                Response.Redirect(
                    "ResetPassword.aspx");
            }
            else
            {
                lblMessage.Text =
                    "Invalid OTP.";
            }
        }
    }
}
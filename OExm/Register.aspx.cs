using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace OExm
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtUsername.Attributes["value"] = "";
                txtPassword.Attributes["value"] = "";
                txtConfirmPassword.Attributes["value"] = "";
            }
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            pnlSuccess.Visible = false;
            pnlError.Visible = false;

            lblSuccess.Text = "";
            lblError.Text = "";

            try
            {
                string fullname =
                    txtFullName.Text.Trim();

                string username =
                    txtUsername.Text.Trim();

                string email =
                    txtEmail.Text.Trim();

                string password =
                    txtPassword.Text.Trim();

                string confirmPassword =
                    txtConfirmPassword.Text.Trim();

                // EMPTY CHECK

                if (
                    fullname == "" ||
                    username == "" ||
                    email == "" ||
                    password == "" ||
                    confirmPassword == ""
                   )
                {
                    pnlError.Visible = true;

                    lblError.Text =
                        "All fields are required.";

                    return;
                }

                // EMAIL CHECK

                if (!email.Contains("@"))
                {
                    pnlError.Visible = true;

                    lblError.Text =
                        "Please enter a valid email.";

                    return;
                }

                // PASSWORD LENGTH

                if (password.Length < 6)
                {
                    pnlError.Visible = true;

                    lblError.Text =
                        "Password must be at least 6 characters.";

                    return;
                }

                // PASSWORD MATCH

                if (password != confirmPassword)
                {
                    pnlError.Visible = true;

                    lblError.Text =
                        "Passwords do not match.";

                    return;
                }

                // USERNAME EXISTS

                string usernameQuery =
                    "SELECT * FROM Users WHERE Username=@u";

                SqlParameter[] usernameParams =
                {
                    new SqlParameter("@u", username)
                };

                DataTable dtUser =
                    DatabaseHelper.ExecuteQuery(
                        usernameQuery,
                        usernameParams);

                if (dtUser.Rows.Count > 0)
                {
                    pnlError.Visible = true;

                    lblError.Text =
                        "Username already exists.";

                    return;
                }

                // EMAIL EXISTS

                string emailQuery =
                    "SELECT * FROM Users WHERE Email=@e";

                SqlParameter[] emailParams =
                {
                    new SqlParameter("@e", email)
                };

                DataTable dtEmail =
                    DatabaseHelper.ExecuteQuery(
                        emailQuery,
                        emailParams);

                if (dtEmail.Rows.Count > 0)
                {
                    pnlError.Visible = true;

                    lblError.Text =
                        "Email already exists.";

                    return;
                }

                // INSERT USER

                string insertQuery = @"
                INSERT INTO Users
                (
                    Username,
                    PasswordHash,
                    FullName,
                    Email,
                    Role
                )
                VALUES
                (
                    @u,
                    @p,
                    @f,
                    @e,
                    'Student'
                )";

                SqlParameter[] insertParams =
                {
                    new SqlParameter("@u", username),

                    new SqlParameter(
                        "@p",
                        DatabaseHelper.HashPassword(password)
                    ),

                    new SqlParameter("@f", fullname),

                    new SqlParameter("@e", email)
                };

                DatabaseHelper.ExecuteNonQuery(
                    insertQuery,
                    insertParams);

                // ACTIVITY LOG

                try
                {
                    string logQuery = @"
                    INSERT INTO ActivityLogs
                    (
                        UserId,
                        ActionName
                    )
                    VALUES
                    (
                        (SELECT UserId FROM Users WHERE Username=@u),
                        'User Registered'
                    )";

                    DatabaseHelper.ExecuteNonQuery(
                        logQuery,
                        new SqlParameter[]
                        {
                            new SqlParameter("@u", username)
                        });
                }
                catch
                {
                }

                // EMAIL

                try
                {
                    EmailHelper.SendEmail(
                        email,
                        "Welcome to OExm Portal",
                        "<h2>Registration Successful</h2>" +
                        "<p>Welcome to OExm Online Examination System.</p>" +
                        "<p>Your account has been created successfully.</p>"
                    );
                }
                catch
                {
                }

                // SUCCESS

                pnlSuccess.Visible = true;

                lblSuccess.Text =
                    "Registration Successful! Redirecting to Login...";

                Session["SuccessMessage"] =
                    "Registration successful. Please login.";

                ClearForm();

                Response.AddHeader(
                    "REFRESH",
                    "2;URL=Login.aspx");
            }
            catch
            {
                pnlError.Visible = true;

                lblError.Text =
                    "Something went wrong. Please try again.";
            }
        }

        private void ClearForm()
        {
            txtFullName.Text = "";
            txtUsername.Text = "";
            txtEmail.Text = "";
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
        }
    }
}
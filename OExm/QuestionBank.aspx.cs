using System;
using System.Data;
using System.Data.SqlClient;

namespace OExm
{
    public partial class QuestionBank : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"]?.ToString() != "Admin")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadBankDropdown();
                LoadQuestions();
            }
        }

        // =========================================================
        // BANK DROPDOWN
        // =========================================================

        private void LoadBankDropdown()
        {
            DataTable dt = BankHelper.GetAllBanks();

            ddlBank.DataSource = dt;
            ddlBank.DataTextField = "BankName";
            ddlBank.DataValueField = "BankName";   // we use the name, not the id
            ddlBank.DataBind();
        }

        protected void ddlBank_Changed(object sender, EventArgs e)
        {
            gvQuestions.PageIndex = 0;
            LoadQuestions();
        }

        // =========================================================
        // CREATE NEW BANK (creates the physical SQL table)
        // =========================================================

        protected void btnCreateBank_Click(object sender, EventArgs e)
        {
            string rawName = txtNewBank.Text.Trim();
            if (rawName == "")
            {
                lblBankMsg.ForeColor = System.Drawing.Color.Red;
                lblBankMsg.Text = "Please enter a bank name.";
                return;
            }

            string tableName = BankHelper.ToTableName(rawName);
            if (tableName == "")
            {
                lblBankMsg.ForeColor = System.Drawing.Color.Red;
                lblBankMsg.Text = "That name produced an invalid table name. Use letters and spaces only.";
                return;
            }

            // Check the QuestionBanks registry (not just sys.tables)
            object exists = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM QuestionBanks WHERE BankName=@n",
                new SqlParameter[] { new SqlParameter("@n", rawName) });

            if (Convert.ToInt32(exists) > 0)
            {
                lblBankMsg.ForeColor = System.Drawing.Color.Red;
                lblBankMsg.Text = "A bank called '" + rawName + "' already exists.";
                return;
            }

            // 1. Create the physical table
            BankHelper.CreateBankTable(tableName);

            // 2. Register it in QuestionBanks so dropdowns find it
            DatabaseHelper.ExecuteNonQuery(
                "INSERT INTO QuestionBanks (BankName) VALUES (@n)",
                new SqlParameter[] { new SqlParameter("@n", rawName) });

            txtNewBank.Text = "";
            lblBankMsg.ForeColor = System.Drawing.Color.Green;
            lblBankMsg.Text = "Bank '" + rawName + "' created. Table [" + tableName + "] is ready in the database.";

            LoadBankDropdown();
            ddlBank.SelectedValue = rawName;
            LoadQuestions();
        }

        // =========================================================
        // QUESTION LIST
        // =========================================================

        private void LoadQuestions()
        {
            if (ddlBank.Items.Count == 0) return;

            string tableName = BankHelper.ToTableName(ddlBank.SelectedValue);
            if (!BankHelper.TableExists(tableName))
            {
                gvQuestions.DataSource = null;
                gvQuestions.DataBind();
                return;
            }

            string filter = txtSearch.Text.Trim();

            DataTable dt;
            if (filter == "")
            {
                dt = BankHelper.GetAllRows(tableName);
            }
            else
            {
                dt = DatabaseHelper.ExecuteQuery(
                    "SELECT * FROM [" + tableName + "] WHERE Question LIKE @f ORDER BY SlNo",
                    new SqlParameter[] { new SqlParameter("@f", "%" + filter + "%") });
            }

            gvQuestions.DataSource = dt;
            gvQuestions.DataBind();
        }

        protected void txtSearch_Changed(object sender, EventArgs e)
        {
            gvQuestions.PageIndex = 0;
            LoadQuestions();
        }

        protected void gvQuestions_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            gvQuestions.PageIndex = e.NewPageIndex;
            LoadQuestions();
        }

        // =========================================================
        // SAVE QUESTION (ADD or EDIT)
        // =========================================================

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ddlBank.Items.Count == 0)
            {
                ShowMsg("Please create a question bank first.", true);
                return;
            }

            string tableName = BankHelper.ToTableName(ddlBank.SelectedValue);

            string question = txtQuestion.Text.Trim();
            string m1 = txtM1.Text.Trim();
            string m2 = txtM2.Text.Trim();
            string m3 = txtM3.Text.Trim();
            string m4 = txtM4.Text.Trim();
            string ans = txtAns.Text.Trim();

            if (question == "" || m1 == "" || m2 == "" || m3 == "" || m4 == "" || ans == "")
            {
                ShowMsg("All fields are required.", true);
                return;
            }

            // Ans must match one of the four options exactly (same rule as Bulk Upload)
            bool ansMatches = ans.Equals(m1, StringComparison.OrdinalIgnoreCase)
                           || ans.Equals(m2, StringComparison.OrdinalIgnoreCase)
                           || ans.Equals(m3, StringComparison.OrdinalIgnoreCase)
                           || ans.Equals(m4, StringComparison.OrdinalIgnoreCase);

            if (!ansMatches)
            {
                ShowMsg("'Correct Answer' must match one of M1-M4 exactly.", true);
                return;
            }

            bool isEditing = hfEditSlNo.Value != "";

            if (isEditing)
            {
                // Update the existing row
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE [" + tableName + "] SET Question=@q,M1=@m1,M2=@m2,M3=@m3,M4=@m4,Ans=@ans WHERE SlNo=@s",
                    new SqlParameter[]
                    {
                        new SqlParameter("@q",   question),
                        new SqlParameter("@m1",  m1),
                        new SqlParameter("@m2",  m2),
                        new SqlParameter("@m3",  m3),
                        new SqlParameter("@m4",  m4),
                        new SqlParameter("@ans", ans),
                        new SqlParameter("@s",   hfEditSlNo.Value)
                    });

                ShowMsg("Question updated.", false);
            }
            else
            {
                BankHelper.InsertQuestion(tableName, question, m1, m2, m3, m4, ans);
                ShowMsg("Question saved.", false);
            }

            ClearForm();
            LoadQuestions();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        // =========================================================
        // EDIT / DELETE
        // =========================================================

        protected void gvQuestions_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            int slNo = Convert.ToInt32(gvQuestions.DataKeys[e.NewEditIndex].Value);
            string tableName = BankHelper.ToTableName(ddlBank.SelectedValue);

            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT * FROM [" + tableName + "] WHERE SlNo=@s",
                new SqlParameter[] { new SqlParameter("@s", slNo) });

            if (dt.Rows.Count > 0)
            {
                txtQuestion.Text = dt.Rows[0]["Question"].ToString();
                txtM1.Text = dt.Rows[0]["M1"].ToString();
                txtM2.Text = dt.Rows[0]["M2"].ToString();
                txtM3.Text = dt.Rows[0]["M3"].ToString();
                txtM4.Text = dt.Rows[0]["M4"].ToString();
                txtAns.Text = dt.Rows[0]["Ans"].ToString();
                hfEditSlNo.Value = slNo.ToString();
                lblFormTitle.Text = "Edit Question #" + slNo;
            }

            e.Cancel = true;
        }

        protected void gvQuestions_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int slNo = Convert.ToInt32(gvQuestions.DataKeys[e.RowIndex].Value);
            string tableName = BankHelper.ToTableName(ddlBank.SelectedValue);

            BankHelper.DeleteQuestion(tableName, slNo);
            LoadQuestions();
        }

        // =========================================================
        // HELPERS
        // =========================================================

        private void ClearForm()
        {
            txtQuestion.Text = "";
            txtM1.Text = "";
            txtM2.Text = "";
            txtM3.Text = "";
            txtM4.Text = "";
            txtAns.Text = "";
            hfEditSlNo.Value = "";
            lblFormTitle.Text = "Add a Question";
        }

        private void ShowMsg(string message, bool isError)
        {
            lblMsg.ForeColor = isError ? System.Drawing.Color.Red : System.Drawing.Color.Green;
            lblMsg.Text = message;
        }
    }
}

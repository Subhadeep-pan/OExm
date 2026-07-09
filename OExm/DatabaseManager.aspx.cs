using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace OExm
{
    public partial class DatabaseManager : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadTables();

                if (ddlTables.Items.Count > 0)
                {
                    ddlTables.SelectedIndex = 0;
                    LoadData();
                }
            }
        }

        // =========================================================
        // LOAD TABLE DROPDOWN
        // Includes all system tables + any bank tables created by admin
        // =========================================================

        private void LoadTables()
        {
            ddlTables.Items.Clear();

            // Fixed system tables
            ddlTables.Items.Add("Users");
            ddlTables.Items.Add("Exams");
            ddlTables.Items.Add("ExamQuestions");
            ddlTables.Items.Add("StudentExams");
            ddlTables.Items.Add("StudentResponses");
            ddlTables.Items.Add("ActivityLogs");
            ddlTables.Items.Add("ExamViolations");
            ddlTables.Items.Add("PasswordResetOTP");
            ddlTables.Items.Add("StudentExamAttempts");
            ddlTables.Items.Add("QuestionBanks");
            ddlTables.Items.Add("Questions");
            ddlTables.Items.Add("Sections");
            ddlTables.Items.Add("SectionAnalysis");

            // Dynamic bank tables (e.g. Java, DBMS, SQL ...)
            // These are registered in QuestionBanks and have their own physical table
            DataTable banks = BankHelper.GetAllBanks();
            foreach (DataRow row in banks.Rows)
            {
                string tableName = BankHelper.ToTableName(row["BankName"].ToString());
                if (BankHelper.TableExists(tableName))
                {
                    // Add with a [BANK] prefix so admin can tell them apart
                    ddlTables.Items.Add(new ListItem("[BANK] " + row["BankName"].ToString(), tableName));
                }
            }
        }

        // =========================================================
        // PRIMARY KEY — needed for the delete button to work
        // =========================================================

        private string GetPrimaryKey(string table)
        {
            switch (table)
            {
                case "Users": return "UserId";
                case "Exams": return "ExamId";
                case "Sections": return "SectionId";
                case "Questions": return "QuestionId";
                case "QuestionBanks": return "BankId";
                case "ExamQuestions": return "ExamQuestionId";
                case "StudentExams": return "StudentExamId";
                case "StudentResponses": return "ResponseId";
                case "ActivityLogs": return "LogId";
                case "ExamViolations": return "ViolationId";
                case "StudentExamAttempts": return "AttemptId";
                case "SectionAnalysis": return "AnalysisId";
                case "PasswordResetOTP": return "OTPId";
                default:
                    // Bank tables all use SlNo as primary key
                    return "SlNo";
            }
        }

        // =========================================================
        // LOAD DATA INTO GRID
        // =========================================================

        private void LoadData()
        {
            lblMessage.Text = "";

            string table = ddlTables.SelectedValue;
            string search = txtSearch.Text.Trim();

            string query;

            if (search != "")
            {
                // Search across all text columns using a single broad LIKE
                // This works for both system tables and bank tables
                query = BuildSearchQuery(table, search);
            }
            else
            {
                query = "SELECT * FROM [" + table + "]";
            }

            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(query);

                string pk = GetPrimaryKey(table);
                if (pk != "")
                {
                    gvData.DataKeyNames = new string[] { pk };
                }

                gvData.DataSource = dt;
                gvData.DataBind();

                lblCount.Text = "Total rows: " + dt.Rows.Count;
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Error loading table: " + ex.Message;
            }
        }

        // Builds a search query that checks the first few text-like columns.
        // Works for both system tables and bank tables.
        private string BuildSearchQuery(string table, string search)
        {
            // For bank tables: search Question column
            if (GetPrimaryKey(table) == "SlNo")
            {
                return "SELECT * FROM [" + table + "] WHERE Question LIKE '%" + search.Replace("'", "''") + "%'";
            }

            // For system tables: do a generic search on common text columns
            // We use TRY_CAST to avoid errors on non-text columns
            return "SELECT * FROM [" + table + "] WHERE CAST(" + GetPrimaryKey(table) + " AS NVARCHAR) LIKE '%" + search.Replace("'", "''") + "%'";
        }

        // =========================================================
        // SEARCH BUTTON
        // =========================================================

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvData.PageIndex = 0;
            LoadData();
        }

        // =========================================================
        // TABLE DROPDOWN CHANGED
        // =========================================================

        protected void ddlTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            gvData.PageIndex = 0;
            LoadData();
        }

        // =========================================================
        // PAGING
        // =========================================================

        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvData.PageIndex = e.NewPageIndex;
            LoadData();
        }

        // =========================================================
        // DELETE ROW
        // =========================================================

        protected void gvData_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                string table = ddlTables.SelectedValue;
                string pk = GetPrimaryKey(table);

                if (pk == "") return;

                object id = gvData.DataKeys[e.RowIndex].Value;

                DatabaseHelper.ExecuteNonQuery(
                    "DELETE FROM [" + table + "] WHERE [" + pk + "]=@id",
                    new SqlParameter[] { new SqlParameter("@id", id) });

                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "Row deleted successfully.";

                LoadData();
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Delete failed: " + ex.Message;
            }
        }
    }
}

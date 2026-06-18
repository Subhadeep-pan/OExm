using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace OExm
{
    public partial class DatabaseManager :
        System.Web.UI.Page
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (Session["Role"] == null ||
                Session["Role"].ToString() != "Admin")
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

        private void LoadTables()
        {
            ddlTables.Items.Clear();

            ddlTables.Items.Add("Users");
            ddlTables.Items.Add("Exams");
            ddlTables.Items.Add("Sections");
            ddlTables.Items.Add("Questions");
            ddlTables.Items.Add("QuestionBanks");
            ddlTables.Items.Add("ExamQuestions");
            ddlTables.Items.Add("StudentExams");
            ddlTables.Items.Add("StudentResponses");
            ddlTables.Items.Add("ActivityLogs");
            ddlTables.Items.Add("ExamViolations");
            ddlTables.Items.Add("StudentExamAttempts");
            ddlTables.Items.Add("SectionAnalysis");
        }

        private string GetPrimaryKey(string table)
        {
            switch (table)
            {
                case "Users":
                    return "UserId";

                case "Exams":
                    return "ExamId";

                case "Sections":
                    return "SectionId";

                case "Questions":
                    return "QuestionId";

                case "QuestionBanks":
                    return "BankId";

                case "ExamQuestions":
                    return "ExamQuestionId";

                case "StudentExams":
                    return "StudentExamId";

                case "StudentResponses":
                    return "ResponseId";

                case "ActivityLogs":
                    return "LogId";

                case "ExamViolations":
                    return "ViolationId";

                case "StudentExamAttempts":
                    return "AttemptId";

                case "SectionAnalysis":
                    return "AnalysisId";

                default:
                    return "";
            }
        }
        protected void btnSearch_Click(
    object sender,
    EventArgs e)
        {
            // Temporary fix
        }

        protected void gvData_PageIndexChanging(
     object sender,
     GridViewPageEventArgs e)
        {
            gvData.PageIndex = e.NewPageIndex;

            LoadData();
        }
        private void LoadData()
        {
            string query =
                "SELECT * FROM " +
                ddlTables.SelectedValue;

            DataTable dt =
                DatabaseHelper.ExecuteQuery(query);

            string pk =
                GetPrimaryKey(
                    ddlTables.SelectedValue);

            if (pk != "")
            {
                gvData.DataKeyNames =
                    new string[] { pk };
            }

            gvData.DataSource = dt;
            gvData.DataBind();
        }

        protected void ddlTables_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            LoadData();
        }

        protected void gvData_RowDeleting(
            object sender,
            GridViewDeleteEventArgs e)
        {
            try
            {
                string table =
                    ddlTables.SelectedValue;

                string pk =
                    GetPrimaryKey(table);

                if (pk == "")
                {
                    return;
                }

                object id =
                    gvData.DataKeys[e.RowIndex].Value;

                string query =
    "DELETE FROM " +
    table +
    " WHERE " +
    pk +
    "=@id";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@id", id)
                };

                DatabaseHelper.ExecuteNonQuery(
                    query,
                    parameters);

                LoadData();
            }
            catch (Exception ex)
            {
                Response.Write(
                    "<script>alert('" +
                    ex.Message.Replace("'", "") +
                    "')</script>");
            }
        }
    }
}
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace OExm
{
    public partial class ManageSections : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(
    HttpCacheability.NoCache);

            Response.Cache.SetNoStore();

            Response.Cache.SetExpires(
                DateTime.Now.AddDays(-1));

            Response.Cache.SetRevalidation(
                HttpCacheRevalidation.AllCaches);
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
                LoadExams();
                LoadSections();
            }
        }

        // LOAD EXAMS

        private void LoadExams()
        {
            string query =
    @"SELECT ExamId, ExamName
      FROM Exams
      WHERE IsActive = 1
      ORDER BY ExamName";

            DataTable dt =
                DatabaseHelper.ExecuteQuery(query);

            ddlExams.DataSource = dt;
            ddlExams.DataTextField = "ExamName";
            ddlExams.DataValueField = "ExamId";
            ddlExams.DataBind();
            ddlExams.Items.Insert(
    0,
    new ListItem(
        "-- Select Exam --",
        ""));
        }

        // LOAD SECTIONS

        private void LoadSections()
        {
            string query = @"
    SELECT s.SectionId,
           e.ExamName,
           s.SectionName,
           s.OrderIndex
    FROM Sections s
    INNER JOIN Exams e
        ON s.ExamId = e.ExamId
    WHERE s.IsActive = 1
      AND e.IsActive = 1";

            DataTable dt =
                DatabaseHelper.ExecuteQuery(query);

            gvSections.DataSource = dt;
            gvSections.DataBind();
        }

        // SAVE

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ddlExams.SelectedIndex == 0)
            {
                lblMessage.Text =
                    "Please Select Exam";

                return;
            }
            string examId = ddlExams.SelectedValue;
            string section = txtSectionName.Text.Trim();
            if (section == "")
            {
                lblMessage.Text =
                    "Please Enter Section Name";
                return;
            }

            string checkQuery =
@"SELECT COUNT(*)
  FROM Sections
  WHERE ExamId=@e
  AND SectionName=@s
  AND IsActive=1";

            object exists =
                DatabaseHelper.ExecuteScalar(
                    checkQuery,
                    new SqlParameter[]
                    {
            new SqlParameter("@e", examId),
            new SqlParameter("@s", section)
                    });

            if (Convert.ToInt32(exists) > 0 &&
                hfSectionId.Value == "")
            {
                lblMessage.Text =
                    "Section Already Exists";
                return;
            }
            int order;

            if (!int.TryParse(
                txtOrderIndex.Text,
                out order))
            {
                lblMessage.Text =
                    "Enter valid Order Index";

                return;
            }

            // INSERT

            if (hfSectionId.Value == "")
            {
                string query =
                    "INSERT INTO Sections (ExamId, SectionName, OrderIndex) VALUES (@e,@s,@o)";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@e", examId),
                    new SqlParameter("@s", section),
                    new SqlParameter("@o", order)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
            }

            // UPDATE

            else
            {
                string query =
                    "UPDATE Sections SET ExamId=@e, SectionName=@s, OrderIndex=@o WHERE SectionId=@id";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@e", examId),
                    new SqlParameter("@s", section),
                    new SqlParameter("@o", order),
                    new SqlParameter("@id", hfSectionId.Value)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
            }

            ClearForm();
            LoadSections();
            lblMessage.Text =
    "Section Saved Successfully";
        }

        // EDIT

        protected void gvSections_RowEditing(object sender,
            System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            int id =
                Convert.ToInt32(gvSections.DataKeys[e.NewEditIndex].Value);

            string query =
                "SELECT * FROM Sections WHERE SectionId=@id";

            SqlParameter[] parameters =
            {
                new SqlParameter("@id", id)
            };

            DataTable dt =
    DatabaseHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                ddlExams.SelectedValue =
                    dt.Rows[0]["ExamId"].ToString();

                txtSectionName.Text =
                    dt.Rows[0]["SectionName"].ToString();

                txtOrderIndex.Text =
                    dt.Rows[0]["OrderIndex"].ToString();

                hfSectionId.Value = id.ToString();
            }

            e.Cancel = true;
        }

        // DELETE

        protected void gvSections_RowDeleting(object sender,
            System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int id =
                Convert.ToInt32(gvSections.DataKeys[e.RowIndex].Value);

            string query =
@"UPDATE Sections
  SET IsActive = 0
  WHERE SectionId=@id";

            SqlParameter[] parameters =
            {
                new SqlParameter("@id", id)
            };

            DatabaseHelper.ExecuteNonQuery(query, parameters);

            LoadSections();
            lblMessage.Text =
    "Section Deleted Successfully";
        }

        // CLEAR

        private void ClearForm()
        {
            txtSectionName.Text = "";
            txtOrderIndex.Text = "";
            hfSectionId.Value = "";
            ddlExams.SelectedIndex = 0;
        }
    }
}
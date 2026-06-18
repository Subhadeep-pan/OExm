using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;
using static iTextSharp.text.pdf.AcroFields;

namespace OExm
{
    public partial class ManageExams : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

            Response.Headers["Cache-Control"] =
                "no-cache, no-store, must-revalidate";

            Response.Headers["Pragma"] =
                "no-cache";

            Response.Headers["Expires"] =
                "0";

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
                pnlManualQuestions.Visible = false;
                pnlQuestionCount.Visible = true;
                LoadQuestionBanks();
                LoadExams();

                pnlManualQuestions.Visible = false;
            }
        }



        protected void ddlQuestionBank_SelectedIndexChanged(
    object sender,
    EventArgs e)
        {
            if (ddlQuestionMode.SelectedValue == "Manual")
            {
                pnlManualQuestions.Visible = true;
                LoadQuestions();
            }
        }

        // LOAD EXAMS

        private void LoadExams()
        {
            string query =
         "SELECT * FROM Exams WHERE IsActive=1 ORDER BY ExamId DESC";

            DataTable dt =
                DatabaseHelper.ExecuteQuery(query);

            gvExams.DataSource = dt;
            gvExams.DataBind();
        }

        // SAVE EXAM

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtExamName.Text.Trim();


if (name == "")
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('Please Enter Exam Name');",
                    true);

                return;
            }

            string duration = txtDuration.Text.Trim();
            int mins;

            if (!int.TryParse(duration, out mins))
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('Enter valid Duration');",
                    true);

                return;
            }

            string desc = txtDescription.Text.Trim();
            bool active = chkIsActive.Checked;

            string status =
                ddlExamStatus.SelectedValue;
            DateTime startDate;
            DateTime endDate;

            if (!DateTime.TryParse(
                    txtStartDate.Text,
                    out startDate))
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('Please select Start Date');",
                    true);

                return;
            }

            if (!DateTime.TryParse(
                    txtEndDate.Text,
                    out endDate))
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('Please select End Date');",
                    true);

                return;
            }

            if (endDate <= startDate)
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('End Date must be greater than Start Date');",
                    true);

                return;
            }
            string mode =
                ddlQuestionMode.SelectedValue;

            int count = 0;

            if (mode == "Random")
            {
                if (!int.TryParse(
                        txtQuestionCount.Text,
                        out count))
                {
                    ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "msg",
                        "alert('Enter valid Question Count');",
                        true);

                    return;
                }
            }
            else
            {
                foreach (ListItem item in cblQuestions.Items)
                {
                    if (item.Selected)
                    {
                        count++;
                    }
                }

                if (count == 0)
                {
                    ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "msg",
                        "alert('Please select at least one question');",
                        true);

                    return;
                }
            }

            int bankId =
                Convert.ToInt32(
                    ddlQuestionBank.SelectedValue);

            if (bankId == 0)
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('Please select a Question Bank.');",
                    true);

                return;
            }

            string checkQuery =
          

@"SELECT COUNT(*)
FROM Exams
WHERE ExamName=@n
AND IsActive=1
AND ExamId<>@id";

object exists =
    DatabaseHelper.ExecuteScalar(
        checkQuery,
        new SqlParameter[]
        {
            new SqlParameter("@n", name),

            new SqlParameter(
                "@id",
                hfExamId.Value == ""
                ? 0
                : Convert.ToInt32(hfExamId.Value))
        });

            if (Convert.ToInt32(exists) > 0)
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('Exam Already Exists');",
                    true);

                return;
            }

            // INSERT
            if (hfExamId.Value == "")
            {
                string query =


@"INSERT INTO Exams
(
ExamName,
Description,
DurationMinutes,
IsActive,
ExamStatus,
QuestionMode,
QuestionCount,
QuestionBankId,
StartDate,
EndDate
)
VALUES
(
@n,
@d,
@m,
@a,
@status,
@mode,
@count,
@bank,
@start,
@end
)";


    SqlParameter[] parameters =
    {
        new SqlParameter("@n", name),
        new SqlParameter("@d", desc),
        new SqlParameter("@m", mins),
        new SqlParameter("@a", active),
        new SqlParameter("@status", status),
        new SqlParameter("@mode", mode),
        new SqlParameter("@count", count),
        new SqlParameter("@bank", bankId),
        new SqlParameter("@start", startDate),
        new SqlParameter("@end", endDate)
    };

                DatabaseHelper.ExecuteNonQuery(
                    query,
                    parameters);

                int examId =
                    Convert.ToInt32(
                        DatabaseHelper.ExecuteScalar(
                            "SELECT MAX(ExamId) FROM Exams"));

                if (mode == "Manual")
                {
                    SaveManualQuestions(examId);
                }
            }

            // UPDATE
            else
            {
                string query =


@"UPDATE Exams
SET
ExamName=@n,
Description=@d,
DurationMinutes=@m,
IsActive=@a,
ExamStatus=@status,
QuestionMode=@mode,
QuestionCount=@count,
QuestionBankId=@bank,
StartDate=@start,
EndDate=@end
WHERE ExamId=@id";

    SqlParameter[] parameters =
    {
        new SqlParameter("@n", name),
        new SqlParameter("@d", desc),
        new SqlParameter("@m", mins),
        new SqlParameter("@a", active),
        new SqlParameter("@status", status),
        new SqlParameter("@mode", mode),
        new SqlParameter("@count", count),
        new SqlParameter("@bank", bankId),
         new SqlParameter("@start", startDate),
        new SqlParameter("@end", endDate),
        new SqlParameter("@id", hfExamId.Value)
    };

                DatabaseHelper.ExecuteNonQuery(
                    query,
                    parameters);

                if (mode == "Manual")
                {
                    SaveManualQuestions(
                        Convert.ToInt32(
                            hfExamId.Value));
                }
            }

            ClearForm();
            LoadExams();

            ClientScript.RegisterStartupScript(
                this.GetType(),
                "msg",
                "alert('Exam Saved Successfully');",
                true);

}

        // EDIT EXAM

        protected void gvExams_RowEditing(
    object sender,
    System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            try
            {
                int id =
                    Convert.ToInt32(
                        gvExams.DataKeys[e.NewEditIndex].Value);

                string query =
                    "SELECT * FROM Exams WHERE ExamId=@id";

                SqlParameter[] parameters =
                {
            new SqlParameter("@id", id)
        };

                DataTable dt =
                    DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    txtExamName.Text =
                        dt.Rows[0]["ExamName"].ToString();

                    txtDescription.Text =
                        dt.Rows[0]["Description"].ToString();

                    txtDuration.Text =
                        dt.Rows[0]["DurationMinutes"].ToString();

                    chkIsActive.Checked =
                        Convert.ToBoolean(dt.Rows[0]["IsActive"]);

                    ddlExamStatus.SelectedValue =
                        dt.Rows[0]["ExamStatus"].ToString();

                    ddlQuestionMode.SelectedValue =
                        dt.Rows[0]["QuestionMode"].ToString();

                    txtQuestionCount.Text =
                        dt.Rows[0]["QuestionCount"].ToString();

                    ddlQuestionBank.SelectedValue =
                        dt.Rows[0]["QuestionBankId"].ToString();
                    if (dt.Rows[0]["StartDate"] != DBNull.Value)
                    {
                        txtStartDate.Text =
                            Convert.ToDateTime(
                                dt.Rows[0]["StartDate"])
                            .ToString("yyyy-MM-ddTHH:mm");
                    }

                    if (dt.Rows[0]["EndDate"] != DBNull.Value)
                    {
                        txtEndDate.Text =
                            Convert.ToDateTime(
                                dt.Rows[0]["EndDate"])
                            .ToString("yyyy-MM-ddTHH:mm");
                    }
                    hfExamId.Value = id.ToString();
                }
            }
            catch (Exception)
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('Error loading exam.');",
                    true);
            }

            e.Cancel = true;
        }

        // DELETE EXAM

        protected void gvExams_RowDeleting(
     object sender,
     System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            try
            {
                int id =
                    Convert.ToInt32(
                        gvExams.DataKeys[e.RowIndex].Value);

                string query =
                    "UPDATE Exams SET IsActive=0 WHERE ExamId=@id; " +
                    "UPDATE Sections SET IsActive=0 WHERE ExamId=@id;";

                SqlParameter[] parameters =
                {
            new SqlParameter("@id", id)
        };

                DatabaseHelper.ExecuteNonQuery(
    query,
    parameters);

                LoadExams();

                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('Exam deleted successfully.');",
                    true);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
                Response.End();
            }
        }

        private void SaveManualQuestions(int examId)
        {
            int selectedCount = 0;

            string deleteQuery =
                "DELETE FROM ExamQuestions WHERE ExamId=@ExamId";

            DatabaseHelper.ExecuteNonQuery(
                deleteQuery,
                new SqlParameter[]
                {
            new SqlParameter("@ExamId", examId)
                });

            foreach (ListItem item in cblQuestions.Items)
            {
                if (item.Selected)
                {
                    selectedCount++;

                    string insertQuery =
                    @"INSERT INTO ExamQuestions
            (
                ExamId,
                QuestionId
            )
            VALUES
            (
                @ExamId,
                @QuestionId
            )";

                    DatabaseHelper.ExecuteNonQuery(
                        insertQuery,
                        new SqlParameter[]
                        {
                    new SqlParameter("@ExamId", examId),
                    new SqlParameter("@QuestionId", item.Value)
                        });
                }
            }

            Response.Write(
                "<script>alert('Selected Questions = "
                + selectedCount +
                "');</script>");
        }
        private void LoadQuestions()
        {
            if (ddlQuestionBank.SelectedValue == "0")
                return;

            string query =
            @"SELECT QuestionId,
             QuestionText
      FROM Questions
      WHERE IsActive = 1
      AND BankId = @BankId";

            SqlParameter[] parameters =
            {
        new SqlParameter(
            "@BankId",
            ddlQuestionBank.SelectedValue)
    };

            DataTable dt =
                DatabaseHelper.ExecuteQuery(
                    query,
                    parameters);

            cblQuestions.DataSource = dt;
            cblQuestions.DataTextField = "QuestionText";
            cblQuestions.DataValueField = "QuestionId";
            cblQuestions.DataBind();
        }
        private void LoadQuestionBanks()
        {
            string query =
                "SELECT * FROM QuestionBanks";

            DataTable dt =
                DatabaseHelper.ExecuteQuery(query);

            ddlQuestionBank.DataSource = dt;
            ddlQuestionBank.DataTextField = "BankName";
            ddlQuestionBank.DataValueField = "BankId";
            ddlQuestionBank.DataBind();

            ddlQuestionBank.Items.Insert(
    0,
    new ListItem(
        "-- Select Bank --",
        "0"));
        }

        // CLEAR FORM
        protected void ddlQuestionMode_SelectedIndexChanged(
    object sender,
    EventArgs e)
        {
            bool isManual =
                ddlQuestionMode.SelectedValue == "Manual";

            pnlManualQuestions.Visible =
                isManual;

            pnlQuestionCount.Visible =
                !isManual;

            if (isManual)
            {
                LoadQuestions();
            }
        }
        private void ClearForm()
        {
            txtExamName.Text = "";
            txtDescription.Text = "";
            txtDuration.Text = "";
            chkIsActive.Checked = true;
            ddlExamStatus.SelectedIndex = 0;
            hfExamId.Value = "";
            ddlQuestionMode.SelectedIndex = 0;
            txtQuestionCount.Text = "";
            ddlQuestionBank.SelectedIndex = 0;
        }
    }
}
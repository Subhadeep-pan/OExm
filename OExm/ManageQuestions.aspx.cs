using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace OExm
{
    public partial class ManageQuestions : System.Web.UI.Page
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
                LoadBanks();
                LoadQuestions();
            }
        }

        // LOAD EXAMS
        private void LoadBanks()
        {
            string query =
                "SELECT * FROM QuestionBanks";

            DataTable dt =
                DatabaseHelper.ExecuteQuery(query);

            ddlBank.DataSource = dt;
            ddlBank.DataTextField = "BankName";
            ddlBank.DataValueField = "BankId";
            ddlBank.DataBind();
            ddlBank.Items.Insert(
    0,
    new ListItem(
        "-- Select Bank --",
        ""));

        }
        private void LoadExams()
        {
            string query =
 @"SELECT ExamId,
         ExamName
  FROM Exams
  WHERE IsActive = 1
  ORDER BY ExamName";

            DataTable dt =
                DatabaseHelper.ExecuteQuery(query);

            ddlExams.DataSource = dt;
            ddlExams.DataTextField = "ExamName";
            ddlExams.DataValueField = "ExamId";
            ddlExams.DataBind();

            LoadSections();
        }

        // LOAD SECTIONS

        private void LoadSections()
        {
            string query =
                "SELECT * FROM Sections WHERE ExamId=@id AND IsActive=1 ORDER BY OrderIndex";

            SqlParameter[] parameters =
            {
                new SqlParameter("@id", ddlExams.SelectedValue)
            };

            DataTable dt =
                DatabaseHelper.ExecuteQuery(query, parameters);

            ddlSections.DataSource = dt;
            ddlSections.DataTextField = "SectionName";
            ddlSections.DataValueField = "SectionId";
            ddlSections.DataBind();
            ddlSections.Items.Insert(
    0,
    new ListItem(
        "-- Select Section --",
        ""));
        }

        // EXAM CHANGE

        protected void ddlExams_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSections();
        }

        // LOAD QUESTIONS

        private void LoadQuestions()
        {
            string query =
        @"SELECT *
  FROM Questions
  WHERE IsActive = 1
  ORDER BY QuestionId DESC";

            DataTable dt =
                DatabaseHelper.ExecuteQuery(query);

            gvQuestions.DataSource = dt;
            gvQuestions.DataBind();
        }

        // SAVE QUESTION

        protected void btnSave_Click(object sender, EventArgs e)
        {

            decimal positive;
            decimal negative;

            if (!decimal.TryParse(
                    txtPositive.Text,
                    out positive))
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('Invalid Positive Marks');",
                    true);

                return;
            }


            if (!decimal.TryParse(
                    txtNegative.Text,
                    out negative))
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('Invalid Negative Marks');",
                    true);

                return;
            }
            if (ddlBank.SelectedValue == "")
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('Please select a Question Bank');",
                    true);
                return;
            }

            if (ddlSections.SelectedValue == "")
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('Please select a Section');",
                    true);
                return;
            }

            if (txtQuestion.Text.Trim() == "")
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('Question cannot be empty');",
                    true);
                return;
            }
            if (txtOptionA.Text.Trim() == "" ||
    txtOptionB.Text.Trim() == "" ||
    txtOptionC.Text.Trim() == "" ||
    txtOptionD.Text.Trim() == "")
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('All Options are Required');",
                    true);

                return;
            }


            if (txtOptionA.Text.Trim() ==
    txtOptionB.Text.Trim() ||

    txtOptionA.Text.Trim() ==
    txtOptionC.Text.Trim() ||

    txtOptionA.Text.Trim() ==
    txtOptionD.Text.Trim() ||

    txtOptionB.Text.Trim() ==
    txtOptionC.Text.Trim() ||

    txtOptionB.Text.Trim() ==
    txtOptionD.Text.Trim() ||

    txtOptionC.Text.Trim() ==
    txtOptionD.Text.Trim())
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "msg",
                    "alert('Options cannot be same');",
                    true);

                return;
            }
            // INSERT

            if (hfQuestionId.Value == "")
            {
                string checkQuery =
@"SELECT COUNT(*)
  FROM Questions
  WHERE QuestionText=@q
  AND IsActive=1
  AND QuestionId<>@id";

                object exists =
    DatabaseHelper.ExecuteScalar(
        checkQuery,
        new SqlParameter[]
        {
            new SqlParameter(
                "@q",
                txtQuestion.Text.Trim()),

            new SqlParameter(
                "@id",
                hfQuestionId.Value == ""
                ? 0
                : Convert.ToInt32(
                    hfQuestionId.Value))
        });

                if (Convert.ToInt32(exists) > 0)
                {
                    ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "msg",
                        "alert('Question Already Exists');",
                        true);

                    return;
                }
                string query = @"
    INSERT INTO Questions
    (
        SectionId,
        BankId,
        QuestionText,
        OptionA,
        OptionB,
        OptionC,
        OptionD,
        CorrectOption,
        PositiveMarks,
        NegativeMarks
    )
    VALUES
    (
        @s,
        @bank,
        @q,
        @a,
        @b,
        @c,
        @d,
        @co,
        @p,
        @n
    )";

                SqlParameter[] parameters =
                {
        new SqlParameter("@s", ddlSections.SelectedValue),
        new SqlParameter("@bank", ddlBank.SelectedValue),
        new SqlParameter("@q", txtQuestion.Text),
        new SqlParameter("@a", txtOptionA.Text),
        new SqlParameter("@b", txtOptionB.Text),
        new SqlParameter("@c", txtOptionC.Text),
        new SqlParameter("@d", txtOptionD.Text),
        new SqlParameter("@co", ddlCorrect.SelectedValue),
        new SqlParameter("@p", txtPositive.Text),
        new SqlParameter("@n", txtNegative.Text)
    };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
            }

            // UPDATE

            else
            {
                string query = @"
    UPDATE Questions SET
    SectionId=@s,
    BankId=@bank,
    QuestionText=@q,
    OptionA=@a,
    OptionB=@b,
    OptionC=@c,
    OptionD=@d,
    CorrectOption=@co,
    PositiveMarks=@p,
    NegativeMarks=@n
    WHERE QuestionId=@id";

                SqlParameter[] parameters =
                {
        new SqlParameter("@s", ddlSections.SelectedValue),
        new SqlParameter("@bank", ddlBank.SelectedValue),
        new SqlParameter("@q", txtQuestion.Text),
        new SqlParameter("@a", txtOptionA.Text),
        new SqlParameter("@b", txtOptionB.Text),
        new SqlParameter("@c", txtOptionC.Text),
        new SqlParameter("@d", txtOptionD.Text),
        new SqlParameter("@co", ddlCorrect.SelectedValue),
        new SqlParameter("@p", txtPositive.Text),
        new SqlParameter("@n", txtNegative.Text),
        new SqlParameter("@id", hfQuestionId.Value)
    };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
            }

            ClearForm();
            LoadQuestions();
            ClientScript.RegisterStartupScript(
    this.GetType(),
    "msg",
    "alert('Question Saved Successfully');",
    true);

        }

        // EDIT

        protected void gvQuestions_RowEditing(object sender,
            System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            int id =
                Convert.ToInt32(gvQuestions.DataKeys[e.NewEditIndex].Value);

            string query =
                "SELECT * FROM Questions WHERE QuestionId=@id";

            SqlParameter[] parameters =
            {
                new SqlParameter("@id", id)
            };

            DataTable dt =
    DatabaseHelper.ExecuteQuery(query, parameters);

            string sectionId =
                dt.Rows[0]["SectionId"].ToString();

            string examQuery =
            @"SELECT ExamId
  FROM Sections
  WHERE SectionId=@sid";

            DataTable examDt =
                DatabaseHelper.ExecuteQuery(
                    examQuery,
                    new SqlParameter[]
                    {
            new SqlParameter("@sid", sectionId)
                    });

            if (examDt.Rows.Count > 0)
            {
                ddlExams.SelectedValue =
                    examDt.Rows[0]["ExamId"].ToString();

                LoadSections();

                ddlSections.SelectedValue =
                    sectionId;
            }

            txtQuestion.Text =
                dt.Rows[0]["QuestionText"].ToString();

            txtOptionA.Text =
                dt.Rows[0]["OptionA"].ToString();

            txtOptionB.Text =
                dt.Rows[0]["OptionB"].ToString();

            txtOptionC.Text =
                dt.Rows[0]["OptionC"].ToString();

            txtOptionD.Text =
                dt.Rows[0]["OptionD"].ToString();

            ddlCorrect.SelectedValue =
                dt.Rows[0]["CorrectOption"].ToString();

            txtPositive.Text =
                dt.Rows[0]["PositiveMarks"].ToString();

            txtNegative.Text =
                dt.Rows[0]["NegativeMarks"].ToString();
            ddlBank.SelectedValue =
    dt.Rows[0]["BankId"].ToString();
            hfQuestionId.Value = id.ToString();

            e.Cancel = true;
        }

        // DELETE

        protected void gvQuestions_RowDeleting(object sender,
            System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int id =
                Convert.ToInt32(gvQuestions.DataKeys[e.RowIndex].Value);

            string query =
                "UPDATE Questions SET IsActive=0 WHERE QuestionId=@id";

            SqlParameter[] parameters =
            {
                new SqlParameter("@id", id)
            };

            DatabaseHelper.ExecuteNonQuery(query, parameters);

            LoadQuestions();
            ClientScript.RegisterStartupScript(
    this.GetType(),
    "msg",
    "alert('Question Deleted Successfully');",
    true);
        }

        // CLEAR

        private void ClearForm()
        {
            ddlExams.SelectedIndex = 0;

            LoadSections();

            ddlSections.SelectedIndex = 0;

            ddlBank.SelectedIndex = 0;

            txtQuestion.Text = "";

            txtOptionA.Text = "";

            txtOptionB.Text = "";

            txtOptionC.Text = "";

            txtOptionD.Text = "";

            txtPositive.Text = "";

            txtNegative.Text = "";

            ddlCorrect.SelectedIndex = 0;

            hfQuestionId.Value = "";
        }
    }
}
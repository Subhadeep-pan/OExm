using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace OExm
{
    public partial class ExamBuilder : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));

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
                LoadQuestionBankDropdowns();
                LoadContextExamDropdown();
                gvExams.PageIndex = 0;
                LoadExamsGrid();
                ResetToNewExam();
            }
        }

        private void Toast(string message, string type)
        {
            string js = "showToast(" + HttpUtility.JavaScriptStringEncode(message, true) + ",'" + type + "');";
            ClientScript.RegisterStartupScript(this.GetType(), "toast" + Guid.NewGuid().ToString("N"), js, true);
        }

        // =========================================================
        // CONTEXT BAR - choosing which exam you're editing
        // =========================================================

        private void LoadContextExamDropdown()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT ExamId, ExamName FROM Exams WHERE IsActive=1 ORDER BY ExamName");

            ddlContextExam.DataSource = dt;
            ddlContextExam.DataTextField = "ExamName";
            ddlContextExam.DataValueField = "ExamId";
            ddlContextExam.DataBind();
            ddlContextExam.Items.Insert(0, new ListItem("-- Create New Exam --", ""));
        }

        protected void ddlContextExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlContextExam.SelectedValue == "")
            {
                ResetToNewExam();
            }
            else
            {
                LoadExamIntoContext(Convert.ToInt32(ddlContextExam.SelectedValue));
            }
        }

        protected void btnNewExam_Click(object sender, EventArgs e)
        {
            ResetToNewExam();
            Toast("Ready to build a new exam.", "info");
        }

        private void ResetToNewExam()
        {
            hfExamId.Value = "";
            txtExamName.Text = "";
            txtDescription.Text = "";
            txtDuration.Text = "";
            txtPassingMarks.Text = "";
            txtPositivePerQuestion.Text = "";
            txtNegativePerQuestion.Text = "";
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            ddlExamStatus.SelectedIndex = 0;
            chkIsActive.Checked = true;
            ddlQuestionMode.SelectedIndex = 0;
            txtQuestionCount.Text = "";
            ddlQuestionBank.SelectedIndex = 0;
            ddlManualBank.SelectedIndex = 0;
            cblManualQuestions.Items.Clear();
            pnlQuestionCount.Visible = true;
            pnlManualQuestions.Visible = false;

            ddlContextExam.SelectedValue = "";
        }

        private void LoadExamIntoContext(int examId)
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT * FROM Exams WHERE ExamId=@id",
                new SqlParameter[] { new SqlParameter("@id", examId) });

            if (dt.Rows.Count == 0)
            {
                Toast("That exam could not be found. It may have been deleted.", "error");
                ResetToNewExam();
                return;
            }

            hfExamId.Value = examId.ToString();
            ddlContextExam.SelectedValue = examId.ToString();

            txtExamName.Text = dt.Rows[0]["ExamName"].ToString();
            txtDescription.Text = dt.Rows[0]["Description"].ToString();
            txtDuration.Text = dt.Rows[0]["DurationMinutes"].ToString();
            txtPassingMarks.Text = dt.Rows[0]["PassingMarks"] == DBNull.Value ? "" : dt.Rows[0]["PassingMarks"].ToString();
            txtPositivePerQuestion.Text = dt.Rows[0]["PositiveMarksPerQuestion"] == DBNull.Value ? "" : dt.Rows[0]["PositiveMarksPerQuestion"].ToString();
            txtNegativePerQuestion.Text = dt.Rows[0]["NegativeMarksPerQuestion"] == DBNull.Value ? "" : dt.Rows[0]["NegativeMarksPerQuestion"].ToString();
            chkIsActive.Checked = Convert.ToBoolean(dt.Rows[0]["IsActive"]);

            if (dt.Rows[0]["ExamStatus"] != DBNull.Value)
                ddlExamStatus.SelectedValue = dt.Rows[0]["ExamStatus"].ToString();

            if (dt.Rows[0]["QuestionMode"] != DBNull.Value)
                ddlQuestionMode.SelectedValue = dt.Rows[0]["QuestionMode"].ToString();

            bool isManual = ddlQuestionMode.SelectedValue == "Manual";
            pnlManualQuestions.Visible = isManual;
            pnlQuestionCount.Visible = !isManual;

            if (dt.Rows[0]["QuestionCount"] != DBNull.Value)
                txtQuestionCount.Text = dt.Rows[0]["QuestionCount"].ToString();

            if (dt.Rows[0]["QuestionBankId"] != DBNull.Value)
            {
                // Dropdown uses BankName as value, so look up the name from the stored BankId
                object bankNameObj = DatabaseHelper.ExecuteScalar(
                    "SELECT BankName FROM QuestionBanks WHERE BankId=@id",
                    new SqlParameter[] { new SqlParameter("@id", dt.Rows[0]["QuestionBankId"]) });

                if (bankNameObj != null && bankNameObj != DBNull.Value)
                {
                    string bankName = bankNameObj.ToString();
                    if (ddlQuestionBank.Items.FindByValue(bankName) != null)
                        ddlQuestionBank.SelectedValue = bankName;
                    if (ddlManualBank.Items.FindByValue(bankName) != null)
                        ddlManualBank.SelectedValue = bankName;
                }
            }

            if (dt.Rows[0]["StartDate"] != DBNull.Value)
                txtStartDate.Text = Convert.ToDateTime(dt.Rows[0]["StartDate"]).ToString("yyyy-MM-ddTHH:mm");

            if (dt.Rows[0]["EndDate"] != DBNull.Value)
                txtEndDate.Text = Convert.ToDateTime(dt.Rows[0]["EndDate"]).ToString("yyyy-MM-ddTHH:mm");

            if (isManual)
            {
                LoadManualQuestionsChecklist(examId);
            }
        }

        // =========================================================
        // RANDOM / MANUAL MODE SWITCH
        // =========================================================

        protected void ddlQuestionMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isManual = ddlQuestionMode.SelectedValue == "Manual";
            pnlManualQuestions.Visible = isManual;
            pnlQuestionCount.Visible = !isManual;

            if (isManual && ddlManualBank.SelectedValue != "")
            {
                LoadManualQuestionsChecklist(hfExamId.Value == "" ? 0 : Convert.ToInt32(hfExamId.Value));
            }
        }

        protected void ddlManualBank_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadManualQuestionsChecklist(hfExamId.Value == "" ? 0 : Convert.ToInt32(hfExamId.Value));
        }

        private void LoadQuestionBankDropdowns()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM QuestionBanks ORDER BY BankName");

            // Use BankName as the value (not BankId) so we can pass it
            // directly to BankHelper.ToTableName() when loading questions.
            ddlQuestionBank.DataSource = dt;
            ddlQuestionBank.DataTextField = "BankName";
            ddlQuestionBank.DataValueField = "BankName";
            ddlQuestionBank.DataBind();
            ddlQuestionBank.Items.Insert(0, new ListItem("-- Select Bank --", ""));

            ddlManualBank.DataSource = dt.Copy();
            ddlManualBank.DataTextField = "BankName";
            ddlManualBank.DataValueField = "BankName";
            ddlManualBank.DataBind();
            ddlManualBank.Items.Insert(0, new ListItem("-- Select Bank --", ""));
        }

        // Shows every question in the selected bank table as a checkbox.
        // Reads from [DBMS], [Java] etc. — not the old Questions table.
        private void LoadManualQuestionsChecklist(int examId)
        {
            if (ddlManualBank.SelectedValue == "")
            {
                cblManualQuestions.Items.Clear();
                return;
            }

            string tableName = BankHelper.ToTableName(ddlManualBank.SelectedValue);

            if (!BankHelper.TableExists(tableName))
            {
                cblManualQuestions.Items.Clear();
                return;
            }

            // Load all questions from the bank table (SlNo as value, Question as label)
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT SlNo, Question FROM [" + tableName + "] ORDER BY SlNo");

            cblManualQuestions.DataSource = dt;
            cblManualQuestions.DataTextField = "Question";
            cblManualQuestions.DataValueField = "SlNo";
            cblManualQuestions.DataBind();

            // If editing an existing exam, pre-tick questions already saved
            // We store the selected SlNos in ExamQuestions as QuestionId
            if (examId > 0)
            {
                DataTable selected = DatabaseHelper.ExecuteQuery(
                    "SELECT QuestionId FROM ExamQuestions WHERE ExamId=@id",
                    new SqlParameter[] { new SqlParameter("@id", examId) });

                foreach (DataRow row in selected.Rows)
                {
                    ListItem item = cblManualQuestions.Items.FindByValue(row["QuestionId"].ToString());
                    if (item != null) item.Selected = true;
                }
            }
        }

        // =========================================================
        // ALL EXAMS GRID
        // =========================================================

        private void LoadExamsGrid()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Exams WHERE IsActive=1 ORDER BY ExamId DESC");
            gvExams.DataSource = dt;
            gvExams.DataBind();
        }

        protected void gvExams_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvExams.PageIndex = e.NewPageIndex;
            LoadExamsGrid();
        }

        protected void gvExams_RowEditing(object sender, GridViewEditEventArgs e)
        {
            int id = Convert.ToInt32(gvExams.DataKeys[e.NewEditIndex].Value);
            LoadExamIntoContext(id);
            e.Cancel = true;
        }

        protected void gvExams_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvExams.DataKeys[e.RowIndex].Value);

            DatabaseHelper.ExecuteNonQuery(
                "UPDATE Exams SET IsActive=0 WHERE ExamId=@id",
                new SqlParameter[] { new SqlParameter("@id", id) });

            if (hfExamId.Value == id.ToString())
            {
                ResetToNewExam();
            }

            LoadContextExamDropdown();
            LoadExamsGrid();
            Toast("Exam deleted.", "success");
        }

        // =========================================================
        // SAVE EXAM (details + which questions it uses)
        // =========================================================

        protected void btnSaveExam_Click(object sender, EventArgs e)
        {
            string name = txtExamName.Text.Trim();
            if (name == "")
            {
                Toast("Please enter an exam name.", "error");
                return;
            }

            int durationMinutes;
            if (!int.TryParse(txtDuration.Text.Trim(), out durationMinutes))
            {
                Toast("Please enter a valid duration in minutes.", "error");
                return;
            }

            decimal passingMarks;
            if (!decimal.TryParse(txtPassingMarks.Text.Trim(), out passingMarks))
            {
                Toast("Please enter valid passing marks.", "error");
                return;
            }

            decimal positivePerQuestion;
            if (!decimal.TryParse(txtPositivePerQuestion.Text.Trim(), out positivePerQuestion))
            {
                Toast("Please enter marks awarded per correct answer.", "error");
                return;
            }

            decimal negativePerQuestion;
            if (!decimal.TryParse(txtNegativePerQuestion.Text.Trim(), out negativePerQuestion))
            {
                Toast("Please enter marks deducted per wrong answer.", "error");
                return;
            }

            DateTime startDate, endDate;
            if (!DateTime.TryParse(txtStartDate.Text, out startDate))
            {
                Toast("Please select a start date & time.", "error");
                return;
            }

            if (!DateTime.TryParse(txtEndDate.Text, out endDate))
            {
                Toast("Please select an end date & time.", "error");
                return;
            }

            if (endDate <= startDate)
            {
                Toast("End date must be after the start date.", "error");
                return;
            }

            string mode = ddlQuestionMode.SelectedValue;
            int questionCount = 0;
            int bankId = 0;
            string bankName = "";

            if (mode == "Random")
            {
                if (!int.TryParse(txtQuestionCount.Text, out questionCount) || questionCount <= 0)
                {
                    Toast("Please enter a valid question count.", "error");
                    return;
                }

                if (ddlQuestionBank.SelectedValue == "")
                {
                    Toast("Please select a question bank.", "error");
                    return;
                }

                bankName = ddlQuestionBank.SelectedValue;

                // Look up the BankId from the name (dropdowns store BankName as value)
                object bankIdObj = DatabaseHelper.ExecuteScalar(
                    "SELECT BankId FROM QuestionBanks WHERE BankName=@n",
                    new SqlParameter[] { new SqlParameter("@n", bankName) });

                if (bankIdObj == null || bankIdObj == DBNull.Value)
                {
                    Toast("Could not find the selected question bank.", "error");
                    return;
                }

                bankId = Convert.ToInt32(bankIdObj);
            }
            else
            {
                if (ddlManualBank.SelectedValue == "")
                {
                    Toast("Please select a question bank to pick questions from.", "error");
                    return;
                }

                bankName = ddlManualBank.SelectedValue;

                object bankIdObj = DatabaseHelper.ExecuteScalar(
                    "SELECT BankId FROM QuestionBanks WHERE BankName=@n",
                    new SqlParameter[] { new SqlParameter("@n", bankName) });

                if (bankIdObj == null || bankIdObj == DBNull.Value)
                {
                    Toast("Could not find the selected question bank.", "error");
                    return;
                }

                bankId = Convert.ToInt32(bankIdObj);

                foreach (ListItem item in cblManualQuestions.Items)
                {
                    if (item.Selected) questionCount++;
                }

                if (questionCount == 0)
                {
                    Toast("Please select at least one question.", "error");
                    return;
                }
            }

            object duplicate = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM Exams WHERE ExamName=@n AND IsActive=1 AND ExamId<>@id",
                new SqlParameter[]
                {
                    new SqlParameter("@n", name),
                    new SqlParameter("@id", hfExamId.Value == "" ? 0 : Convert.ToInt32(hfExamId.Value))
                });

            if (Convert.ToInt32(duplicate) > 0)
            {
                Toast("An exam with this name already exists.", "error");
                return;
            }

            int examId = SaveExamDetails(name, durationMinutes, passingMarks, positivePerQuestion, negativePerQuestion, startDate, endDate, mode, questionCount, bankId);

            if (mode == "Manual")
            {
                SaveManualQuestionSelection(examId);
            }

            LoadContextExamDropdown();
            LoadExamIntoContext(examId);
            LoadExamsGrid();

            Toast("Exam saved!", "success");
        }

        private int SaveExamDetails(string name, int durationMinutes, decimal passingMarks,
            decimal positivePerQuestion, decimal negativePerQuestion,
            DateTime startDate, DateTime endDate, string mode, int questionCount, int bankId)
        {
            string desc = txtDescription.Text.Trim();
            bool active = chkIsActive.Checked;
            string status = ddlExamStatus.SelectedValue;

            if (hfExamId.Value == "")
            {
                string query = @"INSERT INTO Exams
                    (ExamName, Description, DurationMinutes, PassingMarks, PositiveMarksPerQuestion, NegativeMarksPerQuestion,
                     IsActive, ExamStatus, QuestionMode, QuestionCount, QuestionBankId, StartDate, EndDate)
                    VALUES (@n, @d, @m, @pass, @posM, @negM, @a, @status, @mode, @count, @bank, @start, @end);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@n", name),
                    new SqlParameter("@d", desc),
                    new SqlParameter("@m", durationMinutes),
                    new SqlParameter("@pass", passingMarks),
                    new SqlParameter("@posM", positivePerQuestion),
                    new SqlParameter("@negM", negativePerQuestion),
                    new SqlParameter("@a", active),
                    new SqlParameter("@status", status),
                    new SqlParameter("@mode", mode),
                    new SqlParameter("@count", questionCount),
                    new SqlParameter("@bank", bankId),
                    new SqlParameter("@start", startDate),
                    new SqlParameter("@end", endDate)
                };

                return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
            }
            else
            {
                int examId = Convert.ToInt32(hfExamId.Value);

                string query = @"UPDATE Exams SET
                    ExamName=@n, Description=@d, DurationMinutes=@m, PassingMarks=@pass,
                    PositiveMarksPerQuestion=@posM, NegativeMarksPerQuestion=@negM, IsActive=@a,
                    ExamStatus=@status, QuestionMode=@mode, QuestionCount=@count,
                    QuestionBankId=@bank, StartDate=@start, EndDate=@end
                    WHERE ExamId=@id";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@n", name),
                    new SqlParameter("@d", desc),
                    new SqlParameter("@m", durationMinutes),
                    new SqlParameter("@pass", passingMarks),
                    new SqlParameter("@posM", positivePerQuestion),
                    new SqlParameter("@negM", negativePerQuestion),
                    new SqlParameter("@a", active),
                    new SqlParameter("@status", status),
                    new SqlParameter("@mode", mode),
                    new SqlParameter("@count", questionCount),
                    new SqlParameter("@bank", bankId),
                    new SqlParameter("@start", startDate),
                    new SqlParameter("@end", endDate),
                    new SqlParameter("@id", examId)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                return examId;
            }
        }

        // Saves which questions the admin ticked in the checklist.
        // Stores the bank table's SlNo in ExamQuestions.QuestionId column
        // so ExamPortal can look them up later.
        private void SaveManualQuestionSelection(int examId)
        {
            DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM ExamQuestions WHERE ExamId=@ExamId",
                new SqlParameter[] { new SqlParameter("@ExamId", examId) });

            foreach (ListItem item in cblManualQuestions.Items)
            {
                if (!item.Selected) continue;

                // item.Value is the SlNo from the bank table
                DatabaseHelper.ExecuteNonQuery(
                    "INSERT INTO ExamQuestions (ExamId, QuestionId) VALUES (@ExamId, @QuestionId)",
                    new SqlParameter[]
                    {
                        new SqlParameter("@ExamId", examId),
                        new SqlParameter("@QuestionId", item.Value)
                    });
            }
        }
    }
}

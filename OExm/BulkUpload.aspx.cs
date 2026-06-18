using OfficeOpenXml;
using System;
using System.Data.SqlClient;
using System.IO;

namespace OExm
{
    public partial class BulkUpload :
        System.Web.UI.Page
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (Session["Role"] == null ||
                Session["Role"].ToString() != "Admin")
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void btnUpload_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (!fuExcel.HasFile)
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "Please select Excel file.";

                    return;
                }

                string ext =
                    Path.GetExtension(
                        fuExcel.FileName);

                if (ext != ".xlsx")
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "Only .xlsx files allowed.";

                    return;
                }

                using (ExcelPackage package =
                    new ExcelPackage(
                        fuExcel.FileContent))
                {
                    ExcelWorksheet ws =
                        package.Workbook
                               .Worksheets[1];

                    int rows =
                        ws.Dimension.End.Row;

                    int inserted = 0;

                    for (int i = 2;
                         i <= rows;
                         i++)
                    {
                        string query =
@"
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
    NegativeMarks,
    IsActive
)
VALUES
(
    @SectionId,
    @BankId,
    @QuestionText,
    @OptionA,
    @OptionB,
    @OptionC,
    @OptionD,
    @CorrectOption,
    @PositiveMarks,
    @NegativeMarks,
    1
)
";

                        SqlParameter[] p =
                        {
                            new SqlParameter(
                                "@SectionId",
                                ws.Cells[i,1].Text),

                            new SqlParameter(
                                "@BankId",
                                ws.Cells[i,2].Text),

                            new SqlParameter(
                                "@QuestionText",
                                ws.Cells[i,3].Text),

                            new SqlParameter(
                                "@OptionA",
                                ws.Cells[i,4].Text),

                            new SqlParameter(
                                "@OptionB",
                                ws.Cells[i,5].Text),

                            new SqlParameter(
                                "@OptionC",
                                ws.Cells[i,6].Text),

                            new SqlParameter(
                                "@OptionD",
                                ws.Cells[i,7].Text),

                            new SqlParameter(
                                "@CorrectOption",
                                ws.Cells[i,8].Text),

                            new SqlParameter(
                                "@PositiveMarks",
                                ws.Cells[i,9].Text),

                            new SqlParameter(
                                "@NegativeMarks",
                                ws.Cells[i,10].Text)
                        };

                        DatabaseHelper.ExecuteNonQuery(
                            query,
                            p);

                        inserted++;
                    }

                    lblMessage.ForeColor =
                        System.Drawing.Color.Green;

                    lblMessage.Text =
                        inserted +
                        " Questions Uploaded Successfully.";
                }
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    ex.Message;
            }
        }
    }
}
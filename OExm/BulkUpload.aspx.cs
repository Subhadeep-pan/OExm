using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;

namespace OExm
{
    public partial class BulkUpload : System.Web.UI.Page
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
            }
        }

        private void LoadBankDropdown()
        {
            var dt = BankHelper.GetAllBanks();
            ddlBank.DataSource = dt;
            ddlBank.DataTextField = "BankName";
            ddlBank.DataValueField = "BankName";  // use the name directly
            ddlBank.DataBind();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (ddlBank.SelectedValue == "")
            {
                ShowMessage("Please select a question bank.", true);
                return;
            }

            if (!fuExcel.HasFile)
            {
                ShowMessage("Please select an Excel file (.xlsx).", true);
                return;
            }

            if (System.IO.Path.GetExtension(fuExcel.FileName).ToLower() != ".xlsx")
            {
                ShowMessage("Only .xlsx files are allowed.", true);
                return;
            }

            string tableName = BankHelper.ToTableName(ddlBank.SelectedValue);

            if (!BankHelper.TableExists(tableName))
            {
                ShowMessage("The table for bank '" + ddlBank.SelectedValue + "' does not exist. Create it first in Question Bank.", true);
                return;
            }

            int inserted = 0;
            List<string> skipped = new List<string>();

            try
            {
                using (ExcelPackage pkg = new ExcelPackage(fuExcel.FileContent))
                {
                    if (pkg.Workbook.Worksheets.Count == 0)
                    {
                        ShowMessage("No worksheet found in the file.", true);
                        return;
                    }

                    var ws = pkg.Workbook.Worksheets[1];

                    if (ws.Dimension == null)
                    {
                        ShowMessage("The worksheet is empty.", true);
                        return;
                    }

                    int lastRow = ws.Dimension.End.Row;

                    // Row 1 is the header (SlNo | Question | M1 | M2 | M3 | M4 | Ans) — skip it.
                    for (int row = 2; row <= lastRow; row++)
                    {
                        // Column 1 = SlNo (ignored), 2 = Question, 3-6 = M1-M4, 7 = Ans
                        string question = ws.Cells[row, 2].Text.Trim();
                        string m1 = ws.Cells[row, 3].Text.Trim();
                        string m2 = ws.Cells[row, 4].Text.Trim();
                        string m3 = ws.Cells[row, 5].Text.Trim();
                        string m4 = ws.Cells[row, 6].Text.Trim();
                        string ans = ws.Cells[row, 7].Text.Trim();

                        // Completely blank row → skip quietly
                        if (question == "" && m1 == "" && m2 == "" && m3 == "" && m4 == "" && ans == "")
                            continue;

                        if (question == "" || m1 == "" || m2 == "" || m3 == "" || m4 == "" || ans == "")
                        {
                            skipped.Add("Row " + row + ": one or more required columns are blank.");
                            continue;
                        }

                        // Ans must exactly match one of M1-M4 (case insensitive)
                        bool valid = ans.Equals(m1, StringComparison.OrdinalIgnoreCase)
                                  || ans.Equals(m2, StringComparison.OrdinalIgnoreCase)
                                  || ans.Equals(m3, StringComparison.OrdinalIgnoreCase)
                                  || ans.Equals(m4, StringComparison.OrdinalIgnoreCase);

                        if (!valid)
                        {
                            skipped.Add("Row " + row + ": Ans doesn't match M1-M4.");
                            continue;
                        }

                        // Insert directly into the bank table (e.g. INSERT INTO [Java] ...)
                        BankHelper.InsertQuestion(tableName, question, m1, m2, m3, m4, ans);
                        inserted++;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Upload failed: " + ex.Message, true);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(inserted + " question(s) uploaded to [" + tableName + "].");

            if (skipped.Count > 0)
            {
                sb.Append("\n" + skipped.Count + " row(s) skipped:\n");
                int shown = 0;
                foreach (string reason in skipped)
                {
                    if (shown >= 10) { sb.Append("...and " + (skipped.Count - shown) + " more."); break; }
                    sb.Append("- " + reason + "\n");
                    shown++;
                }
            }

            ShowMessage(sb.ToString(), skipped.Count > 0 && inserted == 0);
        }

        private void ShowMessage(string message, bool isError)
        {
            lblMessage.ForeColor = isError ? System.Drawing.Color.Red : System.Drawing.Color.Green;
            lblMessage.Text = message;
        }
    }
}

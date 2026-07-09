using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace OExm
{
    public partial class Result : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CalculateResult();
            }
        }

        // Works out which exam attempt to show the result for.
        private int GetStudentExamId()
        {
            if (Request.QueryString["AttemptId"] != null)
            {
                return Convert.ToInt32(Request.QueryString["AttemptId"]);
            }

            if (Session["StudentExamId"] != null)
            {
                return Convert.ToInt32(Session["StudentExamId"]);
            }

            // Fall back to the student's most recently completed attempt.
            int userId = Convert.ToInt32(Session["UserId"]);
            object latest = DatabaseHelper.ExecuteScalar(
                "SELECT TOP 1 StudentExamId FROM StudentExams WHERE UserId=@u AND Status='Completed' ORDER BY StartTime DESC",
                new SqlParameter[] { new SqlParameter("@u", userId) });

            return (latest == null || latest == DBNull.Value) ? 0 : Convert.ToInt32(latest);
        }

        // =========================================================
        // MAIN CALCULATION - fills in every label on the page
        // =========================================================

        private void CalculateResult()
        {
            int studentExamId = GetStudentExamId();
            if (studentExamId == 0)
            {
                ShowNoResultFound();
                return;
            }

            DataTable attempt = GetAttemptInfo(studentExamId);
            if (attempt.Rows.Count == 0)
            {
                ShowNoResultFound();
                return;
            }

            string studentName = attempt.Rows[0]["FullName"].ToString();
            string examName = attempt.Rows[0]["ExamName"].ToString();
            decimal passingMarks = attempt.Rows[0]["PassingMarks"] == DBNull.Value ? 0 : Convert.ToDecimal(attempt.Rows[0]["PassingMarks"]);
            decimal positivePerQuestion = attempt.Rows[0]["PositiveMarksPerQuestion"] == DBNull.Value ? 1 : Convert.ToDecimal(attempt.Rows[0]["PositiveMarksPerQuestion"]);
            decimal negativePerQuestion = attempt.Rows[0]["NegativeMarksPerQuestion"] == DBNull.Value ? 0 : Convert.ToDecimal(attempt.Rows[0]["NegativeMarksPerQuestion"]);
            DateTime startTime = Convert.ToDateTime(attempt.Rows[0]["StartTime"]);
            DateTime endTime = attempt.Rows[0]["EndTime"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(attempt.Rows[0]["EndTime"]);

            // Every question the student was actually shown has a row here
            // (even if they left it unanswered), so this works correctly
            // for both "Manual" and "Random" question modes.
            DataTable responses = GetStudentResponses(studentExamId);

            int totalQuestions = responses.Rows.Count;
            int attempted = 0;
            int correct = 0;
            int wrong = 0;

            foreach (DataRow row in responses.Rows)
            {
                if (row["Status"].ToString() != "Answered") continue;

                attempted++;

                string selected = row["SelectedOption"].ToString().Trim();
                string correctOption = row["CorrectOption"].ToString().Trim();

                if (selected.Equals(correctOption, StringComparison.OrdinalIgnoreCase))
                {
                    correct++;
                }
                else
                {
                    wrong++;
                }
            }

            decimal totalScore = (correct * positivePerQuestion) - (wrong * negativePerQuestion);
            decimal maxScore = totalQuestions * positivePerQuestion;

            int unattempted = totalQuestions - attempted;
            decimal percentage = maxScore > 0 ? (totalScore / maxScore) * 100 : 0;
            bool passed = totalScore >= passingMarks;

            // ---- Fill in every label on the page ----

            lblStudentName.Text = studentName;
            lblExamName.Text = examName;
            lblDate.Text = endTime.ToString("dd MMM yyyy, hh:mm tt");
            lblGrade.Text = GetGrade(percentage);

            lblTotalQuestions.Text = totalQuestions.ToString();
            lblAttempted.Text = attempted.ToString();
            lblUnattempted.Text = unattempted.ToString();
            lblCorrect.Text = correct.ToString();
            lblWrong.Text = wrong.ToString();

            lblScore.Text = totalScore.ToString("0.0");
            lblPercentage.Text = percentage.ToString("0.00"); // markup already prints the % sign

            lblResultStatus.Text = passed ? "PASSED" : "FAILED";
            lblResultStatus.CssClass = passed ? "pass" : "fail";

            SaveFinalScore(studentExamId, totalScore, endTime);

            if (Session["ResultEmailSent"] == null)
            {
                SendResultEmail(studentName, examName, totalScore, percentage, correct, wrong);
                Session["ResultEmailSent"] = true;
            }
        }

        private void ShowNoResultFound()
        {
            lblStudentName.Text = "-";
            lblExamName.Text = "-";
            lblDate.Text = "-";
            lblGrade.Text = "-";
            lblTotalQuestions.Text = "0";
            lblAttempted.Text = "0";
            lblUnattempted.Text = "0";
            lblCorrect.Text = "0";
            lblWrong.Text = "0";
            lblScore.Text = "N/A";
            lblPercentage.Text = "0.00";
            lblResultStatus.Text = "No result found";
            lblResultStatus.CssClass = "fail";
        }

        // Simple grade bands based on the percentage score. Adjust the
        // cut-offs here if your institution uses a different scale.
        private string GetGrade(decimal percentage)
        {
            if (percentage >= 90) return "A+";
            if (percentage >= 75) return "A";
            if (percentage >= 60) return "B";
            if (percentage >= 40) return "C";
            return "F";
        }

        private DataTable GetAttemptInfo(int studentExamId)
        {
            string query = @"
                SELECT u.FullName, e.ExamName, e.PassingMarks, e.PositiveMarksPerQuestion, e.NegativeMarksPerQuestion,
                       se.StartTime, se.EndTime
                FROM StudentExams se
                INNER JOIN Users u ON se.UserId = u.UserId
                INNER JOIN Exams e ON se.ExamId = e.ExamId
                WHERE se.StudentExamId = @se";

            return DatabaseHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@se", studentExamId) });
        }

        private DataTable GetStudentResponses(int studentExamId)
        {
            // sr.QuestionId is a bank table SlNo, not a legacy Questions.QuestionId
            // (this app scores exclusively from the dynamic bank tables now --
            // see BankHelper). Each Exam has exactly one QuestionBankId, so we can
            // resolve the correct bank table for this attempt with no ambiguity,
            // then look up each response's correct answer from there.
            string tableName = BankHelper.GetBankTableNameForAttempt(studentExamId);
            Dictionary<int, string> correctOptions = BankHelper.GetCorrectOptionMap(tableName);

            DataTable responses = DatabaseHelper.ExecuteQuery(
                "SELECT QuestionId, SelectedOption, Status FROM StudentResponses WHERE StudentExamId=@se",
                new SqlParameter[] { new SqlParameter("@se", studentExamId) });

            DataTable result = new DataTable();
            result.Columns.Add("SelectedOption", typeof(string));
            result.Columns.Add("Status", typeof(string));
            result.Columns.Add("CorrectOption", typeof(string));

            foreach (DataRow row in responses.Rows)
            {
                int questionId = Convert.ToInt32(row["QuestionId"]);
                string correctOption;
                correctOptions.TryGetValue(questionId, out correctOption);

                DataRow r = result.NewRow();
                r["SelectedOption"] = row["SelectedOption"];
                r["Status"] = row["Status"];
                r["CorrectOption"] = correctOption ?? "";
                result.Rows.Add(r);
            }

            return result;
        }

        private void SaveFinalScore(int studentExamId, decimal totalScore, DateTime endTime)
        {
            string query = "UPDATE StudentExams SET Score=@score, EndTime=@end, Status='Completed' WHERE StudentExamId=@se";
            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[]
            {
                new SqlParameter("@score", totalScore),
                new SqlParameter("@end", endTime),
                new SqlParameter("@se", studentExamId)
            });
        }

        // =========================================================
        // EMAIL THE RESULT
        // =========================================================

        private void SendResultEmail(string studentName, string examName, decimal score, decimal percentage, int correct, int wrong)
        {
            try
            {
                object emailResult = DatabaseHelper.ExecuteScalar(
                    "SELECT Email FROM Users WHERE UserId=@u",
                    new SqlParameter[] { new SqlParameter("@u", Session["UserId"]) });

                if (emailResult == null || emailResult == DBNull.Value) return;
                string email = emailResult.ToString();

                string body = $@"
<div style='font-family:Segoe UI,Arial,sans-serif; max-width:700px; margin:auto; border:1px solid #e5e7eb; border-radius:12px; overflow:hidden;'>
    <div style='background:#2563eb; color:white; padding:25px; text-align:center;'>
        <h1>OExm Examination Result</h1>
    </div>
    <div style='padding:30px;'>
        <p>Hi {studentName}, your exam <b>{examName}</b> has been evaluated successfully.</p>
        <table style='width:100%; border-collapse:collapse; margin-top:20px;'>
            <tr><td><b>Score</b></td><td>{score}</td></tr>
            <tr><td><b>Percentage</b></td><td>{percentage:0.00}%</td></tr>
            <tr><td><b>Correct Answers</b></td><td>{correct}</td></tr>
            <tr><td><b>Wrong Answers</b></td><td>{wrong}</td></tr>
        </table>
        <br />
        <p>Thank you for participating in the examination.</p>
        <p>Regards,<br /><b>OExm Examination System</b></p>
    </div>
</div>";

                EmailHelper.SendEmail(email, "Exam Result Report", body);
            }
            catch
            {
                // Email is a nice-to-have here; a failed send should never break the result page.
            }
        }

        // =========================================================
        // DOWNLOAD PDF CERTIFICATE
        // =========================================================

        protected void btnDownloadPDF_Click(object sender, EventArgs e)
        {
            int studentExamId = GetStudentExamId();
            if (studentExamId == 0) return;

            string query = @"
                SELECT u.FullName, e.ExamName, se.StartTime, se.EndTime, se.Score
                FROM StudentExams se
                INNER JOIN Users u ON se.UserId = u.UserId
                INNER JOIN Exams e ON se.ExamId = e.ExamId
                WHERE se.StudentExamId=@se";
            SqlParameter[] parameters = { new SqlParameter("@se", studentExamId) };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count == 0) return;

            string studentName = dt.Rows[0]["FullName"].ToString();
            string examName = dt.Rows[0]["ExamName"].ToString();
            DateTime start = Convert.ToDateTime(dt.Rows[0]["StartTime"]);
            DateTime end = dt.Rows[0]["EndTime"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["EndTime"]) : DateTime.Now;
            decimal finalScore = dt.Rows[0]["Score"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["Score"]) : 0;

            Document doc = new Document(PageSize.A4, 50, 50, 50, 50);
            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                PdfContentByte cb = writer.DirectContent;
                cb.SetColorStroke(new BaseColor(37, 99, 235));
                cb.SetLineWidth(3);
                cb.Rectangle(20, 20, doc.PageSize.Width - 40, doc.PageSize.Height - 40);
                cb.Stroke();

                cb.SetColorStroke(BaseColor.DARK_GRAY);
                cb.SetLineWidth(1);
                cb.Rectangle(25, 25, doc.PageSize.Width - 50, doc.PageSize.Height - 50);
                cb.Stroke();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 26, BaseColor.DARK_GRAY);
                Font subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 13, BaseColor.GRAY);
                Font bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);
                Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                Font scoreLabelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.GRAY);
                Font resultFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 36, new BaseColor(37, 99, 235));

                Paragraph pTitle = new Paragraph("CERTIFICATE OF EXAM RESULT", titleFont);
                pTitle.Alignment = Element.ALIGN_CENTER;
                pTitle.SpacingBefore = 40;
                pTitle.SpacingAfter = 5;
                doc.Add(pTitle);

                Paragraph pSub = new Paragraph("Online Examination Portal Verification", subtitleFont);
                pSub.Alignment = Element.ALIGN_CENTER;
                pSub.SpacingAfter = 40;
                doc.Add(pSub);

                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1.5f, 90, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, 0)));
                line.SpacingAfter = 30;
                doc.Add(line);

                Paragraph pBody = new Paragraph();
                pBody.Add(new Chunk("This is to officially certify that candidate ", bodyFont));
                pBody.Add(new Chunk(studentName, boldFont));
                pBody.Add(new Chunk(" has successfully attempted and concluded the exam ", bodyFont));
                pBody.Add(new Chunk(examName, boldFont));
                pBody.Add(new Chunk(" online.", bodyFont));
                pBody.Alignment = Element.ALIGN_CENTER;
                pBody.SpacingAfter = 30;
                doc.Add(pBody);

                PdfPTable table = new PdfPTable(2);
                table.WidthPercentage = 80;
                table.SpacingAfter = 30;

                AddTableCell(table, "Candidate Name:", boldFont);
                AddTableCell(table, studentName, bodyFont);

                AddTableCell(table, "Exam Title:", boldFont);
                AddTableCell(table, examName, bodyFont);

                AddTableCell(table, "Completion Date:", boldFont);
                AddTableCell(table, end.ToString("dd MMM yyyy HH:mm"), bodyFont);

                AddTableCell(table, "Duration Elapsed:", boldFont);
                AddTableCell(table, ((int)(end - start).TotalMinutes).ToString() + " minutes", bodyFont);

                doc.Add(table);

                Paragraph pScoreLabel = new Paragraph("FINAL SCORE OBTAINED", scoreLabelFont);
                pScoreLabel.Alignment = Element.ALIGN_CENTER;
                doc.Add(pScoreLabel);

                Paragraph pScore = new Paragraph(finalScore.ToString("0.0"), resultFont);
                pScore.Alignment = Element.ALIGN_CENTER;
                pScore.SpacingAfter = 60;
                doc.Add(pScore);

                Paragraph pFooter = new Paragraph("Verified Online Examination Certificate. No signature required.", subtitleFont);
                pFooter.Alignment = Element.ALIGN_CENTER;
                doc.Add(pFooter);

                doc.Close();

                byte[] bytes = ms.ToArray();
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment; filename=ExamResult_" + studentExamId + ".pdf");
                Response.Buffer = true;
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(bytes);
                Response.End();
            }
        }

        private void AddTableCell(PdfPTable table, string label, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(label, font));
            cell.Border = Rectangle.NO_BORDER;
            cell.Padding = 8;
            table.AddCell(cell);
        }
    }
}

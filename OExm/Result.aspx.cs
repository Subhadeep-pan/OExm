using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace OExm
{
    public partial class Result : System.Web.UI.Page
    {
        // Public properties accessible in the ASPX markup
        public string AccuracyPercent = "0%";
        public string TimeSpentStr = "0m 0s";
        public string WeakAreasStr = "None";

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
                GetStudentExamId();
            }
        }
        private void SendResultEmail(
    decimal score,
    decimal percentage,
    int correct,
    int wrong)
        {
            try
            {
                string email = DatabaseHelper.ExecuteScalar(
                    "SELECT Email FROM Users WHERE UserId=@u",
                    new SqlParameter[]
                    {
                new SqlParameter("@u",
                Session["UserId"])
                    }).ToString();

                string body = $@"

<div style='font-family:Segoe UI,Arial,sans-serif;
            max-width:700px;
            margin:auto;
            border:1px solid #e5e7eb;
            border-radius:12px;
            overflow:hidden;'>

```
<div style='background:#2563eb;
            color:white;
            padding:25px;
            text-align:center;'>

    <h1>OExm Examination Result</h1>

</div>

<div style='padding:30px;'>

    <p>
        Your examination has been evaluated successfully.
    </p>

    <table style='width:100%;
                   border-collapse:collapse;
                   margin-top:20px;'>

        

        <tr>
            <td><b>Score</b></td>
            <td>{score}</td>
        </tr>

        <tr>
            <td><b>Percentage</b></td>
            <td>{percentage}%</td>
        </tr>

        <tr>
            <td><b>Correct Answers</b></td>
            <td>{correct}</td>
        </tr>

        <tr>
            <td><b>Wrong Answers</b></td>
            <td>{wrong}</td>
        </tr>


    </table>

    <br />

    <p>
        Thank you for participating in the examination.
    </p>

    <p>
        Regards,<br />
        <b>OExm Examination System</b>
    </p>

</div>
```

</div>";


                EmailHelper.SendEmail(
                    email,
                    "Exam Result Report",
                    body);
            }
            catch
            {
            }
        }
        private int GetStudentExamId()
        {
            int studentExamId = 0;
            if (Request.QueryString["AttemptId"] != null)
            {
                studentExamId = Convert.ToInt32(Request.QueryString["AttemptId"]);
            }
            else if (Session["StudentExamId"] != null)
            {
                studentExamId = Convert.ToInt32(Session["StudentExamId"]);
            }

            if (studentExamId == 0)
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                object latest = DatabaseHelper.ExecuteScalar(
                    "SELECT TOP 1 StudentExamId FROM StudentExams WHERE UserId=@u AND Status='Completed' ORDER BY StartTime DESC",
                    new SqlParameter[] { new SqlParameter("@u", userId) }
                );
                if (latest != null && latest != DBNull.Value)
                    studentExamId = Convert.ToInt32(latest);
            }
            return studentExamId;
        }

        private void CalculateResult()
        {
            int studentExamId = GetStudentExamId();
            if (studentExamId == 0)
            {
                lblScore.Text = "N/A";
                lblCorrect.Text = "0";
                lblWrong.Text = "0";
                lblPercentage.Text = "0.00%";
                return;
            }

            // 1. Get Exam attempt details
            string attemptQuery = "SELECT ExamId, StartTime, EndTime FROM StudentExams WHERE StudentExamId=@se";
            DataTable dtAttempt = DatabaseHelper.ExecuteQuery(attemptQuery, new SqlParameter[] { new SqlParameter("@se", studentExamId) });
            if (dtAttempt.Rows.Count == 0) return;

            int examId = Convert.ToInt32(dtAttempt.Rows[0]["ExamId"]);
            DateTime startTime = Convert.ToDateTime(dtAttempt.Rows[0]["StartTime"]);
            DateTime endTime = dtAttempt.Rows[0]["EndTime"] != DBNull.Value ? Convert.ToDateTime(dtAttempt.Rows[0]["EndTime"]) : DateTime.Now;

            // 2. Fetch all questions in this exam
            string qQuery = @"

SELECT
    q.QuestionId,
    q.PositiveMarks,
    s.SectionId,
    s.SectionName

FROM ExamQuestions eq

INNER JOIN Questions q
    ON eq.QuestionId = q.QuestionId

INNER JOIN Sections s
    ON q.SectionId = s.SectionId

WHERE eq.ExamId = @examId

";
            DataTable dtQuestions = DatabaseHelper.ExecuteQuery(qQuery, new SqlParameter[] { new SqlParameter("@examId", examId) });

            int totalQuestions = dtQuestions.Rows.Count;
            decimal maxScore = 0;
            Dictionary<int, decimal> qMarks = new Dictionary<int, decimal>();
            Dictionary<int, string> qSections = new Dictionary<int, string>();
            
            // Track Section-wise Max Marks: SectionName -> MaxMarks
            Dictionary<string, decimal> sectionMaxMarks = new Dictionary<string, decimal>();

            foreach (DataRow r in dtQuestions.Rows)
            {
                int qId = Convert.ToInt32(r["QuestionId"]);
                decimal pos = Convert.ToDecimal(r["PositiveMarks"]);
                string secName = r["SectionName"].ToString();

                maxScore += pos;
                qMarks[qId] = pos;
                qSections[qId] = secName;

                if (!sectionMaxMarks.ContainsKey(secName))
                    sectionMaxMarks[secName] = 0;
                sectionMaxMarks[secName] += pos;
            }

            // 3. Fetch student responses
            string rQuery = @"

SELECT
    sr.QuestionId,
    sr.SelectedOption,
    sr.Status,
    q.CorrectOption,
    q.PositiveMarks,
    q.NegativeMarks,
    s.SectionName

FROM StudentResponses sr

INNER JOIN Questions q
    ON sr.QuestionId = q.QuestionId

INNER JOIN Sections s
    ON q.SectionId = s.SectionId

WHERE sr.StudentExamId=@se

";
            DataTable dtResponses =
    DatabaseHelper.ExecuteQuery(
        rQuery,
        new SqlParameter[]
        {
            new SqlParameter("@se", studentExamId)
        });
          

            int correct = 0;
            int wrong = 0;
            int attempted = 0;
            decimal totalScore = 0;

            // Track Section-wise Scores: SectionName -> Score
            Dictionary<string, decimal> sectionScores = new Dictionary<string, decimal>();

            foreach (DataRow row in dtResponses.Rows)
            {
                int qId = Convert.ToInt32(row["QuestionId"]);
                string selected = row["SelectedOption"].ToString().Trim();
                string correctOption = row["CorrectOption"].ToString().Trim();
                decimal positive = Convert.ToDecimal(row["PositiveMarks"]);
                decimal negative = Convert.ToDecimal(row["NegativeMarks"]);
                string secName = row["SectionName"].ToString();

                if (!sectionScores.ContainsKey(secName))
                    sectionScores[secName] = 0;

                // An answer counts if they chose an option
                if (row["Status"].ToString() == "Answered")
                {
                    attempted++;

                    if (selected.Equals(correctOption, StringComparison.OrdinalIgnoreCase))
                    {
                        correct++;
                        totalScore += positive;
                        sectionScores[secName] += positive;
                    }
                    else
                    {
                        wrong++;
                        totalScore -= negative;
                        sectionScores[secName] -= negative;
                    }
                }
            }

            int unattempted = totalQuestions - attempted;
            if (unattempted < 0)
            {
                unattempted = 0;
            }

            lblUnattempted.Text =
                unattempted.ToString();

            // 4. Compute Metrics
            decimal percentage = 0;
            if (maxScore > 0)
            {
                percentage = (totalScore / maxScore) * 100;
            }

            decimal accuracy = attempted > 0 ? ((decimal)correct / attempted) * 100 : 0;
            AccuracyPercent = accuracy.ToString("0.00") + "%";

            // Time Spent Display
            TimeSpan timeSpent = endTime - startTime;
            TimeSpentStr = string.Format("{0}m {1}s", (int)timeSpent.TotalMinutes, timeSpent.Seconds);

            // Weak Areas Detection (Sections scoring less than 50%)
            List<string> weakSections = new List<string>();
            foreach (var sec in sectionMaxMarks)
            {
                string secName = sec.Key;
                decimal secMax = sec.Value;
                decimal secScore = sectionScores.ContainsKey(secName) ? sectionScores[secName] : 0;
                
                decimal secPercent = secMax > 0 ? (secScore / secMax) * 100 : 0;
                if (secPercent < 50)
                {
                    weakSections.Add(string.Format("{0} ({1:0.0}%)", secName, secPercent));
                }
            }
            WeakAreasStr = weakSections.Count > 0 ? string.Join(", ", weakSections) : "None (All sections > 50%)";

           
            // Populate Chart.js JSON arrays
            StringBuilder sbLabels = new StringBuilder();
            StringBuilder sbScores = new StringBuilder();
            
            sbLabels.Append("[");
            sbScores.Append("[");
            
            int counter = 0;
            foreach (var sec in sectionMaxMarks)
            {
                string secName = sec.Key;
                decimal scoreObt = sectionScores.ContainsKey(secName) ? sectionScores[secName] : 0;

                if (counter > 0)
                {
                    sbLabels.Append(",");
                    sbScores.Append(",");
                }
                
                sbLabels.Append("\"" + HttpUtility.JavaScriptStringEncode(secName) + "\"");
                sbScores.Append(scoreObt.ToString("0.0"));
                counter++;
            }
            
            sbLabels.Append("]");
            sbScores.Append("]");
            

            // 5. Update Attempt record with final calculated Score
            string updateAttemptQuery = "UPDATE StudentExams SET Score=@score, EndTime=@end, Status='Completed' WHERE StudentExamId=@se";
            SqlParameter[] updateParams = {
                new SqlParameter("@score", totalScore),
                new SqlParameter("@end", endTime),
                new SqlParameter("@se", studentExamId)
            };
            DatabaseHelper.ExecuteNonQuery(updateAttemptQuery, updateParams);

            // Show Result labels
            lblScore.Text = totalScore.ToString("0.0");
            lblCorrect.Text = correct.ToString();
            lblWrong.Text = wrong.ToString();
            lblPercentage.Text = percentage.ToString("0.00") + "%";


            if (Session["ResultEmailSent"] == null)
            {
                SendResultEmail(
                    totalScore,
                    percentage,
                    correct,
                    wrong);

                Session["ResultEmailSent"] = true;
            }
        }

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

            // Generate professional iTextSharp PDF certificate
            Document doc = new Document(PageSize.A4, 50, 50, 50, 50);
            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Certificate Border
                PdfContentByte cb = writer.DirectContent;
                cb.SetColorStroke(new BaseColor(37, 99, 235)); // Accent Blue
                cb.SetLineWidth(3);
                cb.Rectangle(20, 20, doc.PageSize.Width - 40, doc.PageSize.Height - 40);
                cb.Stroke();

                cb.SetColorStroke(BaseColor.DARK_GRAY);
                cb.SetLineWidth(1);
                cb.Rectangle(25, 25, doc.PageSize.Width - 50, doc.PageSize.Height - 50);
                cb.Stroke();

                // Fonts
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 26, BaseColor.DARK_GRAY);
                Font subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 13, BaseColor.GRAY);
                Font bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);
                Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                Font scoreLabelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.GRAY);
                Font resultFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 36, new BaseColor(37, 99, 235));

                // Title
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

                // Certificate body
                Paragraph pBody = new Paragraph();
                pBody.Add(new Chunk("This is to officially certify that candidate ", bodyFont));
                pBody.Add(new Chunk(studentName, boldFont));
                pBody.Add(new Chunk(" has successfully attempted and concluded the exam ", bodyFont));
                pBody.Add(new Chunk(examName, boldFont));
                pBody.Add(new Chunk(" online.", bodyFont));
                pBody.Alignment = Element.ALIGN_CENTER;
                pBody.SpacingAfter = 30;
                doc.Add(pBody);

                // Table
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

                // Score block
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
using System.Net;
using System.Net.Mail;

namespace OExm
{
    public static class EmailHelper
    {
        public static void SendEmail(
            string toEmail,
            string subject,
            string body)
        {
            MailMessage mail =
                new MailMessage();

            mail.To.Add(toEmail);

            mail.Subject = subject;

            mail.Body = body;

            mail.IsBodyHtml = true;

            SmtpClient smtp =
                new SmtpClient();

            smtp.Send(mail);
        }
    }
}
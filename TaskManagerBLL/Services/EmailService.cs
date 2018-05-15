using System.Configuration;
using System.Net;
using System.Net.Mail;
using TaskManagerBLL.Interfaces;

namespace TaskManagerBLL.Services
{
    public class EmailService : IEmailService
    {
        public const string SUBJECT_NEW_TEAM_MEMBER = "You joined a team!";

        public const string BODY_NEW_TEAM_MEMBER =
            "<p>Hi {0},</p>" +
            "<br />" +
            "<p>You joined team \"{1}\"! Congrats!!</p>" +
            "<p>Please login and manage your tasks.</p>" +
            "<br />" +
            "<p>Thank you,<br />" +
            "{2}</p>";

        public void Send(string from, string to, string subject, string body)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            var smtp = new SmtpClient();
            smtp.Host = ConfigurationManager.AppSettings["smtp.host"];
            smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtp.port"]);
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["smtp.user"],
                                                     ConfigurationManager.AppSettings["smtp.password"]);
            
            smtp.Send(mail);
        }
    }
}

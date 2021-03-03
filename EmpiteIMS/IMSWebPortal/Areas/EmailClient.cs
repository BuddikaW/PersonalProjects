using IMSWebPortal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace IMSWebPortal.Areas
{
    public class EmailClient
    {
        private readonly ApplicationDbContext _context;
        private int port;
        private string host;
        private string userName;
        private string password;

        public EmailClient(ApplicationDbContext context)
        {
            _context = context;
            GetMailConfigurations();
        }

        public void GetMailConfigurations()
        {
            var mailConfigurations = _context.EmailDetails.FirstOrDefault();
            port = mailConfigurations.Port;
            host = mailConfigurations.Host;
            userName = mailConfigurations.UserName;
            password = mailConfigurations.Password;
        }

        public bool SendEmail(string htmlString, string toRecipient, string subject)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress(userName);
                message.To.Add(new MailAddress(toRecipient));
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = htmlString;
                SmtpClient smtp = new SmtpClient();
                smtp.Port = port;
                smtp.Host = host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(userName, password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }

        public void SendEmail(string htmlString, List<string> toRecipients, string subject)
        {
            try
            {
                foreach(var toRecipient in toRecipients)
                {
                    SendEmail(htmlString, toRecipient, subject);
                }
            }
            catch (Exception ex) {
            }
        }
    }
}

using System.Net;
using System.Net.Mail;

namespace Food_Sharing_Food.Models
{
    public class Gmail
    {
        public string To { get; set; }
        public string Body { get; set; }

        public void SendMail()
        {
            MailMessage mc = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString(), To);

            // Set the HTML body directly
            mc.Body = Body;
            mc.IsBodyHtml = true; // Set to true to use HTML in the email body.

            // Configure SMTP information
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Timeout = 1000000;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            // Configure Gmail account for sending email
            NetworkCredential nc = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString(), System.Configuration.ConfigurationManager.AppSettings["Password"].ToString());
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;

            // Send email
            smtp.Send(mc);
        }
    }
}

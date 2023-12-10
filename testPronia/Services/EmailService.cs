using System.Net;
using System.Net.Mail;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using testPronia.Interfaces;

namespace testPronia.Services
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendMailAsync(string mailTo, string subject, string body,bool isHTML)
        {
           SmtpClient smtpClient = new SmtpClient(_configuration["Email:Host"],Convert.ToInt32(_configuration["Email:Port"]));
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(_configuration["Email:LoginEmail"], _configuration["Email:Password"]);

            MailAddress to = new MailAddress(mailTo);
            MailAddress from = new MailAddress(_configuration["Email:LoginEmail"],"Pronia");

            MailMessage message = new MailMessage(from,to);
            message.Subject=subject;
            message.Body=body;
            message.IsBodyHtml=isHTML;
        }
    }
    
}

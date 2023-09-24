using Microsoft.AspNetCore.Html;
using System;
using System.Net;
using System.Net.Mail;

namespace Pipelights.Website.BusinessService
{
    public interface IEmailService
    {
        bool SendEmail(string toEmail, string subject, string body);
        
    }
    public class EmailService: IEmailService
    {
        public EmailService()
        {
            
        }

        const string fromPassword = "kxivjuyuykcohrxk";

        public bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                var fromAddress = new MailAddress("adrian12nagy@gmail.com", "Pipelights Website");
                var toAddress = new MailAddress(toEmail, "Pipelights by Serox");

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}

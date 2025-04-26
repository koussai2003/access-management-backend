using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using AccessManagementAPI.Models;
using AccessManagementAPI.Data;
using System.Threading.Tasks;

namespace AccessManagementAPI.Services
{
     public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string FromName { get; set; }
        public string FromAddress { get; set; }
    }
    public class EmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailSettings _emailSettings;
        private readonly EmailTemplateService _templateService;

        public EmailService(
            ApplicationDbContext context, 
            IOptions<EmailSettings> emailSettings,
            EmailTemplateService templateService)
        {
            _context = context;
            _emailSettings = emailSettings.Value;
            _templateService = templateService;
        }

        public async Task SendValidationEmail(
            string recipientEmail,
            string senderEmail,
            string applicationName,
            string status,
            string validatorName,
            int requestId)
        {
            string subject = "Request Validation Update";
            string body = _templateService.GetValidationEmail(applicationName, status, validatorName);
            
            await SendAndRecordEmail(
                recipientEmail,
                senderEmail,
                subject,
                body,
                requestId,
                "validation");
        }

        public async Task SendRejectionEmail(
            string recipientEmail,
            string senderEmail,
            string applicationName,
            string comment,
            string rejectorName,
            int requestId)
        {
            string subject = "Request Rejected";
            string body = _templateService.GetRejectionEmail(applicationName, comment, rejectorName);
            
            await SendAndRecordEmail(
                recipientEmail,
                senderEmail,
                subject,
                body,
                requestId,
                "rejection");
        }

        public async Task SendApprovalEmail(
            string recipientEmail,
            string senderEmail,
            string applicationName,
            string approverName,
            int requestId)
        {
            string subject = "Request Approved";
            string body = _templateService.GetApprovalEmail(applicationName, approverName);
            
            await SendAndRecordEmail(
                recipientEmail,
                senderEmail,
                subject,
                body,
                requestId,
                "approval");
        }

        public async Task SendAndRecordEmail(
            string recipientEmail,
            string senderEmail,
            string subject,
            string body,
            int requestId,
            string emailType)
        {
            // Send the email
            await SendEmailAsync(recipientEmail, subject, body);
            
            // Record in database
            await SaveEmailNotification(recipientEmail, senderEmail, subject, body, requestId, emailType);
        }

        private async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromAddress, _emailSettings.FromName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }

        private async Task SaveEmailNotification(
            string recipientEmail,
            string senderEmail,
            string subject,
            string body,
            int requestId,
            string emailType)
        {
            var email = new EmailNotification
            {
                RecipientEmail = recipientEmail,
                SenderEmail = senderEmail,
                Subject = subject,
                Body = body,
                RelatedRequestId = requestId,
                EmailType = emailType
            };

            _context.EmailNotifications.Add(email);
            await _context.SaveChangesAsync();
        }
    }
}
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MainOps.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.Key, subject, message, email);
        }
        public Task SendEmailAsync2(string email, string subject, string message,string footstrHTML,string footstrPLAIN,string filename,string file)
        {
            return Execute2(Options.Key, subject, message, email,footstrHTML,footstrPLAIN,filename,file);
        }
        public Task SendEmailAsync3(string email, string subject, string message, string footstrHTML, string footstrPLAIN)
        {
            return Execute3(Options.Key, subject, message, email, footstrHTML, footstrPLAIN);
        }
        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                // should be a domain other than yahoo.com, outlook.com, hotmail.com, gmail.com
                From = new EmailAddress(Options.DefaultSenderEmail, Options.DefaultSenderDisplayName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);
        }
        public Task Execute2(string apiKey, string subject, string message, string email, string footstrHTML, string footstrPLAIN,string filename,string file)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Options.DefaultSenderEmail, Options.DefaultSenderDisplayName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.SetFooterSetting(true, footstrHTML, footstrPLAIN);
            msg.AddTo(new EmailAddress(email));
            msg.AddAttachment(filename,file);
            return client.SendEmailAsync(msg);
        }
        public Task Execute3(string apiKey, string subject, string message, string email, string footstrHTML, string footstrPLAIN)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Options.DefaultSenderEmail, Options.DefaultSenderDisplayName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.SetFooterSetting(true, footstrHTML, footstrPLAIN);
            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);
        }
    }
}

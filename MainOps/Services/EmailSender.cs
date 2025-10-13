using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Task TestSendEmailAsync(string email, string subject, string message, string footstrHTML, string footstrPLAIN, string filename, string file)
        {
            return Execute_(null, subject, message, email,footstrHTML,footstrPLAIN, filename, file);
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



        // test send mail
        public Task Execute_(string apiKey, string subject, string message, string email, string footstrHTML, string footstrPLAIN, string filename, string file)
        {
            var client = new SendGridClient(apiKey: "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");// apiKey);
            var msg = new SendGridMessage()
            {
                // should be a domain other than yahoo.com, outlook.com, hotmail.com, gmail.com
                //From = new EmailAddress(Options.DefaultSenderEmail, Options.DefaultSenderDisplayName),
                From = new EmailAddress("no-reply@hj-mainops.com", "H-J Support"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            msg.SetFooterSetting(true, footstrHTML, footstrPLAIN);
            msg.AddAttachment(filename, file);
            var response = client.SendEmailAsync(msg);
            Debug.WriteLine($"Status Code: {response.Status.ToString()}");

            return response;// client.SendEmailAsync(msg);
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

        public async Task<bool> SendMail(string toEmail, string subject, string body)
        {
            //if (string.IsNullOrEmpty(_sendGridApiKey))
            //    throw new Exception("SendGrid API Key is missing!");

            var client = new SendGridClient("");
            var from = new EmailAddress("your-email@example.com", "Your Name");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);

            var response = await client.SendEmailAsync(msg);

            return response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }


    }
}

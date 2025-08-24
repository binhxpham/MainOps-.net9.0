using MainOps.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MainOps.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
        public static Task SendAccountConfirmationAsync(this IEmailSender emailSender, string email, string link, ApplicationUser user)
        {
            return emailSender.SendEmailAsync(email, "Confirm Account",
                String.Concat("Please confirm the identity of \r\nUser: ",user.Email,"\r\nusername: ", user.UserName,"\r\nFirst name: ",user.FirstName, "\r\nLast name: ", user.LastName, $" <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>"));
        }
        public static Task SendRedoHourRegistrationAsync(this IEmailSender emailSender,string email, string link, ApplicationUser user,string comment)
        {
            return emailSender.SendEmailAsync(email, "Redo Hour Sheet",
                String.Concat("Hi ", user.full_name(), ",<br />Your Hourregistration has been cancelled with the following comment:<br /><br />", comment, "<br /><br />Please follow the link below to Re-make your hour sheet<br /><br />", $" <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>"));

        }
    }
}

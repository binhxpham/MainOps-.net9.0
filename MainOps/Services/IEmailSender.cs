using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailAsync2(string email, string subject, string message, string footstrHTML, string footstrPLAIN, string filename, string file);
        Task SendEmailAsync3(string email, string subject, string message, string footstrHTML, string footstrPLAIN);
    }
}

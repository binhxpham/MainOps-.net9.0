using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainOps.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MainOps.ExtensionMethods
{
    public static class UrlHelperExtensions
    {
        private static IHttpContextAccessor HttpContextAccessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ConfirmEmail),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string AddToAccountCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ConfirmAccount),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }
        public static string RedoHourSheetCallbackLink(this IUrlHelper urlHelper, string userId, string code,string HourRegistrationId, string scheme)
        {
            return urlHelper.Action(
                action: nameof(TrackItemsController.RedoHourSheetUser),
                controller: "TrackItems",
                values: new { userId, code,HourRegistrationId },
                protocol: scheme);
        }
        public static string ToHTML(string s)
        {
            var sb = new StringBuilder();
            if (s.Contains("&su"))
            {
                return s;
            }
            foreach (var c in s)
            {
                if (c == '2')
                {
                    sb.Append("&sup2;");
                }
                else if (c == '3')
                {
                    sb.Append("&sup3;");
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();

        }
    }
}

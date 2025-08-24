using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Override
{
    public class RouteDataRequestCultureProvider : RequestCultureProvider
    {
        public int IndexOfCulture;
        public int IndexofUICulture;

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            string culture = null;
            string uiCulture = null;

            var twoLetterCultureName = httpContext.Request.Path.Value.Split('/')[IndexOfCulture]?.ToString();
            var twoLetterUICultureName = httpContext.Request.Path.Value.Split('/')[IndexofUICulture]?.ToString();

            if (twoLetterCultureName == "de")
                culture = "de-DE";
            else if (twoLetterCultureName == "en")
                culture = uiCulture = "en-US";
            else if (twoLetterCultureName == "nl")
            {
                culture = uiCulture = "nl-NL";
            }
            else if(twoLetterCultureName == "da")
            {
                culture = uiCulture = "da-DK";
            }

            if (twoLetterUICultureName == "de")
                culture = "de-DE";
            else if (twoLetterUICultureName == "en")
                culture = uiCulture = "en-US";
            else if (twoLetterCultureName == "nl")
            {
                culture = uiCulture = "nl-NL";
            }
            else if (twoLetterCultureName == "da")
            {
                culture = uiCulture = "da-DK";
            }
            if (culture == null && uiCulture == null)
                return NullProviderCultureResult;

            if (culture != null && uiCulture == null)
                uiCulture = culture;

            if (culture == null && uiCulture != null)
                culture = uiCulture;

            var providerResultCulture = new ProviderCultureResult(culture, uiCulture);

            return Task.FromResult(providerResultCulture);
        }
    }
}

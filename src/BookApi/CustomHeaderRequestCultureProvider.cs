using Microsoft.AspNetCore.Localization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BookApi
{
    public class CustomHeaderRequestCultureProvider : RequestCultureProvider
    {
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var headerKeyValue = string.Empty;

            if (httpContext.Request.Headers.TryGetValue("user-locale", out var value))
            {
                headerKeyValue = value;
            }
            return Task.FromResult(new ProviderCultureResult(headerKeyValue));
        }
    }
}
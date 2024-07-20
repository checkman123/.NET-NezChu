using Carter;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Auth0.AspNetCore.Authentication;

namespace NezChu.Api
{
    public class AuthenticationsModule : CarterModule
    {
        private readonly ILogger<AuthenticationsModule> _logger;
        public AuthenticationsModule(ILogger<AuthenticationsModule> logger)
            : base("/api/Account")
        {
            base.WithTags("Auth0 Authentication");
            this._logger = logger;
        }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            //Get Request
            app.MapGet("/Login", Login).WithSummary("Auth0 Login");

            app.MapGet("/Logout", Logout).WithSummary("Auth0 Logout");
        }

        internal async Task Login(HttpContext httpContext, string redirectUri = "/")
        {
            var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(redirectUri)
            .Build();

            await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        }
        internal async Task Logout(HttpContext httpContext, string redirectUri = "/")
        {
            var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
              .WithRedirectUri(redirectUri)
              .Build();

            await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}

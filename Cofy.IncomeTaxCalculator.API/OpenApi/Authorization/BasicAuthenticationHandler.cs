using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Cofy.IncomeTaxCalculator.API.OpenApi.Authorization
{
    public class BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        private readonly string? _userName = configuration["BasicAuthentication:UserName"];
        private readonly string? _password = configuration["BasicAuthentication:Password"];

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Role, "Admin") };
            var identity = new ClaimsIdentity(claims, AuthenticationSchemes.Basic);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (authorizationHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
            {
                var token = authorizationHeader["Basic ".Length..].Trim();
                var credentialsAsEncodedString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var credentials = credentialsAsEncodedString.Split(':');
                return !ValidateCredentials(credentials[0], credentials[1]) ?
                    await Task.FromResult(AuthenticateResult.Fail("Basic authentication failed. Invalid Username Or Password.")) :
                    await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
            }

            Response.StatusCode = 401;
            return await Task.FromResult(AuthenticateResult.Fail("Basic authentication failed. Invalid Authorization Header."));
        }

        private bool ValidateCredentials(string username, string password)
        {
            return username.Equals(_userName) && password.Equals(_password);
        }
    }
}
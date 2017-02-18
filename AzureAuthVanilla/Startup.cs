[assembly: Microsoft.Owin.OwinStartup(typeof(AzureAuthVanilla.Startup))]

namespace AzureAuthVanilla
{
    using System.Collections.Generic;
    using System.IdentityModel.Tokens;
    using System.Web.Configuration;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.Cookies;
    using Microsoft.Owin.Security.OpenIdConnect;
    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                Provider = new CookieAuthenticationProvider
                {
                    OnResponseSignIn = ctx =>
                    {
                        string clientId = WebConfigurationManager.AppSettings["b2c:ClientId"];
                        string secret = WebConfigurationManager.AppSettings["b2c:ClientSecret"];
                        string tenant = WebConfigurationManager.AppSettings["b2c:Tenant"];
                        var graphClient = new GraphClient(clientId, secret, tenant);

                        string userObjectId = ctx.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                        List<string> result = graphClient.MemberOf(userObjectId).Result;
                    }
                }
            });

            string policy = "B2C_1_Blog_SignIn_SignUp";
            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = WebConfigurationManager.AppSettings["ida:ClientId"],
                    MetadataAddress = string.Format(
                        "https://login.microsoftonline.com/{0}/v2.0/.well-known/openid-configuration?p={1}", 
                        WebConfigurationManager.AppSettings["ida:Tenant"], 
                        policy),
                    RedirectUri = "http://localhost:44404/",
                    Scope = "openid",
                    ResponseType = "id_token",
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                    }
                });

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
        }
    }
}
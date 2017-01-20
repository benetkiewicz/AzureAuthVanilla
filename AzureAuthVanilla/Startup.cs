[assembly: Microsoft.Owin.OwinStartup(typeof(AzureAuthVanilla.Startup))]

namespace AzureAuthVanilla
{
    using System.IdentityModel.Tokens;
    using System.Web.Configuration;
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
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            string policy = "B2C_1_Blog_SignIn_SignUp";
            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = WebConfigurationManager.AppSettings["ida:ClientId"],
                    MetadataAddress = string.Format(
                        "https://login.microsoftonline.com/{0}/v2.0/.well-known/openid-configuration?p={1}", 
                        WebConfigurationManager.AppSettings["ida:Tenant"], 
                        policy),
                    AuthenticationType = policy,
                    RedirectUri = "http://localhost:44404/",
                    Scope = "openid",
                    ResponseType = "id_token",
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        SaveSigninToken = true //important to save the token in boostrapcontext
                    }
                });

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
        }
    }
}
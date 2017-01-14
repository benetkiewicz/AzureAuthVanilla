[assembly: Microsoft.Owin.OwinStartup(typeof(AzureAuthVanilla.Startup))]

namespace AzureAuthVanilla
{
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
            string authorityUrl = string.Format("https://login.microsoftonline.com/{0}", WebConfigurationManager.AppSettings["ida:Tenant"]);
            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = WebConfigurationManager.AppSettings["ida:ClientId"],
                    Authority = authorityUrl
                });

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
        }
    }
}
using DigiAeon.EvilApiClient.UI.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(DigiAeon.EvilApiClient.UI.Startup))]
namespace DigiAeon.EvilApiClient.UI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(string.Format("/{0}/{1}", LoginControllerConstants.ControllerName, LoginControllerConstants.IndexAction))
            });
        }
    }
}
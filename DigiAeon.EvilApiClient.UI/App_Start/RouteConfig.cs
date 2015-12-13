using System.Web.Mvc;
using System.Web.Routing;
using DigiAeon.EvilApiClient.UI.Controllers;

namespace DigiAeon.EvilApiClient.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = CustomerFileControllerConstants.ControllerName, action = CustomerFileControllerConstants.UploadAction, id = UrlParameter.Optional }
            );
        }
    }
}

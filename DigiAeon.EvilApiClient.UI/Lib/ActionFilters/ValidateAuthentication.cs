using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigiAeon.EvilApiClient.UI.Lib.ActionFilters
{
    public class ValidateAuthentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!HttpContext.Current.Request.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
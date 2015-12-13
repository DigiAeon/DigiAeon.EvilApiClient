using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace DigiAeon.EvilApiClient.UI.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        [HttpGet]
        [ActionName(LoginControllerConstants.IndexAction)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ActionName(LoginControllerConstants.IndexAction)]
        [ValidateAntiForgeryToken]
        public ActionResult IndexPost()
        {
            var authenticationManager = HttpContext.GetOwinContext().Authentication;

            if (authenticationManager != null)
            {
                if (authenticationManager.User == null || authenticationManager.User.Identity == null ||
                    string.Compare(Session.SessionID, authenticationManager.User.Identity.Name, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    var claims = new List<Claim> { new Claim(ClaimTypes.Name, Session.SessionID), new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
                    var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                    authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

                    // It is important to have atleast one value in session otherwise new session ID will be created on each request
                    Session["FakeAuthenticationGiven"] = true;
                }
            }

            return RedirectToAction(CustomerFileControllerConstants.UploadAction, CustomerFileControllerConstants.ControllerName);
        }
    }

    public class LoginControllerConstants
    {
        public const string ControllerName = "LoginController";
        public const string IndexAction = "Index";
    }
}
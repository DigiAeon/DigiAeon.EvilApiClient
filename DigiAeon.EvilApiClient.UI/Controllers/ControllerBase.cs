using System.Web.Mvc;
using DigiAeon.EvilApiClient.UI.Bootstrapper.Interfaces;
using DigiAeon.EvilApiClient.UI.Lib.ActionFilters;

namespace DigiAeon.EvilApiClient.UI.Controllers
{
    [ValidateAuthentication]
    public abstract class ControllerBase : Controller
    {
        protected ControllerBase(IConfig config)
        {
            Config = config;
        }
        public IConfig Config { get; }
    }
}
using System.Web.Mvc;

namespace CoreyAndRick.Project.Website.Controllers
{
    public abstract class BaseController : Controller
    {
        protected RedirectResult RedirectToReferrer()
        {
            var redirectUrl = Request.UrlReferrer;
            return redirectUrl == null
                ? new RedirectResult(Request.Url?.ToString())
                : new RedirectResult(redirectUrl.ToString());
        }
    }
}

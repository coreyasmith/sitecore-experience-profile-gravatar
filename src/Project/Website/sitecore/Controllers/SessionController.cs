using System.Web.Mvc;
using Sitecore.Analytics;

namespace CoreyAndRick.Project.Website.Controllers
{
    public class SessionController : BaseController
    {
        [HttpPost]
        public ActionResult AbandonSession()
        {
            Tracker.Current.EndTracking();
            Session.Abandon();
            return RedirectToReferrer();
        }
    }
}

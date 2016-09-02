using System.Web;
using System.Web.Mvc;

namespace MovieResources.Filters
{
    public class LogonFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("/Mine/Index");
            }
        }
    }
}
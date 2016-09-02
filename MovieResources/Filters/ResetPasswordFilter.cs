using System;
using System.Web.Mvc;

namespace MovieResources.Filters
{
    public class ResetPasswordFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Convert.ToBoolean(filterContext.HttpContext.Session["CanReset"]))
            {
                filterContext.Result = new RedirectResult("/Account/ForgotPassword");
            }
        }
    }
}
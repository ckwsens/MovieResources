using System.Linq;
using System.Web.Mvc;

namespace MovieResources.Filters
{
    public class AdminFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            MR_DataClassesDataContext _db = new MR_DataClassesDataContext();
            if (string.IsNullOrEmpty(filterContext.HttpContext.User.Identity.Name) || string.IsNullOrWhiteSpace(filterContext.HttpContext.User.Identity.Name))
            {
                filterContext.Result = new RedirectResult("/Account/Login");
                return;
            }
            if (!(bool)_db.tbl_UserAccount.Single(m => m.user_Account == filterContext.HttpContext.User.Identity.Name).user_IsAdmin)
            {
                filterContext.Result = new RedirectResult("/Error/NotAdmin");
            }
        }
    }
}
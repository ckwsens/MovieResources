using MovieResources.Helpers;
using System.Web.Mvc;

namespace MovieResources.Controllers
{
    public class MarkController : Controller
    {
        //
        // GET: /Mark/Create/
        [Authorize]
        public ActionResult Create(string target, int type, string returnurl)
        {
            if (type<=0||type>=7)
            {
                return RedirectToAction("NotFound", "Error");
            }
            else
            {
                if (type == 1 || type == 2 || type == 3)
                {
                    if (!MovieManager.Exist(target))
                    {
                        return RedirectToAction("NotFound", "Error");
                    }
                }
                else if (type == 4)
                {
                    if (!CelebManager.Exist(target))
                    {
                        return RedirectToAction("NotFound", "Error");
                    }
                }
                else if (type == 5)
                {
                    if (!ResManager.Exist(target))
                    {
                        return RedirectToAction("NotFound", "Error");
                    }
                }
                else if (type == 6)
                {
                    if (!AskManager.Exist(target))
                    {
                        return RedirectToAction("NotFound", "Error");
                    }
                }
                else
                {
                    if (!AlbumManager.Exist(target))
                    {
                        return RedirectToAction("NotFound", "Error");
                    }
                }
            }
            if (User.Identity.Name != null)
            {
                MarkManager.Create(target, AccountManager.GetId(User.Identity.Name), type);
            }
            return Redirect(returnurl);
        }

        //
        // GET: /Mark/Cancel/
        [Authorize]
        public ActionResult Cancel(string target, int type, string returnurl)
        {
            if (type <= 0 || type >= 7)
            {
                return RedirectToAction("NotFound", "Error");
            }
            else
            {
                if (type == 1 || type == 2 || type == 3)
                {
                    if (!MovieManager.Exist(target))
                    {
                        return RedirectToAction("NotFound", "Error");
                    }
                }
                else if (type == 4)
                {
                    if (!CelebManager.Exist(target))
                    {
                        return RedirectToAction("NotFound", "Error");
                    }
                }
                else if (type == 5)
                {
                    if (!ResManager.Exist(target))
                    {
                        return RedirectToAction("NotFound", "Error");
                    }
                }
                else if (type == 6)
                {
                    if (!AskManager.Exist(target))
                    {
                        return RedirectToAction("NotFound", "Error");
                    }
                }
                else
                {
                    if (!AlbumManager.Exist(target))
                    {
                        return RedirectToAction("NotFound", "Error");
                    }
                }
            }
            if (User.Identity.Name != null)
            {
                MarkManager.Cancel(target, AccountManager.GetId(User.Identity.Name), type);
            }
            return Redirect(returnurl);
        }
    }
}
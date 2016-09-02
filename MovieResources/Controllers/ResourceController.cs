using MovieResources.Helpers;
using MovieResources.Models;
using System.Linq;
using System.Web.Mvc;

namespace MovieResources.Controllers
{
    public class ResourceController : Controller
    {
        private MR_DataClassesDataContext _db = new MR_DataClassesDataContext();

        #region 创建资源
        //
        // GET: /Resource/Create/
        [Authorize]
        public ActionResult Create(string id)
        {
            if (!MovieManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            ManageResViewModel res = new ManageResViewModel() { MovieId = id, MovieTitle = _db.tbl_Movie.Single(m => m.movie_Id == id).movie_Title };
            return View(res);
        }

        //
        // POST: /Resource/Create/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ManageResViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Content) && !string.IsNullOrWhiteSpace(model.Content))
            {
                model.Content = System.Web.HttpUtility.UrlDecode(model.Content);

                var user = _db.tbl_UserAccount.SingleOrDefault(u => u.user_Account == User.Identity.Name);
                if ((bool)user.user_IsAdmin)
                {
                    model.Status = 2;
                }
                else
                {
                    model.Status = 0;
                    model.User = user.user_Id;
                }
                if (model.Content.StartsWith("ed2k", true, null))
                {
                    model.ResType = 0;
                    ResManager.Create(model);
                    return RedirectToAction("Index", "Movie", new { id = model.MovieId });
                }
                else if (model.Content.StartsWith("magnet", true, null))
                {
                    model.ResType = 1;
                    ResManager.Create(model);
                    return RedirectToAction("Index", "Movie", new { id = model.MovieId });
                }
                else
                {
                    ModelState.AddModelError("", "URL不合法，请重新输入。");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "URL不能为空，请重新输入URL。");
                return View(model);
            }
        }

        //
        // GET: /Resource/CreateTorrent/
        [Authorize]
        public ActionResult CreateTorrent(string id)
        {
            if (!MovieManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            ManageResViewModel res = new ManageResViewModel() { MovieId = id, MovieTitle = _db.tbl_Movie.Single(m => m.movie_Id == id).movie_Title };
            return View(res);
        }

        //
        // POST: /Resource/CreateTorrent/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTorrent(ManageResViewModel model, System.Web.HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (file != null && file.ContentLength > 0)
            {
                if (System.IO.Path.GetExtension(file.FileName) == ".torrent")
                {
                    var fileName = System.IO.Path.Combine(Request.MapPath("~/Content/Torrent/"), model.Id + System.IO.Path.GetFileName(file.FileName));
                    file.SaveAs(fileName);
                    model.Content = System.IO.Path.GetFileName(file.FileName);
                    model.ResType = 2;

                    var user = _db.tbl_UserAccount.SingleOrDefault(u => u.user_Account == User.Identity.Name);
                    if ((bool)user.user_IsAdmin)
                    {
                        model.Status = 2;
                    }
                    else
                    {
                        model.Status = 0;
                    }
                    model.User = user.user_Id;
                    ResManager.Create(model);
                    return RedirectToAction("Index", "Movie", new { id = model.MovieId });
                }
                else
                {
                    ModelState.AddModelError("", "请选择正确的torrent文件");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "请选择一个torrent文件");
                return View(model);
            }
        }
        #endregion

        #region 删除资源
        //
        // GET: /Resource/Delete/
        [Authorize]
        public ActionResult Delete(string id, string returnurl)
        {
            if (!MovieManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }

            var user = _db.tbl_UserAccount.SingleOrDefault(m => m.user_Account == User.Identity.Name).user_Id;
            var favor = _db.tbl_Resource.SingleOrDefault(m => m.res_Id == id && m.res_User == user);
            if (favor != null)
            {
                string oldRes = _db.tbl_Resource.SingleOrDefault(m => m.res_Id == id).res_Content;
                ImageManager.Delete(Server.MapPath("~/Content/Torrent/" + oldRes));
                ResManager.Delete(id);
            }
            return Redirect(returnurl);
        }
        #endregion

        #region 点赞资源
        //
        // GET: /Resource/Favor/
        [Authorize]
        public ActionResult Favor(string id, string returnurl)
        {
            if (!MovieManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }

            var user = _db.tbl_UserAccount.SingleOrDefault(m => m.user_Account == User.Identity.Name).user_Id;
            var favor = _db.tbl_Mark.SingleOrDefault(m => m.mark_Target == id && m.mark_User == user);
            if (favor != null)
            {
                return RedirectToAction("Create", "Mark", new { target = id, type = 5, returnurl = returnurl });
            }
            else
            {
                return RedirectToAction("Cancel", "Mark", new { target = id, type = 5, returnurl = returnurl });
            }
        }
        #endregion
    }
}
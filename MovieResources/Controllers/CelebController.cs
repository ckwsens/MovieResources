using MovieResources.Helpers;
using MovieResources.Models;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Web.Mvc;

namespace MovieResources.Controllers
{
    public class CelebController : Controller
    {
        private MR_DataClassesDataContext _db = new MR_DataClassesDataContext();

        #region 影人详情页
        //
        // GET: /Celeb/Index/
        //public ActionResult Index(string id)
        //{
        //    if (!CelebManager.Exist(id))
        //    {
        //        return RedirectToAction("NotFound", "Error");
        //    }
        //    tbl_Celebrity tblCeleb = _db.tbl_Celebrity.SingleOrDefault(s => s.celeb_Id == id);
        //    CelebViewModel celeb = new CelebViewModel(tblCeleb);

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        celeb.IsCollect = MarkManager.Validate(tblCeleb.celeb_Id, AccountManager.GetId(User.Identity.Name), 4);

        //        celeb.CollectCount = _db.tbl_Mark.Where(m => m.mark_Target == id && m.mark_Type == 4).Count();

        //        if (tblCeleb.celeb_Create == AccountManager.GetId(User.Identity.Name) || (bool)_db.tbl_UserAccount.SingleOrDefault(a => a.user_Account == User.Identity.Name).user_IsAdmin)
        //        {
        //            celeb.IsCreate = true;
        //        }
        //    }
        //    return View(celeb);
        //}
        public ActionResult Index(string id)
        {
            if (!CelebManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            tbl_Celebrity tblCeleb = _db.tbl_Celebrity.SingleOrDefault(s => s.celeb_Id == id);
            CelebViewModel celeb = new CelebViewModel(tblCeleb);

            if (User.Identity.IsAuthenticated)
            {
                celeb.IsCollect = MarkManager.Validate(tblCeleb.celeb_Id, AccountManager.GetId(User.Identity.Name), 4);

                celeb.CollectCount = _db.tbl_Mark.Where(m => m.mark_Target == id && m.mark_Type == 4).Count();

                if (tblCeleb.celeb_Create == AccountManager.GetId(User.Identity.Name) || (bool)_db.tbl_UserAccount.SingleOrDefault(a => a.user_Account == User.Identity.Name).user_IsAdmin)
                {
                    celeb.IsCreate = true;
                }
            }
            return View(celeb);
        }
        #endregion

        #region 创建影人
        //
        // GET: /Celeb/Create/
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Celeb/Create/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateCelebViewModel celeb)
        {
            if (!ModelState.IsValid)
            {
                return View(celeb);
            }
            foreach (var item in celeb.DoubanID.Split('\n'))
            {
                if (item.Trim().Length == 0)
                    continue;
                JObject json = HtmlDecoder.GetJson("https://api.douban.com/v2/movie/celebrity/" + item);
                JToken msg;
                if (json.TryGetValue("msg", out msg))
                {
                    ModelState.AddModelError("", string.Format("{0} {1} {2}", "添加编号为", item, "的影人 失败"));
                }
                else
                {
                    ModelState.AddModelError("", string.Format("{0}{1}{2}", "添加编号为", item, "的影人 成功"));
                    CelebManager.CreateJson(json, Server.MapPath("~/Content/Celeb/"), AccountManager.GetId(User.Identity.Name));
                }
            }
            return View();
        }

        //
        // GET: /Celeb/CreateCeleb/
        [Authorize]
        public ActionResult CreateCeleb()
        {
            return View();
        }

        //
        // POST: /Celeb/CreateCeleb/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCeleb(ManageCelebViewModel celeb, System.Web.HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(celeb);
            }
            if (file != null && file.ContentLength > 0)
            {
                var fileName = System.IO.Path.Combine(Request.MapPath("~/Content/Celeb/"), System.IO.Path.GetFileName(file.FileName));
                file.SaveAs(fileName);
                celeb.Avatar = System.IO.Path.GetFileName(file.FileName);
            }
            if ((bool)_db.tbl_UserAccount.SingleOrDefault(u => u.user_Account == User.Identity.Name).user_IsAdmin)
            {
                celeb.Status = 2;
            }
            else
            {
                celeb.Status = 0;
            }
            celeb.Create = AccountManager.GetId(User.Identity.Name);
            string newId = CelebManager.CreateCeleb(celeb);
            return RedirectToAction("Edit", new { id = newId });
        }
        #endregion

        #region 修改影人
        //
        // GET: /Celeb/Edit/
        [Authorize]
        public ActionResult Edit(string id)
        {
            if (!CelebManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            tbl_Celebrity tblceleb = _db.tbl_Celebrity.SingleOrDefault(s => s.celeb_Id == id);
            ManageCelebViewModel celeb = new ManageCelebViewModel(tblceleb);
            return View(celeb);
        }

        //
        // POST: /Celeb/Edit/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ManageCelebViewModel model, System.Web.HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (file != null && file.ContentLength > 0)
            {
                var fileName = System.IO.Path.Combine(Request.MapPath("~/Content/Celeb/"), System.IO.Path.GetFileName(file.FileName));
                file.SaveAs(fileName);
                model.Avatar = System.IO.Path.GetFileName(file.FileName);

                string oldAvatar = _db.tbl_Celebrity.SingleOrDefault(c => c.celeb_Id == model.Id).celeb_Avatar;
                if (model.Avatar != oldAvatar && oldAvatar != "Celeb_1.jpg")
                {
                    ImageManager.Delete(Server.MapPath("~/Content/Celeb/" + oldAvatar));
                }
            }
            CelebManager.Edit(model);
            return RedirectToAction("Edit", new { id = model.Id });
        }
        #endregion

        #region 删除影人
        //
        // GET: /Celeb/Delete/
        [Authorize]
        public ActionResult Delete(string id, string returnurl)
        {
            if (!CelebManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //    return HttpNotFound();
            //}
            tbl_Celebrity tblceleb = _db.tbl_Celebrity.SingleOrDefault(s => s.celeb_Id == id);
            ManageCelebViewModel celeb = new ManageCelebViewModel(tblceleb);
            ViewBag.ReturnUrl = returnurl;
            return View(celeb);
        }

        //
        // Post: /Celeb/Delete/
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(string id, string returnurl)
        {
            if (!CelebManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }

            string oldAvatar = _db.tbl_Celebrity.SingleOrDefault(c => c.celeb_Id == id).celeb_Avatar;
            if (oldAvatar != "Celeb_1.jpg")
            {
                ImageManager.Delete(Server.MapPath("~/Content/Celeb/" + oldAvatar));
            }
            CelebManager.Delete(id);
            return RedirectToLocal(returnurl);
        }
        #endregion

        #region 更新影人
        //
        // GET: /Celeb/Refresh/
        [Authorize]
        public ActionResult Refresh(string id)
        {
            if (!CelebManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            RefreshCelebViewModel refresh = new RefreshCelebViewModel();

            tbl_Celebrity tblceleb = _db.tbl_Celebrity.SingleOrDefault(s => s.celeb_Id == id);
            refresh.Old = new ManageCelebViewModel(tblceleb);

            JObject json = HtmlDecoder.GetJson("https://api.douban.com/v2/movie/celebrity/" + tblceleb.celeb_DoubanID);
            JToken msg;
            if (json.TryGetValue("msg", out msg))
            {
                refresh.New = new ManageCelebViewModel();
                refresh.New.Id = refresh.Old.Id;
            }
            else
            {
                tblceleb = CelebManager.JsonToCeleb(json, Server.MapPath("~/Content/Celeb/"));
                refresh.New = new ManageCelebViewModel(tblceleb);
                refresh.New.Id = refresh.Old.Id;
            }
            TempData["New"] = refresh.New;

            return View(refresh);
        }

        //
        // Post: /Celeb/Refresh/
        [HttpPost, ActionName("Refresh")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult RefreshConfirm(string id)
        {
            if (!CelebManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            ManageCelebViewModel movie = TempData["New"] as ManageCelebViewModel;
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = movie.Id });
            }
            CelebManager.Edit(movie);
            return RedirectToAction("Edit", new { id = movie.Id });
        }
        #endregion

        #region 影人作品页
        //
        // GET: /Celeb/Work/
        public ActionResult Work(string id)
        {
            if (!CelebManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            tbl_Celebrity tblCeleb = _db.tbl_Celebrity.SingleOrDefault(s => s.celeb_Id == id);
            CelebViewModel celeb = new CelebViewModel(tblCeleb);

            if (User.Identity.IsAuthenticated)
            {
                for (int i = 0; i < celeb.Works.Count(); i++)
                {
                    MovieViewModel movie = celeb.Works[i].Work;
                    celeb.Works[i].Work.IsPlan = MarkManager.Validate(celeb.Works[i].Work.Id, AccountManager.GetId(User.Identity.Name), 1);
                    celeb.Works[i].Work.IsFinish = MarkManager.Validate(celeb.Works[i].Work.Id, AccountManager.GetId(User.Identity.Name), 2);
                    celeb.Works[i].Work.IsFavor = MarkManager.Validate(celeb.Works[i].Work.Id, AccountManager.GetId(User.Identity.Name), 3);
                }
            }
            return View(celeb);
        }
        #endregion

        #region 帮助程序
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (!Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl) && !string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}
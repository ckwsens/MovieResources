using MovieResources.Filters;
using MovieResources.Helpers;
using MovieResources.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MovieResources.Areas.Manage.Controllers
{
    public class CelebController : Controller
    {
        MR_DataClassesDataContext _db = new MR_DataClassesDataContext();

        #region 影人管理首页
        //
        // GET: /Manage/Celeb/
        [AdminFilter]
        public ActionResult Index(string search, int page = 1)
        {
            var query = from c in _db.tbl_Celebrity
                        where c.celeb_Status == 2
                        select c;

            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = (IOrderedQueryable<tbl_Celebrity>)query.Where(s => s.celeb_Name.ToLower().Contains(search) || s.celeb_NameEn.ToLower().Contains(search) || s.celeb_Aka.ToLower().Contains(search) || s.celeb_AkaEn.ToLower().Contains(search));
            }
            query = query.Skip((page - 1) * 20).Take(20).OrderByDescending(m => m.celeb_Time);
            List<ManageCelebViewModel> celebs = new List<ManageCelebViewModel>();
            foreach (var item in query)
            {
                celebs.Add(new ManageCelebViewModel(item));
            }
            return View(celebs);
        }

        //
        // GET: /Manage/Celeb/IndexAudit/
        [AdminFilter]
        public ActionResult IndexAudit(string search, int page = 1)
        {
            var query = from m in _db.tbl_Celebrity
                        select m;

            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = (IOrderedQueryable<tbl_Celebrity>)query.Where(s => s.celeb_Name.ToLower().Contains(search) || s.celeb_NameEn.ToLower().Contains(search) || s.celeb_Aka.ToLower().Contains(search) || s.celeb_AkaEn.ToLower().Contains(search));
            }
            query = query.OrderByDescending(m => m.celeb_Time);
            List<ManageCelebViewModel> celebs = new List<ManageCelebViewModel>();
            foreach (var item in query)
            {
                if (!(bool)_db.tbl_UserAccount.SingleOrDefault(u => u.user_Id == item.celeb_Create).user_IsAdmin)
                {
                    celebs.Add(new ManageCelebViewModel(item));
                }
            }
            return View(celebs);
        }
        #endregion

        #region 审核用户上传影人
        //
        // GET: /Manage/Celeb/Audit/
        [AdminFilter]
        public ActionResult Audit(string id, string returnurl)
        {
            if (!MovieManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            CelebManager.Audit(id);
            return RedirectToLocal(returnurl);
        }

        //
        // GET: /Manage/Celeb/Reject/
        [AdminFilter]
        public ActionResult Reject(string id, string returnurl)
        {
            if (!CelebManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            RejectViewModel model = new RejectViewModel();
            model.Id = id;
            ViewBag.ReturnUrl = returnurl;
            return View(model);
        }

        //
        // POST: /Manage/Celeb/Reject/
        [HttpPost]
        [AdminFilter]
        public ActionResult Reject(RejectViewModel model, string returnurl)
        {
            if (model.Note == "0")
            {
                model.Note = "信息有误";
            }
            else if (model.Note == "1")
            {
                model.Note = "已存在";
            }
            else
            {
                model.Note = "其他";
            }
            CelebManager.Reject(model);
            return RedirectToLocal(returnurl);
        }
        #endregion

        #region 创建影人
        //
        // GET: /Manage/Celeb/Create/
        [AdminFilter]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Manage/Celeb/Create/
        [HttpPost]
        [AdminFilter]
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
        // GET: /Manage/Celeb/CreateCeleb/
        [AdminFilter]
        public ActionResult CreateCeleb()
        {
            return View();
        }

        //
        // POST: /Manage/Celeb/CreateCeleb/
        [HttpPost]
        [AdminFilter]
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
            string newId = CelebManager.CreateCeleb(celeb);
            return RedirectToAction("Edit", new { id = newId });
        }
        #endregion

        #region 修改影人
        //
        // GET: /Manage/Celeb/Edit/
        [AdminFilter]
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
        // POST: /Manage/Celeb/Edit/
        [HttpPost]
        [AdminFilter]
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
        // GET: /Manage/Celeb/Delete/
        [AdminFilter]
        public ActionResult Delete(string id)
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
        // Post: /Manage/Celeb/Delete/
        [HttpPost, ActionName("Delete")]
        [AdminFilter]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(string id)
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
            return RedirectToAction("Index");
        }
        #endregion

        #region 更新影人
        //
        // GET: /Manage/Celeb/Refresh/
        [AdminFilter]
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
            TempData["NewCeleb"] = refresh.New;

            return View(refresh);
        }

        //
        // Post: /Manage/Celeb/Refresh/
        [HttpPost, ActionName("Refresh")]
        [AdminFilter]
        [ValidateAntiForgeryToken]
        public ActionResult RefreshConfirm(string id)
        {
            if (!CelebManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            ManageCelebViewModel newceleb = TempData["NewCeleb"] as ManageCelebViewModel;
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = newceleb.Id });
            }
            CelebManager.Edit(newceleb);
            return RedirectToAction("Edit", new { id = newceleb.Id });
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
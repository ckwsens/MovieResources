using MovieResources.Helpers;
using MovieResources.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MovieResources.Controllers
{
    public class AlbumController : Controller
    {
        private MR_DataClassesDataContext _db = new MR_DataClassesDataContext();

        #region 专辑首页
        //
        // GET: /Album/Index/
        public ActionResult Index()
        {
            var allalbum = _db.tbl_Album.OrderByDescending(a => a.album_Visit);
            List<AlbumListItem> model = new List<AlbumListItem>();
            foreach (var item in allalbum)
            {
                AlbumListItem album = new AlbumListItem(item);
                model.Add(album);
            }
            return View(model);
        }
        #endregion

        #region 新建专辑
        //
        // GET: /Album/Create/
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Album/Create/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ManageAlbumViewModel model, System.Web.HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (file != null && file.ContentLength > 0)
            {
                var fileName = System.IO.Path.Combine(Request.MapPath("~/Content/Album/"), System.IO.Path.GetFileName(file.FileName));
                file.SaveAs(fileName);
                model.Cover = System.IO.Path.GetFileName(file.FileName);
            }
            else
            {
                model.Cover = "Album_1.jpg";
            }
            model.UserId = AccountManager.GetId(User.Identity.Name);
            model.Id = AlbumManager.Create(model);
            return RedirectToAction("Detail", new { id = model.Id });
        }
        #endregion

        #region 专辑详情页
        //
        // GET: /Album/Detail/
        public ActionResult Detail(string id, int page = 1)
        {
            if (!AlbumManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }

            tbl_Album tblalbum = _db.tbl_Album.SingleOrDefault(s => s.album_Id == id);
            AlbumViewModel album = new AlbumViewModel(tblalbum);
            if (tblalbum.album_User == AccountManager.GetId(User.Identity.Name))
            {
                album.IsCreator = true;
            }
            if (User.Identity.IsAuthenticated)
            {
                if (_db.tbl_Mark.SingleOrDefault(f => f.mark_Target == id && f.mark_User == AccountManager.GetId(User.Identity.Name) && f.mark_Type == 7) != null)
                {
                    album.HasFollow = true;
                }
            }

            List<AlbumItemViewModel> allItem = new List<AlbumItemViewModel>();
            //album.Items = new List<AlbumItemViewModel>();
            allItem = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AlbumItemViewModel>>(album.ItemJson);
            album.Count = allItem.Count;
            album.Items = allItem.Skip((page - 1) * album.ItemSize).Take(album.ItemSize).ToList();
            foreach (var item in album.Items)
            {
                item.MovieInfo = new MovieViewModel(_db.tbl_Movie.SingleOrDefault(m => m.movie_Id == item.Movie));
            }

            album.Page = page;
            album.PagingCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(album.Count) / album.ItemSize));

            if (page > album.PagingCount && album.Items.Count > 0)
            {
                return RedirectToAction("NotFound", "Error");
            }

            AlbumManager.Visit(id);
            return View(album);
        }
        #endregion

        #region 修改专辑
        //
        // GET: /Album/Edit/
        [Authorize]
        public ActionResult Edit(string id)
        {
            if (!AlbumManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            ManageAlbumViewModel album = new ManageAlbumViewModel(_db.tbl_Album.SingleOrDefault(s => s.album_Id == id));
            return View(album);
        }

        //
        // POST: /Album/Edit/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ManageAlbumViewModel model, System.Web.HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (file != null && file.ContentLength > 0)
            {
                var fileName = System.IO.Path.Combine(Request.MapPath("~/Content/Album/"), System.IO.Path.GetFileName(file.FileName));
                file.SaveAs(fileName);
                model.Cover = System.IO.Path.GetFileName(file.FileName);

                string oldCover = _db.tbl_Album.SingleOrDefault(a => a.album_Id == model.Id).album_Cover;
                if (model.Cover != oldCover && oldCover != "Album_1.jpg")
                {
                    ImageManager.Delete(Server.MapPath("~/Content/Album/" + oldCover));
                }
            }
            AlbumManager.Edit(model);
            ModelState.AddModelError("", "修改成功");
            return RedirectToAction("Edit", new { id = model.Id });
        }
        #endregion

        #region 删除专辑
        //
        // GET: /Album/Delete/
        [Authorize]
        public ActionResult Delete(string id, string returnurl)
        {
            if (!AlbumManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            ViewBag.ReturnUrl = returnurl;
            ManageAlbumViewModel album = new ManageAlbumViewModel(_db.tbl_Album.SingleOrDefault(s => s.album_Id == id));
            return View(album);
        }

        //
        // POST: /Album/Delete/
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(string id)
        {
            if (!AlbumManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }

            string oldCover = _db.tbl_Album.SingleOrDefault(a => a.album_Id == id).album_Cover;
            if (oldCover != "Album_1.jpg")
            {
                ImageManager.Delete(Server.MapPath("~/Content/Album/" + oldCover));
            }
            AlbumManager.Delete(id);
            return RedirectToAction("Index");
        }

        //
        // GET: /Album/Delete/
        public ActionResult CancelDelete(string returnurl)
        {
            return RedirectToLocal(returnurl);
        }
        #endregion

        #region 专辑项目增删
        //
        // POST: /Album/AddItem/
        [HttpPost]
        [Authorize]
        public ActionResult AddItem(string id, string movie, string note, string returnurl)
        {
            AlbumItemViewModel item = new AlbumItemViewModel();
            item.Movie = movie;
            item.Note = note;
            item.Time = DateTime.Now.ToString();
            AlbumManager.Add(id, item);
            return RedirectToLocal(returnurl);
        }

        //
        // GET: /Album/AddItem/
        [Authorize]
        public ActionResult DeleteItem(string id, string movie, string returnurl)
        {
            if (!AlbumManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            AlbumManager.Minus(id, movie);
            return RedirectToLocal(returnurl);
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
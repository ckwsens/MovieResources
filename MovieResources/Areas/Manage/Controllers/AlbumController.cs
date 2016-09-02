using MovieResources.Filters;
using MovieResources.Helpers;
using MovieResources.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MovieResources.Areas.Manage.Controllers
{
    public class AlbumController : Controller
    {
        MR_DataClassesDataContext _db = new MR_DataClassesDataContext();

        #region 专辑管理首页
        //
        // GET: /Manage/Album/
        [AdminFilter]
        public ActionResult Index(string search, int page = 1)
        {
            var query = from a in _db.tbl_Album
                        select a;

            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = (IOrderedQueryable<tbl_Album>)query.Where(s => s.album_Title.ToLower().Contains(search));
            }
            query = query.Skip((page - 1) * 20).Take(20).OrderByDescending(m => m.album_Time);
            List<ManageAlbumViewModel> albums = new List<ManageAlbumViewModel>();
            foreach (var item in query)
            {
                albums.Add(new ManageAlbumViewModel(item));
            }
            return View(albums);
        }
        #endregion

        #region 创建专辑
        //
        // GET: /Manage/Create/
        [AdminFilter]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Manage/Create/
        [HttpPost]
        [AdminFilter]
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
            AlbumManager.Create(model);
            return View(new ManageAlbumViewModel());
        }
        #endregion

        #region 修改专辑
        //
        // GET: /Manage/Edit/
        [AdminFilter]
        public ActionResult Edit(string id)
        {
            if (!AlbumManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            tbl_Album tblalbum = _db.tbl_Album.SingleOrDefault(s => s.album_Id == id);
            ManageAlbumViewModel album = new ManageAlbumViewModel(tblalbum);
            return View(album);
        }

        //
        // POST: /Manage/Edit/
        [HttpPost]
        [AdminFilter]
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
        // GET: /Manage/Delete/
        [AdminFilter]
        public ActionResult Delete(string id)
        {
            if (!AlbumManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            tbl_Album tblalbum = _db.tbl_Album.SingleOrDefault(s => s.album_Id == id);
            ManageAlbumViewModel album = new ManageAlbumViewModel(tblalbum);
            return View(album);
        }

        //
        // Post: /Manage/Delete/
        [HttpPost, ActionName("Delete")]
        [AdminFilter]
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
        #endregion
    }
}
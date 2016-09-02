using MovieResources.Helpers;
using MovieResources.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MovieResources.Controllers
{
    public class MovieController : Controller
    {
        private MR_DataClassesDataContext _db = new MR_DataClassesDataContext();

        #region 电影详情页
        ////
        //// GET: /Movie/IndexNew/
        public ActionResult Index(string id)
        {
            if (!MovieManager.Exist(id))
            {
                //return HttpNotFound();
                return RedirectToAction("NotFound", "Error");
            }
            tbl_Movie tblmovie = _db.tbl_Movie.SingleOrDefault(s => s.movie_Id == id);
            if (tblmovie == null)
            {
                //return HttpNotFound();
                return RedirectToAction("NotFound", "Error");
            }
            MovieViewModel movie = new MovieViewModel(tblmovie);
            if (User.Identity.IsAuthenticated)
            {
                movie.IsPlan = MarkManager.Validate(tblmovie.movie_Id, AccountManager.GetId(User.Identity.Name), 1);
                movie.IsFinish = MarkManager.Validate(tblmovie.movie_Id, AccountManager.GetId(User.Identity.Name), 2);
                movie.IsFavor = MarkManager.Validate(tblmovie.movie_Id, AccountManager.GetId(User.Identity.Name), 3);

                movie.PlanCount = _db.tbl_Mark.Where(m => m.mark_Target == id && m.mark_Type == 1).Count();
                movie.FinishCount = _db.tbl_Mark.Where(m => m.mark_Target == id && m.mark_Type == 2).Count();
                movie.FavorCount = _db.tbl_Mark.Where(m => m.mark_Target == id && m.mark_Type == 3).Count();

                if (tblmovie.movie_Create == AccountManager.GetId(User.Identity.Name) || (bool)_db.tbl_UserAccount.SingleOrDefault(a => a.user_Account == User.Identity.Name).user_IsAdmin)
                {
                    movie.IsCreate = true;
                }
                var cmt = _db.tbl_Comment.SingleOrDefault(c => c.cmt_Movie == id && c.cmt_User == AccountManager.GetId(User.Identity.Name));
                if (cmt != null)
                {
                    movie.MyComment = new CommentViewModel(cmt);
                }
            }

            var ress = _db.tbl_Resource.Where(m => m.res_Movie == movie.Id && m.res_Status == 2);
            movie.Resources = new List<ResViewModel>();
            foreach (var item in ress)
            {
                movie.Resources.Add(new ResViewModel(item));
            }

            var allcmt = _db.tbl_Comment.Where(c => c.cmt_Movie == id).OrderByDescending(c => c.cmt_Time).Take(10);
            movie.Comments = new List<CommentViewModel>();
            foreach (var item in allcmt)
            {
                movie.Comments.Add(new CommentViewModel(item));
            }
            movie.CommentCount = _db.tbl_Comment.Where(c => c.cmt_Movie == id).Count();

            var albums = _db.tbl_Album.Where(a => a.album_Item.Contains(movie.Id)).OrderByDescending(a => a.album_Time).Take(15);
            movie.Albums = new List<LinkItem>();
            foreach (var item in albums)
            {
                movie.Albums.Add(new LinkItem() { Id = item.album_Id, Title = item.album_Title });
            }

            MovieManager.Visit(id);
            return View(movie);
        }
        #endregion

        #region 创建电影
        //
        // GET: /Movie/Create/
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Movie/Create/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateMovieViewModel movie)
        {
            if (!ModelState.IsValid)
            {
                return View(movie);
            }
            foreach (var item in movie.DoubanID.Split('\n'))
            {
                if (item.Trim().Length == 0)
                {
                    continue;
                }
                JObject json = HtmlDecoder.GetJson("https://api.douban.com/v2/movie/subject/" + item.Replace("https://movie.douban.com/subject/", "").Replace("/", ""));
                JToken msg;
                if (json.TryGetValue("msg", out msg))
                {
                    ModelState.AddModelError("", string.Format("{0} {1} {2}", "添加编号为", item, "的电影 失败"));
                }
                else
                {
                    ModelState.AddModelError("", string.Format("{0} {1} {2}", "添加编号为", item, "的电影 成功"));
                    MovieManager.CreateJson(json, Server.MapPath("~/Content/Movie/"), AccountManager.GetId(User.Identity.Name));
                }
            }
            return View();
        }

        //
        // GET: /Movie/CreateMovie/
        [Authorize]
        public ActionResult CreateMovie()
        {
            return View();
        }

        //
        // POST: /Movie/CreateMovie/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMovie(ManageMovieViewModel movie, System.Web.HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(movie);
            }
            if (file != null && file.ContentLength > 0)
            {
                var fileName = System.IO.Path.Combine(Request.MapPath("~/Content/Movie/"), System.IO.Path.GetFileName(file.FileName));
                file.SaveAs(fileName);
                movie.Avatar = System.IO.Path.GetFileName(file.FileName);
            }
            MR_DataClassesDataContext _db = new MR_DataClassesDataContext();
            if ((bool)_db.tbl_UserAccount.SingleOrDefault(u => u.user_Account == User.Identity.Name).user_IsAdmin)
            {
                movie.Status = 2;
            }
            else
            {
                movie.Status = 0;
            }
            movie.Create = AccountManager.GetId(User.Identity.Name);
            string newId = MovieManager.CreateMovie(movie);
            return RedirectToAction("Index", new { id = newId });
        }
        #endregion

        #region 修改电影
        //
        // GET: /Movie/Edit/
        [Authorize]
        public ActionResult Edit(string id)
        {
            if (!MovieManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            tbl_Movie tblmovie = _db.tbl_Movie.SingleOrDefault(s => s.movie_Id == id);
            ManageMovieViewModel movie = new ManageMovieViewModel(tblmovie);
            return View(movie);
        }

        //
        // POST: /Movie/Edit/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ManageMovieViewModel model, System.Web.HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (file != null && file.ContentLength > 0)
            {
                var fileName = System.IO.Path.Combine(Request.MapPath("~/Content/Movie/"), System.IO.Path.GetFileName(file.FileName));
                file.SaveAs(fileName);
                model.Avatar = System.IO.Path.GetFileName(file.FileName);
                string oldAvatar = _db.tbl_Movie.SingleOrDefault(m => m.movie_Id == model.Id).movie_Avatar;
                if (oldAvatar != "Movie_Default.png")
                {
                    ImageManager.Delete(Server.MapPath("~/Content/Movie/" + oldAvatar));
                }
            }
            else
            {
                MR_DataClassesDataContext _db = new MR_DataClassesDataContext();
                model.Avatar = _db.tbl_Movie.SingleOrDefault(m => m.movie_Id == model.Id).movie_Avatar;
            }
            MovieManager.Edit(model);
            return RedirectToAction("Index", new { id = model.Id });
        }
        #endregion

        #region 更新电影
        //
        // GET: /Movie/Refresh/
        [Authorize]
        public ActionResult Refresh(string id)
        {
            if (!MovieManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            RefreshMovieViewModel refresh = new RefreshMovieViewModel();

            tbl_Movie tblmovie = _db.tbl_Movie.SingleOrDefault(s => s.movie_Id == id);
            refresh.Old = new ManageMovieViewModel(tblmovie);

            JObject json = HtmlDecoder.GetJson("https://api.douban.com/v2/movie/subject/" + tblmovie.movie_DoubanID);
            JToken msg;
            if (json.TryGetValue("msg", out msg))
            {
                refresh.New = new ManageMovieViewModel();
                refresh.New.Id = refresh.Old.Id;
            }
            else
            {
                tblmovie = MovieManager.JsonToMovie(json, Server.MapPath("~/Content/Movie/"));
                refresh.New = new ManageMovieViewModel(tblmovie);
                refresh.New.Id = refresh.Old.Id;
            }
            TempData["New"] = refresh.New;

            return View(refresh);
        }

        //
        // Post: /Movie/Refresh/
        [HttpPost, ActionName("Refresh")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult RefreshConfirm(string id)
        {
            if (!MovieManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            ManageMovieViewModel movie = TempData["New"] as ManageMovieViewModel;
            MovieManager.Edit(movie);
            return RedirectToAction("Index", new { id = movie.Id });
        }
        #endregion

        #region 删除电影
        //
        // GET: /Movie/Delete/
        [Authorize]
        public ActionResult Delete(string id, string returnurl)
        {
            if (!MovieManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            ManageMovieViewModel movie = new ManageMovieViewModel(_db.tbl_Movie.SingleOrDefault(s => s.movie_Id == id));
            ViewBag.ReturnUrl = returnurl;
            return View(movie);
        }

        //
        // Post: /Movie/Delete/
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(string id, string returnurl)
        {
            if (!MovieManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }

            string oldAvatar = _db.tbl_Movie.SingleOrDefault(m => m.movie_Id == id).movie_Avatar;
            if (oldAvatar != "Movie_Default.png")
            {
                ImageManager.Delete(Server.MapPath("~/Content/Movie/" + oldAvatar));
            }
            MovieManager.Delete(id);
            return Redirect(returnurl);
        }
        #endregion

        #region 下载bt种子文件
        //
        // GET: /Movie/DownTorrent/
        public ActionResult DownTorrent(string filename)
        {
            var path = Server.MapPath("~/Content/Torrent/" + filename);
            var name = System.IO.Path.GetFileName(path);
            return File(path, "application/zip-x-compressed", name);
        }
        #endregion

        #region 全部评论页
        //
        // GET: /Movie/Comment/
        public ActionResult Comment(string id)
        {
            if (!MovieManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            MovieViewModel movie = new MovieViewModel(_db.tbl_Movie.SingleOrDefault(s => s.movie_Id == id));

            var allcmt = _db.tbl_Comment.Where(c => c.cmt_Movie == id).OrderByDescending(c => c.cmt_Time);
            movie.CommentCount = allcmt.Count();
            movie.Comments = new List<CommentViewModel>();
            foreach (var item in allcmt)
            {
                movie.Comments.Add(new CommentViewModel(item));
            }
            return View(movie);
        }
        #endregion

    }
}
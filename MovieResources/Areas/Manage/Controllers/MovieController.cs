using MovieResources.Filters;
using MovieResources.Helpers;
using MovieResources.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MovieResources.Areas.Manage.Controllers
{
    public class MovieController : Controller
    {
        MR_DataClassesDataContext _db = new MR_DataClassesDataContext();

        #region 电影管理首页
        //
        // GET: /Manage/Movie/
        [AdminFilter]
        public ActionResult Index(string search, int page = 1)
        {
            var query = from m in _db.tbl_Movie
                        where m.movie_Status == 2
                        select m;

            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = (IOrderedQueryable<tbl_Movie>)query.Where(m => m.movie_Title.ToLower().Contains(search) || m.movie_TitleEn.ToLower().Contains(search) || m.movie_Aka.ToLower().Contains(search));
            }
            query = query.OrderByDescending(m => m.movie_Time);
            List<ManageMovieViewModel> movies = new List<ManageMovieViewModel>();
            foreach (var item in query)
            {
                movies.Add(new ManageMovieViewModel(item));
            }
            return View(movies);
        }

        //
        // GET: /Manage/Movie/IndexAudit/
        [AdminFilter]
        public ActionResult IndexAudit(string search, int page = 1)
        {
            var query = from m in _db.tbl_Movie
                        select m;

            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = (IOrderedQueryable<tbl_Movie>)query.Where(s => s.movie_Title.ToLower().Contains(search) || s.movie_TitleEn.ToLower().Contains(search) || s.movie_Aka.ToLower().Contains(search));
            }
            query = query.OrderByDescending(m => m.movie_Time);
            List<ManageMovieViewModel> movies = new List<ManageMovieViewModel>();
            foreach (var item in query)
            {
                if (!(bool)_db.tbl_UserAccount.SingleOrDefault(u => u.user_Id == item.movie_Create).user_IsAdmin)
                {
                    movies.Add(new ManageMovieViewModel(item));
                }
            }
            return View(movies);
        }
        #endregion

        #region 审核用户上传电影
        //
        // GET: /Manage/Movie/Audit/
        [AdminFilter]
        public ActionResult Audit(string id, string returnurl)
        {
            if (!MovieManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            MovieManager.Audit(id);
            return RedirectToLocal(returnurl);
        }

        //
        // GET: /Manage/Movie/Reject/
        [AdminFilter]
        public ActionResult Reject(string id, string returnurl)
        {
            if (!MovieManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            RejectViewModel model = new RejectViewModel();
            model.Id = id;
            ViewBag.ReturnUrl = returnurl;
            return View(model);
        }

        //
        // POST: /Manage/Movie/Reject/
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
            MovieManager.Reject(model);
            return RedirectToLocal(returnurl);
        }
        #endregion

        #region 创建电影
        //
        // GET: /Manage/Movie/Create/
        [AdminFilter]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Manage/Movie/Create/
        [HttpPost]
        [AdminFilter]
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
        // GET: /Manage/Movie/CreateMovie/
        [AdminFilter]
        public ActionResult CreateMovie()
        {
            return View();
        }

        //
        // POST: /Manage/Movie/CreateMovie/
        [HttpPost]
        [AdminFilter]
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
            movie.Create = AccountManager.GetId(User.Identity.Name);
            movie.Status = 2;
            string newId = MovieManager.CreateMovie(movie);
            return RedirectToAction("Edit", new { id = newId });
        }
        #endregion

        #region 修改电影
        //
        // GET: /Manage/Movie/Edit/
        [AdminFilter]
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
        // POST: /Manage/Movie/Edit/
        [HttpPost]
        [AdminFilter]
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
            MovieManager.Edit(model);
            return RedirectToAction("Edit", new { id = model.Id });
        }
        #endregion

        #region 删除电影
        //
        // GET: /Manage/Movie/Delete/
        [AdminFilter]
        public ActionResult Delete(string id)
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
        // Post: /Manage/Movie/Delete/
        [HttpPost, ActionName("Delete")]
        [AdminFilter]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(string id)
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
            return RedirectToAction("Index");
        }
        #endregion

        #region 更新电影
        //
        // GET: /Manage/Movie/Refresh/
        [AdminFilter]
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
            TempData["NewMovie"] = refresh.New;

            return View(refresh);
        }

        //
        // Post: /Manage/Movie/Refresh/
        [HttpPost, ActionName("Refresh")]
        [AdminFilter]
        [ValidateAntiForgeryToken]
        public ActionResult RefreshConfirm(string id)
        {
            if (!MovieManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            ManageMovieViewModel newmovie = TempData["NewMovie"] as ManageMovieViewModel;
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = newmovie.Id });
            }
            MovieManager.Edit(newmovie);
            return RedirectToAction("Edit", new { id = newmovie.Id });
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
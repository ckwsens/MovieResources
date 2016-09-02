using MovieResources.Helpers;
using MovieResources.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieResources.Controllers
{
    public class MineController : Controller
    {
        MR_DataClassesDataContext _db = new MR_DataClassesDataContext();

        #region 用户主页
        //
        // GET: /Mine/Index/
        [Authorize]
        public ActionResult Index()
        {
            MineViewModel model = new MineViewModel();
            model.Account = User.Identity.Name;
            model.Avatar = _db.tbl_UserAccount.Single(m => m.user_Account == model.Account).user_Avatar;
            model.Cover = _db.tbl_UserAccount.Single(m => m.user_Account == model.Account).user_Cover;
            model.Id = AccountManager.GetId(model.Account);

            var plans = _db.tbl_Mark.Where(m => m.mark_User == model.Id && m.mark_Type == 1).OrderByDescending(m => m.mark_Time);
            model.PlanCount = plans.Count();
            model.MoviePlans = new List<MarkItem>();
            if (model.PlanCount != 0)
            {
                foreach (var item in plans.Take(10))
                {
                    MarkItem mitem = new MarkItem();
                    mitem.Id = item.mark_Target;
                    mitem.Avatar = _db.tbl_Movie.Single(m => m.movie_Id == item.mark_Target).movie_Avatar;
                    mitem.Title = _db.tbl_Movie.Single(m => m.movie_Id == item.mark_Target).movie_Title;
                    model.MoviePlans.Add(mitem);
                }
            }

            var finishs = _db.tbl_Mark.Where(m => m.mark_User == model.Id && m.mark_Type == 2).OrderByDescending(m => m.mark_Time);
            model.FinishCount = finishs.Count();
            model.MovieFinishs = new List<MarkItem>();
            if (model.FinishCount != 0)
            {
                foreach (var item in finishs.Take(10))
                {
                    MarkItem mitem = new MarkItem();
                    mitem.Id = item.mark_Target;
                    mitem.Avatar = _db.tbl_Movie.Single(m => m.movie_Id == item.mark_Target).movie_Avatar;
                    mitem.Title = _db.tbl_Movie.Single(m => m.movie_Id == item.mark_Target).movie_Title;
                    model.MovieFinishs.Add(mitem);
                }
            }

            var favors = _db.tbl_Mark.Where(m => m.mark_User == model.Id && m.mark_Type == 3).OrderByDescending(m => m.mark_Time);
            model.FavorCount = favors.Count();
            model.MovieFavors = new List<MarkItem>();
            if (model.FavorCount != 0)
            {
                foreach (var item in favors.Take(10))
                {
                    MarkItem mitem = new MarkItem();
                    mitem.Id = item.mark_Target;
                    mitem.Avatar = _db.tbl_Movie.Single(m => m.movie_Id == item.mark_Target).movie_Avatar;
                    mitem.Title = _db.tbl_Movie.Single(m => m.movie_Id == item.mark_Target).movie_Title;
                    model.MovieFavors.Add(mitem);
                }
            }

            var collects = _db.tbl_Mark.Where(m => m.mark_User == model.Id && m.mark_Type == 4).OrderByDescending(m => m.mark_Time);
            model.CollectCount = collects.Count();
            model.CelebCollects = new List<MarkItem>();
            if (model.CollectCount != 0)
            {
                foreach (var item in collects.Take(10))
                {
                    MarkItem mitem = new MarkItem();
                    mitem.Id = item.mark_Target;
                    mitem.Avatar = _db.tbl_Celebrity.Single(m => m.celeb_Id == item.mark_Target).celeb_Avatar;
                    mitem.Title = _db.tbl_Celebrity.Single(m => m.celeb_Id == item.mark_Target).celeb_Name;
                    model.CelebCollects.Add(mitem);
                }
            }

            var albums = _db.tbl_Album.Where(a => a.album_User == model.Id).OrderByDescending(a => a.album_AlterTime);
            model.AlbumCount = albums.Count();
            model.Albums = new List<AlbumListItem>();
            if (model.AlbumCount != 0)
            {
                foreach (var item in albums)
                {
                    model.Albums.Add(new AlbumListItem(item));
                }
            }

            var comments = _db.tbl_Comment.Where(c => c.cmt_User == model.Id).OrderByDescending(c => c.cmt_Time);
            model.CommentCount = comments.Count();
            model.Comments = new List<CommentItem>();
            if (model.AlbumCount != 0)
            {
                foreach (var item in comments)
                {
                    CommentItem citem = new CommentItem();
                    citem.MovieID = item.cmt_Movie;
                    citem.MovieTitle = MovieManager.GetTitle(item.cmt_Movie);
                    citem.Comment = item.cmt_Content;
                    citem.Time = ((System.DateTime)item.cmt_Time).ToString("yyyy-MM-dd hh:mm:ss");
                    model.Comments.Add(citem);
                }
            }

            return View(model);
        }
        #endregion

        #region 用户基本信息修改
        //
        // GET: /Mine/ChangeAvatar/
        [Authorize]
        public ActionResult ChangeAvatar()
        {
            ChangeAvatarViewModel model = new ChangeAvatarViewModel();
            model.Account = User.Identity.Name;
            model.Avatar = _db.tbl_UserAccount.Single(m => m.user_Account == model.Account).user_Avatar;

            return View(model);
        }

        //
        // POST: /Mine/ChangeAvatar/
        [Authorize]
        [HttpPost]
        public ActionResult ChangeAvatar(ChangeAvatarViewModel model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Id = _db.tbl_UserAccount.Single(m => m.user_Account == User.Identity.Name).user_Id;

            if (file != null && file.ContentLength > 0)
            {
                var fileName = System.IO.Path.Combine(Request.MapPath("~/Content/User/Avatar/"), model.Id + System.IO.Path.GetFileName(file.FileName));
                file.SaveAs(fileName);
                model.Avatar = model.Id + System.IO.Path.GetFileName(file.FileName);
                string oldAvatar = _db.tbl_UserAccount.Single(m => m.user_Account == User.Identity.Name).user_Avatar;
                if (model.Avatar != oldAvatar && oldAvatar != "User_1.jpg")
                {
                    ImageManager.Delete(Server.MapPath("~/Content/User/Avatar/" + oldAvatar));
                }
            }
            else
            {
                ModelState.AddModelError("", "请选择一张图片");
                model.Avatar = _db.tbl_UserAccount.Single(m => m.user_Account == User.Identity.Name).user_Avatar;
                return View(model);
            }
            var result = AccountManager.ChangeAvatar(model);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Error);
            }
            return View(model);
        }

        //
        // GET: /Mine/ChangePassword/
        [Authorize]
        public ActionResult ChangePassword()
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            model.Account = User.Identity.Name;
            model.Avatar = _db.tbl_UserAccount.Single(m => m.user_Account == model.Account).user_Avatar;
            return View(model);
        }

        //
        // POST: /Mine/ChangePassword/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = AccountManager.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                AccountManager.SignIn(User.Identity.Name);
            }
            ModelState.AddModelError("", result.Error);
            return View(model);
        }

        //
        // GET: /Mine/ChangeCover/
        [Authorize]
        public ActionResult ChangeCover()
        {
            ChangeCoverViewModel model = new ChangeCoverViewModel();
            model.Account = User.Identity.Name;
            model.Cover = _db.tbl_UserAccount.Single(m => m.user_Account == model.Account).user_Cover;
            model.Avatar = _db.tbl_UserAccount.Single(m => m.user_Account == model.Account).user_Avatar;

            return View(model);
        }

        //
        // POST: /Mine/ChangeCover/
        [Authorize]
        [HttpPost]
        public ActionResult ChangeCover(ChangeCoverViewModel model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Id = _db.tbl_UserAccount.Single(m => m.user_Account == User.Identity.Name).user_Id;

            if (file != null && file.ContentLength > 0)
            {
                var fileName = System.IO.Path.Combine(Request.MapPath("~/Content/User/Cover/"), model.Id + System.IO.Path.GetFileName(file.FileName));
                file.SaveAs(fileName);
                model.Cover = model.Id + System.IO.Path.GetFileName(file.FileName);
                string oldCover = _db.tbl_UserAccount.Single(m => m.user_Account == User.Identity.Name).user_Cover;
                if (model.Cover != oldCover && oldCover != "Cover_1.jpg" && oldCover != "Cover_2.jpg")
                {
                    ImageManager.Delete(Server.MapPath("~/Content/User/Cover/" + oldCover));
                }
            }
            else
            {
                ModelState.AddModelError("", "请选择一张图片");
                model.Cover = _db.tbl_UserAccount.Single(m => m.user_Account == User.Identity.Name).user_Avatar;
                return View(model);
            }
            var result = AccountManager.ChangeCover(model);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Error);
            }
            return View(model);
        }
        #endregion

        #region 用户电影、影人、资源、求资源、专辑主页
        //
        // GET: /Mine/MineMovie/
        [Authorize]
        public ActionResult MineMovie(int type)
        {
            if (type > 4 || type < 1)
            {
                return RedirectToAction("NotFound", "Error");
            }
            ViewBag.MovieType = type;

            MineMovieViewModel minemovie = new MineMovieViewModel();
            minemovie.Account = User.Identity.Name;
            minemovie.Id = AccountManager.GetId(minemovie.Account);
            minemovie.Avatar = _db.tbl_UserAccount.SingleOrDefault(a => a.user_Id == minemovie.Id).user_Avatar;

            minemovie.PlanCount = _db.tbl_Mark.Where(m => m.mark_User == minemovie.Id && m.mark_Type == 1).Count();
            minemovie.FinishCount = _db.tbl_Mark.Where(m => m.mark_User == minemovie.Id && m.mark_Type == 2).Count();
            minemovie.FavorCount = _db.tbl_Mark.Where(m => m.mark_User == minemovie.Id && m.mark_Type == 3).Count();
            minemovie.CreateCount = _db.tbl_Movie.Where(m => m.movie_Create == minemovie.Id).Count();

            minemovie.Moives = new List<MovieViewModel>();
            if (type == 1 || type == 2 || type == 3)
            {
                var movies = _db.tbl_Mark.Where(m => m.mark_User == minemovie.Id && m.mark_Type == type).OrderByDescending(m => m.mark_Time);
                foreach (var item in movies)
                {
                    MovieViewModel movie = new MovieViewModel(_db.tbl_Movie.Single(m => m.movie_Id == item.mark_Target));
                    movie.IsPlan = MarkManager.Validate(movie.Id, AccountManager.GetId(User.Identity.Name), 1);
                    movie.IsFinish = MarkManager.Validate(movie.Id, AccountManager.GetId(User.Identity.Name), 2);
                    movie.IsFavor = MarkManager.Validate(movie.Id, AccountManager.GetId(User.Identity.Name), 3);
                    minemovie.Moives.Add(movie);
                }
            }
            else
            {
                var movies = _db.tbl_Movie.Where(m => m.movie_Create == minemovie.Id).OrderByDescending(m => m.movie_Time);
                foreach (var item in movies)
                {
                    MovieViewModel movie = new MovieViewModel(item);
                    movie.IsPlan = MarkManager.Validate(movie.Id, AccountManager.GetId(User.Identity.Name), 1);
                    movie.IsFinish = MarkManager.Validate(movie.Id, AccountManager.GetId(User.Identity.Name), 2);
                    movie.IsFavor = MarkManager.Validate(movie.Id, AccountManager.GetId(User.Identity.Name), 3);
                    minemovie.Moives.Add(movie);
                }
            }
            return View(minemovie);
        }

        //
        // GET: /Mine/MarkedCeleb/
        [Authorize]
        public ActionResult MineCeleb(int type)
        {
            if (type > 2 || type < 1)
            {
                return RedirectToAction("NotFound", "Error");
            }
            ViewBag.CelebType = type;

            MineCelebViewModel mineceleb = new MineCelebViewModel();
            mineceleb.Account = User.Identity.Name;
            mineceleb.Id = AccountManager.GetId(mineceleb.Account);
            mineceleb.Avatar = _db.tbl_UserAccount.SingleOrDefault(a => a.user_Id == mineceleb.Id).user_Avatar;

            var collects = _db.tbl_Mark.Where(m => m.mark_User == mineceleb.Id && m.mark_Type == 4).OrderByDescending(m => m.mark_Time);
            mineceleb.CollectCount = collects.Count();
            var creates = _db.tbl_Celebrity.Where(c => c.celeb_Create == mineceleb.Id).OrderByDescending(c => c.celeb_Time);
            mineceleb.CreateCount = creates.Count();

            mineceleb.Celebs = new List<CelebViewModel>();
            if (type == 1)
            {
                foreach (var item in collects)
                {
                    CelebViewModel movie = new CelebViewModel(_db.tbl_Celebrity.Single(m => m.celeb_Id == item.mark_Target));
                    mineceleb.Celebs.Add(movie);
                }
            }
            else
            {
                foreach (var item in creates)
                {
                    CelebViewModel movie = new CelebViewModel(item);
                    mineceleb.Celebs.Add(movie);
                }
            }
            return View(mineceleb);
        }

        //
        // GET: /Mine/MineRes/
        [Authorize]
        public ActionResult MineRes()
        {
            MineResViewModel mineres = new MineResViewModel();
            mineres.Account = User.Identity.Name;
            mineres.Id = AccountManager.GetId(mineres.Account);
            mineres.Avatar = _db.tbl_UserAccount.SingleOrDefault(a => a.user_Id == mineres.Id).user_Avatar;

            var ress = _db.tbl_Resource.Where(r => r.res_User == mineres.Id).OrderByDescending(r => r.res_Time);
            mineres.ResCount = ress.Count();
            var asks = _db.tbl_Ask.Where(a => a.ask_User == mineres.Id).OrderByDescending(m => m.ask_Time);
            mineres.AskCount = asks.Count();

            mineres.Ress = new List<ResViewModel>();
            foreach (var item in ress)
            {
                mineres.Ress.Add(new ResViewModel(item));
            }
            return View(mineres);
        }

        //
        // GET: /Mine/MineAsk/
        [Authorize]
        public ActionResult MineAsk()
        {
            MineAskViewModel mineask = new MineAskViewModel();
            mineask.Account = User.Identity.Name;
            mineask.Id = AccountManager.GetId(mineask.Account);
            mineask.Avatar = _db.tbl_UserAccount.SingleOrDefault(a => a.user_Id == mineask.Id).user_Avatar;

            var asks = _db.tbl_Ask.Where(a => a.ask_User == mineask.Id).OrderByDescending(m => m.ask_Time);
            mineask.AskCount = asks.Count();
            var ress = _db.tbl_Resource.Where(r => r.res_User == mineask.Id).OrderByDescending(r => r.res_Time);
            mineask.ResCount = ress.Count();

            mineask.Asks = new List<AskViewModel>();
            foreach (var item in asks)
            {
                AskViewModel ask = new AskViewModel(_db.tbl_Ask.Single(a => a.ask_Id == item.ask_Id));
                mineask.Asks.Add(ask);
            }
            return View(mineask);
        }

        //
        // GET: /Mine/MineAlbum/
        [Authorize]
        public ActionResult MineAlbum(int type)
        {
            if (type > 2 || type < 1)
            {
                return RedirectToAction("NotFound", "Error");
            }
            MineAlbumViewModel minealbum = new MineAlbumViewModel();
            minealbum.Account = User.Identity.Name;
            minealbum.Id = AccountManager.GetId(minealbum.Account);
            minealbum.Avatar = _db.tbl_UserAccount.SingleOrDefault(a => a.user_Id == minealbum.Id).user_Avatar;
            ViewBag.AlbumType = type;

            var albums = _db.tbl_Album.Where(a => a.album_User == minealbum.Id).OrderByDescending(m => m.album_Time);
            minealbum.AlbumCount = albums.Count();
            var follows = _db.tbl_Mark.Where(f => f.mark_User == minealbum.Id && f.mark_Type == 7).OrderByDescending(r => r.mark_Time);
            minealbum.FollowCount = follows.Count();

            minealbum.Albums = new List<AlbumViewModel>();
            if (type == 1)
            {
                foreach (var item in albums)
                {
                    minealbum.Albums.Add(new AlbumViewModel(item));
                }
            }
            else if (type == 2)
            {
                foreach (var item in follows)
                {
                    var album = _db.tbl_Album.SingleOrDefault(a => a.album_Id == item.mark_Target);
                    minealbum.Albums.Add(new AlbumViewModel(album));
                }
            }
            return View(minealbum);
        }
        #endregion

        #region 帮助程序
        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }
        #endregion
    }
}
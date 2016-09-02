using MovieResources.Helpers;
using MovieResources.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MovieResources.Controllers
{
    public class PeopleController : Controller
    {
        //
        // GET: /People/Index/
        public ActionResult Index(string id, string returnurl)
        {
            if (!AccountManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            MR_DataClassesDataContext _db = new MR_DataClassesDataContext();
            PeopleViewModel model = new PeopleViewModel();
            model.Id = id;
            model.Avatar = _db.tbl_UserAccount.Single(m => m.user_Id == model.Id).user_Avatar;
            model.Cover = _db.tbl_UserAccount.Single(m => m.user_Id == model.Id).user_Cover;
            model.Account = _db.tbl_UserAccount.Single(m => m.user_Id == model.Id).user_Account;

            if ((bool)_db.tbl_UserAccount.Single(m => m.user_Id == model.Id).user_IsAdmin)
            {
                return Redirect(returnurl);
            }

            if (User.Identity.IsAuthenticated)
            {
                string userid = AccountManager.GetId(User.Identity.Name);
                if (userid == id)
                {
                    return RedirectToAction("Index", "Mine");
                }
                model.MovieCommons = new List<MarkItem>();
                var peoplefavors = _db.tbl_Mark.Where(m => m.mark_User == model.Id && m.mark_Type == 3);
                foreach (var item in peoplefavors)
                {
                    if (_db.tbl_Mark.SingleOrDefault(m => m.mark_Target == item.mark_Target && m.mark_User == userid && m.mark_Type == 3) != null)
                    {
                        MarkItem mitem = new MarkItem();
                        mitem.Id = item.mark_Target;
                        mitem.Avatar = _db.tbl_Movie.Single(m => m.movie_Id == item.mark_Target).movie_Avatar;
                        mitem.Title = _db.tbl_Movie.Single(m => m.movie_Id == item.mark_Target).movie_Title;
                        model.MovieCommons.Add(mitem);
                    }
                }
                model.CommonCount = model.MovieCommons.Count;
                model.MovieCommons = model.MovieCommons.Take(10).ToList();
            }
            else
            {
                model.MovieCommons = new List<MarkItem>();
                model.CommonCount = 0;
            }

            var plans = _db.tbl_Mark.Where(m => m.mark_User == model.Id && m.mark_Type == 1).OrderByDescending(m => m.mark_Time).Take(10);
            model.PlanCount = _db.tbl_Mark.Where(m => m.mark_User == model.Id && m.mark_Type == 1).OrderByDescending(m => m.mark_Time).Count();
            model.MoviePlans = new List<MarkItem>();
            if (model.PlanCount != 0)
            {
                foreach (var item in plans)
                {
                    MarkItem mitem = new MarkItem();
                    mitem.Id = item.mark_Target;
                    mitem.Avatar = _db.tbl_Movie.Single(m => m.movie_Id == item.mark_Target).movie_Avatar;
                    mitem.Title = _db.tbl_Movie.Single(m => m.movie_Id == item.mark_Target).movie_Title;
                    model.MoviePlans.Add(mitem);
                }
            }

            var finishs = _db.tbl_Mark.Where(m => m.mark_User == model.Id && m.mark_Type == 2).OrderByDescending(m => m.mark_Time).Take(10);
            model.FinishCount = _db.tbl_Mark.Where(m => m.mark_User == model.Id && m.mark_Type == 2).OrderByDescending(m => m.mark_Time).Count();
            model.MovieFinishs = new List<MarkItem>();
            if (model.FinishCount != 0)
            {
                foreach (var item in finishs)
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
    }
}
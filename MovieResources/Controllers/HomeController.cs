using MovieResources.Helpers;
using MovieResources.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MovieResources.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/Index/
        public ActionResult Index(int offset = 0)
        {
            HomeViewModel model = new HomeViewModel();
            MR_DataClassesDataContext _db = new MR_DataClassesDataContext();

            var dis = _db.tbl_Discovery.OrderByDescending(d => d.disc_Flag).ToList();
            if (offset >= dis.Count || offset < 0)
            {
                return Redirect("/Home/Index?offset=0");
            }
            model.Discovery = new HomeDiscViewModel(dis[offset]);
            model.Discovery.Offset = offset;
            model.Discovery.Post = offset - 1;
            model.Discovery.Pre = offset + 1;
            if (User.Identity.IsAuthenticated)
            {
                model.Discovery.Movie.IsPlan = MarkManager.Validate(model.Discovery.Movie.Id, AccountManager.GetId(User.Identity.Name), 1);
                model.Discovery.Movie.IsFinish = MarkManager.Validate(model.Discovery.Movie.Id, AccountManager.GetId(User.Identity.Name), 2);
                model.Discovery.Movie.IsFavor = MarkManager.Validate(model.Discovery.Movie.Id, AccountManager.GetId(User.Identity.Name), 3);
            }

            var newmovie = _db.tbl_Movie.Where(m => m.movie_Status == 2).OrderByDescending(m => m.movie_Time).ToList().Take(20);
            model.News = new List<MovieListViewModel>();
            foreach (var item in newmovie)
            {
                MovieListViewModel movie = new MovieListViewModel();
                movie.Title = item.movie_Title;
                if (!string.IsNullOrEmpty(item.movie_TitleEn) && !string.IsNullOrWhiteSpace(item.movie_TitleEn))
                {
                    movie.Title += "\t" + item.movie_TitleEn;
                }
                movie.Year = item.movie_Year;
                movie.Id = item.movie_Id;
                model.News.Add(movie);
            }

            var mostmovie = _db.tbl_Movie.Where(m => m.movie_Status == 2).OrderByDescending(m => m.movie_VisitCount).ToList().Take(20);
            model.Mosts = new List<MovieListViewModel>();
            foreach (var item in mostmovie)
            {
                MovieListViewModel movie = new MovieListViewModel();
                movie.Title = item.movie_Title;
                if (!string.IsNullOrEmpty(item.movie_TitleEn) && !string.IsNullOrWhiteSpace(item.movie_TitleEn))
                {
                    movie.Title += "\t" + item.movie_TitleEn;
                }
                movie.Year = item.movie_Year;
                movie.Id = item.movie_Id;
                model.Mosts.Add(movie);
            }

            return View(model);
        }

        //
        // GET: /Home/Test/
        public ActionResult Test()
        {
            MR_DataClassesDataContext _db = new MR_DataClassesDataContext();

            foreach (var celeb in _db.tbl_Celebrity)
            {
                if (celeb.celeb_DoubanID != null)
                    foreach (var item in _db.tbl_Movie)
                    {
                        if (item.movie_DirectorsId != null && item.movie_DirectorsId.Contains(celeb.celeb_DoubanID))
                            item.movie_DirectorsId.Replace(celeb.celeb_DoubanID, celeb.celeb_Id);
                        if (item.movie_WritersId != null && item.movie_WritersId.Contains(celeb.celeb_DoubanID))
                            item.movie_WritersId.Replace(celeb.celeb_DoubanID, celeb.celeb_Id);
                        if (item.movie_CastsId != null && item.movie_CastsId.Contains(celeb.celeb_DoubanID))
                            item.movie_CastsId.Replace(celeb.celeb_DoubanID, celeb.celeb_Id);
                    }
            }
            _db.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}
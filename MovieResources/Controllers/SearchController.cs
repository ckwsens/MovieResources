using MovieResources.Helpers;
using MovieResources.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MovieResources.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/
        public ActionResult Index(string search, string genre = "0", string country = "0", string year = "0", int page = 1)
        {
            MR_DataClassesDataContext _db = new MR_DataClassesDataContext();
            ClassifyViewModel movieGroup = new ClassifyViewModel();
            var filterMovie = from m in _db.tbl_Movie
                              where m.movie_Status == 2
                              select m;
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                filterMovie = filterMovie.Where(m => m.movie_Title.Contains(search) || m.movie_TitleEn.Contains(search) || m.movie_Aka.Contains(search));
            }
            if (genre != "0")
            {
                filterMovie = filterMovie.Where(m => m.movie_Genres.IndexOf(genre) > -1);
            }
            if (country != "0")
            {
                filterMovie = filterMovie.Where(m => m.movie_Countries.IndexOf(country) > -1);
            }
            if (year != "0")
            {
                filterMovie = filterMovie.Where(m => m.movie_Year == year);
            }
            filterMovie.OrderByDescending(m => m.movie_Time);
            movieGroup.listMovies.Clear();
            foreach (var item in filterMovie)
            {
                MovieViewModel movie = new MovieViewModel(item);
                movie.IsPlan = MarkManager.Validate(item.movie_Id, AccountManager.GetId(User.Identity.Name), 1);
                movie.IsFinish = MarkManager.Validate(item.movie_Id, AccountManager.GetId(User.Identity.Name), 2);
                movie.IsFavor = MarkManager.Validate(item.movie_Id, AccountManager.GetId(User.Identity.Name), 3);
                movieGroup.listMovies.Add(movie);
            }
            movieGroup.Count = movieGroup.listMovies.Count();
            movieGroup.Search = search;
            movieGroup.Genre = genre;
            movieGroup.Country = country;
            movieGroup.Year = year;

            movieGroup.Page = page;
            movieGroup.PagingCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(movieGroup.Count) / movieGroup.MovieSize));

            movieGroup.listMovies = movieGroup.listMovies.Skip((page - 1) * movieGroup.MovieSize).Take(movieGroup.MovieSize).ToList();
            if (page > movieGroup.PagingCount && movieGroup.listMovies.Count > 0)
            {
                return RedirectToAction("NotFound", "Error");
            }
            string url = Translator.BuildUrl(Request.Url.ToString(), "search", search);
            url = Translator.BuildUrl(url, "genre", genre);
            url = Translator.BuildUrl(url, "country", country);
            url = Translator.BuildUrl(url, "year", year);
            ViewBag.CurrentUrl = url;
            return View(movieGroup);
        }
    }
}
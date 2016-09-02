using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MovieResources.Models
{
    public class SearchViewModel
    {
        public string Keyword { get; set; }
    }

    public class ClassifyViewModel
    {
        public List<SelectListItem> listGenre { get; set; }
        public List<SelectListItem> listCountry { get; set; }
        public List<SelectListItem> listYear { get; set; }
        public List<MovieViewModel> listMovies { get; set; }

        public int Count { get; set; }
        public int MovieSize { get { return 15; } }

        public string Search { get; set; }
        public string Genre { get; set; }
        public string Country { get; set; }
        public string Year { get; set; }

        public int Page { get; set; }
        public int PagingCount { get; set; }
        public int PagingSize { get { return 10; } }

        public ClassifyViewModel()
        {
            listGenre = new List<SelectListItem>();
            listCountry = new List<SelectListItem>();
            listYear = new List<SelectListItem>();
            listMovies = new List<MovieViewModel>();

            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var tblGenre = _db.tbl_GenreMovie.ToList();
                var tblCountry = _db.tbl_Country.ToList();
                var tblYear = from m in _db.tbl_Movie
                              select m.movie_Year;

                tblYear = tblYear.Distinct().OrderByDescending(y => y);
                listYear.Add(new SelectListItem() { Text = "选择年代", Value = "0" });
                foreach (var item in tblYear)
                {
                    listYear.Add(new SelectListItem() { Text = item, Value = item });
                }

                listGenre.Add(new SelectListItem() { Text = "选择类型", Value = "0" });
                foreach (var item in tblGenre)
                {
                    listGenre.Add(new SelectListItem() { Text = item.genre_Name, Value = item.genre_Id.ToString() });
                }

                listCountry.Add(new SelectListItem() { Text = "选择国家/地区", Value = "0" });
                foreach (var item in tblCountry)
                {
                    listCountry.Add(new SelectListItem() { Text = item.country_Name, Value = item.country_Id.ToString() });
                }
            }
        }
    }
}
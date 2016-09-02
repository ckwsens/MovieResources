using System.Collections.Generic;
using System.Linq;

namespace MovieResources.Models
{
    public class HomeViewModel
    {
        public HomeDiscViewModel Discovery { get; set; }
        public List<MovieListViewModel> News { get; set; }
        public List<MovieListViewModel> Mosts { get; set; }
    }

    public class HomeDiscViewModel
    {
        public string Id { get; set; }

        public MovieViewModel Movie { get; set; }

        public string Image { get; set; }

        public int Flag { get; set; }

        public int Pre { get; set; }

        public int Post { get; set; }

        public int Offset { set; get; }

        public HomeDiscViewModel(tbl_Discovery dis)
        {
            Id = dis.disc_Id;
            MR_DataClassesDataContext _db = new MR_DataClassesDataContext();
            Movie = new MovieViewModel(_db.tbl_Movie.SingleOrDefault(m => m.movie_Id == dis.disc_Movie));
            Image = dis.disc_Image;
            Flag = dis.disc_Flag;
        }
    }

    public class MovieListViewModel
    {
        public string Title { get; set; }
        //public string TitleEn { get; set; }
        public string Year { get; set; }
        public string Id { get; set; }
    }
}
using MovieResources.Helpers;
using System.ComponentModel.DataAnnotations;

namespace MovieResources.Models
{
    public class ManageDiscViewModel
    {
        public string Id { get; set; }

        [Display(Name = "电影")]
        [Required(ErrorMessage ="请选择一个电影")]
        public string Movie { get; set; }

        public string MovieTitle { get; set; }

        [Display(Name = "图片")]
        public string Image { get; set; }

        public int Flag { get; set; }

        public string Time { get; set; }

        public ManageDiscViewModel(tbl_Discovery model)
        {
            Id = model.disc_Id;
            Movie = model.disc_Movie;
            MovieTitle = MovieManager.GetTitle(model.disc_Movie);
            Image = model.disc_Image;
            Flag = model.disc_Flag;
            Time = model.disc_Time.ToString();
        }

        public ManageDiscViewModel()
        {

        }
    }
}
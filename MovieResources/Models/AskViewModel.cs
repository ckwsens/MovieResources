using MovieResources.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MovieResources.Models
{
    public class AskViewModel
    {
        public string Id { get; set; }

        public MovieViewModel Movie { get; set; }

        public string MovieId { get; set; }

        public string Note { get; set; }

        public string User { get; set; }

        public string Account { get; set; }

        public bool State { get; set; }

        public string Time { get; set; }

        public int With { get; set; }

        public bool hadWith { get; set; }

        public AskViewModel(tbl_Ask ask)
        {
            Id = ask.ask_Id;
            MovieId = ask.ask_Movie;
            MR_DataClassesDataContext _db = new MR_DataClassesDataContext();
            tbl_Movie tblmovie = _db.tbl_Movie.SingleOrDefault(m => m.movie_Id == ask.ask_Movie);
            Movie = new MovieViewModel(tblmovie);
            Note = ask.ask_Note;
            State = (bool)ask.ask_State;
            User = ask.ask_User;
            Account = AccountManager.GetAccount(ask.ask_User);
            Time = ((DateTime)ask.ask_Time).Date.ToShortDateString(); ;
            With = (int)ask.ask_With;

            hadWith = false;
        }
    }

    /// <summary>
    /// 所有求资源列表
    /// </summary>
    public class IndexAskViewModel
    {
        public List<AskViewModel> All { get; set; }
        public List<AskViewModel> Most { get; set; }
        public List<AskViewModel> Over { get; set; }

        public IndexAskViewModel()
        {
            All = new List<AskViewModel>();
            Most = new List<AskViewModel>();
            Over = new List<AskViewModel>();
        }
    }

    public class ManageAskViewModel
    {
        public string Id { get; set; }

        public string MovieTitle { get; set; }

        public string MovieId { get; set; }

        [Display(Name = "备注")]
        [Required(ErrorMessage = "备注 不能为空")]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

        public string User { get; set; }
    }
}
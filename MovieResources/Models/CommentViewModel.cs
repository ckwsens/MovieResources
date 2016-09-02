using MovieResources.Helpers;
using System;

namespace MovieResources.Models
{
    public class CommentViewModel
    {
        public string Id { get; set; }

        public string Content { get; set; }

        public string UserId { get; set; }

        public string UserAccount { get; set; }

        public string UserAvatar { get; set; }

        public string Movie { get; set; }

        public string Time { get; set; }

        public CommentViewModel(tbl_Comment cmt)
        {
            Id = cmt.cmt_Id;
            Content = cmt.cmt_Content;
            UserId = cmt.cmt_User;
            UserAccount = AccountManager.GetAccount(cmt.cmt_User);
            UserAvatar = AccountManager.GetAvatar(cmt.cmt_User);
            Movie = cmt.cmt_Movie;
            Time = ((DateTime)cmt.cmt_Time).Date.ToShortDateString();
        }

        public CommentViewModel()
        {

        }
    }
}
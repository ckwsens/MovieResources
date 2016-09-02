using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieResources.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "请输入 原密码。")]
        [DataType(DataType.Password)]
        [Display(Name = "原密码")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "请输入 新密码。")]
        [RegularExpression(@"^(?=.*\d.*)(?=.*[a-zA-Z].*).{6,}$", ErrorMessage = "密码 必须包括字符和数字，且长度不小于6")]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "新密码 和 确认密码不匹配。")]
        public string ConfirmPassword { get; set; }

        public string Account { get; set; }

        public string Avatar { get; set; }
    }

    public class ChangeAvatarViewModel
    {
        [Display(Name = "用户编号")]
        public string Id { get; set; }

        [Display(Name = "昵称")]
        public string Account { get; set; }

        [Display(Name = "头像")]
        public string Avatar { get; set; }
    }

    public class ChangeCoverViewModel
    {
        [Display(Name = "用户编号")]
        public string Id { get; set; }

        [Display(Name = "昵称")]
        public string Account { get; set; }

        [Display(Name = "封面")]
        public string Cover { get; set; }

        public string Avatar { get; set; }
    }

    public class MineViewModel
    {
        public string Id { get; set; }

        public string Account { get; set; }

        public string Avatar { get; set; }

        public string Cover { get; set; }

        public int PlanCount { get; set; }
        public List<MarkItem> MoviePlans { get; set; }

        public int FinishCount { get; set; }
        public List<MarkItem> MovieFinishs { get; set; }

        public int FavorCount { get; set; }
        public List<MarkItem> MovieFavors { get; set; }

        public int CollectCount { get; set; }
        public List<MarkItem> CelebCollects { get; set; }

        public int AlbumCount { get; set; }
        public List<AlbumListItem> Albums { get; set; }

        public int CommentCount { get; set; }
        public List<CommentItem> Comments { get; set; }

    }

    public class PeopleViewModel
    {
        public string Id { get; set; }

        public string Account { get; set; }

        public string Avatar { get; set; }

        public string Cover { get; set; }

        public int PlanCount { get; set; }
        public List<MarkItem> MoviePlans { get; set; }

        public int FinishCount { get; set; }
        public List<MarkItem> MovieFinishs { get; set; }

        public int FavorCount { get; set; }
        public List<MarkItem> MovieFavors { get; set; }

        public int CollectCount { get; set; }
        public List<MarkItem> CelebCollects { get; set; }

        public int CommonCount { get; set; }
        public List<MarkItem> MovieCommons { get; set; }

        public int AlbumCount { get; set; }
        public List<AlbumListItem> Albums { get; set; }

        public int CommentCount { get; set; }
        public List<CommentItem> Comments { get; set; }

    }

    public class MarkItem
    {
        public string Avatar { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }
    }

    public class CommentItem
    {
        public string MovieTitle { get; set; }
        public string MovieID { get; set; }
        public string Time { get; set; }
        public string Comment { get; set; }
    }

    public class MineMovieViewModel
    {
        public List<MovieViewModel> Moives { get; set; }

        public string Account { get; set; }

        public string Id { get; set; }

        public string Avatar { get; set; }

        public int PlanCount { get; set; }

        public int FinishCount { get; set; }

        public int FavorCount { get; set; }

        public int CreateCount { get; set; }
    }

    public class MineCelebViewModel
    {
        public string Account { get; set; }

        public string Id { get; set; }

        public string Avatar { get; set; }

        public List<CelebViewModel> Celebs { get; set; }

        public int CollectCount { get; set; }

        public int CreateCount { get; set; }
    }

    public class MineResViewModel
    {
        public string Account { get; set; }

        public string Id { get; set; }

        public string Avatar { get; set; }

        public List<ResViewModel> Ress { get; set; }

        public int AskCount { get; set; }

        public int ResCount { get; set; }
    }

    public class MineAskViewModel
    {
        public string Account { get; set; }

        public string Id { get; set; }

        public string Avatar { get; set; }

        public List<AskViewModel> Asks { get; set; }

        public int AskCount { get; set; }

        public int ResCount { get; set; }
    }

    public class MineAlbumViewModel
    {
        public string Account { get; set; }

        public string Id { get; set; }

        public string Avatar { get; set; }

        public List<AlbumViewModel> Albums { get; set; }

        public int AlbumCount { get; set; }

        public int FollowCount { get; set; }
    }

}
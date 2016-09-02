using MovieResources.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MovieResources.Models
{
    public class ManageResViewModel
    {
        public string Id { get; set; }

        public string MovieTitle { get; set; }

        public string MovieId { get; set; }

        [Display(Name = "资源链接")]
        //[Required(ErrorMessage = "资源链接 不能为空")]
        public string Content { get; set; }

        [Display(Name = "资源标题")]
        [Required(ErrorMessage = "资源标题 不能为空")]
        public string FileName { get; set; }

        [Display(Name = "资源大小")]
        [Required(ErrorMessage = "资源大小 不能为空")]
        [Range(0, float.MaxValue, ErrorMessage = ("请输入有效的文件大小"))]
        public string FileSize { get; set; }

        public byte ResType { get; set; }

        public byte Status { get; set; }

        public string User { get; set; }
    }

    public class ResViewModel
    {
        public string Id { get; set; }

        public string Content { get; set; }

        public string FileName { get; set; }

        public string FileSize { get; set; }

        public int FavorCount { get; set; }

        public byte ResType { get; set; }

        public string User { get; set; }

        public string Account { get; set; }

        public string Movie { get; set; }

        public string MovieTitle { get; set; }

        public byte Status { get; set; }

        public string Note { get; set; }

        public ResViewModel(tbl_Resource res)
        {
            Id = res.res_Id;
            Content = res.res_Content;
            FileName = res.res_Name;
            if (res.res_Size > 1024 * 1024 * 1024)
            {
                FileSize = (res.res_Size / (1024 * 1024 * 1024)).ToString() + " G";
            }
            else if (res.res_Size > 1024 * 1024)
            {
                FileSize = (res.res_Size / (1024 * 1024)).ToString() + " M";
            }
            else if (res.res_Size > 1024)
            {
                FileSize = (res.res_Size / (1024)).ToString() + " K";
            }
            else
            {
                FileSize = res.res_Size.ToString() + " 字节";
            }
            FavorCount = (int)res.res_FavorCount;
            ResType = (byte)res.res_Type;
            Status = (byte)res.res_Status;
            Note = res.res_Note;
            if (!string.IsNullOrEmpty(res.res_User) && !string.IsNullOrWhiteSpace(res.res_User))
            {
                MR_DataClassesDataContext _db = new MR_DataClassesDataContext();

                User = res.res_User;
                Account = _db.tbl_UserAccount.SingleOrDefault(u => u.user_Id == res.res_User).user_Account;
            }

            Movie = res.res_Movie;
            MovieTitle = MovieManager.GetTitle(res.res_Movie);
        }
    }

    public class FilterResViewModel
    {
        public List<ResViewModel> Ress { get; set; }

        public string Search { get; set; }

        public bool OnlyUnaudit { get; set; }

        public int Page { get; set; }
    }

    public class RejectResViewModel
    {
        public string Id { get; set; }
        public string Note { get; set; }
    }

}
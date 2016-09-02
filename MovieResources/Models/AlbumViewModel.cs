using MovieResources.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MovieResources.Models
{
    /// <summary>
    /// 用于管理的专辑
    /// </summary>
    public class ManageAlbumViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "请输入 专辑名称。")]
        [Display(Name = "专辑名称")]
        public string Title { get; set; }

        [Display(Name = "专辑简介")]
        [DataType(DataType.MultilineText)]
        public string Summary { get; set; }

        [Display(Name = "专辑封面")]
        public string Cover { get; set; }

        [Display(Name = "创建人")]
        public string UserId { get; set; }
        public string UserAccount { get; set; }

        public string Item { get; set; }

        [Display(Name = "创建时间")]
        public string CreateTime { get; set; }

        [Display(Name = "修改时间")]
        public string AlterTime { get; set; }

        public ManageAlbumViewModel(tbl_Album album)
        {
            Id = album.album_Id;
            Title = album.album_Title;
            Summary = album.album_Summary;
            Cover = album.album_Cover;
            UserId = album.album_User;
            UserAccount = AccountManager.GetAccount(album.album_User);
            CreateTime = ((DateTime)album.album_Time).Date.ToShortDateString();
            AlterTime = ((DateTime)album.album_AlterTime).Date.ToShortDateString();
            Item = album.album_Item;
        }

        public ManageAlbumViewModel()
        {
        }
    }

    /// <summary>
    /// 用户展示信息的专辑
    /// </summary>
    public class AlbumViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Cover { get; set; }

        public string UserId { get; set; }

        public string UserAccount { get; set; }

        public string ItemJson { get; set; }

        public string CreateTime { get; set; }

        public string AlterTime { get; set; }

        public bool IsCreator { get; set; }

        public bool HasFollow { get; set; }

        public int Count { get; set; }
        public int ItemSize { get { return 15; } }
        public int Page { get; set; }
        public int PagingCount { get; set; }
        public int PagingSize { get { return 10; } }

        public List<AlbumItemViewModel> Items { get; set; }

        public List<AlbumListItem> Albums { get; set; }

        public AlbumViewModel(tbl_Album album)
        {
            Id = album.album_Id;
            Title = album.album_Title;
            Summary = album.album_Summary;
            Cover = album.album_Cover;
            UserId = album.album_User;
            UserAccount = AccountManager.GetAccount(album.album_User);
            CreateTime = ((DateTime)album.album_Time).Date.ToShortDateString();
            AlterTime = ((DateTime)album.album_AlterTime).Date.ToShortDateString();

            MR_DataClassesDataContext _db = new MR_DataClassesDataContext();
            ItemJson = album.album_Item;


            var others = _db.tbl_Album.Where(a => a.album_User == album.album_User);
            Albums = new List<AlbumListItem>();
            foreach (var item in others)
            {
                if (item.album_Id != album.album_Id)
                {
                    Albums.Add(new AlbumListItem(item));
                }
            }
            IsCreator = false;
            HasFollow = false;
        }

        public AlbumViewModel()
        {
        }
    }

    /// <summary>
    /// 专辑内项目
    /// </summary>
    public class AlbumItemViewModel
    {
        public string Movie { get; set; }
        public string Note { get; set; }
        public string Time { get; set; }

        public MovieViewModel MovieInfo { get; set; }
    }

    /// <summary>
    /// 所有专辑列表
    /// </summary>
    public class AlbumListItem
    {
        public string Cover { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserAccount { get; set; }
        public int VisitCount { get; set; }
        public int FollowCount { get; set; }

        public AlbumListItem(tbl_Album album)
        {
            Cover = album.album_Cover;
            Id = album.album_Id;
            Title = album.album_Title;
            Summary = album.album_Summary;
            if (!AccountManager.IsAdmin(album.album_User))
            {
                UserId = album.album_User;
                UserAccount = AccountManager.GetAccount(album.album_User);
            }
            VisitCount = (int)album.album_Visit;
            MR_DataClassesDataContext _db = new MR_DataClassesDataContext();
            FollowCount = _db.tbl_Mark.Where(m => m.mark_Target == album.album_Id && m.mark_Type == 7).Count();
        }

        public AlbumListItem()
        {
        }
    }

}
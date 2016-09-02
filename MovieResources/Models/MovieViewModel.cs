using MovieResources.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MovieResources.Models
{
    /// <summary>
    /// Display用
    /// </summary>
    public class MovieViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string TitleEn { get; set; }

        public string Aka { get; set; }

        public List<LinkItem> Directors { get; set; }

        public List<LinkItem> Casts { get; set; }

        public List<LinkItem> Writers { get; set; }

        public string Year { get; set; }

        public string Pubdates { get; set; }

        public string Durations { get; set; }

        public string Genres { get; set; }

        public string Languages { get; set; }

        public string Countries { get; set; }

        public string Rating { get; set; }

        public string RatingCount { get; set; }

        public string DoubanID { get; set; }

        public string IMDbID { get; set; }

        public string Summary { get; set; }

        public string SummaryShort { get; set; }

        public string[] SummaryPara { get; set; }

        public string Avatar { get; set; }

        public bool IsPlan { get; set; }
        public bool IsFinish { get; set; }
        public bool IsFavor { get; set; }

        public int PlanCount { get; set; }
        public int FinishCount { get; set; }
        public int FavorCount { get; set; }

        public int VisitCount { get; set; }//浏览量

        public bool IsCreate { get; set; }//当前用户是否是创建者

        public string Create { get; set; }//创建者id

        public byte Status { get; set; }//审核状态

        public string Note { get; set; }//审核备注

        public List<ResViewModel> Resources { get; set; }

        public CommentViewModel MyComment { get; set; }

        public int CommentCount { get; set; }
        public List<CommentViewModel> Comments { get; set; }

        public List<LinkItem> Albums { get; set; }


        public MovieViewModel(tbl_Movie movie)
        {
            Id = movie.movie_Id;
            Title = movie.movie_Title;
            TitleEn = movie.movie_TitleEn;
            Aka = movie.movie_Aka;
            Pubdates = movie.movie_Pubdates;
            Year = movie.movie_Year;
            Durations = movie.movie_Durations;
            Genres = string.IsNullOrEmpty(movie.movie_Genres) || string.IsNullOrWhiteSpace(movie.movie_Genres) ? null : Translator.GenreToName(movie.movie_Genres);
            Languages = string.IsNullOrEmpty(movie.movie_Languages) || string.IsNullOrWhiteSpace(movie.movie_Languages) ? null : Translator.LangToName(movie.movie_Languages);
            Countries = string.IsNullOrEmpty(movie.movie_Countries) || string.IsNullOrWhiteSpace(movie.movie_Countries) ? null : Translator.CountryToName(movie.movie_Countries);
            Rating = movie.movie_Rating;
            RatingCount = movie.movie_RatingCount;
            Avatar = movie.movie_Avatar;
            Summary = movie.movie_Summary;
            DoubanID = movie.movie_DoubanID;
            IMDbID = movie.movie_IMDbID;
            VisitCount = (int)movie.movie_VisitCount;
            Create = movie.movie_Create;
            Status = (byte)movie.movie_Status;
            Note = movie.movie_Note;

            if (movie.movie_Summary != null && movie.movie_Summary.Length > 250)
            {
                SummaryShort = movie.movie_Summary.Substring(0, 247) + "...";
            }
            else
            {
                SummaryShort = movie.movie_Summary;
            }
            if (movie.movie_Summary != null)
            {
                if (SummaryShort.LastIndexOfAny(new char[] { ',', '.', '，', '。', '\n' }) > 0)
                {
                    SummaryShort = SummaryShort.Substring(0, SummaryShort.LastIndexOfAny(new char[] { ',', '.', '，', '。', '\n' })) + "...";
                }
                else
                {
                    SummaryShort = SummaryShort.Replace("\n", "<br/>") + "...";
                }
                SummaryPara = movie.movie_Summary.Split('\n');
            }
            else
            {
                SummaryPara = null;
            }

            IsCreate = false;
            IsPlan = false;
            IsFinish = false;
            IsFavor = false;
            PlanCount = 0;
            FinishCount = 0;
            FavorCount = 0;

            MR_DataClassesDataContext _db = new MR_DataClassesDataContext();
            Directors = new List<LinkItem>();
            if (string.IsNullOrEmpty(movie.movie_DirectorsId))
            {
                if (!string.IsNullOrEmpty(movie.movie_Directors))
                {
                    foreach (var item in movie.movie_Directors.Split('/'))
                    {
                        Directors.Add(new LinkItem() { Title = item });
                    }
                }
            }
            else
            {
                int i = 0;
                foreach (var item in movie.movie_DirectorsId.Split('/'))
                {
                    if (item.Length == 32)
                    {
                        var celeb = _db.tbl_Celebrity.SingleOrDefault(c => c.celeb_Id == item);
                        if (celeb != null)
                        {
                            Directors.Add(new LinkItem() { Title = celeb.celeb_Name, Id = celeb.celeb_Id });
                        }
                    }
                    else
                    {
                        Directors.Add(new LinkItem() { Title = movie.movie_Directors.Split('/')[i] });
                    }
                    i++;
                }
            }

            Writers = new List<LinkItem>();
            if (string.IsNullOrEmpty(movie.movie_WritersId))
            {
                if (!string.IsNullOrEmpty(movie.movie_Writers))
                {
                    foreach (var item in movie.movie_Writers.Split('/'))
                    {
                        Writers.Add(new LinkItem() { Title = item });
                    }
                }
            }
            else
            {
                int i = 0;
                foreach (var item in movie.movie_WritersId.Split('/'))
                {
                    if (item.Length == 32)
                    {
                        var celeb = _db.tbl_Celebrity.SingleOrDefault(c => c.celeb_Id == item);
                        if (celeb != null)
                        {
                            Writers.Add(new LinkItem() { Title = celeb.celeb_Name, Id = celeb.celeb_Id });
                        }
                    }
                    else
                    {
                        Writers.Add(new LinkItem() { Title = movie.movie_Writers.Split('/')[i] });
                    }
                    i++;
                }
            }


            Casts = new List<LinkItem>();
            if (string.IsNullOrEmpty(movie.movie_CastsId))
            {
                if (!string.IsNullOrEmpty(movie.movie_Casts))
                {
                    foreach (var item in movie.movie_Casts.Split('/'))
                    {
                        Casts.Add(new LinkItem() { Title = item });
                    }
                }
            }
            else
            {
                int i = 0;
                foreach (var item in movie.movie_CastsId.Split('/'))
                {
                    if (item.Length == 32)
                    {
                        var celeb = _db.tbl_Celebrity.SingleOrDefault(c => c.celeb_Id == item);
                        if (celeb != null)
                        {
                            Casts.Add(new LinkItem() { Title = celeb.celeb_Name, Id = celeb.celeb_Id });
                        }
                    }
                    else
                    {
                        Casts.Add(new LinkItem() { Title = movie.movie_Casts.Split('/')[i] });
                    }
                    i++;
                }
            }
        }
        public MovieViewModel()
        {
        }
    }

    public class LinkItem
    {
        public string Title { get; set; }
        public string Id { get; set; }
    }

    /// <summary>
    /// 管理用
    /// </summary>
    public class ManageMovieViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "请输入 电影名。")]
        [Display(Name = "电影名")]
        public string Title { get; set; }

        [Display(Name = "外文名")]
        public string TitleEn { get; set; }

        [Display(Name = "更多电影名")]
        public string Aka { get; set; }

        [Display(Name = "导演")]
        public string Directors { get; set; }

        [Display(Name = "编剧")]
        public string Writers { get; set; }

        [Display(Name = "演员")]
        public string Casts { get; set; }

        public string DirectorsId { get; set; }

        public string WritersId { get; set; }

        public string CastsId { get; set; }

        [Range(0, 9999, ErrorMessage = "请输入正确的年份")]
        [Display(Name = "年代")]
        public string Year { get; set; }

        [Display(Name = "上映日期")]
        public string Pubdates { get; set; }

        [Display(Name = "时长")]
        public string Durations { get; set; }

        [Display(Name = "类型")]
        public string Genres { get; set; }

        [Display(Name = "语言")]
        public string Languages { get; set; }

        [Display(Name = "制片国家/地区")]
        public string Countries { get; set; }

        [Range(0.0, 10.0, ErrorMessage = "请输入小于10的数")]
        [Display(Name = "评分")]
        public string Rating { get; set; }

        [Display(Name = "评分人数")]
        public string RatingCount { get; set; }

        [Display(Name = "豆瓣编号")]
        public string DoubanID { get; set; }

        [Display(Name = "IMDb编号")]
        public string IMDbID { get; set; }

        [Display(Name = "电影简介")]
        [DataType(DataType.MultilineText)]
        public string Summary { get; set; }

        [Display(Name = "海报")]
        [DataType(DataType.Upload)]
        public string Avatar { get; set; }

        public byte Status { get; set; }

        public string Create { get; set; }

        public string CreateAccount { get; set; }

        public string Note { get; set; }

        public ManageMovieViewModel(tbl_Movie movie)
        {
            Id = movie.movie_Id;
            Title = movie.movie_Title;
            TitleEn = movie.movie_TitleEn;
            Aka = movie.movie_Aka;
            Directors = movie.movie_Directors;
            Writers = movie.movie_Writers;
            Casts = movie.movie_Casts;
            DirectorsId = movie.movie_DirectorsId;
            WritersId = movie.movie_WritersId;
            CastsId = movie.movie_CastsId;
            Pubdates = movie.movie_Pubdates;
            Year = movie.movie_Year;
            Durations = movie.movie_Durations;
            Genres = string.IsNullOrEmpty(movie.movie_Genres) || string.IsNullOrWhiteSpace(movie.movie_Genres) ? null : Translator.GenreToName(movie.movie_Genres);
            Languages = string.IsNullOrEmpty(movie.movie_Languages) || string.IsNullOrWhiteSpace(movie.movie_Languages) ? null : Translator.LangToName(movie.movie_Languages);
            Countries = string.IsNullOrEmpty(movie.movie_Countries) || string.IsNullOrWhiteSpace(movie.movie_Countries) ? null : Translator.CountryToName(movie.movie_Countries);
            Rating = movie.movie_Rating;
            RatingCount = movie.movie_RatingCount;
            Avatar = movie.movie_Avatar;
            Summary = movie.movie_Summary;
            DoubanID = movie.movie_DoubanID;
            IMDbID = movie.movie_IMDbID;
            Create = movie.movie_Create;
            CreateAccount = AccountManager.GetAccount(movie.movie_Create);
            if (movie.movie_Status != null)
            {
                Status = (byte)movie.movie_Status;
            }
            Note = movie.movie_Note;
        }

        public ManageMovieViewModel()
        {
        }
    }

    public class CreateMovieViewModel
    {
        [Display(Name = "豆瓣编号")]
        [Required(ErrorMessage = "豆瓣编号不能为空")]
        [DataType(DataType.MultilineText)]
        public string DoubanID { get; set; }
    }

    public class RefreshMovieViewModel
    {
        public ManageMovieViewModel Old { get; set; }
        public ManageMovieViewModel New { get; set; }
    }

    public class RejectViewModel
    {
        public string Id { get; set; }
        public string Note { get; set; }
    }

    public class MovieCommentViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string TitleEn { get; set; }

        public string Aka { get; set; }

        public List<LinkItem> Directors { get; set; }

        public List<LinkItem> Casts { get; set; }

        public List<LinkItem> Writers { get; set; }

        public string Year { get; set; }

        public string Pubdates { get; set; }

        public string Durations { get; set; }

        public string Genres { get; set; }

        public string Countries { get; set; }

        public string Languages { get; set; }

        public string Avatar { get; set; }

        public List<CommentViewModel> Comments { get; set; }

        public int CommentCount { get; set; }

        public MovieCommentViewModel(tbl_Movie movie)
        {
            Id = movie.movie_Id;
            Title = movie.movie_Title;
            TitleEn = movie.movie_TitleEn;
            Aka = movie.movie_Aka;
            Pubdates = movie.movie_Pubdates;
            Year = movie.movie_Year;
            Durations = movie.movie_Durations;
            Genres = string.IsNullOrEmpty(movie.movie_Genres) || string.IsNullOrWhiteSpace(movie.movie_Genres) ? null : Translator.GenreToName(movie.movie_Genres);
            Languages = string.IsNullOrEmpty(movie.movie_Languages) || string.IsNullOrWhiteSpace(movie.movie_Languages) ? null : Translator.LangToName(movie.movie_Languages);
            Countries = string.IsNullOrEmpty(movie.movie_Countries) || string.IsNullOrWhiteSpace(movie.movie_Countries) ? null : Translator.CountryToName(movie.movie_Countries);
            Avatar = movie.movie_Avatar;
        }
        public MovieCommentViewModel()
        {
        }
    }
}
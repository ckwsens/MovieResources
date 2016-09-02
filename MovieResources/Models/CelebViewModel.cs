using MovieResources.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MovieResources.Models
{
    public class CelebViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string NameEn { get; set; }

        public string Aka { get; set; }

        public string AkaEn { get; set; }

        public string Gender { get; set; }

        public string Birthday { get; set; }

        public string Deathday { get; set; }

        public string BornPlace { get; set; }

        public string Pro { get; set; }

        public string Family { get; set; }

        public string DoubanID { get; set; }

        public string IMDbID { get; set; }

        public string Summary { get; set; }

        public string SummaryShort { get; set; }

        public string Avatar { get; set; }

        public string[] SummaryPara { get; set; }

        public bool IsCollect { get; set; }

        public int CollectCount { get; set; }

        public List<CelebWorkItem> Works { get; set; }

        public bool IsCreate { get; set; }

        public string Create { get; set; }

        public byte Status { get; set; }

        public string Note { get; set; }

        public CelebViewModel(tbl_Celebrity celeb)
        {
            Id = celeb.celeb_Id;
            Name = celeb.celeb_Name;
            Aka = celeb.celeb_Aka;
            NameEn = celeb.celeb_NameEn;
            AkaEn = celeb.celeb_AkaEn;
            Gender = Translator.GenderToTitle(celeb.celeb_Gender);
            Birthday = celeb.celeb_Birthday;
            Deathday = celeb.celeb_Deathday;
            BornPlace = celeb.celeb_BornPlace;
            Pro = celeb.celeb_Pro;
            Family = celeb.celeb_Family;
            DoubanID = celeb.celeb_DoubanID;
            IMDbID = celeb.celeb_IMDbID;
            Summary = celeb.celeb_Summary;
            Avatar = celeb.celeb_Avatar;

            Create = celeb.celeb_Create;
            Status = (byte)celeb.celeb_Status;
            Note = celeb.celeb_Note;

            if (celeb.celeb_Summary != null && celeb.celeb_Summary.Length > 250)
            {
                SummaryShort = celeb.celeb_Summary.Substring(0, 247) + "...";
            }
            else
            {
                SummaryShort = celeb.celeb_Summary;
            }
            if (celeb.celeb_Summary != null)
            {
                if (SummaryShort.LastIndexOfAny(new char[] { ',', '.', '，', '。', '\n' }) > 0)
                {
                    SummaryShort = SummaryShort.Substring(0, SummaryShort.LastIndexOfAny(new char[] { ',', '.', '，', '。', '\n' })) + "...";
                }
                else
                {
                    SummaryShort = SummaryShort.Replace("\n", "<br/>") + "...";
                }
                SummaryPara = celeb.celeb_Summary.Split('\n');
            }
            else
            {
                SummaryPara = null;
            }

            Works = new List<CelebWorkItem>();
            if (DoubanID != null)
            {
                MR_DataClassesDataContext _db = new MR_DataClassesDataContext();
                var works = _db.tbl_Movie.Where(
                    m => m.movie_DirectorsId.Contains(celeb.celeb_DoubanID) ||
                    m.movie_DirectorsId.Contains(celeb.celeb_Id) ||
                    m.movie_WritersId.Contains(celeb.celeb_DoubanID) ||
                    m.movie_WritersId.Contains(celeb.celeb_Id) ||
                    m.movie_CastsId.Contains(celeb.celeb_DoubanID) ||
                    m.movie_CastsId.Contains(celeb.celeb_Id)).OrderBy(m => m.movie_Rating).Take(10);
                foreach (var item in works)
                {
                    MovieViewModel work = new MovieViewModel(item);
                    System.Text.StringBuilder temp = new System.Text.StringBuilder();
                    temp.Append("[");
                    if (item.movie_DirectorsId.Contains(celeb.celeb_DoubanID) || item.movie_DirectorsId.Contains(celeb.celeb_Id))
                    {
                        temp.Append(" 导演 ");
                    }
                    if (item.movie_WritersId.Contains(celeb.celeb_DoubanID) || item.movie_WritersId.Contains(celeb.celeb_Id))
                    {
                        temp.Append(" 编剧 ");
                    }
                    if (item.movie_CastsId.Contains(celeb.celeb_DoubanID) || item.movie_CastsId.Contains(celeb.celeb_Id))
                    {
                        temp.Append(" 演员 ");
                    }
                    temp.Append("]");

                    Works.Add(new CelebWorkItem() { Work = work, Pro = temp.ToString() });
                }
            }
            IsCreate = false;
        }
        public CelebViewModel()
        {
        }
    }

    public class CelebWorkItem
    {
        public MovieViewModel Work { get; set; }
        public string Pro { get; set; }
    }

    public class ManageCelebViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "请输入 中文名。")]
        [Display(Name = "中文名")]
        public string Name { get; set; }

        [Display(Name = "外文名")]
        public string NameEn { get; set; }

        [Display(Name = "更多中文名")]
        public string Aka { get; set; }

        [Display(Name = "更多外文名")]
        public string AkaEn { get; set; }

        [Display(Name = "性别")]
        public string Gender { get; set; }

        [Display(Name = "出生日期")]
        [DataType(DataType.Date, ErrorMessage = "请输入正确的日期格式")]
        public string Birthday { get; set; }

        [Display(Name = "逝世日期")]
        [DataType(DataType.Date, ErrorMessage = "请输入正确的日期格式")]
        public string Deathday { get; set; }

        [Display(Name = "出生地")]
        public string BornPlace { get; set; }

        [Display(Name = "职业")]
        public string Pro { get; set; }

        [Display(Name = "家庭成员")]
        public string Family { get; set; }

        [Display(Name = "豆瓣影人编号")]
        public string DoubanID { get; set; }

        [Display(Name = "IMDb编号")]
        public string IMDbID { get; set; }

        [Display(Name = "影人简介")]
        [DataType(DataType.MultilineText)]
        public string Summary { get; set; }

        [Display(Name = "海报")]
        public string Avatar { get; set; }

        public byte Status { get; set; }

        public string Create { get; set; }

        public string CreateAccount { get; set; }

        public string Note { get; set; }

        public ManageCelebViewModel(tbl_Celebrity celeb)
        {
            Id = celeb.celeb_Id;
            Name = celeb.celeb_Name;
            Aka = celeb.celeb_Aka;
            NameEn = celeb.celeb_NameEn;
            AkaEn = celeb.celeb_AkaEn;
            Gender = Translator.GenderToTitle(celeb.celeb_Gender);
            Birthday = celeb.celeb_Birthday;
            Deathday = celeb.celeb_Deathday;
            BornPlace = celeb.celeb_BornPlace;
            Pro = celeb.celeb_Pro;
            Family = celeb.celeb_Family;
            DoubanID = celeb.celeb_DoubanID;
            IMDbID = celeb.celeb_IMDbID;
            Summary = celeb.celeb_Summary;
            Avatar = celeb.celeb_Avatar;
            Create = celeb.celeb_Create;
            CreateAccount = AccountManager.GetAccount(celeb.celeb_Create);
            if (celeb.celeb_Status != null)
            {
                Status = (byte)celeb.celeb_Status;
            }
            Note = celeb.celeb_Note;
        }
        public ManageCelebViewModel()
        {
        }
    }

    public class CreateCelebViewModel
    {
        [Display(Name = "豆瓣编号")]
        [Required(ErrorMessage = "豆瓣编号不能为空")]
        [DataType(DataType.MultilineText)]
        public string DoubanID { get; set; }
    }

    public class RefreshCelebViewModel
    {
        public ManageCelebViewModel Old { get; set; }
        public ManageCelebViewModel New { get; set; }
    }
}
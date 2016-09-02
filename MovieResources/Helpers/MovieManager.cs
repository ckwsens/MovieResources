using MovieResources.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace MovieResources.Helpers
{
    public class MovieManager
    {
        /// <summary>
        /// 从豆瓣返回的json中提取电影信息
        /// </summary>
        /// <param name="json">豆瓣电影json</param>
        /// <param name="mappath">电影海报保存地址</param>
        /// <returns></returns>
        public static tbl_Movie JsonToMovie(JObject json, string mappath)
        {
            tbl_Movie movie = new tbl_Movie();
            //电影名
            movie.movie_Title = json["title"].ToString();
            //外文名
            movie.movie_TitleEn = json["original_title"].ToString();

            //又名
            System.Text.StringBuilder temp = new System.Text.StringBuilder();
            var aka = json["aka"].Children().Values();
            foreach (var item in aka)
            {
                temp.Append(item.ToString()).Append("/");
            }
            if (temp.Length > 0)
            {
                movie.movie_Aka = temp.Remove(temp.Length - 1, 1).ToString();
            }

            movie.movie_Summary = json["summary"].ToString();//剧情简介
            movie.movie_DoubanID = json["id"].ToString();//豆瓣编号
            movie.movie_Year = json["year"].ToString();//年代

            //类型
            temp.Clear();
            var genre = json["genres"].Children().Values();
            foreach (var item in genre)
            {
                temp.Append(item.ToString()).Append("/");
            }
            if (temp.Length > 0)
            {
                movie.movie_Genres = Translator.GenreToId(temp.Remove(temp.Length - 1, 1).ToString());
            }

            //制片国家/地区
            temp.Clear();
            var country = json["countries"].Children().Values();
            foreach (var item in country)
            {
                temp.Append(item.ToString()).Append("/");
            }
            if (temp.Length > 0)
            {
                movie.movie_Countries = Translator.CountryToId(temp.Remove(temp.Length - 1, 1).ToString());
            }

            temp.Clear();
            temp.Append(json["rating"]["average"].ToString());//评分
            if (temp.Length == 1)
            {
                temp.Append(".0");
            }
            movie.movie_Rating = temp.ToString();
            movie.movie_RatingCount = json["ratings_count"].ToString();//评分人数

            //海报
            string imgurl = json["images"]["large"].ToString();
            System.IO.MemoryStream ms = new System.IO.MemoryStream(HtmlDecoder.GetImage(imgurl));
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
            Uri uri = new Uri(imgurl);
            string filePath = mappath + uri.Segments[uri.Segments.Length - 1];
            img.Save(filePath);
            movie.movie_Avatar = uri.Segments[uri.Segments.Length - 1];

            //导演
            string sHtmlCode = HtmlDecoder.GetHtmlCode(string.Format("{0}{1}{2}", "http://movie.douban.com/subject/", movie.movie_DoubanID, "/"));
            string sDivInfo = HtmlDecoder.CutString(sHtmlCode, "<div id=\"info\">", "</div>");
            if (sDivInfo.Contains(">导演"))
            {
                movie.movie_Directors = HtmlDecoder.SplitRegex(HtmlDecoder.CutString(sDivInfo, "<span class='pl'>导演</span>", "</span></span><br/>"), "<a[^>]+>([^<]+)</a>");
                movie.movie_DirectorsId = HtmlDecoder.SplitRegex(HtmlDecoder.CutString(sDivInfo, "<span class='pl'>导演</span>", "</span></span><br/>"), "<a[^>]+>([^<]+)</a>", "href=\"/celebrity/", "/\"");
                temp.Clear();
                MR_DataClassesDataContext _db = new MR_DataClassesDataContext();
                foreach (var item in movie.movie_DirectorsId.Split('/'))
                {
                    var director = _db.tbl_Celebrity.SingleOrDefault(c => c.celeb_DoubanID != null && c.celeb_DoubanID == item);
                    if (director == null)
                    {
                        temp.Append(item.Trim()).Append("/");
                    }
                    else
                    {
                        temp.Append(director.celeb_Id).Append("/");
                    }
                }
                movie.movie_DirectorsId = temp.Remove(temp.Length - 1, 1).ToString();
            }
            //编剧
            if (sDivInfo.Contains(">编剧"))
            {
                movie.movie_Writers = HtmlDecoder.SplitRegex(HtmlDecoder.CutString(sDivInfo, "<span class='pl'>编剧</span>: <span class='attrs'>", "</span></span><br/>"), "<a[^>]+>([^<]+)</a>");
                movie.movie_WritersId = HtmlDecoder.SplitRegex(HtmlDecoder.CutString(sDivInfo, "<span class='pl'>编剧</span>: <span class='attrs'>", "</span></span><br/>"), "<a[^>]+>([^<]+)</a>", "href=\"/celebrity/", "/\"");
                temp.Clear();
                MR_DataClassesDataContext _db = new MR_DataClassesDataContext();
                foreach (var item in movie.movie_WritersId.Split('/'))
                {
                    var writer = _db.tbl_Celebrity.SingleOrDefault(c => c.celeb_DoubanID != null && c.celeb_DoubanID == item);
                    if (writer == null)
                    {
                        temp.Append(item.Trim()).Append("/");
                    }
                    else
                    {
                        temp.Append(writer.celeb_Id).Append("/");
                    }
                }
                movie.movie_WritersId = temp.Remove(temp.Length - 1, 1).ToString();
            }
            //主演
            if (sDivInfo.Contains(">主演"))
            {
                movie.movie_Casts = HtmlDecoder.SplitRegex(HtmlDecoder.CutString(sDivInfo, "<span class=\"actor\"><span class='pl'>主演</span>: <span class='attrs'>", "</span></span><br/>"), "<a[^>]+>([^<]+)</a>");
                movie.movie_CastsId = HtmlDecoder.SplitRegex(HtmlDecoder.CutString(sDivInfo, "<span class=\"actor\"><span class='pl'>主演</span>: <span class='attrs'>", "</span></span><br/>"), "<a[^>]+>([^<]+)</a>", "href=\"/celebrity/", "/\"");
                temp.Clear();
                MR_DataClassesDataContext _db = new MR_DataClassesDataContext();
                foreach (var item in movie.movie_CastsId.Split('/'))
                {
                    var cast = _db.tbl_Celebrity.SingleOrDefault(c => c.celeb_DoubanID != null && c.celeb_DoubanID == item);
                    if (cast == null)
                    {
                        temp.Append(item.Trim()).Append("/");
                    }
                    else
                    {
                        temp.Append(cast.celeb_Id).Append("/");
                    }
                }
                movie.movie_CastsId = temp.Remove(temp.Length - 1, 1).ToString();
            }
            //语言
            if (sDivInfo.Contains(">语言"))
            {
                string sLangs = HtmlDecoder.CutString(sDivInfo, "<span class=\"pl\">语言:</span>", "<br/>");
                movie.movie_Languages = Translator.LangToId(HtmlDecoder.SplitSlash(sLangs));
            }
            //上映日期
            if (sDivInfo.Contains(">上映日期"))
            {
                movie.movie_Pubdates = HtmlDecoder.SplitRegex(HtmlDecoder.CutString(sDivInfo, "<span class=\"pl\">上映日期:</span>", "<br/>"), "<span[^>]+>([^<]+)</span>");
            }
            //片长
            if (sDivInfo.Contains(">片长"))
            {
                movie.movie_Durations = HtmlDecoder.CutString(HtmlDecoder.CutString(sDivInfo, "<span class=\"pl\">片长:</span>", "<br/>"), "\">", "</span>").Trim();
            }
            //IMDb链接
            if (sDivInfo.Contains(">IMDb链接"))
            {
                movie.movie_IMDbID = HtmlDecoder.CutString(HtmlDecoder.CutString(sDivInfo, "<span class=\"pl\">IMDb链接:</span>", "<br>"), "\">", "</a>").Trim();
            }

            return movie;
        }

        /// <summary>
        /// 通过json创建电影
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="mappath">电影海报保存地址</param>
        /// <param name="create">电影创建者</param>
        /// <returns></returns>
        public static string CreateJson(JObject json, string mappath, string create)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var createMovie = new tbl_Movie();
                createMovie = MovieManager.JsonToMovie(json, mappath);

                string guid;
                do
                {
                    guid = Guid.NewGuid().ToString("N").ToUpper();
                } while (_db.tbl_Movie.Where(p => p.movie_Id == guid).Count() != 0);
                createMovie.movie_Id = guid;
                createMovie.movie_Create = create;
                createMovie.movie_VisitCount = 0;
                if ((bool)_db.tbl_UserAccount.SingleOrDefault(u => u.user_Id == create).user_IsAdmin)
                {
                    createMovie.movie_Status = 2;
                }
                else
                {
                    createMovie.movie_Status = 0;
                }

                _db.tbl_Movie.InsertOnSubmit(createMovie);
                _db.SubmitChanges();
                _db.SetMovieTime(guid);

                return createMovie.movie_Id;
            }
        }

        /// <summary>
        /// 创建电影
        /// </summary>
        /// <param name="movie"></param>
        /// <returns>电影id</returns>
        public static string CreateMovie(ManageMovieViewModel movie)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                string guid;
                do
                {
                    guid = Guid.NewGuid().ToString("N").ToUpper();
                } while (_db.tbl_Movie.Where(p => p.movie_Id == guid).Count() != 0);

                if (!string.IsNullOrEmpty(movie.Genres) && !string.IsNullOrWhiteSpace(movie.Genres))
                {
                    movie.Genres = Translator.GenreToId(movie.Genres);
                }
                if (!string.IsNullOrEmpty(movie.Countries) && !string.IsNullOrWhiteSpace(movie.Countries))
                {
                    movie.Countries = Translator.CountryToId(movie.Countries);
                }
                if (!string.IsNullOrEmpty(movie.Languages) && !string.IsNullOrWhiteSpace(movie.Countries))
                {
                    movie.Languages = Translator.LangToId(movie.Languages);
                }

                var createMovie = new tbl_Movie()
                {
                    movie_Id = guid,
                    movie_Title = movie.Title,
                    movie_TitleEn = movie.TitleEn,
                    movie_Aka = movie.Aka,
                    movie_Directors = movie.Directors,
                    movie_Writers = movie.Writers,
                    movie_Casts = movie.Casts,
                    movie_Durations = movie.Durations,
                    movie_Genres = movie.Genres,
                    movie_Countries = movie.Countries,
                    movie_Languages = movie.Languages,
                    movie_Pubdates = movie.Pubdates,
                    movie_Year = movie.Year,
                    movie_Rating = movie.Rating,
                    movie_RatingCount = movie.RatingCount,
                    movie_Summary = movie.Summary,
                    movie_DoubanID = movie.DoubanID,
                    movie_IMDbID = movie.IMDbID,
                    movie_VisitCount = 0,
                    movie_Create = movie.Create,
                    movie_Status = movie.Status,
                    movie_Avatar = movie.Avatar == null ? "Movie_Default.png" : movie.Avatar
                };
                _db.tbl_Movie.InsertOnSubmit(createMovie);
                _db.SubmitChanges();
                _db.SetMovieTime(guid);

                return createMovie.movie_Id;
            }
        }

        /// <summary>
        /// 审核用户创建的电影通过
        /// </summary>
        /// <param name="id">电影id</param>
        public static void Audit(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Movie movie = _db.tbl_Movie.SingleOrDefault(m => m.movie_Id == id);
                movie.movie_Status = 2;

                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 审核用户创建的电影不通过
        /// </summary>
        /// <param name="model"></param>
        public static void Reject(RejectViewModel model)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Movie movie = _db.tbl_Movie.SingleOrDefault(m => m.movie_Id == model.Id);
                movie.movie_Status = 1;
                movie.movie_Note = model.Note;
                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 修改电影
        /// </summary>
        /// <param name="movie"></param>
        public static void Edit(ManageMovieViewModel movie)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                bool hasChange = false;
                var oldMovie = _db.tbl_Movie.SingleOrDefault(p => p.movie_Id == movie.Id);

                #region 是否发生变化
                if (oldMovie.movie_Title != movie.Title)
                {
                    oldMovie.movie_Title = movie.Title;
                    hasChange = true;
                }
                if (oldMovie.movie_TitleEn != movie.TitleEn)
                {
                    oldMovie.movie_TitleEn = movie.TitleEn;
                    hasChange = true;
                }
                if (oldMovie.movie_Aka != movie.Aka)
                {
                    oldMovie.movie_Aka = movie.Aka;
                    hasChange = true;
                }
                if (oldMovie.movie_Directors != movie.Directors)
                {
                    oldMovie.movie_Directors = movie.Directors;
                    hasChange = true;
                }
                if (oldMovie.movie_DirectorsId != movie.DirectorsId)
                {
                    oldMovie.movie_DirectorsId = movie.DirectorsId;
                    hasChange = true;
                }
                if (oldMovie.movie_Writers != movie.Writers)
                {
                    oldMovie.movie_Writers = movie.Writers;
                    hasChange = true;
                }
                if (oldMovie.movie_WritersId != movie.WritersId)
                {
                    oldMovie.movie_WritersId = movie.WritersId;
                    hasChange = true;
                }
                if (oldMovie.movie_CastsId != movie.CastsId)
                {
                    oldMovie.movie_CastsId = movie.CastsId;
                    hasChange = true;
                }
                if (oldMovie.movie_Year != movie.Year)
                {
                    oldMovie.movie_Year = movie.Year;
                    hasChange = true;
                }
                if (oldMovie.movie_Pubdates != movie.Pubdates)
                {
                    oldMovie.movie_Pubdates = movie.Pubdates;
                    hasChange = true;
                }
                if (oldMovie.movie_Durations != movie.Durations)
                {
                    oldMovie.movie_Durations = movie.Durations;
                    hasChange = true;
                }
                string id = string.IsNullOrEmpty(movie.Genres) || string.IsNullOrWhiteSpace(movie.Genres) ? null : Translator.GenreToId(movie.Genres);
                if (oldMovie.movie_Genres != id)
                {
                    oldMovie.movie_Genres = id;
                    hasChange = true;
                }
                id = string.IsNullOrEmpty(movie.Countries) || string.IsNullOrWhiteSpace(movie.Countries) ? null : Translator.CountryToId(movie.Countries);
                if (oldMovie.movie_Countries != id)
                {
                    oldMovie.movie_Countries = id;
                    hasChange = true;
                }
                id = string.IsNullOrEmpty(movie.Languages) || string.IsNullOrWhiteSpace(movie.Languages) ? null : Translator.LangToId(movie.Languages);
                if (oldMovie.movie_Languages != id)
                {
                    oldMovie.movie_Languages = id;
                    hasChange = true;
                }
                if (oldMovie.movie_Rating != movie.Rating)
                {
                    oldMovie.movie_Rating = movie.Rating;
                    hasChange = true;
                }
                if (oldMovie.movie_RatingCount != movie.RatingCount)
                {
                    oldMovie.movie_RatingCount = movie.RatingCount;
                    hasChange = true;
                }
                if (oldMovie.movie_DoubanID != movie.DoubanID)
                {
                    oldMovie.movie_DoubanID = movie.DoubanID;
                    hasChange = true;
                }
                if (oldMovie.movie_IMDbID != movie.IMDbID)
                {
                    oldMovie.movie_IMDbID = movie.IMDbID;
                    hasChange = true;
                }
                if (oldMovie.movie_Avatar != movie.Avatar)
                {
                    oldMovie.movie_Avatar = movie.Avatar;
                    hasChange = true;
                }
                if (oldMovie.movie_Summary != movie.Summary)
                {
                    oldMovie.movie_Summary = movie.Summary;
                    hasChange = true;
                }
                #endregion
                if (hasChange)
                {
                    _db.SubmitChanges();
                    _db.AlterMovieAlterTime(movie.Id);
                }
            }
        }

        /// <summary>
        /// 删除电影
        /// </summary>
        /// <param name="id">电影id</param>
        public static void Delete(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Movie movie = _db.tbl_Movie.SingleOrDefault(s => s.movie_Id == id);
                _db.tbl_Movie.DeleteOnSubmit(movie);
                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 获取电影名
        /// </summary>
        /// <param name="id">电影id</param>
        /// <returns></returns>
        public static string GetTitle(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Movie movie = _db.tbl_Movie.SingleOrDefault(s => s.movie_Id == id);
                if (movie == null)
                {
                    return null;
                }
                else
                {
                    return movie.movie_Title;
                }
            }
        }

        /// <summary>
        /// 访问电影，电影的访问量+1
        /// </summary>
        /// <param name="id">电影id</param>
        public static void Visit(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Movie movie = _db.tbl_Movie.SingleOrDefault(s => s.movie_Id == id);
                movie.movie_VisitCount++;
                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 更新所有电影信息中导演、演员、编剧id
        /// </summary>
        /// <param name="celeb">影人id</param>
        /// <param name="douban">影人豆瓣id</param>
        public static void RefreshCeleb(string celeb, string douban)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var movies = _db.tbl_Movie;

                foreach (var item in movies)
                {
                    if (!string.IsNullOrEmpty(item.movie_DirectorsId) && item.movie_DirectorsId.Contains(douban))
                    {
                        item.movie_DirectorsId = item.movie_DirectorsId.Replace(douban, celeb);
                    }
                    if (!string.IsNullOrEmpty(item.movie_WritersId) && item.movie_WritersId.Contains(douban))
                    {
                        item.movie_WritersId = item.movie_WritersId.Replace(douban, celeb);
                    }
                    if (!string.IsNullOrEmpty(item.movie_CastsId) && item.movie_CastsId.Contains(douban))
                    {
                        item.movie_CastsId = item.movie_CastsId.Replace(douban, celeb);
                    }
                }

                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 电影是否存在
        /// </summary>
        /// <param name="id">电影的id</param>
        /// <returns>存在true，不存在false</returns>
        public static bool Exist(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id) || _db.tbl_Movie.SingleOrDefault(p => p.movie_Id == id) == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
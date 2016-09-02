using MovieResources.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace MovieResources.Helpers
{
    public class CelebManager
    {
        /// <summary>
        /// 从豆瓣返回的json中提取影人信息
        /// </summary>
        /// <param name="json">豆瓣影人json</param>
        /// <param name="mappath">影人海报保存地址</param>
        /// <returns></returns>
        public static tbl_Celebrity JsonToCeleb(JObject json, string mappath)
        {
            tbl_Celebrity celeb = new tbl_Celebrity();
            celeb.celeb_Name = json["name"].ToString();//中文名
            celeb.celeb_NameEn = json["name_en"].ToString();//外文名
            celeb.celeb_Gender = Translator.GenderToId(json["gender"].ToString());//性别
            celeb.celeb_DoubanID = json["id"].ToString();//豆瓣编号
            celeb.celeb_BornPlace = json["born_place"].ToString();//出生地

            //更多中文名
            System.Text.StringBuilder temp = new System.Text.StringBuilder();
            var aka = json["aka"].Children().Values();
            foreach (var item in aka)
            {
                temp.Append(item.ToString()).Append("/");
            }
            if (temp.Length > 0)
            {
                celeb.celeb_Aka = temp.Remove(temp.Length - 1, 1).ToString();
            }

            //更多外文名
            temp = new System.Text.StringBuilder();
            var akaen = json["aka_en"].Children().Values();
            foreach (var item in akaen)
            {
                temp.Append(item.ToString()).Append("/");
            }
            if (temp.Length > 0)
            {
                celeb.celeb_AkaEn = temp.Remove(temp.Length - 1, 1).ToString();
            }


            //照片
            string imgurl = json["avatars"]["large"].ToString();
            System.IO.MemoryStream ms = new System.IO.MemoryStream(HtmlDecoder.GetImage(imgurl));
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
            Uri uri = new Uri(imgurl);
            string filePath = mappath + uri.Segments[uri.Segments.Length - 1];
            img.Save(filePath);
            celeb.celeb_Avatar = uri.Segments[uri.Segments.Length - 1];

            //生卒日期
            string sHtmlCode = HtmlDecoder.GetHtmlCode(string.Format("{0}{1}{2}", "https://movie.douban.com/celebrity/", celeb.celeb_DoubanID, "/"));
            string sDivInfo = HtmlDecoder.CutString(sHtmlCode, "<div id=\"headline\" class=\"item\">", "<div id=\"opt-bar\" class=\"mod\">");
            sDivInfo = HtmlDecoder.CutString(sDivInfo, "<div class=\"info\">", "</div>");
            if (sDivInfo.Contains("<span>出生日期"))
            {
                celeb.celeb_Birthday = HtmlDecoder.CutString(sDivInfo, "<span>出生日期</span>:", "</li>");
            }
            else if (sDivInfo.Contains("<span>生卒日期"))
            {
                celeb.celeb_Birthday = HtmlDecoder.CutString(sDivInfo, "<span>生卒日期</span>:", "</li>").Split('至')[0].Trim();
                celeb.celeb_Deathday = HtmlDecoder.CutString(sDivInfo, "<span>生卒日期</span>:", "</li>").Split('至')[1].Trim();
            }

            //职业
            if (sDivInfo.Contains("<span>职业"))
            {
                string pros = HtmlDecoder.CutString(sDivInfo, "<span>职业</span>:", "</li>");
                temp.Clear();
                foreach (var item in pros.Split('/'))
                {
                    temp.Append(item.Trim()).Append("/");
                }
                if (temp.Length > 0)
                {
                    celeb.celeb_Pro = temp.Remove(temp.Length - 1, 1).ToString();
                }
            }

            //家庭成员
            if (sDivInfo.Contains("<span>家庭成员"))
            {
                string families = HtmlDecoder.CutString(sDivInfo, "<span>家庭成员</span>:", "</li>");
                temp.Clear();
                foreach (var item in families.Split('/'))
                {
                    temp.Append(item.Trim()).Append("/");
                }
                if (temp.Length > 0)
                {
                    celeb.celeb_Family = temp.Remove(temp.Length - 1, 1).ToString();
                }
            }

            //imdb编号        http://www.imdb.com/name/nm0000701
            if (sDivInfo.Contains("<span>imdb编号"))
            {
                celeb.celeb_IMDbID = HtmlDecoder.CutString(HtmlDecoder.CutString(sDivInfo, "<span>imdb编号</span>:", "</li>"), "target=\"_self\">", "</a>");
            }

            //影人简介
            sDivInfo = HtmlDecoder.CutString(sHtmlCode, "<div id=\"intro\" class=\"mod\">", "<div class=\"mod\">");
            if (sDivInfo.Contains("<span class=\"all hidden\">"))
            {
                celeb.celeb_Summary = HtmlDecoder.CutString(sDivInfo, "<span class=\"all hidden\">", "</span>").Replace("<br/>", "$").Trim();
            }
            else
            {
                celeb.celeb_Summary = HtmlDecoder.CutString(sDivInfo, "<div class=\"bd\">", "</div>").Replace("<br/>", "$").Trim();
            }
            string[] summarys = celeb.celeb_Summary.Split('$');
            temp.Clear();
            foreach (var item in summarys)
            {
                if (string.IsNullOrEmpty(item) || string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                temp.Append("　　").Append(item.Trim()).Append('\n');
            }
            celeb.celeb_Summary = temp.ToString();

            return celeb;
        }

        /// <summary>
        /// 通过json创建影人
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="mappath">影人海报保存地址</param>
        /// <param name="create">影人创建者</param>
        /// <returns></returns>
        public static string CreateJson(JObject json, string mappath, string create)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var createCeleb = new tbl_Celebrity();
                createCeleb = CelebManager.JsonToCeleb(json, mappath);

                string guid;
                do
                {
                    guid = Guid.NewGuid().ToString("N").ToUpper();
                } while (_db.tbl_Celebrity.Where(p => p.celeb_Id == guid).Count() != 0);
                createCeleb.celeb_Id = guid;
                createCeleb.celeb_Create = create;
                if ((bool)_db.tbl_UserAccount.SingleOrDefault(u => u.user_Id == create).user_IsAdmin)
                {
                    createCeleb.celeb_Status = 2;
                }
                else
                {
                    createCeleb.celeb_Status = 0;
                }

                _db.tbl_Celebrity.InsertOnSubmit(createCeleb);
                _db.SubmitChanges();
                _db.SetCelebTime(guid);

                if (!string.IsNullOrEmpty(createCeleb.celeb_DoubanID))
                {
                    MovieManager.RefreshCeleb(createCeleb.celeb_Id, createCeleb.celeb_DoubanID);
                }

                return createCeleb.celeb_Id;
            }
        }

        /// <summary>
        /// 创建影人
        /// </summary>
        /// <param name="celeb"></param>
        /// <returns></returns>
        public static string CreateCeleb(ManageCelebViewModel celeb)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                string guid;
                do
                {
                    guid = Guid.NewGuid().ToString("N").ToUpper();
                } while (_db.tbl_Celebrity.Where(p => p.celeb_Id == guid).Count() != 0);
                celeb.Gender = Translator.GenderToId(celeb.Gender);

                var createCeleb = new tbl_Celebrity()
                {
                    celeb_Id = guid,
                    celeb_Name = celeb.Name,
                    celeb_NameEn = celeb.NameEn,
                    celeb_Aka = celeb.Aka,
                    celeb_AkaEn = celeb.AkaEn,
                    celeb_Gender = celeb.Gender,
                    celeb_Birthday = celeb.Birthday,
                    celeb_Deathday = celeb.Deathday,
                    celeb_BornPlace = celeb.BornPlace,
                    celeb_Pro = celeb.Pro,
                    celeb_Family = celeb.Family,
                    celeb_Summary = celeb.Summary,
                    celeb_DoubanID = celeb.DoubanID,
                    celeb_IMDbID = celeb.IMDbID,
                    celeb_Create = celeb.Create,
                    celeb_Status = celeb.Status,
                    celeb_Avatar = celeb.Avatar == null ? "Celeb_1.jpg" : celeb.Avatar
                };
                _db.tbl_Celebrity.InsertOnSubmit(createCeleb);
                _db.SubmitChanges();
                _db.SetCelebTime(guid);

                return createCeleb.celeb_Id;
            }
        }

        /// <summary>
        /// 审核影人通过
        /// </summary>
        /// <param name="id">影人id</param>
        public static void Audit(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Celebrity celeb = _db.tbl_Celebrity.SingleOrDefault(m => m.celeb_Id == id);
                celeb.celeb_Status = 2;

                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 审核影人不通过
        /// </summary>
        /// <param name="model"></param>
        public static void Reject(RejectViewModel model)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Celebrity celeb = _db.tbl_Celebrity.SingleOrDefault(m => m.celeb_Id == model.Id);
                celeb.celeb_Status = 1;
                celeb.celeb_Note = model.Note;
                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 修改影人
        /// </summary>
        /// <param name="celeb"></param>
        public static void Edit(ManageCelebViewModel celeb)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                bool hasChange = false;
                var oldCeleb = _db.tbl_Celebrity.SingleOrDefault(p => p.celeb_Id == celeb.Id);

                #region 是否修改过
                if (oldCeleb.celeb_Name != celeb.Name)
                {
                    oldCeleb.celeb_Name = celeb.Name;
                    hasChange = true;
                }
                if (oldCeleb.celeb_NameEn != celeb.NameEn)
                {
                    oldCeleb.celeb_NameEn = celeb.NameEn;
                    hasChange = true;
                }
                if (oldCeleb.celeb_Aka != celeb.Aka)
                {
                    oldCeleb.celeb_Aka = celeb.Aka;
                    hasChange = true;
                }
                if (oldCeleb.celeb_AkaEn != celeb.AkaEn)
                {
                    oldCeleb.celeb_AkaEn = celeb.AkaEn;
                    hasChange = true;
                }
                if (oldCeleb.celeb_Gender != Translator.GenderToId(celeb.Gender))
                {
                    oldCeleb.celeb_Gender = Translator.GenderToId(celeb.Gender);
                    hasChange = true;
                }
                if (oldCeleb.celeb_Birthday != celeb.Birthday)
                {
                    oldCeleb.celeb_Birthday = celeb.Birthday;
                    hasChange = true;
                }
                if (oldCeleb.celeb_BornPlace != celeb.BornPlace)
                {
                    oldCeleb.celeb_BornPlace = celeb.BornPlace;
                    hasChange = true;
                }
                if (oldCeleb.celeb_Pro != celeb.Pro)
                {
                    oldCeleb.celeb_Pro = celeb.Pro;
                    hasChange = true;
                }
                if (oldCeleb.celeb_Family != celeb.Family)
                {
                    oldCeleb.celeb_Family = celeb.Family;
                    hasChange = true;
                }
                if (oldCeleb.celeb_DoubanID != celeb.DoubanID)
                {
                    oldCeleb.celeb_DoubanID = celeb.DoubanID;
                    hasChange = true;
                }
                if (oldCeleb.celeb_IMDbID != celeb.IMDbID)
                {
                    oldCeleb.celeb_IMDbID = celeb.IMDbID;
                    hasChange = true;
                }
                if (oldCeleb.celeb_Summary != celeb.Summary)
                {
                    oldCeleb.celeb_Summary = celeb.Summary;
                    hasChange = true;
                }
                if (oldCeleb.celeb_Avatar != celeb.Avatar)
                {
                    oldCeleb.celeb_Avatar = celeb.Avatar;
                    hasChange = true;
                }
                #endregion

                if (hasChange)
                {
                    _db.SubmitChanges();
                    _db.AlterCelebAlterTime(celeb.Id);
                }
            }
        }

        /// <summary>
        /// 删除影人
        /// </summary>
        /// <param name="id"></param>
        public static void Delete(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Celebrity celeb = _db.tbl_Celebrity.SingleOrDefault(s => s.celeb_Id == id);
                _db.tbl_Celebrity.DeleteOnSubmit(celeb);
                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 检查影人是否存在
        /// </summary>
        /// <param name="id">影人id</param>
        /// <returns>存在true，不存在false</returns>
        public static bool Exist(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id) || _db.tbl_Celebrity.SingleOrDefault(p => p.celeb_Id == id) == null)
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
using System.Collections.Generic;
using System.Linq;

namespace MovieResources.Helpers
{
    /// <summary>
    /// 电影类型、国家/地区、语言的数据库ID与名称之间的转换
    /// </summary>
    public class Translator
    {
        /// <summary>
        /// 类型id转类型名
        /// </summary>
        /// <param name="id">多个id用“/”隔开</param>
        /// <returns>转换后的类型id</returns>
        public static string GenreToName(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                List<string> genres = new List<string>();
                foreach (string item in id.Split('/'))
                {
                    genres.Add(_db.tbl_GenreMovie.SingleOrDefault(genre => genre.genre_Id == byte.Parse(item)).genre_Name);
                }
                return string.Join(" / ", genres);
            }
        }

        /// <summary>
        /// 类型名传类型id
        /// </summary>
        /// <param name="name">多个name用“/”隔开</param>
        /// <returns>转换后的类型id</returns>
        public static string GenreToId(string name)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                List<int> genres = new List<int>();
                foreach (string item in name.Split('/'))
                {
                    tbl_GenreMovie tblGenre = _db.tbl_GenreMovie.SingleOrDefault(p => p.genre_Name == item);
                    if (tblGenre == null)
                    {
                        genres.Add(_db.InsertGenre(item));
                    }
                    else
                    {
                        genres.Add(_db.tbl_GenreMovie.SingleOrDefault(p => p.genre_Name == item).genre_Id);
                    }
                }
                return string.Join("/", genres);
            }
        }

        /// <summary>
        /// 国家id转国家名
        /// </summary>
        /// <param name="id">多个id用“/”隔开</param>
        /// <returns>转换后的国家名</returns>
        public static string CountryToName(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                List<string> countries = new List<string>();
                foreach (string item in id.Split('/'))
                {
                    countries.Add(_db.tbl_Country.SingleOrDefault(country => country.country_Id == byte.Parse(item)).country_Name);
                }
                return string.Join(" / ", countries);
            }
        }

        /// <summary>
        /// 国家名转国家id
        /// </summary>
        /// <param name="name">多个name用“/”隔开</param>
        /// <returns>转换后的国家id</returns>
        public static string CountryToId(string name)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                List<int> countries = new List<int>();
                foreach (string item in name.Split('/'))
                {
                    tbl_Country tblCountry = _db.tbl_Country.SingleOrDefault(p => p.country_Name == item);
                    if (tblCountry == null)
                    {
                        countries.Add(_db.InsertCountry(item));
                    }
                    else
                    {
                        countries.Add(_db.tbl_Country.SingleOrDefault(p => p.country_Name == item).country_Id);
                    }
                }
                return string.Join("/", countries);
            }
        }

        /// <summary>
        /// 语言id转语言名
        /// </summary>
        /// <param name="id">多个id用“/”隔开</param>
        /// <returns>转换后的语言名</returns>
        public static string LangToName(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                List<string> langs = new List<string>();
                foreach (string item in id.Split('/'))
                {
                    langs.Add(_db.tbl_Language.SingleOrDefault(lang => lang.lang_Id == byte.Parse(item)).lang_Name);
                }
                return string.Join(" / ", langs);
            }
        }

        /// <summary>
        /// 语言名传语言id
        /// </summary>
        /// <param name="name">多个name用“/”隔开</param>
        /// <returns>转换后的语言id</returns>
        public static string LangToId(string name)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                List<int> languages = new List<int>();
                foreach (string item in name.Split('/'))
                {
                    tbl_Language tblLang = _db.tbl_Language.SingleOrDefault(p => p.lang_Name == item);
                    if (tblLang == null)
                    {
                        languages.Add(_db.InsertLang(item));
                    }
                    else
                    {
                        languages.Add(_db.tbl_Language.SingleOrDefault(p => p.lang_Name == item).lang_Id);
                    }
                }
                return string.Join("/", languages);
            }
        }

        /// <summary>
        /// 性别编号转性别名
        /// </summary>
        /// <param name="id">性别编号</param>
        /// <returns>性别名</returns>
        public static string GenderToTitle(string id)
        {
            if (id == "1")
            {
                return "男";
            }
            else if (id == "2")
            {
                return "女";
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 性别名转性别编号
        /// </summary>
        /// <param name="title">性别名</param>
        /// <returns>性别编号</returns>
        public static string GenderToId(string title)
        {
            if (title == "男")
            {
                return "1";
            }
            else if (title == "女")
            {
                return "2";
            }
            else
            {
                return "1";
            }
        }

        /// <summary>
        /// url里有key的值，就替换为value,没有的话就追加.
        /// </summary>
        /// <param name="url">原url</param>
        /// <param name="ParamText">参数名</param>
        /// <param name="ParamValue">参数值</param>
        /// <returns></returns>
        public static string BuildUrl(string url, string ParamText, string ParamValue)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(string.Format("{0}=[^&]*", ParamText), System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex("[&]{2,}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            string _url = reg.Replace(url, "");
            //_url = reg1.Replace(_url, "");
            if (_url.IndexOf("?") == -1)
            {
                _url += string.Format("?{0}={1}", ParamText, ParamValue);//?
            }
            else
            {
                _url += string.Format("&{0}={1}", ParamText, ParamValue);//&
            }
            _url = reg1.Replace(_url, "&");
            _url = _url.Replace("?&", "?");
            return _url;
        }

        /// <summary>
        /// unicode编码转中文
        /// </summary>
        /// <param name="unicode">unicode编码</param>
        /// <returns>中文</returns>
        public static string UnicodeToChinese(string unicode)
        {
            string outStr = "";
            if (!string.IsNullOrEmpty(unicode) && !string.IsNullOrWhiteSpace(unicode))
            {
                string[] strlist = unicode.Replace("//", "").Split('u');
                try
                {
                    for (int i = 1; i < strlist.Length; i++)
                    {
                        //将unicode字符转为10进制整数，然后转为char中文字符  
                        outStr += (char)int.Parse(strlist[i], System.Globalization.NumberStyles.HexNumber);
                    }
                }
                catch (System.FormatException ex)
                {
                    outStr = ex.Message;
                }
            }
            return outStr;
        }
    }
}
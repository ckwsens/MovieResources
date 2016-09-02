using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace MovieResources.Helpers
{
    public class HtmlDecoder
    {
        /// <summary>
        /// 获取豆瓣电影返回数据
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns>Json数据结果</returns>
        public static Newtonsoft.Json.Linq.JObject GetJson(string url)
        {
            try
            {
                WebClient myWebClient = new WebClient();
                Stream myStream = myWebClient.OpenRead(url);
                StreamReader sr = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
                string strJson = sr.ReadToEnd();
                myStream.Close();
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(strJson);
                return json;
            }
            catch (WebException)
            {
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse("{\"msg\":\"movie_not_found\",\"code\":5000}");
                return json;
            }
        }

        /// <summary>
        /// 获取指定网址的网页源码
        /// </summary>
        /// <param name="url">需要获取的网址</param>
        /// <returns>网页源码字符串</returns>
        public static string GetHtmlCode(string url)
        {
            try
            {
                WebClient myWebClient = new WebClient();
                Stream myStream = myWebClient.OpenRead(url);
                StreamReader sr = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
                string strHTML = sr.ReadToEnd();
                myStream.Close();
                return strHTML;
            }
            catch (WebException)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取图片数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns>二进制图片数据</returns>
        public static byte[] GetImage(string url)
        {
            try
            {
                WebClient wc = new WebClient();
                return wc.DownloadData(url);
            }
            catch (WebException)
            {
                return null;
            }
        }

        /// <summary>
        /// 从源字符串中截取两个指定字符串之间的字符串
        /// </summary>
        /// <param name="sOperated">源字符串</param>
        /// <param name="sBegin">开始位置的字符串</param>
        /// <param name="sEnd">结束位置的字符串</param>
        /// <returns>处理后字符串</returns>
        public static string CutString(string sOperated, string sBegin, string sEnd)
        {
            string sResult = string.Empty;

            int iBegin = sOperated.IndexOf(sBegin, 0) + sBegin.Length;   //开始位置偏移量 
            int iEnd = sOperated.IndexOf(sEnd, iBegin);//结束位置偏移量
            try
            {
                sResult = sOperated.Substring(iBegin, iEnd - iBegin);
            }
            catch (Exception)
            {
                return null;
            }
            return sResult.Trim();
        }

        /// <summary>
        /// 从源字符串中截取指定位置开始到指定字符串的字符串
        /// </summary>
        /// <param name="sOperated">源字符串</param>
        /// <param name="iBegin">起始地址</param>
        /// <param name="sEnd">结束位置的字符串</param>
        /// <returns>处理后字符串</returns>
        public static string CutString(string sOperated, int iBegin, string sEnd)
        {
            string sResult = string.Empty;

            int iEnd = sOperated.IndexOf(sEnd, iBegin);//结束位置偏移量
            try
            {
                sResult = sOperated.Substring(iBegin, iEnd - iBegin);
            }
            catch (Exception)
            {
                return null;
            }
            return sResult.Trim();
        }

        /// <summary>
        /// 从源字符串中截取指定字符串位置开始的字符串
        /// </summary>
        /// <param name="sOperated">源字符串</param>
        /// <param name="sBegin">起始地址</param>
        /// <returns>处理后字符串</returns>
        public static string CutString(string sOperated, string sBegin)
        {
            string sResult = string.Empty;

            int iBegin = sOperated.IndexOf(sBegin, 0) + sBegin.Length;   //开始位置偏移量 
            int iEnd = sOperated.Length;//结束位置偏移量
            try
            {
                sResult = sOperated.Substring(iBegin, iEnd - iBegin);
            }
            catch (Exception)
            {
                return null;
            }
            return sResult.Trim();
        }

        /// <summary>
        /// 按正则表达式处理字符串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="regex">正则表达式</param>
        /// <returns>处理后字符串</returns>
        public static string SplitRegex(string str, string regex)
        {
            System.Text.StringBuilder temp = new System.Text.StringBuilder();
            Regex rWriters = new Regex(regex, RegexOptions.IgnoreCase);
            MatchCollection mcWriters = rWriters.Matches(str);
            foreach (Match item in mcWriters)
            {
                temp.Append(HtmlDecoder.CutString(item.ToString(), ">", "<")).Append("/");
            }
            return temp.Remove(temp.Length - 1, 1).ToString();
        }

        /// <summary>
        /// 按正则表达式处理字符串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="regex">正则表达式</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns>处理后字符串</returns>
        public static string SplitRegex(string str, string regex, string begin, string end)
        {
            System.Text.StringBuilder temp = new System.Text.StringBuilder();
            Regex rWriters = new Regex(regex, RegexOptions.IgnoreCase);
            MatchCollection mcWriters = rWriters.Matches(str);
            foreach (Match item in mcWriters)
            {
                temp.Append(HtmlDecoder.CutString(item.ToString(), begin, end)).Append("/");
            }
            return temp.Remove(temp.Length - 1, 1).ToString();
        }

        /// <summary>
        /// 按/处理字符串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <returns>处理后字符串</returns>
        public static string SplitSlash(string str)
        {
            System.Text.StringBuilder temp = new System.Text.StringBuilder();
            string[] langs = str.Split('/');
            foreach (var item in langs)
            {
                temp.Append(item.Trim()).Append("/");
            }
            return temp.Remove(temp.Length - 1, 1).ToString();
        }
    }
}
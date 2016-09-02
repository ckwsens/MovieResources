using System.Collections.Generic;
using System.Text;

namespace MovieResources.Helpers
{
    public static class Controls
    {
        /// <summary>
        /// 生成分页
        /// </summary>
        /// <param name="page">当前页码</param>
        /// <param name="size">总页码数</param>
        /// <param name="url">当前页面url</param>
        /// <returns></returns>
        public static System.Web.Mvc.MvcHtmlString PageTagList(int page, int size, int count, string url)
        {
            StringBuilder sbPageTag = new StringBuilder();
            sbPageTag.Append("<nav class=\"text-center\"><ul class=\"pagination\">");
            if (page == 1)
            {
                sbPageTag.Append("<li class=\"disabled\"><a href=\"javascript:void(0)\" aria-label=\"Previous\"><span aria-hidden=\"true\">上一页</span></a></li>");
            }
            else
            {
                sbPageTag.Append("<li><a href=\"").Append(Translator.BuildUrl(url, "page", (page - 1).ToString())).Append("\" aria-label=\"Previous\"><span aria-hidden=\"true\">上一页</span></a></li>");
            }

            List<int> sbPageNumbers = new List<int>();
            for (int i = page; i <= count && sbPageNumbers.Count < size; i++)
            {
                sbPageNumbers.Add(i);
            }
            if (sbPageNumbers.Count < size)
            {
                for (int i = page - 1; i >= 1 && sbPageNumbers.Count < size; i--)
                {
                    sbPageNumbers.Insert(0, i);
                }
            }
            foreach (var item in sbPageNumbers)
            {
                if (item == page)
                {
                    sbPageTag.Append("<li class=\"active\"><a href=\"").Append(Translator.BuildUrl(url, "page", item.ToString())).Append("\">").Append(item).Append("</a></li>");
                }
                else
                {
                    sbPageTag.Append("<li><a href=\"").Append(Translator.BuildUrl(url, "page", item.ToString())).Append("\">").Append(item).Append("</a></li>");
                }
            }

            if (page == count)
            {
                sbPageTag.Append("<li class=\"disabled\"><a href=\"javascript:void(0)\" aria-label=\"Previous\"><span aria-hidden=\"true\">下一页</span></a></li>");
            }
            else
            {
                sbPageTag.Append("<li><a href=\"").Append(Translator.BuildUrl(url, "page", (page + 1).ToString())).Append("\" aria-label=\"Previous\"><span aria-hidden=\"true\">下一页</span></a></li>");
            }
            sbPageTag.Append("</ul></nav>");
            return System.Web.Mvc.MvcHtmlString.Create(sbPageTag.ToString());
        }
    }
}
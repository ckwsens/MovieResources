using System;
using System.Linq;

namespace MovieResources.Helpers
{
    public class CommentManager
    {
        /// <summary>
        /// 创建评论
        /// </summary>
        /// <param name="content">评论内容</param>
        /// <param name="movie">评论的电影id</param>
        /// <param name="user">用户id</param>
        /// <returns>评论id</returns>
        public static string Create(string content, string movie, string user)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Comment cmt = new tbl_Comment()
                {
                    cmt_Content = content,
                    cmt_User = user,
                    cmt_Movie = movie
                };

                string guid;
                do
                {
                    guid = Guid.NewGuid().ToString("N").ToUpper();
                } while (_db.tbl_Comment.Where(p => p.cmt_Id == guid).Count() != 0);
                cmt.cmt_Id = guid;

                _db.tbl_Comment.InsertOnSubmit(cmt);
                _db.SubmitChanges();
                _db.SetCmtTime(guid);

                return cmt.cmt_Id;
            }
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="id">评论id</param>
        public static void Delete(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var cmt = _db.tbl_Comment.SingleOrDefault(a => a.cmt_Id == id);

                _db.tbl_Comment.DeleteOnSubmit(cmt);
                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 检查电影评论是否存在
        /// </summary>
        /// <param name="id">电影评论id</param>
        /// <returns>存在true，不存在false</returns>
        public static bool Exist(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id) || _db.tbl_Comment.SingleOrDefault(p => p.cmt_Id == id) == null)
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
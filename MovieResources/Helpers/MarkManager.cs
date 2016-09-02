using System;
using System.Linq;

namespace MovieResources.Helpers
{
    public class MarkManager
    {
        /// <summary>
        /// 检查是否已经标记过
        /// </summary>
        /// <param name="tagret">标记对象id</param>
        /// <param name="user">用户id</param>
        /// <param name="type">标记类型</param>
        /// <returns>标记过true，否则false</returns>
        public static bool Validate(string tagret, string user, int type)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var marks = from m in _db.tbl_Mark
                            where m.mark_Type == type
                            select m;
                var suit = marks.SingleOrDefault(m => m.mark_User == user && m.mark_Target == tagret);
                if (suit == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 标记
        /// </summary>
        /// <param name="targrt">被标记目标id</param>
        /// <param name="user">用户id</param>
        /// <param name="type">1想看电影，2看过电影，3喜欢电影，4收藏影人，5赞资源，6同求资源，7关注专辑</param>
        public static void Create(string targrt, string user, int type)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                if (MarkManager.Validate(targrt, user, type))
                {
                    return;
                }
                var mark = new tbl_Mark();
                string guid;
                do
                {
                    guid = Guid.NewGuid().ToString("N").ToUpper();
                } while (_db.tbl_Mark.Where(p => p.mark_Id == guid).Count() != 0);
                mark.mark_Id = guid;
                mark.mark_Target = targrt;
                mark.mark_User = user;
                mark.mark_Type = (byte)type;

                _db.tbl_Mark.InsertOnSubmit(mark);
                _db.SubmitChanges();
                _db.SetMarkMovieTime(mark.mark_Id);
            }
        }

        /// <summary>
        /// 取消标记电影
        /// </summary>
        /// <param name="targrt">被标记目标id</param>
        /// <param name="user">用户id</param>
        /// <param name="type">1想看电影，2看过电影，3喜欢电影，4收藏影人，5赞资源，6同求资源，7关注专辑</param>
        public static void Cancel(string targrt, string user, int type)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var mark = _db.tbl_Mark.SingleOrDefault(m => m.mark_User == user && m.mark_Target == targrt && m.mark_Type == type);
                if (mark != null)
                {
                    _db.tbl_Mark.DeleteOnSubmit(mark);
                    _db.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// 检查标记是否存在
        /// </summary>
        /// <param name="id">标记id</param>
        /// <returns>存在true，不存在false</returns>
        public static bool Exist(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id) || _db.tbl_UserAccount.SingleOrDefault(p => p.user_Id == id) == null)
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
using MovieResources.Models;
using System;
using System.Linq;

namespace MovieResources.Helpers
{
    public class AskManager
    {
        /// <summary>
        /// 创建求资源
        /// </summary>
        /// <param name="model"></param>
        public static void Create(ManageAskViewModel model)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Ask ask = new tbl_Ask()
                {
                    ask_Movie = model.MovieId,
                    ask_Note = model.Note,
                    ask_State = false,
                    ask_With = 0
                };
                if (model.User != null)
                {
                    ask.ask_User = model.User;
                }
                string guid;
                do
                {
                    guid = Guid.NewGuid().ToString("N").ToUpper();
                } while (_db.tbl_Ask.Where(p => p.ask_Id == guid).Count() != 0);
                ask.ask_Id = guid;

                _db.tbl_Ask.InsertOnSubmit(ask);
                _db.SubmitChanges();
                _db.SetAskTime(guid);
            }
        }

        /// <summary>
        /// 取消求资源
        /// </summary>
        /// <param name="id">求资源id</param>
        public static void Delete(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Ask ask = _db.tbl_Ask.SingleOrDefault(s => s.ask_Id == id);
                _db.tbl_Ask.DeleteOnSubmit(ask);
                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 求资源已求到
        /// </summary>
        /// <param name="id">求资源id</param>
        public static void Over(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {

                tbl_Ask tblask = _db.tbl_Ask.SingleOrDefault(s => s.ask_Id == id);
                tblask.ask_State = true;

                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 检查求资源是否存在
        /// </summary>
        /// <param name="id">求资源id</param>
        /// <returns>存在true，不存在false</returns>
        public static bool Exist(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id) || _db.tbl_Ask.SingleOrDefault(p => p.ask_Id == id) == null)
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
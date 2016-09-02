using MovieResources.Models;
using System;
using System.Linq;

namespace MovieResources.Helpers
{
    public class ResManager
    {
        /// <summary>
        /// 创建资源
        /// </summary>
        /// <param name="model"></param>
        public static void Create(ManageResViewModel model)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Resource res = new tbl_Resource()
                {
                    res_Movie = model.MovieId,
                    res_Name = model.FileName,
                    res_Content = model.Content,
                    res_Size = float.Parse(model.FileSize),
                    res_Type = model.ResType,
                    res_FavorCount = 0,
                    res_Status = model.Status
                };
                if (model.User != null)
                {
                    res.res_User = model.User;
                }
                string guid;
                do
                {
                    guid = Guid.NewGuid().ToString("N").ToUpper();
                } while (_db.tbl_Resource.Where(p => p.res_Id == guid).Count() != 0);
                res.res_Id = guid;

                _db.tbl_Resource.InsertOnSubmit(res);
                _db.SubmitChanges();
                _db.SetResTime(guid);
            }
        }

        /// <summary>
        /// 删除资源
        /// </summary>
        /// <param name="id">资源id</param>
        public static void Delete(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Resource res = _db.tbl_Resource.SingleOrDefault(s => s.res_Id == id);
                _db.tbl_Resource.DeleteOnSubmit(res);
                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 审核资源通过
        /// </summary>
        /// <param name="id">资源id</param>
        public static void Audit(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Resource res = _db.tbl_Resource.SingleOrDefault(s => s.res_Id == id);
                res.res_Status = 2;

                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 审核资源不通过
        /// </summary>
        /// <param name="model"></param>
        public static void Reject(RejectResViewModel model)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Resource res = _db.tbl_Resource.SingleOrDefault(s => s.res_Id == model.Id);
                res.res_Status = 1;
                res.res_Note = model.Note;
                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 电影资源是否存在
        /// </summary>
        /// <param name="id">电影资源的id</param>
        /// <returns>存在true，不存在false</returns>
        public static bool Exist(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id) || _db.tbl_Resource.SingleOrDefault(p => p.res_Id == id) == null)
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
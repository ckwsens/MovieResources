using MovieResources.Models;
using System;
using System.Linq;

namespace MovieResources.Helpers
{
    public class DiscManager
    {
        /// <summary>
        /// 添加每日发现电影
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string Create(ManageDiscViewModel model)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Discovery disc = new tbl_Discovery()
                {
                    disc_Movie = model.Movie,
                    disc_Image = model.Image
                };

                string guid;
                do
                {
                    guid = Guid.NewGuid().ToString("N").ToUpper();
                } while (_db.tbl_Discovery.Where(p => p.disc_Id == guid).Count() != 0);
                disc.disc_Id = guid;
                disc.disc_Flag = _db.tbl_Discovery.OrderByDescending(d => d.disc_Flag).ToList()[0].disc_Flag + 1;

                _db.tbl_Discovery.InsertOnSubmit(disc);
                _db.SubmitChanges();
                _db.SetDiscTime(guid);

                return disc.disc_Id;
            }
        }

        /// <summary>
        /// 删除每日发现电影
        /// </summary>
        /// <param name="id">电影id</param>
        public static void Delete(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var disc = _db.tbl_Discovery.SingleOrDefault(a => a.disc_Id == id);

                _db.tbl_Discovery.DeleteOnSubmit(disc);
                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 电影发现是否存在
        /// </summary>
        /// <param name="id">电影发现的id</param>
        /// <returns>存在true，不存在false</returns>
        public static bool Exist(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id) || _db.tbl_Discovery.SingleOrDefault(p => p.disc_Id == id) == null)
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
using MovieResources.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieResources.Helpers
{
    public class AlbumManager
    {
        /// <summary>
        /// 创建专辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns>专辑id</returns>
        public static string Create(ManageAlbumViewModel model)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Album album = new tbl_Album()
                {
                    album_Title = model.Title,
                    album_Summary = model.Summary,
                    album_User = model.UserId,
                    album_Cover = model.Cover
                };

                string guid;
                do
                {
                    guid = Guid.NewGuid().ToString("N").ToUpper();
                } while (_db.tbl_Album.Where(p => p.album_Id == guid).Count() != 0);
                album.album_Id = guid;
                album.album_Item = "[]";
                album.album_Visit = 0;

                _db.tbl_Album.InsertOnSubmit(album);
                _db.SubmitChanges();
                _db.SetAlbumTime(guid);

                return album.album_Id;
            }
        }

        /// <summary>
        /// 修改专辑
        /// </summary>
        /// <param name="model"></param>
        public static void Edit(ManageAlbumViewModel model)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var album = _db.tbl_Album.SingleOrDefault(a => a.album_Id == model.Id);

                album.album_Title = model.Title;
                album.album_Summary = model.Summary;
                if (!string.IsNullOrEmpty(model.Cover) && !string.IsNullOrWhiteSpace(model.Cover))
                {
                    album.album_Cover = model.Cover;
                }

                _db.SubmitChanges();
                _db.AlterAlbumAlterTime(album.album_Id);
            }
        }

        /// <summary>
        /// 删除专辑
        /// </summary>
        /// <param name="id">专辑id</param>
        public static void Delete(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var album = _db.tbl_Album.SingleOrDefault(a => a.album_Id == id);

                _db.tbl_Album.DeleteOnSubmit(album);
                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 为专辑添加项目
        /// </summary>
        /// <param name="id">专辑id</param>
        /// <param name="model">添加的项目</param>
        public static void Add(string id, AlbumItemViewModel model)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var album = _db.tbl_Album.SingleOrDefault(a => a.album_Id == id);
                if (_db.tbl_Movie.SingleOrDefault(m => m.movie_Id == model.Movie) == null)
                {
                    return;
                }

                List<AlbumItemViewModel> all = JsonConvert.DeserializeObject<List<AlbumItemViewModel>>(album.album_Item);
                all.Add(model);

                album.album_Item = JsonConvert.SerializeObject(all);
                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 删除专辑项目
        /// </summary>
        /// <param name="id">专辑id</param>
        /// <param name="movie">电影id</param>
        public static void Minus(string id, string movie)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var album = _db.tbl_Album.SingleOrDefault(a => a.album_Id == id);

                List<AlbumItemViewModel> all = JsonConvert.DeserializeObject<List<AlbumItemViewModel>>(album.album_Item);
                all = all.SkipWhile(m => m.Movie == movie).ToList();

                album.album_Item = JsonConvert.SerializeObject(all);
                _db.SubmitChanges();
            }
        }

        /// <summary>
        /// 检查专辑是否存在
        /// </summary>
        /// <param name="id">专辑id</param>
        /// <returns>存在true，不存在false</returns>
        public static bool Exist(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id) || _db.tbl_Album.SingleOrDefault(p => p.album_Id == id) == null)
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
        /// 访问电影，专辑的访问量+1
        /// </summary>
        /// <param name="id">专辑id</param>
        public static void Visit(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                tbl_Album album = _db.tbl_Album.SingleOrDefault(a => a.album_Id == id);
                album.album_Visit++;
                _db.SubmitChanges();
            }
        }
    }
}
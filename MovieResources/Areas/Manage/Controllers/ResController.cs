using MovieResources.Filters;
using MovieResources.Helpers;
using MovieResources.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MovieResources.Areas.Manage.Controllers
{
    public class ResController : Controller
    {
        #region 资源管理首页
        //
        // GET: /Manage/Res/
        [AdminFilter]
        public ActionResult Index(string search, int page = 1)
        {
            MR_DataClassesDataContext _db = new MR_DataClassesDataContext();
            var query = from m in _db.tbl_Resource
                        select m;

            if (!string.IsNullOrWhiteSpace(search) && !string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = (IOrderedQueryable<tbl_Resource>)query.Where(s => s.res_Name.ToLower().Contains(search) || s.res_Content.ToLower().Contains(search));
            }
            query = query.Skip((page - 1) * 20).Take(20).OrderByDescending(m => m.res_Time);
            List<ResViewModel> ress = new List<ResViewModel>();
            foreach (var item in query)
            {
                if (!string.IsNullOrEmpty(item.res_User) && !string.IsNullOrWhiteSpace(item.res_User))
                {
                    ress.Add(new ResViewModel(item));
                }
            }

            FilterResViewModel resModel = new FilterResViewModel();
            resModel.Ress = ress;
            resModel.Search = search;
            resModel.Page = page;
            return View(resModel);
        }
        #endregion

        #region 下载bt种子文件
        //
        // GET: /Manage/DownTorrent/
        [AdminFilter]
        public ActionResult DownTorrent(string filename)
        {
            var path = Server.MapPath("~/Content/Torrent/" + filename);
            var name = System.IO.Path.GetFileName(path);
            return File(path, "application/zip-x-compressed", name);
        }
        #endregion

        #region 审核用户上传的资源
        //
        // GET: /Manage/Audit/
        [AdminFilter]
        public ActionResult Audit(string id)
        {
            if (!ResManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            ResManager.Audit(id);
            return RedirectToAction("Index");
        }

        //
        // GET: /Manage/Reject/
        [AdminFilter]
        public ActionResult Reject(string id)
        {
            if (!ResManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            RejectResViewModel model = new RejectResViewModel();
            model.Id = id;
            return View(model);
        }

        //
        // POST: /Manage/Reject/
        [HttpPost]
        [AdminFilter]
        public ActionResult Reject(RejectResViewModel model)
        {
            if (model.Note == "0")
            {
                model.Note = "无效";
            }
            else if (model.Note == "1")
            {
                model.Note = "已存在";
            }
            else
            {
                model.Note = "不匹配";
            }
            ResManager.Reject(model);
            return RedirectToAction("Index");
        }
        #endregion
    }
}
using MovieResources.Filters;
using MovieResources.Helpers;
using MovieResources.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MovieResources.Areas.Manage.Controllers
{
    public class DiscController : Controller
    {
        MR_DataClassesDataContext _db = new MR_DataClassesDataContext();

        #region 每日发现管理首页
        //
        // GET: /Manage/Disc/
        [AdminFilter]
        public ActionResult Index(int page = 1)
        {
            var query = from m in _db.tbl_Discovery
                        select m;

            query = query.Skip((page - 1) * 20).Take(20).OrderByDescending(m => m.disc_Flag);
            List<ManageDiscViewModel> discs = new List<ManageDiscViewModel>();
            foreach (var item in query)
            {
                discs.Add(new ManageDiscViewModel(item));
            }
            return View(discs);
        }
        #endregion

        #region 添加每日发现电影
        //
        // GET: /Manage/Create/
        [AdminFilter]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Manage/Create/
        [HttpPost]
        [AdminFilter]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ManageDiscViewModel model, System.Web.HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (!MovieManager.Exist(model.Movie))
            {
                ModelState.AddModelError("", "输入的电影不存在，请输入正确的电影编号");
                return View();
            }
            if (file != null && file.ContentLength > 0)
            {
                var fileName = System.IO.Path.Combine(Request.MapPath("~/Content/Discovery/"), System.IO.Path.GetFileName(file.FileName));
                file.SaveAs(fileName);
                model.Image = System.IO.Path.GetFileName(file.FileName);
            }
            DiscManager.Create(model);
            return RedirectToAction("Index");
        }
        #endregion

        #region 删除每日发现电影
        //
        // GET: /Manage/Delete/
        [AdminFilter]
        public ActionResult Delete(string id)
        {
            if (!DiscManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            else
            {
                string image = Server.MapPath("~/Content/Discovery/" + _db.tbl_Discovery.SingleOrDefault(s => s.disc_Id == id).disc_Image);
                if (System.IO.File.Exists(image))
                {
                    System.IO.File.Delete(image);
                }
            }
            DiscManager.Delete(id);
            return RedirectToAction("Index");
        }
        #endregion
    }
}
using MovieResources.Helpers;
using MovieResources.Models;
using System.Linq;
using System.Web.Mvc;

namespace MovieResources.Controllers
{
    public class AskController : Controller
    {
        private MR_DataClassesDataContext _db = new MR_DataClassesDataContext();

        #region 求资源首页
        //
        // GET: /Ask/Index/
        public ActionResult Index()
        {
            IndexAskViewModel model = new IndexAskViewModel();

            var queryall = _db.tbl_Ask.OrderByDescending(a => a.ask_Time);

            foreach (var item in queryall)
            {
                AskViewModel ask = new AskViewModel(item);
                if (User.Identity.IsAuthenticated)
                {
                    string user = AccountManager.GetId(User.Identity.Name);
                    if ((item.ask_User == user) || (_db.tbl_Mark.SingleOrDefault(w => w.mark_User == user && w.mark_Target == item.ask_Id && w.mark_Type == 6) != null))
                    {
                        ask.hadWith = true;
                    }
                }
                model.All.Add(ask);
            }

            var querymost = _db.tbl_Ask.OrderByDescending(a => a.ask_With).Take(20);
            foreach (var item in querymost)
            {
                AskViewModel ask = new AskViewModel(item);
                model.Most.Add(ask);
            }

            var queryover = _db.tbl_Ask.Where(a => a.ask_State == true).Take(20);
            foreach (var item in queryover)
            {
                AskViewModel ask = new AskViewModel(item);
                model.Over.Add(ask);
            }

            return View(model);
        }
        #endregion

        #region 创建求资源
        //
        // GET: /Ask/Create/
        [Authorize]
        public ActionResult Create(string id)
        {
            if (!MovieManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            ManageAskViewModel ask = new ManageAskViewModel() { MovieId = id, MovieTitle = _db.tbl_Movie.Single(m => m.movie_Id == id).movie_Title };
            return View(ask);
        }

        //
        // POST: /Ask/Create/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ManageAskViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Note) && !string.IsNullOrWhiteSpace(model.Note))
            {
                var user = _db.tbl_UserAccount.SingleOrDefault(u => u.user_Account == User.Identity.Name);
                model.User = user.user_Id;
                AskManager.Create(model);
                return RedirectToAction("Index", "Movie", new { id = model.MovieId });
            }
            else
            {
                ModelState.AddModelError("", "备注 不能为空，请重新输入 备注。");
                return View(model);
            }
        }
        #endregion

        #region 删除求资源
        //
        // GET: /Ask/Delete/
        [Authorize]
        public ActionResult Delete(string id, string returnurl)
        {
            if (!AskManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }

            var user = _db.tbl_UserAccount.SingleOrDefault(m => m.user_Account == User.Identity.Name).user_Id;
            var favor = _db.tbl_Ask.SingleOrDefault(m => m.ask_Id == id && m.ask_User == user);
            if (favor != null)
            {
                AskManager.Delete(id);
            }
            return Redirect(returnurl);
        }
        #endregion

        #region 已求到
        //
        // GET: /Ask/Over/
        [Authorize]
        public ActionResult Over(string id, string returnUrl)
        {
            if (!AskManager.Exist(id))
            {
                return RedirectToAction("NotFound", "Error");
            }
            string user = AccountManager.GetId(User.Identity.Name);

            if (_db.tbl_Mark.SingleOrDefault(w => w.mark_Target == id && w.mark_User == user && w.mark_Type == 6) == null)
            {
                AskManager.Over(id);
            }
            return RedirectToLocal(returnUrl);
        }
        #endregion

        #region 帮助程序
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (!Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl) && !string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}
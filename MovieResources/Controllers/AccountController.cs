using MovieResources.Filters;
using MovieResources.Helpers;
using MovieResources.Models;
using System.Web.Mvc;
using System.Web.Security;

namespace MovieResources.Controllers
{
    public class AccountController : Controller
    {
        #region 登录
        //
        // GET: /Account/Login/
        [AllowAnonymous]
        [LogonFilter]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Mine");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login/
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnurl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = AccountManager.PasswordSignIn(model.Account, model.Password);
            switch (result)
            {
                case SignInStatus.Success:
                    if (AccountManager.IsAdmin(AccountManager.GetId(model.Account)))
                    {
                        //return RedirectToAction("Index", "ManageMovie");
                        return RedirectToAction("Index", "Movie", new { Area = "Manage" });
                    }
                    else
                    {
                        return RedirectToLocal(returnurl);
                    }
                case SignInStatus.UndefinedAccount:
                    ModelState.AddModelError("", "用户名不存在。");
                    return View(model);
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "请检查用户名或密码是否正确。");
                    return View(model);
            }
        }
        #endregion

        #region 注册
        //
        // GET: /Account/Register/
        [AllowAnonymous]
        public ActionResult Register(string returnurl)
        {
            ViewBag.ReturnUrl = returnurl;
            return View();
        }

        //
        // POST: /Account/Register/
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model, string returnurl)
        {
            if (ModelState.IsValid)
            {
                var result = AccountManager.Create(model.Account, model.Password);
                if (result.Succeeded)
                {
                    AccountManager.SignIn(model.Account);

                    if (AccountManager.IsAdmin(AccountManager.GetId(User.Identity.Name)))
                    {
                        return RedirectToAction("Index", "ManageMovie");
                    }
                    else
                    {
                        return RedirectToLocal(returnurl);
                    }
                }
                ModelState.AddModelError("", result.Error);
            }

            return View(model);
        }
        #endregion

        #region 注销
        //
        // POST: /Account/LogOff/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff(string returnurl)
        {
            FormsAuthentication.SignOut();
            Session.RemoveAll();
            return RedirectToLocal(returnurl);
        }
        #endregion

        #region 忘记密码
        //
        // GET: /Account/ForgotPassword/
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword/
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = AccountManager.ValidateEmail(model.Account, model.Email);
                if (result.Succeeded)
                {
                    Session["ResetAccount"] = model.Account;
                    Session["CanReset"] = true;
                    return RedirectToAction("ResetPassword", "Account");
                }
                ModelState.AddModelError("", result.Error);
            }

            return View(model);
        }
        #endregion

        #region 重置密码
        //
        // GET: /Account/ResetPassword/
        [AllowAnonymous]
        [ResetPasswordFilter]
        public ActionResult ResetPassword()
        {
            return View();
        }

        //
        // POST: /Account/ResetPassword/
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.Account = Session["ResetAccount"].ToString();
            var result = AccountManager.ResetPassword(model.Account, model.Password);
            if (result.Succeeded)
            {
                Session["CanReset"] = false;
                return RedirectToAction("Login", "Account");
            }
            ModelState.AddModelError("", "重置密码失败，请重试。");
            return View();
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
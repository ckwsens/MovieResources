using MovieResources.Models;
using System;
using System.Linq;
using System.Web.Security;

namespace MovieResources.Helpers
{
    public class AccountManager
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>登录状态</returns>
        public static SignInStatus PasswordSignIn(string account, string password)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var truePassword = _db.tbl_UserAccount.Where(p => p.user_Account == account).ToList();

                if (truePassword.Count() == 0)
                {
                    return SignInStatus.UndefinedAccount;
                }
                else if (truePassword[0].user_Password == DESEncryption.DesEncrypt(password))
                {
                    FormsAuthentication.SetAuthCookie(account, true, FormsAuthentication.FormsCookiePath);
                    return SignInStatus.Success;
                }
                else
                {
                    return SignInStatus.Failure;
                }
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account">用户名</param>
        public static void SignIn(string account)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var loginAccount = _db.tbl_UserAccount.Single(p => p.user_Account == account);
                FormsAuthentication.SetAuthCookie(account, false);
            }
        }

        /// <summary>
        /// 创建账户
        /// </summary>
        /// <param name="account">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>成功 or 失败，错误信息</returns>
        public static RequestResult Create(string account, string password)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var hasAccount = _db.tbl_UserAccount.Where(p => p.user_Account == account);
                if (hasAccount.Count() > 0)
                {
                    return new RequestResult() { Succeeded = false, Error = "用户名已存在" };
                }
                else
                {
                    password = DESEncryption.DesEncrypt(password);
                    string guid;
                    do
                    {
                        guid = Guid.NewGuid().ToString("N").ToUpper();
                    } while (_db.tbl_UserAccount.Where(p => p.user_Id == guid).Count() != 0);
                    var addAccount = new tbl_UserAccount()
                    {
                        user_Id = guid,
                        user_Account = account,
                        user_Password = password,
                        user_IsAdmin = false,
                        user_CreateTime = DateTime.Now
                    };
                    addAccount.user_Avatar = "User_1.jpg";
                    addAccount.user_Cover = "Cover_1.jpg";
                    _db.tbl_UserAccount.InsertOnSubmit(addAccount);
                    _db.SubmitChanges();
                    _db.SetUserTime(guid);
                    return new RequestResult() { Succeeded = true };
                }
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="oldpassword">原密码</param>
        /// <param name="newpassword">新密码</param>
        /// <returns>成功 or 失败，错误信息</returns>
        public static RequestResult ChangePassword(string account, string oldpassword, string newpassword)
        {
            if (oldpassword == newpassword)
            {
                return new RequestResult() { Succeeded = false, Error = "新密码不能与原密码相同" };
            }
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                oldpassword = DESEncryption.DesEncrypt(oldpassword);
                var rightPassword = _db.tbl_UserAccount.Where(p => p.user_Account == account && p.user_Password == oldpassword).ToList();

                if (rightPassword.Count() == 0)
                {
                    return new RequestResult() { Succeeded = false, Error = "原密码输入错误" };
                }
                else
                {
                    newpassword = DESEncryption.DesEncrypt(newpassword);
                    var newAccount = _db.tbl_UserAccount.SingleOrDefault(p => p.user_Account == account);
                    newAccount.user_Password = newpassword;
                    _db.SubmitChanges();
                    _db.AlterUserAlterTime(newAccount.user_Id);
                    return new RequestResult() { Succeeded = true, Error = "密码 更改成功" };
                }
            }
        }

        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="email">输入的邮箱</param>
        /// <returns>成功 or 失败，错误信息</returns>
        public static RequestResult ValidateEmail(string account, string email)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var hasAccount = _db.tbl_UserAccount.Where(p => p.user_Account == account).ToList();

                if (hasAccount.Count() == 0)
                {
                    return new RequestResult() { Succeeded = false, Error = "用户名不存在" };
                }
                else if (hasAccount[0].user_EmailAddress != email)
                {
                    return new RequestResult() { Succeeded = false, Error = "电子邮件错误" };
                }
                else
                {
                    return new RequestResult() { Succeeded = true };
                }
            }
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <returns>成功 or 失败，错误信息</returns>
        public static RequestResult ResetPassword(string account, string password)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                password = DESEncryption.DesEncrypt(password);
                var newAccount = _db.tbl_UserAccount.SingleOrDefault(p => p.user_Account == account);
                newAccount.user_Password = password;
                _db.SubmitChanges();
                return new RequestResult() { Succeeded = true };
            }
        }

        /// <summary>
        /// 修改头像
        /// </summary>
        /// <param name="model"></param>
        /// <returns>成功 or 失败，错误信息</returns>
        public static RequestResult ChangeAvatar(ChangeAvatarViewModel model)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                bool hasModified = false;
                var account = _db.tbl_UserAccount.SingleOrDefault(p => p.user_Id == model.Id);

                if (model.Avatar != account.user_Avatar && !string.IsNullOrEmpty(model.Avatar) && !string.IsNullOrWhiteSpace(model.Avatar))
                {
                    account.user_Avatar = model.Avatar;
                    hasModified = true;
                }

                if (hasModified)
                {
                    _db.SubmitChanges();
                    _db.AlterUserAlterTime(account.user_Id);
                    return new RequestResult() { Succeeded = true };
                }
                else
                {
                    return new RequestResult() { Error = "没有改变。。。" };
                }
            }
        }

        /// <summary>
        /// 修改封面
        /// </summary>
        /// <param name="model"></param>
        /// <returns>成功 or 失败，错误信息</returns>
        public static RequestResult ChangeCover(ChangeCoverViewModel model)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                bool hasModified = false;
                var account = _db.tbl_UserAccount.SingleOrDefault(p => p.user_Id == model.Id);

                if (model.Cover != account.user_Avatar && !string.IsNullOrEmpty(model.Cover) && !string.IsNullOrWhiteSpace(model.Cover))
                {
                    account.user_Cover = model.Cover;
                    hasModified = true;
                }

                if (hasModified)
                {
                    _db.SubmitChanges();
                    _db.AlterUserAlterTime(account.user_Id);
                    return new RequestResult() { Succeeded = true };
                }
                else
                {
                    return new RequestResult() { Error = "没有改变。。。" };
                }
            }
        }

        /// <summary>
        /// 获取用户id
        /// </summary>
        /// <param name="account">用户名</param>
        /// <returns>用户id</returns>
        public static string GetId(string account)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var user = _db.tbl_UserAccount.SingleOrDefault(p => p.user_Account == account);
                if (user != null)
                {
                    return user.user_Id;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns>用户名</returns>
        public static string GetAccount(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var user = _db.tbl_UserAccount.SingleOrDefault(p => p.user_Id == id);
                if (user != null)
                {
                    return user.user_Account;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 检查用户是否是管理员
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns>是管理员true，否则false</returns>
        public static bool IsAdmin(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var user = _db.tbl_UserAccount.SingleOrDefault(p => p.user_Id == id);
                if (user != null)
                {
                    return (bool)user.user_IsAdmin;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 获取用户头像
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns>头像文件名</returns>
        public static string GetAvatar(string id)
        {
            using (MR_DataClassesDataContext _db = new MR_DataClassesDataContext())
            {
                var user = _db.tbl_UserAccount.SingleOrDefault(p => p.user_Id == id);
                if (user != null)
                {
                    return user.user_Avatar;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <param name="id">用户id</param>
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

    public class RequestResult
    {
        public bool Succeeded { get; set; }
        public string Error { get; set; }
    }
}
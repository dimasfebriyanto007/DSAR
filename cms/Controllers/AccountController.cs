using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using cms.Models;
using System.Configuration;

namespace cms.Controllers
{
    public class AccountController : CommonController
    {

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            DateTime lastValidSessionTime = DateTime.Now.AddMinutes(-30);

            bool a = _db.DatabaseExists();


            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    var checkUser = _db.Users.Where(c => c.UserName == model.UserName);

                    if (checkUser.Count() > 0)
                    {
                        if (CommonModel.checkLoginStatus(model.UserName))
                        {
                            User loginUser = checkUser.First();

                            FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                            loginUser.LastLoginDate = DateTime.Now;
                            loginUser.LastActiveSession = DateTime.Now;
                            _db.ApplyCurrentValues(loginUser.EntityKey.EntitySetName, loginUser);
                            _db.SaveChanges();

                            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                            {
                                string message = LogsModel.SaveUserLoginLogs(model.UserName, true, "Login Success", true);
                                if (string.IsNullOrEmpty(message))
                                {
                                    return Redirect(returnUrl);
                                }
                                else
                                {
                                    ModelState.AddModelError("", message);
                                }
                            }
                            else
                            {
                                string message = LogsModel.SaveUserLoginLogs(model.UserName, true, "Login Success", true);
                                if (string.IsNullOrEmpty(message))
                                {
                                    if (CommonModel.checkEXPPass(model.UserName) || CommonModel.CheckFirstLogin(model.UserName))
                                    {
                                        return RedirectToAction("ChangePassword60days", "Account");
                                    }
                                    return RedirectToAction("Index", "Home");
                                }
                                else
                                {
                                    ModelState.AddModelError("", message);
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Anda sudah logi di tempat lain");
                            string message = LogsModel.SaveUserLoginLogs(model.UserName, false, "Anda sudah logi di tempat lain", false);
                        }
                    }
                    else
                    {
                        string message = LogsModel.SaveUserLoginLogs(model.UserName, false, "Invalid user name.", false);
                        ModelState.AddModelError("", "Invalid user name." + message);
                    }
                }
                else
                {
                    // ------------  added by Aditia - 20160810
                    // var checkUser = _db.Users.Where(c => c.UserName == model.UserName && c.IsLockedOut == true);
                    var checkUser = _db.Users.Where(c => c.UserName == model.UserName).FirstOrDefault();

                    if (checkUser != null)
                    {
                        //string message = LogsModel.SaveUserLoginLogs(model.UserName, false, "The NPK is not active. Please send mail to dsar_admin@maybank.co.id for activation. Before sent mail, please make sure Your status not in traning or leave !", false);
                        if (checkUser.IsLockedOut == true)
                        {
                            ModelState.AddModelError("", "The NPK is not active. Please send mail to dsar_admin@maybank.co.id for activation. ");
                            ModelState.AddModelError("", "Before sent mail, please make sure Your status not in traning or leave ! ");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Password Anda Salah");
                            string messagem = LogsModel.SaveUserLoginLogs(model.UserName, false, "Password anda salah", false);

                            string message = CommonModel.inActiveUser3wrongpass(model.UserName);
                            if (!string.IsNullOrEmpty(message) || !string.IsNullOrEmpty(messagem))
                            {
                                ModelState.AddModelError("", message + messagem);
                            }
                        }

                    }
                    // ------------ EOF 
                    else
                    {
                       // string message = LogsModel.SaveUserLoginLogs(model.UserName, false, "The NPK or password provided is Invalid.", false);

                        ModelState.AddModelError("", "The NPK or password provided is Invalid. ");
                    }

                }
            }
            else
            {
                ModelState.AddModelError("", "The NPK or password provided is Invalid. ");
            }

            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            var checkUser = _db.Users.Where(c => c.UserName == currentUser.UserName);
            if (checkUser.Count() > 0)
            {
                User loginUser = checkUser.First();
                loginUser.LastActiveSession = null;
                _db.ApplyCurrentValues(loginUser.EntityKey.EntitySetName, loginUser);
                _db.SaveChanges();
            }
            FormsAuthentication.SignOut();

            Session["role"] = "";
            string message = LogsModel.SaveUserLoginLogs(currentUser.UserName, true, "Log Off Success.", false);

            return RedirectToAction("Index", "Home");
        }


        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        public ActionResult ChangePassword60days()
        {

            string messagebehind = "Your Paaword expired , ";
            User user = CommonModel.GetCurrentUser();

            if (CommonModel.CheckFirstLogin(user.UserName))
            {
                messagebehind = "You must change your password when you first time login , ";
            }

            ViewData["asd"] = messagebehind;


            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded = false;
                UserRepository _user = new UserRepository();

                try
                {
                    if (!PasswordPolicyModel.IsValid(model.NewPassword))
                    {
                        ModelState.AddModelError("", "minimum 6 karakter berkombinasi numerik dan alphabet");
                        //ModelState.AddModelError("", "Password doesn't meet policy (Minimum length 6, at least 1 capital letter, at least 1 letter non alphanumeric and at least 1 numeric letter.");
                    }
                    else
                    {

                        if (CommonModel.getChangePasswordfor6time(User.Identity.Name, model.NewPassword))
                        {
                            MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                            //changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                            changePasswordSucceeded = _user.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                            if (changePasswordSucceeded)
                            {
                                string message = LogsModel.SaveChangePasswordLog(User.Identity.Name, model.NewPassword);
                                if (!string.IsNullOrEmpty(message))
                                {
                                    changePasswordSucceeded = false;

                                }
                            }


                        }
                        else
                        {
                            changePasswordSucceeded = false;
                            ModelState.AddModelError("", "passwor ini sudah dipakai sebelum 6x ganti");

                        }


                    }
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            ViewData["PasswordLength"] = Membership.MinRequiredPasswordLength;

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword60days(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded = false;
                UserRepository _user = new UserRepository();

                try
                {
                    if (!PasswordPolicyModel.IsValid(model.NewPassword))
                    {
                        ModelState.AddModelError("", "minimum 6 karakter berkombinasi numerik dan alphabet");
                        //ModelState.AddModelError("", "Password doesn't meet policy (Minimum length 6, at least 1 capital letter, at least 1 letter non alphanumeric and at least 1 numeric letter.");
                    }
                    else
                    {

                        if (CommonModel.getChangePasswordfor6time(User.Identity.Name, model.NewPassword))
                        {
                            MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                            //changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                            changePasswordSucceeded = _user.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                            if (changePasswordSucceeded)
                            {
                                string message = LogsModel.SaveChangePasswordLog(User.Identity.Name, model.NewPassword);
                                if (!string.IsNullOrEmpty(message))
                                {
                                    changePasswordSucceeded = false;

                                }
                            }


                        }
                        else
                        {
                            changePasswordSucceeded = false;
                            ModelState.AddModelError("", "passwor ini sudah dipakai sebelum 6x ganti");

                        }


                    }
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            ViewData["PasswordLength"] = Membership.MinRequiredPasswordLength;

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }


        public ActionResult Register()
        {
            ViewData["Teams"] = _db.SalesTeams.Where(c => c.TeamId != 15 && c.TeamId != 14).OrderBy(c => c.Name).ToList();  // added by Aditia 2016.10.04 ( Hide DSAR ADMIN  Role )
            ViewData["Branchs"] = _db.Branchs.OrderBy(c => c.Name).ToList();
            ViewData["Leaders"] = _db.SalesManagers.OrderBy(c => c.Name).ToList();
            ViewData["BMs"] = _db.BranchManagers.OrderBy(c => c.Name).ToList();
            ViewData["Status"] = "1";

            var getBranch = _db.Branchs.OrderBy(c => c.Name);
            if (getBranch.Count() > 0)
            {
                Branch branch = getBranch.First();
                ViewData["Leaders"] = _db.SalesManagers.Where(c => c.BranchCode == branch.BranchCode).OrderBy(c => c.Name).ToList();
            }

            return View();
        }

        //
        // POST: /Sales/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Register([Bind(Exclude = "SalesId")] Sale dataToCreate)
        {
            ViewData["Teams"] = _db.SalesTeams.OrderBy(c => c.Name).ToList();
            ViewData["Branchs"] = _db.Branchs.OrderBy(c => c.Name).ToList();
            ViewData["Leaders"] = _db.SalesManagers.Where(c => c.BranchCode == dataToCreate.BranchCode).OrderBy(c => c.Name).ToList();
            ViewData["BMs"] = _db.BranchManagers.OrderBy(c => c.Name).ToList();
            ViewData["Username"] = Request["Username"];

            try
            {
                dataToCreate.JoinDate = DateTime.Now;

                if (!ModelState.IsValid) return View(dataToCreate);

                string Username = Request["Username"];
                string Password = Request["Password"];
                string ConfirmPassword = Request["ConfirmPassword"];

                var checkNpk = _db.Sales.Where(c => c.Npk == dataToCreate.Npk);
                if (checkNpk.Count() > 0) { ViewData["Output"] = "NPK already exists"; return View(dataToCreate); }
                // added by Aditia S - 20160801
                if (dataToCreate.Npk == "0000000000") { ViewBag.Output = "Invalid NPK"; return View(dataToCreate); }
                if (dataToCreate.Npk.Length != 10) { ViewBag.Output = "NPK have to 10 digits"; return View(dataToCreate); }
                if (!CommonModel.isNumber(dataToCreate.Npk)) { ViewBag.Output = "NPK is numeric only"; return View(dataToCreate); }
                // --------- EOF Aditia S

                if (dataToCreate.Email == null) { ViewBag.Output = "Alamat email harus diisi"; return View(dataToCreate); }
                if (!CommonModel.isEmail(dataToCreate.Email)) { ViewBag.Output = "Invalid email address"; return View(dataToCreate); }
                if (dataToCreate.Phone == null) { ViewBag.Output = "Nomor handphone harus diisi"; return View(dataToCreate); }
                if (!CommonModel.isNumber(dataToCreate.Phone)) { ViewBag.Output = "Invalid phone number"; return View(dataToCreate); }

                var checkSM = _db.SalesManagers.Where(c => c.BranchCode == dataToCreate.BranchCode);
                if (checkSM.Count() > 0)
                {
                    if (dataToCreate.SmId == null || dataToCreate.SmId == 0) { ViewBag.Output = "Sales Leader belum diisi"; return View(dataToCreate); }
                }

                if (String.IsNullOrWhiteSpace(Username)) { ViewData["Output"] = "Invalid Username"; return View(dataToCreate); }
                if (String.IsNullOrWhiteSpace(Password)) { ViewData["Output"] = "Password cannot be empty"; return View(dataToCreate); }
                if (!PasswordPolicyModel.IsValid(Password)) { ViewData["Output"] = "Password doesn't meet policy (Minimum length 6, at least 1 capital letter, at least 1 letter non alphanumeric and at least 1 numeric letter)"; return View(dataToCreate); }
                if (Password != ConfirmPassword) { ViewData["Output"] = "Confirmation password is not match"; return View(dataToCreate); }

                var checkUser = _db.Users.Where(c => c.UserName == Username);
                if (checkUser.Count() > 0) { ViewData["Output"] = "Username already exists"; return View(dataToCreate); }

                _db.AddToSales(dataToCreate);
                _db.SaveChanges();

                User user = new User();
                user.UserName = Username;
                user.PasswordSalt = UserRepository.CreateSalt();
                user.Password = UserRepository.CreatePasswordHash(Password, user.PasswordSalt);
                user.PasswordText = Password;
                user.Role = "SALES";
                user.RelatedId = dataToCreate.SalesId;
                user.CreatedDate = DateTime.Now;
                user.IsLockedOut = true;
                user.LastLoginDate = DateTime.Now;

                _db.AddToUsers(user);
                _db.SaveChanges();



                //  ---- Added by Aditia - 20160812 - SEND EMAIL REGISTRASION

                string mailSubject = "Permohonan aktifasi registrasi baru - " + dataToCreate.Npk + " - " + dataToCreate.Name;
                string mailBody = "Dear Admin," +
                                  "<br /><br />Berikut ini adalah permintaan aktifasi registrasi baru : " +
                                  "<br /><strong>NPK :</strong> " + dataToCreate.Npk +
                                  "<br/><strong>Nama :</strong> " + dataToCreate.Name +
                                  "<br/><strong>Email :</strong> " + dataToCreate.Email +
                                  "<br/><br />Regards,<br />Pemohon";

                if (CommonModel.isEmail(dataToCreate.Email))
                {
                    if (CommonModel.SendMail("dsar_admin@maybank.co.id", mailSubject, mailBody, dataToCreate.Email))
                    {
                        user.PasswordSalt = UserRepository.CreateSalt();
                        user.Password = UserRepository.CreatePasswordHash(Password, user.PasswordSalt);
                        user.PasswordText = Password;

                        _db.ApplyCurrentValues(user.EntityKey.EntitySetName, user);
                        _db.SaveChanges();

                        ViewData["Success"] = "Pendaftaran user dengan NPK : " + dataToCreate.Npk + " telah berhasil. Anda baru bisa login setelah mendapat email aktifasi dari Admin, email aktivasi akan dikirim ke " + dataToCreate.Email + " .";
                    }
                    else
                    {
                        //ViewData["Output"] = "Tidak dapat mengirimkan email. Silahkan hubungi Admin.";
                        ViewData["Success"] = "Pendaftaran user dengan NPK : " + dataToCreate.Npk + " telah berhasil. Anda baru bisa login setelah user Anda diaktifkan oleh Admin. Silahkan Hubungi Admin!";
                    }

                }
                else
                {
                    //ViewData["Output"] = "Alamat email Anda tidak valid.";
                    ViewData["Success"] = "Pendaftaran user dengan NPK : " + dataToCreate.Npk + " telah berhasil. Anda baru bisa login setelah user Anda diaktifkan oleh Admin. Silahkan Hubungi Admin!";
                }


                // ----- EOF - 20160812  - SEND EMAIL REGISTRASION


            }
            catch (Exception e)
            {
                ViewData["Output"] = e.Message;
            }

            return View();
        }

        public ActionResult LoadSM(string id = "")
        {
            ViewBag.Output = "<option value=\"\"></option>";

            var getSM = _db.SalesManagers.Where(c => c.BranchCode == id).OrderBy(c => c.Name);
            if (getSM.Count() > 0)
            {
                foreach (SalesManager item in getSM)
                {
                    ViewBag.Output += "<option value=\"" + item.ManagerId + "\">" + item.Name + "</option>";
                }
            }

            return View("result");
        }

        public ActionResult ForgetPassword()
        {
            if (Request.HttpMethod == "POST")
            {
                //try
                //{
                string npk = Request["npk"];
                ViewData["npk"] = npk;

                if (String.IsNullOrWhiteSpace(npk)) { ViewData["Output"] = "Silahkan isi Username atau NPK Anda"; return View(); }

                var checkNpk = _db.Users.Where(c => c.UserName == npk && c.IsLockedOut == false);

                if (checkNpk.Count() > 0)
                {
                    User user = checkNpk.First();
                    UserDetailModel userDetail = CommonModel.GetUserDetail(user.UserName);

                    if (userDetail != null)
                    {

                        string Password = CommonModel.GenerateRandomString(8);
                        string mailSubject = "Reset Password DSAR";
                        string mailBody = "Dear " + userDetail.Name + "," +
                                          "<br />Berikut ini adalah password baru Anda " +
                                          "<br /><strong>Username :</strong> " + user.UserName +
                                          "<br/><strong>Password :</strong> " + Password +
                                          "<br/><br />Regards,<br />Admin DSAR";

                        if (CommonModel.isEmail(userDetail.Email))
                        {
                            if (CommonModel.SendMail(userDetail.Email, mailSubject, mailBody, "noreply@maybank.co.id"))
                            {
                                user.PasswordSalt = UserRepository.CreateSalt();
                                user.Password = UserRepository.CreatePasswordHash(Password, user.PasswordSalt);
                                user.PasswordText = Password;

                                _db.ApplyCurrentValues(user.EntityKey.EntitySetName, user);
                                _db.SaveChanges();

                                ViewData["Success"] = "Password Anda berhasil direset. Password baru Anda telah kami kirimkan melalui email " + userDetail.Email + " .";
                            }
                            else
                            {
                                ViewData["Output"] = "Tidak dapat mengirimkan email. Silahkan hubungi Admin.";
                            }

                        }
                        else
                        {
                            ViewData["Output"] = "Alamat email Anda tidak valid.";
                        }

                    }
                    else
                    {
                        ViewData["Output"] = "Terjadi kesalahan. Silahkan hubungi Admin.";
                    }
                }
                else
                {
                    // ------------  added by Aditia - 20160810
                    var checkNpk2 = _db.Users.Where(c => c.UserName == npk && c.IsLockedOut == true);

                    if (checkNpk2.Count() > 0)
                    {
                        ViewData["Output"] = "Username atau NPK yang Anda masukkan tidak aktif ! Hubungi Admin untuk aktifasi."; return View();
                    }
                    // -------- EOF 
                    else
                    {
                        ViewData["Output"] = "Username atau NPK yang Anda masukkan tidak ditemukan"; return View();
                    }

                }

                //}
                //catch (Exception e)
                //{
                //    ViewData["Output"] = e.Message;
                //}
            }

            return View();
        }


        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}

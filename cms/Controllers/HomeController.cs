using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cms.Models;

namespace cms.Controllers
{

    [HandleError]
    [Authorize]
    public class HomeController : CommonController
    {        

        public ActionResult Index()
        {
            User user = CommonModel.GetCurrentUser();
            string messagePassExp = string.Empty;
            if (CommonModel.rememberPassword(user.UserName))
            {
                messagePassExp = "Password anda sudah mau habis";
            }

            ViewBag.RememberPassword = messagePassExp;

            ViewBag.notAbsen = "1";

            if (Request.HttpMethod == "POST")
            {
                Response.Write(Request["tes"]);
                Response.End();
            }

            if (CommonModel.UserRole() == "SALES")
            {
                //User user = CommonModel.GetCurrentUser();
                Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();

                ViewData["Username"] = user.UserName;
                ViewData["BranchName"] = sales.Branch.Name;
                ViewData["Status"] = (user.IsLockedOut) ? "0" : "1";
                ViewBag.Output = "";

                var checkAbsen = _db.Absensis.Where(c => c.SalesId == sales.SalesId && c.DateAbsen.Year == DateTime.Now.Year && c.DateAbsen.Month == DateTime.Now.Month && c.DateAbsen.Day == DateTime.Now.Day);
                if (checkAbsen.Count() > 0)
                {
                    ViewBag.notAbsen = "0";
                }

                var checkSM = _db.SalesManagers.Where(c => c.BranchCode == sales.BranchCode);
                ViewData["Leaders"] = checkSM;
                ViewBag.smExists = "0";
                if (checkSM.Count() > 0)
                {
                    ViewBag.smExists = "1";
                }

                ViewBag.PopupProduct = String.Empty;
                ViewBag.ProductId = String.Empty;
                var checkProd = _db.Products.Where(c => c.ImageName != "").OrderByDescending(c => c.ProductId);
                if (checkProd.Count() > 0)
                {
                    Product prod = checkProd.First();
                    ViewBag.PopupProduct = Url.Content("~/images/product/" + prod.ImageName)+".jpg";
                    ViewBag.ProductId = prod.ProductId;
                }

                return View("SalesEdit",sales);
            }

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SalesEdit(Sale dataToEdit)
        {
            if (CommonModel.UserRole() != "SALES") return RedirectToAction("Index", "Home");

            User user = CommonModel.GetCurrentUser();
            Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();

            ViewBag.notAbsen = "1";

            ViewData["Username"] = user.UserName;
            ViewData["BranchName"] = sales.Branch.Name;
            ViewData["Status"] = (user.IsLockedOut) ? "0" : "1";
            ViewBag.Output = "";

            var checkAbsen = _db.Absensis.Where(c => c.SalesId == sales.SalesId && c.DateAbsen.Year == DateTime.Now.Year && c.DateAbsen.Month == DateTime.Now.Month && c.DateAbsen.Day == DateTime.Now.Day);
            if (checkAbsen.Count() > 0)
            {
                ViewBag.notAbsen = "0";
            }

            var checkSM = _db.SalesManagers.Where(c => c.BranchCode == sales.BranchCode);
            ViewData["Leaders"] = checkSM;
            ViewBag.smExists = "0";
            if (checkSM.Count() > 0)
            {
                ViewBag.smExists = "1";
            }
            
            var originalData = (from m in _db.Sales
                                where m.SalesId == sales.SalesId
                                select m).First();

            var originalUser = (from m in _db.Users
                                where m.RelatedId == sales.SalesId
                                select m).First();

            ViewData["Username"] = originalUser.UserName;
            ViewData["Status"] = (originalUser.IsLockedOut) ? "1" : "2";
            ViewBag.Output = "";

            if (dataToEdit.Email==null) { ViewBag.Output = "<div class=\"error\">Alamat email harus diisi</div>"; return View(dataToEdit); }            
            if (!CommonModel.isEmail(dataToEdit.Email)) { ViewBag.Output = "<div class=\"error\">Invalid email address</div>"; return View(dataToEdit); }
            if (dataToEdit.Phone==null) { ViewBag.Output = "<div class=\"error\">Nomor handphone harus diisi</div>"; return View(dataToEdit); }
            if (!CommonModel.isNumber(dataToEdit.Phone)) { ViewBag.Output = "<div class=\"error\">Invalid phone number</div>"; return View(dataToEdit); }

            if (checkSM.Count() > 0)
            {
                if (dataToEdit.SmId==null || dataToEdit.SmId==0) { ViewBag.Output = "<div class=\"error\">Sales Leader belum diisi</div>"; return View(dataToEdit); }
            }

            try
            {

                dataToEdit.SalesId = originalData.SalesId;
                dataToEdit.Npk = originalData.Npk;
                dataToEdit.JoinDate = originalData.JoinDate;
                dataToEdit.TeamId = originalData.TeamId;
                dataToEdit.BranchCode = originalData.BranchCode;
                dataToEdit.BmId = originalData.BmId;

                if (!ModelState.IsValid)
                    return View(dataToEdit);
                
                string Password = Request["Password"];
                string ConfirmPassword = Request["ConfirmPassword"];                

                if (!String.IsNullOrWhiteSpace(Password))
                {
                    if (String.IsNullOrWhiteSpace(Password)) { ViewBag.Output = "<div class=\"error\">Password cannot be empty</div>"; return View(dataToEdit); }
                    if (!PasswordPolicyModel.IsValid(Password)) { ViewBag.Output = "<div class=\"error\">Password doesn't meet policy (Minimum length 6, at least 1 capital letter, at least 1 letter non alphanumeric and at least 1 numeric letter)</div>"; return View(dataToEdit); }
                    if (Password != ConfirmPassword) { ViewBag.Output = "<div class=\"error\">Confirmation password is not match</div>"; return View(dataToEdit); }
                }

                _db.ApplyCurrentValues(originalData.EntityKey.EntitySetName, dataToEdit);
                _db.SaveChanges();

                if (!String.IsNullOrWhiteSpace(Password))
                {
                    originalUser.PasswordSalt = UserRepository.CreateSalt();
                    originalUser.Password = UserRepository.CreatePasswordHash(Password, originalUser.PasswordSalt);
                    originalUser.PasswordText = Password;
                }

                _db.ApplyCurrentValues(originalUser.EntityKey.EntitySetName, originalUser);
                _db.SaveChanges();

                ViewBag.Output = "<div class=\"success\">Profile successfully saved</div>";
                return View("SalesEdit",dataToEdit);
            }
            catch (Exception e)
            {
                ViewBag.Output = e.Message;
                return View(dataToEdit);
            }
        }

        public ActionResult Absen()
        {
            if (CommonModel.UserRole() != "SALES") return RedirectToAction("Index", "Home");

            User user = CommonModel.GetCurrentUser();
            Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();

            var checkSM = _db.SalesManagers.Where(c => c.BranchCode == sales.BranchCode);
            ViewData["Leaders"] = checkSM;
            ViewBag.smExists = "0";
            if (checkSM.Count() > 0)
            {
                ViewBag.smExists = "1";
            }

            ViewBag.notAbsen = "1";
            ViewData["Username"] = user.UserName;
            ViewData["Status"] = (user.IsLockedOut) ? "0" : "1";
            ViewBag.Output = "";

            var checkAbsen = _db.Absensis.Where(c => c.SalesId == sales.SalesId && c.DateAbsen.Year == DateTime.Now.Year && c.DateAbsen.Month == DateTime.Now.Month && c.DateAbsen.Day == DateTime.Now.Day);
            if (checkAbsen.Count() > 0)
            {
                ViewBag.notAbsen = "0";
            }
            else
            {
                Absensi absen = new Absensi();
                absen.SalesId = sales.SalesId;
                absen.DateAbsen = DateTime.Now.Date;
                absen.DateSubmit = DateTime.Now;
                absen.Hadir = 1;

                _db.AddToAbsensis(absen);
                _db.SaveChanges();

                ViewBag.notAbsen = "0";

                ViewBag.Output = "<div class=\"success\">Absensi Anda berhasil disimpan</div>";
            }

            return View("SalesEdit", sales);
        }
    }


}

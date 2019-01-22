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
    [Authorize]
    public class AdminController : CommonController
    {
        public ActionResult Index()
        {
            //if (CommonModel.UserRole() != "ADMIN") return RedirectToAction("Index", "Home");
            //if (CommonModel.UserRole() != "ADMINUSER") return RedirectToAction("Index", "Home");
            if (!(CommonModel.UserRole() == "ADMIN" || CommonModel.UserRole() == "ADMINUSER")) return RedirectToAction("Index", "Home");

            //ViewData["Branchs"] = _db.Branchs.OrderBy(c => c.Name).ToList();
            return View();
        }

        public ActionResult list(int page = 1, string key = "", int perpage = 20, string orderBy = "", string orderMode = "", string Role = "")
        {
            if (CommonModel.UserRole() != "ADMIN") return RedirectToAction("Index", "Home");

            double cur_page = page;
            page -= 1;
            int per_page = perpage;
            bool first_btn = true;
            bool last_btn = true;
            bool previous_btn = true;
            bool next_btn = true;
            int start = page * per_page;
            string msg = "";

            var data = (from c in _db.Users
                        where c.Role == "ADMINBUSSINES" || c.Role == "ADMINUSER"
                        join d in _db.Admins on c.UserId equals d.UserID
                        select
                        new
                        {
                            userID = c.UserId,
                            Nik = d.NIK,
                            Nama = d.Name,
                            Role = c.Role,
                            CreatedDate = c.CreatedDate,
                            lastLogin = c.LastLoginDate,
                            active = c.IsLockedOut





                        }
                        ).Where(c => 1 == 1);

            if (!String.IsNullOrEmpty(key))
            {
                data = data.Where(c => c.Nama.Contains(key));
            }
            if (Role != "")
            {
                data = data.Where(c => c.Role == Role);
            }


            int count = data.Count();

            switch (orderBy)
            {
                case "name": if (orderMode == "asc") data = data.OrderBy(c => c.Nama); else data = data.OrderByDescending(c => c.Nama); break;
                case "role": if (orderMode == "asc") data = data.OrderBy(c => c.Role); else data = data.OrderByDescending(c => c.Role); break;
                default: if (orderMode == "desc") data = data.OrderByDescending(c => c.lastLogin); else data = data.OrderBy(c => c.lastLogin); break;
            }

            double no_of_paginations = Math.Ceiling(Convert.ToDouble(count) / Convert.ToDouble(per_page));
            double start_loop;
            double end_loop;
            double nex;
            double pre;
            double i = 0;

            var result = data.Skip(start).Take(per_page);

            if (cur_page >= 5)
            {
                start_loop = cur_page - 2;
                if (no_of_paginations > cur_page + 2)
                    end_loop = cur_page + 2;
                else if (cur_page <= no_of_paginations && cur_page > no_of_paginations - 4)
                {
                    start_loop = no_of_paginations - 4;
                    end_loop = no_of_paginations;
                }
                else
                {
                    end_loop = no_of_paginations;
                }
            }
            else
            {
                start_loop = 1;
                if (no_of_paginations > 5)
                    end_loop = 5;
                else
                    end_loop = no_of_paginations;
            }

            double dataStart = (result.Count() == 0) ? 0 : (cur_page * per_page) - per_page + 1;
            double dataEnd = dataStart + per_page - 1;
            int dataTotal = count;
            if (dataEnd > dataTotal) dataEnd = dataTotal;

            msg += "<table class=\"grid-style\" width=\"100%\"><thead><tr><th>No</th><th>NPK</th><th>Name</th><th>Role</th><th>Created Date</th><th>Last Login</th><th>Status</th><th>Option</th>";

            msg += "</tr><tbody>";

            i = dataStart;

            foreach (var item in result)
            {
                var checkUser = _db.Users.Where(c => c.UserId == item.userID);
                string publish = "&nbsp;";
                if (checkUser.Count() > 0)
                {
                    User uInfo = checkUser.First();
                    publish = (!uInfo.IsLockedOut) ? "<img src=\"" + Url.Content("~/Content/images/icon-yes.gif") + "\" />" : "<img src=\"" + Url.Content("~/Content/images/icon-no.gif") + "\" />";
                }
                string date = item.lastLogin.ToString("dd MMM yyyy HH:mm");
                string dateCreated = item.CreatedDate.ToString("dd MMM yyyy HH:mm");
                string klas = (i % 2 == 0) ? "gridrow_alternate" : "gridrow";

                string Roles = (item.Role == "ADMINUSER") ? "User Admin" : "Bussines Admin";


                msg += "<tr class=\"" + klas + "\">" +
                       "<td>" + i + "</td>" +
                       "<td>" + item.Nik + "</td>" +
                       "<td>" + item.Nama + "</td>" +
                       "<td>" + Roles + "</td>" +
                       "<td>" + dateCreated + "</td>" +
                       "<td align=\"center\">" + date + "</td>" +
                       "<td>" + publish + "</td>" +
                       "<td align=\"center\"><a href=\"" + Url.Content("~/Admin/Edit/" + item.userID) + "\" title=\"Edit\"><img src=\"" + Url.Content("~/Content/images/icon-edit.gif") + "\" border=\"0\"/></a> <a href=\"" + Url.Content("~/Admin/Delete/" + item.userID) + "\" title=\"Delete\"><img src=\"" + Url.Content("~/Content/images/icon-delete.gif") + "\" border=\"0\"/></a></td>" +
                       "</tr>";

                i++;
            }
            msg += "</tbody></table>";


            if (i == 0) msg += "<div class=\"arial\" style=\"text-align:center;padding:10px 0;\">No result found</div>";

            msg += "<div class=\"clear\"></div><br /><div class=\"numb\">";

            if (first_btn && cur_page > 1)
            {
                msg += " <a href=\"JavaScript:void(0);\" p=\"1\" class=\"page-menu\">FIRST</a> ";
            }

            if (previous_btn && cur_page > 1)
            {
                pre = cur_page - 1;
                msg += " <a href=\"JavaScript:void(0);\" p=\"" + pre + "\" class=\"page-menu\">PREV</a> ";
            }

            for (i = start_loop; i <= end_loop; i++)
            {
                if (cur_page == i)
                {
                    msg += " <a href=\"JavaScript:void(0);\" p=\"" + i + "\" class=\"on page-menu\">" + i + "</a> ";
                }
                else
                {
                    msg += " <a href=\"JavaScript:void(0);\" p=\"" + i + "\" class=\"page-menu\">" + i + "</a> ";
                }
            }

            if (next_btn && cur_page < no_of_paginations)
            {
                nex = cur_page + 1;
                msg += " <a href=\"JavaScript:void(0);\" p=\"" + nex + "\" class=\"page-menu\"> NEXT</a> ";
            }

            if (last_btn && cur_page < no_of_paginations && no_of_paginations > 0)
            {
                msg += " <a href=\"JavaScript:void(0);\" p=\"" + no_of_paginations + "\" class=\"page-menu\">LAST</a> ";
            }

            dataStart = (result.Count() == 0) ? 0 : (cur_page * per_page) - per_page + 1;
            dataEnd = dataStart + per_page - 1;
            dataTotal = count;
            if (dataEnd > dataTotal) dataEnd = dataTotal;

            msg += " &nbsp; menampilkan " + dataStart + " sampai " + dataEnd + " dari " + dataTotal + " data";
            msg += "</div>";

            ViewBag.Output = msg;

            return View("output");
        }

        //
        // GET: /SalesManager/Create

        public ActionResult Create()
        {
            if (!(CommonModel.UserRole() == "ADMIN" || CommonModel.UserRole() == "ADMINUSER")) return RedirectToAction("Index", "Home");

            //ViewData["Branchs"] = _db.Branchs.OrderBy(c => c.Name).ToList();
            ViewData["Status"] = "1";
            return View();
        }

        //
        // POST: /SalesManager/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(User dataToCreate)
        {
            //if (CommonModel.UserRole() != "ADMIN") return RedirectToAction("Index", "Home");
            //if (CommonModel.UserRole() != "ADMINUSER") return RedirectToAction("Index", "Home");
            if (!(CommonModel.UserRole() == "ADMIN" || CommonModel.UserRole() == "ADMINUSER")) return RedirectToAction("Index", "Home");

            //ViewData["Branchs"] = _db.Branchs.OrderBy(c => c.Name).ToList();
            //ViewData["Username"] = Request["Username"];

            try
            {
                //if (!ModelState.IsValid) return View(dataToCreate);

                string Nik = Request["Nik"];
                string Nama = Request["Nama"];
                string Username = Request["Username"];
                string Password = Request["Password"];
                string ConfirmPassword = Request["ConfirmPassword"];
                string Role = Request["Role"];
                string Status = Request["Status"];
                ViewData["Status"] = Status;

                bool validasi = true;
                string message = string.Empty;
                if (String.IsNullOrWhiteSpace(Nik)) { message += "- Invalid NPK <br>"; validasi = false; }
                else if (!NPKPolicy.IsValid(Nik)) { message += "- NPK minimum 10 char <br>"; validasi = false; }

                if (String.IsNullOrWhiteSpace(Username)) { message += "- Invalid Username <br>"; validasi = false; }
                if (String.IsNullOrWhiteSpace(Nama)) { message += "- Invalid Username <br>"; validasi = false; }
                if (String.IsNullOrWhiteSpace(Password)) { message += "- Password cannot be empty <br>"; validasi = false; }
                else if (!PasswordPolicyModel.IsValid(Password)) { message += "- Password doesn't meet policy (Minimum length 6, at least 1 capital letter, at least 1 letter non alphanumeric and at least 1 numeric letter) <br>"; validasi = false; }
                else if (Password != ConfirmPassword) { message += "- Confirmation password is not match <br>"; validasi = false; }

                var checkUser = _db.Users.Where(c => c.UserName == Nik);
                if (checkUser.Count() > 0) { message += "- Nik already exists <br>"; validasi = false; }
                ViewData["Output"] = message;
                if (!validasi)
                {
                    return View(dataToCreate);
                }

                bool locked = (Status == "0") ? true : false;

                User user = new User();
                //user.UserName = Username;
                user.UserName = Username;
                user.PasswordSalt = UserRepository.CreateSalt();
                user.Password = UserRepository.CreatePasswordHash(Password, user.PasswordSalt);
                user.PasswordText = Password;
                user.Role = Role;
                user.CreatedDate = DateTime.Now;
                user.IsLockedOut = locked;
                user.LastLoginDate = DateTime.Now;



                _db.AddToUsers(user);
                _db.SaveChanges();


                Admin admin = new Admin();
                admin.NIK = Nik;
                admin.UserID = user.UserId;
                admin.Name = Nama;
                _db.AddToAdmins(admin);
                _db.SaveChanges();

                //------ RDF Change
                UserLogModel dataUserNew = new UserLogModel();
                dataUserNew = LogsModel.MappingUserFromEFtoModel(user);

                string messageError = LogsModel.SaveLogAdmin(new Admin(), admin, new UserLogModel(), dataUserNew, "Add");
                if (!string.IsNullOrEmpty(messageError))
                {
                    ViewData["Output"] = messageError;
                    return View();
                }
                //------ End RDF Change

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData["Output"] = e.Message;
                return View();
            }
        }

        //
        // GET: /SalesManager/Edit/5

        public ActionResult Edit(int id)
        {
            if (!(CommonModel.UserRole() == "ADMIN" || CommonModel.UserRole() == "ADMINUSER")) return RedirectToAction("Index", "Home");


            var dataToEdit = (from m in _db.Admins
                              where m.UserID == id
                              select m).First();
            User user = _db.Users.Where(c => c.UserId == dataToEdit.UserID).First();

            ViewData["Role"] = user.Role;
            ViewData["Nik"] = dataToEdit.NIK;
            ViewData["Nama"] = dataToEdit.Name;
            ViewData["Username"] = user.UserName;
            ViewData["Status"] = (user.IsLockedOut) ? "0" : "1";

            return View(dataToEdit);
        }

        //
        // POST: /SalesManager/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Admin dataToEdit)
        {
            //if (CommonModel.UserRole() != "ADMIN") return RedirectToAction("Index", "Home");
            //if (CommonModel.UserRole() != "ADMINUSER") return RedirectToAction("Index", "Home");
            if (!(CommonModel.UserRole() == "ADMIN" || CommonModel.UserRole() == "ADMINUSER")) return RedirectToAction("Index", "Home");

            //ViewData["Branchs"] = _db.Branchs.OrderBy(c => c.Name).ToList();

            try
            {

                var originalData = (from m in _db.Admins
                                    where m.UserID == dataToEdit.UserID
                                    select m).First();

                var originalUser = (from m in _db.Users
                                    where m.UserId == dataToEdit.UserID
                                    select m).First();
                UserLogModel dataUserOld = LogsModel.MappingUserFromEFtoModel(originalUser);

                ViewData["Username"] = Request["Username"];
                ViewData["Nama"] = Request["Nama"];
                ViewData["Status"] = Request["Status"];
                ViewData["Role"] = Request["Role"];

                if (!ModelState.IsValid)
                    return View(dataToEdit);

                string NPK = Request["Nik"];
                string Nama = Request["Nama"];
                string Username = Request["Username"];
                string Password = Request["Password"];
                string ConfirmPassword = Request["ConfirmPassword"];
                string Status = Request["Status"];

                var checkNpk = _db.Admins.Where(c => c.NIK == NPK && c.UserID != dataToEdit.UserID);
                if (checkNpk.Count() > 0) { ViewData["Output"] = "NPK already exists"; return View(dataToEdit); }

                if (String.IsNullOrWhiteSpace(Username)) { ViewData["Output"] = "Invalid Username"; return View(dataToEdit); }
                if (!String.IsNullOrWhiteSpace(Password))
                {
                    if (String.IsNullOrWhiteSpace(Password)) { ViewData["Output"] = "Password cannot be empty"; return View(dataToEdit); }
                    if (!PasswordPolicyModel.IsValid(Password)) { ViewData["Output"] = "Password doesn't meet policy (Minimum length 6, at least 1 capital letter, at least 1 letter non alphanumeric and at least 1 numeric letter)"; return View(dataToEdit); }
                    if (Password != ConfirmPassword) { ViewData["Output"] = "Confirmation password is not match"; return View(dataToEdit); }
                }

                originalData.NIK = NPK;
                originalData.Name = Nama;


                //_db.ApplyCurrentValues(originalData.EntityKey.EntitySetName, dataToEdit);
                //_db.SaveChanges();

                var checkUser = _db.Users.Where(c => c.UserName == Username && c.UserId != dataToEdit.UserID);
                if (checkUser.Count() > 0) { ViewData["Output"] = "Username already exists"; return View(dataToEdit); }

                originalUser.UserName = Username;
                originalUser.Role = Request["Role"];
                originalUser.IsLockedOut = (Status == "0") ? true : false;

                if (!String.IsNullOrWhiteSpace(Password))
                {
                    originalUser.PasswordSalt = UserRepository.CreateSalt();
                    originalUser.Password = UserRepository.CreatePasswordHash(Password, originalUser.PasswordSalt);
                    originalUser.PasswordText = Password;
                }
                //-- add by rdf
                UserLogModel dataUserNew = LogsModel.MappingUserFromEFtoModel(originalUser);
                Admin dataOld = new Admin();
                Admin dataNew = new Admin();
                dataOld = originalData;
                dataNew = dataToEdit;
                string messageError = LogsModel.SaveLogAdmin(dataOld, dataNew, dataUserOld, dataUserNew, "Edit");
                if (!string.IsNullOrEmpty(messageError))
                {
                    ViewData["Output"] = messageError;
                    return View();
                }
                //-- end add by rdf
                _db.ApplyCurrentValues(originalData.EntityKey.EntitySetName, dataToEdit);
                _db.SaveChanges();
                _db.ApplyCurrentValues(originalUser.EntityKey.EntitySetName, originalUser);
                _db.SaveChanges();
                if (Status == "1")
                {
                    CommonModel.ActiveUser3wrongpass(NPK);
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData["Output"] = e.Message;
                return View(dataToEdit);
            }
        }

        public ActionResult Delete(int id)
        {
            //if (CommonModel.UserRole() != "ADMIN") return RedirectToAction("Index", "Home");
            //if (CommonModel.UserRole() != "ADMINUSER") return RedirectToAction("Index", "Home");
            if (!(CommonModel.UserRole() == "ADMIN" || CommonModel.UserRole() == "ADMINUSER")) return RedirectToAction("Index", "Home");


            var dataToDelete = (from m in _db.Admins
                                where m.UserID == id
                                select m).First();

            return View(dataToDelete);
        }


        [HttpPost]
        public ActionResult Delete(int id, Admin dataToDelete)
        {
            //if (CommonModel.UserRole() != "ADMIN") return RedirectToAction("Index", "Home");
            //if (CommonModel.UserRole() != "ADMINUSER") return RedirectToAction("Index", "Home");
            if (!(CommonModel.UserRole() == "ADMIN" || CommonModel.UserRole() == "ADMINUSER")) return RedirectToAction("Index", "Home");


            var originalData = (from m in _db.Admins
                                where m.UserID == id
                                select m).First();

            var originalUser = (from m in _db.Users
                                where m.UserId == id
                                select m).First();
            try
            {

                if (!ModelState.IsValid)
                    return View(originalData);
                //-- add by RDF
                UserLogModel originalUsers = LogsModel.MappingUserFromEFtoModel(originalUser);

                string messageError = LogsModel.SaveLogAdmin(originalData, new Admin(), originalUsers, new UserLogModel(), "Delete");
                if (!string.IsNullOrEmpty(messageError))
                {
                    ViewData["Output"] = messageError;
                    return View(originalData);
                }
                //--- end add by rdf 
                _db.DeleteObject(originalData);
                _db.DeleteObject(originalUser);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData["Output"] = "<h3 style=\"color:red\">" + e.Message + "</h3>";
                return View(originalData);
            }
        }
    }
}
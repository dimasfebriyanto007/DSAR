using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cms.Models;
using System.IO;
using MvcContrib.UI.Grid;
using MvcContrib.Sorting;

namespace cms.Controllers
{
    [Authorize]
    public class RbhController : CommonController
    {

        public ActionResult Index()
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINUSER"))) return RedirectToAction("Index", "Home");

            ViewData["Regions"] = _db.Regions.OrderBy(c => c.Name).ToList();
            return View();
        }

        public ActionResult list(int page = 1, string key = "", int perpage = 20, string orderBy = "", string orderMode = "", string Region = "")
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINUSER"))) return RedirectToAction("Index", "Home");

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
                        where c.Role == "RBH"
                        join d in _db.RBHs on c.RelatedId equals d.RbhId
                        join e in _db.Regions on d.RegionCode equals e.RegionCode into join1                            
                            from resJoin1 in join1.DefaultIfEmpty()
                        select 
                            new
                            {
                                RbhId = d.RbhId,
                                Username = c.UserName,
                                NPK = d.Npk,
                                Name = d.Name,
                                Email = d.Email,
                                Phone = d.Phone,
                                RegionCode = d.RegionCode,
                                RegionName = resJoin1.Name,
                                CreatedDate = c.CreatedDate,
                                LastLoginDate = c.LastLoginDate
                            }
                        ).Where(c => 1==1);            

            if (!String.IsNullOrEmpty(key))
            {
                data = data.Where(c => c.Name.Contains(key) || c.NPK.Contains(key) || c.Username.Contains(key));
            }
            
            if (Region != "")
            {
                data = data.Where(c => c.RegionCode == Region);
            }

            int count = data.Count();

            switch (orderBy)
            {
                case "npk": if (orderMode == "asc") data = data.OrderBy(c => c.NPK); else data = data.OrderByDescending(c => c.NPK); break;
                case "name": if (orderMode == "asc") data = data.OrderBy(c => c.Name); else data = data.OrderByDescending(c => c.Name); break;
                case "region": if (orderMode == "asc") data = data.OrderBy(c => c.RegionName); else data = data.OrderByDescending(c => c.RegionName); break;
                default: if (orderMode == "desc") data = data.OrderByDescending(c => c.LastLoginDate); else data = data.OrderBy(c => c.LastLoginDate); break;
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

            msg += "<table class=\"grid-style\" width=\"100%\"><thead><tr><th>No</th><th>NPK</th><th>Name</th><th>Email</th><th>Phone</th><th>Region</th><th>Last Login</th><th>Option</th>";

            msg += "</tr><tbody>";

            i = dataStart;
            
            foreach (var item in result)
            {
                string date = item.LastLoginDate.ToString("dd MMM yyyy HH:mm");
                string klas = (i % 2 == 0) ? "gridrow_alternate" : "gridrow";                
                
                msg += "<tr class=\"" + klas + "\">" +
                       "<td>" + i + "</td>" +
                       "<td align=\"center\">"+item.NPK+"</td>" +
                       "<td>" + item.Name + "</td>" +                       
                       "<td>" + item.Email + "</td>" +
                       "<td>" + item.Phone + "</td>" +
                       "<td>" + item.RegionName + "</td>" +
                       "<td align=\"center\">" + date + "</td>" +                       
                       "<td align=\"center\"><a href=\"" + Url.Content("~/Rbh/Edit/" + item.RbhId) + "\" title=\"Edit\"><img src=\"" + Url.Content("~/Content/images/icon-edit.gif") + "\" border=\"0\"/></a> <a href=\"" + Url.Content("~/Rbh/Delete/" + item.RbhId) + "\" title=\"Delete\"><img src=\"" + Url.Content("~/Content/images/icon-delete.gif") + "\" border=\"0\"/></a></td>" +
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
        // GET: /Rbh/Create

        public ActionResult Create()
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINUSER"))) return RedirectToAction("Index", "Home");

            ViewData["Regions"] = _db.Regions.OrderBy(c => c.Name).ToList();
            ViewData["Status"] = "1";
            return View();
        }

        //
        // POST: /Rbh/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Exclude = "RbhId")] RBH dataToCreate)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINUSER"))) return RedirectToAction("Index", "Home");

            ViewData["Regions"] = _db.Regions.OrderBy(c => c.Name).ToList();
            ViewData["Username"] = Request["Username"];

            try
            {                
                if (!ModelState.IsValid) return View(dataToCreate);

                string Username = Request["Username"];
                string Password = Request["Password"];
                string ConfirmPassword = Request["ConfirmPassword"];
                string Status = Request["Status"];
                ViewData["Status"] = Status;

                var checkNpk = _db.RBHs.Where(c => c.Npk == dataToCreate.Npk);
                if (checkNpk.Count() > 0) { ViewData["Output"] = "NPK already exists"; return View(dataToCreate); }

                if (String.IsNullOrWhiteSpace(Username)) { ViewData["Output"] = "Invalid Username"; return View(dataToCreate); }
                if (String.IsNullOrWhiteSpace(Password)) { ViewData["Output"] = "Password cannot be empty"; return View(dataToCreate); }
                if (!PasswordPolicyModel.IsValid(Password)) { ViewData["Output"] = "Password doesn't meet policy (Minimum length 6, at least 1 capital letter, at least 1 letter non alphanumeric and at least 1 numeric letter)"; return View(dataToCreate); }
                if (Password!=ConfirmPassword) { ViewData["Output"] = "Confirmation password is not match"; return View(dataToCreate); }

                var checkUser = _db.Users.Where(c => c.UserName == Username);
                if (checkUser.Count() > 0) { ViewData["Output"] = "Username already exists"; return View(dataToCreate); }

                _db.AddToRBHs(dataToCreate);
                _db.SaveChanges();

                bool locked = (Status == "0") ? true : false;

                User user = new User();
                user.UserName = Username;
                user.PasswordSalt = UserRepository.CreateSalt();
                user.Password = UserRepository.CreatePasswordHash(Password, user.PasswordSalt);
                user.PasswordText = Password;
                user.Role = "RBH";
                user.RelatedId = dataToCreate.RbhId;
                user.CreatedDate = DateTime.Now;
                user.IsLockedOut = locked;
                user.LastLoginDate = DateTime.Now;

                _db.AddToUsers(user);
                _db.SaveChanges();
                //------ RDF Change
                UserLogModel dataUserNew = new UserLogModel();
                dataUserNew = LogsModel.MappingUserFromEFtoModel(user);

                string messageError = LogsModel.SaveLogRBH(new RBH(), dataToCreate, new UserLogModel(), dataUserNew, "Add");
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
        // GET: /Rbh/Edit/5

        public ActionResult Edit(int id)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINUSER"))) return RedirectToAction("Index", "Home");

            ViewData["Regions"] = _db.Regions.OrderBy(c => c.Name).ToList();
            
            var dataToEdit = (from m in _db.RBHs
                              where m.RbhId == id
                              select m).First();
            User user = _db.Users.Where(c => c.RelatedId == dataToEdit.RbhId && c.Role == "RBH").First();

            ViewData["Username"] = user.UserName;
            ViewData["Status"] = (user.IsLockedOut) ? "0" : "1";
            
            return View(dataToEdit);
        }

        //
        // POST: /Rbh/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(RBH dataToEdit)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINUSER"))) return RedirectToAction("Index", "Home");

            ViewData["Regions"] = _db.Regions.OrderBy(c => c.Name).ToList();
            
            try
            {
                
                var originalData = (from m in _db.RBHs
                                    where m.RbhId == dataToEdit.RbhId
                                    select m).First();

                var originalUser = (from m in _db.Users
                                    where m.RelatedId == dataToEdit.RbhId && m.Role == "RBH"
                                    select m).First();
                UserLogModel dataUserOld = LogsModel.MappingUserFromEFtoModel(originalUser);

                ViewData["Username"] = Request["Username"];
                ViewData["Status"] = Request["Status"];

                if (!ModelState.IsValid)
                    return View(dataToEdit);

                string Username = Request["Username"];
                string Password = Request["Password"];
                string ConfirmPassword = Request["ConfirmPassword"];                
                string Status = Request["Status"];

                var checkNpk = _db.RBHs.Where(c => c.Npk == dataToEdit.Npk && c.RbhId!=originalData.RbhId);
                if (checkNpk.Count() > 0) { ViewData["Output"] = "NPK already exists"; return View(dataToEdit); }

                if (String.IsNullOrWhiteSpace(Username)) { ViewData["Output"] = "Invalid Username"; return View(dataToEdit); }
                if (!String.IsNullOrWhiteSpace(Password))
                {
                    if (String.IsNullOrWhiteSpace(Password)) { ViewData["Output"] = "Password cannot be empty"; return View(dataToEdit); }
                    if (!PasswordPolicyModel.IsValid(Password)) { ViewData["Output"] = "Password doesn't meet policy (Minimum length 6, at least 1 capital letter, at least 1 letter non alphanumeric and at least 1 numeric letter)"; return View(dataToEdit); }
                    if (Password != ConfirmPassword) { ViewData["Output"] = "Confirmation password is not match"; return View(dataToEdit); }
                }

                

                var checkUser = _db.Users.Where(c => c.UserName == Username && c.UserId != originalUser.UserId);
                if (checkUser.Count() > 0) { ViewData["Output"] = "Username already exists"; return View(dataToEdit); }                

                originalUser.UserName = Username;
                originalUser.IsLockedOut = (Status=="0") ? true : false;

                if (!String.IsNullOrWhiteSpace(Password))
                {
                    originalUser.PasswordSalt = UserRepository.CreateSalt();
                    originalUser.Password = UserRepository.CreatePasswordHash(Password, originalUser.PasswordSalt);
                    originalUser.PasswordText = Password;
                }
                //-- add by rdf
                UserLogModel dataUserNew = LogsModel.MappingUserFromEFtoModel(originalUser);
                RBH dataOld = new RBH();
                RBH dataNew = new RBH();
                dataOld = originalData;
                dataNew = dataToEdit;
                string messageError = LogsModel.SaveLogRBH(dataOld, dataNew, dataUserOld, dataUserNew, "Edit");
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
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINUSER"))) return RedirectToAction("Index", "Home");

            var dataToDelete = (from m in _db.RBHs
                                where m.RbhId == id
                                select m).First();

            return View(dataToDelete);
        }
        

        [HttpPost]
        public ActionResult Delete(int id, RBH dataToDelete)
        {
            var originalData = (from m in _db.RBHs
                                where m.RbhId == id
                                select m).First();

            var originalUser = (from m in _db.Users
                                where m.RelatedId == originalData.RbhId && m.Role == "RBH"
                                select m).First();
            try
            {

                if (!ModelState.IsValid)
                    return View(originalData);
                //-- add by RDF
                UserLogModel originalUsers = LogsModel.MappingUserFromEFtoModel(originalUser);

                string messageError = LogsModel.SaveLogRBH(originalData, new RBH(), originalUsers, new UserLogModel(), "Delete");
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

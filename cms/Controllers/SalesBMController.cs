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
    public class SalesBMController : CommonController
    {

        public ActionResult Index()
        {
            if (CommonModel.UserRole() != "BM") return RedirectToAction("Index", "Home");

            ViewData["Teams"] = _db.SalesTeams.OrderBy(c => c.Name).ToList();
            ViewData["Branchs"] = _db.Branchs.OrderBy(c => c.Name).ToList();
            return View();
        }

        public ActionResult list(int page = 1, string key = "", int perpage = 20, string orderBy = "", string orderMode = "", string Team = "", string Branch = "")
        {
            if (CommonModel.UserRole() != "BM") return RedirectToAction("Index", "Home");

            BranchManager bm = _db.BranchManagers.Where(c => c.BmId == user.RelatedId).First();

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
                        where c.Role == "SALES"
                        join d in _db.Sales on c.RelatedId equals d.SalesId
                            where d.BranchCode == bm.BranchCode
                        join e in _db.Branchs on d.BranchCode equals e.BranchCode into join1                            
                            from resJoin1 in join1.DefaultIfEmpty()
                        join f in _db.SalesManagers on d.SmId equals f.ManagerId into join2                            
                            from resJoin2 in join2.DefaultIfEmpty()
                        join g in _db.SalesTeams on d.TeamId equals g.TeamId
                        select 
                            new
                            {
                                SalesId = d.SalesId,
                                Username = c.UserName,
                                NPK = d.Npk,
                                Name = d.Name,
                                Email = d.Email,
                                Phone = d.Phone,
                                TeamId = d.TeamId,
                                TeamName = g.Name,
                                BranchCode = d.BranchCode,
                                BranchName = resJoin1.Name,
                                ManagerName = resJoin2.Name,
                                CreatedDate = c.CreatedDate,
                                LastLoginDate = c.LastLoginDate

                            }
                        ).Where(c => 1==1);            

            if (!String.IsNullOrEmpty(key))
            {
                data = data.Where(c => c.Name.Contains(key) || c.NPK.Contains(key) || c.Username.Contains(key));
            }

            if (Team != "")
            {
                int TeamId = Convert.ToInt32(Team);
                data = data.Where(c => c.TeamId == TeamId);
            }

            if (Branch != "")
            {
                data = data.Where(c => c.BranchCode == Branch);
            }

            int count = data.Count();

            switch (orderBy)
            {
                case "npk": if (orderMode == "asc") data = data.OrderBy(c => c.NPK); else data = data.OrderByDescending(c => c.NPK); break;
                case "name": if (orderMode == "asc") data = data.OrderBy(c => c.Name); else data = data.OrderByDescending(c => c.Name); break;
                case "team": if (orderMode == "asc") data = data.OrderBy(c => c.TeamName); else data = data.OrderByDescending(c => c.TeamName); break;
                case "branch": if (orderMode == "asc") data = data.OrderBy(c => c.BranchName); else data = data.OrderByDescending(c => c.BranchName); break;
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

            msg += "<table class=\"grid-style\" width=\"100%\"><thead><tr><th>No</th><th>NPK</th><th>Name</th><th>Role</th><th>Leader</th><th>Branch</th><th>Email</th><th>Phone</th><th>Last Login</th><th>Option</th>";

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
                       "<td>" + item.TeamName + "</td>" +
                       "<td>" + item.ManagerName + "</td>" +
                       "<td>" + item.BranchName + "</td>" +
                       "<td>" + item.Email + "</td>" +
                       "<td>" + item.Phone + "</td>" +
                       "<td align=\"center\">" + date + "</td>" +                       
                       "<td align=\"center\"><a href=\"" + Url.Content("~/SalesBM/Edit/" + item.SalesId) + "\" title=\"Edit\"><img src=\"" + Url.Content("~/Content/images/icon-edit.gif") + "\" border=\"0\"/></a></td>" +
                       "</tr>";
                // <a href=\"" + Url.Content("~/Sales/Delete/" + item.SalesId) + "\" title=\"Delete\"><img src=\"" + Url.Content("~/Content/images/icon-delete.gif") + "\" border=\"0\"/></a>
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
        // GET: /Sales/Create

        public ActionResult Create()
        {
            if (CommonModel.UserRole() != "BM") return RedirectToAction("Index", "Home");
            
            BranchManager bm = _db.BranchManagers.Where(c => c.BmId == user.RelatedId).First();

            ViewData["Teams"] = _db.SalesTeams.OrderBy(c => c.Name).ToList();
            ViewData["Branchs"] = _db.Branchs.OrderBy(c => c.Name).ToList();
            ViewData["Leaders"] = _db.SalesManagers.Where(c => c.BranchCode == bm.BranchCode).OrderBy(c => c.Name).ToList();
            ViewData["BMs"] = _db.BranchManagers.OrderBy(c => c.Name).ToList();
            ViewData["Status"] = "1";

            return View();
        }

        //
        // POST: /Sales/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Exclude = "SalesId")] Sale dataToCreate)
        {
            if (CommonModel.UserRole() != "BM") return RedirectToAction("Index", "Home");
            BranchManager bm = _db.BranchManagers.Where(c => c.BmId == user.RelatedId).First();

            ViewData["Teams"] = _db.SalesTeams.OrderBy(c => c.Name).ToList();
            ViewData["Branchs"] = _db.Branchs.OrderBy(c => c.Name).ToList();
            ViewData["Leaders"] = _db.SalesManagers.Where(c => c.BranchCode == bm.BranchCode).OrderBy(c => c.Name).ToList();
            ViewData["BMs"] = _db.BranchManagers.OrderBy(c => c.Name).ToList();
            ViewData["Username"] = Request["Username"];

            try
            {
                dataToCreate.JoinDate = DateTime.Now;
                dataToCreate.BranchCode = bm.BranchCode;

                if (!ModelState.IsValid) return View(dataToCreate);

                string Username = Request["Username"];
                string Password = Request["Password"];
                string ConfirmPassword = Request["ConfirmPassword"];
                string Status = Request["Status"];
                ViewData["Status"] = Status;

                var checkNpk = _db.Sales.Where(c => c.Npk == dataToCreate.Npk);
                if (checkNpk.Count() > 0) { ViewData["Output"] = "NPK already exists"; return View(dataToCreate); }

                // added by Aditia S - 20160830


                if (dataToCreate.Npk.Substring(0, 2).ToUpper() == "BM")
                {
                    dataToCreate.Npk = dataToCreate.Npk.ToUpper();
                    if (dataToCreate.Npk.Length != 5) { ViewData["Output"] = "NPK for BM have to 5 digits (BMXXX) !"; return View(dataToCreate); }
                    if (!CommonModel.isNumber(dataToCreate.Npk.Substring(2, 3))) { ViewData["Output"] = "Digit : 3, 4 & 5 have to numeric !"; return View(dataToCreate); }
                }
                else
                {
                    if (dataToCreate.Npk == "0000000000") { ViewData["Output"] = "Invalid NPK"; return View(dataToCreate); }
                    if (dataToCreate.Npk.Length != 10) { ViewData["Output"] = "NPK have to 10 digits"; return View(dataToCreate); }
                    if (!CommonModel.isNumber(dataToCreate.Npk)) { ViewData["Output"] = "NPK is numeric only"; return View(dataToCreate); }
                }

                // ---------------------------- EOF Aditia S

                if (String.IsNullOrWhiteSpace(Username)) { ViewData["Output"] = "Invalid Username"; return View(dataToCreate); }
                if (String.IsNullOrWhiteSpace(Password)) { ViewData["Output"] = "Password cannot be empty"; return View(dataToCreate); }
                if (!PasswordPolicyModel.IsValid(Password)) { ViewData["Output"] = "Password doesn't meet policy (Minimum length 6, at least 1 capital letter, at least 1 letter non alphanumeric and at least 1 numeric letter)"; return View(dataToCreate); }
                if (Password!=ConfirmPassword) { ViewData["Output"] = "Confirmation password is not match"; return View(dataToCreate); }

                var checkUser = _db.Users.Where(c => c.UserName == Username);
                if (checkUser.Count() > 0) { ViewData["Output"] = "Username already exists"; return View(dataToCreate); }

                _db.AddToSales(dataToCreate);
                _db.SaveChanges();

                bool locked = (Status == "0") ? true : false;

                User user = new User();
                user.UserName = Username;
                user.PasswordSalt = UserRepository.CreateSalt();
                user.Password = UserRepository.CreatePasswordHash(Password, user.PasswordSalt);
                user.PasswordText = Password;
                user.Role = "SALES";
                user.RelatedId = dataToCreate.SalesId;
                user.CreatedDate = DateTime.Now;
                user.IsLockedOut = locked;
                user.LastLoginDate = DateTime.Now;

                _db.AddToUsers(user);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData["Output"] = e.Message;
                return View();
            }
        }

        //
        // GET: /Sales/Edit/5

        public ActionResult Edit(int id)
        {
            if (CommonModel.UserRole() != "BM") return RedirectToAction("Index", "Home");

            BranchManager bm = _db.BranchManagers.Where(c => c.BmId == user.RelatedId).First();

            ViewData["Teams"] = _db.SalesTeams.OrderBy(c => c.Name).ToList();
            ViewData["Branchs"] = _db.Branchs.OrderBy(c => c.Name).ToList();
            ViewData["Leaders"] = _db.SalesManagers.Where(c => c.BranchCode == bm.BranchCode).OrderBy(c => c.Name).ToList();
            ViewData["BMs"] = _db.BranchManagers.OrderBy(c => c.Name).ToList();

            var dataToEdit = (from m in _db.Sales
                              where m.SalesId == id
                              select m).First();
            User usr = _db.Users.Where(c => c.RelatedId == dataToEdit.SalesId && c.Role == "SALES").First();

            ViewData["Username"] = usr.UserName;
            ViewData["Status"] = (usr.IsLockedOut) ? "0" : "1";
            
            return View(dataToEdit);
        }

        //
        // POST: /Sales/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Sale dataToEdit)
        {
            if (CommonModel.UserRole() != "BM") return RedirectToAction("Index", "Home");

            BranchManager bm = _db.BranchManagers.Where(c => c.BmId == user.RelatedId).First();

            ViewData["Teams"] = _db.SalesTeams.OrderBy(c => c.Name).ToList();
            ViewData["Branchs"] = _db.Branchs.OrderBy(c => c.Name).ToList();
            ViewData["Leaders"] = _db.SalesManagers.Where(c => c.BranchCode == bm.BranchCode).OrderBy(c => c.Name).ToList();
            ViewData["BMs"] = _db.BranchManagers.OrderBy(c => c.Name).ToList();
            
            try
            {
                
                var originalData = (from m in _db.Sales
                                    where m.SalesId == dataToEdit.SalesId
                                    select m).First();

                var originalUser = (from m in _db.Users
                                    where m.RelatedId == dataToEdit.SalesId && m.Role == "SALES"
                                    select m).First();

                ViewData["Status"] = Request["Status"];

                dataToEdit.JoinDate = originalData.JoinDate;
                dataToEdit.BmId = originalData.BmId;
                dataToEdit.BranchCode = originalData.BranchCode;

                if (!ModelState.IsValid)
                    return View(dataToEdit);

                string Status = Request["Status"];

                var checkNpk = _db.Sales.Where(c => c.Npk == dataToEdit.Npk && c.SalesId!=originalData.SalesId);
                if (checkNpk.Count() > 0) { ViewData["Output"] = "NPK already exists"; return View(dataToEdit); }

                _db.ApplyCurrentValues(originalData.EntityKey.EntitySetName, dataToEdit);
                _db.SaveChanges();

                originalUser.IsLockedOut = (Status=="0") ? true : false;

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
            if (CommonModel.UserRole() != "TEMP") return RedirectToAction("Index", "Home");

            var dataToDelete = (from m in _db.Sales
                                where m.SalesId == id
                                select m).First();

            return View(dataToDelete);
        }
        

        [HttpPost]
        public ActionResult Delete(int id, Sale dataToDelete)
        {
            if (CommonModel.UserRole() != "TEMP") return RedirectToAction("Index", "Home");

            var originalData = (from m in _db.Sales
                                where m.SalesId == id
                                select m).First();

            var originalUser = (from m in _db.Users
                                where m.RelatedId == originalData.SalesId && m.Role == "SALES"
                                select m).First();
            try
            {

                if (!ModelState.IsValid)
                    return View(originalData);
                                
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

﻿using System;
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
    public class BranchController : CommonController
    {

        public ActionResult Index()
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");

            return View();
        }

        public ActionResult list(int page = 1, string key = "", int perpage = 20, string orderBy = "", string orderMode = "")
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");

            double cur_page = page;
            page -= 1;
            int per_page = perpage;
            bool first_btn = true;
            bool last_btn = true;
            bool previous_btn = true;
            bool next_btn = true;
            int start = page * per_page;
            string msg = "";

            var data = (from c in _db.Branchs select c).Where(c => 1 == 1);

            if (!String.IsNullOrEmpty(key))
            {
                data = data.Where(c => c.Name.Contains(key) || c.Area.Name.Contains(key));
            }

            int count = data.Count();

            switch (orderBy)
            {
                case "name": if (orderMode == "asc") data = data.OrderBy(c => c.Name); else data = data.OrderByDescending(c => c.Name); break;
                case "area": if (orderMode == "asc") data = data.OrderBy(c => c.Area.Name); else data = data.OrderByDescending(c => c.Area.Name); break;
                default: if (orderMode == "asc") data = data.OrderBy(c => c.BranchCode); else data = data.OrderByDescending(c => c.BranchCode); break;
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

            msg += "<table class=\"grid-style\" style=\"width:600px\"><thead><tr><th>Branch Code</th><th>Branch Name</th><th>Area</th><th>Option</th>";

            msg += "</tr><tbody>";

            i = dataStart;

            foreach (var item in result)
            {
                string klas = (i % 2 == 0) ? "gridrow_alternate" : "gridrow";

                msg += "<tr class=\"" + klas + "\">" +
                       "<td align=\"center\">" + item.BranchCode + "</td>" +
                       "<td>" + item.Name + "</td>" +
                       "<td>" + item.Area.Name + "</td>" +
                       "<td align=\"center\"><a href=\"" + Url.Content("~/Branch/Edit/" + item.BranchCode) + "\" title=\"Edit\"><img src=\"" + Url.Content("~/Content/images/icon-edit.gif") + "\" border=\"0\"/></a> <a href=\"" + Url.Content("~/Branch/Delete/" + item.BranchCode) + "\" title=\"Delete\"><img src=\"" + Url.Content("~/Content/images/icon-delete.gif") + "\" border=\"0\"/></a></td>" +
                       "</tr>";

                i++;
            }
            msg += "</tbody></table>";


            if (i == 0) msg += "<div class=\"arial\" style=\"width:600px;text-align:center;padding:10px 0;\">No result found</div>";

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
        // GET: /Product/Create

        public ActionResult Create()
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");
            ViewData["Areas"] = _db.Areas.OrderBy(c => c.Name).ToList();
            return View();
        }

        //
        // POST: /Product/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Branch dataToCreate)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");
            ViewData["Areas"] = _db.Areas.OrderBy(c => c.Name).ToList();

            try
            {
                if (!ModelState.IsValid) return View(dataToCreate);


                _db.AddToBranchs(dataToCreate);
                _db.SaveChanges();

                string MessageError = LogsModel.SaveLogBranch(new Branch(), dataToCreate, "Add");

                if (string.IsNullOrEmpty(MessageError))
                {
                    ViewData["Output"] = "Data successfully saved";

                    return View("Index");
                }
                else
                {
                    ViewData["Output"] = MessageError;
                    return View();
                }
            }
            catch (Exception e)
            {
                ViewData["Output"] = e.Message;
                return View();
            }
        }

        //
        // GET: /Product/Edit/5

        public ActionResult Edit(string id)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");
            ViewData["Areas"] = _db.Areas.OrderBy(c => c.Name).ToList();

            var dataToEdit = (from m in _db.Branchs
                              where m.BranchCode == id
                              select m).First();

            return View(dataToEdit);
        }

        //
        // POST: /Product/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Branch dataToEdit)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");
            ViewData["Areas"] = _db.Areas.OrderBy(c => c.Name).ToList();

            try
            {

                var originalData = (from m in _db.Branchs
                                    where m.BranchCode == dataToEdit.BranchCode
                                    select m).First();

                if (!ModelState.IsValid)
                    return View(dataToEdit);
                string MessageError = LogsModel.SaveLogBranch(originalData, dataToEdit, "Edit");

                if (!string.IsNullOrEmpty(MessageError))
                {
                    ViewData["Output"] = MessageError;

                    return View(dataToEdit);
                }
                _db.ApplyCurrentValues(originalData.EntityKey.EntitySetName, dataToEdit);
                _db.SaveChanges();

                ViewData["Output"] = "Data successfully saved";

                return View("Index");
            }
            catch (Exception e)
            {
                ViewData["Output"] = e.Message;
                return View(dataToEdit);
            }
        }

        public ActionResult Delete(string id)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");

            var dataToDelete = (from m in _db.Branchs
                                where m.BranchCode == id
                                select m).First();

            return View(dataToDelete);
        }


        [HttpPost]
        public ActionResult Delete(string id, Branch dataToDelete)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");

            var originalData = (from m in _db.Branchs
                                where m.BranchCode == id
                                select m).First();
            string MessageError = MessageError = LogsModel.SaveLogBranch(originalData, new Branch(), "Delete");
            if (string.IsNullOrEmpty(MessageError))
            {
                try
                {

                    if (!ModelState.IsValid)
                        return View(originalData);

                    _db.DeleteObject(originalData);
                    _db.SaveChanges();

                    ViewData["Output"] = "Data successfully deleted";

                    return View("Index");
                }
                catch (Exception e)
                {
                    ViewData["Output"] = "<h3 style=\"color:red\">" + e.Message + "</h3>";
                    return View(originalData);
                }
            }
            ViewData["Output"] = "<h3 style=\"color:red\">" + MessageError + "</h3>";
            return View(originalData);
        }

    }
}

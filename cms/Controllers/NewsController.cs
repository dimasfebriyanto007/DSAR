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
    public class NewsController : CommonController
    {
        public NewsController()
        {
            ViewData["YesNoOption"] = YesNoOption;
        }

        public ActionResult Index()
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");

            return View();
        }

        public ActionResult Home()
        {
            var data = _db.News.Where(c => c.Publish == 1).OrderByDescending(c => c.NewsDate).Take(6);
            string msg = "<div class=\"news\">";

            foreach (News item in data)
            {
                string date = item.NewsDate.ToString("dd MMM yyyy HH:mm");

                msg += "<div class=\"news-title\"><a href=\"" + Url.Content("~/News/Read/" + item.NewsId) + "\">" + item.Title + "</a></div>" +
                       "<div class=\"news-date\">" + date + "</div>" +
                       "<div class=\"news-intro\">" + item.IntroText + "</div>";
            }

            msg += "</div>";

            ViewBag.Output = msg;

            return View("output");
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

            var data = _db.News.Where(c => 1 == 1);

            if (!String.IsNullOrEmpty(key))
            {
                data = data.Where(c => c.Title.Contains(key) || c.IntroText.Contains(key) || c.ContentText.Contains(key));
            }

            int count = data.Count();

            switch (orderBy)
            {
                case "title": if (orderMode == "asc") data = data.OrderBy(c => c.Title); else data = data.OrderByDescending(c => c.Title); break;
                default: if (orderMode == "desc") data = data.OrderByDescending(c => c.NewsDate); else data = data.OrderBy(c => c.NewsDate); break;
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

            msg += "<table class=\"grid-style\" width=\"100%\"><thead><tr><th>No</th><th>Title</th><th>Intro Text</th><th>Publish</th><th>Date</th><th>Option</th>";

            msg += "</tr><tbody>";

            i = dataStart;

            foreach (var item in result)
            {
                string publish = (item.Publish == 1) ? "<img src=\"" + Url.Content("~/Content/images/icon-yes.gif") + "\" />" : "<img src=\"" + Url.Content("~/Content/images/icon-no.gif") + "\" />";
                string date = item.NewsDate.ToString("dd MMM yyyy HH:mm");
                string klas = (i % 2 == 0) ? "gridrow_alternate" : "gridrow";

                msg += "<tr class=\"" + klas + "\">" +
                       "<td>" + i + "</td>" +
                       "<td>" + item.Title + "</td>" +
                       "<td>" + item.IntroText + "</td>" +
                       "<td style=\"text-align:center\">" + publish + "</td>" +
                       "<td align=\"center\">" + date + "</td>" +
                       "<td align=\"center\"><a href=\"" + Url.Content("~/News/Edit/" + item.NewsId) + "\" title=\"Edit\"><img src=\"" + Url.Content("~/Content/images/icon-edit.gif") + "\" border=\"0\"/></a> <a href=\"" + Url.Content("~/News/Delete/" + item.NewsId) + "\" title=\"Delete\"><img src=\"" + Url.Content("~/Content/images/icon-delete.gif") + "\" border=\"0\"/></a></td>" +
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

        public ActionResult Read(int id)
        {
            var dataToEdit = (from m in _db.News
                              where m.NewsId == id
                              select m).First();

            return View(dataToEdit);
        }

        //
        // GET: /News/Create

        public ActionResult Create()
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");

            return View();
        }

        //
        // POST: /News/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Exclude = "NewsId")] News dataToCreate)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");

            try
            {
                dataToCreate.NewsDate = DateTime.Now;

                if (!ModelState.IsValid) return View(dataToCreate);

                _db.AddToNews(dataToCreate);
                _db.SaveChanges();

                string MessageError = LogsModel.SaveLogNews(new News(), dataToCreate, "Add");

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
        // GET: /News/Edit/5

        public ActionResult Edit(int id)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");

            var dataToEdit = (from m in _db.News
                              where m.NewsId == id
                              select m).First();

            return View(dataToEdit);
        }

        //
        // POST: /News/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(News dataToEdit)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");

            try
            {

                var originalData = (from m in _db.News
                                    where m.NewsId == dataToEdit.NewsId
                                    select m).First();

                dataToEdit.NewsDate = originalData.NewsDate;

                if (!ModelState.IsValid)
                    return View(dataToEdit);
                string MessageError = LogsModel.SaveLogNews(originalData, dataToEdit, "Edit");
                if (!string.IsNullOrEmpty(MessageError))
                {
                    ViewData["Output"] = MessageError;

                    return View(dataToEdit);
                }

                _db.ApplyCurrentValues(originalData.EntityKey.EntitySetName, dataToEdit);
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
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");

            var dataToDelete = (from m in _db.News
                                where m.NewsId == id
                                select m).First();

            return View(dataToDelete);
        }


        [HttpPost]
        public ActionResult Delete(int id, News dataToDelete)
        {
            var originalData = (from m in _db.News
                                where m.NewsId == id
                                select m).First();
            string MessageError = MessageError = LogsModel.SaveLogNews(originalData, new News(), "Delete");


            if (string.IsNullOrEmpty(MessageError))
            {
                try
                {

                    _db.DeleteObject(originalData);
                    _db.SaveChanges();

                    return RedirectToAction("Index");
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

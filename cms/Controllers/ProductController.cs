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
    public class ProductController : CommonController
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

            var data = (from c in _db.Products select c).Where(c => 1 == 1);

            if (!String.IsNullOrEmpty(key))
            {
                data = data.Where(c => c.Name.Contains(key) || c.Code.Contains(key));
            }

            int count = data.Count();

            switch (orderBy)
            {
                default: if (orderMode == "asc") data = data.OrderBy(c => c.Name); else data = data.OrderByDescending(c => c.Name); break;
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

            msg += "<table class=\"grid-style\" width=\"100%\"><thead><tr><th>No</th><th>Product Name</th><th>Product Code</th><th>Category</th><th>Description</th><th>Option</th>";

            msg += "</tr><tbody>";

            i = dataStart;

            foreach (var item in result)
            {
                string klas = (i % 2 == 0) ? "gridrow_alternate" : "gridrow";

                msg += "<tr class=\"" + klas + "\">" +
                       "<td align=\"center\">" + i + "</td>" +
                       "<td>" + item.Name + "</td>" +
                       "<td align=\"center\">" + item.Code + "</td>" +
                       "<td>" + item.ProductCategory.Name + "</td>" +
                       "<td>" + item.Description + "</td>" +
                       "<td align=\"center\"><a href=\"" + Url.Content("~/Product/Edit/" + item.ProductId) + "\" title=\"Edit\"><img src=\"" + Url.Content("~/Content/images/icon-edit.gif") + "\" border=\"0\"/></a> <a href=\"" + Url.Content("~/Product/Delete/" + item.ProductId) + "\" title=\"Delete\"><img src=\"" + Url.Content("~/Content/images/icon-delete.gif") + "\" border=\"0\"/></a></td>" +
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
        // GET: /Product/Create

        public ActionResult Create()
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");
            ViewData["Categories"] = _db.ProductCategories.OrderBy(c => c.Name).ToList();
            return View();
        }

        //
        // POST: /Product/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Exclude = "ProductId")] Product dataToCreate)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");

            ViewData["Categories"] = _db.ProductCategories.OrderBy(c => c.Name).ToList();
            HttpPostedFileBase hpf1 = Request.Files["image"];

            try
            {
                if (!ModelState.IsValid) return View(dataToCreate);

                int NexOrderId = 1;
                var checkProd = _db.Products.OrderByDescending(c => c.ProductId);
                if (checkProd.Count() > 0)
                {
                    Product prod = checkProd.First();
                    NexOrderId = prod.ProductId + 1;
                }

                if (hpf1 != null)
                {
                    if (!hpf1.ContentType.ToLower().Contains("image"))
                    {
                        ViewData["Output"] = "Image popup yang diperbolehkan adalah tipe file gambar";
                        return View(dataToCreate);
                    }

                    if (hpf1.ContentLength > 0)
                    {
                        string serverPath = Server.MapPath("~");
                        string imagePath = serverPath + "\\images\\product\\";
                        DirectoryInfo dirInfo = new DirectoryInfo(imagePath);
                        CommonModel.CreateDirectory(dirInfo);
                        string imageName = NexOrderId + "_" + CommonModel.FriendlyString(dataToCreate.Name);

                        CommonModel.ResizeAndSave(imagePath, imageName, hpf1.InputStream, 900, false, "jpg");

                        dataToCreate.ImageName = imageName;
                        _db.ApplyCurrentValues(dataToCreate.EntityKey.EntitySetName, dataToCreate);
                        _db.SaveChanges();
                    }
                }

                _db.AddToProducts(dataToCreate);
                _db.SaveChanges();

                //Product OldData = new Product();

                string MessageError = LogsModel.SaveLogProduct(new Product(), dataToCreate, "Add");

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

        public ActionResult Edit(int id)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");

            ViewData["Categories"] = _db.ProductCategories.OrderBy(c => c.Name).ToList();

            var dataToEdit = (from m in _db.Products
                              where m.ProductId == id
                              select m).First();

            return View(dataToEdit);
        }

        //
        // POST: /Product/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Product dataToEdit)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");

            ViewData["Categories"] = _db.ProductCategories.OrderBy(c => c.Name).ToList();
            HttpPostedFileBase hpf1 = Request.Files["image"];

            //try
            //{
            string MessageError = string.Empty;
            try
            {
                var originalData = (from m in _db.Products
                                    where m.ProductId == dataToEdit.ProductId
                                    select m).First();

                Product originalDatas = originalData;

                dataToEdit.ImageName = originalData.ImageName;

                if (!ModelState.IsValid)
                    return View(dataToEdit);

                if (hpf1 != null)
                {
                    if (!hpf1.ContentType.ToLower().Contains("image"))
                    {
                        ViewData["Output"] = "Image popup yang diperbolehkan adalah tipe file gambar";
                        return View(dataToEdit);
                    }

                    if (hpf1.ContentLength > 0)
                    {
                        string serverPath = Server.MapPath("~");
                        string imagePath = serverPath + "\\images\\product\\";
                        DirectoryInfo dirInfo = new DirectoryInfo(imagePath);
                        CommonModel.CreateDirectory(dirInfo);
                        string imageName = dataToEdit.ProductId + "_" + CommonModel.FriendlyString(dataToEdit.Name);

                        CommonModel.ResizeAndSave(imagePath, imageName, hpf1.InputStream, 900, false, "jpg");
                        dataToEdit.ImageName = imageName;

                        MessageError = LogsModel.SaveLogProduct(originalDatas, dataToEdit, "Edit");
                        if (!string.IsNullOrEmpty(MessageError))
                        {
                            ViewData["Output"] = MessageError;

                            return View(dataToEdit);
                        }
                        _db.ApplyCurrentValues(originalData.EntityKey.EntitySetName, dataToEdit);
                        _db.SaveChanges();
                    }
                }

                _db.ApplyCurrentValues(originalData.EntityKey.EntitySetName, dataToEdit);
                _db.SaveChanges();


            }
            catch (Exception ex)
            {

                ViewData["Output"] = ex.Message;

                return View(dataToEdit);
            }

            ViewData["Output"] = "Data successfully saved";

            return View("Index");
            //else
            //{
            //    ViewData["Output"] = MessageError;

            //    return View("Index");
            //}


            //}
            //catch (Exception e)
            //{
            //    ViewData["Output"] = e.Message;
            //    return View(dataToEdit);
            //}
        }

        public ActionResult Delete(int id)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");

            var dataToDelete = (from m in _db.Products
                                where m.ProductId == id
                                select m).First();

            return View(dataToDelete);
        }


        [HttpPost]
        public ActionResult Delete(int id, Product dataToDelete)
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINBUSSINES"))) return RedirectToAction("Index", "Home");


            var originalData = (from m in _db.Products
                                where m.ProductId == id
                                select m).First();
            string MessageError = MessageError = LogsModel.SaveLogProduct(originalData, new Product(), "Delete");


            if (string.IsNullOrEmpty(MessageError))
            {
                try
                {

                    if (!ModelState.IsValid)
                        return View(originalData);

                    if (!String.IsNullOrWhiteSpace(originalData.ImageName))
                    {
                        string serverPath = Server.MapPath("~");
                        string imagePath = serverPath + "\\images\\product\\";
                        FileInfo fInfo = new FileInfo(imagePath + originalData.ImageName + ".jpg");
                        if (fInfo.Exists)
                        {
                            fInfo.Delete();
                        }
                    }


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

        public ActionResult Detail(int id)
        {
            var dataToEdit = (from m in _db.Products
                              where m.ProductId == id
                              select m).First();

            return View(dataToEdit);
        }

    }
}

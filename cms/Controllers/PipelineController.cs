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
    public class PipelineController : CommonController
    {
        
        public ActionResult Index()
        {
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }

            switch (CommonModel.UserRole())
            {
                case "SALES": ViewData["Sales"] = _db.Sales.Where(c => c.SalesId == user.RelatedId).OrderBy(c => c.Name);
                    break;
                case "SM": ViewData["Sales"] = _db.Sales.Where(c => c.SmId == user.RelatedId).OrderBy(c => c.Name);
                    break;
                case "BM":
                    BranchManager bm = _db.BranchManagers.Where(c => c.BmId == user.RelatedId).First();
                    ViewData["Sales"] = _db.Sales.Where(c => c.BranchCode == bm.BranchCode).OrderBy(c => c.Name);
                    break;
                case "ABM":
                    ABM abm = _db.ABMs.Where(c => c.AbmId == user.RelatedId).First();
                    ViewData["Branchs"] = _db.Branchs.Where(c => c.AreaCode == abm.AreaCode).OrderBy(c => c.Name);
                    ViewData["Sales"] = _db.Sales.Where(c => false);
                    break;
                case "RBH":
                    RBH rbh = _db.RBHs.Where(c => c.RbhId == user.RelatedId).First();
                    ViewData["Branchs"] = _db.Branchs.Where(c => c.Area.RegionCode == rbh.RegionCode).OrderBy(c => c.Name);
                    ViewData["Sales"] = _db.Sales.Where(c => false);
                    break;
            }

            return View();
        }

        public ActionResult list(int page = 1, string key = "", int perpage = 20, string status = "", string orderBy = "", string orderMode = "", int selectedSales = 0)
        {
            double cur_page = page;
            page -= 1;
            int per_page = perpage;
            bool first_btn = true;
            bool last_btn = true;
            bool previous_btn = true;
            bool next_btn = true;
            int start = page * per_page;
            string msg = "";

            var data = (from c in _db.NasabahProducts 
                        select c).Where(c => c.Status == "WARM" || c.Status == "HOT");
            int TeamId = 0;

            if (CommonModel.UserRole() == "SALES")
            {
                User user = CommonModel.GetCurrentUser();
                Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();
                TeamId = sales.TeamId;

                data = from c in _db.SalesNasabahs
                       where c.SalesId == user.RelatedId
                       join d in _db.NasabahProducts on c.NasabahId equals d.NasabahId
                        where (d.Status == "WARM" || d.Status == "HOT") && d.SalesId == sales.SalesId
                       select d;
            }
            else if (CommonModel.UserRole() == "SM")
            {
                if (selectedSales>0)
                {
                    data = from c in _db.SalesNasabahs
                           where c.SalesId == selectedSales
                           join d in _db.NasabahProducts on c.NasabahId equals d.NasabahId
                           where (d.Status == "WARM" || d.Status == "HOT") && d.SalesId == selectedSales
                           select d;
                }
                else
                {
                    var checkSales = _db.Sales.Where(c => c.SmId == user.RelatedId);
                    if (checkSales.Count() > 0)
                    {
                        List<int> arraySales = new List<int>();
                        foreach (Sale item in checkSales)
                        {
                            arraySales.Add(item.SalesId);
                        }

                        data = from c in _db.SalesNasabahs
                               where arraySales.Contains(c.SalesId)
                               join d in _db.NasabahProducts on c.NasabahId equals d.NasabahId
                               where (d.Status == "WARM" || d.Status == "HOT")
                               select d;
                    }
                    else
                    {
                        data = (from c in _db.NasabahProducts select c).Where(c => false);
                    }
                }
            }
            else if (CommonModel.UserRole() == "BM")
            {
                if (selectedSales > 0)
                {
                    data = from c in _db.SalesNasabahs
                           where c.SalesId == selectedSales
                           join d in _db.NasabahProducts on c.NasabahId equals d.NasabahId
                           where (d.Status == "WARM" || d.Status == "HOT") && d.SalesId == selectedSales
                           select d;
                }
                else
                {
                    BranchManager bm = _db.BranchManagers.Where(c => c.BmId == user.RelatedId).First();
                    var checkSales = _db.Sales.Where(c => c.BranchCode == bm.BranchCode);
                    if (checkSales.Count() > 0)
                    {
                        List<int> arraySales = new List<int>();
                        foreach (Sale item in checkSales)
                        {
                            arraySales.Add(item.SalesId);
                        }

                        data = from c in _db.SalesNasabahs
                               where arraySales.Contains(c.SalesId)
                               join d in _db.NasabahProducts on c.NasabahId equals d.NasabahId
                               where (d.Status == "WARM" || d.Status == "HOT")
                               select d;
                    }
                    else
                    {
                        data = (from c in _db.NasabahProducts select c).Where(c => false);
                    }
                }
            }
            else if (CommonModel.UserRole() == "ABM")
            {
                if (selectedSales > 0)
                {
                    data = from c in _db.SalesNasabahs
                           where c.SalesId == selectedSales
                           join d in _db.NasabahProducts on c.NasabahId equals d.NasabahId
                           where (d.Status == "WARM" || d.Status == "HOT") && d.SalesId == selectedSales
                           select d;
                }
                else
                {
                    ABM abm = _db.ABMs.Where(c => c.AbmId == user.RelatedId).First();

                    var checkBranch = _db.Branchs.Where(c => c.AreaCode == abm.AreaCode);
                    List<string> arrayBranch = new List<string>();
                    foreach (Branch item in checkBranch)
                    {
                        arrayBranch.Add(item.BranchCode);
                    }

                    var checkSales = _db.Sales.Where(c => arrayBranch.Contains(c.BranchCode));
                    if (checkSales.Count() > 0)
                    {
                        List<int> arraySales = new List<int>();
                        foreach (Sale item in checkSales)
                        {
                            arraySales.Add(item.SalesId);
                        }

                        data = from c in _db.SalesNasabahs
                               where arraySales.Contains(c.SalesId)
                               join d in _db.NasabahProducts on c.NasabahId equals d.NasabahId
                               where (d.Status == "WARM" || d.Status == "HOT")
                               select d;
                    }
                    else
                    {
                        data = (from c in _db.NasabahProducts select c).Where(c => false);
                    }
                }
            }
            else if (CommonModel.UserRole() == "RBH")
            {
                if (selectedSales > 0)
                {
                    data = from c in _db.SalesNasabahs
                           where c.SalesId == selectedSales
                           join d in _db.NasabahProducts on c.NasabahId equals d.NasabahId
                           where (d.Status == "WARM" || d.Status == "HOT") && d.SalesId == selectedSales
                           select d;
                }
                else
                {
                    RBH rbh = _db.RBHs.Where(c => c.RbhId == user.RelatedId).First();

                    var checkArea = _db.Areas.Where(c => c.RegionCode == rbh.RegionCode);
                    List<string> arrayArea = new List<string>();
                    foreach (Area item in checkArea)
                    {
                        arrayArea.Add(item.AreaCode);
                    }

                    var checkBranch = _db.Branchs.Where(c => arrayArea.Contains(c.AreaCode));
                    List<string> arrayBranch = new List<string>();
                    foreach (Branch item in checkBranch)
                    {
                        arrayBranch.Add(item.BranchCode);
                    }

                    var checkSales = _db.Sales.Where(c => arrayBranch.Contains(c.BranchCode));
                    if (checkSales.Count() > 0)
                    {
                        List<int> arraySales = new List<int>();
                        foreach (Sale item in checkSales)
                        {
                            arraySales.Add(item.SalesId);
                        }

                        data = from c in _db.SalesNasabahs
                               where arraySales.Contains(c.SalesId)
                               join d in _db.NasabahProducts on c.NasabahId equals d.NasabahId
                               where (d.Status == "WARM" || d.Status == "HOT")
                               select d;
                    }
                    else
                    {
                        data = (from c in _db.NasabahProducts select c).Where(c => false);
                    }
                }
            }

            if (!String.IsNullOrEmpty(key))
            {
                data = data.Where(c => c.Nasabah.GCIF.Contains(key) || c.Nasabah.Name.Contains(key));
            }
            if (status!="")
            {
                data = data.Where(c => c.Status == status);
            }

            int count = data.Count();

            switch (orderBy)
            {
                case "gcif": if (orderMode == "asc") data = data.OrderBy(c => c.Nasabah.GCIF); else data = data.OrderByDescending(c => c.Nasabah.GCIF); break;
                case "name": if (orderMode == "asc") data = data.OrderBy(c => c.Nasabah.Name); else data = data.OrderByDescending(c => c.Nasabah.Name); break;
                default: if (orderMode == "asc") data = data.OrderBy(c => c.Status); else data = data.OrderBy(c => c.Status); break;
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

            var checkProduk = _db.Products.OrderBy(c => c.ProductId);
            int jml = checkProduk.Count();

            msg += "<table class=\"grid-cust\" width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"6\"><tr>" +
                    "<th scope=\"col\">ID</th><th scope=\"col\">GCIF</th><th scope=\"col\">NAMA</th>" +
                    "<th scope=\"col\">TELPON</th><th scope=\"col\">NO. HP</th><th scope=\"col\">PRODUCT</th>" +
                    "<th scope=\"col\">AMOUNT</th><th scope=\"col\">SALES</th>" +
                    "<th scope=\"col\">CALL</th><th scope=\"col\">VISIT</th><th scope=\"col\">STATUS</th><th scope=\"col\">TANGGAL</th>";

            if (CommonModel.UserRole() == "ADMIN")
            {
                msg += "<th scope=\"col\">OPTION</th>";
            }
            else if (CommonModel.UserRole() == "SALES")
            {
                msg += "<th scope=\"col\">ACTIVITY</th>";
            }

            msg += "</tr><tbody>";
            
            i = dataStart;

            foreach (NasabahProduct item in result)
            {
                string GCIF = (item.Nasabah != null) ? item.Nasabah.GCIF : "";
                string NasabahId = (item.Nasabah != null) ? item.Nasabah.NasabahId.ToString() : "";
                string NasabahName = (item.Nasabah != null) ? item.Nasabah.Name : "";
                string HomePhone = (item.Nasabah != null) ? item.Nasabah.HomePhone : "";
                string MobilePhone = (item.Nasabah != null) ? item.Nasabah.MobilePhone : "";
                string ProductName = (item.Product != null) ? item.Product.Name : "";
                string SalesName = (item.Sale != null) ? item.Sale.Name : "";
                string Amount = (item.Amount != null) ? "Rp. " + item.Amount.GetValueOrDefault(0).ToString("F2") : "";

                string klas = (i % 2 == 0) ? "gridrow_alternate" : "gridrow";
                int callCount = _db.Calls.Where(c => c.NasabahId == item.NasabahId && c.ProductId == item.ProductId && c.SalesId == item.SalesId).Count();
                int visitCount = _db.Visits.Where(c => c.NasabahId == item.NasabahId && c.ProductId == item.ProductId && c.SalesId == item.SalesId).Count();
                string lastUpdate = item.LastUpdate.ToString("dd MMM yyyy");

                msg += "<tr class=\"" + klas + "\">" +
                        "<td align=\"center\">" + i + "</td>" +
                       "<td align=\"center\">" + GCIF + "</td>" +
                       "<td><a href=\"" + Url.Content("~/Customer/Detail/" + NasabahId) + "\" class=\"ajax\">" + NasabahName + "</a></td>" +
                       "<td>" + HomePhone + "</td>" +
                       "<td>" + MobilePhone + "</td>" +
                       "<td>" + ProductName + "</td>" +
                       "<td align=\"right\">" + Amount + "</td>" +
                       "<td>" + SalesName + "</td>" +
                       "<td align=\"center\">" + callCount + "</td>" +
                       "<td align=\"center\">" + visitCount + "</td>" +
                       "<td align=\"center\">" + item.Status + "</td>" +
                       "<td align=\"center\">" + lastUpdate + "</td>";

                if (CommonModel.UserRole() == "ADMIN" || CommonModel.UserRole() == "SALES")
                {
                    msg += "<td align=\"center\">";

                    switch (CommonModel.UserRole())
                    {
                        case "ADMIN":
                            msg += "<a href=\"" + Url.Content("~/Pipeline/Edit/" + item.Id) + "\" title=\"Edit\"><img src=\"" + Url.Content("~/Content/images/icon-edit.gif") + "\" border=\"0\"/></a>" +
                                    " <a href=\"" + Url.Content("~/Pipeline/Delete/" + item.Id) + "\" title=\"Delete\"><img src=\"" + Url.Content("~/Content/images/icon-delete.gif") + "\" border=\"0\"/></a>";
                            break;
                        case "SALES":
                            msg += " <a href=\"" + Url.Content("~/Pipeline/Edit/" + item.Id) + "\" title=\"Edit\" class=\"edit-btn\">Edit</a>";
                            //msg += " <a href=\"" + Url.Content("~/SalesActivity/Reference/" + item.NasabahId) + "?pId="+item.ProductId+"\" title=\"Reference\" class=\"ref-btn\">ReferTo</a>";
                            break;
                    }

                    msg += "</td></tr>";
                }
                else
                {
                    msg += "</tr>";
                }

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

        public ActionResult Edit(int id)
        {
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }            

            var dataToEdit = (from m in _db.NasabahProducts
                              where m.Id == id
                              select m).First();

            ViewData["Status"] = PipelineOption;

            if (CommonModel.UserRole() == "SALES")
            {
                Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();
                if (CommonModel.UserRole() == "SALES" && dataToEdit.SalesId != sales.SalesId) return RedirectToAction("Index", "Home");
            }

            return View(dataToEdit);
        }

        //
        // POST: /Video/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(NasabahProduct dataToEdit)
        {
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }
            
            ViewBag.Status = PipelineOption;

            try
            {         
                var originalData = (from m in _db.NasabahProducts
                                    where m.Id == dataToEdit.Id
                                    select m).First();

                if (CommonModel.UserRole() == "SALES")
                {
                    Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();
                    if (CommonModel.UserRole() == "SALES" && originalData.SalesId != sales.SalesId) return RedirectToAction("Index", "Home");
                }

                dataToEdit.SalesId = originalData.SalesId;
                dataToEdit.NasabahId = originalData.NasabahId;
                dataToEdit.ProductId = originalData.ProductId;
                dataToEdit.LastUpdate = DateTime.Now;

                if (!ModelState.IsValid)
                    return View(dataToEdit);

                _db.ApplyCurrentValues(originalData.EntityKey.EntitySetName, dataToEdit);
                _db.SaveChanges();

                CommonModel.UpdateProductStatus(dataToEdit.NasabahId);

                ViewData["Output"] = "Pipeline successfully saved";
                return View("Index");
            }
            catch (Exception e)
            {
                ViewData["Output"] = e.Message;
                return View(dataToEdit);
            }
        }

        public ActionResult Delete(int id)
        {
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.UserRole() != "ADMIN") return RedirectToAction("Index", "Home");

            var dataToDelete = (from m in _db.NasabahProducts
                                where m.Id == id
                                select m).First();

            return View(dataToDelete);
        }


        [HttpPost]
        public ActionResult Delete(int id, NasabahProduct dataToDelete)
        {
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.UserRole() != "ADMIN") return RedirectToAction("Index", "Home");

            var originalData = (from m in _db.NasabahProducts
                                where m.Id == id
                                select m).First();
            try
            {

                if (!ModelState.IsValid)
                    return View(originalData);

                _db.DeleteObject(originalData);
                _db.SaveChanges();

                ViewData["Output"] = "Pipeline successfully deleted";
                return View("Index");
            }
            catch (Exception e)
            {
                ViewData["Output"] = e.Message;
                return View(originalData);
            }
        }

    }
}

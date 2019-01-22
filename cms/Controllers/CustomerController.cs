using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cms.Models;
using System.IO;
using MvcContrib.UI.Grid;
using MvcContrib.Sorting;
using System.Data;
using System.Data.OleDb;
using System.Web.Script.Serialization;

namespace cms.Controllers
{
    [Authorize]
    public class CustomerController : CommonController
    {

        public ActionResult Index()
        {
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }

            if (CommonModel.UserRole() == "ADMIN" && Request.HttpMethod == "POST")
            {
                string npk = Request["npk"];
                var checkNPK = _db.Sales.Where(c => c.Npk == npk && c.TeamId != 4);
                if (checkNPK.Count() > 0)
                {
                    Sale sales = checkNPK.First();

                    int i = 0;
                    foreach (string s in Request.Params.Keys)
                    {
                        string substr = s.Substring(0, 2);
                        string str = Request[s];
                        if (substr == "cb" && !String.IsNullOrWhiteSpace(str))
                        {
                            i++;
                            int nId = Convert.ToInt32(str);
                            var checkSN = _db.SalesNasabahs.Where(c => c.SalesId == sales.SalesId && c.NasabahId == nId);
                            if (checkSN.Count() == 0)
                            {
                                SalesNasabah sn = new SalesNasabah();
                                sn.SalesId = sales.SalesId;
                                sn.NasabahId = nId;
                                sn.CreatedDate = DateTime.Now;
                                _db.AddToSalesNasabahs(sn);
                            }
                        }
                    }

                    if (i > 0)
                    {
                        ViewData["Success"] = "Successfully assign selected Customer to Sales NPK " + npk;
                    }
                    else
                    {
                        ViewData["Output"] = "Please choose Customer to assign it to Sales";
                    }
                }
                else
                {
                    ViewData["Output"] = "Cannot find NPK " + npk;
                }

                _db.SaveChanges();
            }

            return View();
        }

        public ActionResult list(int page = 1, string key = "", int perpage = 20, string status = "", string orderBy = "", string orderMode = "", int SelectedSalesId = 0)
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

            //var data = (from c in _db.Nasabahs select c).Where(c => 1 == 1);
            var data = _db.SalesNasabahs.Where(c => true);
            int TeamId = 0;

            if (CommonModel.UserRole() == "SALES")
            {
                Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();
                TeamId = sales.TeamId;

                /*data = from c in _db.SalesNasabahs
                       where c.SalesId == user.RelatedId
                       group c by c.NasabahId into grup
                       join d in _db.Nasabahs on grup.FirstOrDefault().NasabahId equals d.NasabahId                       
                       select d;*/
                data = from c in _db.SalesNasabahs
                       where c.SalesId == user.RelatedId && !(from o in _db.SalesNasabahs
                                                              where o.RefferalFrom == user.RelatedId
                                                              select o.NasabahId).Contains(c.NasabahId)
                       && !(from x in _db.NasabahProducts where x.NasabahId == c.NasabahId select x.SalesId).Contains(0)
                       select c; // Added by Aditia 2016.10.05 -> cust yang sudah direfer akan langsung hilang
                // Added by Aditia 2016.10.13 -> nasabah LEADs yg expired NP -> salesid=0

                //data = data.Where(c => c.SalesId == user.RelatedId);
            }
            else if (CommonModel.UserRole() == "SM")
            {
                if (SelectedSalesId > 0)
                {
                    /*data = from c in _db.SalesNasabahs
                           where c.SalesId == SelectedSalesId
                           group c by c.NasabahId into grup
                           join d in _db.Nasabahs on grup.FirstOrDefault().NasabahId equals d.NasabahId
                           select d;*/
                    //data = _db.SalesNasabahs.Where(c => c.SalesId == SelectedSalesId);
                    data = from c in _db.SalesNasabahs
                           where c.SalesId == SelectedSalesId && !(from o in _db.SalesNasabahs
                                                                   where o.RefferalFrom == SelectedSalesId
                                                                   select o.NasabahId).Contains(c.NasabahId)
                            && !(from x in _db.NasabahProducts where x.NasabahId == c.NasabahId select x.SalesId).Contains(0)
                           select c;

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

                        /*data = from c in _db.SalesNasabahs
                               where arraySales.Contains(c.SalesId)
                               group c by c.NasabahId into grup
                               join d in _db.Nasabahs on grup.FirstOrDefault().NasabahId equals d.NasabahId
                               select d;*/
                        data = _db.SalesNasabahs.Where(c => arraySales.Contains(c.SalesId));
                    }
                    else
                    {
                        data = _db.SalesNasabahs.Where(c => false);
                    }
                }
            }
            else if (CommonModel.UserRole() == "BM")
            {
                if (SelectedSalesId > 0)
                {
                    /*data = from c in _db.SalesNasabahs
                           where c.SalesId == SelectedSalesId
                           group c by c.NasabahId into grup
                           join d in _db.Nasabahs on grup.FirstOrDefault().NasabahId equals d.NasabahId
                           select d;*/
                    //data = _db.SalesNasabahs.Where(c => c.SalesId == SelectedSalesId);
                    data = from c in _db.SalesNasabahs
                           where c.SalesId == SelectedSalesId && !(from o in _db.SalesNasabahs
                                                                   where o.RefferalFrom == SelectedSalesId
                                                                   select o.NasabahId).Contains(c.NasabahId)
                            && !(from x in _db.NasabahProducts where x.NasabahId == c.NasabahId select x.SalesId).Contains(0)
                           select c;

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

                        /*data = from c in _db.SalesNasabahs
                               where arraySales.Contains(c.SalesId)
                               group c by c.NasabahId into grup
                               join d in _db.Nasabahs on grup.FirstOrDefault().NasabahId equals d.NasabahId
                               select d;*/
                        data = _db.SalesNasabahs.Where(c => arraySales.Contains(c.SalesId));
                    }
                    else
                    {
                        data = _db.SalesNasabahs.Where(c => false);
                    }
                }
            }
            else if (CommonModel.UserRole() == "ABM")
            {
                if (SelectedSalesId > 0)
                {
                    /*data = from c in _db.SalesNasabahs
                           where c.SalesId == SelectedSalesId
                           group c by c.NasabahId into grup
                           join d in _db.Nasabahs on grup.FirstOrDefault().NasabahId equals d.NasabahId
                           select d;*/
                    //data = _db.SalesNasabahs.Where(c => c.SalesId == SelectedSalesId);
                    data = from c in _db.SalesNasabahs
                           where c.SalesId == SelectedSalesId && !(from o in _db.SalesNasabahs
                                                                   where o.RefferalFrom == SelectedSalesId
                                                                   select o.NasabahId).Contains(c.NasabahId)
                            && !(from x in _db.NasabahProducts where x.NasabahId == c.NasabahId select x.SalesId).Contains(0)
                           select c;

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

                        /*data = from c in _db.SalesNasabahs
                               where arraySales.Contains(c.SalesId)
                               group c by c.NasabahId into grup
                               join d in _db.Nasabahs on grup.FirstOrDefault().NasabahId equals d.NasabahId
                               select d;*/
                        data = _db.SalesNasabahs.Where(c => arraySales.Contains(c.SalesId));
                    }
                    else
                    {
                        //data = (from c in _db.Nasabahs select c).Where(c => false);
                        data = _db.SalesNasabahs.Where(c => false);
                    }
                }
            }
            else if (CommonModel.UserRole() == "RBH")
            {
                if (SelectedSalesId > 0)
                {
                    /*data = from c in _db.SalesNasabahs
                           where c.SalesId == SelectedSalesId
                           group c by c.NasabahId into grup
                           join d in _db.Nasabahs on grup.FirstOrDefault().NasabahId equals d.NasabahId
                           select d;*/
                    //data = _db.SalesNasabahs.Where(c => c.SalesId == SelectedSalesId);
                    data = from c in _db.SalesNasabahs
                           where c.SalesId == SelectedSalesId && !(from o in _db.SalesNasabahs
                                                                   where o.RefferalFrom == SelectedSalesId
                                                                   select o.NasabahId).Contains(c.NasabahId)
                            && !(from x in _db.NasabahProducts where x.NasabahId == c.NasabahId select x.SalesId).Contains(0)
                           select c;
                    data = data.GroupBy(c => c.NasabahId).Select(g => g.FirstOrDefault());
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

                        /*data = from c in _db.SalesNasabahs
                               where arraySales.Contains(c.SalesId)
                               group c by c.NasabahId into grup
                               join d in _db.Nasabahs on grup.FirstOrDefault().NasabahId equals d.NasabahId
                               select d;*/
                        data = _db.SalesNasabahs.Where(c => arraySales.Contains(c.SalesId));
                    }
                    else
                    {
                        //data = (from c in _db.Nasabahs select c).Where(c => false);
                        data = _db.SalesNasabahs.Where(c => false);
                    }
                }
            }

            if (!String.IsNullOrEmpty(key))
            {
                data = data.Where(c => c.Nasabah.GCIF.Contains(key) || c.Nasabah.Name.Contains(key));
            }
            if (status != "")
            {
                data = data.Where(c => c.Nasabah.Status == status);
            }

            data = data.GroupBy(c => c.NasabahId).Select(g => g.FirstOrDefault());

            int count = data.Count();

            switch (orderBy)
            {
                case "gcif": if (orderMode == "asc") data = data.OrderBy(c => c.Nasabah.GCIF); else data = data.OrderByDescending(c => c.Nasabah.GCIF); break;
                case "name": if (orderMode == "asc") data = data.OrderBy(c => c.Nasabah.Name); else data = data.OrderByDescending(c => c.Nasabah.Name); break;
                case "status": if (orderMode == "asc") data = data.OrderBy(c => c.Nasabah.Status); else data = data.OrderBy(c => c.Nasabah.Status); break;
                case "lastupdate": if (orderMode == "asc") data = data.OrderBy(c => c.LastUpdate); else data = data.OrderBy(c => c.LastUpdate); break;
                default: if (orderMode == "asc") data = data.OrderBy(c => c.Nasabah.CreatedDate); else data = data.OrderByDescending(c => c.Nasabah.CreatedDate); break;
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

            msg += "<table class=\"grid-cust\" width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"6\">";

            if (CommonModel.UserRole() == "ADMIN")
                msg += "<tr><th rowspan=\"2\" scope=\"col\"><input type=\"checkbox\" id=\"checkall\" /></th>";

            msg += "<th rowspan=\"2\" scope=\"col\">ID</th><th rowspan=\"2\" scope=\"col\">GCIF</th><th rowspan=\"2\" scope=\"col\">NAMA</th>" +
                        "<th rowspan=\"2\" scope=\"col\">TELPON</th><th rowspan=\"2\" scope=\"col\">NO. HP</th><th rowspan=\"2\" scope=\"col\">STATUS</th>";
            //"<th colspan=\""+jml+"\" scope=\"col\">PRODUCT</th>";

            if (CommonModel.UserRole() == "ADMIN")
            {
                msg += "<th rowspan=\"2\" scope=\"col\">OPTION</th>";
            }
            else if (CommonModel.UserRole() == "SALES")
            {
                msg += "<th rowspan=\"2\" scope=\"col\">ACTIVITY</th>";
            }

            msg += "</tr><tr>";

            /*string strColumn = String.Empty;
            if (checkProduk.Count() > 0)
            {
                foreach (Product pr in checkProduk)
                {
                    msg += "<th>" + pr.Code + "</th>";
                    strColumn += "<td align=\"center\"><span style=\"display:none\">" + pr.ProductId + "</span></td>";
                }
            }*/

            msg += "</tr><tbody>";

            i = dataStart;

            foreach (SalesNasabah sn in result)
            {
                var item = sn.Nasabah;
                string klas = (i % 2 == 0) ? "gridrow_alternate" : "gridrow";

                msg += "<tr class=\"" + klas + "\">";
                if (CommonModel.UserRole() == "ADMIN")
                    msg += "<td align=\"center\"><input type=\"checkbox\" class=\"cbox\" name=\"cbox_" + item.NasabahId + "\" value=\"" + item.NasabahId + "\" /></td>";

                msg += "<td align=\"center\">" + i + "</td>" +
                       "<td align=\"center\">" + item.GCIF + "</td>" +
                       "<td><a href=\"" + Url.Content("~/Customer/Detail/" + item.NasabahId) + "\" class=\"ajax\">" + item.Name + "</a></td>" +
                       "<td>" + item.HomePhone + "</td>" +
                       "<td>" + item.MobilePhone + "</td>" +
                       "<td align=\"center\">" + item.Status + "</td>";
                /*
                if (!String.IsNullOrWhiteSpace(item.ProductStatus))
                {
                    string[] arrayPStatus = item.ProductStatus.Split(',');
                    string NewStrColumn = strColumn;
                    
                    foreach (string strPStatus in arrayPStatus)
                    {
                        if (!String.IsNullOrWhiteSpace(strPStatus))
                        {
                            string[] arr = strPStatus.Split(':');
                            string pId = arr[0];
                            string sId = arr[1];
                            string imageId = arr[2];
                            string imageDesc = arr[3];
                            string strIcon = String.Empty;
                            bool changeIcon = true;

                            if (SelectedSalesId > 0)
                            {
                                if (SelectedSalesId.ToString() != sId) changeIcon = false;
                            }

                            if (changeIcon)
                            {
                                switch (imageId)
                                {
                                    case "1": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_existing.png") + "\" title=\"" + imageDesc + "\" />";
                                        break;
                                    case "2": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_inisiatif_warm.png") + "\" title=\"" + imageDesc + "\" />";
                                        break;
                                    case "3": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_inisiatif_hot.png") + "\" title=\"" + imageDesc + "\" />";
                                        break;
                                    case "4": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_inisiatif_booking.png") + "\" title=\"" + imageDesc + "\" />";
                                        break;
                                    case "5": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_inisiatif_cancel.png") + "\" title=\"" + imageDesc + "\" />";
                                        break;
                                    case "6": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_rekomendasi_new.png") + "\" title=\"" + imageDesc + "\" />";
                                        break;
                                    case "7": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_rekomendasi_warm.png") + "\" title=\"" + imageDesc + "\" />";
                                        break;
                                    case "8": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_rekomendasi_hot.png") + "\" title=\"" + imageDesc + "\" />";
                                        break;
                                    case "9": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_rekomendasi_booking.png") + "\" title=\"" + imageDesc + "\" />";
                                        break;
                                    case "10": strIcon = "<img src=\"" + Url.Content("~/Content/images/icon_rekomendasi_cancel.png") + "\" title=\"" + imageDesc + "\" />";
                                        break;
                                }

                                NewStrColumn = NewStrColumn.Replace("<span style=\"display:none\">" + pId + "</span>", strIcon);
                                
                            }

                        }
                    }

                    msg += NewStrColumn;

                } else msg += strColumn;
                */
                if (CommonModel.UserRole() == "ADMIN" || CommonModel.UserRole() == "SALES")
                {

                    msg += (CommonModel.UserRole() == "SALES") ? "<td align=\"center\" style=\"width:230px\">" : "<td align=\"center\">";

                    switch (CommonModel.UserRole())
                    {
                        case "ADMIN":
                            msg += "<a href=\"" + Url.Content("~/Customer/Edit/" + item.NasabahId) + "\" title=\"Edit\"><img src=\"" + Url.Content("~/Content/images/icon-edit.gif") + "\" border=\"0\"/></a>" +
                                    " <a href=\"" + Url.Content("~/Customer/Delete/" + item.NasabahId) + "\" title=\"Delete\"><img src=\"" + Url.Content("~/Content/images/icon-delete.gif") + "\" border=\"0\"/></a>";
                            break;
                        case "SALES":
                            msg += "<a href=\"" + Url.Content("~/Customer/Edit/" + item.NasabahId) + "\" title=\"Edit\" class=\"edit-btn\">Edit</a>";
                            if (TeamId != 4)
                            {
                                msg += " &nbsp;<a href=\"" + Url.Content("~/SalesActivity/Call/" + item.NasabahId) + "\" title=\"Call\" class=\"call-btn\">Call</a>" +
                                        " &nbsp;<a href=\"" + Url.Content("~/SalesActivity/Visit/" + item.NasabahId) + "\" title=\"Visit\" class=\"visit-btn\">Visit</a>";
                            }
                            msg += "  &nbsp;<a href=\"" + Url.Content("~/SalesActivity/Reference/" + item.NasabahId) + "\" title=\"Reference\" class=\"ref-btn\">Refer</a>";
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

            if (CommonModel.UserRole() == "ADMIN")
                msg += "<div class=\"clear\"></div>Assign selected Customer to Sales NPK <input type=\"text\" name=\"npk\" style=\"width:150px\" /> &nbsp;<input type=\"submit\" class=\"button\" name=\"assign\" value=\" Assign \" /><br />";

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
        // GET: /Video/Create

        public ActionResult Create()
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

            ViewData["Gender"] = "m";
            ViewData["Status"] = "NEW";
            return View();
        }

        //
        // POST: /Video/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Exclude = "NasabahId")] Nasabah dataToCreate)
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

            string strSalesId = Request["SalesId"];

            try
            {

                dataToCreate.CreatedDate = DateTime.Now;
                ViewData["Gender"] = dataToCreate.Gender;
                ViewData["Status"] = dataToCreate.Status;
                //dataToCreate.Status = (CommonModel.UserRole() == "ADMIN") ? "EXISTING" : "NEW";

                if (!ModelState.IsValid) return View(dataToCreate);

                _db.AddToNasabahs(dataToCreate);
                _db.SaveChanges();

                if (CommonModel.UserRole() == "SALES")
                {
                    SalesNasabah sn = new SalesNasabah();
                    sn.SalesId = user.RelatedId.GetValueOrDefault();
                    sn.NasabahId = dataToCreate.NasabahId;
                    sn.CreatedDate = DateTime.Now;
                    sn.LastUpdate = DateTime.Now;
                    _db.AddToSalesNasabahs(sn);
                    _db.SaveChanges();
                }

                if (!String.IsNullOrWhiteSpace(strSalesId))
                {
                    SalesNasabah sn = new SalesNasabah();
                    sn.SalesId = Convert.ToInt32(strSalesId);
                    sn.NasabahId = dataToCreate.NasabahId;
                    sn.CreatedDate = DateTime.Now;
                    sn.LastUpdate = DateTime.Now;
                    _db.AddToSalesNasabahs(sn);
                    _db.SaveChanges();
                }
                //-- add by rdf

                CustomerLogModel DataBaru = LogsModel.MappingCustomerFromEFtoModel(dataToCreate);
                string message = LogsModel.SaveLogCustomer(new CustomerLogModel(), DataBaru, "Add");

                if (!string.IsNullOrEmpty(message))
                {
                    ViewData["Output"] = "Your Save data success but create log error with : " + message;

                    return View();
                }

                //-- end add by rdf
                TempData["Success"] = "Data customer berhasil disimpan";

                if (CommonModel.UserRole() != "SALES" && CommonModel.UserRole() != "ADMIN")
                {
                    return RedirectToAction("Review");
                }



                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData["Output"] = e.Message;
                return View();
            }
        }

        //
        // GET: /Video/Edit/5

        public ActionResult Edit(int id)
        {
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.UserRole() != "ADMIN" && CommonModel.UserRole() != "SALES") return RedirectToAction("Index", "Home");

            var dataToEdit = (from m in _db.Nasabahs
                              where m.NasabahId == id
                              select m).First();

            ViewData["Gender"] = (dataToEdit.Gender == "f") ? "f" : "m";
            ViewData["Status"] = dataToEdit.Status;
            return View(dataToEdit);
        }

        //
        // POST: /Video/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Nasabah dataToEdit)
        {
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.UserRole() != "ADMIN" && CommonModel.UserRole() != "SALES") return RedirectToAction("Index", "Home");

            try
            {
                var originalData = (from m in _db.Nasabahs
                                    where m.NasabahId == dataToEdit.NasabahId
                                    select m).First();

                CustomerLogModel DataLama = LogsModel.MappingCustomerFromEFtoModel(originalData);

                dataToEdit.CreatedDate = originalData.CreatedDate;
                //dataToEdit.Status = originalData.Status;

                ViewData["Gender"] = (dataToEdit.Gender == "f") ? "f" : "m";
                ViewData["Status"] = dataToEdit.Status;

                if (!ModelState.IsValid)
                    return View(dataToEdit);

                _db.ApplyCurrentValues(originalData.EntityKey.EntitySetName, dataToEdit);
                _db.SaveChanges();

                //-- add by rdf

                CustomerLogModel DataBaru = LogsModel.MappingCustomerFromEFtoModel(dataToEdit);
                DataBaru.SalesChange = DataLama.Sales;
                string message = LogsModel.SaveLogCustomer(DataLama, DataBaru, "Edit");

                if (!string.IsNullOrEmpty(message))
                {
                    ViewData["Output"] = "Your Save data success but create log error with : " + message;
                    return View();
                }

                //-- end add by rdf


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
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.UserRole() != "ADMIN") return RedirectToAction("Index", "Home");

            var dataToDelete = (from m in _db.Nasabahs
                                where m.NasabahId == id
                                select m).First();

            return View(dataToDelete);
        }


        [HttpPost]
        public ActionResult Delete(int id, Nasabah dataToDelete)
        {
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.UserRole() != "ADMIN") return RedirectToAction("Index", "Home");

            var originalData = (from m in _db.Nasabahs
                                where m.NasabahId == id
                                select m).First();
            CustomerLogModel Datalama = LogsModel.MappingCustomerFromEFtoModel(originalData);

            try
            {

                if (!ModelState.IsValid)
                    return View(originalData);

                var checkNP = _db.NasabahProducts.Where(c => c.NasabahId == id);
                if (checkNP.Count() > 0)
                {
                    foreach (NasabahProduct np in checkNP)
                    {
                        _db.DeleteObject(np);
                    }
                }

                var checkSN = _db.SalesNasabahs.Where(c => c.NasabahId == id);
                if (checkSN.Count() > 0)
                {
                    foreach (SalesNasabah sn in checkSN)
                    {
                        _db.DeleteObject(sn);
                    }
                }

                var checkCall = _db.Calls.Where(c => c.NasabahId == id);
                if (checkCall.Count() > 0)
                {
                    foreach (Call sn in checkCall)
                    {
                        _db.DeleteObject(sn);
                    }
                }

                var checkVisit = _db.Visits.Where(c => c.NasabahId == id);
                if (checkVisit.Count() > 0)
                {
                    foreach (Visit sn in checkVisit)
                    {
                        _db.DeleteObject(sn);
                    }
                }

                string message = LogsModel.SaveLogCustomer(Datalama, new CustomerLogModel(), "Delete");

                if (!string.IsNullOrEmpty(message))
                {
                    ViewData["Output"] = "Your Save data success but create log error with : " + message;
                    return View();
                }

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


        public ActionResult Review()
        {
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


        public ActionResult Import()
        {
            if (CommonModel.UserRole() != "ADMIN") return RedirectToAction("Index", "Home");

            return View();
        }


        [HttpPost]
        public ActionResult DoImport()
        {
            if (CommonModel.UserRole() != "ADMIN") return RedirectToAction("Index", "Home");

            string serverPath = Server.MapPath("~");
            string excelPath = serverPath + "\\Content\\excel\\";
            HttpPostedFileBase File = Request.Files["excel"];
            ViewBag.Output = "";

            if (File.ContentLength > 0)
            {
                var dirFile = new DirectoryInfo(excelPath);
                CommonModel.CreateDirectory(dirFile);

                string randomName = CommonModel.GenerateRandomString(6);
                string downloadFname = randomName + (Path.GetFileName(File.FileName)).Replace(" ", "");
                string downloadFile = Path.Combine(excelPath, downloadFname);
                File.SaveAs(downloadFile);

                string extension = Path.GetExtension(downloadFile);
                if (extension.ToLower() != ".xls")
                {
                    ViewBag.Output = "Anda hanya diperbolehkan mengupload file excel";
                    return View("result");
                }

                string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + downloadFile + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";

                List<ImportModel> ImportList = new List<ImportModel>();
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    using (OleDbCommand command = new OleDbCommand("SELECT * FROM [Sheet1$]", conn))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ImportModel model = new ImportModel();
                                model.NPK = (reader["NPK"] != null) ? reader["NPK"].ToString() : "";
                                model.GCIF = (reader["GCIF"] != null) ? reader["GCIF"].ToString() : "";
                                model.CIF = (reader["CIF"] != null) ? reader["CIF"].ToString() : "";
                                model.ACCT = (reader["ACCT"] != null) ? reader["ACCT"].ToString() : "";
                                model.PRODUCT = (reader["PRODUCT"] != null) ? reader["PRODUCT"].ToString() : "";
                                model.PRODUCTNAME = (reader["PRODUCT NAME"] != null) ? reader["PRODUCT NAME"].ToString() : "";
                                model.PRODUCTSTATUS = (reader["PRODUCT STATUS"] != null) ? reader["PRODUCT STATUS"].ToString() : "";
                                model.NAMA = (reader["NAMA"] != null) ? reader["NAMA"].ToString() : "";
                                model.TELEPON = (reader["TELEPON RUMAH"] != null) ? reader["TELEPON RUMAH"].ToString() : "";
                                model.HANDPHONE = (reader["HANDPHONE"] != null) ? reader["HANDPHONE"].ToString() : "";
                                model.ALAMAT = (reader["ALAMAT"] != null) ? reader["ALAMAT"].ToString() : "";
                                model.KTP = (reader["ID KTP"] != null) ? reader["ID KTP"].ToString() : "";
                                model.GENDER = (reader["GENDER"] != null) ? reader["GENDER"].ToString() : "";
                                model.GENDER = (model.GENDER.ToLower() == "l") ? "m" : "f";

                                //ViewBag.Output += reader["TGL LAHIR"] + "<br />";

                                string TglLahir = (reader["TGL LAHIR"] != null) ? reader["TGL LAHIR"].ToString() : "";
                                if (TglLahir.Trim() != "")
                                {
                                    string[] strDate1 = TglLahir.Split(' ');
                                    string[] strDate = strDate1[0].Split('/');

                                    int intYear = int.Parse(strDate[2]);
                                    int intDay = int.Parse(strDate[0]);
                                    int intMonth = int.Parse(strDate[1]);
                                    DateTime dtime = new DateTime(intYear, intMonth, intDay);
                                    model.TGLLAHIR = dtime;
                                }

                                ImportList.Add(model);
                            }
                        }
                    }
                }

                //return View("output");

                if (ImportList.Count() == 0)
                {
                    ViewBag.Output = "Tidak ada data customer yang terbaca. Silahkan periksa kembali file yang Anda upload.";
                }
                else
                {
                    int i = 0;
                    foreach (ImportModel im in ImportList)
                    {
                        //Get product id
                        int productID = 0;
                        var cekProduk = _db.Products.Where(c => c.Name == im.PRODUCT || c.Code == im.PRODUCT);
                        if (cekProduk.Count() > 0)
                        {
                            Product prod = cekProduk.First();
                            productID = prod.ProductId;
                        }

                        int variantID = 0;
                        var cekVarian = _db.ProductVariants.Where(c => c.Name == im.PRODUCTNAME);
                        if (cekVarian.Count() > 0)
                        {
                            ProductVariant pv = cekVarian.First();
                            variantID = pv.VariantId;
                        }

                        NasabahImport nImport = new NasabahImport();
                        nImport.NPK = im.NPK;
                        nImport.GCIF = im.GCIF;
                        nImport.CIF = im.CIF;
                        nImport.ACCT = im.ACCT;
                        nImport.PRODUCT = im.PRODUCT;
                        if (productID > 0) nImport.PRODUCTID = productID;
                        nImport.PRODUCTNAME = im.PRODUCTNAME;
                        if (variantID > 0) nImport.VARIANTID = variantID;
                        nImport.PRODUCTSTATUS = im.PRODUCTSTATUS;
                        nImport.NAMA = im.NAMA;
                        nImport.TELEPONRUMAH = im.TELEPON;
                        nImport.HANDPHONE = im.HANDPHONE;
                        nImport.ALAMAT = im.ALAMAT;
                        nImport.IDKTP = im.KTP;
                        nImport.GENDER = im.GENDER;
                        if (im.TGLLAHIR.ToString("yyyy-MM-dd") != "0001-01-01")
                        {
                            nImport.TGLLAHIR = im.TGLLAHIR.Date;
                        }
                        nImport.DateCreated = DateTime.Now;


                        var checkSales = _db.Sales.Where(c => c.Npk == im.NPK);
                        if (checkSales.Count() == 0)
                        {
                            nImport.Note = "- NPK tidak ditemukan,";
                        }

                        if (productID == 0 && im.PRODUCT != "")
                        {
                            nImport.Note = "- Produk tidak ada di database,";
                        }

                        var cekNasabah = _db.Nasabahs.Where(c => c.GCIF == im.GCIF);
                        if (cekNasabah.Count() > 0 && productID > 0)
                        {
                            Nasabah nasabah = cekNasabah.First();
                            var cekNP = _db.NasabahProducts.Where(c => c.NasabahId == nasabah.NasabahId && c.ProductId == productID && c.Status != "CANCEL");
                            if (cekNP.Count() > 0)
                            {
                                nImport.Note += "- Nasabah dg produk tsb sudah terdaftar sebelumnya,";
                            }
                        }

                        if (nImport.Note != null)
                        {
                            nImport.Note = nImport.Note.TrimEnd(',');
                            nImport.Note = nImport.Note.Replace(",", "<br />");
                        }

                        _db.AddToNasabahImports(nImport);
                        _db.SaveChanges();

                        i++;
                    }

                    ViewBag.Output = "success";
                }
            }
            else
            {
                ViewBag.Output = "Anda belum mengupload file excel";
            }

            return View("result");
        }


        public ActionResult ImportResult()
        {
            if (CommonModel.UserRole() != "ADMIN") return RedirectToAction("Index", "Home");

            if (Request.HttpMethod == "POST" && Request["action"] == "import")
            {
                int i = 0;
                foreach (string s in Request.Params.Keys)
                {
                    string substr = s.Substring(0, 2);
                    string str = Request[s];
                    if (substr == "cb" && !String.IsNullOrWhiteSpace(str))
                    {
                        int nId = Convert.ToInt32(str);
                        var checkNI = _db.NasabahImports.Where(c => c.Id == nId);
                        if (checkNI.Count() > 0)
                        {
                            NasabahImport ni = checkNI.First();

                            var checkNasabah = _db.Nasabahs.Where(c => c.GCIF == ni.GCIF);
                            CustomerLogModel Datalama = LogsModel.MappingCustomerFromEFtoModel(checkNasabah.FirstOrDefault());
                            


                            Nasabah nasabah = new Nasabah();
                            if (checkNasabah.Count() > 0)
                            {
                                nasabah = checkNasabah.First();
                            }
                            else
                            {
                                nasabah.Status = "EXISTING";
                                nasabah.CreatedDate = DateTime.Now;
                            }

                            nasabah.GCIF = ni.GCIF;
                            nasabah.Name = ni.NAMA;
                            nasabah.KtpId = ni.IDKTP;
                            nasabah.Gender = ni.GENDER;
                            nasabah.BirthDate = ni.TGLLAHIR;
                            nasabah.Address = ni.ALAMAT;
                            nasabah.HomePhone = ni.TELEPONRUMAH;
                            nasabah.MobilePhone = ni.HANDPHONE;

                            if (checkNasabah.Count() > 0)
                            {
                                //-- add by rdf

                                CustomerLogModel DataBaru = LogsModel.MappingCustomerFromEFtoModel(nasabah);
                                DataBaru.SalesChange = Datalama.Sales;
                                string message = LogsModel.SaveLogCustomer(Datalama, DataBaru, "Edit");

                                if (!string.IsNullOrEmpty(message))
                                {
                                    ViewData["Output"] = "Your Save data success but create log error with : " + message;
                                    return View();
                                }

                                //-- end add by rdf

                                _db.ApplyCurrentValues(nasabah.EntityKey.EntitySetName, nasabah);
                            }
                            else
                            {
                                //-- add by rdf

                                CustomerLogModel DataBaru = LogsModel.MappingCustomerFromEFtoModel(nasabah);
                                string message = LogsModel.SaveLogCustomer(new CustomerLogModel(), DataBaru, "Add");

                                if (!string.IsNullOrEmpty(message))
                                {
                                    ViewData["Output"] = "Your Save data success but create log error with : " + message;
                                }

                                _db.AddToNasabahs(nasabah);
                            }
                            _db.SaveChanges();

                            if (ni.PRODUCTID != null)
                            {
                                int varianID = 0;
                                if (ni.PRODUCTNAME != "")
                                {
                                    var checkVarian = _db.ProductVariants.Where(c => c.Name == ni.PRODUCTNAME);
                                    if (checkVarian.Count() == 0)
                                    {
                                        ProductVariant pv = new ProductVariant();
                                        pv.Name = ni.PRODUCTNAME;
                                        pv.ProductId = ni.PRODUCTID.GetValueOrDefault();

                                        _db.AddToProductVariants(pv);
                                        _db.SaveChanges();
                                        varianID = pv.VariantId;
                                    }
                                    else
                                    {
                                        ProductVariant pv = checkVarian.First();
                                        varianID = pv.VariantId;
                                    }
                                }

                                var checkSales = _db.Sales.Where(c => c.Npk == ni.NPK);
                                Sale sales = new Sale();
                                if (checkSales.Count() > 0)
                                {
                                    sales = checkSales.First();
                                }
                                string PStatus = (ni.PRODUCTSTATUS.Trim() == "EXISTING" || ni.PRODUCTSTATUS.Trim() == "REKOMENDASI") ? ni.PRODUCTSTATUS.Trim() : "REKOMENDASI";

                                NasabahProduct np = new NasabahProduct();

                                np.NasabahId = nasabah.NasabahId;
                                np.ProductId = ni.PRODUCTID.GetValueOrDefault();
                                if (varianID > 0) np.VariantId = varianID;
                                np.CIF = ni.CIF;
                                np.ACCT = ni.ACCT;
                                np.Status = PStatus;
                                np.LastUpdate = DateTime.Now;
                                if (checkSales.Count() > 0) np.SalesId = sales.SalesId;

                                _db.AddToNasabahProducts(np);
                                _db.SaveChanges();

                                if (checkSales.Count() > 0)
                                {
                                    SalesNasabah sn = new SalesNasabah();
                                    sn.SalesId = sales.SalesId;
                                    sn.NasabahId = nasabah.NasabahId;
                                    sn.CreatedDate = DateTime.Now;
                                    sn.LastUpdate = DateTime.Now;
                                    _db.AddToSalesNasabahs(sn);
                                    _db.SaveChanges();
                                }

                                _db.DeleteObject(ni);
                                _db.SaveChanges();

                                i++;
                            }
                        }
                    }

                }

                if (i > 0)
                {
                    ViewData["Success"] = i + " customer successfully imported.";
                }
                else
                {
                    ViewData["Output"] = "Please choose customer to be imported.";
                }
            }


            if (Request.HttpMethod == "POST" && Request["action"] == "delete")
            {
                int i = 0;
                foreach (string s in Request.Params.Keys)
                {
                    string substr = s.Substring(0, 2);
                    string str = Request[s];

                    if (substr == "cb" && !String.IsNullOrWhiteSpace(str))
                    {
                        int nId = Convert.ToInt32(str);
                        var checkNI = _db.NasabahImports.Where(c => c.Id == nId);
                        if (checkNI.Count() > 0)
                        {
                            NasabahImport ni = checkNI.First();
                            _db.DeleteObject(ni);
                            i++;
                        }
                    }

                }

                _db.SaveChanges();

                if (i > 0)
                {
                    ViewData["Success"] = i + " data successfully deleted.";
                }
                else
                {
                    ViewData["Output"] = "Please choose customer to be deleted";
                }
            }

            return View();
        }

        public ActionResult importlist(int page = 1, string key = "", int perpage = 20, string status = "", string orderBy = "", string orderMode = "")
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

            var data = (from c in _db.NasabahImports select c).Where(c => 1 == 1);
            int TeamId = 0;


            if (!String.IsNullOrEmpty(key))
            {
                data = data.Where(c => c.GCIF.Contains(key) || c.NAMA.Contains(key));
            }

            int count = data.Count();

            data = data.OrderByDescending(c => c.DateCreated);

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

            msg += "<table class=\"grid-cust small\" width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"6\">";
            msg += "<tr><th scope=\"col\"><input type=\"checkbox\" id=\"checkall\" /></th>";

            msg += "<th scope=\"col\">No</th><th scope=\"col\">NPK</th><th scope=\"col\">GCIF</th><th scope=\"col\">CIF</th>" +
                        "<th scope=\"col\">ACCT</th><th scope=\"col\">PRODUCT</th><th scope=\"col\">PRODUCT NAME</th><th scope=\"col\">PRODUCT STATUS</th>" +
                        "<th scope=\"col\">NAMA</th><th scope=\"col\">TELPON</th><th scope=\"col\">HANDPHONE</th>" +
                        "<th scope=\"col\">ALAMAT</th><th scope=\"col\">ID KTP</th><th scope=\"col\">GENDER</th><th scope=\"col\">TGL LAHIR</th><th scope=\"col\">NOTE</th>";

            msg += "</tr><tbody>";

            i = dataStart;

            foreach (NasabahImport item in result)
            {
                string klas = (i % 2 == 0) ? "gridrow_alternate" : "gridrow";
                string gender = (item.GENDER == "f") ? "P" : "L";
                string tglLahir = (item.TGLLAHIR != null) ? item.TGLLAHIR.GetValueOrDefault().ToString("dd-MM-yyyy") : "";

                msg += "<tr class=\"" + klas + "\">";
                msg += "<td align=\"center\"><input type=\"checkbox\" class=\"cbox\" name=\"cbox_" + item.Id + "\" value=\"" + item.Id + "\" /></td>";

                msg += "<td align=\"center\">" + i + "</td>" +
                       "<td align=\"center\">" + item.NPK + "</td>" +
                       "<td align=\"center\">" + item.GCIF + "</td>" +
                       "<td align=\"center\">" + item.CIF + "</td>" +
                       "<td>" + item.ACCT + "</td>" +
                       "<td>" + item.PRODUCT + "</td>" +
                       "<td>" + item.PRODUCTNAME + "</td>" +
                       "<td align=\"center\">" + item.PRODUCTSTATUS + "</td>" +
                       "<td>" + item.NAMA + "</td>" +
                       "<td>" + item.TELEPONRUMAH + "</td>" +
                       "<td>" + item.HANDPHONE + "</td>" +
                       "<td>" + item.ALAMAT + "</td>" +
                       "<td>" + item.IDKTP + "</td>" +
                       "<td align=\"center\">" + gender + "</td>" +
                       "<td align=\"center\">" + tglLahir + "</td>" +
                       "<td><font style=\"color:red\">" + item.Note + "</font></td>";

                msg += "</tr>";

                i++;
            }
            msg += "</tbody></table>";


            if (i == 0) msg += "<div class=\"arial\" style=\"text-align:center;padding:10px 0;\">No result found</div>";

            msg += "<div class=\"clear\"></div><input type=\"hidden\" name=\"action\" id=\"action\" value=\"import\" /><input type=\"button\" class=\"button\" name=\"import\" id=\"import\" value=\" Import Selected Customer \" /> &nbsp; <input type=\"button\" class=\"button\" name=\"delete\" id=\"delete\" id=\"delete\" value=\" Delete Selected Data \" style=\"background:red;color:white;border:1px solid #000\" /><br />";

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


        public ViewResult Detail(int id)
        {
            var checkCust = _db.Nasabahs.Where(c => c.NasabahId == id);
            if (checkCust.Count() > 0)
            {
                Nasabah cust = checkCust.First();
                return View(cust);
            }

            return View("index");
        }


        public ActionResult UpdateProductStatus(int id = 0)
        {
            var data = (from c in _db.Nasabahs select c).OrderByDescending(c => c.NasabahId).Skip(id).Take(1000);

            var checkProduk = _db.Products.OrderBy(c => c.ProductId);
            int i = id + 1;
            foreach (Nasabah item in data)
            {
                string ProductStatus = String.Empty;

                foreach (Product pr in checkProduk)
                {
                    var checkNP = _db.NasabahProducts.Where(c => c.NasabahId == item.NasabahId && c.ProductId == pr.ProductId).OrderByDescending(c => c.LastUpdate);
                    if (checkNP.Count() > 0)
                    {
                        NasabahProduct np = checkNP.First();

                        string strReferal = "";
                        var checkRef = _db.SalesNasabahs.Where(c => c.NasabahId == item.NasabahId && c.IsReferral == 1 && c.ProductId == pr.ProductId);
                        if (checkRef.Count() > 0)
                        {
                            var sn = checkRef.First();
                            Sale salesFrom = _db.Sales.Where(c => c.SalesId == sn.RefferalFrom).FirstOrDefault();
                            Sale salesTo = _db.Sales.Where(c => c.SalesId == sn.SalesId).FirstOrDefault();
                            strReferal = " - REFERRAL FROM " + salesFrom.Name + " TO " + salesTo.Name;
                        }

                        string salesName = (np.SalesId != null) ? np.Sale.Name + " (" + np.Status + ")" + strReferal : np.Status;
                        string imageId = "";

                        if (np.SalesId > 0)
                        {
                            if (item.Status == "NEW")
                            {
                                switch (np.Status)
                                {
                                    case "REKOMENDASI": imageId = "6"; break;
                                    case "EXISTING": imageId = "1"; break;
                                    case "WARM": imageId = "2"; break;
                                    case "HOT": imageId = "3"; break;
                                    case "BOOKING": imageId = "4"; break;
                                    case "CANCEL": imageId = "5"; break;
                                }
                            }
                            else
                            {
                                switch (np.Status)
                                {
                                    case "REKOMENDASI": imageId = "6"; break;
                                    case "EXISTING": imageId = "1"; break;
                                    case "WARM": imageId = "7"; break;
                                    case "HOT": imageId = "8"; break;
                                    case "BOOKING": imageId = "9"; break;
                                    case "CANCEL": imageId = "10"; break;
                                }
                            }
                        }
                        else
                        {
                            imageId = "1";
                        }

                        ProductStatus += pr.ProductId + ":" + np.SalesId + ":" + imageId + ":" + salesName + ",";
                    }

                }

                ProductStatus = ProductStatus.TrimEnd(',');
                item.ProductStatus = ProductStatus;

                _db.ApplyCurrentValues(item.EntityKey.EntitySetName, item);

                i++;
            }

            _db.SaveChanges();

            int dataStart = id + 1;
            int dataEnd = i;

            if (dataStart == dataEnd)
            {
                ViewBag.Output = "Tidak ada data nasabah yang diupdate.";
            }
            else
            {
                ViewBag.Output = "Nasabah " + dataStart + " sampai " + dataEnd + " telah berhasil diupdate.";
            }

            return View("output");
        }
    }
}

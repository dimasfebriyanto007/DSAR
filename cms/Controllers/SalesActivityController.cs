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
    public class SalesActivityController : CommonController
    {

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Customer");
        }
        
        public ActionResult Call(int id)
        {
            if (CommonModel.UserRole() != "SALES") return RedirectToAction("Index", "Home");
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }

            var checkNasabah = _db.Nasabahs.Where(c => c.NasabahId == id);
            if (checkNasabah.Count() == 0) return RedirectToAction("Index", "Home");

            Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();
            if (sales.TeamId == 4) return RedirectToAction("Index", "Home");
            
            var checkSN = _db.SalesNasabahs.Where(c => c.SalesId == sales.SalesId && c.NasabahId == id);
            if (checkSN.Count() == 0) return RedirectToAction("Index", "Home");

            
            Nasabah nasabah = checkNasabah.First();
            var Reasons = _db.CallReasons.OrderBy(c => c.ReasonId).ToList();
            var Products = _db.Products.OrderBy(c => c.Name);
            var Status = PipelineOption;
            ViewData["NasabahName"] = nasabah.Name;
            ViewData["NasabahId"] = id;     

            string htmlProducts = "<div style=\"border:1px solid #eee;width:98%;padding:5px 10px;overflow-x:auto\">";
            if (Products.Count() > 0)
            {
                htmlProducts += "<table class=\"grid-cust\" width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"6\"><tr>" +
                        "<th scope=\"col\">NAMA PRODUK</th><th scope=\"col\">NOTE</th><th scope=\"col\">CALL STATUS</th><th scope=\"col\">REASON</th>" +
                        "<th scope=\"col\">DESCRIPTION</th><th scope=\"col\">CALL DATE</th>" +
                        "<th scope=\"col\">SCHEDULE VISIT</th>" +
                        "<th scope=\"col\">ACCOUNT INFO</th></tr><tbody>";
                int i = 1;
                foreach (Product prod in Products)
                {
                    string klas = (i % 2 == 0) ? "gridrow_alternate" : "gridrow";
                    var checkNP = _db.NasabahProducts.Where(c => c.NasabahId == nasabah.NasabahId && c.ProductId == prod.ProductId).OrderByDescending(c => c.LastUpdate);
                    string strStatus = String.Empty;
                    string reasonId = (Request["reason_" + prod.ProductId] != null) ? Request["reason_" + prod.ProductId] : "";
                    string desc = (Request["desc_" + prod.ProductId] != null) ? Request["desc_" + prod.ProductId] : "";
                    string callDate = (Request["calldate_" + prod.ProductId] != null) ? Request["calldate_" + prod.ProductId] : DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    string visitDate = (Request["visitdate_" + prod.ProductId] != null) ? Request["visitdate_" + prod.ProductId] : "";
                    string postStatus = (Request["status_" + prod.ProductId] != null) ? Request["status_" + prod.ProductId] : "";

                    if (checkNP.Count() > 0)
                    {
                        string strReferal = "";

                        var checkRef = _db.SalesNasabahs.Where(c => c.NasabahId == nasabah.NasabahId && c.SalesId == sales.SalesId && c.ProductId == prod.ProductId);
                        if (checkRef.Count() > 0)
                        {
                            SalesNasabah sn = checkRef.First();
                            if (sn.IsReferral == 1)
                            {
                                Sale salesFrom = _db.Sales.Where(c => c.SalesId == sn.RefferalFrom).FirstOrDefault();
                                strReferal = " - REFERRAL FROM " + salesFrom.Name;
                            }
                        }

                        var checkRef2 = _db.SalesNasabahs.Where(c => c.NasabahId == nasabah.NasabahId && c.RefferalFrom == sales.SalesId && c.ProductId == prod.ProductId);
                        if (checkRef2.Count() > 0)
                        {
                            SalesNasabah sn = checkRef2.First();
                            if (sn.IsReferral == 1)
                            {
                                Sale salesFrom = _db.Sales.Where(c => c.SalesId == sn.SalesId).FirstOrDefault();
                                strReferal = " - REFER TO " + salesFrom.Name;
                            }
                        }

                        NasabahProduct np = checkNP.First();
                        string salesName = (np.SalesId != null) ?   np.Sale.Name + " (" + np.Status + ")" + strReferal : np.Status;
                        string icon_name = (np.Nasabah.Status == "NEW") ? "inisiatif" : "rekomendasi";
                        switch (np.Status)
                        {
                            case "REKOMENDASI": strStatus += "<img src=\"" + Url.Content("~/Content/images/icon_rekomendasi_new.png") + "\" title=\"" + salesName + "\" />";
                                break;
                            case "EXISTING": strStatus += "<img src=\"" + Url.Content("~/Content/images/icon_existing.png") + "\" title=\"" + salesName + "\" />";
                                break;
                            case "WARM": strStatus += "<img src=\"" + Url.Content("~/Content/images/icon_" + icon_name + "_warm.png") + "\" title=\"" + salesName + "\" />";
                                break;
                            case "HOT": strStatus += "<img src=\"" + Url.Content("~/Content/images/icon_" + icon_name + "_hot.png") + "\" title=\"" + salesName + "\" />";
                                break;
                            case "BOOKING": strStatus += "<img src=\"" + Url.Content("~/Content/images/icon_" + icon_name + "_booking.png") + "\" title=\"" + salesName + "\" />";
                                break;
                            case "CANCEL": strStatus += "<img src=\"" + Url.Content("~/Content/images/icon_" + icon_name + "_cancel.png") + "\" title=\"" + salesName + "\" />";
                                break;
                            default: strStatus += (np.SalesId > 0) ? "<img src=\"" + Url.Content("~/Content/images/icon_rekomendasi_new.png") + "\" title=\"" + salesName + "\" />" : "<img src=\"" + Url.Content("~/Content/images/icon_existing.png") + "\" title=\"" + salesName + "\" />";
                                break;
                        }
                    }

                    string htmlReason = "<select name=\"reason_" + prod.ProductId + "\" id=\"reason_" + prod.ProductId + "\" rel=\"" + prod.ProductId + "\" class=\"reason\">";
                    htmlReason += "<option value=\"\"></option>";
                    //foreach (CallReason cr in Reasons)
                    //{
                    //    if (cr.ReasonId == 3 || cr.ReasonId == 4 || cr.ReasonId == 5 || cr.ReasonId == 6) // added by Aditia S - 20160921
                    //    {
                    //        string selected = (reasonId == cr.ReasonId.ToString()) ? " selected=\"selected\"" : "";
                    //        htmlReason += "<option value=\"" + cr.ReasonId + "\"" + selected + ">" + cr.Description + "</option>";
                    //    }
                    //}
                    htmlReason += "</option>";


                    string callReason = "<select name=\"status_" + prod.ProductId + "\" id=\"status_" + prod.ProductId + "\" class=\"status\" rel=\"" + prod.ProductId + "\">";
                    callReason += "<option value=\"\">--CHOOSE--</option>";
                    foreach (PipelineOptionModel pm in Status)
                    {
                        string selected = (postStatus == pm.OptionId) ? " selected=\"selected\"" : "";
                        callReason += "<option value=\"" + pm.OptionId + "\"" + selected + ">" + pm.OptionString + "</option>";
                    }
                    callReason += "</option>";

                    htmlProducts += "<tr class=\"" + klas + "\">" +
                                    "<td>" + prod.Name + "</td>" +
                                    "<td style=\"text-align:center;width:60px\">" + strStatus + "</td>" +
                                    "<td>" + callReason + "</td>" +
                                    "<td>" + htmlReason + "</td>" +
                                    "<td><input type=\"text\" name=\"desc_" + prod.ProductId + "\" id=\"desc_" + prod.ProductId + "\" value=\""+desc+"\" style=\"width:150px\" /></td>" +
                                    "<td><input type=\"text\" name=\"calldate_" + prod.ProductId + "\" id=\"calldate_" + prod.ProductId + "\" value=\"" + callDate + "\" style=\"width:130px\" class=\"tanggal\" /></td>" +
                                    "<td><input type=\"text\" name=\"visitdate_" + prod.ProductId + "\" id=\"visitdate_" + prod.ProductId + "\" value=\"" + visitDate + "\" style=\"width:130px\" class=\"tanggal\" /></td>" +
                                    "<td><div id=\"ifBooking_" + prod.ProductId + "\"></div></td></tr>";
                    i++;
                }

                htmlProducts += "</tbody></table>";
            }
            htmlProducts += "</div>";

            ViewBag.htmlProducts = htmlProducts;


            if (Request.HttpMethod == "POST")
            {
                foreach (string s in Request.Params.Keys)
                {
                    if (s.Contains("reason_"))
                    {
                        if (String.IsNullOrWhiteSpace(Request[s])) continue;

                        string[] arrayStr = s.Split('_');
                        Call dataToCreate = new Call();
                        dataToCreate.NasabahId = id;
                        dataToCreate.ProductId = Convert.ToInt32(arrayStr[1]);

                        var checkNP = _db.NasabahProducts.Where(c => c.NasabahId == dataToCreate.NasabahId && c.ProductId == dataToCreate.ProductId && c.SalesId != sales.SalesId && (c.Status == "COLD" || c.Status == "WARM" || c.Status == "HOT"));
                        if (checkNP.Count() > 0)
                        {
                            NasabahProduct np = checkNP.First();
                            ViewData["Output"] = "Nasabah yang Anda pilih sudah pernah ditawarkan produk " + np.Product.Name + " oleh sales " + np.Sale.Name + " (" + np.Sale.Npk + ")";
                            return View();
                        }
                    }
                }

                int i = 0;
                foreach (string s in Request.Params.Keys)
                {
                    if (s.Contains("reason_"))
                    {
                        if (String.IsNullOrWhiteSpace(Request[s])) continue;

                        string[] arrayStr = s.Split('_');
                        Call dataToCreate = new Call();
                        dataToCreate.NasabahId = id;
                        dataToCreate.CreatedDate = DateTime.Now;
                        dataToCreate.SalesId = sales.SalesId;
                        dataToCreate.ProductId = Convert.ToInt32(arrayStr[1]);
                        dataToCreate.Status = Request["status_" + dataToCreate.ProductId];
                        dataToCreate.ReasonId = Convert.ToInt32(Request["reason_" + dataToCreate.ProductId]);
                        dataToCreate.Note = (Request["desc_" + dataToCreate.ProductId] != null) ? Request["desc_" + dataToCreate.ProductId] : "";
                        if (Request["calldate_" + dataToCreate.ProductId] != null)
                        {
                            if (!String.IsNullOrWhiteSpace(Request["calldate_" + dataToCreate.ProductId]))
                            {
                                DateTime MyDateTime = new DateTime();
                                MyDateTime = DateTime.ParseExact(Request["calldate_" + dataToCreate.ProductId], "yyyy-MM-dd HH:mm", null);
                                dataToCreate.CallDate = MyDateTime;
                            }
                        }
                        else
                        {
                            dataToCreate.CallDate = DateTime.Now;
                        }

                        if (!String.IsNullOrWhiteSpace(Request["visitdate_" + dataToCreate.ProductId]))
                        {
                            DateTime MyDateTime = new DateTime();
                            MyDateTime = DateTime.ParseExact(Request["visitdate_" + dataToCreate.ProductId], "yyyy-MM-dd HH:mm", null);
                            dataToCreate.VisitSchedule = MyDateTime;
                        }
                        

                        var checkNP2 = _db.NasabahProducts.Where(c => c.NasabahId == dataToCreate.NasabahId && c.ProductId == dataToCreate.ProductId && c.SalesId == sales.SalesId && (c.Status == "COLD" || c.Status == "WARM" || c.Status == "HOT"));
                        if (checkNP2.Count() > 0)
                        {
                            NasabahProduct np = checkNP2.First();
                            np.Status = dataToCreate.Status;
                            np.LastUpdate = DateTime.Now;
                            _db.ApplyCurrentValues(np.EntityKey.EntitySetName, np);
                        }
                        else
                        {

                            NasabahProduct np = new NasabahProduct();
                            np.SalesId = sales.SalesId;
                            np.NasabahId = dataToCreate.NasabahId;
                            np.ProductId = dataToCreate.ProductId;

                            if (!String.IsNullOrWhiteSpace(Request["CIF_" + dataToCreate.ProductId]))
                            {
                                np.CIF = Request["CIF_" + dataToCreate.ProductId].ToString();
                            }
                            if (!String.IsNullOrWhiteSpace(Request["ACCT_" + dataToCreate.ProductId]))
                            {
                                np.ACCT = Request["ACCT_" + dataToCreate.ProductId].ToString();
                            }
                            if (!String.IsNullOrWhiteSpace(Request["Amount_" + dataToCreate.ProductId]))
                            {
                                np.Amount = Convert.ToDecimal(Request["Amount_" + dataToCreate.ProductId]);
                            }
                            np.Status = dataToCreate.Status;
                            np.LastUpdate = DateTime.Now;

                            _db.AddToNasabahProducts(np);
                        }

                        _db.AddToCalls(dataToCreate);
                        _db.SaveChanges();

                        if (Request["visitdate_" + dataToCreate.ProductId] != null)
                        {
                            if (!String.IsNullOrWhiteSpace(Request["visitdate_" + dataToCreate.ProductId]))
                            {
                                Visit newVisit = new Visit();
                                newVisit.SalesId = sales.SalesId;
                                newVisit.NasabahId = dataToCreate.NasabahId;
                                newVisit.ProductId = dataToCreate.ProductId;
                                newVisit.Note = "PLAN TO VISIT (FROM CALL:" + dataToCreate.CallDate.ToString("dd MMM yyyy HH:mm") + ")";  // added by Aditia S - 20160921

                                DateTime MyDateTime = new DateTime();
                                MyDateTime = DateTime.ParseExact(Request["visitdate_" + dataToCreate.ProductId], "yyyy-MM-dd HH:mm", null);
                                newVisit.VisitDate = MyDateTime;

                                newVisit.CreatedDate = DateTime.Now;
                                _db.AddToVisits(newVisit);
                                _db.SaveChanges();
                            }
                        }

                        i++;
                    }
                }

                if (i > 0)
                {

                    var checkSN2 = _db.SalesNasabahs.Where(c => c.NasabahId == id && c.SalesId == sales.SalesId);
                    if (checkSN2.Count() > 0)
                    {
                        SalesNasabah sn2 = checkSN2.First();
                        sn2.LastUpdate = DateTime.Now;
                        _db.ApplyCurrentValues(sn2.EntityKey.EntitySetName, sn2);
                    }

                    CommonModel.UpdateProductStatus(nasabah.NasabahId);
                    ViewData["Success"] = "Call activity successfully saved";
                    return View("Index");
                }
            }

            
            return View();
        }
        
        public ActionResult Visit(int id)
        {
            if (CommonModel.UserRole() != "SALES") return RedirectToAction("Index", "Home");
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }

            var checkNasabah = _db.Nasabahs.Where(c => c.NasabahId == id);
            if (checkNasabah.Count() == 0) return RedirectToAction("Index", "Home");

            Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();
            if (sales.TeamId == 4) return RedirectToAction("Index", "Home");

            var checkSN = _db.SalesNasabahs.Where(c => c.SalesId == sales.SalesId && c.NasabahId == id);
            if (checkSN.Count() == 0) return RedirectToAction("Index", "Home");


            Nasabah nasabah = checkNasabah.First();
            var Reasons = _db.VisitReasons.OrderBy(c => c.ReasonId).ToList();
            var Products = _db.Products.OrderBy(c => c.Name);
            var Status = PipelineOption;
            ViewData["NasabahName"] = nasabah.Name;
            ViewData["NasabahId"] = id;

            string htmlProducts = "<div style=\"border:1px solid #eee;width:98%;padding:5px 10px;overflow-x:auto\">";
            if (Products.Count() > 0)
            {
                htmlProducts += "<table class=\"grid-cust\" width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"6\"><tr>" +
                        "<th scope=\"col\">NAMA PRODUK</th><th scope=\"col\">NOTE</th><th scope=\"col\">VISIT STATUS</th>" +
                        "<th scope=\"col\">REASON</th><th scope=\"col\">DESCRIPTION</th><th scope=\"col\">VISIT DATE</th>" +
                        "<th scope=\"col\">ACCOUNT INFO</th></tr><tbody>";
                int i = 1;
                foreach (Product prod in Products)
                {
                    string klas = (i % 2 == 0) ? "gridrow_alternate" : "gridrow";
                    var checkNP = _db.NasabahProducts.Where(c => c.NasabahId == nasabah.NasabahId && c.ProductId == prod.ProductId).OrderByDescending(c => c.LastUpdate);
                    string strStatus = String.Empty;
                    string reasonId = (Request["reason_" + prod.ProductId] != null) ? Request["reason_" + prod.ProductId] : "";
                    string desc = (Request["desc_" + prod.ProductId] != null) ? Request["desc_" + prod.ProductId] : "";
                    string visitDate = (Request["visitdate_" + prod.ProductId] != null) ? Request["visitdate_" + prod.ProductId] : DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    string postStatus = (Request["status_" + prod.ProductId] != null) ? Request["status_" + prod.ProductId] : "";

                    if (checkNP.Count() > 0)
                    {
                        string strReferal = "";

                        var checkRef = _db.SalesNasabahs.Where(c => c.NasabahId == nasabah.NasabahId && c.SalesId == sales.SalesId && c.ProductId == prod.ProductId);
                        if (checkRef.Count() > 0)
                        {
                            SalesNasabah sn = checkRef.First();
                            if (sn.IsReferral == 1)
                            {
                                Sale salesFrom = _db.Sales.Where(c => c.SalesId == sn.RefferalFrom).FirstOrDefault();
                                strReferal = " - REFERRAL FROM " + salesFrom.Name;
                            }
                        }

                        var checkRef2 = _db.SalesNasabahs.Where(c => c.NasabahId == nasabah.NasabahId && c.RefferalFrom == sales.SalesId && c.ProductId == prod.ProductId);
                        if (checkRef2.Count() > 0)
                        {
                            SalesNasabah sn = checkRef2.First();
                            if (sn.IsReferral == 1)
                            {
                                Sale salesFrom = _db.Sales.Where(c => c.SalesId == sn.SalesId).FirstOrDefault();
                                strReferal = " - REFER TO " + salesFrom.Name;
                            }
                        }

                        NasabahProduct np = checkNP.First();
                        string salesName = (np.SalesId != null) ? np.Sale.Name + " (" + np.Status + ")" + strReferal : np.Status;
                        string icon_name = (np.Nasabah.Status == "NEW") ? "inisiatif" : "rekomendasi";
                        switch (np.Status)
                        {
                            case "REKOMENDASI": strStatus += "<img src=\"" + Url.Content("~/Content/images/icon_rekomendasi_new.png") + "\" title=\"" + salesName + "\" />";
                                break;
                            case "EXISTING": strStatus += "<img src=\"" + Url.Content("~/Content/images/icon_existing.png") + "\" title=\"" + salesName + "\" />";
                                break;
                            case "WARM": strStatus += "<img src=\"" + Url.Content("~/Content/images/icon_" + icon_name + "_warm.png") + "\" title=\"" + salesName + "\" />";
                                break;
                            case "HOT": strStatus += "<img src=\"" + Url.Content("~/Content/images/icon_" + icon_name + "_hot.png") + "\" title=\"" + salesName + "\" />";
                                break;
                            case "BOOKING": strStatus += "<img src=\"" + Url.Content("~/Content/images/icon_" + icon_name + "_booking.png") + "\" title=\"" + salesName + "\" />";
                                break;
                            case "CANCEL": strStatus += "<img src=\"" + Url.Content("~/Content/images/icon_" + icon_name + "_cancel.png") + "\" title=\"" + salesName + "\" />";
                                break;
                            default: strStatus += (np.SalesId > 0) ? "<img src=\"" + Url.Content("~/Content/images/icon_rekomendasi_new.png") + "\" title=\"" + salesName + "\" />" : "<img src=\"" + Url.Content("~/Content/images/icon_existing.png") + "\" title=\"" + salesName + "\" />";
                                break;
                        }
                    }

                    string htmlReason = "<select name=\"reason_" + prod.ProductId + "\" id=\"reason_" + prod.ProductId + "\" rel=\"" + prod.ProductId + "\" class=\"reason\">";
                    htmlReason += "<option value=\"\"></option>";
                    //foreach (VisitReason cr in Reasons)
                    //{
                    //    if (cr.ReasonId == 1 || cr.ReasonId == 2 || cr.ReasonId == 5 || cr.ReasonId == 6)    // added by Aditia S - 20160921
                    //    {
                    //        string selected = (reasonId == cr.ReasonId.ToString()) ? " selected=\"selected\"" : "";
                    //        htmlReason += "<option value=\"" + cr.ReasonId + "\"" + selected + ">" + cr.Description + "</option>";
                    //    }
                    //}
                    htmlReason += "</option>";


                    string callReason = "<select name=\"status_" + prod.ProductId + "\" id=\"status_" + prod.ProductId + "\" class=\"status\" rel=\"" + prod.ProductId + "\">";
                    callReason += "<option value=\"\">--CHOOSE--</option>";
                    foreach (PipelineOptionModel pm in Status)
                    {
                        string selected = (postStatus == pm.OptionId) ? " selected=\"selected\"" : "";
                        callReason += "<option value=\"" + pm.OptionId + "\"" + selected + ">" + pm.OptionString + "</option>";
                    }
                    callReason += "</option>";

                    htmlProducts += "<tr class=\"" + klas + "\">" +
                                    "<td>" + prod.Name + "</td>" +
                                    "<td style=\"text-align:center;width:60px\">" + strStatus + "</td><td>" + callReason + "</td>" +
                                    "<td>" + htmlReason + "</td>" +
                                    "<td><input type=\"text\" name=\"desc_" + prod.ProductId + "\" id=\"desc_" + prod.ProductId + "\" value=\"" + desc + "\" style=\"width:150px\" /></td>" +
                                    "<td><input type=\"text\" name=\"visitdate_" + prod.ProductId + "\" id=\"visitdate_" + prod.ProductId + "\" value=\"" + visitDate + "\" style=\"width:130px\" class=\"tanggal\" /></td>" +
                                    "<td><div id=\"ifBooking_" + prod.ProductId + "\"></div></td></tr>";
                    i++;
                }

                htmlProducts += "</tbody></table>";
            }
            htmlProducts += "</div>";

            ViewBag.htmlProducts = htmlProducts;


            if (Request.HttpMethod == "POST")
            {
                foreach (string s in Request.Params.Keys)
                {
                    if (s.Contains("reason_"))
                    {
                        if (String.IsNullOrWhiteSpace(Request[s])) continue;

                        string[] arrayStr = s.Split('_');
                        Call dataToCreate = new Call();
                        dataToCreate.NasabahId = id;
                        dataToCreate.ProductId = Convert.ToInt32(arrayStr[1]);

                        var checkNP = _db.NasabahProducts.Where(c => c.NasabahId == dataToCreate.NasabahId && c.ProductId == dataToCreate.ProductId && c.SalesId != sales.SalesId && (c.Status == "COLD" || c.Status == "WARM" || c.Status == "HOT"));
                        if (checkNP.Count() > 0)
                        {
                            NasabahProduct np = checkNP.First();
                            ViewData["Output"] = "Nasabah yang Anda pilih sudah pernah ditawarkan produk " + np.Product.Name + " oleh sales " + np.Sale.Name + " (" + np.Sale.Npk + ")";
                            return View();
                        }
                    }
                }

                int i = 0;
                foreach (string s in Request.Params.Keys)
                {
                    if (s.Contains("reason_"))
                    {
                        if (String.IsNullOrWhiteSpace(Request[s])) continue;

                        string[] arrayStr = s.Split('_');
                        Visit dataToCreate = new Visit();
                        dataToCreate.NasabahId = id;
                        dataToCreate.CreatedDate = DateTime.Now;
                        dataToCreate.SalesId = sales.SalesId;
                        dataToCreate.ProductId = Convert.ToInt32(arrayStr[1]);
                        dataToCreate.Status = Request["status_" + dataToCreate.ProductId];
                        dataToCreate.ReasonId = Convert.ToInt32(Request["reason_" + dataToCreate.ProductId]);
                        dataToCreate.Note = (Request["desc_" + dataToCreate.ProductId] != null) ? Request["desc_" + dataToCreate.ProductId] : "";


                        if (!String.IsNullOrWhiteSpace(Request["visitdate_" + dataToCreate.ProductId]))
                        {
                            DateTime MyDateTime = new DateTime();
                            MyDateTime = DateTime.ParseExact(Request["visitdate_" + dataToCreate.ProductId], "yyyy-MM-dd HH:mm", null);
                            dataToCreate.VisitDate = MyDateTime;
                        }
                        else
                        {
                            dataToCreate.VisitDate = DateTime.Now;
                        }

                        var checkNP2 = _db.NasabahProducts.Where(c => c.NasabahId == dataToCreate.NasabahId && c.ProductId == dataToCreate.ProductId && c.SalesId == sales.SalesId && (c.Status == "COLD" || c.Status == "WARM" || c.Status == "HOT"));
                        if (checkNP2.Count() > 0)
                        {
                            NasabahProduct np = checkNP2.First();
                            np.Status = dataToCreate.Status;
                            np.LastUpdate = DateTime.Now;
                            _db.ApplyCurrentValues(np.EntityKey.EntitySetName, np);
                        }
                        else
                        {

                            NasabahProduct np = new NasabahProduct();
                            np.SalesId = sales.SalesId;
                            np.NasabahId = dataToCreate.NasabahId;
                            np.ProductId = dataToCreate.ProductId;

                            if (!String.IsNullOrWhiteSpace(Request["CIF_" + dataToCreate.ProductId]))
                            {
                                np.CIF = Request["CIF_" + dataToCreate.ProductId].ToString();
                            }
                            if (!String.IsNullOrWhiteSpace(Request["ACCT_" + dataToCreate.ProductId]))
                            {
                                np.ACCT = Request["ACCT_" + dataToCreate.ProductId].ToString();
                            }
                            if (!String.IsNullOrWhiteSpace(Request["Amount_" + dataToCreate.ProductId]))
                            {
                                np.Amount = Convert.ToDecimal(Request["Amount_" + dataToCreate.ProductId]);
                            }
                            np.Status = dataToCreate.Status;
                            np.LastUpdate = DateTime.Now;

                            _db.AddToNasabahProducts(np);
                        }

                        _db.AddToVisits(dataToCreate);
                        _db.SaveChanges();
                        
                        i++;
                    }
                }

                if (i > 0)
                {

                    var checkSN2 = _db.SalesNasabahs.Where(c => c.NasabahId == id && c.SalesId == sales.SalesId);
                    if (checkSN2.Count() > 0)
                    {
                        SalesNasabah sn2 = checkSN2.First();
                        sn2.LastUpdate = DateTime.Now;
                        _db.ApplyCurrentValues(sn2.EntityKey.EntitySetName, sn2);
                    }

                    CommonModel.UpdateProductStatus(nasabah.NasabahId);

                    ViewData["Success"] = "Call activity successfully saved";
                    return View("Index");
                }
            }            

            return View();
        }

        public ActionResult Reference(int id)
        {
            if (CommonModel.UserRole() != "SALES") return RedirectToAction("Index", "Home");
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }

            var checkNasabah = _db.Nasabahs.Where(c => c.NasabahId == id);
            if (checkNasabah.Count() == 0) return RedirectToAction("Index", "Home");

            Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();
            
            var checkSN = _db.SalesNasabahs.Where(c => c.SalesId == sales.SalesId && c.NasabahId == id);
            if (checkSN.Count() == 0) return RedirectToAction("Index", "Home");

            string strPID = (!String.IsNullOrEmpty(Request["pId"])) ? Request["pId"] : "";

            SalesNasabah np = new SalesNasabah();
            if (strPID != "")
            {
                int pId = Convert.ToInt32(strPID);                
                var checkProduk = _db.Products.Where(c => c.ProductId == pId);
                if (checkProduk.Count()>0){
                    np.ProductId = pId;
                    Product produk = checkProduk.First();
                    ViewData["ProductName"] = produk.Name;
                }                
            }

            Nasabah nasabah = checkNasabah.First();
            ViewData["Sales"] = from c in _db.Sales where c.TeamId != 4 && c.SalesId!=sales.SalesId orderby c.Name 
                                select new { 
                                    SalesId = c.SalesId,
                                    Name = c.Name + " (" + c.Npk + ")"
                                };
            ViewData["Products"] = _db.Products.OrderBy(c => c.Name).ToList();
            ViewData["NasabahName"] = nasabah.Name;
            ViewData["NasabahId"] = id;

            return View(np);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Reference([Bind(Exclude = "Id")] SalesNasabah dataToCreate)
        {
            if (CommonModel.UserRole() != "SALES") return RedirectToAction("Index", "Home");
            if (CommonModel.BelumAbsen()) return RedirectToAction("Index", "Home");
            
            var checkNasabah = _db.Nasabahs.Where(c => c.NasabahId == dataToCreate.NasabahId);
            if (checkNasabah.Count() == 0) return RedirectToAction("Index", "Home");

            Sale currentSales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();

            var checkSN = _db.SalesNasabahs.Where(c => c.SalesId == currentSales.SalesId && c.NasabahId == dataToCreate.NasabahId);
            if (checkSN.Count() == 0) return RedirectToAction("Index", "Home");

            Nasabah nasabah = checkNasabah.First();
            ViewData["Sales"] = from c in _db.Sales
                                where c.TeamId != 4  && c.SalesId != currentSales.SalesId
                                orderby c.Name
                                select new
                                {
                                    SalesId = c.SalesId,
                                    Name = c.Name + " (" + c.Npk + ")"
                                };
            ViewData["Products"] = _db.Products.OrderBy(c => c.Name).ToList();
            ViewData["NasabahName"] = nasabah.Name;
            ViewData["NasabahId"] = dataToCreate.NasabahId;

            try
            {
                dataToCreate.CreatedDate = DateTime.Now;
                dataToCreate.LastUpdate = DateTime.Now;
                dataToCreate.IsReferral = 1;
                dataToCreate.RefferalFrom = currentSales.SalesId;

                
                var checkNP = _db.SalesNasabahs.Where(c => c.NasabahId == dataToCreate.NasabahId && (c.SalesId == dataToCreate.SalesId || c.RefferalFrom == dataToCreate.SalesId)); //_db.NasabahProducts.Where(c => c.NasabahId == dataToCreate.NasabahId && c.SalesId == dataToCreate.SalesId); // deleted by Aditia -> c.SalesId != currentSales.SalesId && c.ProductId == dataToCreate.ProductId && c.Status != "CANCEL"
                if (checkNP.Count() > 0)
                {
                    Sale sales = _db.Sales.Where(c => c.SalesId == dataToCreate.SalesId).First();
                    ViewData["Output"] = "Nasabah yang Anda pilih sudah pernah di-handle oleh sales dengan Nama : " +  sales.Name + "(" + sales.Npk + "-" + sales.Branch.Name + ")";  
                    
                    return View(dataToCreate);
                }


                var checkNP2 = _db.NasabahProducts.Where(c => c.NasabahId == dataToCreate.NasabahId && c.ProductId == dataToCreate.ProductId && c.SalesId == currentSales.SalesId);
                if (checkNP2.Count() > 0)
                {
                    NasabahProduct np = checkNP2.First();
                    Sale sales = _db.Sales.Where(c => c.SalesId == currentSales.SalesId).First();
                    np.Status = "REFERRAL";
                    np.SalesId = dataToCreate.SalesId;
                    np.Note =np.Note + "[FROM:"  + sales.Name + "-" + sales.Npk + "-" + sales.Branch.Name + "]";  
                    np.LastUpdate = DateTime.Now;
                    _db.ApplyCurrentValues(np.EntityKey.EntitySetName, np);                    
                }
                else
                {
                    NasabahProduct np = new NasabahProduct();
                    Sale sales = _db.Sales.Where(c => c.SalesId == currentSales.SalesId).First();
                    np.SalesId = dataToCreate.SalesId;
                    np.NasabahId = dataToCreate.NasabahId;
                    np.ProductId = dataToCreate.ProductId.GetValueOrDefault();
                    np.Status = "REFERRAL";
                    np.Note = "[FROM:" + sales.Name + "-" + sales.Npk + "-" + sales.Branch.Name + "]";  
                    np.LastUpdate = DateTime.Now;
                    _db.AddToNasabahProducts(np);
                }

                _db.AddToSalesNasabahs(dataToCreate);
                _db.SaveChanges();
                /*
                if (Request["From"] != null)
                {
                    ViewData["Output"] = "Referral successfully saved";
                    return View("Pipeline");
                }*/

                ViewData["Success"] = "Customer referral successfully saved";
                return View("Index");
            }
            catch (Exception e)
            {
                ViewData["Output"] = e.InnerException;
                return View(dataToCreate);
            }
        }
    }
}

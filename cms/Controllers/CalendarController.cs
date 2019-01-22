using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using cms.Models;
using System.Globalization;

namespace cms.Controllers
{
    public class CalendarController : CommonController
    {
        //
        // GET: /Calendar/

        public ActionResult Index()
        {
            if (CommonModel.UserRole() != "SALES") return RedirectToAction("Index", "Home");
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }

            Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();

            //var checkCall = _db.Calls.Where(c => c.SalesId == sales.SalesId && c.CallDate > DateTime.Now.AddMonths(-1)).OrderBy(c => c.CallDate);
            var checkCall = _db.Calls.Where(c => c.SalesId == sales.SalesId).OrderBy(c => c.CallDate);

            List<CalendarModel> json = new List<CalendarModel>();
            if (checkCall.Count() > 0)
            {
                foreach (Call item in checkCall)
                {
                    string visitSchedule = (!String.IsNullOrWhiteSpace(item.VisitSchedule.ToString())) ? "<strong>Visit Schedule :</strong> " + item.VisitSchedule.GetValueOrDefault().ToString("dd MMM yyyy HH:mm") + "<br />" : "";

                    CalendarModel call = new CalendarModel();
                    call.title = "Call: " + item.Nasabah.Name;
                    call.start = item.CallDate.ToString("yyyy-MM-dd HH:mm");
                    call.allDay = false;
                    call.color = "#3366CC";
                    call.tooltip = "<div class='tooltip'><strong>Activity :</strong> Call<br />" +
                                    "<strong>Customer :</strong> " + item.Nasabah.Name + "<br />" +
                                    "<strong>Product :</strong> " + item.Product.Name + "<br />" +
                                    "<strong>Reason :</strong> " + item.CallReason.Description + "<br />" +
                                    "<strong>Call Date :</strong> " + item.CallDate.ToString("dd MMM yyyy HH:mm") + "<br />" +
                                    visitSchedule +
                                    "<strong>Description :<br /></strong> " + item.Note +
                                    "</div>";

                    json.Add(call);
                }
            }

            var checkVisit = _db.Visits.Where(c => c.SalesId == sales.SalesId).OrderBy(c => c.VisitDate);

            if (checkVisit.Count() > 0)
            {
                foreach (Visit item in checkVisit)
                {
                    string strDesc = (item.ReasonId != null) ? item.VisitReason.Description : "";
                    CalendarModel visit = new CalendarModel();
                    visit.title = "Visit: " + item.Nasabah.Name;
                    visit.start = item.VisitDate.ToString("yyyy-MM-dd HH:mm");
                    visit.allDay = false;
                    visit.color = "#B7005C";
                    visit.tooltip = "<div class='tooltip'><strong>Activity :</strong> Visit<br />" +
                                    "<strong>Customer :</strong> " + item.Nasabah.Name + "<br />" +
                                    "<strong>Product :</strong> " + item.Product.Name + "<br />" +
                                    "<strong>Reason :</strong> " + strDesc + "<br />" +
                                    "<strong>Visit Date :</strong> " + item.VisitDate.ToString("dd MMM yyyy HH:mm") + "<br />" +
                                    "<strong>Description :<br /></strong> " + item.Note +
                                    "</div>";

                    json.Add(visit);
                }
            }

            // added by Aditia S - 20160801
            JsonResult jResult = new JsonResult { Data = json };
            JavaScriptSerializer jsJson = new JavaScriptSerializer();
            jsJson.MaxJsonLength = 2147483644;
            ViewBag.strJson = jsJson.Serialize(jResult.Data);
            // ----------------------- EOF Aditia S

            return View();
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

            List<int> YearOption = new List<int>();
            for (int i = DateTime.Now.Year; i >= 2013; i--)
            {
                YearOption.Add(i);
            }
            ViewData["YearOption"] = YearOption;

            List<SelectOptionModel> MonthOption = new List<SelectOptionModel>();
            string cultureString = "id-ID";
            var culture = new System.Globalization.CultureInfo(cultureString);
            for (int i = 1; i <= 12; i++)
            {
                SelectOptionModel option = new SelectOptionModel();
                option.OptionId = i;
                option.OptionString = culture.DateTimeFormat.GetMonthName(i);
                MonthOption.Add(option);
            }
            ViewData["MonthOption"] = MonthOption;

            return View();
        }

        public ActionResult Show(int year, int month, int salesid)
        {
            Sale sales = _db.Sales.Where(c => c.SalesId == salesid).First();

            var checkCall = _db.Calls.Where(c => c.SalesId == salesid).Where(c => c.CallDate.Month == month && c.CallDate.Year == year).OrderBy(c => c.CallDate);

            List<CalendarModel> json = new List<CalendarModel>();
            if (checkCall.Count() > 0)
            {
                foreach (Call item in checkCall)
                {
                    string visitSchedule = (!String.IsNullOrWhiteSpace(item.VisitSchedule.ToString())) ? "<strong>Visit Schedule :</strong> " + item.VisitSchedule.GetValueOrDefault().ToString("dd MMM yyyy HH:mm") + "<br />" : "";

                    CalendarModel call = new CalendarModel();
                    call.title = "Call: " + item.Nasabah.Name;
                    call.start = item.CallDate.ToString("yyyy-MM-dd HH:mm");
                    call.allDay = false;
                    call.color = "#3366CC";
                    call.tooltip = "<div class='tooltip'><strong>Activity :</strong> Call<br />" +
                                    "<strong>Customer :</strong> " + item.Nasabah.Name + "<br />" +
                                    "<strong>Product :</strong> " + item.Product.Name + "<br />" +
                                    "<strong>Reason :</strong> " + item.CallReason.Description + "<br />" +
                                    "<strong>Call Date :</strong> " + item.CallDate.ToString("dd MMM yyyy HH:mm") + "<br />" +
                                    visitSchedule +
                                    "<strong>Description :<br /></strong> " + item.Note +
                                    "</div>";

                    json.Add(call);
                }
            }

            var checkVisit = _db.Visits.Where(c => c.SalesId == salesid).Where(c => c.VisitDate.Month == month && c.VisitDate.Year == year).OrderBy(c => c.VisitDate);

            if (checkVisit.Count() > 0)
            {
                foreach (Visit item in checkVisit)
                {
                    string strDesc = (item.ReasonId != null) ? item.VisitReason.Description : "";
                    CalendarModel visit = new CalendarModel();
                    visit.title = "Visit: " + item.Nasabah.Name;
                    visit.start = item.VisitDate.ToString("yyyy-MM-dd HH:mm");
                    visit.allDay = false;
                    visit.color = "#B7005C";
                    visit.tooltip = "<div class='tooltip'><strong>Activity :</strong> Visit<br />" +
                                    "<strong>Customer :</strong> " + item.Nasabah.Name + "<br />" +
                                    "<strong>Product :</strong> " + item.Product.Name + "<br />" +
                                    "<strong>Reason :</strong> " + strDesc + "<br />" +
                                    "<strong>Visit Date :</strong> " + item.VisitDate.ToString("dd MMM yyyy HH:mm") + "<br />" +
                                    "<strong>Description :<br /></strong> " + item.Note +
                                    "</div>";

                    json.Add(visit);
                }
            }

            // added by Aditia S - 20160801
            JsonResult jResult = new JsonResult { Data = json };
            JavaScriptSerializer jsJson = new JavaScriptSerializer();
            jsJson.MaxJsonLength = 2147483644;
            ViewBag.strJson = jsJson.Serialize(jResult.Data);
            // ---------------- EOF Aditia S

            string cultureString = "id-ID";
            var culture = new System.Globalization.CultureInfo(cultureString);
            string monthName = culture.DateTimeFormat.GetMonthName(month);

            ViewData["year"] = year;
            ViewData["month"] = month;
            ViewData["SalesName"] = sales.Name;
            ViewData["Title"] = "Activity " + sales.Name + " bulan " + monthName + " " + year;

            return View();
        }
    }
}

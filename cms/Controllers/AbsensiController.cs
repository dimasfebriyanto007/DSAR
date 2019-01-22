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
    public class AbsensiController : CommonController
    {
        //
        // GET: /Calendar/
        
        public ActionResult Index()
        {
            if (CommonModel.UserRole() != "SALES") return RedirectToAction("Index", "Home");
            if (CommonModel.BelumAbsen()){ TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }

            Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();

            DateTime StartDate = new DateTime(2016, 1, 1);  // 2013, 4, 15  - Modified by Aditia 20160819
            DateTime EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            int DayInterval = 1;

            List<CalendarModel> json = new List<CalendarModel>();            

            while (StartDate <= EndDate)
            {
                CalendarModel absen = new CalendarModel();
                absen.start = StartDate.ToString("yyyy-MM-dd");

                var checkAbsen = _db.Absensis.Where(c => c.SalesId == sales.SalesId && c.DateAbsen == StartDate && c.Hadir == 1);
                if (checkAbsen.Count() > 0)
                {
                    absen.title = "Hadir";
                    absen.allDay = true;
                    absen.color = "#209316";
                    absen.tooltip = "<div class='tooltip'><strong>Tanggal :</strong> "+StartDate.ToString("dd MMM yyyy")+"<br />" +
                                    "<strong>Absensi :</strong> Hadir" +
                                    "</div>";
                }
                else
                {
                    var checkAbsenLagi = _db.Absensis.Where(c => c.SalesId == sales.SalesId && c.DateAbsen == StartDate && c.Hadir == 0);
                    string status = (checkAbsenLagi.Count() > 0) ? checkAbsenLagi.First().Reason : "Tidak Hadir";
                    string warna = (checkAbsenLagi.Count() > 0) ? "blue" : "#F90000";
                    absen.title = status;
                    absen.allDay = true;
                    absen.color = warna;
                    absen.tooltip = "<div class='tooltip'><strong>Tanggal :</strong> " + StartDate.ToString("dd MMM yyyy") + "<br />" +
                                    "<strong>Absensi :</strong> " + status +
                                    "</div>";
                    absen.url = Url.Content("~/Absensi/Edit/" + StartDate.ToString("yyyyMMdd"));
                }                                
                json.Add(absen);

                StartDate = StartDate.AddDays(DayInterval);
            }            
            
            JsonResult jResult = new JsonResult { Data = json };
            ViewBag.strJson = new JavaScriptSerializer().Serialize(jResult.Data);

            return View();
        }

        public ActionResult Edit(string id)
        {
            if (CommonModel.UserRole() != "SALES") return RedirectToAction("Index", "Home");
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }

            Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();
            
            ViewData["Date"] = id;

            if (id.Length != 8) return RedirectToAction("Index");
            
            int intYear = int.Parse(id.Substring(0,4));
            int intMonth = int.Parse(id.Substring(4,2));
            int intDay = int.Parse(id.Substring(6,2));

            ViewData["strDate"] = intYear + "-" + intMonth + "-" + intDay;

            var checkAbsen = _db.Absensis.Where(c => c.SalesId == sales.SalesId && c.DateAbsen.Year == intYear && c.DateAbsen.Month == intMonth && c.DateAbsen.Day == intDay && c.Hadir == 0);
            if (checkAbsen.Count()>0){
                ViewData["Reason"] = checkAbsen.First().Reason;
            }

            ViewData["Reasons"] = AbsenReason;

            return View();
        }

        //
        // POST: /Video/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Absensi dataToEdit)
        {
            if (CommonModel.UserRole() != "SALES") return RedirectToAction("Index", "Home");
            if (CommonModel.BelumAbsen()) { TempData["alert"] = "1"; return RedirectToAction("Index", "Home"); }
            if (CommonModel.ProfileNotUpdated()) { TempData["alert"] = "2"; return RedirectToAction("Index", "Home"); }

            Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();
            ViewData["Reasons"] = AbsenReason;
            string reqDate = Request["strDate"];
            ViewData["Date"] = reqDate;

            try
            {                

                int intYear = int.Parse(reqDate.Substring(0, 4));
                int intMonth = int.Parse(reqDate.Substring(4, 2));
                int intDay = int.Parse(reqDate.Substring(6, 2));

                var checkAbsen = _db.Absensis.Where(c => c.SalesId == sales.SalesId && c.DateAbsen.Year == intYear && c.DateAbsen.Month == intMonth && c.DateAbsen.Day == intDay && c.Hadir == 0);
                if (checkAbsen.Count() > 0)
                {
                    Absensi absen = checkAbsen.First();
                    absen.Reason = dataToEdit.Reason;
                    absen.DateSubmit = DateTime.Now;
                    _db.ApplyCurrentValues(absen.EntityKey.EntitySetName, absen);
                }
                else
                {
                    dataToEdit.SalesId = sales.SalesId;
                    dataToEdit.Hadir = 0;
                    dataToEdit.DateAbsen = new DateTime(intYear, intMonth, intDay);
                    dataToEdit.DateSubmit = DateTime.Now;
                    _db.AddToAbsensis(dataToEdit);
                }
                
                _db.SaveChanges();
                
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData["Output"] = e.Message;
                return View(dataToEdit);
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

        public ActionResult Show(int month, int year, int salesid)
        {
            Sale sales = _db.Sales.Where(c => c.SalesId == salesid).First();

            DateTime StartDate = new DateTime(year, month, 1);
            DateTime EndDate = new DateTime(year, month, DateTime.DaysInMonth(year,month));
            int DayInterval = 1;

            List<CalendarModel> json = new List<CalendarModel>();

            while (StartDate <= EndDate)
            {
                CalendarModel absen = new CalendarModel();
                absen.start = StartDate.ToString("yyyy-MM-dd");

                var checkAbsen = _db.Absensis.Where(c => c.SalesId == sales.SalesId && c.DateAbsen == StartDate && c.Hadir == 1);
                if (checkAbsen.Count() > 0)
                {
                    absen.title = "Hadir";
                    absen.allDay = true;
                    absen.color = "#209316";
                    absen.tooltip = "<div class='tooltip'><strong>Tanggal :</strong> " + StartDate.ToString("dd MMM yyyy") + "<br />" +
                                    "<strong>Absensi :</strong> Hadir" +
                                    "</div>";
                }
                else
                {
                    var checkAbsenLagi = _db.Absensis.Where(c => c.SalesId == sales.SalesId && c.DateAbsen == StartDate && c.Hadir == 0);
                    string status = (checkAbsenLagi.Count() > 0) ? checkAbsenLagi.First().Reason : "Tidak Hadir";
                    string warna = (checkAbsenLagi.Count() > 0) ? "blue" : "#F90000";
                    absen.title = status;
                    absen.allDay = true;
                    absen.color = warna;
                    absen.tooltip = "<div class='tooltip'><strong>Tanggal :</strong> " + StartDate.ToString("dd MMM yyyy") + "<br />" +
                                    "<strong>Absensi :</strong> " + status +
                                    "</div>";
                }
                json.Add(absen);

                StartDate = StartDate.AddDays(DayInterval);
            }

            JsonResult jResult = new JsonResult { Data = json };
            ViewBag.strJson = new JavaScriptSerializer().Serialize(jResult.Data);

            string cultureString = "id-ID";
            var culture = new System.Globalization.CultureInfo(cultureString);
            string monthName = culture.DateTimeFormat.GetMonthName(month);

            ViewData["year"] = year;
            ViewData["month"] = month;
            ViewData["SalesName"] = sales.Name;
            ViewData["Title"] = "Absensi " + sales.Name + " bulan " + monthName + " " + year;

            return View();
        }

    }
}

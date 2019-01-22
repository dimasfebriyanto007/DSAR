using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using cms.Models;
using System.Globalization;
using System.Net;

namespace cms.Controllers
{
    public class ReportController : CommonController
    {
        //
        // GET: /Calendar/
        
        public ActionResult Index()
        {
            if (CommonModel.UserRole() != "SALES") return RedirectToAction("Index", "Home");
            
            Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();
            ViewData["Role"] = sales.SalesTeam.Name;
            ViewData["Npk"] = sales.Npk;
            ViewData["Name"] = sales.Name;
            ViewData["BranchCode"] = sales.BranchCode;
            ViewData["BranchName"] = sales.Branch.Name;
            
            List<int> YearOption = new List<int>();
            for (int i = DateTime.Now.Year; i >= 2013; i--)
            {
                YearOption.Add(i);
            }
            ViewData["YearOption"] = YearOption;

            List<SelectOptionModel> MonthOption = new List<SelectOptionModel>();
            for (int i = 0; i <= 11; i++)
            {
                SelectOptionModel option = new SelectOptionModel();
                option.OptionId = i+1;
                option.OptionString = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[i];
                MonthOption.Add(option);
            }
            ViewData["MonthOption"] = MonthOption;

            return View();
        }


        public ActionResult list(int month, int year, int export = 0)
        {
            if (CommonModel.UserRole() != "SALES") return RedirectToAction("Index", "Home");            

            DateTime StartDate = new DateTime(year, month, 1);
            DateTime EndDate = StartDate.AddMonths(1).AddDays(-1);
            int DayInterval = 1;
            string msg = "";
            int pembagi = 1000000;
            Sale sales = _db.Sales.Where(c => c.SalesId == user.RelatedId).First();

            if (export == 1)
            {
                ViewData["Role"] = sales.SalesTeam.Name;
                ViewData["Npk"] = sales.Npk;
                ViewData["Name"] = sales.Name;
                ViewData["BranchCode"] = sales.BranchCode;
                ViewData["BranchName"] = sales.Branch.Name;                
                ViewData["Year"] = year;                
                ViewData["Month"] = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[month-1];

                Response.AddHeader("Content-Type", "application/ms-excel");
                Response.AddHeader("Content-Disposition", "attachment; filename=DailySalesReport_"+StartDate.ToString("MMMM-yyyy")+".xls");                
            }

            //Check Absen
            var checkAbsen = _db.Absensis.Where(c => c.SalesId == sales.SalesId && c.DateAbsen.Year == year && c.DateAbsen.Month == month).OrderBy(c => c.DateAbsen);
            Dictionary<string, string> arrayAbsen = new Dictionary<string, string>();
            if (checkAbsen.Count() > 0)
            {                
                foreach (Absensi absensi in checkAbsen)
                {
                    string key = absensi.DateAbsen.ToString("yyyyMMdd");
                    string val = (absensi.Hadir == 1) ? "HADIR" : absensi.Reason;
                    arrayAbsen[key] = val;
                }
            }

            //Check Call
            var checkCall = from c in _db.Calls
                            //where c.SalesId == sales.SalesId && c.CallDate.Year == year && c.CallDate.Month == month && (c.Status == "WARM" || c.Status == "HOT" || c.Status == "BOOKING")
                            where c.SalesId == sales.SalesId && c.CallDate.Year == year && c.CallDate.Month == month
                            group c by new { Year = c.CallDate.Year, Month = c.CallDate.Month, Day = c.CallDate.Day } into grup
                            select
                            new
                            {
                                CallDate = grup.FirstOrDefault().CallDate,
                                Num = grup.Count()
                            };

            Dictionary<string, string> arrayCall = new Dictionary<string, string>();
            if (checkCall.Count() > 0)
            {
                foreach (var call in checkCall)
                {
                    string key = call.CallDate.ToString("yyyyMMdd");
                    string val = call.Num.ToString();
                    arrayCall[key] = val;
                }
            }

            //Check Visit
            var checkVisit = from c in _db.Visits
                             //where c.SalesId == sales.SalesId && c.VisitDate.Year == year && c.VisitDate.Month == month && (c.Status == "WARM" || c.Status == "HOT" || c.Status == "BOOKING")
                             where c.SalesId == sales.SalesId && c.VisitDate.Year == year && c.VisitDate.Month == month && c.ReasonId != null  //&& c.Status != "CANCEL"
                            group c by new { Year = c.VisitDate.Year, Month = c.VisitDate.Month, Day = c.VisitDate.Day } into grup
                            select
                            new
                            {
                                VisitDate = grup.FirstOrDefault().VisitDate,
                                Num = grup.Count()
                            };

            Dictionary<string, string> arrayVisit = new Dictionary<string, string>();
            if (checkVisit.Count() > 0)
            {
                foreach (var visit in checkVisit)
                {
                    string key = visit.VisitDate.ToString("yyyyMMdd");
                    string val = visit.Num.ToString();
                    arrayVisit[key] = val;
                }
            }

            //Check Warm
            var checkWarmCall = from c in _db.Calls
                                where c.SalesId == sales.SalesId && c.CallDate.Year == year && c.CallDate.Month == month && c.Status == "WARM"
                                group c by new { Year = c.CallDate.Year, Month = c.CallDate.Month, Day = c.CallDate.Day } into grup
                                select
                                new
                                {
                                    CallDate = grup.FirstOrDefault().CallDate,
                                    Num = grup.Count()
                                };

            var checkWarmVisit = from c in _db.Visits
                                 where c.SalesId == sales.SalesId && c.VisitDate.Year == year && c.VisitDate.Month == month && c.Status == "WARM"
                                 group c by new { Year = c.VisitDate.Year, Month = c.VisitDate.Month, Day = c.VisitDate.Day } into grup
                                select
                                new
                                {
                                    VisitDate = grup.FirstOrDefault().VisitDate,
                                    Num = grup.Count()
                                };

            Dictionary<string, int> arrayWarm = new Dictionary<string, int>();
            if (checkWarmCall.Count() > 0)
            {
                foreach (var call in checkWarmCall)
                {
                    string key = call.CallDate.ToString("yyyyMMdd");
                    int val = call.Num;
                    arrayWarm[key] = val;
                }
            }
            if (checkWarmVisit.Count() > 0)
            {
                foreach (var visit in checkWarmVisit)
                {
                    string key = visit.VisitDate.ToString("yyyyMMdd");
                    int val = visit.Num;
                    if (arrayWarm.ContainsKey(key))
                    {
                        arrayWarm[key] += val;
                    }
                    else
                    {
                        arrayWarm[key] = val;
                    }
                }
            }

            //Check Hot
            var checkHotCall = from c in _db.Calls
                                where c.SalesId == sales.SalesId && c.CallDate.Year == year && c.CallDate.Month == month && c.Status == "HOT"
                                group c by new { Year = c.CallDate.Year, Month = c.CallDate.Month, Day = c.CallDate.Day } into grup
                                select
                                new
                                {
                                    CallDate = grup.FirstOrDefault().CallDate,
                                    Num = grup.Count()
                                };

            var checkHotVisit = from c in _db.Visits
                                where c.SalesId == sales.SalesId && c.VisitDate.Year == year && c.VisitDate.Month == month && c.Status == "HOT"
                                 group c by new { Year = c.VisitDate.Year, Month = c.VisitDate.Month, Day = c.VisitDate.Day } into grup
                                 select
                                 new
                                 {
                                     VisitDate = grup.FirstOrDefault().VisitDate,
                                     Num = grup.Count()
                                 };

            Dictionary<string, int> arrayHot = new Dictionary<string, int>();
            if (checkHotCall.Count() > 0)
            {
                foreach (var call in checkHotCall)
                {
                    string key = call.CallDate.ToString("yyyyMMdd");
                    int val = call.Num;
                    arrayHot[key] = val;
                }
            }
            if (checkHotVisit.Count() > 0)
            {
                foreach (var visit in checkHotVisit)
                {
                    string key = visit.VisitDate.ToString("yyyyMMdd");
                    int val = visit.Num;
                    if (arrayHot.ContainsKey(key))
                    {
                        arrayHot[key] += val;
                    }
                    else
                    {
                        arrayHot[key] = val;
                    }
                }
            }

            //Check SME Loan
            var checkSME = from c in _db.NasabahProducts
                           where c.SalesId == sales.SalesId && c.LastUpdate.Year == year && c.LastUpdate.Month == month && c.Status == "BOOKING" && c.Product.Code == "SME Loan"
                           group c by new { Year = c.LastUpdate.Year, Month = c.LastUpdate.Month, Day = c.LastUpdate.Day } into grup
                            select
                            new
                            {
                                Date = grup.FirstOrDefault().LastUpdate,
                                Num = grup.Count(),
                                Total = grup.Sum(d => d.Amount)
                            };

            Dictionary<string, string> arraySME = new Dictionary<string, string>();
            Dictionary<string, decimal> arraySMEAmount = new Dictionary<string, decimal>();
            if (checkSME.Count() > 0)
            {
                foreach (var sme in checkSME)
                {
                    string key = sme.Date.ToString("yyyyMMdd");
                    string val = sme.Num.ToString();
                    arraySME[key] = val;
                    arraySMEAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                }
            }


            //Check CA
            var checkCA = from c in _db.NasabahProducts
                           where c.SalesId == sales.SalesId && c.LastUpdate.Year == year && c.LastUpdate.Month == month && c.Status == "BOOKING" && c.Product.Code == "CA"
                           group c by new { Year = c.LastUpdate.Year, Month = c.LastUpdate.Month, Day = c.LastUpdate.Day } into grup
                           select
                           new
                           {
                               Date = grup.FirstOrDefault().LastUpdate,
                               Num = grup.Count(),
                               Total = grup.Sum(d => d.Amount)
                           };

            Dictionary<string, string> arrayCA = new Dictionary<string, string>();
            Dictionary<string, decimal> arrayCAAmount = new Dictionary<string, decimal>();
            if (checkCA.Count() > 0)
            {
                foreach (var ca in checkCA)
                {
                    string key = ca.Date.ToString("yyyyMMdd");
                    string val = ca.Num.ToString();
                    arrayCA[key] = val;
                    arrayCAAmount[key] = (ca.Total.GetValueOrDefault() != 0) ? ca.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check SA
            var checkSA = from c in _db.NasabahProducts
                           where c.SalesId == sales.SalesId && c.LastUpdate.Year == year && c.LastUpdate.Month == month && c.Status == "BOOKING" && c.Product.Code == "SA"
                           group c by new { Year = c.LastUpdate.Year, Month = c.LastUpdate.Month, Day = c.LastUpdate.Day } into grup
                           select
                           new
                           {
                               Date = grup.FirstOrDefault().LastUpdate,
                               Num = grup.Count(),
                               Total = grup.Sum(d => d.Amount)
                           };

            Dictionary<string, string> arraySA = new Dictionary<string, string>();
            Dictionary<string, decimal> arraySAAmount = new Dictionary<string, decimal>();
            if (checkSA.Count() > 0)
            {
                foreach (var sme in checkSA)
                {
                    string key = sme.Date.ToString("yyyyMMdd");
                    string val = sme.Num.ToString();
                    arraySA[key] = val;
                    arraySAAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check SAPAY
            var checkSAPAY = from c in _db.NasabahProducts
                          where c.SalesId == sales.SalesId && c.LastUpdate.Year == year && c.LastUpdate.Month == month && c.Status == "BOOKING" && c.Product.Code == "SA PAY"
                          group c by new { Year = c.LastUpdate.Year, Month = c.LastUpdate.Month, Day = c.LastUpdate.Day } into grup
                          select
                          new
                          {
                              Date = grup.FirstOrDefault().LastUpdate,
                              Num = grup.Count(),
                              Total = grup.Sum(d => d.Amount)
                          };

            Dictionary<string, string> arraySAPAY = new Dictionary<string, string>();
            Dictionary<string, decimal> arraySAPAYAmount = new Dictionary<string, decimal>();
            if (checkSAPAY.Count() > 0)
            {
                foreach (var sme in checkSAPAY)
                {
                    string key = sme.Date.ToString("yyyyMMdd");
                    string val = sme.Num.ToString();
                    arraySAPAY[key] = val;
                    arraySAPAYAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check TD
            var checkTD = from c in _db.NasabahProducts
                             where c.SalesId == sales.SalesId && c.LastUpdate.Year == year && c.LastUpdate.Month == month && c.Status == "BOOKING" && c.Product.Code == "TD"
                             group c by new { Year = c.LastUpdate.Year, Month = c.LastUpdate.Month, Day = c.LastUpdate.Day } into grup
                             select
                             new
                             {
                                 Date = grup.FirstOrDefault().LastUpdate,
                                 Num = grup.Count(),
                                 Total = grup.Sum(d => d.Amount)
                             };

            Dictionary<string, string> arrayTD = new Dictionary<string, string>();
            Dictionary<string, decimal> arrayTDAmount = new Dictionary<string, decimal>();
            if (checkTD.Count() > 0)
            {
                foreach (var sme in checkTD)
                {
                    string key = sme.Date.ToString("yyyyMMdd");
                    string val = sme.Num.ToString();
                    arrayTD[key] = val;
                    arrayTDAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check Referral EB Loan
            var checkRefferal = _db.SalesNasabahs.Where(c => c.RefferalFrom == sales.SalesId);
            Dictionary<string, string> arrayEB = new Dictionary<string, string>();
            Dictionary<string, string> arrayKPM = new Dictionary<string, string>();
            Dictionary<string, string> arrayCC = new Dictionary<string, string>();
            Dictionary<string, string> arrayKPR = new Dictionary<string, string>();
            Dictionary<string, string> arrayBANC = new Dictionary<string, string>();

            if (checkRefferal.Count() > 0)
            {
                SalesNasabah sn = checkRefferal.First();
                var checkEB = from c in _db.NasabahProducts
                                where c.SalesId == sn.SalesId && c.LastUpdate.Year == year && c.LastUpdate.Month == month && c.Status == "BOOKING" && c.Product.Code == "SME Loan"
                                group c by new { Year = c.LastUpdate.Year, Month = c.LastUpdate.Month, Day = c.LastUpdate.Day } into grup
                                select
                                new
                                {
                                    Date = grup.FirstOrDefault().LastUpdate,
                                    Num = grup.Count()
                                };
                    
                if (checkEB.Count() > 0)
                {
                    foreach (var eb in checkEB)
                    {
                        string key = eb.Date.ToString("yyyyMMdd");
                        arrayEB[key] = eb.Num.ToString();                            
                    }
                }
                
                var checkKPM = from c in _db.NasabahProducts
                              where c.SalesId == sn.SalesId && c.LastUpdate.Year == year && c.LastUpdate.Month == month && c.Status == "BOOKING" && c.Product.Code == "KPM"
                              group c by new { Year = c.LastUpdate.Year, Month = c.LastUpdate.Month, Day = c.LastUpdate.Day } into grup
                              select
                              new
                              {
                                  Date = grup.FirstOrDefault().LastUpdate,
                                  Num = grup.Count()
                              };

                if (checkKPM.Count() > 0)
                {
                    foreach (var kpm in checkKPM)
                    {
                        string key = kpm.Date.ToString("yyyyMMdd");
                        arrayKPM[key] = kpm.Num.ToString();
                    }
                }

                var checkCC = from c in _db.NasabahProducts
                               where c.SalesId == sn.SalesId && c.LastUpdate.Year == year && c.LastUpdate.Month == month && c.Status == "BOOKING" && c.Product.Code == "CC"
                               group c by new { Year = c.LastUpdate.Year, Month = c.LastUpdate.Month, Day = c.LastUpdate.Day } into grup
                               select
                               new
                               {
                                   Date = grup.FirstOrDefault().LastUpdate,
                                   Num = grup.Count()
                               };

                if (checkCC.Count() > 0)
                {
                    foreach (var kpm in checkCC)
                    {
                        string key = kpm.Date.ToString("yyyyMMdd");
                        arrayCC[key] = kpm.Num.ToString();
                    }
                }

                var checkKPR = from c in _db.NasabahProducts
                              where c.SalesId == sn.SalesId && c.LastUpdate.Year == year && c.LastUpdate.Month == month && c.Status == "BOOKING" && c.Product.Code == "KPR"
                              group c by new { Year = c.LastUpdate.Year, Month = c.LastUpdate.Month, Day = c.LastUpdate.Day } into grup
                              select
                              new
                              {
                                  Date = grup.FirstOrDefault().LastUpdate,
                                  Num = grup.Count()
                              };

                if (checkKPR.Count() > 0)
                {
                    foreach (var kpm in checkKPR)
                    {
                        string key = kpm.Date.ToString("yyyyMMdd");
                        arrayKPR[key] = kpm.Num.ToString();
                    }
                }

                var checkBANC = from c in _db.NasabahProducts
                                where c.SalesId == sn.SalesId && c.LastUpdate.Year == year && c.LastUpdate.Month == month && c.Status == "BOOKING" && c.Product.Code == "INS"
                              group c by new { Year = c.LastUpdate.Year, Month = c.LastUpdate.Month, Day = c.LastUpdate.Day } into grup
                              select
                              new
                              {
                                  Date = grup.FirstOrDefault().LastUpdate,
                                  Num = grup.Count()
                              };

                if (checkBANC.Count() > 0)
                {
                    foreach (var kpm in checkBANC)
                    {
                        string key = kpm.Date.ToString("yyyyMMdd");
                        arrayBANC[key] = kpm.Num.ToString();
                    }
                }
                
            }

            int i = 1;
            int w = 1;
            int numCallTotal = 0;
            int numVisitTotal = 0;
            int numWarmTotal = 0;
            int numHotTotal = 0;
            int numSMETotal = 0;            
            decimal numSMEAmountTotal = 0;
            int numCATotal = 0;
            decimal numCAAmountTotal = 0;
            int numSATotal = 0;
            decimal numSAAmountTotal = 0;
            int numSAPAYTotal = 0;
            decimal numSAPAYAmountTotal = 0;
            int numTDTotal = 0;
            decimal numTDAmountTotal = 0;
            int numExSAPAY = 0;
            int numExSAPAYTotal = 0;
            int numEBTotal = 0;
            int numKPMTotal = 0;
            int numCCTotal = 0;
            int numKPRTotal = 0;
            int numBANCTotal = 0;
            int numREF = 0;
            int numREFTotal = 0;
            int numCIF = 0;
            int numCIFTotal = 0;

            int numCallGrandTotal = 0;
            int numVisitGrandTotal = 0;
            int numWarmGrandTotal = 0;
            int numHotGrandTotal = 0;
            int numSMEGrandTotal = 0;
            decimal numSMEAmountGrandTotal = 0;
            int numCAGrandTotal = 0;
            decimal numCAAmountGrandTotal = 0;
            int numSAGrandTotal = 0;
            decimal numSAAmountGrandTotal = 0;
            int numSAPAYGrandTotal = 0;
            decimal numSAPAYAmountGrandTotal = 0;
            int numTDGrandTotal = 0;
            decimal numTDAmountGrandTotal = 0;            
            int numExSAPAYGrandTotal = 0;
            int numEBGrandTotal = 0;
            int numKPMGrandTotal = 0;
            int numCCGrandTotal = 0;
            int numKPRGrandTotal = 0;
            int numBANCGrandTotal = 0;
            int numREFGrandTotal = 0;
            int numCIFGrandTotal = 0;

            while (StartDate <= EndDate)
            {
                int last = i+7;
                int rowspan = (last>EndDate.Day) ? EndDate.Day - i + 2 : 8;

                string strRowspan = (i % 7 == 1) ? "<td rowspan=\"" + rowspan + "\" style=\"text-align:center;font-weight:bold\">W" + w + "</td>" : "";
                string strDate = StartDate.ToString("dd MMMM yyyy");
                string key = StartDate.ToString("yyyyMMdd");

                string absenStatus = (arrayAbsen.ContainsKey(key)) ? arrayAbsen[key] : "TIDAK HADIR";
                
                string numCall = (arrayCall.ContainsKey(key)) ? arrayCall[key] : "&nbsp;";                
                if (arrayCall.ContainsKey(key))
                {
                    numCallTotal += int.Parse(arrayCall[key]);
                    numCallGrandTotal += int.Parse(arrayCall[key]);
                }
                
                string numVisit = (arrayVisit.ContainsKey(key)) ? arrayVisit[key] : "&nbsp;";
                if (arrayVisit.ContainsKey(key))
                {
                    numVisitTotal += int.Parse(arrayVisit[key]);
                    numVisitGrandTotal += int.Parse(arrayVisit[key]);
                }

                int numWarm = (arrayWarm.ContainsKey(key)) ? arrayWarm[key] : 0;
                string strWarm = (numWarm != 0) ? numWarm.ToString() : "&nbsp;";
                if (arrayWarm.ContainsKey(key))
                {
                    numWarmTotal += arrayWarm[key];
                    numWarmGrandTotal += arrayWarm[key];
                }

                int numHot = (arrayHot.ContainsKey(key)) ? arrayHot[key] : 0;
                string strHot = (numHot != 0) ? numHot.ToString() : "&nbsp;";
                if (arrayHot.ContainsKey(key))
                {
                    numHotTotal += arrayHot[key];
                    numHotGrandTotal += arrayHot[key];
                }

                string numSME = (arraySME.ContainsKey(key)) ? arraySME[key] : "&nbsp;";
                if (arraySME.ContainsKey(key))
                {
                    numSMETotal += int.Parse(arraySME[key]);
                    numSMEGrandTotal += int.Parse(arraySME[key]);
                    numCIF++;
                    numCIFTotal++;
                    numCIFGrandTotal++;
                }

                decimal numSMEAmount = (arraySMEAmount.ContainsKey(key)) ? arraySMEAmount[key] : 0;
                string strSMEAmount = (numSMEAmount != 0) ? numSMEAmount.ToString("F0") : "&nbsp;";
                if (arraySMEAmount.ContainsKey(key))
                {
                    numSMEAmountTotal += arraySMEAmount[key];
                    numSMEAmountGrandTotal += arraySMEAmount[key];
                }

                string numCA = (arrayCA.ContainsKey(key)) ? arrayCA[key] : "&nbsp;";
                if (arrayCA.ContainsKey(key))
                {
                    numCATotal += int.Parse(arrayCA[key]);
                    numCAGrandTotal += int.Parse(arrayCA[key]);
                    numExSAPAY += int.Parse(arrayCA[key]);
                    numExSAPAYTotal += int.Parse(arrayCA[key]);
                    numExSAPAYGrandTotal += int.Parse(arrayCA[key]);
                    numCIF++;
                    numCIFTotal++;
                    numCIFGrandTotal++;
                }

                decimal numCAAmount = (arrayCAAmount.ContainsKey(key)) ? arrayCAAmount[key] : 0;
                string strCAAmount = (numCAAmount != 0) ? numCAAmount.ToString("F0") : "&nbsp;";
                if (arrayCAAmount.ContainsKey(key))
                {
                    numCAAmountTotal += arrayCAAmount[key];
                    numCAAmountGrandTotal += arrayCAAmount[key];
                }

                string numSA = (arraySA.ContainsKey(key)) ? arraySA[key] : "&nbsp;";
                if (arraySA.ContainsKey(key))
                {
                    numSATotal += int.Parse(arraySA[key]);
                    numSAGrandTotal += int.Parse(arraySA[key]);
                    numExSAPAY += int.Parse(arraySA[key]);
                    numExSAPAYTotal += int.Parse(arraySA[key]);
                    numExSAPAYGrandTotal += int.Parse(arraySA[key]);
                    numCIF++;
                    numCIFTotal++;
                    numCIFGrandTotal++;
                }

                decimal numSAAmount = (arraySAAmount.ContainsKey(key)) ? arraySAAmount[key] : 0;
                string strSAAmount = (numSAAmount != 0) ? numSAAmount.ToString("F0") : "&nbsp;";
                if (arraySAAmount.ContainsKey(key))
                {
                    numSAAmountTotal += arraySAAmount[key];
                    numSAAmountGrandTotal += arraySAAmount[key];
                }

                string numSAPAY = (arraySAPAY.ContainsKey(key)) ? arraySAPAY[key] : "&nbsp;";
                if (arraySAPAY.ContainsKey(key))
                {
                    numSAPAYTotal += int.Parse(arraySAPAY[key]);
                    numSAPAYGrandTotal += int.Parse(arraySAPAY[key]);
                    numCIF++;
                    numCIFTotal++;
                    numCIFGrandTotal++;
                }

                decimal numSAPAYAmount = (arraySAPAYAmount.ContainsKey(key)) ? arraySAPAYAmount[key] : 0;
                string strSAPAYAmount = (numSAPAYAmount != 0) ? numSAPAYAmount.ToString("F0") : "&nbsp;";
                if (arraySAPAYAmount.ContainsKey(key))
                {
                    numSAPAYAmountTotal += arraySAPAYAmount[key];
                    numSAPAYAmountGrandTotal += arraySAPAYAmount[key];
                }

                string numTD = (arrayTD.ContainsKey(key)) ? arrayTD[key] : "&nbsp;";
                if (arrayTD.ContainsKey(key))
                {
                    numTDTotal += int.Parse(arrayTD[key]);
                    numTDGrandTotal += int.Parse(arrayTD[key]);
                    numExSAPAY += int.Parse(arrayTD[key]);
                    numExSAPAYTotal += int.Parse(arrayTD[key]);
                    numExSAPAYGrandTotal += int.Parse(arrayTD[key]);
                    numCIF++;
                    numCIFTotal++;
                    numCIFGrandTotal++;
                }

                decimal numTDAmount = (arrayTDAmount.ContainsKey(key)) ? arrayTDAmount[key] : 0;
                string strTDAmount = (numTDAmount != 0) ? numTDAmount.ToString("F0") : "&nbsp;";
                if (arrayTDAmount.ContainsKey(key))
                {
                    numTDAmountTotal += arrayTDAmount[key];
                    numTDAmountGrandTotal += arrayTDAmount[key];
                }

                string numEB = (arrayEB.ContainsKey(key)) ? arrayEB[key] : "&nbsp;";
                if (arrayEB.ContainsKey(key))
                {
                    numEBTotal += int.Parse(arrayEB[key]);
                    numEBGrandTotal += int.Parse(arrayEB[key]);
                    numREF += int.Parse(arrayEB[key]);
                    numREFTotal += int.Parse(arrayEB[key]);
                    numREFGrandTotal += int.Parse(arrayEB[key]);
                    numCIF++;
                    numCIFTotal++;
                    numCIFGrandTotal++;
                }

                string numKPM = (arrayKPM.ContainsKey(key)) ? arrayKPM[key] : "&nbsp;";
                if (arrayKPM.ContainsKey(key))
                {
                    numKPMTotal += int.Parse(arrayKPM[key]);
                    numKPMGrandTotal += int.Parse(arrayKPM[key]);
                    numREF += int.Parse(arrayKPM[key]);
                    numREFTotal += int.Parse(arrayKPM[key]);
                    numREFGrandTotal += int.Parse(arrayKPM[key]);
                    numCIF++;
                    numCIFTotal++;
                    numCIFGrandTotal++;
                }

                string numCC = (arrayCC.ContainsKey(key)) ? arrayCC[key] : "&nbsp;";
                if (arrayCC.ContainsKey(key))
                {
                    numCCTotal += int.Parse(arrayCC[key]);
                    numCCGrandTotal += int.Parse(arrayCC[key]);
                    numREF += int.Parse(arrayCC[key]);
                    numREFTotal += int.Parse(arrayCC[key]);
                    numREFGrandTotal += int.Parse(arrayCC[key]);
                    numCIF++;
                    numCIFTotal++;
                    numCIFGrandTotal++;
                }

                string numKPR = (arrayKPR.ContainsKey(key)) ? arrayKPR[key] : "&nbsp;";
                if (arrayKPR.ContainsKey(key))
                {
                    numKPRTotal += int.Parse(arrayKPR[key]);
                    numKPRGrandTotal += int.Parse(arrayKPR[key]);
                    numREF += int.Parse(arrayKPR[key]);
                    numREFTotal += int.Parse(arrayKPR[key]);
                    numREFGrandTotal += int.Parse(arrayKPR[key]);
                    numCIF++;
                    numCIFTotal++;
                    numCIFGrandTotal++;
                }

                string numBANC = (arrayBANC.ContainsKey(key)) ? arrayBANC[key] : "&nbsp;";
                if (arrayBANC.ContainsKey(key))
                {
                    numBANCTotal += int.Parse(arrayBANC[key]);
                    numBANCGrandTotal += int.Parse(arrayBANC[key]);
                    numREF += int.Parse(arrayBANC[key]);
                    numREFTotal += int.Parse(arrayBANC[key]);
                    numREFGrandTotal += int.Parse(arrayBANC[key]);
                    numCIF++;
                    numCIFTotal++;
                    numCIFGrandTotal++;
                }

                msg += "<tr>" +
                        strRowspan +
                        "<td style=\"text-align:center\">"+strDate+"</td>" +
                        "<td class=\"orange\" style=\"text-align:center\">"+absenStatus+"</td>" +
                        "<td style=\"text-align:center\">" + numCall + "</td>" +
                        "<td style=\"text-align:center\">" + numVisit + "</td>" +
                        "<td style=\"text-align:center\">" + strWarm + "</td>" +
                        "<td style=\"text-align:center\">" + strHot + "</td>" +
                        "<td style=\"text-align:center\">" + numSME + "</td>" +
                        "<td style=\"text-align:center\">" + strSMEAmount + "</td>" +
                        "<td style=\"text-align:center\">" + numCA + "</td>" +
                        "<td style=\"text-align:center\">" + strCAAmount + "</td>" +
                        "<td style=\"text-align:center\">" + numSA + "</td>" +
                        "<td style=\"text-align:center\">" + strSAAmount + "</td>" +
                        "<td style=\"text-align:center\">" + numSAPAY + "</td>" +
                        "<td style=\"text-align:center\">" + strSAPAYAmount + "</td>" +
                        "<td style=\"text-align:center\">" + numTD + "</td>" +
                        "<td style=\"text-align:center\">" + strTDAmount + "</td>" +
                        "<td style=\"text-align:center\">" + numExSAPAY + "</td>" +
                        "<td style=\"text-align:center\">" + numEB + "</td>" +
                        "<td style=\"text-align:center\">" + numKPM + "</td>" +
                        "<td style=\"text-align:center\">" + numCC + "</td>" +
                        "<td style=\"text-align:center\">" + numKPR + "</td>" +
                        "<td style=\"text-align:center\">" + numBANC + "</td>" +
                        "<td style=\"text-align:center\">" + numREF + "</td>" +
                        "<td style=\"text-align:center\">" + numCIF + "</td>" +
                      "</tr>";

                if (i % 7 == 0 || i == EndDate.Day)
                {
                    msg += "<tr>" +
                        "<td style=\"text-align:center;font-weigth:bold;background:#ccc;color:#000\" colspan=\"2\">TOTAL</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numCallTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numVisitTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numWarmTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numHotTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numSMETotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numSMEAmountTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numCATotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numCAAmountTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numSATotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numSAAmountTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numSAPAYTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numSAPAYAmountTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numTDTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numTDAmountTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numExSAPAYTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numEBTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numKPMTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numCCTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numKPRTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numBANCTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numREFTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numCIFTotal + "</td>" +
                      "</tr>";
                }

                StartDate = StartDate.AddDays(DayInterval);
                i++;
                numExSAPAY = 0;
                numREF = 0;
                numCIF = 0;
                if (i % 7 == 1)
                {
                    numCallTotal = 0;
                    numVisitTotal = 0;
                    numWarmTotal = 0;
                    numHotTotal = 0;
                    numSMETotal = 0;
                    numSMEAmountTotal = 0;
                    numCATotal = 0;
                    numCAAmountTotal = 0;
                    numSATotal = 0;
                    numSAAmountTotal = 0;
                    numSAPAYTotal = 0;
                    numSAPAYAmountTotal = 0;
                    numTDTotal = 0;
                    numTDAmountTotal = 0;
                    numEBTotal = 0;
                    numKPMTotal = 0;
                    numCCTotal = 0;
                    numKPRTotal = 0;
                    numBANCTotal = 0;
                    numREFTotal = 0;
                    numCIFTotal = 0;
                    w++;
                }
            }

            msg += "<tr><td colspan=\"25\" style=\"border-right:none;border-left:none;\"></td></tr>";
            msg += "<tr style=\"border:2px solid #000\">" +
                        "<td style=\"text-align:center;font-weigth:bold;background:#ccc;color:#000\" colspan=\"3\">GRAND TOTAL</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numCallGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numVisitGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numWarmGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numHotGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numSMEGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numSMEAmountGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numCAGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numCAAmountGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numSAGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numSAAmountGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numSAPAYGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numSAPAYAmountGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numTDGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numTDAmountGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numExSAPAYGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numEBGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numKPMGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numCCGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numKPRGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numBANCGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numREFGrandTotal + "</td>" +
                        "<td style=\"text-align:center;background:#ccc\">" + numCIFGrandTotal + "</td>" +
                      "</tr>";

            msg += "</table>";
            ViewBag.Output = msg;
            string view = (export == 1) ? "list_export" : "list";

            return View(view);
        }

    }
}

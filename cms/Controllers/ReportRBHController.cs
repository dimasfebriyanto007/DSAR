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
    public class ReportRBHController : CommonController
    {
        //
        // GET: /Calendar/
        
        public ActionResult Index()
        {
            if (CommonModel.UserRole() != "RBH") return RedirectToAction("Index", "Home");

            RBH rbh = _db.RBHs.Where(c => c.RbhId == user.RelatedId).First();

            ViewData["Teams"] = _db.SalesTeams.OrderBy(c => c.Name).ToList();
            ViewData["Branchs"] = _db.Branchs.Where(c => c.Area.RegionCode == rbh.RegionCode).OrderBy(c => c.Name).ToList();
            ViewData["date"] = DateTime.Now.ToString("dd-MM-yyyy");

            return View();
        }
        
        public ActionResult list(string date, string branch, int team, int export = 0)
        {
            if (CommonModel.UserRole() != "RBH") return RedirectToAction("Index", "Home");

            string[] arrDate = date.Split('-');
            if (arrDate.Count() != 3)
            {
                Response.Write("<div class=\"arial\" style=\"text-align:center;padding:10px 0;\">Invalid date</div>");
                Response.End();
            }

            ViewData["BranchCode"] = branch;
            ViewData["Date"] = date;

            var checkBranch = _db.Branchs.Where(c => c.BranchCode == branch);
            if (checkBranch.Count() > 0)
            {
                Branch br = checkBranch.First();
                ViewData["BranchName"] = br.Name;
            }
            else
            {
                Response.Write("<div class=\"arial\" style=\"text-align:center;padding:10px 0;\">Invalid branch</div>");
                Response.End();
            }

            var checkTeam = _db.SalesTeams.Where(c => c.TeamId == team);
            if (checkTeam.Count() > 0)
            {
                SalesTeam st = checkTeam.First();
                ViewData["Role"] = st.Name;
            }
            else
            {
                Response.Write("<div class=\"arial\" style=\"text-align:center;padding:10px 0;\">Invalid sales role</div>");
                Response.End();
            }

            int intDate = int.Parse(arrDate[0]);
            int intMonth = int.Parse(arrDate[1]);
            int intYear = int.Parse(arrDate[2]);

            string msg = "";
            int pembagi = 1000000;
            
            if (export == 1)
            {                
                Response.AddHeader("Content-Type", "application/ms-excel");
                Response.AddHeader("Content-Disposition", "attachment; filename=DailySalesReport_"+date+".xls");                
            }

            var checkSales = _db.Sales.Where(c => c.BranchCode == branch && c.TeamId == team).OrderBy(c => c.Name);
            var SalesList = checkSales.Select(c => c.SalesId).ToList();            

            //Check Call
            var checkCall = from c in _db.Calls
                            //where SalesList.Contains(c.SalesId) && c.CallDate.Year == intYear && c.CallDate.Month == intMonth && c.CallDate.Day == intDate && (c.Status == "WARM" || c.Status == "HOT" || c.Status == "BOOKING")
                            where SalesList.Contains(c.SalesId) && c.CallDate.Year == intYear && c.CallDate.Month == intMonth && c.CallDate.Day == intDate
                            group c by c.SalesId into grup
                            select
                            new
                            {
                                SalesId = grup.FirstOrDefault().SalesId,
                                Num = grup.Count()
                            };  

            Dictionary<int, string> arrayCall = new Dictionary<int, string>();
            if (checkCall.Count() > 0)
            {
                foreach (var call in checkCall)
                {
                    int key = call.SalesId;
                    string val = call.Num.ToString();
                    arrayCall[key] = val;
                }
            }
            int numCallTotal = 0;

            //Check Visit
            var checkVisit = from c in _db.Visits
                            //where SalesList.Contains(c.SalesId) && c.VisitDate.Year == intYear && c.VisitDate.Month == intMonth && c.VisitDate.Day == intDate && (c.Status == "WARM" || c.Status == "HOT" || c.Status == "BOOKING")
                             where SalesList.Contains(c.SalesId) && c.VisitDate.Year == intYear && c.VisitDate.Month == intMonth && c.VisitDate.Day == intDate && c.ReasonId != null // && c.Status != "CANCEL"
                            group c by c.SalesId into grup
                            select
                            new
                            {
                                SalesId = grup.FirstOrDefault().SalesId,
                                Num = grup.Count()
                            };

            Dictionary<int, string> arrayVisit = new Dictionary<int, string>();
            if (checkVisit.Count() > 0)
            {
                foreach (var call in checkVisit)
                {
                    int key = call.SalesId;
                    string val = call.Num.ToString();
                    arrayVisit[key] = val;
                }
            }
            int numVisitTotal = 0;

            //Check Warm
            var checkWarmCall = from c in _db.Calls
                                where SalesList.Contains(c.SalesId) && c.CallDate.Year == intYear && c.CallDate.Month == intMonth && c.CallDate.Day == intDate && c.Status == "WARM"
                                group c by c.SalesId into grup
                                select
                                new
                                {
                                    SalesId = grup.FirstOrDefault().SalesId,
                                    Num = grup.Count()
                                };

            var checkWarmVisit = from c in _db.Visits
                                 where SalesList.Contains(c.SalesId) && c.VisitDate.Year == intYear && c.VisitDate.Month == intMonth && c.VisitDate.Day == intDate && c.Status == "WARM"
                                 group c by c.SalesId into grup
                                 select
                                 new
                                 {
                                     SalesId = grup.FirstOrDefault().SalesId,
                                     Num = grup.Count()
                                 };

            Dictionary<int, int> arrayWarm = new Dictionary<int, int>();
            if (checkWarmCall.Count() > 0)
            {
                foreach (var call in checkWarmCall)
                {
                    int key = call.SalesId;
                    int val = call.Num;
                    arrayWarm[key] = val;
                }
            }
            if (checkWarmVisit.Count() > 0)
            {
                foreach (var visit in checkWarmVisit)
                {
                    int key = visit.SalesId;
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
            int numWarmTotal = 0;

            //Check Hot
            var checkHotCall = from c in _db.Calls
                               where SalesList.Contains(c.SalesId) && c.CallDate.Year == intYear && c.CallDate.Month == intMonth && c.CallDate.Day == intDate && c.Status == "HOT"
                               group c by c.SalesId into grup
                               select
                               new
                               {
                                   SalesId = grup.FirstOrDefault().SalesId,
                                   Num = grup.Count()
                               };

            var checkHotVisit = from c in _db.Visits
                                where SalesList.Contains(c.SalesId) && c.VisitDate.Year == intYear && c.VisitDate.Month == intMonth && c.VisitDate.Day == intDate && c.Status == "HOT"
                                group c by c.SalesId into grup
                                select
                                new
                                {
                                    SalesId = grup.FirstOrDefault().SalesId,
                                    Num = grup.Count()
                                };

            Dictionary<int, int> arrayHot = new Dictionary<int, int>();
            if (checkHotCall.Count() > 0)
            {
                foreach (var call in checkHotCall)
                {
                    int key = call.SalesId;
                    int val = call.Num;
                    arrayHot[key] = val;
                }
            }
            if (checkHotVisit.Count() > 0)
            {
                foreach (var visit in checkHotVisit)
                {
                    int key = visit.SalesId;
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
            int numHotTotal = 0;

            //Check SME Loan
            var checkSME = from c in _db.NasabahProducts
                           where (c.SalesId != null && SalesList.Contains(c.SalesId.Value)) && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.LastUpdate.Day == intDate && c.Status == "BOOKING" && c.Product.Code == "SME Loan"
                           group c by c.SalesId into grup
                           select
                           new
                           {
                               SalesId = grup.FirstOrDefault().SalesId,
                               Num = grup.Count(),
                               Total = grup.Sum(d => d.Amount)
                           };

            Dictionary<int, string> arraySME = new Dictionary<int, string>();
            Dictionary<int, decimal> arraySMEAmount = new Dictionary<int, decimal>();
            if (checkSME.Count() > 0)
            {
                foreach (var sme in checkSME)
                {
                    int key = sme.SalesId.GetValueOrDefault();
                    string val = sme.Num.ToString();
                    arraySME[key] = val;
                    arraySMEAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                }
            }


            //Check CA
            var checkCA = from c in _db.NasabahProducts
                          where SalesList.Contains(c.SalesId.Value) && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.LastUpdate.Day == intDate && c.Status == "BOOKING" && c.Product.Code == "CA"
                          group c by c.SalesId into grup
                          select
                          new
                          {
                              SalesId = grup.FirstOrDefault().SalesId,
                              Num = grup.Count(),
                              Total = grup.Sum(d => d.Amount)
                          };

            Dictionary<int, string> arrayCA = new Dictionary<int, string>();
            Dictionary<int, decimal> arrayCAAmount = new Dictionary<int, decimal>();
            if (checkCA.Count() > 0)
            {
                foreach (var ca in checkCA)
                {
                    int key = ca.SalesId.GetValueOrDefault();
                    string val = ca.Num.ToString();
                    arrayCA[key] = val;
                    arrayCAAmount[key] = (ca.Total.GetValueOrDefault() != 0) ? ca.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check SA
            var checkSA = from c in _db.NasabahProducts
                          where SalesList.Contains(c.SalesId.Value) && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.LastUpdate.Day == intDate && c.Status == "BOOKING" && c.Product.Code == "SA"
                          group c by c.SalesId into grup
                          select
                          new
                          {
                              SalesId = grup.FirstOrDefault().SalesId,
                              Num = grup.Count(),
                              Total = grup.Sum(d => d.Amount)
                          };

            Dictionary<int, string> arraySA = new Dictionary<int, string>();
            Dictionary<int, decimal> arraySAAmount = new Dictionary<int, decimal>();
            if (checkSA.Count() > 0)
            {
                foreach (var sme in checkSA)
                {
                    int key = sme.SalesId.GetValueOrDefault();
                    string val = sme.Num.ToString();
                    arraySA[key] = val;
                    arraySAAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check SAPAY
            var checkSAPAY = from c in _db.NasabahProducts
                             where SalesList.Contains(c.SalesId.Value) && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.LastUpdate.Day == intDate && c.Status == "BOOKING" && c.Product.Code == "SA PAY"
                             group c by c.SalesId into grup
                             select
                             new
                             {
                                 SalesId = grup.FirstOrDefault().SalesId,
                                 Num = grup.Count(),
                                 Total = grup.Sum(d => d.Amount)
                             };

            Dictionary<int, string> arraySAPAY = new Dictionary<int, string>();
            Dictionary<int, decimal> arraySAPAYAmount = new Dictionary<int, decimal>();
            if (checkSAPAY.Count() > 0)
            {
                foreach (var sme in checkSAPAY)
                {
                    int key = sme.SalesId.GetValueOrDefault();
                    string val = sme.Num.ToString();
                    arraySAPAY[key] = val;
                    arraySAPAYAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check TD
            var checkTD = from c in _db.NasabahProducts
                          where SalesList.Contains(c.SalesId.Value) && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.LastUpdate.Day == intDate && c.Status == "BOOKING" && c.Product.Code == "TD"
                          group c by c.SalesId into grup
                          select
                          new
                          {
                              SalesId = grup.FirstOrDefault().SalesId,
                              Num = grup.Count(),
                              Total = grup.Sum(d => d.Amount)
                          };

            Dictionary<int, string> arrayTD = new Dictionary<int, string>();
            Dictionary<int, decimal> arrayTDAmount = new Dictionary<int, decimal>();
            if (checkTD.Count() > 0)
            {
                foreach (var sme in checkTD)
                {
                    int key = sme.SalesId.GetValueOrDefault();
                    string val = sme.Num.ToString();
                    arrayTD[key] = val;
                    arrayTDAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check Referral EB Loan
            var checkRefferal = _db.SalesNasabahs.Where(c => SalesList.Contains(c.RefferalFrom.Value));
            Dictionary<int, int> arrayEB = new Dictionary<int, int>();
            Dictionary<int, int> arrayKPM = new Dictionary<int, int>();
            Dictionary<int, int> arrayCC = new Dictionary<int, int>();
            Dictionary<int, int> arrayKPR = new Dictionary<int, int>();
            Dictionary<int, int> arrayBANC = new Dictionary<int, int>();

            if (checkRefferal.Count() > 0)
            {
                foreach (SalesNasabah sn in checkRefferal)
                {
                    var checkEB = from c in _db.NasabahProducts
                                  where c.SalesId == sn.SalesId && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.LastUpdate.Day == intDate && c.Status == "BOOKING" && c.Product.Code == "SME Loan"
                                  select c;

                    if (checkEB.Count() > 0)
                    {
                        int key = sn.RefferalFrom.GetValueOrDefault();
                        arrayEB[key] = (arrayEB.ContainsKey(key)) ? arrayEB[key] + 1 : 1;
                    }

                    var checkKPM = from c in _db.NasabahProducts
                                   where c.SalesId == sn.SalesId && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.LastUpdate.Day == intDate && c.Status == "BOOKING" && c.Product.Code == "KPM"
                                   select c;

                    if (checkKPM.Count() > 0)
                    {
                        int key = sn.RefferalFrom.GetValueOrDefault();
                        arrayKPM[key] = (arrayKPM.ContainsKey(key)) ? arrayKPM[key] + 1 : 1;
                    }

                    var checkCC = from c in _db.NasabahProducts
                                  where c.SalesId == sn.SalesId && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.LastUpdate.Day == intDate && c.Status == "BOOKING" && c.Product.Code == "CC"
                                  select c;

                    if (checkCC.Count() > 0)
                    {
                        int key = sn.RefferalFrom.GetValueOrDefault();
                        arrayCC[key] = (arrayCC.ContainsKey(key)) ? arrayCC[key] + 1 : 1;                        
                    }

                    var checkKPR = from c in _db.NasabahProducts
                                   where c.SalesId == sn.SalesId && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.LastUpdate.Day == intDate && c.Status == "BOOKING" && c.Product.Code == "KPR"
                                   select c;

                    if (checkKPR.Count() > 0)
                    {
                        int key = sn.RefferalFrom.GetValueOrDefault();
                        arrayKPR[key] = (arrayKPR.ContainsKey(key)) ? arrayKPR[key] + 1 : 1;
                    }

                    var checkBANC = from c in _db.NasabahProducts
                                    where c.SalesId == sn.SalesId && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.LastUpdate.Day == intDate && c.Status == "BOOKING" && c.Product.Code == "INS"
                                    select c;

                    if (checkBANC.Count() > 0)
                    {
                        int key = sn.RefferalFrom.GetValueOrDefault();
                        arrayBANC[key] = (arrayBANC.ContainsKey(key)) ? arrayBANC[key] + 1 : 1;
                    }
                }
            }


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

            //loop start

            foreach (Sale item in checkSales)
            {
                int key = item.SalesId;

                numExSAPAY = 0;
                numREF = 0;
                numCIF = 0;

                string numCall = (arrayCall.ContainsKey(key)) ? arrayCall[key] : "&nbsp;";
                if (arrayCall.ContainsKey(key))
                {
                    numCallTotal += int.Parse(arrayCall[key]);                    
                }

                string numVisit = (arrayVisit.ContainsKey(key)) ? arrayVisit[key] : "&nbsp;";
                if (arrayVisit.ContainsKey(key))
                {
                    numVisitTotal += int.Parse(arrayVisit[key]);
                }

                int numWarm = (arrayWarm.ContainsKey(key)) ? arrayWarm[key] : 0;
                string strWarm = (numWarm != 0) ? numWarm.ToString() : "&nbsp;";
                if (arrayWarm.ContainsKey(key))
                {
                    numWarmTotal += arrayWarm[key];
                }

                int numHot = (arrayHot.ContainsKey(key)) ? arrayHot[key] : 0;
                string strHot = (numHot != 0) ? numHot.ToString() : "&nbsp;";
                if (arrayHot.ContainsKey(key))
                {
                    numHotTotal += arrayHot[key];
                }

                string numSME = (arraySME.ContainsKey(key)) ? arraySME[key] : "&nbsp;";
                if (arraySME.ContainsKey(key))
                {
                    numSMETotal += int.Parse(arraySME[key]);
                    numCIF++;
                    numCIFTotal++;                    
                }

                decimal numSMEAmount = (arraySMEAmount.ContainsKey(key)) ? arraySMEAmount[key] : 0;
                string strSMEAmount = (numSMEAmount != 0) ? numSMEAmount.ToString("F0") : "&nbsp;";
                if (arraySMEAmount.ContainsKey(key))
                {
                    numSMEAmountTotal += arraySMEAmount[key];                    
                }

                string numCA = (arrayCA.ContainsKey(key)) ? arrayCA[key] : "&nbsp;";
                if (arrayCA.ContainsKey(key))
                {
                    numCATotal += int.Parse(arrayCA[key]);
                    numExSAPAY += int.Parse(arrayCA[key]);
                    numExSAPAYTotal += int.Parse(arrayCA[key]);
                    numCIF++;
                    numCIFTotal++;                    
                }

                decimal numCAAmount = (arrayCAAmount.ContainsKey(key)) ? arrayCAAmount[key] : 0;
                string strCAAmount = (numCAAmount != 0) ? numCAAmount.ToString("F0") : "&nbsp;";
                if (arrayCAAmount.ContainsKey(key))
                {
                    numCAAmountTotal += arrayCAAmount[key];
                }

                string numSA = (arraySA.ContainsKey(key)) ? arraySA[key] : "&nbsp;";
                if (arraySA.ContainsKey(key))
                {
                    numSATotal += int.Parse(arraySA[key]);
                    numExSAPAY += int.Parse(arraySA[key]);
                    numExSAPAYTotal += int.Parse(arraySA[key]);
                    numCIF++;
                    numCIFTotal++;
                }

                decimal numSAAmount = (arraySAAmount.ContainsKey(key)) ? arraySAAmount[key] : 0;
                string strSAAmount = (numSAAmount != 0) ? numSAAmount.ToString("F0") : "&nbsp;";
                if (arraySAAmount.ContainsKey(key))
                {
                    numSAAmountTotal += arraySAAmount[key];
                }

                string numSAPAY = (arraySAPAY.ContainsKey(key)) ? arraySAPAY[key] : "&nbsp;";
                if (arraySAPAY.ContainsKey(key))
                {
                    numSAPAYTotal += int.Parse(arraySAPAY[key]);
                    numCIF++;
                    numCIFTotal++;
                }

                decimal numSAPAYAmount = (arraySAPAYAmount.ContainsKey(key)) ? arraySAPAYAmount[key] : 0;
                string strSAPAYAmount = (numSAPAYAmount != 0) ? numSAPAYAmount.ToString("F0") : "&nbsp;";
                if (arraySAPAYAmount.ContainsKey(key))
                {
                    numSAPAYAmountTotal += arraySAPAYAmount[key];
                }

                string numTD = (arrayTD.ContainsKey(key)) ? arrayTD[key] : "&nbsp;";
                if (arrayTD.ContainsKey(key))
                {
                    numTDTotal += int.Parse(arrayTD[key]);
                    numExSAPAY += int.Parse(arrayTD[key]);
                    numExSAPAYTotal += int.Parse(arrayTD[key]);
                    numCIF++;
                    numCIFTotal++;
                }

                decimal numTDAmount = (arrayTDAmount.ContainsKey(key)) ? arrayTDAmount[key] : 0;
                string strTDAmount = (numTDAmount != 0) ? numTDAmount.ToString("F0") : "&nbsp;";
                if (arrayTDAmount.ContainsKey(key))
                {
                    numTDAmountTotal += arrayTDAmount[key];
                }


                string numEB = (arrayEB.ContainsKey(key)) ? arrayEB[key].ToString() : "&nbsp;";
                if (arrayEB.ContainsKey(key))
                {
                    numEBTotal += arrayEB[key];
                    numREF += arrayEB[key];
                    numREFTotal += arrayEB[key];
                    numCIF++;
                    numCIFTotal++;                    
                }

                string numKPM = (arrayKPM.ContainsKey(key)) ? arrayKPM[key].ToString() : "&nbsp;";
                if (arrayKPM.ContainsKey(key))
                {
                    numKPMTotal += arrayKPM[key];
                    numREF += arrayKPM[key];
                    numREFTotal += arrayKPM[key];
                    numCIF++;
                    numCIFTotal++;
                }

                string numCC = (arrayCC.ContainsKey(key)) ? arrayCC[key].ToString() : "&nbsp;";
                if (arrayCC.ContainsKey(key))
                {
                    numCCTotal += arrayCC[key];
                    numREF += arrayCC[key];
                    numREFTotal += arrayCC[key];
                    numCIF++;
                    numCIFTotal++;
                }

                string numKPR = (arrayKPR.ContainsKey(key)) ? arrayKPR[key].ToString() : "&nbsp;";
                if (arrayKPR.ContainsKey(key))
                {
                    numKPRTotal += arrayKPR[key];
                    numREF += arrayKPR[key];
                    numREFTotal += arrayKPR[key];
                    numCIF++;
                    numCIFTotal++;
                }

                string numBANC = (arrayBANC.ContainsKey(key)) ? arrayBANC[key].ToString() : "&nbsp;";
                if (arrayBANC.ContainsKey(key))
                {
                    numBANCTotal += arrayBANC[key];
                    numREF += arrayBANC[key];
                    numREFTotal += arrayBANC[key];
                    numCIF++;
                    numCIFTotal++;                    
                }

                msg += "<tr>" +                        
                        "<td>" + item.Name + "</td>" +
                        "<td style=\"text-align:center\">" + item.Npk + "</td>" +
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

            }

            msg += "<tr style=\"border:2px solid #000\">" +
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

            msg += "</table>";
            
            ViewBag.Output = msg;
            string view = (export == 1) ? "list_export" : "list";

            return View(view);
        }

        public ActionResult Weekly()
        {
            if (CommonModel.UserRole() != "RBH") return RedirectToAction("Index", "Home");

            RBH rbh = _db.RBHs.Where(c => c.RbhId == user.RelatedId).First();

            ViewData["Teams"] = _db.SalesTeams.OrderBy(c => c.Name).ToList();
            ViewData["Branchs"] = _db.Branchs.Where(c => c.Area.RegionCode == rbh.RegionCode).OrderBy(c => c.Name).ToList();

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
                option.OptionId = i + 1;
                option.OptionString = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[i];
                MonthOption.Add(option);
            }
            ViewData["MonthOption"] = MonthOption;

            return View();
        }

        public ActionResult WeeklyList(int month, int year, string branch, int team, int export = 0)
        {
            if (CommonModel.UserRole() != "RBH") return RedirectToAction("Index", "Home");

            if (month <= 0 || month > 12 || year <= 0)
            {
                Response.Write("<div class=\"arial\" style=\"text-align:center;padding:10px 0;\">Invalid month</div>");
                Response.End();
            }

            ViewData["BranchCode"] = branch;

            var checkBranch = _db.Branchs.Where(c => c.BranchCode == branch);
            if (checkBranch.Count() > 0)
            {
                Branch br = checkBranch.First();
                ViewData["BranchName"] = br.Name;
            }
            else
            {
                Response.Write("<div class=\"arial\" style=\"text-align:center;padding:10px 0;\">Invalid branch</div>");
                Response.End();
            }

            var checkTeam = _db.SalesTeams.Where(c => c.TeamId == team);
            if (checkTeam.Count() > 0)
            {
                SalesTeam st = checkTeam.First();
                ViewData["Role"] = st.Name;
            }
            else
            {
                Response.Write("<div class=\"arial\" style=\"text-align:center;padding:10px 0;\">Invalid sales role</div>");
                Response.End();
            }

            int intDate = 1;
            int intMonth = month;
            int intYear = year;

            string msg1 = "";
            string msg2 = "";
            string msg3 = "";
            string msg4 = "";
            string msg5 = "";
            int pembagi = 1000000;

            if (export == 1)
            {
                string MonthName = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[month - 1];
                ViewData["Month"] = MonthName + " " + year;

                Response.AddHeader("Content-Type", "application/ms-excel");
                Response.AddHeader("Content-Disposition", "attachment; filename=WeeklySalesReport_" + MonthName + "_" + year + ".xls");
            }

            var checkSales = _db.Sales.Where(c => c.BranchCode == branch && c.TeamId == team).OrderBy(c => c.Name);
            var SalesList = checkSales.Select(c => c.SalesId).ToList();

            //Check Call
            var checkCall = from c in _db.Calls
                            where SalesList.Contains(c.SalesId) && c.CallDate.Year == intYear && c.CallDate.Month == intMonth 
                            //&& (c.Status == "WARM" || c.Status == "HOT" || c.Status == "BOOKING")
                            orderby c.CallDate
                            group c by
                            new
                            {
                                c.SalesId,
                                c.CallDate.Year,
                                c.CallDate.Month,
                                c.CallDate.Day
                            } into grup
                            select
                            new
                            {
                                SalesId = grup.Key.SalesId,
                                Date = grup.FirstOrDefault().CallDate,
                                Num = grup.Count()
                            };

            Dictionary<string, int> arrayCall = new Dictionary<string, int>();
            if (checkCall.Count() > 0)
            {
                foreach (var call in checkCall)
                {
                    int tanggal = call.Date.Day;
                    string key = "";
                    int val = call.Num;

                    if (tanggal <= 7)
                        key = "w1" + call.SalesId;
                    else if (tanggal >= 8 && tanggal <= 14)
                        key = "w2" + call.SalesId;
                    else if (tanggal >= 15 && tanggal <= 21)
                        key = "w3" + call.SalesId;
                    else if (tanggal >= 15 && tanggal <= 28)
                        key = "w4" + call.SalesId;
                    else key = "w5" + call.SalesId;

                    if (arrayCall.ContainsKey(key))
                        arrayCall[key] += val;
                    else
                        arrayCall[key] = val;
                }
            }

            //Check Visit
            var checkVisit = from c in _db.Visits
                             where SalesList.Contains(c.SalesId) && c.VisitDate.Year == intYear && c.VisitDate.Month == intMonth && c.ReasonId != null
                             //&& (c.Status == "WARM" || c.Status == "HOT" || c.Status == "BOOKING")
                             group c by
                             new
                             {
                                 c.SalesId,
                                 c.VisitDate.Year,
                                 c.VisitDate.Month,
                                 c.VisitDate.Day
                             } into grup
                             select
                             new
                             {
                                 SalesId = grup.Key.SalesId,
                                 Date = grup.FirstOrDefault().VisitDate,
                                 Num = grup.Count()
                             };

            Dictionary<string, int> arrayVisit = new Dictionary<string, int>();
            if (checkVisit.Count() > 0)
            {
                foreach (var visit in checkVisit)
                {
                    int tanggal = visit.Date.Day;
                    string key = "";
                    int val = visit.Num;

                    if (tanggal <= 7)
                        key = "w1" + visit.SalesId;
                    else if (tanggal >= 8 && tanggal <= 14)
                        key = "w2" + visit.SalesId;
                    else if (tanggal >= 15 && tanggal <= 21)
                        key = "w3" + visit.SalesId;
                    else if (tanggal >= 15 && tanggal <= 28)
                        key = "w4" + visit.SalesId;
                    else key = "w5" + visit.SalesId;

                    if (arrayVisit.ContainsKey(key))
                        arrayVisit[key] += val;
                    else
                        arrayVisit[key] = val;
                }
            }

            //Check SME Loan
            var checkSME = from c in _db.NasabahProducts
                           where (c.SalesId != null && SalesList.Contains(c.SalesId.Value)) && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.Status == "BOOKING" && c.Product.Code == "SME Loan"
                           group c by
                           new
                           {
                               c.SalesId,
                               c.LastUpdate.Year,
                               c.LastUpdate.Month,
                               c.LastUpdate.Day
                           } into grup
                           select
                           new
                           {
                               SalesId = grup.Key.SalesId,
                               Date = grup.FirstOrDefault().LastUpdate,
                               Num = grup.Count(),
                               Total = grup.Sum(d => d.Amount)
                           };

            Dictionary<string, int> arraySME = new Dictionary<string, int>();
            Dictionary<string, decimal> arraySMEAmount = new Dictionary<string, decimal>();
            if (checkSME.Count() > 0)
            {
                foreach (var sme in checkSME)
                {
                    int tanggal = sme.Date.Day;
                    string key = "";
                    int val = sme.Num;

                    if (tanggal <= 7)
                        key = "w1" + sme.SalesId;
                    else if (tanggal >= 8 && tanggal <= 14)
                        key = "w2" + sme.SalesId;
                    else if (tanggal >= 15 && tanggal <= 21)
                        key = "w3" + sme.SalesId;
                    else if (tanggal >= 15 && tanggal <= 28)
                        key = "w4" + sme.SalesId;
                    else key = "w5" + sme.SalesId;

                    if (arraySME.ContainsKey(key))
                        arraySME[key] += val;
                    else
                        arraySME[key] = val;

                    if (arraySMEAmount.ContainsKey(key))
                        arraySMEAmount[key] += (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                    else
                        arraySMEAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;

                }
            }


            //Check Warm
            var checkWarmCall = from c in _db.Calls
                                where SalesList.Contains(c.SalesId) && c.CallDate.Year == intYear && c.CallDate.Month == intMonth && c.Status == "WARM"
                                group c by
                                new
                                {
                                    c.SalesId,
                                    c.CallDate.Year,
                                    c.CallDate.Month,
                                    c.CallDate.Day
                                } into grup
                                select
                                new
                                {
                                    SalesId = grup.Key.SalesId,
                                    Date = grup.FirstOrDefault().CallDate,
                                    Num = grup.Count()
                                };

            var checkWarmVisit = from c in _db.Visits
                                 where SalesList.Contains(c.SalesId) && c.VisitDate.Year == intYear && c.VisitDate.Month == intMonth && c.VisitDate.Day == intDate && c.Status == "WARM"
                                 group c by
                                 new
                                 {
                                     c.SalesId,
                                     c.VisitDate.Year,
                                     c.VisitDate.Month,
                                     c.VisitDate.Day
                                 } into grup
                                 select
                                 new
                                 {
                                     SalesId = grup.Key.SalesId,
                                     Date = grup.FirstOrDefault().VisitDate,
                                     Num = grup.Count()
                                 };

            Dictionary<string, int> arrayWarm = new Dictionary<string, int>();
            if (checkWarmCall.Count() > 0)
            {
                foreach (var call in checkWarmCall)
                {
                    int tanggal = call.Date.Day;
                    string key = "";
                    int val = call.Num;

                    if (tanggal <= 7)
                        key = "w1" + call.SalesId;
                    else if (tanggal >= 8 && tanggal <= 14)
                        key = "w2" + call.SalesId;
                    else if (tanggal >= 15 && tanggal <= 21)
                        key = "w3" + call.SalesId;
                    else if (tanggal >= 15 && tanggal <= 28)
                        key = "w4" + call.SalesId;
                    else key = "w5" + call.SalesId;

                    if (arrayWarm.ContainsKey(key))
                        arrayWarm[key] += val;
                    else
                        arrayWarm[key] = val;
                }
            }
            if (checkWarmVisit.Count() > 0)
            {
                foreach (var visit in checkWarmVisit)
                {
                    int tanggal = visit.Date.Day;
                    string key = "";
                    int val = visit.Num;

                    if (tanggal <= 7)
                        key = "w1" + visit.SalesId;
                    else if (tanggal >= 8 && tanggal <= 14)
                        key = "w2" + visit.SalesId;
                    else if (tanggal >= 15 && tanggal <= 21)
                        key = "w3" + visit.SalesId;
                    else if (tanggal >= 15 && tanggal <= 28)
                        key = "w4" + visit.SalesId;
                    else key = "w5" + visit.SalesId;

                    if (arrayWarm.ContainsKey(key))
                        arrayWarm[key] += val;
                    else
                        arrayWarm[key] = val;
                }
            }

            //Check Hot
            var checkHotCall = from c in _db.Calls
                               where SalesList.Contains(c.SalesId) && c.CallDate.Year == intYear && c.CallDate.Month == intMonth && c.Status == "HOT"
                               group c by
                               new
                               {
                                   c.SalesId,
                                   c.CallDate.Year,
                                   c.CallDate.Month,
                                   c.CallDate.Day
                               } into grup
                               select
                               new
                               {
                                   SalesId = grup.Key.SalesId,
                                   Date = grup.FirstOrDefault().CallDate,
                                   Num = grup.Count()
                               };

            var checkHotVisit = from c in _db.Visits
                                where SalesList.Contains(c.SalesId) && c.VisitDate.Year == intYear && c.VisitDate.Month == intMonth && c.VisitDate.Day == intDate && c.Status == "HOT"
                                group c by
                                new
                                {
                                    c.SalesId,
                                    c.VisitDate.Year,
                                    c.VisitDate.Month,
                                    c.VisitDate.Day
                                } into grup
                                select
                                new
                                {
                                    SalesId = grup.Key.SalesId,
                                    Date = grup.FirstOrDefault().VisitDate,
                                    Num = grup.Count()
                                };

            Dictionary<string, int> arrayHot = new Dictionary<string, int>();
            if (checkHotCall.Count() > 0)
            {
                foreach (var call in checkHotCall)
                {
                    int tanggal = call.Date.Day;
                    string key = "";
                    int val = call.Num;

                    if (tanggal <= 7)
                        key = "w1" + call.SalesId;
                    else if (tanggal >= 8 && tanggal <= 14)
                        key = "w2" + call.SalesId;
                    else if (tanggal >= 15 && tanggal <= 21)
                        key = "w3" + call.SalesId;
                    else if (tanggal >= 15 && tanggal <= 28)
                        key = "w4" + call.SalesId;
                    else key = "w5" + call.SalesId;

                    if (arrayHot.ContainsKey(key))
                        arrayHot[key] += val;
                    else
                        arrayHot[key] = val;
                }
            }
            if (checkHotVisit.Count() > 0)
            {
                foreach (var visit in checkHotVisit)
                {
                    int tanggal = visit.Date.Day;
                    string key = "";
                    int val = visit.Num;

                    if (tanggal <= 7)
                        key = "w1" + visit.SalesId;
                    else if (tanggal >= 8 && tanggal <= 14)
                        key = "w2" + visit.SalesId;
                    else if (tanggal >= 15 && tanggal <= 21)
                        key = "w3" + visit.SalesId;
                    else if (tanggal >= 15 && tanggal <= 28)
                        key = "w4" + visit.SalesId;
                    else key = "w5" + visit.SalesId;

                    if (arrayHot.ContainsKey(key))
                        arrayHot[key] += val;
                    else
                        arrayHot[key] = val;
                }
            }

            //Check CA
            var checkCA = from c in _db.NasabahProducts
                          where SalesList.Contains(c.SalesId.Value) && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.Status == "BOOKING" && c.Product.Code == "CA"
                          group c by
                          new
                          {
                              c.SalesId,
                              c.LastUpdate.Year,
                              c.LastUpdate.Month,
                              c.LastUpdate.Day
                          } into grup
                          select
                          new
                          {
                              SalesId = grup.Key.SalesId,
                              Date = grup.FirstOrDefault().LastUpdate,
                              Num = grup.Count(),
                              Total = grup.Sum(d => d.Amount)
                          };

            Dictionary<string, int> arrayCA = new Dictionary<string, int>();
            Dictionary<string, decimal> arrayCAAmount = new Dictionary<string, decimal>();
            if (checkCA.Count() > 0)
            {
                foreach (var ca in checkCA)
                {
                    int tanggal = ca.Date.Day;
                    string key = "";
                    int val = ca.Num;

                    if (tanggal <= 7)
                        key = "w1" + ca.SalesId;
                    else if (tanggal >= 8 && tanggal <= 14)
                        key = "w2" + ca.SalesId;
                    else if (tanggal >= 15 && tanggal <= 21)
                        key = "w3" + ca.SalesId;
                    else if (tanggal >= 15 && tanggal <= 28)
                        key = "w4" + ca.SalesId;
                    else key = "w5" + ca.SalesId;

                    if (arrayCA.ContainsKey(key))
                        arrayCA[key] += val;
                    else
                        arrayCA[key] = val;

                    if (arrayCAAmount.ContainsKey(key))
                        arrayCAAmount[key] += (ca.Total.GetValueOrDefault() != 0) ? ca.Total.GetValueOrDefault() / pembagi : 0;
                    else
                        arrayCAAmount[key] = (ca.Total.GetValueOrDefault() != 0) ? ca.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check SA
            var checkSA = from c in _db.NasabahProducts
                          where SalesList.Contains(c.SalesId.Value) && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.Status == "BOOKING" && c.Product.Code == "SA"
                          group c by
                          new
                          {
                              c.SalesId,
                              c.LastUpdate.Year,
                              c.LastUpdate.Month,
                              c.LastUpdate.Day
                          } into grup
                          select
                          new
                          {
                              SalesId = grup.Key.SalesId,
                              Date = grup.FirstOrDefault().LastUpdate,
                              Num = grup.Count(),
                              Total = grup.Sum(d => d.Amount)
                          };

            Dictionary<string, int> arraySA = new Dictionary<string, int>();
            Dictionary<string, decimal> arraySAAmount = new Dictionary<string, decimal>();
            if (checkSA.Count() > 0)
            {
                foreach (var sme in checkSA)
                {
                    int tanggal = sme.Date.Day;
                    string key = "";
                    int val = sme.Num;

                    if (tanggal <= 7)
                        key = "w1" + sme.SalesId;
                    else if (tanggal >= 8 && tanggal <= 14)
                        key = "w2" + sme.SalesId;
                    else if (tanggal >= 15 && tanggal <= 21)
                        key = "w3" + sme.SalesId;
                    else if (tanggal >= 15 && tanggal <= 28)
                        key = "w4" + sme.SalesId;
                    else key = "w5" + sme.SalesId;

                    if (arraySA.ContainsKey(key))
                        arraySA[key] += val;
                    else
                        arraySA[key] = val;

                    if (arraySAAmount.ContainsKey(key))
                        arraySAAmount[key] += (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                    else
                        arraySAAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check SAPAY
            var checkSAPAY = from c in _db.NasabahProducts
                             where SalesList.Contains(c.SalesId.Value) && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.Status == "BOOKING" && c.Product.Code == "SA PAY"
                             group c by
                             new
                             {
                                 c.SalesId,
                                 c.LastUpdate.Year,
                                 c.LastUpdate.Month,
                                 c.LastUpdate.Day
                             } into grup
                             select
                             new
                             {
                                 SalesId = grup.Key.SalesId,
                                 Date = grup.FirstOrDefault().LastUpdate,
                                 Num = grup.Count(),
                                 Total = grup.Sum(d => d.Amount)
                             };

            Dictionary<string, int> arraySAPAY = new Dictionary<string, int>();
            Dictionary<string, decimal> arraySAPAYAmount = new Dictionary<string, decimal>();
            if (checkSAPAY.Count() > 0)
            {
                foreach (var sme in checkSAPAY)
                {
                    int tanggal = sme.Date.Day;
                    string key = "";
                    int val = sme.Num;

                    if (tanggal <= 7)
                        key = "w1" + sme.SalesId;
                    else if (tanggal >= 8 && tanggal <= 14)
                        key = "w2" + sme.SalesId;
                    else if (tanggal >= 15 && tanggal <= 21)
                        key = "w3" + sme.SalesId;
                    else if (tanggal >= 15 && tanggal <= 28)
                        key = "w4" + sme.SalesId;
                    else key = "w5" + sme.SalesId;

                    if (arraySAPAY.ContainsKey(key))
                        arraySAPAY[key] += val;
                    else
                        arraySAPAY[key] = val;

                    if (arraySAPAYAmount.ContainsKey(key))
                        arraySAPAYAmount[key] += (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                    else
                        arraySAPAYAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check TD
            var checkTD = from c in _db.NasabahProducts
                          where SalesList.Contains(c.SalesId.Value) && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.Status == "BOOKING" && c.Product.Code == "TD"
                          group c by
                          new
                          {
                              c.SalesId,
                              c.LastUpdate.Year,
                              c.LastUpdate.Month,
                              c.LastUpdate.Day
                          } into grup
                          select
                          new
                          {
                              SalesId = grup.Key.SalesId,
                              Date = grup.FirstOrDefault().LastUpdate,
                              Num = grup.Count(),
                              Total = grup.Sum(d => d.Amount)
                          };

            Dictionary<string, int> arrayTD = new Dictionary<string, int>();
            Dictionary<string, decimal> arrayTDAmount = new Dictionary<string, decimal>();
            if (checkTD.Count() > 0)
            {
                foreach (var sme in checkTD)
                {
                    int tanggal = sme.Date.Day;
                    string key = "";
                    int val = sme.Num;

                    if (tanggal <= 7)
                        key = "w1" + sme.SalesId;
                    else if (tanggal >= 8 && tanggal <= 14)
                        key = "w2" + sme.SalesId;
                    else if (tanggal >= 15 && tanggal <= 21)
                        key = "w3" + sme.SalesId;
                    else if (tanggal >= 15 && tanggal <= 28)
                        key = "w4" + sme.SalesId;
                    else key = "w5" + sme.SalesId;

                    if (arrayTD.ContainsKey(key))
                        arrayTD[key] += val;
                    else
                        arrayTD[key] = val;

                    if (arrayTDAmount.ContainsKey(key))
                        arrayTDAmount[key] += (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                    else
                        arrayTDAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check Referral EB Loan
            var checkRefferal = _db.SalesNasabahs.Where(c => SalesList.Contains(c.RefferalFrom.Value));
            Dictionary<string, int> arrayEB = new Dictionary<string, int>();
            Dictionary<string, int> arrayKPM = new Dictionary<string, int>();
            Dictionary<string, int> arrayCC = new Dictionary<string, int>();
            Dictionary<string, int> arrayKPR = new Dictionary<string, int>();
            Dictionary<string, int> arrayBANC = new Dictionary<string, int>();

            if (checkRefferal.Count() > 0)
            {
                foreach (SalesNasabah sn in checkRefferal)
                {
                    int intKey = sn.RefferalFrom.GetValueOrDefault();

                    var checkEB = from c in _db.NasabahProducts
                                  where c.SalesId == sn.SalesId && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.Status == "BOOKING" && c.Product.Code == "SME Loan"
                                  select c;

                    if (checkEB.Count() > 0)
                    {
                        foreach (NasabahProduct item in checkEB)
                        {
                            int tanggal = item.LastUpdate.Day;
                            string key = "";

                            if (tanggal <= 7)
                                key = "w1" + intKey;
                            else if (tanggal >= 8 && tanggal <= 14)
                                key = "w2" + intKey;
                            else if (tanggal >= 15 && tanggal <= 21)
                                key = "w3" + intKey;
                            else if (tanggal >= 15 && tanggal <= 28)
                                key = "w4" + intKey;
                            else key = "w5" + intKey;

                            arrayEB[key] = (arrayEB.ContainsKey(key)) ? arrayEB[key] + 1 : 1;
                        }
                    }

                    var checkKPM = from c in _db.NasabahProducts
                                   where c.SalesId == sn.SalesId && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.Status == "BOOKING" && c.Product.Code == "KPM"
                                   select c;

                    if (checkKPM.Count() > 0)
                    {
                        foreach (NasabahProduct item in checkKPM)
                        {
                            int tanggal = item.LastUpdate.Day;
                            string key = "";

                            if (tanggal <= 7)
                                key = "w1" + intKey;
                            else if (tanggal >= 8 && tanggal <= 14)
                                key = "w2" + intKey;
                            else if (tanggal >= 15 && tanggal <= 21)
                                key = "w3" + intKey;
                            else if (tanggal >= 15 && tanggal <= 28)
                                key = "w4" + intKey;
                            else key = "w5" + intKey;

                            arrayKPM[key] = (arrayKPM.ContainsKey(key)) ? arrayKPM[key] + 1 : 1;
                        }
                    }

                    var checkCC = from c in _db.NasabahProducts
                                  where c.SalesId == sn.SalesId && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.Status == "BOOKING" && c.Product.Code == "CC"
                                  select c;

                    if (checkCC.Count() > 0)
                    {
                        foreach (NasabahProduct item in checkCC)
                        {
                            int tanggal = item.LastUpdate.Day;
                            string key = "";

                            if (tanggal <= 7)
                                key = "w1" + intKey;
                            else if (tanggal >= 8 && tanggal <= 14)
                                key = "w2" + intKey;
                            else if (tanggal >= 15 && tanggal <= 21)
                                key = "w3" + intKey;
                            else if (tanggal >= 15 && tanggal <= 28)
                                key = "w4" + intKey;
                            else key = "w5" + intKey;

                            arrayCC[key] = (arrayCC.ContainsKey(key)) ? arrayCC[key] + 1 : 1;
                        }
                    }

                    var checkKPR = from c in _db.NasabahProducts
                                   where c.SalesId == sn.SalesId && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.Status == "BOOKING" && c.Product.Code == "KPR"
                                   select c;

                    if (checkKPR.Count() > 0)
                    {
                        foreach (NasabahProduct item in checkKPR)
                        {
                            int tanggal = item.LastUpdate.Day;
                            string key = "";

                            if (tanggal <= 7)
                                key = "w1" + intKey;
                            else if (tanggal >= 8 && tanggal <= 14)
                                key = "w2" + intKey;
                            else if (tanggal >= 15 && tanggal <= 21)
                                key = "w3" + intKey;
                            else if (tanggal >= 15 && tanggal <= 28)
                                key = "w4" + intKey;
                            else key = "w5" + intKey;

                            arrayKPR[key] = (arrayKPR.ContainsKey(key)) ? arrayKPR[key] + 1 : 1;
                        }
                    }

                    var checkBANC = from c in _db.NasabahProducts
                                    where c.SalesId == sn.SalesId && c.LastUpdate.Year == intYear && c.LastUpdate.Month == intMonth && c.Status == "BOOKING" && c.Product.Code == "INS"
                                    select c;

                    if (checkBANC.Count() > 0)
                    {
                        foreach (NasabahProduct item in checkBANC)
                        {
                            int tanggal = item.LastUpdate.Day;
                            string key = "";

                            if (tanggal <= 7)
                                key = "w1" + intKey;
                            else if (tanggal >= 8 && tanggal <= 14)
                                key = "w2" + intKey;
                            else if (tanggal >= 15 && tanggal <= 21)
                                key = "w3" + intKey;
                            else if (tanggal >= 15 && tanggal <= 28)
                                key = "w4" + intKey;
                            else key = "w5" + intKey;

                            arrayBANC[key] = (arrayBANC.ContainsKey(key)) ? arrayBANC[key] + 1 : 1;
                        }
                    }
                }
            }

            Dictionary<int, int> numCallSubTotal = new Dictionary<int, int>();
            int numCallGrandTotal = 0;
            Dictionary<int, int> numVisitSubTotal = new Dictionary<int, int>();
            int numVisitGrandTotal = 0;
            Dictionary<int, int> numWarmSubTotal = new Dictionary<int, int>();
            int numWarmGrandTotal = 0;
            Dictionary<int, int> numHotSubTotal = new Dictionary<int, int>();
            int numHotGrandTotal = 0;
            Dictionary<int, int> numSMESubTotal = new Dictionary<int, int>();
            int numSMEGrandTotal = 0;
            Dictionary<int, decimal> numSMEAmountSubTotal = new Dictionary<int, decimal>();
            decimal numSMEAmountGrandTotal = 0;
            Dictionary<int, int> numCASubTotal = new Dictionary<int, int>();
            int numCAGrandTotal = 0;
            Dictionary<int, decimal> numCAAmountSubTotal = new Dictionary<int, decimal>();
            decimal numCAAmountGrandTotal = 0;
            Dictionary<int, int> numSASubTotal = new Dictionary<int, int>();
            int numSAGrandTotal = 0;
            Dictionary<int, decimal> numSAAmountSubTotal = new Dictionary<int, decimal>();
            decimal numSAAmountGrandTotal = 0;
            Dictionary<int, int> numSAPAYSubTotal = new Dictionary<int, int>();
            int numSAPAYGrandTotal = 0;
            Dictionary<int, decimal> numSAPAYAmountSubTotal = new Dictionary<int, decimal>();
            decimal numSAPAYAmountGrandTotal = 0;
            Dictionary<int, int> numTDSubTotal = new Dictionary<int, int>();
            int numTDGrandTotal = 0;
            Dictionary<int, decimal> numTDAmountSubTotal = new Dictionary<int, decimal>();
            decimal numTDAmountGrandTotal = 0;
            Dictionary<int, int> numEBSubTotal = new Dictionary<int, int>();
            int numEBGrandTotal = 0;
            Dictionary<int, int> numKPMSubTotal = new Dictionary<int, int>();
            int numKPMGrandTotal = 0;
            Dictionary<int, int> numCCSubTotal = new Dictionary<int, int>();
            int numCCGrandTotal = 0;
            Dictionary<int, int> numKPRSubTotal = new Dictionary<int, int>();
            int numKPRGrandTotal = 0;
            Dictionary<int, int> numBANCSubTotal = new Dictionary<int, int>();
            int numBANCGrandTotal = 0;
            Dictionary<int, int> numCIFSubTotal = new Dictionary<int, int>();
            int numCIFGrandTotal = 0;

            //loop start

            foreach (Sale item in checkSales)
            {
                int key = item.SalesId;

                Dictionary<int, int> numCall = new Dictionary<int, int>();
                int numCallTotal = 0;
                Dictionary<int, int> numVisit = new Dictionary<int, int>();
                int numVisitTotal = 0;
                Dictionary<int, int> numSME = new Dictionary<int, int>();
                int numSMETotal = 0;
                Dictionary<int, decimal> numSMEAmount = new Dictionary<int, decimal>();
                decimal numSMEAmountTotal = 0;
                Dictionary<int, int> numWarm = new Dictionary<int, int>();
                int numWarmTotal = 0;
                Dictionary<int, int> numHot = new Dictionary<int, int>();
                int numHotTotal = 0;
                Dictionary<int, int> numCA = new Dictionary<int, int>();
                Dictionary<int, int> numCIF = new Dictionary<int, int>();
                int numCATotal = 0;
                int numCIFTotal = 0;
                Dictionary<int, decimal> numCAAmount = new Dictionary<int, decimal>();
                decimal numCAAmountTotal = 0;
                Dictionary<int, int> numSA = new Dictionary<int, int>();
                int numSATotal = 0;
                Dictionary<int, decimal> numSAAmount = new Dictionary<int, decimal>();
                decimal numSAAmountTotal = 0;
                Dictionary<int, int> numSAPAY = new Dictionary<int, int>();
                int numSAPAYTotal = 0;
                Dictionary<int, decimal> numSAPAYAmount = new Dictionary<int, decimal>();
                decimal numSAPAYAmountTotal = 0;
                Dictionary<int, int> numTD = new Dictionary<int, int>();
                int numTDTotal = 0;
                Dictionary<int, decimal> numTDAmount = new Dictionary<int, decimal>();
                decimal numTDAmountTotal = 0;
                Dictionary<int, int> numEB = new Dictionary<int, int>();
                int numEBTotal = 0;
                Dictionary<int, int> numKPM = new Dictionary<int, int>();
                int numKPMTotal = 0;
                Dictionary<int, int> numCC = new Dictionary<int, int>();
                int numCCTotal = 0;
                Dictionary<int, int> numKPR = new Dictionary<int, int>();
                int numKPRTotal = 0;
                Dictionary<int, int> numBANC = new Dictionary<int, int>();
                int numBANCTotal = 0;

                for (int i = 1; i <= 5; i++)
                {
                    string index = "w" + i + item.SalesId;
                    numCall[i] = (arrayCall.ContainsKey(index)) ? arrayCall[index] : 0;
                    numCallTotal += numCall[i];
                    numCallSubTotal[i] = (numCallSubTotal.ContainsKey(i)) ? numCallSubTotal[i] + numCall[i] : numCall[i];
                    numCallGrandTotal += numCall[i];

                    numVisit[i] = (arrayVisit.ContainsKey(index)) ? arrayVisit[index] : 0;
                    numVisitTotal += numVisit[i];
                    numVisitSubTotal[i] = (numVisitSubTotal.ContainsKey(i)) ? numVisitSubTotal[i] + numVisit[i] : numVisit[i];
                    numVisitGrandTotal += numVisit[i];

                    numSME[i] = (arraySME.ContainsKey(index)) ? arraySME[index] : 0;
                    numSMETotal += numSME[i];
                    numSMESubTotal[i] = (numSMESubTotal.ContainsKey(i)) ? numSMESubTotal[i] + numSME[i] : numSME[i];
                    numSMEGrandTotal += numSME[i];

                    numSMEAmount[i] = (arraySMEAmount.ContainsKey(index)) ? arraySMEAmount[index] : 0;
                    numSMEAmountTotal += numSMEAmount[i];
                    numSMEAmountSubTotal[i] = (numSMEAmountSubTotal.ContainsKey(i)) ? numSMEAmountSubTotal[i] + numSMEAmount[i] : numSMEAmount[i];
                    numSMEAmountGrandTotal += numSMEAmount[i];

                    numWarm[i] = (arrayWarm.ContainsKey(index)) ? arrayWarm[index] : 0;
                    numWarmTotal += numWarm[i];
                    numWarmSubTotal[i] = (numWarmSubTotal.ContainsKey(i)) ? numWarmSubTotal[i] + numWarm[i] : numWarm[i];
                    numWarmGrandTotal += numWarm[i];

                    numHot[i] = (arrayHot.ContainsKey(index)) ? arrayHot[index] : 0;
                    numHotTotal += numHot[i];
                    numHotSubTotal[i] = (numHotSubTotal.ContainsKey(i)) ? numHotSubTotal[i] + numHot[i] : numHot[i];
                    numHotGrandTotal += numHot[i];

                    numCA[i] = (arrayCA.ContainsKey(index)) ? arrayCA[index] : 0;
                    numCATotal += numCA[i];
                    numCIF[i] = numCA[i];
                    numCIFTotal += numCA[i];
                    numCIFSubTotal[i] = (numCIFSubTotal.ContainsKey(i)) ? numCIFSubTotal[i] + numCA[i] : numCA[i];
                    numCIFGrandTotal += numCA[i];
                    numCASubTotal[i] = (numCASubTotal.ContainsKey(i)) ? numCASubTotal[i] + numCA[i] : numCA[i];
                    numCAGrandTotal += numCA[i];

                    numCAAmount[i] = (arrayCAAmount.ContainsKey(index)) ? arrayCAAmount[index] : 0;
                    numCAAmountTotal += numCAAmount[i];
                    numCAAmountSubTotal[i] = (numCAAmountSubTotal.ContainsKey(i)) ? numCAAmountSubTotal[i] + numCAAmount[i] : numCAAmount[i];
                    numCAAmountGrandTotal += numCAAmount[i];

                    numSA[i] = (arraySA.ContainsKey(index)) ? arraySA[index] : 0;
                    numSATotal += numSA[i];
                    numCIF[i] += numSA[i];
                    numCIFTotal += numSA[i];
                    numCIFSubTotal[i] = (numCIFSubTotal.ContainsKey(i)) ? numCIFSubTotal[i] + numSA[i] : numSA[i];
                    numCIFGrandTotal += numSA[i];
                    numSASubTotal[i] = (numSASubTotal.ContainsKey(i)) ? numSASubTotal[i] + numSA[i] : numSA[i];
                    numSAGrandTotal += numSA[i];

                    numSAAmount[i] = (arraySAAmount.ContainsKey(index)) ? arraySAAmount[index] : 0;
                    numSAAmountTotal += numSAAmount[i];
                    numSAAmountSubTotal[i] = (numSAAmountSubTotal.ContainsKey(i)) ? numSAAmountSubTotal[i] + numSAAmount[i] : numSAAmount[i];
                    numSAAmountGrandTotal += numSAAmount[i];

                    numSAPAY[i] = (arraySAPAY.ContainsKey(index)) ? arraySAPAY[index] : 0;
                    numSAPAYTotal += numSAPAY[i];
                    numCIF[i] += numSAPAY[i];
                    numCIFTotal += numSAPAY[i];
                    numCIFSubTotal[i] = (numCIFSubTotal.ContainsKey(i)) ? numCIFSubTotal[i] + numSAPAY[i] : numSAPAY[i];
                    numCIFGrandTotal += numSAPAY[i];
                    numSAPAYSubTotal[i] = (numSAPAYSubTotal.ContainsKey(i)) ? numSAPAYSubTotal[i] + numSAPAY[i] : numSAPAY[i];
                    numSAPAYGrandTotal += numSAPAY[i];

                    numSAPAYAmount[i] = (arraySAPAYAmount.ContainsKey(index)) ? arraySAPAYAmount[index] : 0;
                    numSAPAYAmountTotal += numSAPAYAmount[i];
                    numSAPAYAmountSubTotal[i] = (numSAPAYAmountSubTotal.ContainsKey(i)) ? numSAPAYAmountSubTotal[i] + numSAPAYAmount[i] : numSAPAYAmount[i];
                    numSAPAYAmountGrandTotal += numSAPAYAmount[i];

                    numTD[i] = (arrayTD.ContainsKey(index)) ? arrayTD[index] : 0;
                    numTDTotal += numTD[i];
                    numCIF[i] += numTD[i];
                    numCIFTotal += numTD[i];
                    numCIFSubTotal[i] = (numCIFSubTotal.ContainsKey(i)) ? numCIFSubTotal[i] + numTD[i] : numTD[i];
                    numCIFGrandTotal += numTD[i];
                    numTDSubTotal[i] = (numTDSubTotal.ContainsKey(i)) ? numTDSubTotal[i] + numTD[i] : numTD[i];
                    numTDGrandTotal += numTD[i];

                    numTDAmount[i] = (arrayTDAmount.ContainsKey(index)) ? arrayTDAmount[index] : 0;
                    numTDAmountTotal += numTDAmount[i];
                    numTDAmountSubTotal[i] = (numTDAmountSubTotal.ContainsKey(i)) ? numTDAmountSubTotal[i] + numTDAmount[i] : numTDAmount[i];
                    numTDAmountGrandTotal += numTDAmount[i];

                    numEB[i] = (arrayEB.ContainsKey(index)) ? arrayEB[index] : 0;
                    numEBTotal += numEB[i];
                    numCIF[i] += numEB[i];
                    numCIFTotal += numEB[i];
                    numCIFSubTotal[i] = (numCIFSubTotal.ContainsKey(i)) ? numCIFSubTotal[i] + numEB[i] : numEB[i];
                    numCIFGrandTotal += numEB[i];
                    numEBSubTotal[i] = (numEBSubTotal.ContainsKey(i)) ? numEBSubTotal[i] + numEB[i] : numEB[i];
                    numEBGrandTotal += numEB[i];

                    numKPM[i] = (arrayKPM.ContainsKey(index)) ? arrayKPM[index] : 0;
                    numKPMTotal += numKPM[i];
                    numCIF[i] += numKPM[i];
                    numCIFTotal += numKPM[i];
                    numCIFSubTotal[i] = (numCIFSubTotal.ContainsKey(i)) ? numCIFSubTotal[i] + numKPM[i] : numKPM[i];
                    numCIFGrandTotal += numKPM[i];
                    numKPMSubTotal[i] = (numKPMSubTotal.ContainsKey(i)) ? numKPMSubTotal[i] + numKPM[i] : numKPM[i];
                    numKPMGrandTotal += numKPM[i];

                    numCC[i] = (arrayCC.ContainsKey(index)) ? arrayCC[index] : 0;
                    numCCTotal += numCC[i];
                    numCIF[i] += numCC[i];
                    numCIFTotal += numCC[i];
                    numCIFSubTotal[i] = (numCIFSubTotal.ContainsKey(i)) ? numCIFSubTotal[i] + numCC[i] : numCC[i];
                    numCIFGrandTotal += numCC[i];
                    numCCSubTotal[i] = (numCCSubTotal.ContainsKey(i)) ? numCCSubTotal[i] + numCC[i] : numCC[i];
                    numCCGrandTotal += numCC[i];

                    numKPR[i] = (arrayKPR.ContainsKey(index)) ? arrayKPR[index] : 0;
                    numKPRTotal += numKPR[i];
                    numCIF[i] += numKPR[i];
                    numCIFTotal += numKPR[i];
                    numCIFSubTotal[i] = (numCIFSubTotal.ContainsKey(i)) ? numCIFSubTotal[i] + numKPR[i] : numKPR[i];
                    numCIFGrandTotal += numKPR[i];
                    numKPRSubTotal[i] = (numKPRSubTotal.ContainsKey(i)) ? numKPRSubTotal[i] + numKPR[i] : numKPR[i];
                    numKPRGrandTotal += numKPR[i];

                    numBANC[i] = (arrayBANC.ContainsKey(index)) ? arrayBANC[index] : 0;
                    numBANCTotal += numBANC[i];
                    numCIF[i] += numBANC[i];
                    numCIFTotal += numBANC[i];
                    numCIFSubTotal[i] = (numCIFSubTotal.ContainsKey(i)) ? numCIFSubTotal[i] + numBANC[i] : numBANC[i];
                    numCIFGrandTotal += numBANC[i];
                    numBANCSubTotal[i] = (numBANCSubTotal.ContainsKey(i)) ? numBANCSubTotal[i] + numBANC[i] : numBANC[i];
                    numBANCGrandTotal += numBANC[i];
                }


                msg1 += "<tr>" +
                        "<td>" + item.Name + "</td>" +
                        "<td style=\"text-align:center\">" + numCall[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numCall[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numCall[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numCall[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numCall[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numCallTotal + "</td>" +
                        "<td style=\"text-align:center\">" + numVisit[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numVisit[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numVisit[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numVisit[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numVisit[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numVisitTotal + "</td>" +
                        "<td style=\"text-align:center\">" + numSME[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numSME[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numSME[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numSME[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numSME[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMETotal + "</td>" +
                        "<td style=\"text-align:center\">" + numSMEAmount[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMEAmount[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMEAmount[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMEAmount[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMEAmount[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMEAmountTotal + "</td>" +
                      "</tr>";

                msg2 += "<tr>" +
                        "<td>" + item.Name + "</td>" +
                        "<td style=\"text-align:center\">" + numHot[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numHot[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numHot[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numHot[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numHot[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numHotTotal + "</td>" +
                        "<td style=\"text-align:center\">" + numWarm[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numWarm[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numWarm[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numWarm[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numWarm[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numWarmTotal + "</td>" +
                      "</tr>";

                msg3 += "<tr>" +
                        "<td>" + item.Name + "</td>" +
                        "<td style=\"text-align:center\">" + numCA[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numCA[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numCA[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numCA[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numCA[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numCATotal + "</td>" +
                        "<td style=\"text-align:center\">" + numSA[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numSA[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numSA[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numSA[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numSA[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numSATotal + "</td>" +
                        "<td style=\"text-align:center\">" + numTD[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numTD[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numTD[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numTD[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numTD[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numTDTotal + "</td>" +
                        "<td style=\"text-align:center\">" + numSAPAY[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numSAPAY[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numSAPAY[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numSAPAY[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numSAPAY[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numSAPAYTotal + "</td>" +
                      "</tr>";

                msg4 += "<tr>" +
                        "<td>" + item.Name + "</td>" +
                        "<td style=\"text-align:center\">" + numCAAmount[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numCAAmount[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numCAAmount[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numCAAmount[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numCAAmount[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numCAAmountTotal + "</td>" +
                        "<td style=\"text-align:center\">" + numSAAmount[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numSAAmount[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numSAAmount[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numSAAmount[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numSAAmount[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numSAAmountTotal + "</td>" +
                        "<td style=\"text-align:center\">" + numTDAmount[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numTDAmount[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numTDAmount[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numTDAmount[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numTDAmount[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numTDAmountTotal + "</td>" +
                        "<td style=\"text-align:center\">" + numSAPAYAmount[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numSAPAYAmount[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numSAPAYAmount[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numSAPAYAmount[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numSAPAYAmount[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numSAPAYAmountTotal + "</td>" +
                        "<td style=\"text-align:center\">" + numCIF[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numCIF[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numCIF[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numCIF[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numCIF[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numCIFTotal + "</td>" +
                      "</tr>";

                msg5 += "<tr>" +
                        "<td>" + item.Name + "</td>" +
                        "<td style=\"text-align:center\">" + numKPM[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numKPM[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numKPM[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numKPM[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numKPM[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numKPMTotal + "</td>" +
                        "<td style=\"text-align:center\">" + numCC[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numCC[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numCC[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numCC[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numCC[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numCCTotal + "</td>" +
                        "<td style=\"text-align:center\">" + numKPR[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numKPR[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numKPR[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numKPR[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numKPR[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numKPRTotal + "</td>" +
                        "<td style=\"text-align:center\">" + numBANC[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numBANC[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numBANC[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numBANC[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numBANC[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numBANCTotal + "</td>" +
                        "<td style=\"text-align:center\">" + numEB[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numEB[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numEB[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numEB[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numEB[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numEBTotal + "</td>" +
                      "</tr>";
            }

            msg1 += "<tr>" +
                        "<td><strong>Total</strong></td>" +
                        "<td style=\"text-align:center\">" + numCallSubTotal[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numCallSubTotal[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numCallSubTotal[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numCallSubTotal[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numCallSubTotal[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numCallGrandTotal + "</td>" +
                        "<td style=\"text-align:center\">" + numVisitSubTotal[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numVisitSubTotal[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numVisitSubTotal[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numVisitSubTotal[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numVisitSubTotal[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numVisitGrandTotal + "</td>" +
                        "<td style=\"text-align:center\">" + numSMESubTotal[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMESubTotal[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMESubTotal[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMESubTotal[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMESubTotal[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMEGrandTotal + "</td>" +
                        "<td style=\"text-align:center\">" + numSMEAmountSubTotal[1] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMEAmountSubTotal[2] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMEAmountSubTotal[3] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMEAmountSubTotal[4] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMEAmountSubTotal[5] + "</td>" +
                        "<td style=\"text-align:center\">" + numSMEAmountGrandTotal + "</td>" +
                      "</tr>";

            msg2 += "<tr>" +
                    "<td><strong>Total</strong></td>" +
                    "<td style=\"text-align:center\">" + numHotSubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numHotSubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numHotSubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numHotSubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numHotSubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numHotGrandTotal + "</td>" +
                    "<td style=\"text-align:center\">" + numWarmSubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numWarmSubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numWarmSubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numWarmSubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numWarmSubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numWarmGrandTotal + "</td>" +
                  "</tr>";

            msg3 += "<tr>" +
                    "<td><strong>Total</strong></td>" +
                    "<td style=\"text-align:center\">" + numCASubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numCASubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numCASubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numCASubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numCASubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numCAGrandTotal + "</td>" +
                    "<td style=\"text-align:center\">" + numSASubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numSASubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numSASubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numSASubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numSASubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAGrandTotal + "</td>" +
                    "<td style=\"text-align:center\">" + numTDSubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numTDSubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numTDSubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numTDSubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numTDSubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numTDGrandTotal + "</td>" +
                    "<td style=\"text-align:center\">" + numSAPAYSubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAPAYSubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAPAYSubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAPAYSubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAPAYSubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAPAYGrandTotal + "</td>" +
                  "</tr>";

            msg4 += "<tr>" +
                    "<td><strong>Total</strong></td>" +
                    "<td style=\"text-align:center\">" + numCAAmountSubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numCAAmountSubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numCAAmountSubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numCAAmountSubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numCAAmountSubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numCAAmountGrandTotal + "</td>" +
                    "<td style=\"text-align:center\">" + numSAAmountSubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAAmountSubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAAmountSubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAAmountSubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAAmountSubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAAmountGrandTotal + "</td>" +
                    "<td style=\"text-align:center\">" + numTDAmountSubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numTDAmountSubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numTDAmountSubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numTDAmountSubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numTDAmountSubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numTDAmountGrandTotal + "</td>" +
                    "<td style=\"text-align:center\">" + numSAPAYAmountSubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAPAYAmountSubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAPAYAmountSubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAPAYAmountSubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAPAYAmountSubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numSAPAYAmountGrandTotal + "</td>" +
                    "<td style=\"text-align:center\">" + numCIFSubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numCIFSubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numCIFSubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numCIFSubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numCIFSubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numCIFGrandTotal + "</td>" +
                  "</tr>";

            msg5 += "<tr>" +
                    "<td><strong>Total</strong></td>" +
                    "<td style=\"text-align:center\">" + numKPMSubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numKPMSubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numKPMSubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numKPMSubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numKPMSubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numKPMGrandTotal + "</td>" +
                    "<td style=\"text-align:center\">" + numCCSubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numCCSubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numCCSubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numCCSubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numCCSubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numCCGrandTotal + "</td>" +
                    "<td style=\"text-align:center\">" + numKPRSubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numKPRSubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numKPRSubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numKPRSubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numKPRSubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numKPRGrandTotal + "</td>" +
                    "<td style=\"text-align:center\">" + numBANCSubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numBANCSubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numBANCSubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numBANCSubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numBANCSubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numBANCGrandTotal + "</td>" +
                    "<td style=\"text-align:center\">" + numEBSubTotal[1] + "</td>" +
                    "<td style=\"text-align:center\">" + numEBSubTotal[2] + "</td>" +
                    "<td style=\"text-align:center\">" + numEBSubTotal[3] + "</td>" +
                    "<td style=\"text-align:center\">" + numEBSubTotal[4] + "</td>" +
                    "<td style=\"text-align:center\">" + numEBSubTotal[5] + "</td>" +
                    "<td style=\"text-align:center\">" + numEBGrandTotal + "</td>" +
                  "</tr>";

            msg1 += "</table>";
            msg2 += "</table>";
            msg3 += "</table>";
            msg4 += "</table>";
            msg5 += "</table>";

            ViewBag.Output1 = msg1;
            ViewBag.Output2 = msg2;
            ViewBag.Output3 = msg3;
            ViewBag.Output4 = msg4;
            ViewBag.Output5 = msg5;

            string view = (export == 1) ? "list_weekly_export" : "list_weekly";

            return View(view);
        }

        public ActionResult Monthly()
        {
            if (CommonModel.UserRole() != "RBH") return RedirectToAction("Index", "Home");

            RBH rbh = _db.RBHs.Where(c => c.RbhId == user.RelatedId).First();

            ViewData["Teams"] = _db.SalesTeams.OrderBy(c => c.Name).ToList();
            ViewData["Branchs"] = _db.Branchs.Where(c => c.Area.RegionCode == rbh.RegionCode).OrderBy(c => c.Name).ToList();

            DateTime currentDate = DateTime.Now;
            DateTime nextMonth = DateTime.Now.AddMonths(1);
            DateTime firstDayOfNextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);
            DateTime lastDay = firstDayOfNextMonth.AddDays(-1);

            int yearStart = currentDate.Year;
            int monthStart = currentDate.Month;
            int dayStart = 1;

            int yearEnd = currentDate.Year;
            int monthEnd = currentDate.Month;
            int dayEnd = lastDay.Day;

            string strMonthStart = (monthStart.ToString().Length == 1) ? "0" + monthStart : monthStart.ToString();
            string strDayStart = (dayStart.ToString().Length == 1) ? "0" + dayStart : dayStart.ToString();

            string strMonthEnd = (monthEnd.ToString().Length == 1) ? "0" + monthEnd : monthEnd.ToString();
            string strDayEnd = (dayEnd.ToString().Length == 1) ? "0" + dayEnd : dayEnd.ToString();

            string strStart = strDayStart + "-" + strMonthStart + "-" + yearStart;
            string strEnd = strDayEnd + "-" + strMonthEnd + "-" + yearEnd;

            ViewData["DateStart"] = strStart;
            ViewData["DateEnd"] = strEnd;

            return View();
        }

        public ActionResult Monthlylist(string DateStart, string DateEnd, string branch, int team, int export = 0)
        {
            if (CommonModel.UserRole() != "RBH") return RedirectToAction("Index", "Home");

            string[] arrDateS = DateStart.Split('-');
            if (arrDateS.Count() != 3)
            {
                Response.Write("<div class=\"arial\" style=\"text-align:center;padding:10px 0;\">Invalid date start</div>");
                Response.End();
            }

            string[] arrDateE = DateEnd.Split('-');
            if (arrDateE.Count() != 3)
            {
                Response.Write("<div class=\"arial\" style=\"text-align:center;padding:10px 0;\">Invalid date end</div>");
                Response.End();
            }

            ViewData["BranchCode"] = branch;

            var checkBranch = _db.Branchs.Where(c => c.BranchCode == branch);
            if (checkBranch.Count() > 0)
            {
                Branch br = checkBranch.First();
                ViewData["BranchName"] = br.Name;
            }
            else
            {
                Response.Write("<div class=\"arial\" style=\"text-align:center;padding:10px 0;\">Invalid branch</div>");
                Response.End();
            }

            var checkTeam = _db.SalesTeams.Where(c => c.TeamId == team);
            if (checkTeam.Count() > 0)
            {
                SalesTeam st = checkTeam.First();
                ViewData["Role"] = st.Name;
            }
            else
            {
                Response.Write("<div class=\"arial\" style=\"text-align:center;padding:10px 0;\">Invalid sales role</div>");
                Response.End();
            }

            int intDateS = int.Parse(arrDateS[0]);
            int intMonthS = int.Parse(arrDateS[1]);
            int intYearS = int.Parse(arrDateS[2]);
            DateTime StartDate = new DateTime(intYearS, intMonthS, intDateS);

            int intDateE = int.Parse(arrDateE[0]);
            int intMonthE = int.Parse(arrDateE[1]);
            int intYearE = int.Parse(arrDateE[2]);
            DateTime EndDate = new DateTime(intYearE, intMonthE, intDateE);

            string msg = "";
            int pembagi = 1000000;

            if (export == 1)
            {
                ViewData["Date"] = StartDate.ToString("dd MMM yyyy") + " - " + EndDate.ToString("dd MMM yyyy");
                Response.AddHeader("Content-Type", "application/ms-excel");
                Response.AddHeader("Content-Disposition", "attachment; filename=SalesReport_" + DateStart + "_to_" + DateEnd + ".xls");
            }

            var checkSales = _db.Sales.Where(c => c.BranchCode == branch && c.TeamId == team).OrderBy(c => c.Name);
            var SalesList = checkSales.Select(c => c.SalesId).ToList();

            //Check Call
            var checkCall = from c in _db.Calls
                            where SalesList.Contains(c.SalesId) && c.CallDate >= StartDate && c.CallDate <= EndDate 
                            //&& (c.Status == "WARM" || c.Status == "HOT" || c.Status == "BOOKING")
                            group c by c.SalesId into grup
                            select
                            new
                            {
                                SalesId = grup.FirstOrDefault().SalesId,
                                Num = grup.Count()
                            };

            Dictionary<int, string> arrayCall = new Dictionary<int, string>();
            if (checkCall.Count() > 0)
            {
                foreach (var call in checkCall)
                {
                    int key = call.SalesId;
                    string val = call.Num.ToString();
                    arrayCall[key] = val;
                }
            }
            int numCallTotal = 0;

            //Check Visit
            var checkVisit = from c in _db.Visits
                             where SalesList.Contains(c.SalesId) && c.VisitDate >= StartDate && c.VisitDate <= EndDate && c.ReasonId != null
                             //&& (c.Status == "WARM" || c.Status == "HOT" || c.Status == "BOOKING")
                             group c by c.SalesId into grup
                             select
                             new
                             {
                                 SalesId = grup.FirstOrDefault().SalesId,
                                 Num = grup.Count()
                             };

            Dictionary<int, string> arrayVisit = new Dictionary<int, string>();
            if (checkVisit.Count() > 0)
            {
                foreach (var call in checkVisit)
                {
                    int key = call.SalesId;
                    string val = call.Num.ToString();
                    arrayVisit[key] = val;
                }
            }
            int numVisitTotal = 0;

            //Check Warm
            var checkWarmCall = from c in _db.Calls
                                where SalesList.Contains(c.SalesId) && c.CallDate >= StartDate && c.CallDate <= EndDate && c.Status == "WARM"
                                group c by c.SalesId into grup
                                select
                                new
                                {
                                    SalesId = grup.FirstOrDefault().SalesId,
                                    Num = grup.Count()
                                };

            var checkWarmVisit = from c in _db.Visits
                                 where SalesList.Contains(c.SalesId) && c.VisitDate >= StartDate && c.VisitDate <= EndDate && c.Status == "WARM"
                                 group c by c.SalesId into grup
                                 select
                                 new
                                 {
                                     SalesId = grup.FirstOrDefault().SalesId,
                                     Num = grup.Count()
                                 };

            Dictionary<int, int> arrayWarm = new Dictionary<int, int>();
            if (checkWarmCall.Count() > 0)
            {
                foreach (var call in checkWarmCall)
                {
                    int key = call.SalesId;
                    int val = call.Num;
                    arrayWarm[key] = val;
                }
            }
            if (checkWarmVisit.Count() > 0)
            {
                foreach (var visit in checkWarmVisit)
                {
                    int key = visit.SalesId;
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
            int numWarmTotal = 0;

            //Check Hot
            var checkHotCall = from c in _db.Calls
                               where SalesList.Contains(c.SalesId) && c.CallDate >= StartDate && c.CallDate <= EndDate && c.Status == "HOT"
                               group c by c.SalesId into grup
                               select
                               new
                               {
                                   SalesId = grup.FirstOrDefault().SalesId,
                                   Num = grup.Count()
                               };

            var checkHotVisit = from c in _db.Visits
                                where SalesList.Contains(c.SalesId) && c.VisitDate >= StartDate && c.VisitDate <= EndDate && c.Status == "HOT"
                                group c by c.SalesId into grup
                                select
                                new
                                {
                                    SalesId = grup.FirstOrDefault().SalesId,
                                    Num = grup.Count()
                                };

            Dictionary<int, int> arrayHot = new Dictionary<int, int>();
            if (checkHotCall.Count() > 0)
            {
                foreach (var call in checkHotCall)
                {
                    int key = call.SalesId;
                    int val = call.Num;
                    arrayHot[key] = val;
                }
            }
            if (checkHotVisit.Count() > 0)
            {
                foreach (var visit in checkHotVisit)
                {
                    int key = visit.SalesId;
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
            int numHotTotal = 0;

            //Check SME Loan
            var checkSME = from c in _db.NasabahProducts
                           where (c.SalesId != null && SalesList.Contains(c.SalesId.Value)) && c.LastUpdate >= StartDate && c.LastUpdate <= EndDate && c.Status == "BOOKING" && c.Product.Code == "SME Loan"
                           group c by c.SalesId into grup
                           select
                           new
                           {
                               SalesId = grup.FirstOrDefault().SalesId,
                               Num = grup.Count(),
                               Total = grup.Sum(d => d.Amount)
                           };

            Dictionary<int, string> arraySME = new Dictionary<int, string>();
            Dictionary<int, decimal> arraySMEAmount = new Dictionary<int, decimal>();
            if (checkSME.Count() > 0)
            {
                foreach (var sme in checkSME)
                {
                    int key = sme.SalesId.GetValueOrDefault();
                    string val = sme.Num.ToString();
                    arraySME[key] = val;
                    arraySMEAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                }
            }


            //Check CA
            var checkCA = from c in _db.NasabahProducts
                          where SalesList.Contains(c.SalesId.Value) && c.LastUpdate >= StartDate && c.LastUpdate <= EndDate && c.Status == "BOOKING" && c.Product.Code == "CA"
                          group c by c.SalesId into grup
                          select
                          new
                          {
                              SalesId = grup.FirstOrDefault().SalesId,
                              Num = grup.Count(),
                              Total = grup.Sum(d => d.Amount)
                          };

            Dictionary<int, string> arrayCA = new Dictionary<int, string>();
            Dictionary<int, decimal> arrayCAAmount = new Dictionary<int, decimal>();
            if (checkCA.Count() > 0)
            {
                foreach (var ca in checkCA)
                {
                    int key = ca.SalesId.GetValueOrDefault();
                    string val = ca.Num.ToString();
                    arrayCA[key] = val;
                    arrayCAAmount[key] = (ca.Total.GetValueOrDefault() != 0) ? ca.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check SA
            var checkSA = from c in _db.NasabahProducts
                          where SalesList.Contains(c.SalesId.Value) && c.LastUpdate >= StartDate && c.LastUpdate <= EndDate && c.Status == "BOOKING" && c.Product.Code == "SA"
                          group c by c.SalesId into grup
                          select
                          new
                          {
                              SalesId = grup.FirstOrDefault().SalesId,
                              Num = grup.Count(),
                              Total = grup.Sum(d => d.Amount)
                          };

            Dictionary<int, string> arraySA = new Dictionary<int, string>();
            Dictionary<int, decimal> arraySAAmount = new Dictionary<int, decimal>();
            if (checkSA.Count() > 0)
            {
                foreach (var sme in checkSA)
                {
                    int key = sme.SalesId.GetValueOrDefault();
                    string val = sme.Num.ToString();
                    arraySA[key] = val;
                    arraySAAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check SAPAY
            var checkSAPAY = from c in _db.NasabahProducts
                             where SalesList.Contains(c.SalesId.Value) && c.LastUpdate >= StartDate && c.LastUpdate <= EndDate && c.Status == "BOOKING" && c.Product.Code == "SA PAY"
                             group c by c.SalesId into grup
                             select
                             new
                             {
                                 SalesId = grup.FirstOrDefault().SalesId,
                                 Num = grup.Count(),
                                 Total = grup.Sum(d => d.Amount)
                             };

            Dictionary<int, string> arraySAPAY = new Dictionary<int, string>();
            Dictionary<int, decimal> arraySAPAYAmount = new Dictionary<int, decimal>();
            if (checkSAPAY.Count() > 0)
            {
                foreach (var sme in checkSAPAY)
                {
                    int key = sme.SalesId.GetValueOrDefault();
                    string val = sme.Num.ToString();
                    arraySAPAY[key] = val;
                    arraySAPAYAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check TD
            var checkTD = from c in _db.NasabahProducts
                          where SalesList.Contains(c.SalesId.Value) && c.LastUpdate >= StartDate && c.LastUpdate <= EndDate && c.Status == "BOOKING" && c.Product.Code == "TD"
                          group c by c.SalesId into grup
                          select
                          new
                          {
                              SalesId = grup.FirstOrDefault().SalesId,
                              Num = grup.Count(),
                              Total = grup.Sum(d => d.Amount)
                          };

            Dictionary<int, string> arrayTD = new Dictionary<int, string>();
            Dictionary<int, decimal> arrayTDAmount = new Dictionary<int, decimal>();
            if (checkTD.Count() > 0)
            {
                foreach (var sme in checkTD)
                {
                    int key = sme.SalesId.GetValueOrDefault();
                    string val = sme.Num.ToString();
                    arrayTD[key] = val;
                    arrayTDAmount[key] = (sme.Total.GetValueOrDefault() != 0) ? sme.Total.GetValueOrDefault() / pembagi : 0;
                }
            }

            //Check Referral EB Loan
            var checkRefferal = _db.SalesNasabahs.Where(c => SalesList.Contains(c.RefferalFrom.Value));
            Dictionary<int, int> arrayEB = new Dictionary<int, int>();
            Dictionary<int, int> arrayKPM = new Dictionary<int, int>();
            Dictionary<int, int> arrayCC = new Dictionary<int, int>();
            Dictionary<int, int> arrayKPR = new Dictionary<int, int>();
            Dictionary<int, int> arrayBANC = new Dictionary<int, int>();

            if (checkRefferal.Count() > 0)
            {
                foreach (SalesNasabah sn in checkRefferal)
                {
                    var checkEB = from c in _db.NasabahProducts
                                  where c.SalesId == sn.SalesId && c.LastUpdate >= StartDate && c.LastUpdate <= EndDate && c.Status == "BOOKING" && c.Product.Code == "SME Loan"
                                  select c;

                    if (checkEB.Count() > 0)
                    {
                        int key = sn.RefferalFrom.GetValueOrDefault();
                        arrayEB[key] = (arrayEB.ContainsKey(key)) ? arrayEB[key] + 1 : 1;
                    }

                    var checkKPM = from c in _db.NasabahProducts
                                   where c.SalesId == sn.SalesId && c.LastUpdate >= StartDate && c.LastUpdate <= EndDate && c.Status == "BOOKING" && c.Product.Code == "KPM"
                                   select c;

                    if (checkKPM.Count() > 0)
                    {
                        int key = sn.RefferalFrom.GetValueOrDefault();
                        arrayKPM[key] = (arrayKPM.ContainsKey(key)) ? arrayKPM[key] + 1 : 1;
                    }

                    var checkCC = from c in _db.NasabahProducts
                                  where c.SalesId == sn.SalesId && c.LastUpdate >= StartDate && c.LastUpdate <= EndDate && c.Status == "BOOKING" && c.Product.Code == "CC"
                                  select c;

                    if (checkCC.Count() > 0)
                    {
                        int key = sn.RefferalFrom.GetValueOrDefault();
                        arrayCC[key] = (arrayCC.ContainsKey(key)) ? arrayCC[key] + 1 : 1;
                    }

                    var checkKPR = from c in _db.NasabahProducts
                                   where c.SalesId == sn.SalesId && c.LastUpdate >= StartDate && c.LastUpdate <= EndDate && c.Status == "BOOKING" && c.Product.Code == "KPR"
                                   select c;

                    if (checkKPR.Count() > 0)
                    {
                        int key = sn.RefferalFrom.GetValueOrDefault();
                        arrayKPR[key] = (arrayKPR.ContainsKey(key)) ? arrayKPR[key] + 1 : 1;
                    }

                    var checkBANC = from c in _db.NasabahProducts
                                    where c.SalesId == sn.SalesId && c.LastUpdate >= StartDate && c.LastUpdate <= EndDate && c.Status == "BOOKING" && c.Product.Code == "INS"
                                    select c;

                    if (checkBANC.Count() > 0)
                    {
                        int key = sn.RefferalFrom.GetValueOrDefault();
                        arrayBANC[key] = (arrayBANC.ContainsKey(key)) ? arrayBANC[key] + 1 : 1;
                    }
                }
            }


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

            int call2Visit = 0;
            int call2VisitTotal = 0;
            int visit2Booking = 0;
            int visit2BookingTotal = 0;
            int call2Booking = 0;
            int call2BookingTotal = 0;

            //loop start

            foreach (Sale item in checkSales)
            {
                int key = item.SalesId;

                numExSAPAY = 0;
                numREF = 0;

                string numCall = (arrayCall.ContainsKey(key)) ? arrayCall[key] : "&nbsp;";
                if (arrayCall.ContainsKey(key))
                {
                    numCallTotal += int.Parse(arrayCall[key]);
                }

                string numVisit = (arrayVisit.ContainsKey(key)) ? arrayVisit[key] : "&nbsp;";
                if (arrayVisit.ContainsKey(key))
                {
                    numVisitTotal += int.Parse(arrayVisit[key]);
                }

                int numWarm = (arrayWarm.ContainsKey(key)) ? arrayWarm[key] : 0;
                string strWarm = (numWarm != 0) ? numWarm.ToString() : "&nbsp;";
                if (arrayWarm.ContainsKey(key))
                {
                    numWarmTotal += arrayWarm[key];
                }

                int numHot = (arrayHot.ContainsKey(key)) ? arrayHot[key] : 0;
                string strHot = (numHot != 0) ? numHot.ToString() : "&nbsp;";
                if (arrayHot.ContainsKey(key))
                {
                    numHotTotal += arrayHot[key];
                }

                string numSME = (arraySME.ContainsKey(key)) ? arraySME[key] : "&nbsp;";
                if (arraySME.ContainsKey(key))
                {
                    numSMETotal += int.Parse(arraySME[key]);
                    numCIF++;
                    numCIFTotal++;
                }

                decimal numSMEAmount = (arraySMEAmount.ContainsKey(key)) ? arraySMEAmount[key] : 0;
                string strSMEAmount = (numSMEAmount != 0) ? numSMEAmount.ToString("F0") : "&nbsp;";
                if (arraySMEAmount.ContainsKey(key))
                {
                    numSMEAmountTotal += arraySMEAmount[key];
                }

                string numCA = (arrayCA.ContainsKey(key)) ? arrayCA[key] : "&nbsp;";
                if (arrayCA.ContainsKey(key))
                {
                    numCATotal += int.Parse(arrayCA[key]);
                    numExSAPAY += int.Parse(arrayCA[key]);
                    numExSAPAYTotal += int.Parse(arrayCA[key]);
                    numCIF++;
                    numCIFTotal++;
                }

                decimal numCAAmount = (arrayCAAmount.ContainsKey(key)) ? arrayCAAmount[key] : 0;
                string strCAAmount = (numCAAmount != 0) ? numCAAmount.ToString("F0") : "&nbsp;";
                if (arrayCAAmount.ContainsKey(key))
                {
                    numCAAmountTotal += arrayCAAmount[key];
                }

                string numSA = (arraySA.ContainsKey(key)) ? arraySA[key] : "&nbsp;";
                if (arraySA.ContainsKey(key))
                {
                    numSATotal += int.Parse(arraySA[key]);
                    numExSAPAY += int.Parse(arraySA[key]);
                    numExSAPAYTotal += int.Parse(arraySA[key]);
                    numCIF++;
                    numCIFTotal++;
                }

                decimal numSAAmount = (arraySAAmount.ContainsKey(key)) ? arraySAAmount[key] : 0;
                string strSAAmount = (numSAAmount != 0) ? numSAAmount.ToString("F0") : "&nbsp;";
                if (arraySAAmount.ContainsKey(key))
                {
                    numSAAmountTotal += arraySAAmount[key];
                }

                string numSAPAY = (arraySAPAY.ContainsKey(key)) ? arraySAPAY[key] : "&nbsp;";
                if (arraySAPAY.ContainsKey(key))
                {
                    numSAPAYTotal += int.Parse(arraySAPAY[key]);
                    numCIF++;
                    numCIFTotal++;
                }

                decimal numSAPAYAmount = (arraySAPAYAmount.ContainsKey(key)) ? arraySAPAYAmount[key] : 0;
                string strSAPAYAmount = (numSAPAYAmount != 0) ? numSAPAYAmount.ToString("F0") : "&nbsp;";
                if (arraySAPAYAmount.ContainsKey(key))
                {
                    numSAPAYAmountTotal += arraySAPAYAmount[key];
                }

                string numTD = (arrayTD.ContainsKey(key)) ? arrayTD[key] : "&nbsp;";
                if (arrayTD.ContainsKey(key))
                {
                    numTDTotal += int.Parse(arrayTD[key]);
                    numExSAPAY += int.Parse(arrayTD[key]);
                    numExSAPAYTotal += int.Parse(arrayTD[key]);
                    numCIF++;
                    numCIFTotal++;
                }

                decimal numTDAmount = (arrayTDAmount.ContainsKey(key)) ? arrayTDAmount[key] : 0;
                string strTDAmount = (numTDAmount != 0) ? numTDAmount.ToString("F0") : "&nbsp;";
                if (arrayTDAmount.ContainsKey(key))
                {
                    numTDAmountTotal += arrayTDAmount[key];
                }


                string numEB = (arrayEB.ContainsKey(key)) ? arrayEB[key].ToString() : "&nbsp;";
                if (arrayEB.ContainsKey(key))
                {
                    numEBTotal += arrayEB[key];
                    numREF += arrayEB[key];
                    numREFTotal += arrayEB[key];
                    numCIF++;
                    numCIFTotal++;
                }

                string numKPM = (arrayKPM.ContainsKey(key)) ? arrayKPM[key].ToString() : "&nbsp;";
                if (arrayKPM.ContainsKey(key))
                {
                    numKPMTotal += arrayKPM[key];
                    numREF += arrayKPM[key];
                    numREFTotal += arrayKPM[key];
                    numCIF++;
                    numCIFTotal++;
                }

                string numCC = (arrayCC.ContainsKey(key)) ? arrayCC[key].ToString() : "&nbsp;";
                if (arrayCC.ContainsKey(key))
                {
                    numCCTotal += arrayCC[key];
                    numREF += arrayCC[key];
                    numREFTotal += arrayCC[key];
                    numCIF++;
                    numCIFTotal++;
                }

                string numKPR = (arrayKPR.ContainsKey(key)) ? arrayKPR[key].ToString() : "&nbsp;";
                if (arrayKPR.ContainsKey(key))
                {
                    numKPRTotal += arrayKPR[key];
                    numREF += arrayKPR[key];
                    numREFTotal += arrayKPR[key];
                    numCIF++;
                    numCIFTotal++;
                }

                string numBANC = (arrayBANC.ContainsKey(key)) ? arrayBANC[key].ToString() : "&nbsp;";
                if (arrayBANC.ContainsKey(key))
                {
                    numBANCTotal += arrayBANC[key];
                    numREF += arrayBANC[key];
                    numREFTotal += arrayBANC[key];
                    numCIF++;
                    numCIFTotal++;
                }

                msg += "<tr>" +
                        "<td>" + item.Name + "</td>" +
                        "<td style=\"text-align:center\">" + item.Npk + "</td>" +
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

            }

            msg += "<tr style=\"border:2px solid #000\">" +
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

            msg += "</table>";

            ViewBag.Output = msg;
            string view = (export == 1) ? "list_monthly_export" : "list_monthly";

            return View(view);
        }

    }
}

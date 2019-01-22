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
    public class UserAllLogController : CommonController
    {
        public ActionResult Index()
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINUSER"))) return RedirectToAction("Index", "Home");

            return View();
        }
        //data: "page=" + page + "&perpage=" + perpage + "&DateTo=" + DateTo +"&DateEnd="+DateEnd+"&Action="+Action+"&Npk="+Npk+"&Name="+Name +"&orderBy=" + orderBy + "&orderMode=" + orderMode,
        public ActionResult list(int page = 1, int perpage = 20, string orderBy = "", string orderMode = "", string DateTo = "", string DateEnd = "", string Action = "", string Npk = "", string NpkUser = "", string Name = "", string UserName = "")
        {
            if (!((CommonModel.UserRole() == "ADMIN") || (CommonModel.UserRole() == "ADMINUSER"))) return RedirectToAction("Index", "Home");

            double cur_page = page;
            page -= 1;
            int per_page = perpage;
            bool first_btn = true;
            bool last_btn = true;
            bool previous_btn = true;
            bool next_btn = true;
            int start = page * per_page;
            string msg = "";

            var dataAdminLogs = (from c in _db.AdminLogs select c).Where(c => 1 == 1);
            var dataSalesLogs = (from c in _db.SalesLogs select c).Where(c => 1 == 1);
            var dataBMLogs = (from c in _db.BranchManagerLogs select c).Where(c => 1 == 1);
            var dataABMLogs = (from c in _db.ABMLogs select c).Where(c => 1 == 1);
            var dataRBHLogs = (from c in _db.RBHLogs select c).Where(c => 1 == 1);
            var dataSMLogs = (from c in _db.SalesManagerLogs select c).Where(c => 1 == 1);


            //------Get data and mapping all user
            List<UserAllLogModel> modelsAll = new List<UserAllLogModel>();
            modelsAll.AddRange(LogsModel.MappingAdminsFromEFtoModel(dataAdminLogs));
            modelsAll.AddRange(LogsModel.MappingSalesFromEFtoModel(dataSalesLogs));
            modelsAll.AddRange(LogsModel.MappingSMFromEFtoModel(dataSMLogs));
            modelsAll.AddRange(LogsModel.MappingBMFromEFtoModel(dataBMLogs));
            modelsAll.AddRange(LogsModel.MappingABMFromEFtoModel(dataABMLogs));
            modelsAll.AddRange(LogsModel.MappingRBHFromEFtoModel(dataRBHLogs));

            var data = modelsAll.Where(c => 1 == 1);


            //------end Get data and mapping all user




            if (!string.IsNullOrEmpty(DateTo) && !string.IsNullOrEmpty(DateEnd))
            {
                DateTime to = DateTime.Parse(DateTo);
                DateTime end = DateTime.Parse(DateEnd);
                if (to <= end)
                {
                    data = data.Where(c => c.ActionDate > to && c.ActionDate < end);
                }
                ViewBag.OutputError += "Date To harus lebih kecil dari Date End /n";
            }
            if (!string.IsNullOrEmpty(DateTo) && string.IsNullOrEmpty(DateEnd))
            {
                DateTime to = DateTime.Parse(DateTo);
                data = data.Where(c => c.ActionDate > to);

            }
            if (string.IsNullOrEmpty(DateTo) && !string.IsNullOrEmpty(DateEnd))
            {
                DateTime end = DateTime.Parse(DateEnd);
                data = data.Where(c => c.ActionDate < end);

            }
            if (!string.IsNullOrEmpty(Action))
            {
                data = data.Where(c => c.Action == Action);

            }
            if (!string.IsNullOrEmpty(Npk))
            {
                data = data.Where(c => c.NPK.Contains(Npk));
            }

            if (!String.IsNullOrEmpty(Name))
            {
                data = data.Where(c => c.NPKUser.Contains(Name) || c.NPKUserChange.Contains(Name));
            }

            if (!String.IsNullOrEmpty(NpkUser))
            {
                data = data.Where(c => c.NPKUser.Contains(NpkUser) || c.NPKUserChange.Contains(NpkUser));
            }

            if (!String.IsNullOrEmpty(UserName))
            {
                data = data.Where(c => c.UserName.Contains(UserName) || c.UserNameChange.Contains(UserName));
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

            msg += "<table class=\"grid-style\" width=\"100%\">" +
                "<thead>" +
                    "<tr>" +
                        "<th>No</th>" +
                        "<th>Condition</th>" +
                        "<th>Area Code</th>" +
                        "<th>Area Name</th>" +
                        "<th>Region</th>" +

                        "<th>NPK</th>" +
                        "<th>Name</th>" +
                        "<th>Action</th>" +
                        "<th>Action Date</th>" +
                    "</tr>" +
                "</thead>"
                ;

            msg += "<tbody>";

            i = dataStart;

            foreach (var item in result)
            {
                string klas = (i % 2 == 0) ? "gridrow_alternate" : "gridrow";
                string date = item.ActionDate.Value.ToString("dd MMM yyyy HH:mm");

                //Data Lama
                msg += "<tr class=\"" + klas + "\">" +
                           "<td align=\"center\">" + i + "</td>" +
                           "<td align=\"center\">Current</td>" +
                           "<td>" + item.UserNameChange + "</td>" +
                           "<td>" + item.UserNameChange + "</td>" +
                           "<td>" + item.UserNameChange + "</td>" +

                           "<td>" + item.NPK + "</td>" +
                           "<td>" + item.UserName + "</td>" +
                           "<td>" + item.Action + "</td>" +
                           "<td>" + date + "</td>" +
                       "</tr>" +
                    //Data Baru
                "<tr class=\"" + klas + "\">" +
                           "<td></td>" +
                           "<td align=\"center\">Previous</td>" +
                           "<td>" + item.UserName + "</td>" +
                           "<td>" + item.UserName + "</td>" +
                           "<td>" + item.UserName + "</td>" +

                           "<td></td>" +
                           "<td></td>" +
                           "<td></td>" +
                           "<td></td>" +
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

    }
}
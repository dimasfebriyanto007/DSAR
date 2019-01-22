using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cms.Models;
using System.IO;
using System.Web.Security;
using System.Web.Routing;

namespace cms.Controllers
{
    public class CommonController : Controller
    {

        protected dsarEntities _db = new dsarEntities();
        protected User user = CommonModel.GetCurrentUser();

        public List<SelectOptionModel> YesNoOption = new List<SelectOptionModel>();
        public List<SelectOptionModel> StatusOption = new List<SelectOptionModel>();
        public List<PipelineOptionModel> PipelineOption = new List<PipelineOptionModel>();
        public List<string> AbsenReason = new List<string>();
        public User currentUser = CommonModel.GetCurrentUser();

        public CommonController()
        {
            string param = CommonModel.GetParam(3);
            if (!string.IsNullOrEmpty(param))
            {
                int w = Convert.ToInt32(param) * 60000;
                ViewBag.abc = w.ToString();

            }

            if (currentUser.UserName != null)
            {
                var checkUser = _db.Users.Where(c => c.UserName == currentUser.UserName);
                if (checkUser.Count() > 0)
                {
                    User loginUser = checkUser.First();
                    loginUser.LastActiveSession = DateTime.Now;
                    _db.ApplyCurrentValues(loginUser.EntityKey.EntitySetName, loginUser);
                    _db.SaveChanges();
                }
            }

            SelectOptionModel option1 = new SelectOptionModel();
            option1.OptionId = 1;
            option1.OptionString = "Yes";

            SelectOptionModel option2 = new SelectOptionModel();
            option2.OptionId = 0;
            option2.OptionString = "No";

            YesNoOption.Add(option1);
            YesNoOption.Add(option2);

            //-----
            SelectOptionModel option3 = new SelectOptionModel();
            option3.OptionId = 1;
            option3.OptionString = "Active";

            SelectOptionModel option4 = new SelectOptionModel();
            option4.OptionId = 0;
            option4.OptionString = "Not Active";

            StatusOption.Add(option3);
            StatusOption.Add(option4);


            //-----
            PipelineOptionModel option9 = new PipelineOptionModel();
            option9.OptionId = "COLD";
            option9.OptionString = "COLD";

            PipelineOptionModel option5 = new PipelineOptionModel();
            option5.OptionId = "WARM";
            option5.OptionString = "WARM";

            PipelineOptionModel option6 = new PipelineOptionModel();
            option6.OptionId = "HOT";
            option6.OptionString = "HOT";

            PipelineOptionModel option7 = new PipelineOptionModel();
            option7.OptionId = "BOOKING";
            option7.OptionString = "BOOKING";

            PipelineOptionModel option8 = new PipelineOptionModel();
            option8.OptionId = "CANCEL";
            option8.OptionString = "CANCEL";

            PipelineOption.Add(option9);
            PipelineOption.Add(option5);
            PipelineOption.Add(option6);
            PipelineOption.Add(option7);
            PipelineOption.Add(option8);

            AbsenReason.Add("CUTI");
            AbsenReason.Add("SAKIT");
            AbsenReason.Add("TRAINING");
            AbsenReason.Add("LAINNYA");


            if (currentUser.UserId > 0)
            {
                switch (currentUser.Role)
                {
                    case "SALES":
                        var checkSales = _db.Sales.Where(c => c.SalesId == currentUser.RelatedId);
                        if (checkSales.Count() > 0)
                        {
                            Sale data = checkSales.First();
                            ViewData["CurrentUserName"] = data.Name;
                            ViewData["CurrentUserRoleName"] = "Sales - " + data.Branch.Name;
                        }
                        break;
                    case "SM":
                        var checkSM = _db.SalesManagers.Where(c => c.ManagerId == currentUser.RelatedId);
                        if (checkSM.Count() > 0)
                        {
                            SalesManager data = checkSM.First();
                            ViewData["CurrentUserName"] = data.Name;
                            ViewData["CurrentUserRoleName"] = "SM -  " + data.Branch.Name + " - LOGIN NOW[" + currentUser.LastLoginDate + "]";
                        }
                        break;
                    case "BM":
                        var checkBM = _db.BranchManagers.Where(c => c.BmId == currentUser.RelatedId);
                        if (checkBM.Count() > 0)
                        {
                            BranchManager data = checkBM.First();
                            ViewData["CurrentUserName"] = data.Name;
                            ViewData["CurrentUserRoleName"] = "BM - LOGIN NOW[" + currentUser.LastLoginDate + "]";
                        }
                        break;
                    case "ABM":
                        var checkABM = _db.ABMs.Where(c => c.AbmId == currentUser.RelatedId);
                        if (checkABM.Count() > 0)
                        {
                            ABM data = checkABM.First();
                            ViewData["CurrentUserName"] = data.Name;
                            ViewData["CurrentUserRoleName"] = "ABM - LOGIN NOW[" + currentUser.LastLoginDate + "]";
                        }
                        break;
                    case "RBH":
                        var checkRBH = _db.RBHs.Where(c => c.RbhId == currentUser.RelatedId);
                        if (checkRBH.Count() > 0)
                        {
                            RBH data = checkRBH.First();
                            ViewData["CurrentUserName"] = data.Name;
                            ViewData["CurrentUserRoleName"] = "RBH - " + data.Region.Name;
                        }
                        break;
                    case "ADMIN":
                        ViewData["CurrentUserName"] = currentUser.UserName;
                        ViewData["CurrentUserRoleName"] = "Administrator";
                        break;
                    case "ADMINUSER":
                        var checkAdmin = _db.Admins.Where(c => c.UserID == currentUser.UserId);
                        if (checkAdmin.Count() > 0)
                        {
                            Admin data = checkAdmin.First();
                            ViewData["CurrentUserName"] = data.Name;
                            ViewData["CurrentUserRoleName"] = "Admin User";
                        }
                        break;
                    case "ADMINBUSSINES":
                        var checkAdminS = _db.Admins.Where(c => c.UserID == currentUser.UserId);
                        if (checkAdminS.Count() > 0)
                        {
                            Admin data = checkAdminS.First();
                            ViewData["CurrentUserName"] = data.Name;
                            ViewData["CurrentUserRoleName"] = "Admin Bussines";
                        }
                        break;
                }
            }

        }


    }
}
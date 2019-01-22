using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.NetworkInformation;
using System.Net;

namespace cms.Models
{
    public class LogsModel
    {
        public static string SaveLogProduct(Product ProductDataOld, Product ProductDataNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                string DescCategoryOld = (ProductDataOld.CategoryId == 0) ? "" : (from m in _db.ProductCategories
                                                                                  where m.CategoryId == ProductDataOld.CategoryId
                                                                                  select m.Name).First();
                string DescCategoryNew = (ProductDataNew.CategoryId == 0) ? "" : (from m in _db.ProductCategories
                                                                                  where m.CategoryId == ProductDataNew.CategoryId
                                                                                  select m.Name).First();
                ProductsLog ProducLogData = new ProductsLog();
                ProducLogData.ProductId = (ProductDataNew.ProductId == 0) ? ProductDataOld.ProductId : ProductDataNew.ProductId;

                //-- data lama
                ProducLogData.CategoryDesc = (Action == "Add") ? "" : DescCategoryOld;
                ProducLogData.Code = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(ProductDataOld.Code)) ? "" : ProductDataOld.Code);
                ProducLogData.Name = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(ProductDataOld.Name)) ? "" : ProductDataOld.Name);
                ProducLogData.Description = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(ProductDataOld.Description)) ? "" : ProductDataOld.Description);
                ProducLogData.ImageName = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(ProductDataOld.ImageName)) ? "" : ProductDataOld.ImageName);

                //-- data baru
                ProducLogData.CategoryDescChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(DescCategoryNew)) ? "" : DescCategoryNew) :
                    "";
                ProducLogData.CodeChane = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(ProductDataNew.Code)) ? "" : ProductDataNew.Code) :
                    "";
                ProducLogData.NameChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(ProductDataNew.Name)) ? "" : ProductDataNew.Name) :
                    "";
                ProducLogData.DescriptionChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(ProductDataNew.Description)) ? "" : ProductDataNew.Description) :
                    "";
                ProducLogData.ImageNameChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(ProductDataNew.ImageName)) ? "" : ProductDataNew.ImageName) :
                    "";

                ProducLogData.Action = Action;
                ProducLogData.Date = DateTime.Now;
                ProducLogData.UserName = UserDetail.Name;
                ProducLogData.NIK = UserDetail.NPK;

                _db.AddToProductsLogs(ProducLogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

                message = ex.Message;
            }
            return message;
        }
        public static string SaveLogProductVariant(ProductVariant ProductVariantDataOld, ProductVariant ProductVariantDataNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                string DescProductOld = (ProductVariantDataOld.ProductId == 0) ? "" : (from m in _db.Products
                                                                                       where m.ProductId == ProductVariantDataOld.ProductId
                                                                                       select m.Name).First();
                string DescProductNew = (ProductVariantDataNew.ProductId == 0) ? "" : (from m in _db.Products
                                                                                       where m.ProductId == ProductVariantDataNew.ProductId
                                                                                       select m.Name).First();
                ProductVariantsLog ProducVariantLogData = new ProductVariantsLog();

                ProducVariantLogData.VariantId = (ProductVariantDataOld.VariantId == 0) ? ProductVariantDataNew.VariantId : ProductVariantDataOld.VariantId;

                //-- Data lama
                ProducVariantLogData.ProductDesc = (Action == "Add") ? "" :
                    DescProductOld;
                ProducVariantLogData.Name = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(ProductVariantDataOld.Name)) ? "" : ProductVariantDataOld.Name);

                //-- Data Baru
                ProducVariantLogData.ProductDescChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(DescProductNew)) ? "" : DescProductNew) :
                    "";
                ProducVariantLogData.NameChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(ProductVariantDataNew.Name)) ? "" : ProductVariantDataNew.Name) :
                    "";

                ProducVariantLogData.Action = Action;
                ProducVariantLogData.Date = DateTime.Now;
                ProducVariantLogData.UserName = UserDetail.Name;
                ProducVariantLogData.NPK = UserDetail.NPK;

                _db.AddToProductVariantsLogs(ProducVariantLogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }
        public static string SaveLogProductCategory(ProductCategory ProductCategoriDataOld, ProductCategory ProductCategoriDataNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                //string DescProductOld = (ProductVariantDataOld.ProductId == 0) ? "" : (from m in _db.Products
                //                                                                       where m.ProductId == ProductVariantDataOld.ProductId
                //                                                                       select m.Name).First();
                //string DescProductNew = (ProductVariantDataNew.ProductId == 0) ? "" : (from m in _db.Products
                //                                                                       where m.ProductId == ProductVariantDataNew.ProductId
                //                                                                       select m.Name).First();
                ProductCategoriesLog ProducCategoriLogData = new ProductCategoriesLog();

                ProducCategoriLogData.CategoryId = (ProductCategoriDataOld.CategoryId == 0) ? ProductCategoriDataNew.CategoryId : ProductCategoriDataOld.CategoryId;

                //-- Data lama
                ProducCategoriLogData.Name = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(ProductCategoriDataOld.Name)) ? "" : ProductCategoriDataOld.Name);

                ProducCategoriLogData.Description = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(ProductCategoriDataOld.Description)) ? "" : ProductCategoriDataOld.Description);

                //-- Data Baru
                ProducCategoriLogData.NameChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(ProductCategoriDataNew.Name)) ? "" : ProductCategoriDataNew.Name) :
                    "";
                ProducCategoriLogData.DescriptionChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(ProductCategoriDataNew.Description)) ? "" : ProductCategoriDataNew.Description) :
                    "";

                ProducCategoriLogData.Action = Action;
                ProducCategoriLogData.Date = DateTime.Now;
                ProducCategoriLogData.UserName = UserDetail.Name;
                ProducCategoriLogData.NPK = UserDetail.NPK;

                _db.AddToProductCategoriesLogs(ProducCategoriLogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public static string SaveLogRegion(Region RegionOld, Region RegionNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                //string DescProductOld = (ProductVariantDataOld.ProductId == 0) ? "" : (from m in _db.Products
                //                                                                       where m.ProductId == ProductVariantDataOld.ProductId
                //                                                                       select m.Name).First();
                //string DescProductNew = (ProductVariantDataNew.ProductId == 0) ? "" : (from m in _db.Products
                //                                                                       where m.ProductId == ProductVariantDataNew.ProductId
                //                                                                       select m.Name).First();
                RegionsLog RegionLogData = new RegionsLog();

                //RegionLogData.RegionCode = (string.IsNullOrEmpty(RegionNew.RegionCode)) ? RegionOld.RegionCode : RegionNew.RegionCode;

                //-- Data lama
                RegionLogData.RegionCode = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(RegionOld.RegionCode)) ? "" : RegionOld.RegionCode);
                RegionLogData.Name = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(RegionOld.Name)) ? "" : RegionOld.Name);

                //-- Data Baru
                RegionLogData.RegionCodeChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(RegionNew.RegionCode)) ? "" : RegionNew.RegionCode) :
                    "";
                RegionLogData.NameChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(RegionNew.Name)) ? "" : RegionNew.Name) :
                    "";

                RegionLogData.Action = Action;
                RegionLogData.Date = DateTime.Now;
                RegionLogData.UserName = UserDetail.Name;
                RegionLogData.Npk = UserDetail.NPK;

                _db.AddToRegionsLogs(RegionLogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public static string SaveLogNews(News NewsOld, News NewsNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                NewsLog NewsLogData = new NewsLog();

                NewsLogData.NewsId = (NewsNew.NewsId == 0 || NewsNew.NewsId == null) ? NewsOld.NewsId : NewsNew.NewsId;

                //-- Data lama
                NewsLogData.Title = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(NewsOld.Title)) ? "" : NewsOld.Title);
                NewsLogData.IntroText = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(NewsOld.IntroText)) ? "" : NewsOld.IntroText);
                NewsLogData.ContentText = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(NewsOld.ContentText)) ? "" : NewsOld.ContentText);
                NewsLogData.Publish = (Action == "Add") ? "" :
                    ((NewsOld.Publish == 0 || NewsOld.Publish == null) ? "0" : NewsOld.Publish.ToString());
                NewsLogData.NewsDate = (Action == "Add") ? (DateTime?)null :
                                        ((NewsOld.NewsDate == null) ? (DateTime?)null : NewsOld.NewsDate);

                //-- Data Baru
                NewsLogData.TitleChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(NewsNew.Title)) ? "" : NewsNew.Title) :
                    "";
                NewsLogData.IntroTextChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(NewsNew.IntroText)) ? "" : NewsNew.IntroText) :
                    "";
                NewsLogData.ContentTextChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(NewsNew.ContentText)) ? "" : NewsNew.ContentText) :
                    "";
                NewsLogData.PublishChange = (Action == "Add" || Action == "Edit") ? (NewsNew.Publish == 0 || NewsNew.Publish == null) ? "0" : NewsNew.Publish.ToString() :
                    "";
                NewsLogData.NewsDateChange = (Action == "Add" || Action == "Edit") ? (NewsNew.NewsDate == null) ? ((DateTime?)null) : NewsNew.NewsDate :
                    (DateTime?)null;

                NewsLogData.Action = Action;
                NewsLogData.Date = DateTime.Now;
                NewsLogData.UserName = UserDetail.Name;
                NewsLogData.NPKUser = UserDetail.NPK;

                _db.AddToNewsLogs(NewsLogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public static string SaveLogArea(Area AreaOld, Area AreaNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                string DescRegionOld = (string.IsNullOrEmpty(AreaOld.RegionCode)) ? "" : (from m in _db.Regions
                                                                                          where m.RegionCode == AreaOld.RegionCode
                                                                                          select m.Name).First();
                string DescRegionNew = (string.IsNullOrEmpty(AreaNew.RegionCode)) ? "" : (from m in _db.Regions
                                                                                          where m.RegionCode == AreaNew.RegionCode
                                                                                          select m.Name).First();
                AreasLog AreaLogData = new AreasLog();

                //ProducCategoriLogData.CategoryId = (ProductCategoriDataOld.CategoryId == 0) ? ProductCategoriDataNew.CategoryId : ProductCategoriDataOld.CategoryId;

                //-- Data lama
                AreaLogData.AreaDesc = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(AreaOld.AreaCode)) ? "" : AreaOld.AreaCode);

                AreaLogData.RegionDesc = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DescRegionOld)) ? "" : DescRegionOld);

                AreaLogData.Name = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(AreaOld.Name)) ? "" : AreaOld.Name);

                //-- Data Baru
                AreaLogData.AreaDescChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(AreaNew.AreaCode)) ? "" : AreaNew.AreaCode) :
                    "";
                AreaLogData.RegionDescChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(DescRegionNew)) ? "" : DescRegionNew) :
                    "";
                AreaLogData.NameChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(AreaNew.Name)) ? "" : AreaNew.Name) :
                    "";

                AreaLogData.Action = Action;
                AreaLogData.Date = DateTime.Now;
                AreaLogData.UserName = UserDetail.Name;
                AreaLogData.NPK = UserDetail.NPK;

                _db.AddToAreasLogs(AreaLogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public static string SaveLogBranch(Branch BranchOld, Branch BranchNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                string DescAreaOld = (string.IsNullOrEmpty(BranchOld.AreaCode)) ? "" : (from m in _db.Areas
                                                                                        where m.AreaCode == BranchOld.AreaCode
                                                                                        select m.Name).First();
                string DescAreaNew = (string.IsNullOrEmpty(BranchNew.AreaCode)) ? "" : (from m in _db.Areas
                                                                                        where m.AreaCode == BranchNew.AreaCode
                                                                                        select m.Name).First();
                BranchsLog LogData = new BranchsLog();

                //ProducCategoriLogData.CategoryId = (ProductCategoriDataOld.CategoryId == 0) ? ProductCategoriDataNew.CategoryId : ProductCategoriDataOld.CategoryId;

                //-- Data lama
                LogData.BrancCodeDesc = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(BranchOld.BranchCode)) ? "" : BranchOld.BranchCode);

                LogData.AreaCodeDesc = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DescAreaOld)) ? "" : DescAreaOld);

                LogData.Name = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(BranchOld.Name)) ? "" : BranchOld.Name);

                //-- Data Baru
                LogData.BrancCodeDescChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(BranchNew.BranchCode)) ? "" : BranchNew.BranchCode) :
                    "";
                LogData.AreaCodeDescChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(DescAreaNew)) ? "" : DescAreaNew) :
                    "";
                LogData.NameChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(BranchNew.Name)) ? "" : BranchNew.Name) :
                    "";

                LogData.Action = Action;
                LogData.Date = DateTime.Now;
                LogData.UserName = UserDetail.Name;
                LogData.NPK = UserDetail.NPK;

                _db.AddToBranchsLogs(LogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public static string SaveLogCallReason(CallReason DataOld, CallReason DataNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                //string DescAreaOld = (string.IsNullOrEmpty(BranchOld.AreaCode)) ? "" : (from m in _db.Areas
                //                                                                        where m.AreaCode == BranchOld.AreaCode
                //                                                                        select m.Name).First();
                //string DescAreaNew = (string.IsNullOrEmpty(BranchNew.AreaCode)) ? "" : (from m in _db.Areas
                //                                                                        where m.AreaCode == BranchNew.AreaCode
                //                                                                        select m.Name).First();
                CallReasonsLog LogData = new CallReasonsLog();

                //ProducCategoriLogData.CategoryId = (ProductCategoriDataOld.CategoryId == 0) ? ProductCategoriDataNew.CategoryId : ProductCategoriDataOld.CategoryId;

                //-- Data lama
                LogData.ReasonId = (Action == "Add") ? "" :
                    (DataOld.ReasonId == 0) ? "" : DataOld.ReasonId.ToString();

                //LogData.AreaCodeDesc = (Action == "Add") ? "" :
                //    ((string.IsNullOrEmpty(DataOld)) ? "" : DataOld);

                LogData.Description = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Description)) ? "" : DataOld.Description);

                //-- Data Baru
                LogData.ReasonIdChange = (Action == "Add" || Action == "Edit") ? ((DataNew.ReasonId == 0) ? "" : DataNew.ReasonId.ToString()) :
                    "";
                //LogData.AreaCodeDescChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(DataNew)) ? "" : DataNew) :
                //    "";
                LogData.Description = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(DataNew.Description)) ? "" : DataNew.Description) :
                    "";

                LogData.Action = Action;
                LogData.Date = DateTime.Now;
                LogData.UserName = UserDetail.Name;
                LogData.Npk = UserDetail.NPK;

                _db.AddToCallReasonsLogs(LogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public static string SaveLogVisitReason(VisitReason DataOld, VisitReason DataNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                //string DescAreaOld = (string.IsNullOrEmpty(BranchOld.AreaCode)) ? "" : (from m in _db.Areas
                //                                                                        where m.AreaCode == BranchOld.AreaCode
                //                                                                        select m.Name).First();
                //string DescAreaNew = (string.IsNullOrEmpty(BranchNew.AreaCode)) ? "" : (from m in _db.Areas
                //                                                                        where m.AreaCode == BranchNew.AreaCode
                //                                                                        select m.Name).First();
                VisitReasonLog LogData = new VisitReasonLog();

                //ProducCategoriLogData.CategoryId = (ProductCategoriDataOld.CategoryId == 0) ? ProductCategoriDataNew.CategoryId : ProductCategoriDataOld.CategoryId;

                //-- Data lama
                LogData.ReasonId = (Action == "Add") ? "" :
                    (DataOld.ReasonId == 0) ? "" : DataOld.ReasonId.ToString();

                //LogData.AreaCodeDesc = (Action == "Add") ? "" :
                //    ((string.IsNullOrEmpty(DataOld)) ? "" : DataOld);

                LogData.Description = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Description)) ? "" : DataOld.Description);

                //-- Data Baru
                LogData.ReasonIdChange = (Action == "Add" || Action == "Edit") ? ((DataNew.ReasonId == 0) ? "" : DataNew.ReasonId.ToString()) :
                    "";
                //LogData.AreaCodeDescChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(DataNew)) ? "" : DataNew) :
                //    "";
                LogData.Description = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(DataNew.Description)) ? "" : DataNew.Description) :
                    "";

                LogData.Action = Action;
                LogData.Date = DateTime.Now;
                LogData.UserName = UserDetail.Name;
                LogData.Npk = UserDetail.NPK;

                _db.AddToVisitReasonLogs(LogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public static string SaveLogSalesTeam(SalesTeam DataOld, SalesTeam DataNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                //string DescAreaOld = (string.IsNullOrEmpty(BranchOld.AreaCode)) ? "" : (from m in _db.Areas
                //                                                                        where m.AreaCode == BranchOld.AreaCode
                //                                                                        select m.Name).First();
                //string DescAreaNew = (string.IsNullOrEmpty(BranchNew.AreaCode)) ? "" : (from m in _db.Areas
                //                                                                        where m.AreaCode == BranchNew.AreaCode
                //                                                                        select m.Name).First();
                SalesTeamLog LogData = new SalesTeamLog();

                //ProducCategoriLogData.CategoryId = (ProductCategoriDataOld.CategoryId == 0) ? ProductCategoriDataNew.CategoryId : ProductCategoriDataOld.CategoryId;

                //-- Data lama
                LogData.TeamIdDesc = (Action == "Add") ? "" :
                    (DataOld.TeamId == 0) ? "" : DataOld.TeamId.ToString();

                //LogData.AreaCodeDesc = (Action == "Add") ? "" :
                //    ((string.IsNullOrEmpty(DataOld)) ? "" : DataOld);

                LogData.Name = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Name)) ? "" : DataOld.Name);

                //-- Data Baru
                LogData.TeamIdDescChange = (Action == "Add" || Action == "Edit") ? ((DataNew.TeamId == 0) ? "" : DataNew.TeamId.ToString()) :
                    "";
                //LogData.AreaCodeDescChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(DataNew)) ? "" : DataNew) :
                //    "";
                LogData.NameChange = (Action == "Add" || Action == "Edit") ? ((string.IsNullOrEmpty(DataNew.Name)) ? "" : DataNew.Name) :
                    "";

                LogData.Action = Action;
                LogData.Date = DateTime.Now;
                LogData.UserName = UserDetail.Name;
                LogData.Npk = UserDetail.NPK;

                _db.AddToSalesTeamLogs(LogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public static string SaveLogSales(Sale DataOld, Sale DataNew, UserLogModel DataRelaOld, UserLogModel DataRelaNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                int salesId = (DataOld.SalesId == 0) ? DataNew.SalesId : DataOld.SalesId;


                string DescSalesRoleOld = (DataOld.TeamId == 0) ? "" : (from m in _db.SalesTeams
                                                                        where m.TeamId == DataOld.TeamId
                                                                        select m.Name).First();

                string DescSalesRoleNew = (DataNew.TeamId == 0) ? "" : (from m in _db.SalesTeams
                                                                        where m.TeamId == DataNew.TeamId
                                                                        select m.Name).First();

                string DescBranchOld = (string.IsNullOrEmpty(DataOld.BranchCode)) ? "" : (from m in _db.Branchs
                                                                                          where m.BranchCode == DataOld.BranchCode
                                                                                          select m.Name).First();

                string DescBranchNew = (string.IsNullOrEmpty(DataNew.BranchCode)) ? "" : (from m in _db.Branchs
                                                                                          where m.BranchCode == DataNew.BranchCode
                                                                                          select m.Name).First();

                string DescSalesManagerOld = (DataOld.SmId == null || DataOld.SmId == 0) ? "" : (from m in _db.SalesManagers
                                                                                                 where m.ManagerId == DataOld.SmId
                                                                                                 select m.Name).First();

                string DescSalesManagerNew = (DataNew.SmId == null || DataNew.SmId == 0) ? "" : (from m in _db.SalesManagers
                                                                                                 where m.ManagerId == DataNew.SmId
                                                                                                 select m.Name).First();


                SalesLog LogData = new SalesLog();

                LogData.SalesId = salesId;

                //-- Data lama
                LogData.Npk = (Action == "Add") ? "" :
                    (string.IsNullOrEmpty(DataOld.Npk)) ? "" : DataOld.Npk;

                LogData.Name = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Name)) ? "" : DataOld.Name);

                LogData.Email = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Email)) ? "" : DataOld.Email);

                LogData.Phone = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Phone)) ? "" : DataOld.Phone);

                LogData.Status = (Action == "Add") ? "" :
                    ((DataRelaOld.IsLockedOut) ? "1" : "0");

                LogData.SalesRole = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DescSalesRoleOld)) ? "" : DescSalesRoleOld);

                LogData.Branch = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DescBranchOld)) ? "" : DescBranchOld);

                LogData.SalesManager = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DescSalesManagerOld)) ? "" : DescSalesManagerOld);

                LogData.Password = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataRelaOld.Password)) ? "" : DataRelaOld.Password);
                LogData.UserNameSales = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataRelaOld.UserName)) ? "" : DataRelaOld.UserName);



                //-- Data Baru
                LogData.NpkChange = (Action == "Add" || Action == "Edit") ?
                    ((string.IsNullOrEmpty(DataNew.Npk)) ? "" : DataNew.Npk.ToString()) : "";

                LogData.NameChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Name)) ? "" : DataNew.Name) : "";

                LogData.EmailChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Email)) ? "" : DataNew.Email) : "";

                LogData.PhoneChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Phone)) ? "" : DataNew.Phone) : "";

                LogData.StatusChange = (Action == "Add" || Action == "Edit") ?
                   ((DataRelaNew.IsLockedOut) ? "1" : "0") : "";

                LogData.SalesRoleChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DescSalesRoleNew)) ? "" : DescSalesRoleNew) : "";

                LogData.BranchChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DescBranchNew)) ? "" : DescBranchNew) : "";

                LogData.SalesManagerChange = (Action == "Add" || Action == "Edit") ?
                  ((string.IsNullOrEmpty(DescSalesManagerNew)) ? "" : DescSalesManagerNew) : "";

                LogData.PasswordChange = (Action == "Add" || Action == "Edit") ?
                  ((string.IsNullOrEmpty(DataRelaNew.Password)) ? "" : DataRelaNew.Password) : "";

                LogData.UserNameSalesChange = (Action == "Add" || Action == "Edit") ?
                  ((string.IsNullOrEmpty(DataRelaNew.UserName)) ? "" : DataRelaNew.UserName) : "";

                LogData.Action = Action;
                LogData.Date = DateTime.Now;
                LogData.UserName = UserDetail.Name;
                LogData.NPKUser = UserDetail.NPK;

                _db.AddToSalesLogs(LogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public static string SaveLogSalesManager(SalesManager DataOld, SalesManager DataNew, UserLogModel DataRelaOld, UserLogModel DataRelaNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                int ManagerId = (DataOld.ManagerId == 0) ? DataNew.ManagerId : DataOld.ManagerId;


                //string DescSalesRoleOld = (DataOld.TeamId == 0) ? "" : (from m in _db.SalesTeams
                //                                                        where m.TeamId == DataOld.TeamId
                //                                                        select m.Name).First();

                //string DescSalesRoleNew = (DataNew.TeamId == 0) ? "" : (from m in _db.SalesTeams
                //                                                        where m.TeamId == DataNew.TeamId
                //                                                        select m.Name).First();

                string DescBranchOld = (string.IsNullOrEmpty(DataOld.BranchCode)) ? "" : (from m in _db.Branchs
                                                                                          where m.BranchCode == DataOld.BranchCode
                                                                                          select m.Name).First();

                string DescBranchNew = (string.IsNullOrEmpty(DataNew.BranchCode)) ? "" : (from m in _db.Branchs
                                                                                          where m.BranchCode == DataNew.BranchCode
                                                                                          select m.Name).First();

                //string DescSalesManagerOld = (DataOld.SmId == 0) ? "" : (from m in _db.SalesManagers
                //                                                         where m.ManagerId == DataOld.SmId
                //                                                         select m.Name).First();

                //string DescSalesManagerNew = (DataNew.SmId == 0) ? "" : (from m in _db.SalesManagers
                //                                                         where m.ManagerId == DataNew.SmId
                //                                                         select m.Name).First();


                SalesManagerLog LogData = new SalesManagerLog();

                LogData.ManagerId = ManagerId;

                //-- Data lama
                LogData.Npk = (Action == "Add") ? "" :
                    (string.IsNullOrEmpty(DataOld.Npk)) ? "" : DataOld.Npk;

                LogData.Name = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Name)) ? "" : DataOld.Name);

                LogData.Email = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Email)) ? "" : DataOld.Email);

                LogData.Phone = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Phone)) ? "" : DataOld.Phone);

                LogData.Status = (Action == "Add") ? "" :
                    ((DataRelaOld.IsLockedOut) ? "1" : "0");

                //LogData.SalesRole = (Action == "Add") ? "" :
                //    ((string.IsNullOrEmpty(DescSalesRoleOld)) ? "" : DescSalesRoleOld);

                LogData.Branch = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DescBranchOld)) ? "" : DescBranchOld);

                //LogData.SalesManager = (Action == "Add") ? "" :
                //    ((string.IsNullOrEmpty(DescSalesManagerOld)) ? "" : DescSalesManagerOld);

                LogData.Password = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataRelaOld.Password)) ? "" : DataRelaOld.Password);

                LogData.UserNameSalesManager = (Action == "Add") ? "" :
                   ((string.IsNullOrEmpty(DataRelaOld.UserName)) ? "" : DataRelaOld.UserName);


                //-- Data Baru
                LogData.NpkChange = (Action == "Add" || Action == "Edit") ?
                    ((string.IsNullOrEmpty(DataNew.Npk)) ? "" : DataNew.Npk.ToString()) : "";

                LogData.NameChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Name)) ? "" : DataNew.Name) : "";

                LogData.EmailChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Email)) ? "" : DataNew.Email) : "";

                LogData.PhoneChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Phone)) ? "" : DataNew.Phone) : "";

                LogData.StatusChange = (Action == "Add" || Action == "Edit") ?
                   ((DataRelaNew.IsLockedOut) ? "1" : "0") : "";

                //LogData.SalesRoleChange = (Action == "Add" || Action == "Edit") ?
                //   ((string.IsNullOrEmpty(DescSalesRoleNew)) ? "" : DescSalesRoleNew) : "";

                LogData.BranchChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DescBranchNew)) ? "" : DescBranchNew) : "";

                //LogData.SalesManagerChange = (Action == "Add" || Action == "Edit") ?
                //  ((string.IsNullOrEmpty(DescSalesManagerNew)) ? "" : DescSalesManagerNew) : "";

                LogData.PasswordChange = (Action == "Add" || Action == "Edit") ?
                  ((string.IsNullOrEmpty(DataRelaNew.Password)) ? "" : DataRelaNew.Password) : "";

                LogData.UserNameSalesManagerChange = (Action == "Add" || Action == "Edit") ?
                 ((string.IsNullOrEmpty(DataRelaNew.UserName)) ? "" : DataRelaNew.UserName) : "";

                LogData.Action = Action;
                LogData.Date = DateTime.Now;
                LogData.UserName = UserDetail.Name;
                LogData.Npk = UserDetail.NPK;

                _db.AddToSalesManagerLogs(LogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public static string SaveLogBranchManager(BranchManager DataOld, BranchManager DataNew, UserLogModel DataRelaOld, UserLogModel DataRelaNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                int BmId = (DataOld.BmId == 0) ? DataNew.BmId : DataOld.BmId;


                //string DescSalesRoleOld = (DataOld.TeamId == 0) ? "" : (from m in _db.SalesTeams
                //                                                        where m.TeamId == DataOld.TeamId
                //                                                        select m.Name).First();

                //string DescSalesRoleNew = (DataNew.TeamId == 0) ? "" : (from m in _db.SalesTeams
                //                                                        where m.TeamId == DataNew.TeamId
                //                                                        select m.Name).First();

                string DescBranchOld = (string.IsNullOrEmpty(DataOld.BranchCode)) ? "" : (from m in _db.Branchs
                                                                                          where m.BranchCode == DataOld.BranchCode
                                                                                          select m.Name).First();

                string DescBranchNew = (string.IsNullOrEmpty(DataNew.BranchCode)) ? "" : (from m in _db.Branchs
                                                                                          where m.BranchCode == DataNew.BranchCode
                                                                                          select m.Name).First();

                //string DescSalesManagerOld = (DataOld.SmId == 0) ? "" : (from m in _db.SalesManagers
                //                                                         where m.ManagerId == DataOld.SmId
                //                                                         select m.Name).First();

                //string DescSalesManagerNew = (DataNew.SmId == 0) ? "" : (from m in _db.SalesManagers
                //                                                         where m.ManagerId == DataNew.SmId
                //                                                         select m.Name).First();


                BranchManagerLog LogData = new BranchManagerLog();

                LogData.BmId = BmId;

                //-- Data lama
                LogData.Npk = (Action == "Add") ? "" :
                    (string.IsNullOrEmpty(DataOld.Npk)) ? "" : DataOld.Npk;

                LogData.Name = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Name)) ? "" : DataOld.Name);

                LogData.Email = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Email)) ? "" : DataOld.Email);

                LogData.Phone = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Phone)) ? "" : DataOld.Phone);

                LogData.Status = (Action == "Add") ? "" :
                    ((DataRelaOld.IsLockedOut) ? "1" : "0");

                //LogData.SalesRole = (Action == "Add") ? "" :
                //    ((string.IsNullOrEmpty(DescSalesRoleOld)) ? "" : DescSalesRoleOld);

                LogData.Branch = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DescBranchOld)) ? "" : DescBranchOld);

                //LogData.SalesManager = (Action == "Add") ? "" :
                //    ((string.IsNullOrEmpty(DescSalesManagerOld)) ? "" : DescSalesManagerOld);

                LogData.Password = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataRelaOld.Password)) ? "" : DataRelaOld.Password);

                LogData.UserNameBM = (Action == "Add") ? "" :
                   ((string.IsNullOrEmpty(DataRelaOld.UserName)) ? "" : DataRelaOld.UserName);


                //-- Data Baru
                LogData.NpkChange = (Action == "Add" || Action == "Edit") ?
                    ((string.IsNullOrEmpty(DataNew.Npk)) ? "" : DataNew.Npk.ToString()) : "";

                LogData.NameChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Name)) ? "" : DataNew.Name) : "";

                LogData.EmailChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Email)) ? "" : DataNew.Email) : "";

                LogData.PhoneChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Phone)) ? "" : DataNew.Phone) : "";

                LogData.StatusChange = (Action == "Add" || Action == "Edit") ?
                   ((DataRelaNew.IsLockedOut) ? "1" : "0") : "";

                //LogData.SalesRoleChange = (Action == "Add" || Action == "Edit") ?
                //   ((string.IsNullOrEmpty(DescSalesRoleNew)) ? "" : DescSalesRoleNew) : "";

                LogData.BranchChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DescBranchNew)) ? "" : DescBranchNew) : "";

                //LogData.SalesManagerChange = (Action == "Add" || Action == "Edit") ?
                //  ((string.IsNullOrEmpty(DescSalesManagerNew)) ? "" : DescSalesManagerNew) : "";

                LogData.PasswordChange = (Action == "Add" || Action == "Edit") ?
                  ((string.IsNullOrEmpty(DataRelaNew.Password)) ? "" : DataRelaNew.Password) : "";

                LogData.UserNameBMChange = (Action == "Add" || Action == "Edit") ?
                  ((string.IsNullOrEmpty(DataRelaNew.UserName)) ? "" : DataRelaNew.UserName) : "";

                LogData.Action = Action;
                LogData.Date = DateTime.Now;
                LogData.UserName = UserDetail.Name;
                LogData.Npk = UserDetail.NPK;

                _db.AddToBranchManagerLogs(LogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public static string SaveLogABM(ABM DataOld, ABM DataNew, UserLogModel DataRelaOld, UserLogModel DataRelaNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                int AbmId = (DataOld.AbmId == 0) ? DataNew.AbmId : DataOld.AbmId;


                //string DescSalesRoleOld = (DataOld.TeamId == 0) ? "" : (from m in _db.SalesTeams
                //                                                        where m.TeamId == DataOld.TeamId
                //                                                        select m.Name).First();

                //string DescSalesRoleNew = (DataNew.TeamId == 0) ? "" : (from m in _db.SalesTeams
                //                                                        where m.TeamId == DataNew.TeamId
                //                                                        select m.Name).First();

                string DescAreaOld = (string.IsNullOrEmpty(DataOld.AreaCode)) ? "" : (from m in _db.Areas
                                                                                      where m.AreaCode == DataOld.AreaCode
                                                                                      select m.Name).First();

                string DescAreaNew = (string.IsNullOrEmpty(DataNew.AreaCode)) ? "" : (from m in _db.Areas
                                                                                      where m.AreaCode == DataNew.AreaCode
                                                                                      select m.Name).First();

                //string DescSalesManagerOld = (DataOld.SmId == 0) ? "" : (from m in _db.SalesManagers
                //                                                         where m.ManagerId == DataOld.SmId
                //                                                         select m.Name).First();

                //string DescSalesManagerNew = (DataNew.SmId == 0) ? "" : (from m in _db.SalesManagers
                //                                                         where m.ManagerId == DataNew.SmId
                //                                                         select m.Name).First();


                ABMLog LogData = new ABMLog();

                LogData.ABMId = AbmId;

                //-- Data lama
                LogData.Npk = (Action == "Add") ? "" :
                    (string.IsNullOrEmpty(DataOld.Npk)) ? "" : DataOld.Npk;

                LogData.Name = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Name)) ? "" : DataOld.Name);

                LogData.Email = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Email)) ? "" : DataOld.Email);

                LogData.Phone = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Phone)) ? "" : DataOld.Phone);

                LogData.Status = (Action == "Add") ? "" :
                    ((DataRelaOld.IsLockedOut) ? "1" : "0");

                //LogData.SalesRole = (Action == "Add") ? "" :
                //    ((string.IsNullOrEmpty(DescSalesRoleOld)) ? "" : DescSalesRoleOld);

                LogData.Area = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DescAreaOld)) ? "" : DescAreaOld);

                //LogData.SalesManager = (Action == "Add") ? "" :
                //    ((string.IsNullOrEmpty(DescSalesManagerOld)) ? "" : DescSalesManagerOld);

                LogData.Password = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataRelaOld.Password)) ? "" : DataRelaOld.Password);
                LogData.UserNameABM = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataRelaOld.UserName)) ? "" : DataRelaOld.UserName);



                //-- Data Baru
                LogData.NpkChange = (Action == "Add" || Action == "Edit") ?
                    ((string.IsNullOrEmpty(DataNew.Npk)) ? "" : DataNew.Npk.ToString()) : "";

                LogData.NameChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Name)) ? "" : DataNew.Name) : "";

                LogData.EmailChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Email)) ? "" : DataNew.Email) : "";

                LogData.PhoneChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Phone)) ? "" : DataNew.Phone) : "";

                LogData.StatusChange = (Action == "Add" || Action == "Edit") ?
                   ((DataRelaNew.IsLockedOut) ? "1" : "0") : "";

                //LogData.SalesRoleChange = (Action == "Add" || Action == "Edit") ?
                //   ((string.IsNullOrEmpty(DescSalesRoleNew)) ? "" : DescSalesRoleNew) : "";

                LogData.AreaChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DescAreaNew)) ? "" : DescAreaNew) : "";

                //LogData.SalesManagerChange = (Action == "Add" || Action == "Edit") ?
                //  ((string.IsNullOrEmpty(DescSalesManagerNew)) ? "" : DescSalesManagerNew) : "";

                LogData.PasswordChange = (Action == "Add" || Action == "Edit") ?
                  ((string.IsNullOrEmpty(DataRelaNew.Password)) ? "" : DataRelaNew.Password) : "";

                LogData.UserNameABMChange = (Action == "Add" || Action == "Edit") ?
                 ((string.IsNullOrEmpty(DataRelaNew.UserName)) ? "" : DataRelaNew.UserName) : "";

                LogData.Action = Action;
                LogData.Date = DateTime.Now;
                LogData.UserName = UserDetail.Name;
                LogData.Npk = UserDetail.NPK;

                _db.AddToABMLogs(LogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public static string SaveLogRBH(RBH DataOld, RBH DataNew, UserLogModel DataRelaOld, UserLogModel DataRelaNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                int RbhId = (DataOld.RbhId == 0) ? DataNew.RbhId : DataOld.RbhId;


                //string DescSalesRoleOld = (DataOld.TeamId == 0) ? "" : (from m in _db.SalesTeams
                //                                                        where m.TeamId == DataOld.TeamId
                //                                                        select m.Name).First();

                //string DescSalesRoleNew = (DataNew.TeamId == 0) ? "" : (from m in _db.SalesTeams
                //                                                        where m.TeamId == DataNew.TeamId
                //                                                        select m.Name).First();

                string DescRegionOld = (string.IsNullOrEmpty(DataOld.RegionCode)) ? "" : (from m in _db.Regions
                                                                                          where m.RegionCode == DataOld.RegionCode
                                                                                          select m.Name).First();

                string DescRegionNew = (string.IsNullOrEmpty(DataNew.RegionCode)) ? "" : (from m in _db.Regions
                                                                                          where m.RegionCode == DataNew.RegionCode
                                                                                          select m.Name).First();

                //string DescSalesManagerOld = (DataOld.SmId == 0) ? "" : (from m in _db.SalesManagers
                //                                                         where m.ManagerId == DataOld.SmId
                //                                                         select m.Name).First();

                //string DescSalesManagerNew = (DataNew.SmId == 0) ? "" : (from m in _db.SalesManagers
                //                                                         where m.ManagerId == DataNew.SmId
                //                                                         select m.Name).First();


                RBHLog LogData = new RBHLog();

                LogData.RBHId = RbhId;

                //-- Data lama
                LogData.Npk = (Action == "Add") ? "" :
                    (string.IsNullOrEmpty(DataOld.Npk)) ? "" : DataOld.Npk;

                LogData.Name = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Name)) ? "" : DataOld.Name);

                LogData.Email = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Email)) ? "" : DataOld.Email);

                LogData.Phone = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Phone)) ? "" : DataOld.Phone);

                LogData.Status = (Action == "Add") ? "" :
                    ((DataRelaOld.IsLockedOut) ? "1" : "0");

                //LogData.SalesRole = (Action == "Add") ? "" :
                //    ((string.IsNullOrEmpty(DescSalesRoleOld)) ? "" : DescSalesRoleOld);

                LogData.Region = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DescRegionOld)) ? "" : DescRegionOld);

                //LogData.SalesManager = (Action == "Add") ? "" :
                //    ((string.IsNullOrEmpty(DescSalesManagerOld)) ? "" : DescSalesManagerOld);

                LogData.Password = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataRelaOld.Password)) ? "" : DataRelaOld.Password);

                LogData.UserNameRBH = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataRelaOld.UserName)) ? "" : DataRelaOld.UserName);


                //-- Data Baru
                LogData.NpkChange = (Action == "Add" || Action == "Edit") ?
                    ((string.IsNullOrEmpty(DataNew.Npk)) ? "" : DataNew.Npk.ToString()) : "";

                LogData.NameChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Name)) ? "" : DataNew.Name) : "";

                LogData.EmailChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Email)) ? "" : DataNew.Email) : "";

                LogData.PhoneChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Phone)) ? "" : DataNew.Phone) : "";

                LogData.StatusChange = (Action == "Add" || Action == "Edit") ?
                   ((DataRelaNew.IsLockedOut) ? "1" : "0") : "";

                //LogData.SalesRoleChange = (Action == "Add" || Action == "Edit") ?
                //   ((string.IsNullOrEmpty(DescSalesRoleNew)) ? "" : DescSalesRoleNew) : "";

                LogData.RegionChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DescRegionNew)) ? "" : DescRegionNew) : "";

                //LogData.SalesManagerChange = (Action == "Add" || Action == "Edit") ?
                //  ((string.IsNullOrEmpty(DescSalesManagerNew)) ? "" : DescSalesManagerNew) : "";

                LogData.PasswordChange = (Action == "Add" || Action == "Edit") ?
                  ((string.IsNullOrEmpty(DataRelaNew.Password)) ? "" : DataRelaNew.Password) : "";

                LogData.UserNameRBHChange = (Action == "Add" || Action == "Edit") ?
                  ((string.IsNullOrEmpty(DataRelaNew.UserName)) ? "" : DataRelaNew.UserName) : "";

                LogData.Action = Action;
                LogData.Date = DateTime.Now;
                LogData.UserName = UserDetail.Name;
                LogData.Npk = UserDetail.NPK;

                _db.AddToRBHLogs(LogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public static string SaveLogAdmin(Admin DataOld, Admin DataNew, UserLogModel DataRelaOld, UserLogModel DataRelaNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                int Id = (DataOld.Id == 0) ? DataNew.Id : DataOld.Id;


                //string DescSalesRoleOld = (DataOld.TeamId == 0) ? "" : (from m in _db.SalesTeams
                //                                                        where m.TeamId == DataOld.TeamId
                //                                                        select m.Name).First();

                //string DescSalesRoleNew = (DataNew.TeamId == 0) ? "" : (from m in _db.SalesTeams
                //                                                        where m.TeamId == DataNew.TeamId
                //                                                        select m.Name).First();

                //string DescRegionOld = (string.IsNullOrEmpty(DataOld.RegionCode)) ? "" : (from m in _db.Regions
                //                                                                          where m.RegionCode == DataOld.RegionCode
                //                                                                          select m.Name).First();

                //string DescRegionNew = (string.IsNullOrEmpty(DataNew.RegionCode)) ? "" : (from m in _db.Regions
                //                                                                          where m.RegionCode == DataNew.RegionCode
                //                                                                          select m.Name).First();

                //string DescSalesManagerOld = (DataOld.SmId == 0) ? "" : (from m in _db.SalesManagers
                //                                                         where m.ManagerId == DataOld.SmId
                //                                                         select m.Name).First();

                //string DescSalesManagerNew = (DataNew.SmId == 0) ? "" : (from m in _db.SalesManagers
                //                                                         where m.ManagerId == DataNew.SmId
                //                                                         select m.Name).First();


                AdminLog LogData = new AdminLog();

                LogData.Id = Id;

                //-- Data lama
                LogData.Npk = (Action == "Add") ? "" :
                    (string.IsNullOrEmpty(DataOld.NIK)) ? "" : DataOld.NIK;

                LogData.Name = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataOld.Name)) ? "" : DataOld.Name);

                //LogData.Email = (Action == "Add") ? "" :
                //    ((string.IsNullOrEmpty(DataOld.Email)) ? "" : DataOld.Email);

                //LogData.Phone = (Action == "Add") ? "" :
                //    ((string.IsNullOrEmpty(DataOld.Phone)) ? "" : DataOld.Phone);

                LogData.Status = (Action == "Add") ? "" :
                    ((DataRelaOld.IsLockedOut) ? "1" : "0");

                LogData.Role = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataRelaOld.Role)) ? "" : DataRelaOld.Role);

                //LogData.Region = (Action == "Add") ? "" :
                //    ((string.IsNullOrEmpty(DescRegionOld)) ? "" : DescRegionOld);

                //LogData.SalesManager = (Action == "Add") ? "" :
                //    ((string.IsNullOrEmpty(DescSalesManagerOld)) ? "" : DescSalesManagerOld);

                LogData.Password = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataRelaOld.Password)) ? "" : DataRelaOld.Password);

                LogData.UserNameAdmin = (Action == "Add") ? "" :
                    ((string.IsNullOrEmpty(DataRelaOld.UserName)) ? "" : DataRelaOld.UserName);

                //-- Data Baru
                LogData.NpkChange = (Action == "Add" || Action == "Edit") ?
                    ((string.IsNullOrEmpty(DataNew.NIK)) ? "" : DataNew.NIK.ToString()) : "";

                LogData.NameChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataNew.Name)) ? "" : DataNew.Name) : "";

                //LogData.EmailChange = (Action == "Add" || Action == "Edit") ?
                //   ((string.IsNullOrEmpty(DataNew.Email)) ? "" : DataNew.Email) : "";

                //LogData.PhoneChange = (Action == "Add" || Action == "Edit") ?
                //   ((string.IsNullOrEmpty(DataNew.Phone)) ? "" : DataNew.Phone) : "";

                LogData.StatusChange = (Action == "Add" || Action == "Edit") ?
                   ((DataRelaNew.IsLockedOut) ? "1" : "0") : "";

                LogData.RoleChange = (Action == "Add" || Action == "Edit") ?
                   ((string.IsNullOrEmpty(DataRelaNew.Role)) ? "" : DataRelaNew.Role) : "";

                //LogData.RegionChange = (Action == "Add" || Action == "Edit") ?
                //   ((string.IsNullOrEmpty(DescRegionNew)) ? "" : DescRegionNew) : "";

                //LogData.SalesManagerChange = (Action == "Add" || Action == "Edit") ?
                //  ((string.IsNullOrEmpty(DescSalesManagerNew)) ? "" : DescSalesManagerNew) : "";

                LogData.PasswordChange = (Action == "Add" || Action == "Edit") ?
                  ((string.IsNullOrEmpty(DataRelaNew.Password)) ? "" : DataRelaNew.Password) : "";

                LogData.UserNameAdminChange = (Action == "Add" || Action == "Edit") ?
                  ((string.IsNullOrEmpty(DataRelaNew.UserName)) ? "" : DataRelaNew.UserName) : "";

                LogData.Action = Action;
                LogData.Date = DateTime.Now;
                LogData.UserName = UserDetail.Name;
                LogData.Npk = UserDetail.NPK;

                _db.AddToAdminLogs(LogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public static string SaveUserLoginLogs(string UserName, bool status, string Note, bool StatusLogin)
        {

            string message = string.Empty;

            dsarEntities _db = new dsarEntities();

            try
            {
                User user = CommonModel.GetCurrentUser();
                string Mac = CommonModel.GetMACAddress();

                UserLoginLog UserLoginLogs = new UserLoginLog();
                UserLoginLogs.LoginDate = DateTime.Now;
                UserLoginLogs.UserName = UserName;
                UserLoginLogs.Status = status;
                UserLoginLogs.Address = Mac;
                UserLoginLogs.Note = Note;
                UserLoginLogs.StatusLogin = StatusLogin;

                _db.AddToUserLoginLogs(UserLoginLogs);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

                message = ex.Message;
            }
            return message;
        }
        public static string SaveChangePasswordLog(string UserName, string Password)
        {

            string message = string.Empty;

            dsarEntities _db = new dsarEntities();

            try
            {
                User user = CommonModel.GetCurrentUser();

                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                ChangePasswordLog ChangePasswordLogData = new ChangePasswordLog();
                ChangePasswordLogData.Password = Password;
                ChangePasswordLogData.UserName = user.UserName;

                ChangePasswordLogData.Date = DateTime.Now;
                ChangePasswordLogData.UsernameAction = UserDetail.Name;
                ChangePasswordLogData.NPK = UserDetail.NPK;

                _db.AddToChangePasswordLogs(ChangePasswordLogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

                message = ex.Message;
            }
            return message;
        }

        public static string SaveLogCustomer(CustomerLogModel DataOld, CustomerLogModel DataNew, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                UserDetailModel UserDetail = CommonModel.GetUserDetail(user.UserName);

                int Id = (DataOld.ID == 0) ? DataNew.ID : DataOld.ID;


                NasabahLog LogData = new NasabahLog();

                LogData.IdNasabah = Id;

                //-- Data lama
                LogData.GCIF = (Action == "Add") ? "" :
                    (string.IsNullOrEmpty(DataOld.GCIF)) ? "" : DataOld.GCIF;
                LogData.SourceOfCustomer = (Action == "Add") ? "" :
                    (string.IsNullOrEmpty(DataOld.SourceOfCustomer)) ? "" : DataOld.SourceOfCustomer;
                LogData.CustomerName = (Action == "Add") ? "" :
                                    (string.IsNullOrEmpty(DataOld.CustomerName)) ? "" : DataOld.CustomerName;
                LogData.IDNumber = (Action == "Add") ? "" :
                                    (string.IsNullOrEmpty(DataOld.IDNumber)) ? "" : DataOld.IDNumber;
                LogData.Gender = (Action == "Add") ? "" :
                                    (string.IsNullOrEmpty(DataOld.Gender)) ? "" : DataOld.Gender;
                LogData.BirthDate = (Action == "Add") ? (DateTime?)null :
                                    ((DataOld.BirthDate == null) ? (DateTime?)null : DataOld.BirthDate);
                LogData.Address = (Action == "Add") ? "" :
                                    (string.IsNullOrEmpty(DataOld.Address)) ? "" : DataOld.Address;
                LogData.HomePhone = (Action == "Add") ? "" :
                                    (string.IsNullOrEmpty(DataOld.GCIF)) ? "" : DataOld.HomePhone;
                LogData.MobilePhone = (Action == "Add") ? "" :
                                    (string.IsNullOrEmpty(DataOld.MobilePhone)) ? "" : DataOld.MobilePhone;
                LogData.Sales = (Action == "Add") ? "" :
                                    (string.IsNullOrEmpty(DataOld.Sales)) ? "" : DataOld.Sales;

                //-- Data Baru
                LogData.GCIFChange = (Action == "Add" || Action == "Edit") ?
                    ((string.IsNullOrEmpty(DataNew.GCIF)) ? "" : DataNew.GCIF) : "";
                LogData.SourceOfCustomerChange = (Action == "Add" || Action == "Edit") ?
                    ((string.IsNullOrEmpty(DataNew.SourceOfCustomer)) ? "" : DataNew.SourceOfCustomer) : "";
                LogData.CustomerNameChange = (Action == "Add" || Action == "Edit") ?
                                    ((string.IsNullOrEmpty(DataNew.CustomerName)) ? "" : DataNew.CustomerName) : "";
                LogData.IDNumberChange = (Action == "Add" || Action == "Edit") ?
                                    ((string.IsNullOrEmpty(DataNew.IDNumber)) ? "" : DataNew.IDNumber) : "";
                LogData.GenderChange = (Action == "Add" || Action == "Edit") ?
                                    ((string.IsNullOrEmpty(DataNew.Gender)) ? "" : DataNew.Gender) : "";
                LogData.BirthDateChange = (Action == "Add" || Action == "Edit") ?
                                    ((DataNew.BirthDate == null) ? (DateTime?)null : DataNew.BirthDate) : (DateTime?)null;
                LogData.AddressChange = (Action == "Add" || Action == "Edit") ?
                                    ((string.IsNullOrEmpty(DataNew.Address)) ? "" : DataNew.Address) : "";
                LogData.HomePhoneChange = (Action == "Add" || Action == "Edit") ?
                                    ((string.IsNullOrEmpty(DataNew.HomePhone)) ? "" : DataNew.HomePhone) : "";
                LogData.MobilePhoneChange = (Action == "Add" || Action == "Edit") ?
                                    ((string.IsNullOrEmpty(DataNew.MobilePhone)) ? "" : DataNew.MobilePhone) : "";
                LogData.SalesChange = (Action == "Add" || Action == "Edit") ?
                                    ((string.IsNullOrEmpty(DataNew.Sales)) ? "" : DataNew.Sales) : "";

                LogData.Action = Action;
                LogData.ActionDate = DateTime.Now;
                LogData.Name = UserDetail.Name;
                LogData.Npk = UserDetail.NPK;

                _db.AddToNasabahLogs(LogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public static UserLogModel MappingUserFromEFtoModel(User DataUserEf)
        {

            UserLogModel Model = new UserLogModel();
            Model.UserId = DataUserEf.UserId;
            Model.UserName = DataUserEf.UserName;
            Model.Password = DataUserEf.Password;
            Model.PasswordSalt = DataUserEf.PasswordSalt;
            Model.PasswordText = DataUserEf.PasswordText;
            Model.Role = DataUserEf.Role;
            Model.RelatedId = DataUserEf.RelatedId.Value;
            Model.CreatedDate = DataUserEf.CreatedDate;
            Model.LastLoginDate = DataUserEf.LastLoginDate;
            Model.IsLockedOut = DataUserEf.IsLockedOut;
            Model.LastActiveSession = DataUserEf.LastActiveSession ?? DateTime.Now;

            return Model;
        }

        public static CustomerLogModel MappingCustomerFromEFtoModel(Nasabah DataNasabah)
        {
            string salesNasabah = string.Empty;
            dsarEntities _db = new dsarEntities();

            foreach (var item in DataNasabah.SalesNasabahs)
            {
                salesNasabah = (from m in _db.Sales where m.SalesId == item.SalesId select m.Name).First() + ",";
            }

            CustomerLogModel model = new CustomerLogModel();

            model.GCIF = DataNasabah.GCIF;
            model.SourceOfCustomer = DataNasabah.Status;
            model.CustomerName = DataNasabah.Name;
            model.IDNumber = DataNasabah.KtpId;
            model.Gender = DataNasabah.Gender;
            model.BirthDate = DataNasabah.BirthDate;
            model.Address = DataNasabah.Address;
            model.HomePhone = DataNasabah.HomePhone;
            model.MobilePhone = DataNasabah.MobilePhone;
            model.Sales = salesNasabah.TrimEnd(',');

            return model;


        }

        public static List<UserAllLogModel> MappingAdminsFromEFtoModel(IQueryable<AdminLog> AdminData)
        {
            List<UserAllLogModel> models = new List<UserAllLogModel>();

            foreach (var item in AdminData)
            {
                UserAllLogModel model = new UserAllLogModel();
                //do mapping
                model.NPKUser = item.Npk;
                model.UserName = item.Name;
                model.Role = "Admin";
                model.Email = "";
                model.Phone = "";
                model.SalesRole = "";
                model.AdminRole = item.Role;
                model.Branch = "";
                model.Area = "";
                model.Region = "";
                model.SalesLeader = "";
                model.UserName = item.UserNameAdmin;
                model.Password = item.Password;
                model.Status = item.Status;

                model.NPKUserChange = item.NpkChange;
                model.UserNameChange = item.NameChange;
                model.RoleChange = "Admin";
                model.EmailChange = "";
                model.PhoneChange = "";
                model.SalesRoleChange = "";
                model.AdminRoleChange = item.RoleChange;
                model.BranchChange = "";
                model.AreaChange = "";
                model.RegionChange = "";
                model.SalesLeaderChange = "";
                model.UserNameChange = item.UserNameAdminChange;
                model.PasswordChange = item.PasswordChange;
                model.StatusChange = item.StatusChange;

                model.NPK = item.NPKUser;
                model.Name = item.UserName;
                model.Action = item.Action;
                model.ActionDate = item.Date;

                //--

                models.Add(model);
            }

            return models;
        }

        public static List<UserAllLogModel> MappingRBHFromEFtoModel(IQueryable<RBHLog> RBHData)
        {
            List<UserAllLogModel> models = new List<UserAllLogModel>();

            foreach (var item in RBHData)
            {
                UserAllLogModel model = new UserAllLogModel();
                //do mapping
                model.NPKUser = item.Npk;
                model.UserName = item.Name;
                model.Role = "RBH";
                model.Email = item.Email;
                model.Phone = "";
                model.SalesRole = "";
                model.AdminRole = "";
                model.Branch = "";
                model.Area = "";
                model.Region = item.Region;
                model.SalesLeader = "";
                model.UserName = item.UserNameRBH;
                model.Password = item.Password;
                model.Status = item.Status;

                model.NPKUserChange = item.NpkChange;
                model.UserNameChange = item.NameChange;
                model.RoleChange = "RBH";
                model.EmailChange = item.EmailChange;
                model.PhoneChange = item.PhoneChange;
                model.SalesRoleChange = "";
                model.AdminRoleChange = "";
                model.BranchChange = "";
                model.AreaChange = "";
                model.RegionChange = item.RegionChange;
                model.SalesLeaderChange = "";
                model.UserNameChange = item.UserNameRBHChange;
                model.PasswordChange = item.PasswordChange;
                model.StatusChange = item.StatusChange;

                model.NPK = item.NPKUser;
                model.Name = item.UserName;
                model.Action = item.Action;
                model.ActionDate = item.Date;
            }

            return models;
        }

        public static List<UserAllLogModel> MappingSalesFromEFtoModel(IQueryable<SalesLog> AdminData)
        {
            List<UserAllLogModel> models = new List<UserAllLogModel>();

            return models;
        }

        public static List<UserAllLogModel> MappingSMFromEFtoModel(IQueryable<SalesManagerLog> AdminData)
        {
            List<UserAllLogModel> models = new List<UserAllLogModel>();

            return models;
        }

        public static List<UserAllLogModel> MappingBMFromEFtoModel(IQueryable<BranchManagerLog> AdminData)
        {
            List<UserAllLogModel> models = new List<UserAllLogModel>();

            return models;
        }

        public static List<UserAllLogModel> MappingABMFromEFtoModel(IQueryable<ABMLog> AdminData)
        {
            List<UserAllLogModel> models = new List<UserAllLogModel>();

            return models;
        }

        //public static CustomerLogModel MappingCustomerDataExeltoModel(ImportModel DataNasabah)
        //{
        //    string salesNasabah = string.Empty;
        //    dsarEntities _db = new dsarEntities();



        //    CustomerLogModel model = new CustomerLogModel();

        //    model.GCIF = DataNasabah.GCIF;
        //    model.SourceOfCustomer = DataNasabah.;
        //    model.CustomerName = DataNasabah.Name;
        //    model.IDNumber = DataNasabah.KtpId;
        //    model.Gender = DataNasabah.Gender;
        //    model.BirthDate = DataNasabah.BirthDate;
        //    model.Address = DataNasabah.Address;
        //    model.HomePhone = DataNasabah.HomePhone;
        //    model.MobilePhone = DataNasabah.MobilePhone;
        //    model.Sales = salesNasabah.TrimEnd(',');

        //    return model;


        //}


    }

    public class UserLogModel
    {

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordText { get; set; }
        public string Role { get; set; }
        public int RelatedId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime LastActiveSession { get; set; }


    }

    public class CustomerLogModel
    {
        public int ID { get; set; }
        public string GCIF { get; set; }
        public string SourceOfCustomer { get; set; }
        public string CustomerName { get; set; }
        public string IDNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string Sales { get; set; }

        public string GCIFChange { get; set; }
        public string SourceOfCustomerChange { get; set; }
        public string CustomerNameChange { get; set; }
        public string IDNumberChange { get; set; }
        public string GenderChange { get; set; }
        public DateTime BirthDateChange { get; set; }
        public string AddressChange { get; set; }
        public string HomePhoneChange { get; set; }
        public string MobilePhoneChange { get; set; }
        public string SalesChange { get; set; }

        public string Npk { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public DateTime ActionDate { get; set; }


    }

    public class UserAllLogModel
    {

        public string NPKUser { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string SalesRole { get; set; }
        public string AdminRole { get; set; }
        public string Branch { get; set; }
        public string Area { get; set; }
        public string Region { get; set; }
        public string SalesLeader { get; set; }
        public string UserNameLogin { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }

        public string NPKUserChange { get; set; }
        public string UserNameChange { get; set; }
        public string RoleChange { get; set; }
        public string EmailChange { get; set; }
        public string PhoneChange { get; set; }
        public string SalesRoleChange { get; set; }
        public string AdminRoleChange { get; set; }
        public string BranchChange { get; set; }
        public string AreaChange { get; set; }
        public string RegionChange { get; set; }
        public string SalesLeaderChange { get; set; }
        public string UserNameLoginChange { get; set; }
        public string PasswordChange { get; set; }
        public string StatusChange { get; set; }

        public string NPK { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public DateTime? ActionDate { get; set; }

    }
}
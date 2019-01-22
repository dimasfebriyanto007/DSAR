using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cms.Models
{
    public class ProductLogModel
    {
        protected dsarEntities _db = new dsarEntities();
        public static string SaveLogProduct(Product ProductData, string Action)
        {
            dsarEntities _db = new dsarEntities();

            string message = string.Empty;
            try
            {
                User user = CommonModel.GetCurrentUser();
                var DescCategory  = (from m in _db.ProductCategories
                                        where m.CategoryId == ProductData.CategoryId
                              select m).First();

                ProductsLog ProducLogData = new ProductsLog();
                ProducLogData.ProductId = ProductData.ProductId;
                ProducLogData.CategoryDesc = DescCategory.Name;
                ProducLogData.Code = ProductData.Code;
                ProducLogData.Name = ProductData.Name;
                ProducLogData.Description = ProductData.Description;
                ProducLogData.ImageName = ProductData.ImageName;
                ProducLogData.Action = Action;
                ProducLogData.Date = DateTime.Now;
                ProducLogData.UserName = user.UserName;
                
               
                

                _db.AddToProductsLogs(ProducLogData);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

                message = ex.Message;
            }
            return message;
        } 

    }
}
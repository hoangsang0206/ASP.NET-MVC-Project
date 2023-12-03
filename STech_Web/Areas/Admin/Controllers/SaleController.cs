using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Web.Mvc;
using STech_Web.Filters;
using STech_Web.Models;

namespace STech_Web.Areas.Admin.Controllers
{
    [ManagerAuthorization]
    public class SaleController : Controller
    {
        // GET: Admin/Sale
        public ActionResult Index()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Sale> sales = db.Sales.ToList();

            ViewBag.ActiveNav = "sale";
            return View(sales);
        }

        //--Bắt đầu khuyến mãi
        public ActionResult BeginSale(int saleId)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Sale sale = db.Sales.FirstOrDefault(s => s.SaleID == saleId);
            if(sale != null && sale.Status == "Chờ")
            {
                List<SaleDetail> saleDetails = sale.SaleDetails.ToList();
                foreach(SaleDetail detail in saleDetails)
                {
                    Product product = detail.Product;
                    product.Price = detail.SalePrice;
                }
                sale.Status = "Bắt đầu";
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //--Thay đổi giá của sản phẩm sale nếu kết thúc
        public ActionResult EndSale(int saleId)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Sale sale = db.Sales.FirstOrDefault(s => s.SaleID == saleId);
            if (sale != null && sale.Status == "Bắt đầu")
            {
                List<SaleDetail> saleDetails = sale.SaleDetails.ToList();
                foreach (SaleDetail detail in saleDetails)
                {
                    Product product = detail.Product;
                    product.Price = detail.OriginalPrice;
                }
                sale.Status = "Kết thúc";
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //--Thay đổi giá của tất cả sản phẩm sale
        public ActionResult ChangeSalePrice()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Sale> sales = db.Sales.ToList();
            if (sales.Count > 0)
            {
                foreach (Sale sale in sales)
                {
                    List<SaleDetail> saleDetail = sale.SaleDetails.ToList();
                    if (saleDetail.Count > 0)
                    {
                        foreach (SaleDetail detail in saleDetail)
                        {
                            Product product = detail.Product;
                            if (sale.StartTime <= DateTime.Now && sale.EndTime >= DateTime.Now && sale.Status == "Bắt đầu")
                            {
                                product.Price = detail.SalePrice;
                            }
                            else
                            {
                                product.Price = detail.OriginalPrice;
                            }
                        }

                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}
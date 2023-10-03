using STech_Web.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace STech_Web.Controllers
{
    public class CollectionsController : Controller
    {
        // GET: Categories
        public ActionResult Index()
        {
            return View();
        }

        //Sắp xếp sản phẩm
        public List<Product> Sort(string value, List<Product> products)
        {
            List<Product> productsSort = new List<Product>();
            if (value == "price-ascending")
            {
                ViewBag.SortName = "Giá tăng dần";
                productsSort = products.OrderBy(t => t.Price).ToList();
            }
            else if (value == "price-descending")
            {
                ViewBag.SortName = "Giá giảm dần";
                productsSort = products.OrderByDescending(t => t.Price).ToList();
            }
            else if (value == "name-az")
            {
                ViewBag.SortName = "Tên A - Z";
                productsSort = products.OrderBy(t => t.ProductName).ToList();
            }
            else if (value == "name-za")
            {
                ViewBag.SortName = "Tên Z - A";
                productsSort = products.OrderByDescending(t => t.ProductName).ToList();
            }
            else
            {
                ViewBag.SortName = "Ngẫu nhiên";
                productsSort =  products;
            }    

            return productsSort;

        }

        //Tìm kiếm sản phẩm
        public ActionResult Search(string search = "", string sort = "")
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();

            List<Product> products = new List<Product>();
            if (string.IsNullOrWhiteSpace(search) == false)
            {
               //products = db.Products.Where(t => t.ProductName.Contains(search)).ToList();

                products = db.Products.SearchName(search).ToList();
            }

            if(sort.Length > 0)
            {
                products = Sort(sort, products);
            }    

            ViewBag.searchValue = search;
            ViewBag.products = products;
            return View(products);
        }

        //Lọc sản phẩm
        public List<Product> Filter(List<Product> products, string filterType = "", string value = "")
        {
            List<Product> productsFilter = new List<Product>();

            //Lọc sản phẩm theo thương hiệu
            if (filterType == "brand")
            {

                productsFilter = products.Where(t => t.Brand.BrandID == value).ToList();
            }
            //Lọc sản phẩm theo thương hiệu con (dựa vào tên sản phẩm)
            else if (filterType == "sbrand")
            {
                productsFilter = products.SearchName(value).ToList();
            }    
            //Lọc sản phẩm theo giá cho trước
            else if (filterType == "price")
            {
                if (value == "tren-25-trieu")
                {
                    productsFilter = products.Where(t => t.Price > 25000000).ToList();
                }
                else if (value == "tu-20-den-25-trieu")
                {
                    productsFilter = products.Where(t => t.Price > 20000000 && t.Price < 25000000).ToList();
                }
                else if (value == "tu-15-den-20-trieu")
                {
                    productsFilter = products.Where(t => t.Price > 15000000 && t.Price < 20000000).ToList();
                }
                else if (value == "duoi-15-trieu")
                {
                    productsFilter = products.Where(t => t.Price < 15000000).ToList();
                }
            }
            

            return productsFilter;
        }

        //Lọc sản phẩm theo id danh mục
        public ActionResult GetProductByID(string id = "", string sort = "", string filtertype = "", string filter = "")
        {

            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Product> products = db.Products.Where(t => t.Category.CateID == id).ToList();

            if(sort.Length > 0)
            {
                products = Sort(sort, products);
            }    

            if(filter.Length > 0 && filtertype.Length > 0)
            {
                products = Filter(products, filtertype, filter);
                ViewBag.FilterType = filtertype;
                ViewBag.Filter = filter;
            }    

            Category cate = db.Categories.Where(t => t.CateID == id).FirstOrDefault();

            List<Breadcrumb> breadcrumb = new List<Breadcrumb>();
            breadcrumb.Add( new Breadcrumb("Trang chủ", "/"));
            breadcrumb.Add(new Breadcrumb(cate.CateName, ""));

            ViewBag.cateID = id;
            ViewBag.CateName = cate.CateName;
            ViewBag.Breadcrumb = breadcrumb;

            return View("Index", products);
        }

    }
}
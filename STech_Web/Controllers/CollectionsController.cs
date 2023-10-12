using STech_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

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
                productsSort = products;
                var r = new Random();

                for (int i = 0; i < productsSort.Count; i++)
                {
                    int j = r.Next(i + 1);
                    Product p = productsSort[j];
                    productsSort[j] = productsSort[i];
                    productsSort[i] = p;
                }

                ViewBag.SortName = "Ngẫu nhiên";
            }

            return productsSort;

        }

        public List<Product> Pagination(List<Product> products, int page)
        {
            //Paging ------
            int NoOfProductPerPage = 20;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(
                Convert.ToDouble(products.Count) / Convert.ToDouble(NoOfProductPerPage)));
            int NoOfProductToSkip = (page - 1) * NoOfProductPerPage;
            ViewBag.Page = page;
            ViewBag.NoOfPages = NoOfPages;
            products = products.Skip(NoOfProductToSkip).Take(NoOfProductPerPage).ToList();

            return products;
        }

        //Tìm kiếm sản phẩm
        public ActionResult Search(string search = "", string sort = "", int page = 1)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();

            List<Product> products = new List<Product>();

            if (string.IsNullOrWhiteSpace(search) == false)
            {
                //products = db.Products.Where(t => t.ProductName.Contains(search)).ToList();
                products = db.Products.SearchName(search).ToList();
            }

            if (sort.Length > 0)
            {
                products = Sort(sort, products);
            }

            //Paging -----
            products = Pagination(products, page);
           

            //----------
            ViewBag.searchValue = search;
            ViewBag.sortValue = sort;

            //return
            return View(products);
        }

        //Lọc sản phẩm
        public List<Product> Filter(List<Product> products, string filterType, string value, string sbrand)
        {
            List<Product> productsFilter = new List<Product>();
            string filterName = "";
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            //Lọc sản phẩm theo thương hiệu
            if (filterType == "brand")
            {
                productsFilter = products.Where(t => t.Brand.BrandID == value).ToList();


                if (productsFilter.Count > 0)
                {
                    Product product = productsFilter[0];
                    filterName = product.Brand.BrandName;
                }
                else
                {
                    filterName = textInfo.ToTitleCase(value);
                }
            }
            //Lọc sản phẩm theo thương hiệu con (dựa vào tên sản phẩm)
            else if (filterType == "sbrand")
            {
                //productsFilter = products.SearchName(sbrand).ToList();
                productsFilter = products.Where(t => t.Brand.BrandID == value && t.ProductName.ToLower().Contains(sbrand)).ToList();

                if (productsFilter.Count > 0)
                {
                    Product product = productsFilter[0];
                    Regex regex = new Regex(sbrand, RegexOptions.IgnoreCase);
                    Match match = regex.Match(product.ProductName);
                    filterName = product.Brand.BrandName + " " + match.Value;
                }
                else
                {
                    filterName = textInfo.ToTitleCase(value) + " " + textInfo.ToTitleCase(sbrand);
                }
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

            ViewBag.filterName = filterName;

            return productsFilter;
        }

        //Lọc sản phẩm theo id danh mục
        public ActionResult GetProduct(string id = "", string sort = "", string filtertype = "", string filter = "", string sbrand = "", int page = 1)
        {
            //Lấy danh sách sản phẩm theo danh mục
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Product> products = new List<Product>();

            string breadcrumbItem = "";

            if (id == "all")
            {
                products = db.Products.ToList();
                breadcrumbItem = "Tất cả sản phẩm";
            }
            else
            {
                products = db.Products.Where(t => t.Category.CateID == id).ToList();

                //Lấy danh mục của danh sách sản phẩm
                Category cate = db.Categories.Where(t => t.CateID == id).FirstOrDefault();
                //
                breadcrumbItem = cate.CateName;
            }
            //--------
            //Sắp xếp danh sách sản phẩm
            if (sort.Length > 0)
            {
                products = Sort(sort, products);
            }

            //Lọc danh sách sản phẩm
            if ((products != null) && (filter.Length > 0 && filtertype.Length > 0))
            {
                products = Filter(products, filtertype, filter, sbrand);
                ViewBag.FilterType = filtertype;
                ViewBag.Filter = filter;
                ViewBag.Sbrand = sbrand;
                if (ViewBag.filterName != null || ViewBag.filterName.Length > 0)
                {
                    breadcrumbItem += " " + ViewBag.filterName;
                }
            }

            //Tạo danh sách Breadcrumb
            List<Breadcrumb> breadcrumb = new List<Breadcrumb>();
            breadcrumb.Add(new Breadcrumb("Trang chủ", "/"));
            breadcrumb.Add(new Breadcrumb(breadcrumbItem, ""));

            //Paging ------
            products = Pagination(products, page);

            //-----------
            ViewBag.cateID = id;
            ViewBag.title = breadcrumbItem;
            ViewBag.Breadcrumb = breadcrumb;
            ViewBag.sortValue = sort;

            return View("Index", products);
        }

    }
}
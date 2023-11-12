using STech_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
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
            int NoOfProductPerPage = 40;
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
        public List<Product> Filter(List<Product> products, string filterType, string filter, string sbrand, decimal? minprice, decimal? maxprice)
        {
            List<Product> productsFilter = new List<Product>();
            string filterName = "";
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            
            switch (filterType)
            {
                //Lọc sản phẩm theo thương hiệu
                case "brand":
                    productsFilter = products.Where(t => t.Brand.BrandID == filter).ToList();
                    if (productsFilter.Count > 0)
                    {
                        Product product = productsFilter[0];
                        filterName = product.Brand.BrandName.ToUpper();
                    }
                    else
                    {
                        filterName = textInfo.ToTitleCase(filter);
                    }
                    break;
                //Lọc sản phẩm theo thương hiệu con (dựa vào tên sản phẩm)
                case "sbrand":
                    productsFilter = products.Where(t => t.Brand.BrandID == filter && t.ProductName.ToLower().Contains(sbrand)).ToList();
                    if (productsFilter.Count > 0)
                    {
                        Product product = productsFilter[0];
                        Regex regex = new Regex(sbrand, RegexOptions.IgnoreCase);
                        Match match = regex.Match(product.ProductName);
                        filterName = product.Brand.BrandName.ToUpper() + " " + match.Value;
                    }
                    else
                    {
                        filterName = textInfo.ToTitleCase(filter) + " " + textInfo.ToTitleCase(sbrand);
                    }
                    break;
                //Lọc sản phẩm theo giá cho trước
                case "price":
                    if (minprice == null)
                    {
                        productsFilter = products.Where(t => t.Price <= maxprice).ToList();
                        filterName = "giá dưới " + (int)((decimal)maxprice / 1000000) + " triệu";
                    }
                    else if (maxprice == null)
                    {
                        productsFilter = products.Where(t => t.Price >= minprice).ToList();
                        filterName = "giá trên " + (int)((decimal)minprice / 1000000) + " triệu";
                    }
                    else if(minprice != null && maxprice != null)
                    {
                        productsFilter = products.Where(t => t.Price >= minprice && t.Price <= maxprice).ToList();
                        filterName = "giá từ " + (int)((decimal)minprice / 1000000) + " đến " + (int)((decimal)maxprice / 1000000) + " triệu";
                    }
                    else
                    {
                        filterName = "giá từ ... đến ...";
                    }

                    break;

            }

            ViewBag.filterName = filterName;
            return productsFilter;
        }

        //Kiểm tra danh mục có tồn tại không
        private bool checkCateExist(DatabaseSTechEntities db, string cateID)
        {
            Category category = db.Categories.FirstOrDefault(t => t.CateID == cateID);
            if (category != null || cateID == "all")
            {
                return true;
            }
            return false;
        }

        //Lọc sản phẩm theo id danh mục
        public ActionResult GetProduct(string id = "", string sort = "", string filtertype = "", string filter = "", string sbrand = "", decimal? minprice = null, decimal? maxprice = null, int page = 1)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            if (id.Length > 0 && checkCateExist(db, id))
            {
                //Lấy danh sách sản phẩm theo danh mục
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
                if ((products != null) && filtertype.Length > 0)
                {
                    products = Filter(products, filtertype, filter, sbrand, minprice, maxprice);
                    ViewBag.FilterType = filtertype;
                    ViewBag.Filter = filter;
                    ViewBag.Sbrand = sbrand;
                    ViewBag.MinPrice = minprice;
                    ViewBag.MaxPrice = maxprice;
                    if (ViewBag.filterName != null || ViewBag.filterName.Length > 0)
                    {
                        breadcrumbItem += " " + ViewBag.filterName;
                    }
                }

                //Tạo danh sách Breadcrumb
                List<Breadcrumb> breadcrumb = new List<Breadcrumb>();
                breadcrumb.Add(new Breadcrumb("Trang chủ", "/"));
                breadcrumb.Add(new Breadcrumb(breadcrumbItem, ""));

                //Sắp xếp danh sách sản phẩm
                if (sort.Length > 0)
                {
                    products = Sort(sort, products);
                }

                //Paging ------
                products = Pagination(products, page);

                //-----------
                ViewBag.cateID = id;
                ViewBag.title = breadcrumbItem;
                ViewBag.Breadcrumb = breadcrumb;
                ViewBag.sortValue = sort;

                return View("Index", products);
            }

            return Redirect("/error/notfound");
        }

        //Lọc sản phẩm theo khoảng giá, danh sách hãng,...
        [HttpPost]
        public ActionResult FilterCollections(List<string> brandList = null, string currentCate = "", string minPrice = "", string maxPrice = "", string sort = "", int page = 1)
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                List<Category> categories = db.Categories.ToList();
                List<Product> products = db.Products.Where(t => t.CateID == currentCate).ToList();

                decimal minprice = Convert.ToDecimal(minPrice.Replace("đ", "").Replace(".", ""));
                decimal maxprice = Convert.ToDecimal(maxPrice.Replace("đ", "").Replace(".", ""));

                string breadcrumbItem = "" + db.Categories.FirstOrDefault(t => t.CateID == currentCate);

                //Lọc danh sách sản phẩm
                if (products != null)
                {
                    products = Filter(products, "price", null, null, minprice, maxprice);
                    ViewBag.FilterType = "price";
                    ViewBag.Filter = "";
                    ViewBag.Sbrand = "";
                    ViewBag.MinPrice = minprice;
                    ViewBag.MaxPrice = maxprice;
                    if (ViewBag.filterName != null || ViewBag.filterName.Length > 0)
                    {
                        breadcrumbItem += " " + ViewBag.filterName;
                    }
                }

                //Tạo danh sách Breadcrumb
                List<Breadcrumb> breadcrumb = new List<Breadcrumb>();
                breadcrumb.Add(new Breadcrumb("Trang chủ", "/"));
                breadcrumb.Add(new Breadcrumb(breadcrumbItem, ""));

                //Sắp xếp danh sách sản phẩm
                if (sort.Length > 0)
                {
                    products = Sort(sort, products);
                }

                //Paging ------
                products = Pagination(products, page);

                //-----------
                ViewBag.cateID = currentCate;
                ViewBag.title = breadcrumbItem;
                ViewBag.Breadcrumb = breadcrumb;
                ViewBag.sortValue = sort;

                return View("Index", products);
            }
            catch(Exception ex) { }
            {
                return Redirect("/error/notfound");
            }   
        }

    }
}
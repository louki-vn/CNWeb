using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using WebShop.Models;

namespace WebShop.Areas.Sales.Controllers
{
    public class LoginController : Controller
    {
        Shop db = new Shop();
       
        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            Session["is_logined"] = 0;
            
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            string username = fc.Get("customer[email]").ToString();
            string pass = fc.Get("customer[password]").ToString();
            var u = new SqlParameter("@username", username);
            var p = new SqlParameter("@password", Data.MD5Hash(pass));
            var result = db.Database.SqlQuery<MEMBER>("exec getMEMBERfromusernameandpass @username, @password", u, p).ToList();
            int check = result.Count();
            if (check != 0)
            {
                Session["user_logined"] = username;
                Session["is_logined"] = 1;
                ViewBag.user_logined = Session["user_logined"];
                ViewBag.is_logined = Session["is_logined"];
                
                if (result[0].role == 0)
                {
                    return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
                }
                return RedirectToAction("Home", "HomeSales", new { area = "Sales" });
            }
            else
            {
                return View("~/Areas/Sales/Views/Login/Login.cshtml");
            }
        }
        public ActionResult Logout()
        {
            Session["is_logined"] = 0;
            return View("~/Areas/Sales/Views/Login/Login.cshtml");
        }

        public void Mix_PRODUCT_And_PRODUCT_Plus(List<PRODUCT> productlist, List<PRODUCT_Plus> productpluslist)
        {
            var result_sale = db.Database.SqlQuery<SALE>("exec get_all_from_SALES").ToList();
            foreach (var a in productlist)
            {
                PRODUCT_Plus c = new PRODUCT_Plus();
                c.product_id = a.product_id;
                c.category_id = a.category_id;
                c.sale_id = a.sale_id;
                c.name = a.name;
                c.price = a.price;
                c.brand = a.brand;
                c.sold = a.sold;
                c.size = a.size;
                c.content = a.content;
                c.image_link = a.image_link;
                foreach (var b in result_sale)
                {
                    if (b.sale_id == a.sale_id)
                    {
                        c.sale_name = b.sale_name;
                        c.percent = b.percent;
                    }
                }
                productpluslist.Add(c);
            }
        }
    }
}
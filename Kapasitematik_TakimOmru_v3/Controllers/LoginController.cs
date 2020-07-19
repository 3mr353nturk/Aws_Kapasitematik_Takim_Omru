using Kapasitematik_TakimOmru_v3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;

namespace Kapasitematik_TakimOmru_v3.Controllers
{
    public class LoginController : Controller
    {
        HashCode hs = new HashCode();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return Redirect("Login");
        }
        public ActionResult AddUser(FormCollection form)
        {

            TakimOmruDBEntities db = new TakimOmruDBEntities();
            User user = new User();
            //eğer dosya gelmişse işlemleri yap
            if (Request.Files.Count > 0)
            {
                //Guid nesnesini benzersiz dosya adı oluşturmak için tanımladık ve Replace ile aradaki “-” işaretini atıp yan yana yazma işlemi yaptık.
                string DosyaAdi = Guid.NewGuid().ToString().Replace("-", "");
                //Kaydetceğimiz resmin uzantısını aldık.
                string uzanti = System.IO.Path.GetExtension(Request.Files[0].FileName);
                string TamYolYeri = "/Images/" +DosyaAdi + uzanti;
                //Eklediğimiz Resni Server.MapPath methodu ile Dosya Adıyla birlikte kaydettik.
                Request.Files[0].SaveAs(Server.MapPath(TamYolYeri));
                //Ve veritabanına kayıt için dosya adımızı değişkene aktarıyoruz.
                user.CompanyLogo = DosyaAdi + uzanti;
            }

            user.Company = form["company"];
            user.FirstName = form["firstname"];
            user.LastName = form["lastname"];
            user.Email = form["email"];
            user.UserName = form["username"];
            //user.Password = hs.PassHass(form["password"]);
            user.Password = form["password"];
            db.User.Add(user);
            db.SaveChanges();
            return Redirect("Login");


            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("https://1ns5809cw0.execute-api.eu-west-2.amazonaws.com/Prod/");
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    HttpResponseMessage response = client.PostAsJsonAsync("api/Login/AddUser", user).Result;
            //    if (response.IsSuccessStatusCode)
            //    {
            //        return RedirectToAction("Login");
            //    }
            //    return null;
            //}
        }


        
        [ValidateAntiForgeryToken]
        public ActionResult Token(User User)
        {



            if (ModelState.IsValid)
            {
                using (TakimOmruDBEntities db = new TakimOmruDBEntities())
                {
                    var obj = db.User.Where(a => a.UserName.Equals(User.UserName) && a.Password.Equals(User.Password)).FirstOrDefault();

                    if (obj != null)
                    {
                        if (User.Remember==true)
                        {
                            Response.Cookies["Username"].Value = obj.UserName;
                            Response.Cookies["Password"].Value = obj.Password;
                            Response.Cookies["Username"].Expires = DateTime.Now.AddDays(1);
                            Response.Cookies["Password"].Expires = DateTime.Now.AddDays(1);

                        }
                        else
                        {
                            Response.Cookies["Username"].Expires = DateTime.Now.AddDays(-1);
                            Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);
                        }

                        Session.Add("UserID", obj.UserID.ToString());
                        Session.Add("Company", obj.Company.ToString());
                        Session.Add("Logo", obj.CompanyLogo.ToString());
                        Session.Add("FirstName", obj.FirstName.ToString()+" "+obj.LastName.ToString());

                        return RedirectToAction("TakimOmru", "Home");
                    }
                }
            }
            return RedirectToAction("Login", "Login");
            //TakimOmruDBEntities db = new TakimOmruDBEntities();

            //var u = db.User.Where(x => x.UserName == User.UserName && x.Password == hs.PassHass(User.Password));
            //if (u!=null)
            //{

            //    return RedirectToAction("TakimOmru", "Home");
            //}
            ////Session.Add("Company", u.Select(x=>x.Company));
            ////Session.Add("UserID", u.Select(x=>x.UserID));


            //return RedirectToAction("Login", "Login");

            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("https://1ns5809cw0.execute-api.eu-west-2.amazonaws.com/Prod/");
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    HttpResponseMessage response = client.PostAsJsonAsync("api/Login/Accesstoken",user).Result;
            //    if (response.IsSuccessStatusCode)
            //    {
            //        return RedirectToAction("TakimOmru","Home");
            //    }

            //    return View("Login");

            //}
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Login()
        {
            var username = Request.Form["Username"];
            var password = Request.Form["Password"];
            if (Request.Cookies["Username"] != null && Request.Cookies["Password"] != null)
            {
               
                username= Request.Cookies["Username"].Value;
                password = Request.Cookies["Password"].Value;
                
            }
            return View();
        }
    }
}
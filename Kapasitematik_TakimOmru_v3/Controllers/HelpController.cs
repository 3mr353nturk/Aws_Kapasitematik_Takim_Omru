using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kapasitematik_TakimOmru_v3.Controllers
{
    public class HelpController : Controller
    {
        // GET: Help
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.UserID = Session["UserID"];
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            ViewBag.Name = Session["FirstName"];
            return View();
        }
    }
}
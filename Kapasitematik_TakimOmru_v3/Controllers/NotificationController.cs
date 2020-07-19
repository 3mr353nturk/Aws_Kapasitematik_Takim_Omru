using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kapasitematik_TakimOmru_v3;
using Kapasitematik_TakimOmru_v3.Models;

namespace Kapasitematik_TakimOmru_v3.Controllers
{
    public class NotificationController : Controller
    {
        private TakimOmruDBEntities db = new TakimOmruDBEntities();

        public JsonResult NotificationList()
        {
            var sessionId = Convert.ToInt32(Session["UserID"]);
            var sonyedi = Request.Form.Get("yedigun");
            
            if (sonyedi!=null)
            {
                return Json(sonyedi);
            }
            List<NotificationModel> messages = new List<NotificationModel>();
            NotificationRepository r = new NotificationRepository();
            messages = r.NotificationList(sessionId);
            return Json(messages, JsonRequestBehavior.AllowGet);
        }
        public JsonResult NotificationTumList()
        {
            var sessionId = Convert.ToInt32(Session["UserID"]);
            var sonyedi = Request.Form.Get("yedigun");

            if (sonyedi != null)
            {
                return Json(sonyedi);
            }
            List<NotificationModel> messages = new List<NotificationModel>();
            NotificationTumRepository r = new NotificationTumRepository();
            messages = r.NotificationTumList(sessionId);
            return Json(messages, JsonRequestBehavior.AllowGet);
        }
        public JsonResult NotificationOtuzList()
        {
            var sessionId = Convert.ToInt32(Session["UserID"]);
            var sonyedi = Request.Form.Get("yedigun");

            if (sonyedi != null)
            {
                return Json(sonyedi);
            }
            List<NotificationModel> messages = new List<NotificationModel>();
            NotificationOtuzRepository r = new NotificationOtuzRepository();
            messages = r.NotificationOtuzList(sessionId);
            return Json(messages, JsonRequestBehavior.AllowGet);
        }
        // GET: Notification
        public ActionResult Index()
        {
            var notification = db.Notification.Include(n => n.User);
            return View(notification.ToList());
        }

        // GET: Notification/Details/5
        public ActionResult Details(int? id)
        {
            ViewBag.UserID = Session["UserID"];

            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            ViewBag.Name = Session["FirstName"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notification.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // GET: Notification/Create
        public ActionResult Create()
        {
            ViewBag.FKUserId = new SelectList(db.User, "UserID", "UserName");
            return View();
        }

        // POST: Notification/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NotificationID,Notification_Description,Count,Notification_Date,FKUserId")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Notification.Add(notification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FKUserId = new SelectList(db.User, "UserID", "UserName", notification.FKUserId);
            return View(notification);
        }

        // GET: Notification/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notification.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            ViewBag.FKUserId = new SelectList(db.User, "UserID", "UserName", notification.FKUserId);
            return View(notification);
        }

        // POST: Notification/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NotificationID,Notification_Description,Count,Notification_Date,FKUserId")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FKUserId = new SelectList(db.User, "UserID", "UserName", notification.FKUserId);
            return View(notification);
        }

        // GET: Notification/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notification.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Notification notification = db.Notification.Find(id);
            db.Notification.Remove(notification);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

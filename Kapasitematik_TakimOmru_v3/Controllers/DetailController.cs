using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kapasitematik_TakimOmru_v3;
using Kapasitematik_TakimOmru_v3.Models;
using PagedList;

namespace Kapasitematik_TakimOmru_v3.Controllers
{
    public class DetailController : Controller
    {
        private TakimOmruDBEntities db = new TakimOmruDBEntities();


        public JsonResult DetailList(string basTarih, string bitTarih)
        {
            var sessionId = Convert.ToInt32(Session["UserID"]);

            List<DetailModel> messages = new List<DetailModel>();
            DetailRepository r = new DetailRepository();
            DateTime start = DateTime.MinValue;
            DateTime end = DateTime.MaxValue;
            var sDs = basTarih;
            var eDs = bitTarih;
            DateTime.TryParse(sDs, out start);
            DateTime.TryParse(eDs, out end);
            messages = r.DetailList(sessionId);
            if (start != DateTime.MinValue && end != DateTime.MinValue)
            {
                messages = messages.Where(x => Convert.ToDateTime(x.CreatedDate) >= start && Convert.ToDateTime(x.CreatedDate) <= end).ToList();
            }
            return Json(messages, JsonRequestBehavior.AllowGet);
        }


        // GET: Detail
        public ActionResult Index(int? page)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var sessionId = Convert.ToInt32(Session["UserID"]);
            ViewBag.Name = Session["FirstName"];
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            List<DetailModel> messages = new List<DetailModel>();
            DetailRepository r = new DetailRepository();
            messages = r.DetailList(sessionId);
            return View(messages.ToList());
        }

        // GET: Detail/Details/5
        public ActionResult Details(int? id)
        {
            ViewBag.Name = Session["FirstName"];
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Detail detail = db.Detail.Find(id);
            if (detail == null)
            {
                return HttpNotFound();
            }
            return View(detail);
        }

        // GET: Detail/Create
        public ActionResult Create()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.Name = Session["FirstName"];
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            ViewBag.FKSubPieceID = new SelectList(db.SubPiece, "SubPieceID", "SubPieceName");
            //ViewBag.FKUserID = new SelectList(db.User, "UserID", "UserName");
            return View();
        }

        // POST: Detail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DetailID,PieceCount,FKSubPieceID")]Detail detail)
        {
            ViewBag.UserID = Session["UserID"];
            int ID = Convert.ToInt32(Session["UserID"]);


            if (ModelState.IsValid)
            {
                ViewBag.FKSubPieceID = new SelectList(db.SubPiece, "SubPieceID", "SubPieceName", detail.FKSubPieceID);
                db.Detail.Add(new Detail
                {
                    PieceCount = detail.PieceCount,
                    CreatedDate = DateTime.Now,
                    FKSubPieceID = detail.FKSubPieceID,
                    FKUserID=ID
                    
                });

                db.SaveChanges();

            }
 
           
            return RedirectToAction("Index");



            //ViewBag.FKUserID = new SelectList(db.User, "UserID", "UserName", detail.);

        }

        // GET: Detail/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.Name = Session["FirstName"];
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Detail detail = db.Detail.Find(id);
            if (detail == null)
            {
                return HttpNotFound();
            }
            ViewBag.FKSubPieceID = new SelectList(db.SubPiece, "SubPieceID", "SubPieceName", detail.FKSubPieceID);
            ViewBag.FKUserID = new SelectList(db.User, "UserID", "UserName", detail.FKUserID);
            return View(detail);
        }

        // POST: Detail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DetailID,PieceCount,CreatedDate,FKSubPieceID,FKUserID")] Detail detail)
        {
            ViewBag.UserID = Session["UserID"];
            int ID = Convert.ToInt32(Session["UserID"]);
            if (ModelState.IsValid)
            {
                detail.FKUserID = ID;
                db.Entry(detail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FKSubPieceID = new SelectList(db.SubPiece, "SubPieceID", "SubPieceName", detail.FKSubPieceID);
            ViewBag.FKUserID = new SelectList(db.User, "UserID", "UserName", detail.FKUserID);
            return View(detail);
        }

        // GET: Detail/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.Name = Session["FirstName"];
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Detail detail = db.Detail.Find(id);
            if (detail == null)
            {
                return HttpNotFound();
            }
            return View(detail);
        }

        // POST: Detail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Detail detail = db.Detail.Find(id);
            db.Detail.Remove(detail);
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

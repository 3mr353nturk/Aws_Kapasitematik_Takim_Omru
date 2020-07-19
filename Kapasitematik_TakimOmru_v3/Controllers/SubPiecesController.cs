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
using PagedList;

namespace Kapasitematik_TakimOmru_v3.Controllers
{
    public class SubPiecesController : Controller
    {
        private TakimOmruDBEntities db = new TakimOmruDBEntities();

        public JsonResult SubPieceList(string basTarih, string bitTarih)
        {
            var sessionId = Convert.ToInt32(Session["UserID"]);

            List<ParcaModel> messages = new List<ParcaModel>();
            ParcaRepository r = new ParcaRepository();
            DateTime start = DateTime.MinValue;
            DateTime end = DateTime.MaxValue;
            var sDs = basTarih;
            var eDs = bitTarih;
            DateTime.TryParse(sDs, out start);
            DateTime.TryParse(eDs, out end);
            messages = r.SubPieceList(sessionId);
            if (start != DateTime.MinValue && end != DateTime.MinValue)
            {
                messages = messages.Where(x => Convert.ToDateTime(x.CreatedDate) >= start && Convert.ToDateTime(x.CreatedDate) <= end).ToList();
            }
            return Json(messages, JsonRequestBehavior.AllowGet);
        }
        // GET: SubPieces
        public ActionResult Index()
        {
            var sessionId = Convert.ToInt32(Session["UserID"]);
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            ViewBag.Name = Session["FirstName"];
            List<ParcaModel> messages = new List<ParcaModel>();
            ParcaRepository r = new ParcaRepository();
            messages = r.SubPieceList(sessionId);
            return View(messages);
        }

        // GET: SubPieces/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubPiece subPiece = db.SubPiece.Find(id);
            if (subPiece == null)
            {
                return HttpNotFound();
            }
            return View(subPiece);
        }

        // GET: SubPieces/Create
        public ActionResult Create()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.Name = Session["FirstName"];
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            ViewBag.FKPieceID = new SelectList(db.Piece, "PieceID", "PieceName");
            ViewBag.FKUserID = new SelectList(db.User, "UserID", "UserName");
            return View();
        }

        // POST: SubPieces/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SubPieceID,SubPieceName,ToolLife,Now,FKPieceID,FKUserID,Type")] SubPiece subPiece)
        {
            ViewBag.UserID = Session["UserID"];
            int ID = Convert.ToInt32(Session["UserID"]);
            if (ModelState.IsValid)
            {
                db.SubPiece.Add(new SubPiece()
                {
                    SubPieceName = subPiece.SubPieceName,
                    ToolLife = subPiece.ToolLife,
                    FKPieceID = subPiece.FKPieceID,
                    Type = subPiece.Type,
                    FKUserID = ID
                });
                //db.SubPiece.Add(subPiece);
                db.SaveChanges();

                return RedirectToAction("TakimOmru","Home");
            }

            ViewBag.FKPieceID = new SelectList(db.Piece, "PieceID", "PieceName", subPiece.FKPieceID);
            ViewBag.FKUserID = new SelectList(db.User, "UserID", "UserName", subPiece.FKUserID);
            return View(subPiece);
        }

        // GET: SubPieces/Edit/5
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
            SubPiece subPiece = db.SubPiece.Find(id);
            if (subPiece == null)
            {
                return HttpNotFound();
            }
            ViewBag.FKPieceID = new SelectList(db.Piece, "PieceID", "PieceName", subPiece.FKPieceID);
            ViewBag.FKUserID = new SelectList(db.User, "UserID", "UserName", subPiece.FKUserID);
            return View(subPiece);
        }

        // POST: SubPieces/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SubPieceID,SubPieceName,ToolLife,Now,FKPieceID,FKUserID,Type")] SubPiece subPiece)
        {
            ViewBag.UserID = Session["UserID"];
            int ID = Convert.ToInt32(Session["UserID"]);
            if (ModelState.IsValid)
            {
                subPiece.FKUserID = ID;
                db.Entry(subPiece).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FKPieceID = new SelectList(db.Piece, "PieceID", "PieceName", subPiece.FKPieceID);
            ViewBag.FKUserID = new SelectList(db.User, "UserID", "UserName", subPiece.FKUserID);
            return View(subPiece);
        }

        // GET: SubPieces/Delete/5
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
            SubPiece subPiece = db.SubPiece.Find(id);
            if (subPiece == null)
            {
                return HttpNotFound();
            }
            return View(subPiece);
        }

        // POST: SubPieces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SubPiece subPiece = db.SubPiece.Find(id);
            db.SubPiece.Remove(subPiece);

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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kapasitematik_TakimOmru_v3;

namespace Kapasitematik_TakimOmru_v3.Controllers
{
    public class NotesController : Controller
    {
        private TakimOmruDBEntities db = new TakimOmruDBEntities();

        // GET: Notes
        public ActionResult Index()
        {
            var notes = db.Notes.Include(n => n.Piece).Include(n => n.SubPiece).Include(n => n.User);
            return View(notes.ToList());
        }

        // GET: Notes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // GET: Notes/Create
        public ActionResult Create()
        {
            ViewBag.FkPieceID = new SelectList(db.Piece, "PieceID", "PieceName");
            ViewBag.FkSubPieceID = new SelectList(db.SubPiece, "SubPieceID", "SubPieceName");
            ViewBag.FkUserID = new SelectList(db.User, "UserID", "UserName");
            return View();
        }

        // POST: Notes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNoteTakim(FormCollection form, [Bind(Include = "NoteID,NoteHeader,NoteDescription,CreatedDate,FkPieceID,FkSubPieceID,FkUserID")] Note note)
        {
            var takimid = Convert.ToInt32(form["nottakimid"]);
            var typetakim = Convert.ToBoolean(form["typetakim"]);
            ViewBag.UserID = Session["UserID"];
            int ID = Convert.ToInt32(Session["UserID"]);
            if (ModelState.IsValid)
            {
                db.Notes.Add(new Note()
                {
                    NoteHeader=note.NoteHeader,
                    NoteDescription=note.NoteDescription,
                    CreatedDate=note.CreatedDate.GetValueOrDefault(DateTime.Now),
                    FkPieceID=takimid,
                    FkSubPieceID=note.FkSubPieceID,
                    FkUserID=ID,
                    Type=typetakim
                });
                db.SaveChanges();
                return RedirectToAction("TakimOmru","Home");
            }

            ViewBag.FkPieceID = new SelectList(db.Piece, "PieceID", "PieceName", note.FkPieceID);
            ViewBag.FkSubPieceID = new SelectList(db.SubPiece, "SubPieceID", "SubPieceName", note.FkSubPieceID);
            ViewBag.FkUserID = new SelectList(db.User, "UserID", "UserName", note.FkUserID);
            return View(note);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNoteParca(FormCollection form, [Bind(Include = "NoteID,NoteHeader,NoteDescription,CreatedDate,FkPieceID,FkSubPieceID,FkUserID")] Note note)
        {
            var typeparca =Convert.ToBoolean(form["typeparca"]);
            var parcaid = Convert.ToInt32(form["notparcaid"]);
            ViewBag.UserID = Session["UserID"];
            int ID = Convert.ToInt32(Session["UserID"]);
            if (ModelState.IsValid)
            {
                db.Notes.Add(new Note()
                {
                    NoteHeader = note.NoteHeader,
                    NoteDescription = note.NoteDescription,
                    CreatedDate = note.CreatedDate.GetValueOrDefault(DateTime.Now),
                    FkPieceID = note.FkPieceID,
                    FkSubPieceID = parcaid,
                    FkUserID = ID,
                    Type =typeparca
                });
                db.SaveChanges();
                return RedirectToAction("TakimOmru","Home");
            }

            ViewBag.FkPieceID = new SelectList(db.Piece, "PieceID", "PieceName", note.FkPieceID);
            ViewBag.FkSubPieceID = new SelectList(db.SubPiece, "SubPieceID", "SubPieceName", note.FkSubPieceID);
            ViewBag.FkUserID = new SelectList(db.User, "UserID", "UserName", note.FkUserID);
            return View(note);
        }

        // GET: Notes/Edit/5
        public ActionResult EditGetir(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            ViewBag.FkPieceID = new SelectList(db.Piece, "PieceID", "PieceName", note.FkPieceID);
            ViewBag.FkSubPieceID = new SelectList(db.SubPiece, "SubPieceID", "SubPieceName", note.FkSubPieceID);
            ViewBag.FkUserID = new SelectList(db.User, "UserID", "UserName", note.FkUserID);
            return View(note);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NoteID,NoteHeader,NoteDescription,CreatedDate,FkPieceID,FkSubPieceID,FkUserID")] Note note)
        {
            if (ModelState.IsValid)
            {
                db.Entry(note).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FkPieceID = new SelectList(db.Piece, "PieceID", "PieceName", note.FkPieceID);
            ViewBag.FkSubPieceID = new SelectList(db.SubPiece, "SubPieceID", "SubPieceName", note.FkSubPieceID);
            ViewBag.FkUserID = new SelectList(db.User, "UserID", "UserName", note.FkUserID);
            return View(note);
        }

        //// GET: Notes/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Note note = db.Notes.Find(id);
        //    if (note == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(note);
        //}

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Note note = db.Notes.Find(id);
            db.Notes.Remove(note);
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

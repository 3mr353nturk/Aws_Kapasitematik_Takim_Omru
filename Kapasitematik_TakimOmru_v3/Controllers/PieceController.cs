using Kapasitematik_TakimOmru_v3.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using PagedList;
using System.Data.Entity;

namespace Kapasitematik_TakimOmru_v3.Controllers
{
    public class PieceController : Controller
    {
        public ActionResult Upload(FormCollection formCollection)
        {
            var pieceList = new List<Kapasitematik_TakimOmru_v3.Piece>();
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 1; rowIterator <= noOfRow; rowIterator++)
                        {
                            var piece = new Kapasitematik_TakimOmru_v3.Piece();
                            piece.PieceName = workSheet.Cells[rowIterator, 1].Value.ToString();
                            pieceList.Add(piece);
                        }
                    }
                }
            }
            using (TakimOmruDBEntities db = new TakimOmruDBEntities())
            {
                foreach (var item in pieceList)
                {
                    db.Piece.Add(item);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Indexx", "Piece");
        }
        public ActionResult Export()
        {
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            var kategori = db.Piece.ToList();
            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)xla.ActiveSheet;
            ws.Name = "Takımlar";
            xla.Visible = true;
            ws.Cells[1, 1] = "Takım Adı";
            var i = 2;
            foreach (var item in kategori)
            {
                ws.Cells[i, 1] = item.PieceName;
                i++;
            }
            
            return RedirectToAction("TakimOmru", "Home");
        }
        public ActionResult InsertPiece([FromBody]Piece piece)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://1ns5809cw0.execute-api.eu-west-2.amazonaws.com/Prod/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/prod/parcaeklee", piece).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ParcaEkle");
                }
                return null;
            }
        }
        [System.Web.Http.HttpPost]
        public ActionResult ParcaEklee(Piece piece)
        {
            var sessionId = Convert.ToInt32(Session["UserID"]);
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            ViewBag.Data = new SelectList(db.PimsunDeviceNoes, "DeviceNo", "MachineName", piece.FKDeviceID);
            try
            {
                db.Piece.Add(new Piece()
                {
                    PieceName = piece.PieceName,
                    CreatedDate = piece.CreatedDate.GetValueOrDefault(DateTime.Now),
                    FKUserID = sessionId,
                    FKDeviceID=piece.FKDeviceID

                });
                db.SaveChanges();
                return RedirectToAction("Indexx");
            }
            catch (Exception )
            {
                return RedirectToAction("ParcaEkle");
               
            }
            
            
        }


        public ActionResult EditPiece([FromBody]Piece piece)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://1ns5809cw0.execute-api.eu-west-2.amazonaws.com/Prod/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/prod/parcaedit", piece).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Indexx");
                }
                return null;
            }
        }
        [System.Web.Http.HttpPost]
        public ActionResult DeletePiece(int? id)
        {
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            Piece piece = db.Piece.Find(id);
            db.Piece.Remove(piece);
            db.SaveChanges();
            return RedirectToAction("Indexx");
        }
        //Get
        public ActionResult Edit(int? id)
        {
            ViewBag.Name = Session["FirstName"];
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Piece piece = db.Piece.Find(id);
            if (piece == null)
            {
                return HttpNotFound();
            }
            return View(piece);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PieceID,PieceName,FKUserID,CreatedDate,Adet")] Piece piece)
        {
            ViewBag.UserID = Session["UserID"];
            int ID = Convert.ToInt32(Session["UserID"]);
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            if (ModelState.IsValid)
            {
                piece.FKUserID = ID;
                piece.Adet = 0;
                piece.CreatedDate = DateTime.Now;
                db.Entry(piece).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Indexx");
            }
            ViewBag.FKUserID = new SelectList(db.User, "UserID", "UserName", piece.FKUserID);
            return View(piece);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "DetailID,PieceCount,CreatedDate,FKSubPieceID,FKUserID")] Detail detail)
        //{
        //    ViewBag.UserID = Session["UserID"];
        //    int ID = Convert.ToInt32(Session["UserID"]);
        //    if (ModelState.IsValid)
        //    {
        //        detail.FKUserID = ID;
        //        db.Entry(detail).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.FKSubPieceID = new SelectList(db.SubPiece, "SubPieceID", "SubPieceName", detail.FKSubPieceID);
        //    ViewBag.FKUserID = new SelectList(db.User, "UserID", "UserName", detail.FKUserID);
        //    return View(detail);
        //}

        // GET: Piece/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.Name = Session["FirstName"];
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Piece piece = db.Piece.Find(id);
            if (piece == null)
            {
                return HttpNotFound();
            }
            return View(piece);
        }
        public ActionResult ParcaEkle()
        {
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var item = db.PimsunDeviceNoes.Select(x => new
            {
                x.DeviceNo,
                x.MachineName

            }).ToList();
            ViewBag.Data = item;
            ViewBag.Name = Session["FirstName"];
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            return View();
        }


        public ActionResult Indexx(int? page)
        {
            var sessionId = Convert.ToInt32(Session["UserID"]);
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.Name = Session["FirstName"];
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://1ns5809cw0.execute-api.eu-west-2.amazonaws.com/Prod/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/prod/piecelist").Result;
                var result = new List<Kapasitematik_TakimOmru_v3.Models.Piece>();
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsAsync<List<Kapasitematik_TakimOmru_v3.Models.Piece>>().Result;
                }
                List<PieceModels> messages = new List<PieceModels>();
                Repository r = new Repository();
                messages = r.PieceList(sessionId);
                return View(messages);
            }
        }
        public JsonResult PieceList(string basTarih, string bitTarih)
        {
            var sessionId = Convert.ToInt32(Session["UserID"]);
            List<PieceModels> messages = new List<PieceModels>();
            Repository r = new Repository();
            DateTime start = DateTime.MinValue;
            DateTime end = DateTime.MaxValue;
            var sDs = basTarih;
            var eDs = bitTarih;
            DateTime.TryParse(sDs, out start);
            DateTime.TryParse(eDs, out end);
            messages = r.PieceList(sessionId);
            if (start != DateTime.MinValue && end != DateTime.MinValue)
            {
                messages = messages.Where(x => Convert.ToDateTime(x.CreatedDate) >= start && Convert.ToDateTime(x.CreatedDate) <= end).ToList();
            }
            return Json(messages, JsonRequestBehavior.AllowGet);
        }
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e) //this will be called when any changes occur in db table.  
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MyHub.SendPiece();
            }
        }
    }
}
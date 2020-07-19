using Kapasitematik_TakimOmru_v3.Models;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Kapasitematik_TakimOmru_v3.Controllers
{
    public class SubPieceController : Controller
    {
        // GET: SubPiece
        public ActionResult Index()
        {
            ViewBag.Name = Session["FirstName"];
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            return View();
        }

        public ActionResult InsertSubPiece([FromBody]SubPiece subpiece)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://x1mrph0du3.execute-api.eu-west-2.amazonaws.com/Prod/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/prod/altparcaeklee", subpiece).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                return null;
            }
        }
        public ActionResult UpdateSubPiece([FromBody]Piece piece)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://x1mrph0du3.execute-api.eu-west-2.amazonaws.com/Prod/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/Home/AltParcaGuncelle", piece).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                return null;
            }
        }
        public ActionResult DeleteSubPiece(int id)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://x1mrph0du3.execute-api.eu-west-2.amazonaws.com/Prod/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/Home/AltParcaSil", id).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                return null;
            }
        }

        public ActionResult Upload(FormCollection formCollection)
        {

            var subpieceList = new List<Kapasitematik_TakimOmru_v3.SubPiece>();
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
                            var subpiece = new Kapasitematik_TakimOmru_v3.SubPiece();
                            
                            subpiece.SubPieceName = workSheet.Cells[rowIterator, 1].Value.ToString();

                            subpiece.ToolLife =Convert.ToInt32( workSheet.Cells[rowIterator, 2].Value);
                           
                            

                            subpieceList.Add(subpiece);
                        }
                    }
                }
            }
            using (TakimOmruDBEntities db = new TakimOmruDBEntities())
            {
                foreach (var item in subpieceList)
                {
                    db.SubPiece.Add(new SubPiece()
                    {
                        
                        SubPieceName=item.SubPieceName,
                        ToolLife=item.ToolLife,
                        FKPieceID=Convert.ToInt32(formCollection["id"]),
                        Type=Convert.ToBoolean("false")
                        
                    });
                }
                db.SaveChanges();

            }
            return RedirectToAction("TakimOmru", "Home");
        }


        public ActionResult ExportAll()
        {
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            var subpiece = db.SubPiece.ToList();
            var detail = db.Detail.ToList();
            var altkategori = from s in subpiece
                              join dt in detail on s.SubPieceID equals dt.FKSubPieceID
                             
                              select new
                              {
                                  SubPieceId = s.SubPieceID,
                                  SubPieceName = s.SubPieceName,
                                  ToolLife = s.ToolLife,
                                  PieceId=s.FKPieceID,
                                  PieceCount = dt.PieceCount,
                                  CreatedDate = dt.CreatedDate.Value.ToString("dd.MM.yyyy hh:mm")

                              };
            var data = from obj in altkategori
                       select new
                       {
                           SubPieceId=obj.SubPieceId,
                           SubPieceName = obj.SubPieceName,
                           ToolLife = obj.ToolLife,
                           PieceId=obj.PieceId,
                           PieceCount = obj.PieceCount,
                           CreatedDate = obj.CreatedDate
                       };
            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)xla.ActiveSheet;
            
            xla.Visible = true;

            ws.Cells[1, 1] = "Parça Adı";
            ws.Cells[1, 2] = "Takım Ömrü";
            ws.Cells[1, 3] = "Parça Sayısı";
            ws.Cells[1, 4] = "Eklenme Tarihi";
            var i = 2;
            foreach (var item in data)
            {
                //ws.Name = item.PieceId.ToString();
                ws.Cells[i, 1] = item.SubPieceName;
                ws.Cells[i, 2] = item.ToolLife;
                ws.Cells[i, 3] = item.PieceCount;
                ws.Cells[i, 4] = item.CreatedDate;

                i++;

            }


            return RedirectToAction("TakimOmru", "Home");
        }

        public ActionResult Export(FormCollection form)
        {
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            var subpiece = db.SubPiece.ToList();
            var detail = db.Detail.ToList();
            var altkategori = from s in subpiece
                              join dt in detail on s.SubPieceID equals dt.FKSubPieceID
                              where s.FKPieceID== Convert.ToInt32(form["idsubpiece"])
                              select new
                              {
                                  SubPieceId = s.SubPieceID,
                                  SubPieceName = s.SubPieceName,
                                  ToolLife = s.ToolLife,
                                  PieceId = s.FKPieceID,
                                  PieceCount = dt.PieceCount,
                                  CreatedDate = dt.CreatedDate.Value.ToString("dd.MM.yyyy hh:mm")

                              };
            var data = from obj in altkategori
                       select new
                       {
                           SubPieceId = obj.SubPieceId,
                           SubPieceName = obj.SubPieceName,
                           ToolLife = obj.ToolLife,
                           PieceId = obj.PieceId,
                           PieceCount = obj.PieceCount,
                           CreatedDate = obj.CreatedDate
                       };
            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)xla.ActiveSheet;

            xla.Visible = true;

            ws.Cells[1, 1] = "Parça Adı";
            ws.Cells[1, 2] = "Takım Ömrü";
            ws.Cells[1, 3] = "Parça Sayısı";
            ws.Cells[1, 4] = "Eklenme Tarihi";
            var i = 2;
            foreach (var item in data)
            {
                
                ws.Name = item.PieceId.ToString();
                ws.Cells[i, 1] = item.SubPieceName;
                ws.Cells[i, 2] = item.ToolLife;
                ws.Cells[i, 3] = item.PieceCount;
                ws.Cells[i, 4] = item.CreatedDate;

                i++;

            }


            return RedirectToAction("TakimOmru", "Home");
        }

        public ActionResult AltParcaEkle()
        {
            return View();
        }
        public ActionResult AltParcaGuncelle()
        {
            return View();
        }
        public ActionResult AltParcaSil()
        {
            return View();
        }
    }
}
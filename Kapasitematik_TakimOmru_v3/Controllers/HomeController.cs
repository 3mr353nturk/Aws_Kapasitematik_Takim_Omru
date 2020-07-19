using Kapasitematik_TakimOmru_v3;
using Kapasitematik_TakimOmru_v3.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Kapasitematik_TakimOmru_v3.Controllers
{

    public class HomeController : Controller
    {


        public JsonResult KategoriListe(int machineId)
        {
            var sessionId = Convert.ToInt32(Session["UserID"]);

            List<PieceModels> messages = new List<PieceModels>();
            RepositoryGridPiece r = new RepositoryGridPiece();

            messages = r.GridList(sessionId,machineId);

            return Json(messages, JsonRequestBehavior.AllowGet);
        }
        public JsonResult MachineListe()
        {
            //var sessionId = Convert.ToInt32(Session["UserID"]);

            List<MachineModels> messages = new List<MachineModels>();
            MachineRepository r = new MachineRepository();

            messages = r.MachineList();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult DetayListe(int SubPieceId)
        {
            var sessionId = Convert.ToInt32(Session["UserID"]);

            List<SDetayModel> messages = new List<SDetayModel>();
            DetayRepository r = new DetayRepository();
            messages = r.DetayListe(sessionId,SubPieceId);
            return Json(messages, JsonRequestBehavior.AllowGet);
        }
        public ActionResult About(Piece piece)
        {
            ViewBag.Message = "Your application description page.";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://1ns5809cw0.execute-api.eu-west-2.amazonaws.com/Prod/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/prod/savepiece", piece).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ParcaEkle");
                }
                return null;
            }

        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //public ActionResult ParcaGuncelle(SubPiece subPiece, FormCollection form)
        //{
        //    var id = Convert.ToInt32(form["par"]);
        //    using (var ctx = new TakimOmruDBEntities())
        //    {
        //        var existingsubpiece = ctx.SubPiece.Where(s => s.SubPieceID == id)
        //                                                .FirstOrDefault();

        //        if (existingsubpiece != null)
        //        {
        //            existingsubpiece.SubPieceName = subPiece.SubPieceName;
        //            existingsubpiece.ToolLife = subPiece.ToolLife;

        //            ctx.SaveChanges();
        //        }
        //    }
        //    return RedirectToAction("TakimOmru", "Home");
        //}

        public ActionResult ParcaGuncelle(int ID)
        {
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            ThreeModel test = new ThreeModel();
            test.subPiece = db.SubPiece.Find(ID);
            return RedirectToAction("TakimOmru", "Home",test);
        }

        //public ActionResult ParcaGuncellee(int ID)
        //{
        //    TakimOmruDBEntities db = new TakimOmruDBEntities();
        //    ThreeModel test = new ThreeModel();
        //    test.subPiece = db.SubPiece.Find(ID);
        //    return RedirectToAction("TakimOmru", "Home");
        //}
        [System.Web.Mvc.HttpPost]
        public ActionResult ParcaGuncelle(SubPiece subPiece, FormCollection form)
        {
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            var id = Convert.ToInt32(form["par"]);
            var existingsubpiece = db.SubPiece.Where(s => s.SubPieceID == id).FirstOrDefault();
            if (existingsubpiece != null)
            {
                existingsubpiece.SubPieceName = subPiece.SubPieceName;
                existingsubpiece.ToolLife = subPiece.ToolLife;
                db.SaveChanges();
            }
            return RedirectToAction("TakimOmru", "Home");
        }

        public ActionResult ParcaSil(FormCollection form)
        {
            var id = Convert.ToInt32(form["parcaid"]);
            using (var ctx = new TakimOmruDBEntities())
            {
                SubPiece sub = ctx.SubPiece.Find(id);
                ctx.SubPiece.Remove(sub);
                ctx.SaveChanges();
            }
            return RedirectToAction("TakimOmru","Home");
        }
        public ActionResult Profil()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            TakimOmruDBEntities db = new TakimOmruDBEntities();
            ViewBag.UserID = Session["UserID"];
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            ViewBag.Name = Session["FirstName"];
            int ID = Convert.ToInt32(Session["UserID"]);
            return View(db.User.Where(x=>x.UserID==ID).ToList());
        }
        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.UserID = Session["UserID"];
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            ViewBag.Name = Session["FirstName"];
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [System.Web.Http.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserEdit([Bind(Include = "UserID,UserName,Password,FirstName,LastName,Company,CompanyLogo,Email")] User user)
        {
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    //Guid nesnesini benzersiz dosya adı oluşturmak için tanımladık ve Replace ile aradaki “-” işaretini atıp yan yana yazma işlemi yaptık.
                    string DosyaAdi = Guid.NewGuid().ToString().Replace("-", "");
                    //Kaydetceğimiz resmin uzantısını aldık.
                    string uzanti = System.IO.Path.GetExtension(Request.Files[0].FileName);
                    string TamYolYeri = "/Images/" + DosyaAdi + uzanti;
                    //Eklediğimiz Resni Server.MapPath methodu ile Dosya Adıyla birlikte kaydettik.
                    Request.Files[0].SaveAs(Server.MapPath(TamYolYeri));
                    //Ve veritabanına kayıt için dosya adımızı değişkene aktarıyoruz.
                    user.CompanyLogo = DosyaAdi + uzanti;
                }
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Profil", "Home");
            }

            return View(user);
        }
        public ActionResult TakimOmru(Notification noti, [Bind(Include = "NoteID,NoteHeader,NoteDescription,CreatedDate,FkPieceID,FkSubPieceID,FkUserID")] Note note)
        {

            //ActionResult result = null;
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            ViewBag.FKPieceID = new SelectList(db.Piece, "PieceID", "PieceName");
            //ViewBag.FkSubPieceID = new SelectList(db.SubPiece, "SubPieceID", "SubPieceName");
            //ViewBag.FkUserID = new SelectList(db.User, "UserID", "UserName");
            
            if (Session["UserID"]==null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.UserID = Session["UserID"];
            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            ViewBag.Name = Session["FirstName"];
            int UserId = Convert.ToInt32(Session["UserID"]);
            ThreeModel tm = new ThreeModel();
            tm.notification = db.Notification.ToList();
            tm.pieces = db.Piece.ToList();
            tm.subpieces = db.SubPiece.ToList();
            tm.notes = db.Notes.ToList();
           
            ViewBag.FkPieceIDd = new SelectList(db.Piece, "PieceID", "PieceName", note.FkPieceID);
            ViewBag.FkSubPieceID = new SelectList(db.SubPiece, "SubPieceID", "SubPieceName", note.FkSubPieceID);
            ViewBag.FkUserID = new SelectList(db.User, "UserID", "UserName", note.FkUserID);
            
            var altkategori = from s in db.SubPiece
                              join dt in db.Detail on s.SubPieceID equals dt.FKSubPieceID
                              where s.FKUserID == UserId
                              select new
                              {
                                  SubPieceId = s.SubPieceID,
                                  SubPieceName = s.SubPieceName,
                                  ToolLife = s.ToolLife,
                                  PieceID = s.FKPieceID,
                                  PieceCount = dt.PieceCount,


                              };
            var data = altkategori.GroupBy(x => x.SubPieceId).Select(x => new
            {
                SubPieceId = x.FirstOrDefault().SubPieceId,
                SubPieceName = x.FirstOrDefault().SubPieceName,
                ToolLife = x.FirstOrDefault().ToolLife,
                PieceID = x.FirstOrDefault().PieceID,
                PieceCount = x.Sum(y => y.PieceCount),


            }).Where(v => v.PieceCount > (v.ToolLife * 2) / 3);
            if (data != null && data.Any())
            {

                foreach (var item in data)
                {

                    db.Notification.Add(new Notification()
                    {
                        NotificationID=noti.NotificationID,
                        Notification_Description = noti.Notification_Description = (item.SubPieceName) + " " + "parçasının ömrü tükenmek üzere Kalan parça sayısı " + (item.ToolLife - item.PieceCount),
                        Count = noti.Count = 0,
                        FKUserId =UserId,
                        Notification_Date = noti.Notification_Date.GetValueOrDefault(DateTime.Now),
                        
                    });
                    
                }
                
                db.SaveChanges();
                
            }

            //var noti = db.Notification.ToList();


            //ViewBag.PieceID = new SelectList(db.Piece, "PieceId", "PieceName", subPiece.FKPieceID);
            //ViewBag.Message = "Your application description page.";

            //ViewBag.Piece = new SelectList(db.Piece, "PieceId", "PieceName");
           //var prodecure= db.SP_Notification_Update_Counter_v1(UserId);
            return View(tm);

        }
        public ActionResult Create([Bind(Include = "SubPieceID,SubPieceName,ToolLife,Now,FKPieceID,FKUserID,Type")] SubPiece subPiece)
        {
            //ViewBag.Data = new SelectList(db.PimsunDeviceNoes, "DeviceNo", "MachineName", piece.FKDeviceID);
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            if (ModelState.IsValid)
            {
                db.SubPiece.Add(subPiece);
                db.SaveChanges();
                return RedirectToAction("TakimOmru","Home");
            }

            ViewBag.FKPieceID = new SelectList(db.Piece, "PieceID", "PieceName", subPiece.FKPieceID);
            ViewBag.FKUserID = new SelectList(db.User, "UserID", "UserName", subPiece.FKUserID);
            return RedirectToAction("TakimOmru", "Home");
        }
        //public JsonResult PieceList()
        //{
        //    List<PieceModel> messages = new List<PieceModel>();
        //    Repository r = new Repository();
        //    messages = r.PieceList();
        //    return Json(messages, JsonRequestBehavior.AllowGet);
        //}



        //public JObject View_Notification(String ID)
        //{

        //    JObject objResult = new JObject();

        //    try
        //    {
        //        string str = WebApiConfig.URL_Notification + "/" + ID + "?app_id=" + WebApiConfig.APP_ID;

        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(str);
        //        request.ContentType = "application/json; charset=utf-8";
        //        request.Headers["Authorization"] = "Basic " + WebApiConfig.API_KEY;
        //        request.PreAuthenticate = true;
        //        HttpWebResponse response = request.GetResponse() as HttpWebResponse;


        //        using (Stream responseStream = response.GetResponseStream())
        //        {
        //            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
        //            JObject Jobject = JObject.Parse(reader.ReadToEnd());

        //            int successful = Convert.ToInt32(Jobject.GetValue("successful"));
        //            int converted = Convert.ToInt32(Jobject.GetValue("converted"));
        //            int rate_of_click = (100 / successful * converted);


        //            objResult.Add("app_id", Jobject.GetValue("app_id"));
        //            objResult.Add("id", Jobject.GetValue("id"));
        //            objResult.Add("successful", successful);
        //            objResult.Add("converted", converted);
        //            objResult.Add("rate_of_click", "%" + rate_of_click);
        //            objResult.Add("canceled", Jobject.GetValue("canceled"));
        //            objResult.Add("headings", Jobject.GetValue("headings")["en"]);
        //            objResult.Add("contents", Jobject.GetValue("contents")["en"]);
        //            objResult.Add("url", Jobject.GetValue("url"));
        //            objResult.Add("big_picture", Jobject.GetValue("big_picture"));
        //            objResult.Add("large_icon", Jobject.GetValue("large_icon"));


        //        }
        //        return objResult;
        //    }
        //    catch (Exception e)
        //    {
        //        JObject objError = new JObject();
        //        objError.Add("messages", e.ToString());


        //        return objError;

        //    }

        //}
        public ActionResult Kategori()
        {

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;

            var startDateString = Request.Form.Get("query[filterDatess][firstdatee]");
            var endDateString = Request.Form.Get("query[filterDatess][lastdatee]");


            if (string.IsNullOrEmpty(startDateString) == false && string.IsNullOrEmpty(endDateString) == false)
            {
                DateTime.TryParse(startDateString, out startDate);
                DateTime.TryParse(endDateString, out endDate);
            }

            using (var client = new HttpClient())
            {
                var userid = Session["UserID"];
                client.BaseAddress = new Uri("https://1ns5809cw0.execute-api.eu-west-2.amazonaws.com/Prod/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/prod/kategori?FkUserID=" + userid).Result;

                var result = new List<PieceModel>();

                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsAsync<List<PieceModel>>().Result;

                    if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                    {
                        result = result.Where(x => x.CreatedDate.Date >= startDate.Date && x.CreatedDate.Date <= endDate.Date).ToList();
                    }
                }

                return Json(result);
                //return View(result);
            }
        }

        public ActionResult KategoriTime()
        {

            using (var client = new HttpClient())
            {
                var userid = Session["UserID"];
                client.BaseAddress = new Uri("https://1ns5809cw0.execute-api.eu-west-2.amazonaws.com/Prod/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/prod/kategoritime?FkUserID=" + userid).Result;

                var result = new List<PieceModel>();

                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsAsync<List<PieceModel>>().Result;
                }

                return Json(result);
                //return View(result);
            }
        }

        public ActionResult AltKategori(string pieceId)
        {

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;

            var startDateString = Request.Form.Get("query[filterDates][firstdate]");
            var endDateString = Request.Form.Get("query[filterDates][lastdate]");


            if (string.IsNullOrEmpty(startDateString) == false && string.IsNullOrEmpty(endDateString) == false)
            {
                DateTime.TryParse(startDateString, out startDate);
                DateTime.TryParse(endDateString, out endDate);
            }


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://1ns5809cw0.execute-api.eu-west-2.amazonaws.com/Prod/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/prod/altkategori?PieceId=" + pieceId).Result;

                //HttpResponseMessage response = client.GetAsync("api/prod/altkategori?PieceId=" + pieceId+ "&startDate="startDate+ "&endDate="endate ).Result; //TODO 

                var result = new List<AltKategoriModel>();

                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsAsync<List<AltKategoriModel>>().Result;

                    if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                    {
                        result = result.Where(x => x.createdDate.Date >= startDate.Date && x.createdDate.Date <= endDate.Date).ToList();
                    }
                }




                return Json(result);
            }

        }


        public ActionResult AltKategoriProgress(string pieceId)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://1ns5809cw0.execute-api.eu-west-2.amazonaws.com/Prod/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/prod/altkategoriprogress?PieceId=" + pieceId).Result;

                var result = new List<AltKategoriModel>();

                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsAsync<List<AltKategoriModel>>().Result;
                }

                return Json(result);
            }

        }

        public JsonResult Detayy(string subpieceId)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://1ns5809cw0.execute-api.eu-west-2.amazonaws.com/Prod/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/prod/detayy?SubPieceId=" + subpieceId).Result;

                var result = new List<DetayModel>();

                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsAsync<List<DetayModel>>().Result;
                }

                return Json(result);
            }

        }


        public ActionResult AltKategoriTime(string pieceId)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://1ns5809cw0.execute-api.eu-west-2.amazonaws.com/Prod/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/prod/altkategoritime?PieceId=" + pieceId).Result;

                var result = new List<AltKategoriModel>();

                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsAsync<List<AltKategoriModel>>().Result;
                }

                return Json(result);
            }

        }



        //public ActionResult Notification(Notification notification)
        //{

        //    TakimOmruDBEntities db = new TakimOmruDBEntities();


        //    var altkategori = from s in db.SubPiece
        //                      join dt in db.Detail on s.SubPieceID equals dt.FKSubPieceID

        //                      select new
        //                      {
        //                          SubPieceId = s.SubPieceID,
        //                          SubPieceName = s.SubPieceName,
        //                          ToolLife = s.ToolLife,
        //                          PieceID = s.FKPieceID,
        //                          PieceCount = dt.PieceCount,
        //                          CreatedDate = dt.CreatedDate.Value.ToString("dd'/'MM'/'yyyy hh:mm")

        //                      };
        //    var data = altkategori.GroupBy(x => x.SubPieceId).Select(x => new
        //    {
        //        SubPieceId = x.First().SubPieceId,
        //        SubPieceName = x.First().SubPieceName,
        //        ToolLife = x.First().ToolLife,
        //        PieceID = x.First().PieceID,
        //        PieceCount = x.Sum(y => y.PieceCount),
        //        CreatedDate = x.First().CreatedDate
        //    }).Where(x => (x.PieceCount) > (x.ToolLife * 2) / 3);

        //    if (data != null)
        //    {
        //        db.Notification.Add(new Notification
        //        {
        //            Notification_Description = data.Select(x => x.SubPieceName) + "Parçasının ömrü tükenmek üzere",
        //            Notification_Date = DateTime.Now
        //        });
        //    }

        //    return Json(data);

        //}




        //public ActionResult Detay(int id)
        //{

        //    TakimOmruDBEntities db = new TakimOmruDBEntities();


        //    var altkategori = from s in db.SubPiece
        //                      join dt in db.Detail on s.SubPieceID equals dt.FKSubPieceID
        //                      where dt.FKSubPieceID==id
        //                      select new
        //                      {
        //                          SubPieceId = s.SubPieceID,
        //                          SubPieceName = s.SubPieceName,
        //                          PieceCount = dt.PieceCount,
        //                          CreatedDate = dt.CreatedDate

        //                      };
        //    return Json(
        //        new
        //        {
        //            data = from obj in altkategori
        //                   select new
        //                   {
        //                       SubPieceId = obj.SubPieceId,
        //                       SubPieceName = obj.SubPieceName,
        //                       PieceCount = obj.PieceCount,
        //                       CreatedDate = obj.CreatedDate
        //                   }
        //        }, JsonRequestBehavior.AllowGet);

        //}

        //public ActionResult AltKategoriTime()
        //{

        //    TakimOmruDBEntities db = new TakimOmruDBEntities();


        //    var altkategori = from s in db.SubPiece
        //                      join dt in db.Detail on s.SubPieceID equals dt.FKSubPieceID
        //                      where s.Type == true
        //                      select new
        //                      {
        //                          SubPieceId = s.SubPieceID,
        //                          SubPieceName = s.SubPieceName,
        //                          ToolLife = s.ToolLife,
        //                          PieceID = s.FKPieceID,
        //                          PieceCount = dt.PieceCount,
        //                          CreatedDate = dt.CreatedDate

        //                      };
        //    return Json(
        //        new
        //        {
        //            data = from obj in altkategori
        //                   select new
        //                   {
        //                       SubPieceId = obj.SubPieceId,
        //                       SubPieceName = obj.SubPieceName,
        //                       ToolLife = obj.ToolLife,
        //                       PieceID = obj.PieceID,
        //                       PieceCount = obj.PieceCount,
        //                       CreatedDate = obj.CreatedDate
        //                   }
        //        }, JsonRequestBehavior.AllowGet);

        //}
        //public ActionResult InsertPiece([FromBody]Piece piece)
        //{

        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri("https://x1mrph0du3.execute-api.eu-west-2.amazonaws.com/Prod/");
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        HttpResponseMessage response = client.PostAsJsonAsync("api/prod/parcaekle", piece).Result;
        //        if (response.IsSuccessStatusCode)
        //        {
        //            return RedirectToAction("ParcaEkle");
        //        }
        //        return null;
        //    }
        //}
        public ActionResult Note()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            ViewBag.UserID = Session["UserID"];

            ViewBag.Company = Session["Company"];
            ViewBag.Logo = Session["Logo"];
            ViewBag.Name = Session["FirstName"];
            int UserId = Convert.ToInt32(Session["UserID"]);
            ProfilModel pm = new ProfilModel();
            pm.users = db.User.Where(x=>x.UserID==UserId).ToList();
            pm.notes = db.Notes.ToList();
            return View(pm);
        }
        // POST: Notes/Delete/5
        [System.Web.Http.HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            TakimOmruDBEntities db = new TakimOmruDBEntities();
            Note note = db.Notes.Find(id);
            db.Notes.Remove(note);
            db.SaveChanges();
            return RedirectToAction("Note");
        }
        public ActionResult ParcaEkle()
        {
            return View();
        }


    }
}
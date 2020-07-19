using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Amazon.Lambda.Serialization.Json;
using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain;
using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApplication16.Domain;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Controllers
{

    [ApiController]
    public class PieceController : Controller
    {
        private readonly TakimOmruDBContext db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IHubContext<SignalRHub> _hubContext;
        public PieceController(TakimOmruDBContext _db, IHttpContextAccessor httpContextAccessor, IHubContext<SignalRHub> hubContext)
        {
            db = _db;
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
        }

        //MobileApi Start
        [Authorize]
        [HttpPost]
        [Route("api/prod/takimekle")]
        public IActionResult TakimEkle([FromBody]Piece piece)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var id = Convert.ToInt32(userId);
            db.Piece.Add(new Piece()
            {
                PieceName = piece.PieceName,
                CreatedDate=piece.CreatedDate.GetValueOrDefault(DateTime.Now),
                FkUserID = id


            });
            db.SaveChanges();
            //_hubContext.Clients.All.SendAsync("takimekle", "");
            
            return Ok("Kayıt Başarıyla eklendi");


        }
        [HttpGet]
        [Route("api/prod/detaylist/{id}")]
        public IActionResult Detay(int? id)
        {




            var altkategori = from dt in db.Detail
                              join s in db.SubPiece on dt.FkSubPieceID equals s.SubPieceID
                              where dt.FkSubPieceID == id
                              select
                                    new
                                    {
                                        DetailID = dt.DetailId,
                                        SubPieceName = s.SubPieceName,
                                        PieceCount = dt.PieceCount,
                                        ToolLife = s.ToolLife,
                                        CreatedDate = dt.CreatedDate.Value.ToString("dd.MM.yyyy hh:mm")

                                    };

            var data = from obj in altkategori
                       select new
                       {
                           DetailId = obj.DetailID,
                           SubPieceName = obj.SubPieceName,
                           ToolLife = obj.ToolLife,
                           PieceCount = obj.PieceCount,
                           CreatedDate = obj.CreatedDate
                       };
            _hubContext.Clients.All.SendAsync("DetayList", data);
            return Json(data);

        }
        [Authorize]
        [HttpPut]
        [Route("api/prod/takimguncelle/{id}")]
        public IActionResult TakimGuncelle(int id, [FromBody]Piece piece)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);
            var entity = db.Piece.FirstOrDefault(e => e.PieceId == id && e.FkUserID == uid);
            entity.PieceName = piece.PieceName;
            entity.CreatedDate = piece.CreatedDate.GetValueOrDefault(DateTime.Now);
            db.SaveChanges();
            return Ok("Başarıyla güncellendi");

        }
        [Authorize]
        [HttpDelete]
        [Route("api/prod/takimsil/{id}")]
        public IActionResult TakimSil(int id)
        {
            if (id <= 0)
                return BadRequest("Geçersiz ID girdiniz.");

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);
            db.Piece.Remove(db.Piece.Where(x => x.FkUserID == uid).FirstOrDefault(c => c.PieceId == id));
            db.SaveChanges();


            return Ok("ID'si " + id.ToString() + " olan takım silindi.");
        }
        [Authorize]
        [HttpGet]
        [Route("api/prod/takimlistele")]
        public IQueryable<TakimModul> TakimList()
        {


            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);


            var takim = db.Piece.FromSql<Piece>("sp_TakimList {0}", uid).Select(x => new TakimModul()
            {
                PieceID = x.PieceId,
                PieceName = x.PieceName,
                CreatedDate=x.CreatedDate,
                FKUserID = x.FkUserID,
                Adet = x.Adet

            });

            _hubContext.Clients.All.SendAsync("TakimList", takim);
            return takim;

        }

        


        //[Authorize]
        //[HttpGet]
        //[Route("api/prod/takim")]
        //public List<PieceModel> Takim()
        //{


        //    var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //    var uid = Convert.ToInt32(userId);
        //    var takim = db.Piece.Where(x => x.FkUserID == uid).Select(x => new PieceModel()
        //    {
        //        PieceID = x.PieceId,
        //        PieceName = x.PieceName,
        //        FKUserID = x.FkUserID
        //    }).ToList();
        //    return takim;
        //}

        //[HttpGet]
        //[Route("api/prod/kategori")]
        //public IActionResult Kategori()
        //{
        //    var adet = from s in db.Piece
        //               join sp in db.SubPiece on s.PieceId equals sp.FkPieceID
        //               where sp.Type == false
        //               select new
        //               {
        //                   PieceId = s.PieceId,
        //                   PieceName = s.PieceName,
        //                   Adet = sp.FkPieceID
        //               };
        //    var data = adet.GroupBy(x => x.PieceId).Select(x => new
        //    {
        //        PieceId = x.First().PieceId,
        //        PieceName = x.First().PieceName,
        //        Adet = x.Count(y => Convert.ToBoolean(y.Adet))
        //    });
        //    return Json(data);

        //}

        [Authorize]
        [HttpGet]
        [Route("api/prod/bildirimler")]
        public IActionResult NotificationListele()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var id = Convert.ToInt32(userId);
            var bildirim = db.Notification.Where(c => c.FkuserId == id).Select(c => new Notification()
            {
                NotificationId = c.NotificationId,
                Notification_Description = c.Notification_Description,
                Notification_Date = c.Notification_Date,
                Count = c.Count,
                FkuserId = c.FkuserId
            });
            _hubContext.Clients.All.SendAsync("BildirimList", bildirim);
            return Json(bildirim);
        }


        [Authorize]
        [HttpDelete]
        [Route("api/prod/bildirimsil/{id}")]
        public IActionResult BildirimSil(int id)
        {
            if (id <= 0)
                return BadRequest("Geçersiz ID girdiniz.");

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);
            db.Notification.Remove(db.Notification.Where(x => x.FkuserId == uid).FirstOrDefault(c => c.NotificationId == id));
            db.SaveChanges();


            return Ok("ID'si " + id.ToString() + " olan bildirim silindi.");
        }



        //Subpiece
        [Authorize]
        [Route("api/prod/parcalistele/{id}")]
        public IActionResult Listele(int? id)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);
            var altkategori = from s in db.SubPiece
                              join dt in db.Detail on s.SubPieceID equals dt.FkSubPieceID
                              where s.FkPieceID == id && s.FkuserID == uid
                              select new
                              {
                                  SubPieceId = s.SubPieceID,
                                  subPieceName = s.SubPieceName,
                                  PieceId = s.FkPieceID,
                                  toolLife = s.ToolLife,
                                  pieceCount = dt.PieceCount,
                                  createdDate = dt.CreatedDate

                              };
            var data = altkategori.GroupBy(x => x.SubPieceId).Select(x => new
            {
                SubPieceId = x.First().SubPieceId,
                SubPieceName = x.First().subPieceName,
                ToolLife = x.First().toolLife,
                PieceID = x.First().PieceId,
                PieceCount = x.Sum(y => y.pieceCount),
                CreatedDate = x.First().createdDate
            }).ToList();
            _hubContext.Clients.All.SendAsync("ParcaList", data);
            return Json(data);
        }

        [Authorize]
        [HttpPost]
        [Route("api/prod/parcaekle")]
        public IActionResult ParcaEkle([FromBody]SubPiece subpiece)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);
            db.SubPiece.Add(new SubPiece()
            {
                SubPieceName = subpiece.SubPieceName,
                ToolLife = subpiece.ToolLife,
                FkuserID = uid,
                FkPieceID = subpiece.FkPieceID,
                Type = subpiece.Type



            });
            db.SaveChanges();
            return Ok("Kayıt Başarıyla eklendi");

        }

        [Authorize]
        [HttpPut]
        [Route("api/prod/parcaguncelle/{id}")]
        public IActionResult ParcaGuncelle(int id, [FromBody]SubPiece subpiece)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);
            var entity = db.SubPiece.FirstOrDefault(e => e.SubPieceID == id && e.FkuserID == uid);
            entity.SubPieceName = subpiece.SubPieceName;
            entity.ToolLife = subpiece.ToolLife;
            entity.FkPieceID = subpiece.FkPieceID;
            entity.Type = subpiece.Type;

            db.SaveChanges();
            return Ok("Başarıyla güncellendi");

        }

        [Authorize]
        [HttpDelete]
        [Route("api/prod/parcasil/{id}")]
        public IActionResult ParcaSil(int id)
        {
            if (id <= 0)
                return BadRequest("Geçersiz ID girdiniz.");

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);
            db.SubPiece.Remove(db.SubPiece.Where(x => x.FkuserID == uid).FirstOrDefault(c => c.SubPieceID == id));
            db.SaveChanges();


            return Ok("ID'si " + id.ToString() + " olan parça silindi.");
        }

        [Authorize]
        [HttpGet]
        [Route("api/prod/parcakirmizi/{id}")]
        public IActionResult Kirmizi(int id)
        {


            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);

            var altkategori = from s in db.SubPiece
                              join dt in db.Detail on s.SubPieceID equals dt.FkSubPieceID
                              where s.FkPieceID == id && s.Type == false && s.FkuserID == uid
                              select new
                              {
                                  SubPieceId = s.SubPieceID,
                                  SubPieceName = s.SubPieceName,
                                  ToolLife = s.ToolLife,
                                  PieceID = s.FkPieceID,
                                  PieceCount = dt.PieceCount,
                                  CreatedDate = dt.CreatedDate.Value.ToString("dd.MM.yyyy hh:mm")

                              };
            var data = altkategori.GroupBy(x => x.SubPieceId).Select(x => new
            {
                SubPieceId = x.First().SubPieceId,
                SubPieceName = x.First().SubPieceName,
                ToolLife = x.First().ToolLife,
                PieceID = x.First().PieceID,
                PieceCount = x.Sum(y => y.PieceCount),
                CreatedDate = x.First().CreatedDate
            }).Where(x => (x.PieceCount) > (x.ToolLife * 2) / 3);

            _hubContext.Clients.All.SendAsync("KirmiziParca", data);

            return Json(data);

        }


        [Authorize]
        [HttpGet]
        [Route("api/prod/parcadurum/{id}")]
        public IActionResult Durum(int id)
        {

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);


            var Iyii = 0;
            var normall = 0;
            var kotu = 0;
            var altkategori = from sp in db.SubPiece
                               join dt in db.Detail on sp.SubPieceID equals dt.FkSubPieceID
                              where sp.FkPieceID == id && sp.FkuserID == uid
                              select new 
                              {
                                  SubPieceId = sp.SubPieceID,
                                  ToolLife = sp.ToolLife,
                                  PieceId = sp.FkPieceID,
                                  PieceCount = dt.PieceCount,
                              };
            var yesil = altkategori.GroupBy(x => x.SubPieceId).Select(x => new 
            {
                SubPieceId = x.First().SubPieceId,
                ToolLife = x.First().ToolLife,
                PieceId = x.First().PieceId,
                PieceCount = x.Sum(y => y.PieceCount),



            }).Where(x => x.PieceCount <= (x.ToolLife * 1) / 3).ToList();

            var Sari = altkategori.GroupBy(x => x.SubPieceId).Select(x => new
            {
                SubPieceId = x.First().SubPieceId,
                ToolLife = x.First().ToolLife,
                PieceId = x.First().PieceId,
                PieceCount = x.Sum(y => y.PieceCount),



            }).Where(x => x.PieceCount > ((x.ToolLife * 1) / 3)&& x.PieceCount <= ((x.ToolLife * 2) / 3)).ToList();

            var Kirmizi = altkategori.GroupBy(x => x.SubPieceId).Select(x => new
            {
                SubPieceId = x.First().SubPieceId,
                ToolLife = x.First().ToolLife,
                PieceId = x.First().PieceId,
                PieceCount = x.Sum(y => y.PieceCount),



            }).Where(x => x.PieceCount > ((x.ToolLife * 2) / 3) && x.PieceCount <= x.ToolLife).ToList();

            foreach (var item3 in Kirmizi)
            {
                kotu = Kirmizi.Count();
            }

            foreach (var item2 in Sari)
            {
                normall = Sari.Count();
            }

            foreach (var item in yesil)
            {
                
                    Iyii = yesil.Count();
                
            }

            var liste = db.Piece.Where(x => x.PieceId == id).Select(x => new ParcaDurumModel()
            {
                PieceId = id,
                Iyi = Iyii,
                Normal = normall,
                Kotu = kotu
            }).ToList();
            var bak = Iyii + " " + normall + " " + kotu;
            _hubContext.Clients.All.SendAsync("ParcaDurum", liste);

            return Json(liste);

        }


        [Authorize]
        [HttpGet]
        [Route("api/prod/parcasari/{id}")]
        public IActionResult Sari(int id)
        {


            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);

            var altkategori = from s in db.SubPiece
                              join dt in db.Detail on s.SubPieceID equals dt.FkSubPieceID
                              where s.FkPieceID == id && s.Type == false && s.FkuserID == uid
                              select new
                              {
                                  SubPieceId = s.SubPieceID,
                                  SubPieceName = s.SubPieceName,
                                  ToolLife = s.ToolLife,
                                  PieceID = s.FkPieceID,
                                  PieceCount = dt.PieceCount,
                                  CreatedDate = dt.CreatedDate.Value.ToString("dd.MM.yyyy hh:mm")

                              };
            var data = altkategori.GroupBy(x => x.SubPieceId).Select(x => new
            {
                SubPieceId = x.First().SubPieceId,
                SubPieceName = x.First().SubPieceName,
                ToolLife = x.First().ToolLife,
                PieceID = x.First().PieceID,
                PieceCount = x.Sum(y => y.PieceCount),
                CreatedDate = x.First().CreatedDate
            }).Where(x => (x.PieceCount) > ((x.ToolLife * 1) / 3) && (x.PieceCount) < ((x.ToolLife * 2) / 3));

            _hubContext.Clients.All.SendAsync("SariParca", data);

            return Json(data);

        }

        [Authorize]
        [HttpGet]
        [Route("api/prod/parcayesil/{id}")]
        public IActionResult Yesil(int id)
        {


            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);

            var altkategori = from s in db.SubPiece
                              join dt in db.Detail on s.SubPieceID equals dt.FkSubPieceID
                              where s.FkPieceID == id && s.Type == false && s.FkuserID == uid
                              select new
                              {
                                  SubPieceId = s.SubPieceID,
                                  SubPieceName = s.SubPieceName,
                                  ToolLife = s.ToolLife,
                                  PieceID = s.FkPieceID,
                                  PieceCount = dt.PieceCount,
                                  CreatedDate = dt.CreatedDate.Value.ToString("dd.MM.yyyy hh:mm")

                              };
            var data = altkategori.GroupBy(x => x.SubPieceId).Select(x => new
            {
                SubPieceId = x.First().SubPieceId,
                SubPieceName = x.First().SubPieceName,
                ToolLife = x.First().ToolLife,
                PieceID = x.First().PieceID,
                PieceCount = x.Sum(y => y.PieceCount),
                CreatedDate = x.First().CreatedDate
            }).Where(x => (x.PieceCount) <= (x.ToolLife * 1) / 3);

            _hubContext.Clients.All.SendAsync("YesilParca", data);

            return Json(data);

        }

        [HttpGet]
        [Route("api/prod/hesap")]
        public List<UserModel> Hesap()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);
            var list = db.User.Where(x => x.UserId == uid).Select(x => new UserModel()
            {
                UserId = x.UserId,
                UserName = x.UserName,
                Password = x.Password,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Company = x.Company,
                CompanyLogo = x.CompanyLogo,
                Email = x.Email
            }).ToList();
            _hubContext.Clients.All.SendAsync("Hesap", list);
            return list;

        }

        [Authorize]
        [HttpPut]
        [Route("api/prod/hesapguncelle")]
        public IActionResult HesapGuncelle(User user)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);
            var entity = db.User.FirstOrDefault(e=> e.UserId == uid);
            if (user.FirstName == null)
            {
                user.FirstName = entity.FirstName;
            }
            if (user.LastName == null)
            {
                user.LastName = entity.LastName;
            }
            if (user.Email == null)
            {
                user.Email = entity.Email;
            }
            if (user.Company == null)
            {
                user.Company = entity.Company;
            }
            if (user.UserName == null)
            {
                user.UserName = entity.UserName;
            }
            entity.FirstName = user.FirstName;
            entity.LastName = user.LastName;
            entity.Email = user.Email;
            entity.Company = user.Company;
            entity.UserName = user.UserName;
            db.SaveChanges();
            return Ok("Kullanıcı bilgileri başarıyla güncellendi.");

        }

        [Authorize]
        [HttpPut]
        [Route("api/prod/sifreguncelle")]
        public IActionResult SifreGuncelle(User user)
        {

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);
            var entity = db.User.FirstOrDefault(e => e.UserId == uid);
            if (user.Password=="")
            {
                return BadRequest("Şifre alanı boş bırakılamaz");
            }
            entity.Password = user.Password;
            db.SaveChanges();
            return Ok("Şifre başarıyla güncellendi.");

        }


        [HttpPost]
        [Route("api/prod/adduser")]
        public IActionResult AddUser(User user)
        {

            db.User.Add(user);
            db.SaveChanges();
            return Ok("Kayıt Başarıyla eklendi");

        }



        //MobileApi Start
        [Authorize]
        [HttpPost]
        [Route("api/prod/notekle")]
        public IActionResult NotEkle(string PieceName, string SubPieceName, [FromBody]Note note)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var id = Convert.ToInt32(userId);
            db.Note.Add(new Note()
            {
                NoteHeader = note.NoteHeader,
                NoteDescription = note.NoteDescription,
                CreatedDate = note.CreatedDate.GetValueOrDefault(DateTime.Now),
                FkuserId = id,
                FkpieceId = note.FkpieceId,
                FksubpieceId = note.FksubpieceId,
                Type = note.Type
            });
            db.SaveChanges();
            //_hubContext.Clients.All.SendAsync("takimekle", "");

            return Ok("Kayıt Başarıyla eklendi");
        }



        [Authorize]
        [HttpPut]
        [Route("api/prod/notguncelle/{id}")]
        public IActionResult NotGuncelle(int id, [FromBody]Note note)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var fkPieceId = Convert.ToInt32(db.Piece.Where(x => x.PieceId == note.FkpieceId));
            //var fkSubPieceId = Convert.ToInt32(db.SubPiece.Where(x => x.SubPieceID == note.FksubpieceId));
            var uid = Convert.ToInt32(userId);
            var entity = db.Note.FirstOrDefault(e => e.NoteId == id && e.FkuserId == uid);
            entity.NoteHeader = note.NoteHeader;
            entity.NoteDescription = note.NoteDescription;
            entity.CreatedDate = note.CreatedDate.GetValueOrDefault(DateTime.Now);
            entity.FkuserId = id;
            entity.FkpieceId = note.FkpieceId;
            entity.FksubpieceId = note.FksubpieceId;
            entity.Type = note.Type;
            db.SaveChanges();
            return Ok("Başarıyla güncellendi");

        }


        [Authorize]
        [HttpDelete]
        [Route("api/prod/notsil/{id}")]
        public IActionResult NotSil(int id)
        {
            if (id <= 0)
                return BadRequest("Geçersiz ID girdiniz.");

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var fkPieceId = Convert.ToInt32(db.Piece.Where(x => x.PieceId == note.FkpieceId));
            //var fkSubPieceId = Convert.ToInt32(db.SubPiece.Where(x => x.SubPieceID == note.FksubpieceId));
            var uid = Convert.ToInt32(userId);
            db.Note.Remove(db.Note.Where(x => x.FkuserId == uid).FirstOrDefault(c => c.NoteId == id));
            db.SaveChanges();


            return Ok("ID'si " + id.ToString() + " olan not silindi.");
        }


        [Authorize]
        [HttpGet]
        [Route("api/prod/notlistele")]
        public IQueryable<NoteModel> NotList()
        {


            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);
            var takim = db.Note.Where(x => x.FkuserId == uid).Include(x => x.Fkpiece.PieceName).Include(x => x.Fksubpiece.SubPieceName).Select(x => new NoteModel()
            {
                NoteId = x.NoteId,
                NoteHeader = x.NoteHeader,
                NoteDescription = x.NoteDescription,
                CreatedDate = x.CreatedDate,
                FkuserId = uid,
                FkpieceId = x.FkpieceId,
                pieceName = x.Fkpiece.PieceName,
                subPieceName = x.Fksubpiece.SubPieceName,
                FksubpieceId = x.FksubpieceId,
                Type = x.Type,



            });



            return takim;
        }


        //MobileApiEnd

    }
}
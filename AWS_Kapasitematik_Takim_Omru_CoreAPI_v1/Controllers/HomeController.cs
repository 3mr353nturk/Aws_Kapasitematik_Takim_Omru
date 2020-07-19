using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Json.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ThirdParty.Json.LitJson;
using WebApplication16.Domain;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Controllers
{

    [ApiController]
    public class HomeController : Controller
    {

        private readonly TakimOmruDBContext db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(TakimOmruDBContext _db, IHttpContextAccessor httpContextAccessor)
        {

            db = _db;
            _httpContextAccessor = httpContextAccessor;
        }
        

        //[HttpGet]
        //[Route("api/prod/savepiece")]
        //public async Task<IActionResult> SavePiece(Piece piece)
        //{
        //    await service.SavePiece(piece);
        //    IQueryable<Piece> pieceList = service.GetList();
        //    return Ok(pieceList);
        //}



        [HttpGet]
        [Route("api/prod/kategori")]
        public IActionResult Kategori(int FkUserId)
        {
            var adet = from s in db.Piece
                       join sp in db.SubPiece on s.PieceId equals sp.FkPieceID
                       where sp.Type == false && s.FkUserID == FkUserId
                       select new
                       {
                           PieceId = s.PieceId,
                           PieceName = s.PieceName,
                           CreatedDate = s.CreatedDate,
                           Adet = sp.FkPieceID
                       };
            var data = adet.GroupBy(x => x.PieceId).Select(x => new
            {
                PieceId = x.First().PieceId,
                PieceName = x.First().PieceName,
                CreatedDate = x.First().CreatedDate,
                Adet = x.Count(y => Convert.ToBoolean(y.Adet))
            });
            return Json(data);

        }

        [HttpGet]
        [Route("api/prod/kategoritime")]
        public IActionResult KategoriTime(int FkUserId)
        {
            var adet = from s in db.Piece
                       join sp in db.SubPiece on s.PieceId equals sp.FkPieceID
                       where sp.Type == true && s.FkUserID == FkUserId
                       select new
                       {
                           PieceId = s.PieceId,
                           PieceName = s.PieceName,
                           CreatedDate = s.CreatedDate,
                           Adet = sp.FkPieceID
                       };
            var data = adet.GroupBy(x => x.PieceId).Select(x => new
            {
                PieceId = x.First().PieceId,
                PieceName = x.First().PieceName,
                CreatedDate = x.First().CreatedDate,
                Adet = x.Count(y => Convert.ToBoolean(y.Adet))
            });
            return Json(data);

        }

        [HttpGet]
        [Route("api/prod/altkategori")]
        public IActionResult AltKategori(int PieceId)
        {




            var altkategori = from s in db.SubPiece
                              join dt in db.Detail on s.SubPieceID equals dt.FkSubPieceID
                              where s.FkPieceID == PieceId && s.Type == false
                              select new
                              {
                                  SubPieceId = s.SubPieceID,
                                  SubPieceName = s.SubPieceName,
                                  ToolLife = s.ToolLife,
                                  PieceID = s.FkPieceID,
                                  PieceCount = dt.PieceCount,
                                  CreatedDate = dt.CreatedDate,
                                  Kalan = 0,
                                  SubPiece = s.SubPieceID

                              };
            var data = altkategori.GroupBy(x => x.SubPieceId).Select(x => new
            {
                SubPieceId = x.First().SubPieceId,
                SubPieceName = x.First().SubPieceName,
                ToolLife = x.First().ToolLife,
                PieceID = x.First().PieceID,
                PieceCount = x.Sum(y => y.PieceCount),
                CreatedDate = x.First().CreatedDate,
                Subpiece = x.First().SubPieceId,
                Kalan = x.First().ToolLife - x.Sum(y => y.PieceCount)
            });

            return Json(data);

        }


        [HttpGet]
        [Route("api/prod/altkategoriprogress")]
        public IActionResult AltKategoriProgress(int PieceId)
        {




            var altkategori = from s in db.SubPiece
                              join dt in db.Detail on s.SubPieceID equals dt.FkSubPieceID
                              where s.FkPieceID == PieceId
                              select new
                              {
                                  SubPieceId = s.SubPieceID,
                                  SubPieceName = s.SubPieceName,
                                  ToolLife = s.ToolLife,
                                  PieceID = s.FkPieceID,
                                  PieceCount = dt.PieceCount,
                                  CreatedDate = dt.CreatedDate

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



            return Json(data);

        }


        [HttpGet]
        [Route("api/prod/detayy")]
        public IActionResult Detayy(int SubpieceId)
        {




            var altkategori = from dt in db.Detail
                              join s in db.SubPiece on dt.FkSubPieceID equals s.SubPieceID
                              where dt.FkSubPieceID == SubpieceId
                              select new
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

            return Json(data);

        }

        //[HttpGet]
        //[Route("api/prod/altkategoricount/{id}")]
        //public IActionResult Count(int id)
        //{
        //    var sub = db.SubPiece.ToList();
        //    var sum = from s in db.SubPiece join dt in db.Detail on s.SubPieceID equals dt.FkSubPieceID into


        //    var altkategori = from s in db.SubPiece
        //                      join dt in db.Detail on s.SubPieceID equals dt.FkSubPieceID
        //                      where s.SubPieceID == id && s.Type == false
        //                      select new JoinModel
        //                      {
        //                          SubPieceId = s.SubPieceID,
        //                          subPieceName = s.SubPieceName,
        //                          toolLife = s.ToolLife,
        //                          pieceCount = sum,
        //                          //createdDate = dt.CreatedDate.Value.ToString("dd.MM.yyyy hh:mm")

        //                      };

        //    var data = from obj in altkategori
        //               select new
        //               {
        //                   SubPieceId = obj.SubPieceId,
        //                   SubPieceName = obj.subPieceName,
        //                   ToolLife = obj.toolLife,

        //                   PieceCount = obj.pieceCount,

        //               };

        //    return Json(data);
        //}


        [HttpGet]
        [Route("api/prod/altkategoritime")]
        public IActionResult AltKategoriTime(int PieceId)
        {




            var altkategori = from s in db.SubPiece
                              join dt in db.Detail on s.SubPieceID equals dt.FkSubPieceID
                              where s.FkPieceID == PieceId && s.Type == true
                              select new
                              {
                                  SubPieceId = s.SubPieceID,
                                  SubPieceName = s.SubPieceName,
                                  ToolLife = s.ToolLife,
                                  PieceID = s.FkPieceID,
                                  PieceCount = dt.PieceCount,
                                  CreatedDate = dt.CreatedDate,
                                  Kalan = 0,
                                  SubPiece = s.SubPieceID

                              };

            var data = altkategori.GroupBy(x => x.SubPieceId).Select(x => new
            {
                SubPieceId = x.First().SubPieceId,
                SubPieceName = x.First().SubPieceName,
                ToolLife = x.First().ToolLife,
                PieceID = x.First().PieceID,
                PieceCount = x.Sum(y => y.PieceCount),
                CreatedDate = x.First().CreatedDate,
                Subpiece = x.First().SubPieceId,
                Kalan = x.First().ToolLife - x.Sum(y => y.PieceCount)
            });

            return Json(data);


        }

        //[HttpGet]
        //[Route("api/prod/kategori")]
        //[Produces("application/json")]
        //public ActionResult Kategori()
        //{

        //    var Kategorii = db.Piece.ToList();
        //    return Json(
        //    new
        //    {
        //        data = from obj in Kategorii
        //               select new
        //               {
        //                   Piece = obj.PieceId,
        //                   PieceName = obj.PieceName.ToString(),

        //               }
        //    });
        //}


        ////[HttpGet]
        ////[Route("api/prod/altkategori")]
        ////[Produces("application/json")]
        ////public IActionResult AltKategori()
        ////{

        ////    var Kategorii = db.SubPiece.ToList();
        ////    return Json(
        ////    new
        ////    {
        ////        data = from obj in Kategorii
        ////               select new
        ////               {
        ////                   SubPieceId = obj.SubPieceID,
        ////                   SubPieceName = obj.SubPieceName.ToString(),
        ////                   Count = obj.,
        ////                   TotalCount = obj.TotalCount,
        ////                   Date = obj.TodayDate.Value.ToString("dd.MM.yyyy"),
        ////                   PieceID = obj.PieceId
        ////               }
        ////    });

        ////}
        //[HttpGet]
        //[Route("api/prod/altkategorilist/{id}")]
        //public List<SubPiece> AltkategoriList(int? id)
        //{

        //    return db.SubPiece.Where(c => c.FkPieceID == id).ToList();
        //}
        [HttpPost]
        [Route("api/prod/parcaeklee")]
        public IActionResult ParcaEkle(Piece piece)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var id = Convert.ToInt32(userId);
            ActionResult result = null;

            var isAlreadyExists = db.Piece.Where(x => x.PieceName.Equals(piece.PieceName, StringComparison.InvariantCultureIgnoreCase)).Any();

            if (isAlreadyExists)
            {
                result = NotFound("Aynı isimde takım daha önceden kaydedilmiş...");
            }
            else
            {
                db.Piece.Add(new Piece()
                {
                    PieceName = piece.PieceName,
                    FkUserID=id,
                    CreatedDate=piece.CreatedDate.GetValueOrDefault(DateTime.Now)
                });
                db.SaveChanges();
                result = Ok("Kayıt Başarıyla eklendi");

            }

            return result;

        }


        [HttpPost]
        [Route("api/prod/parcaedit")]
        public IActionResult ParcaGuncelle(Piece piece)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var id = Convert.ToInt32(userId);
            if (ModelState.IsValid)
            {
                piece.FkUserID = id;
                db.Entry(piece).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Ok("Kayıt Başarıyla eklendi");
        }

        [HttpPost]
        [Route("api/prod/parcadelete")]
        public IActionResult ParcaSil(int id)
        {
            Piece piece = db.Piece.Find(id);
            db.Piece.Remove(piece);
            db.SaveChanges();
            return Ok("Kayıt Başarıyla eklendi");
        }


        //[HttpDelete]
        //[Route("api/prod/parcasil/{id}")]
        //public IActionResult ParcaSil(int id)
        //{
        //    if (id <= 0)
        //        return BadRequest("Geçersiz ID girdiniz.");

        //    db.Piece.Remove(db.Piece.FirstOrDefault(c => c.PieceId == id));
        //    db.SaveChanges();


        //    return Ok("ID'si " + id.ToString() + " olan notification silindi.");
        //}

        //[HttpPut]
        //[Route("api/prod/parcaguncelle/{id}")]
        //public IActionResult ParcaGuncelle(int id, [FromBody]Piece piece)
        //{

        //    var entity = db.Piece.FirstOrDefault(e => e.PieceId == id);
        //    entity.PieceName = piece.PieceName;
        //    db.SaveChanges();
        //    return Ok("Başarıyla güncellendi");
        //    //if (ModelState.IsValid)
        //    //{
        //    //    db.Entry(piece).State = EntityState.Modified;
        //    //    db.SaveChanges();
        //    //    return Ok("Kayıt Başarıyla Güncellendi");
        //    //}
        //    //return View(piece);
        //}

        //[HttpDelete]
        //[Route("api/prod/altparcasil/{id}")]
        //public IActionResult AltParcaSil(int id)
        //{
        //    if (id <= 0)
        //        return BadRequest("Geçersiz ID girdiniz.");

        //    db.SubPiece.Remove(db.SubPiece.FirstOrDefault(c => c.SubPieceID == id));
        //    db.SaveChanges();


        //    return Ok("ID'si " + id.ToString() + " olan alt parça silindi.");
        //}

        ////[HttpPut]
        ////[Route("api/prod/altparcaguncelle/{id}")]
        ////public IActionResult AltParcaGuncelle(int id, [FromBody]SubPiece subpiece)
        ////{

        ////    var entity = db.SubPiece.FirstOrDefault(e => e.SubPieceID == id);
        ////    entity.SubPieceName = subpiece.SubPieceName;
        ////    entity = subpiece.Count;
        ////    entity.TodayDate = subpiece.TodayDate;
        ////    entity.TotalCount = subpiece.TotalCount;
        ////    entity.PieceId = subpiece.PieceId;
        ////    db.SaveChanges();
        ////    return Ok("Alt parça Başarıyla güncellendi");
        ////}
        [HttpPost]
        [Route("api/prod/altparcaeklee")]
        public IActionResult Create([FromBody]SubPiece subPiece)
        {
            ActionResult result = null;

            var isAlreadyExists = db.SubPiece.Where(x => x.SubPieceName.Equals(subPiece.SubPieceName, StringComparison.InvariantCultureIgnoreCase)).Any();

            if (isAlreadyExists)
            {
                result = NotFound("Aynı isimde parçadan daha önceden kaydedilmiş...");
            }
            else
            {

                if (ModelState.IsValid)
                {
                    db.SubPiece.Add(subPiece);
                    db.SaveChanges();
                    result = Ok("Kayıt Başarıyla eklendi");
                }
            }


            return result;


            //}

        }
        [HttpGet]
        [Route("api/prod/piecelist")]
        public List<Piece> TakimListele()
        {
            return db.Piece.ToList();
        }





        [HttpGet]
        [Route("api/prod/detay/{id}")]
        public List<Detail> Detay(int id)
        {
            return db.Detail.Where(c => c.FkSubPieceID == id).ToList();
        }


        [Authorize]
        [HttpGet]
        [Route("api/prod/report")]
        public HttpResponseMessage Get()
        {
            string responseString = @" 
@model Kapasitematik_TakimOmru_v3.Models.ThreeModel

<!--hamburger menü-->
<style>
    .red-tooltip + .tooltip > .tooltip-inner {
        background-color: #f00;
        color: white;
    }
    .editor-field textarea
    {
        width: 400px;
        height: 110px;
    }
    .flaticon2-bell-alarm-symbol.badge {
        position: absolute;
        top: -10px;
        right: -10px;
        padding: 3px 6px;
        border-radius: 50%;
        background: red;
        color: white;
        font-size: 11px;
    }
</style>
<div id = 'kt_header_mobile' class='kt-header-mobile  kt-header-mobile--fixed '>
    <div class='kt-header-mobile__logo'>
        <a href = 'index.html' >
        </a>
    </div>
    <div class='kt-header-mobile__toolbar'>
        <button class='kt-header-mobile__toggler kt-header-mobile__toggler--left' id='kt_aside_mobile_toggler'><span></span></button>
        <button class='kt-header-mobile__toggler' id='kt_header_mobile_toggler'><span></span></button>
        <button class='kt-header-mobile__topbar-toggler' id='kt_header_mobile_topbar_toggler'><i class='flaticon-more'></i></button>
    </div>
</div>
<!-- end::Header Mobile -->
<div class='kt-grid kt-grid--hor kt-grid--root'>
    <div class='kt-grid__item kt-grid__item--fluid kt-grid kt-grid--ver kt-page'>
        <!-- begin::Aside -->
        <!-- Uncomment this to display the close button of the panel
        <button class='kt-aside-close ' id='kt_aside_close_btn'><i class='la la-close'></i></button>
        -->
        <div class='kt-aside  kt-aside--fixed  kt-grid__item kt-grid kt-grid--desktop kt-grid--hor-desktop' id='kt_aside'>
            <!-- begin::Aside -->
            <div class='kt-aside__brand kt-grid__item ' id='kt_aside_brand'>
                <div class='kt-aside__brand-logo'>
                </div>
                <div class='kt-aside__brand-tools'>
                    <button class='kt-aside__brand-aside-toggler' id='kt_aside_toggler'>
                        <span>
                            <svg xmlns = 'http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' width='24px' height='24px' viewBox='0 0 24 24' version='1.1' class='kt-svg-icon'>
                                <g stroke = 'none' stroke-width='1' fill='none' fill-rule='evenodd'>
                                    <polygon points = '0 0 24 0 24 24 0 24' />
                                    < path d='M5.29288961,6.70710318 C4.90236532,6.31657888 4.90236532,5.68341391 5.29288961,5.29288961 C5.68341391,4.90236532 6.31657888,4.90236532 6.70710318,5.29288961 L12.7071032,11.2928896 C13.0856821,11.6714686 13.0989277,12.281055 12.7371505,12.675721 L7.23715054,18.675721 C6.86395813,19.08284 6.23139076,19.1103429 5.82427177,18.7371505 C5.41715278,18.3639581 5.38964985,17.7313908 5.76284226,17.3242718 L10.6158586,12.0300721 L5.29288961,6.70710318 Z' fill='#000000' fill-rule='nonzero' transform='translate(8.999997, 11.999999) scale(-1, 1) translate(-8.999997, -11.999999) ' />
                                    <path d = 'M10.7071009,15.7071068 C10.3165766,16.0976311 9.68341162,16.0976311 9.29288733,15.7071068 C8.90236304,15.3165825 8.90236304,14.6834175 9.29288733,14.2928932 L15.2928873,8.29289322 C15.6714663,7.91431428 16.2810527,7.90106866 16.6757187,8.26284586 L22.6757187,13.7628459 C23.0828377,14.1360383 23.1103407,14.7686056 22.7371482,15.1757246 C22.3639558,15.5828436 21.7313885,15.6103465 21.3242695,15.2371541 L16.0300699,10.3841378 L10.7071009,15.7071068 Z' fill='#000000' fill-rule='nonzero' opacity='0.3' transform='translate(15.999997, 11.999999) scale(-1, 1) rotate(-270.000000) translate(-15.999997, -11.999999) ' />
                                </g>
                            </svg>
                        </span>
                        <span>
                            <svg xmlns = 'http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' width='24px' height='24px' viewBox='0 0 24 24' version='1.1' class='kt-svg-icon'>
                                <g stroke = 'none' stroke-width='1' fill='none' fill-rule='evenodd'>
                                    <polygon points = '0 0 24 0 24 24 0 24' />
                                    < path d='M12.2928955,6.70710318 C11.9023712,6.31657888 11.9023712,5.68341391 12.2928955,5.29288961 C12.6834198,4.90236532 13.3165848,4.90236532 13.7071091,5.29288961 L19.7071091,11.2928896 C20.085688,11.6714686 20.0989336,12.281055 19.7371564,12.675721 L14.2371564,18.675721 C13.863964,19.08284 13.2313966,19.1103429 12.8242777,18.7371505 C12.4171587,18.3639581 12.3896557,17.7313908 12.7628481,17.3242718 L17.6158645,12.0300721 L12.2928955,6.70710318 Z' fill='#000000' fill-rule='nonzero' />
                                    <path d = 'M3.70710678,15.7071068 C3.31658249,16.0976311 2.68341751,16.0976311 2.29289322,15.7071068 C1.90236893,15.3165825 1.90236893,14.6834175 2.29289322,14.2928932 L8.29289322,8.29289322 C8.67147216,7.91431428 9.28105859,7.90106866 9.67572463,8.26284586 L15.6757246,13.7628459 C16.0828436,14.1360383 16.1103465,14.7686056 15.7371541,15.1757246 C15.3639617,15.5828436 14.7313944,15.6103465 14.3242754,15.2371541 L9.03007575,10.3841378 L3.70710678,15.7071068 Z' fill='#000000' fill-rule='nonzero' opacity='0.3' transform='translate(9.000003, 11.999999) rotate(-270.000000) translate(-9.000003, -11.999999) ' />
                                </g>
                            </svg>
                        </span>
                    </button>
                    <!--
                    <button class='kt-aside__brand-aside-toggler kt-aside__brand-aside-toggler--left' id='kt_aside_toggler'><span></span></button>
                    -->
                </div>
            </div>
            <!-- end::Aside -->
            <!-- begin::Aside Menu -->
            <div class='kt-aside-menu-wrapper kt-grid__item kt-grid__item--fluid' id='kt_aside_menu_wrapper'>
                <div id = 'kt_aside_menu' class='kt-aside-menu ' data-ktmenu-vertical='1' data-ktmenu-scroll='1' data-ktmenu-dropdown-timeout='500'>
                    <ul class='kt-menu__nav '>
                        <li class='kt-menu__item ' aria-haspopup='true'>
                            <a href = '@Url.Action('TakimOmru', 'Home')' class='kt-menu__link '>
                                <span class='kt-menu__link-icon'>
                                    <img src = 'https://takimomrubucketyeni.s3.eu-west-2.amazonaws.com/Docs/icon-reward.png' style='width:40px; height:40px;' />
                                </span><span class='kt-menu__link-text'>Takım Ömrü</span>
                            </a>
                        </li>
                        <li class='kt-menu__section '>
                            <h4 class='kt-menu__section-text'>Ayarlar</h4>
                            <i class='kt-menu__section-icon flaticon-more-v2'></i>
                        </li>
                        <li class='kt-menu__item  kt-menu__item--submenu' aria-haspopup='true' data-ktmenu-submenu-toggle='hover'>
                            <a href = '' class='kt-menu__link kt-menu__toggle'>
                                <span class='kt-menu__link-icon'>
                                    <img src = 'https://takimomrubucketyeni.s3.eu-west-2.amazonaws.com/Docs/icon-reward.png' style='width:40px; height:40px;' />
                                </span><span class='kt-menu__link-text'>Takım</span>
                            </a>
                            <div class='kt-menu__submenu '>
                                <span class='kt-menu__arrow'></span>
                                <ul class='kt-menu__subnav'>
                                    <li class='kt-menu__item ' aria-haspopup='true'><a href = '@Url.Action('Indexx', 'Piece')' class='kt-menu__link '><i class='kt-menu__link-bullet kt-menu__link-bullet--dot'><span></span></i><span class='kt-menu__link-text'>Takım Ayarları</span></a></li>
                                </ul>
                            </div>
                        </li>
                        <li class='kt-menu__item  kt-menu__item--submenu' aria-haspopup='true' data-ktmenu-submenu-toggle='hover'>
                            <a href = '' class='kt-menu__link kt-menu__toggle'>
                                <span class='kt-menu__link-icon'>
                                    <img src = 'https://takimomrubucketyeni.s3.eu-west-2.amazonaws.com/Docs/icon-reward.png' style='width:40px; height:40px;' />
                                </span><span class='kt-menu__link-text'>Parça</span><i class='kt-menu__ver-arrow la la-angle-right'></i>
                            </a>
                            <div class='kt-menu__submenu '>
                                <span class='kt-menu__arrow'></span>
                                <ul class='kt-menu__subnav'>
                                    <li class='kt-menu__item ' aria-haspopup='true'><a href = '@Url.Action('Index', 'SubPieces')' class='kt-menu__link '><i class='kt-menu__link-bullet kt-menu__link-bullet--dot'><span></span></i><span class='kt-menu__link-text'>Parça Ayarları</span></a></li>
                                </ul>
                            </div>
                            <div class='kt-menu__submenu '>
                                <span class='kt-menu__arrow'></span>
                                <ul class='kt-menu__subnav'>
                                    <li class='kt-menu__item ' aria-haspopup='true'><a href = '@Url.Action('Index', 'Detail')' class='kt-menu__link '><i class='kt-menu__link-bullet kt-menu__link-bullet--dot'><span></span></i><span class='kt-menu__link-text'>Eklenen Parçalar</span></a></li>
                                </ul>
                            </div>
                        </li>
                    </ul>
                </div>
            </div>
            <!-- end::Aside Menu -->
        </div>
        <!-- end::Aside -->
        <div class='kt-grid__item kt-grid__item--fluid kt-grid kt-grid--hor kt-wrapper' id='kt_wrapper'>
            <div id = 'kt_header' class='kt-header kt-grid__item  kt-header--fixed '>
                <!-- begin::Header Menu -->
                <!-- Uncomment this to display the close button of the panel
                <button class='kt-header-menu-wrapper-close' id='kt_header_menu_mobile_close_btn'><i class='la la-close'></i></button>
                -->
                <div class='kt-header-menu-wrapper' id='kt_header_menu_wrapper'>
                    <div id = 'kt_header_menu' class='kt-header-menu kt-header-menu-mobile  kt-header-menu--layout-default '>
                        <ul class='kt-menu__nav '>
                        </ul>
                    </div>
                </div>
                <!-- end::Header Menu -->
                <!-- begin::Header Topbar -->
                <div class='kt-header__topbar'>
                    <!--begin: Quick panel toggler -->
                    <div class='kt-header__topbar-item kt-header__topbar-item--quick-panel' data-placement='right'>
                        <span class='kt-header__topbar-icon' id='kt_quick_panel_toggler_btn'>
                            <span style = 'font-size:15px; margin-right:15px;' class='flaticon2-bell-alarm-symbol btn btn-icon btn-brand btn-sm kt-pulse'>
                                @foreach(var item in Model.notification)
{
                                    < span class='badge'>@Html.DisplayFor(modelItem => item.Count)</span>
                                }
                            </span>
                        </span>
                    </div>
                    <!--end: Quick panel toggler -->
                    <!--begin: User Bar -->
                    <div class='kt-header__topbar-item kt-header__topbar-item--user'>
                        <div class='kt-header__topbar-wrapper' data-toggle='dropdown' data-offset='0px,0px'>
                            <div class='kt-header__topbar-user'>
                                <span class='kt-header__topbar-welcome kt-hidden-mobile'></span>
                                <span class='kt-header__topbar-username kt-hidden-mobile'>@ViewBag.Name</span>
                                <img class='kt-hidden' alt='Pic' src='assets/media/users/300_25.jpg' />
                                <!--use below badge element instead the user avatar to display username's first letter(remove kt-hidden class to display it) -->
                            </div>
                        </div>
                        <div class='dropdown-menu dropdown-menu-fit dropdown-menu-right dropdown-menu-anim dropdown-menu-top-unround dropdown-menu-xl'>
                            <!--begin: Head -->
                            <div class='kt-user-card kt-user-card--skin-dark kt-notification-item-padding-x' style='background-color:#1e1e2d'>
                                <div class='kt-user-card__avatar'>
                                    <!--use below badge element instead the user avatar to display username's first letter(remove kt-hidden class to display it) -->
                                    <span class='kt-badge kt-badge--lg kt-badge--rounded kt-badge--bold kt-font-success'><img src = 'https://takimomrubucketyeni.s3.eu-west-2.amazonaws.com/Docs/icon-reward.png' /></ span >
                                </ div >
                                < div class='kt-user-card__name'>
                                    @ViewBag.Name
                                </div>
                            </div>
                            <!--end: Head -->
                            <!--begin: Navigation -->
                            <div class='kt-notification'>
                                <a href = '@Url.Action('Profil', 'Home')' class='kt-notification__item'>
                                    <div class='kt-notification__item-icon'>
                                        <i class='flaticon2-calendar-3 kt-font-success'></i>
                                    </div>
                                    <div class='kt-notification__item-details'>
                                        <div class='kt-notification__item-title kt-font-bold'>
                                            Hesabım
                                        </div>
                                    </div>
                                </a>
                                <div class='kt-notification__custom kt-space-between'>
                                    <a href = '@Url.Action('Login', 'Login')' class='btn btn-label btn-label-brand btn-sm btn-bold'>Çıkış Yap</a>
  
                                </div>
                            </div>
                            <!--end: Navigation -->
                        </div>
                    </div>
                    <!--end: User Bar -->
                </div>
                <!-- end::Header Topbar -->
            </div>
            <div class='kt-container  kt-container--fluid  kt-grid__item kt-grid__item--fluid' style='margin-bottom:30px;'>
                <div class='kt-portlet kt-portlet--mobile'>
                    <div class='kt-portlet__head kt-portlet__head--lg'>
                        <div class='kt-portlet__head-label'>
                            <span class='kt-portlet__head-icon'>
                                <img src = '/Images/@ViewBag.Logo' style='width:35px; height:35px;' />
                                @*<i class='kt-font-brand flaticon2-line-chart'></i>*@
                            </span>
                            <h3 class='kt-portlet__head-title'>
                                @ViewBag.Company Takım Ömrü
                            </h3>
                        </div>
                        <div class='kt-portlet__head-toolbar'>
                            <div class='kt-portlet__head-wrapper'>
                                @* begin: Quick panel toggler*@
                                @*<div class='kt-header__topbar-item kt-header__topbar-item--quick-panel' data-placement='right'>
                                        <span class='kt-header__topbar-icon' id='kt_quick_panel_toggler_btn'>
                                            <div class='kt-header__topbar-wrapper' data-toggle='dropdown' data-offset='30px,0px' aria-expanded='true'>
                                                <span style = 'font-size:15px; margin-right:15px;' class='flaticon2-bell-alarm-symbol btn btn-icon btn-brand btn-sm kt-pulse'>
                                                    @foreach(var item in Model.notification)
{
                                                        < span class='badge'>@Html.DisplayFor(modelItem => item.Count)</span>
                                                    }
                                                </span>
                                            </div>
                                        </span>
                                    </div>*@
                                @* end: Quick panel toggler
                                     begin::Quick Panel*@
                                <div id = 'kt_quick_panel' class='kt-quick-panel'>
                                    <a href = '#' class='kt-quick-panel__close' id='kt_quick_panel_close_btn'><i class='flaticon2-delete'></i></a>
                                    <div class='kt-quick-panel__nav'>
                                        <ul class='nav nav-tabs nav-tabs-line nav-tabs-bold nav-tabs-line-3x nav-tabs-line-brand  kt-notification-item-padding-x' role='tablist'>
                                            <li class='nav-item active'>
                                                <a class='nav-link active' data-toggle='tab' href='#kt_quick_panel_tab_notifications' role='tab'>Tümü</a>
                                            </li>
                                            <li class='nav-item'>
                                                <a class='nav-link' data-toggle='tab' href='#kt_quick_panel_tab_settings' role='tab'>Son 7 Gün</a>
                                            </li>
                                            <li class='nav-item'>
                                                <a class='nav-link' data-toggle='tab' href='#kt_quick_panel_tab_settings_otuz' role='tab'>Son 7 Gün</a>
                                            </li>
                                            @*<li class='nav-item'>
                                                    <a class='nav-link' data-toggle='tab' href='#kt_quick_panel_tab_settings_otuz' role='tab'>Son 30 Gün</a>
                                                </li>*@
                                        </ul>
                                    </div>
                                    <div class='kt-quick-panel__content'>
                                        <div class='tab-content'>
                                            <div class='tab-pane fade show kt-scroll active' id='kt_quick_panel_tab_notifications' role='tabpanel'>
                                                <div class='kt-notification' id='notificationtumliste'>
                                                    @* @foreach(var item in Model.notification)
{
                                                            < a href = '@Url.Action('Index', 'Notification', new { id=item.NotificationID })' class='kt-notification__item'>
                                                                <div class='kt-notification__item-icon'>
                                                                    <i class='flaticon2-box-1 kt-font-brand'></i>
                                                                </div>
                                                                <div class='kt-notification__item-details'>
                                                                    <div class='kt-notification__item-title'>
                                                                        @Html.DisplayFor(modelItem => item.Notification_Description)
                                                                    </div>
                                                                    <div class='kt-notification__item-time'>
                                                                        @Html.DisplayFor(modelItem => item.Notification_Date)
                                                                    </div>
                                                                </div>
                                                            </a>
                                                        }*@
                                                    @*<a href = '@Url.Action('Details', 'Notification', new { id=item.NotificationID })' class='kt-notification__item'>
                                                            <div class='kt-notification__item-icon'>
                                                                <i class='flaticon2-box-1 kt-font-brand'></i>
                                                            </div>
                                                            <div class='kt-notification__item-details'>
                                                                <div class='kt-notification__item-title' style='font-family:Poppins, Helvetica, sans-serif;'>
                                                                </div>
                                                                <div class='kt-notification__item-time'>
                                                                </div>
                                                            </div>
                                                        </a>*@
                                                </div>
                                            </div>
                                            <div class='tab-pane fade show kt-scroll' id='kt_quick_panel_tab_settings' role='tabpanel'>
                                                <div class='kt-notification' id='notificationyediliste'>
                                                </div>
                                            </div>
                                            <div class='tab-pane fade show kt-scroll' id='kt_quick_panel_tab_settings_otuz' role='tabpanel'>
                                                <div class='kt-notification' id='notificationotuzliste'>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class='kt-form__group kt-form__group--inline'>
                                    <div class='dropdown dropdown-inline'>
                                        <button type = 'button' style='margin-right:15px;' class='btn btn-icon btn-brand btn-sm' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>
                                            <i style = 'font-size:20px;' class='la la-gear'></i>
                                        </button>
                                        <div class='dropdown-menu dropdown-menu-right'>
                                            <ul class='kt-nav'>
                                                <li class='kt-nav__item'>
                                                    <a href = '@Url.Action('Export', 'Piece')' class='kt-nav__link'>
                                                        <i class='kt-nav__link-icon la la-file-excel-o'></i>
                                                        <span class='kt-nav__link-text'>Takımı Dışa Aktar</span>
                                                    </a>
                                                </li>
                                                <li class='kt-nav__item'>
                                                    <a href = '@Url.Action('ExportAll', 'SubPiece')' class='kt-nav__link'>
                                                        <i class='kt-nav__link-icon la la-file-excel-o'></i>
                                                        <span class='kt-nav__link-text'>Tüm Parçaları Dışa Aktar</span>
                                                    </a>
                                                </li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                                <div class='dropdown dropdown-inline'>
                                    <button type = 'button' class='btn btn-brand btn-icon btn-sm' data-toggle='modal' data-target='#exampleModal' aria-haspopup='true' aria-expanded='false'>
                                        <i style = 'font-size:15px; ' class='flaticon2-plus'></i>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                    @* takim import excel *@
                    <div class='modal fade' id='exampleModall' tabindex='-1' role='dialog' aria-labelledby='exampleModalLabel' aria-hidden='true'>
                        <div class='modal-dialog' role='document'>
                            <div class='modal-content' style='width:600px'>
                                <div class='modal-header'>
                                    <h5 class='modal-title' id='exampleModalLabel'>Excel Dosyası Yükle</h5>
                                    <button type = 'button' class='close' data-dismiss='modal' aria-label='Close'>
                                        <span aria-hidden='true'>&times;</span>
                                    </button>
                                </div>
                                <div class='modal-body'>
                                    <!--begin::Form-->
                                    @using(Html.BeginForm('Upload', 'Piece', FormMethod.Post, new
                                    {
                                        enctype = 'multipart/form-data'
                                    }))
                                    {
                                        <table>
                                            <tr>
                                                <td> <input class='btn btn-default' type='file' name='UploadedFile' value='Excel Yükle' /> </td>
                                            </tr>
                                            <tr>
                                                <td colspan = '2' > < input type='submit' value='Kaydet' class='btn btn-default' /></td>
                                            </tr>
                                        </table>
                                    }
                                    <!--end::Form-->
                                </div>
                            </div>
                        </div>
                    </div>
                    @* parca import excel *@
                    <div class='modal fade' id='exampleModalll' tabindex='-1' role='dialog' aria-labelledby='exampleModalLabel' aria-hidden='true'>
                        <div class='modal-dialog' role='document'>
                            <div class='modal-content' style='width:600px'>
                                <div class='modal-header'>
                                    <h5 class='modal-title' id='exampleModalLabel'>Excel Dosyası</h5>
                                    <button type = 'button' class='close' data-dismiss='modal' aria-label='Close'>
                                        <span aria-hidden='true'>&times;</span>
                                    </button>
                                </div>
                                <div class='modal-body'>
                                    @using(Html.BeginForm('Upload', 'SubPiece', FormMethod.Post, new
                                    {
                                        enctype = 'multipart/form-data'
                                    }))
                                    {
                                        <table>
                                            <tr>
                                                <td> <input class='btn btn-default' type='file' name='UploadedFile' value='Excel Yükle' /> </td>
                                                <td> <input type = 'text' name='id' id='importt' value='' /> </td>
                                            </tr>
                                            <tr>
                                                <td colspan = '2' > < input type='submit' value='Kaydet' class='btn btn-default' /></td>
                                            </tr>
                                        </table>
                                    }
                                </div>
                            </div>
                        </div>
                    </div>
                    @* parca export modal *@
                    <div class='modal fade' id='parcaexportmodal' tabindex='-1' role='dialog' aria-labelledby='exampleModalLabel' aria-hidden='true'>
                        <div class='modal-dialog' role='document'>
                            <div class='modal-content' style='width:600px'>
                                <div class='modal-header'>
                                    <h5 class='modal-title' id='exampleModalLabel'>Excel Dosyası</h5>
                                    <button type = 'button' class='close' data-dismiss='modal' aria-label='Close'>
                                        <span aria-hidden='true'>&times;</span>
                                    </button>
                                </div>
                                <div class='modal-body'>
                                    @using(Html.BeginForm('Export', 'SubPiece', FormMethod.Post, new
                                    {
                                        enctype = 'multipart/form-data'
                                    }))
                                    {
                                        <table>
                                            <tr>
                                                <td>Exceli Dışa Aktar</td>
                                                <td> <input type = 'text' name='idsubpiece' id='exportt' value='' /> </td>
                                            </tr>
                                            <tr>
                                                <td colspan = '2' > < input type='submit' value='Dışa Aktar' class='btn btn-default' /></td>
                                            </tr>
                                        </table>
                                    }
                                </div>
                            </div>
                        </div>
                    </div>
                    @* parca düzenle modal *@
                    <div class='modal fade' id='parcaguncelle' tabindex='-1' role='dialog' aria-labelledby='exampleModalLabel' aria-hidden='true'>
                        <div class='modal-dialog' role='document'>
                            <div class='modal-content' style='width:600px'>
                                <div class='modal-header'>
                                    <h5 class='modal-title' id='exampleModalLabel'>Parça Düzenle</h5>
                                    <button type = 'button' class='close' data-dismiss='modal' aria-label='Close'>
                                        <span aria-hidden='true'>&times;</span>
                                    </button>
                                </div>
                                <div class='modal-body'>
                                    @using(Html.BeginForm('ParcaGuncelle', 'Home', FormMethod.Post))
                                    {
                                        @Html.AntiForgeryToken()
                                        <div class='form-horizontal' style='padding-top:10px'>
                                            @Html.ValidationSummary(true, '', new { @class = 'text-danger' })
                                            <div class='form-group'>
                                                @Html.Label('Parça Adı: ', new { @class = 'control-label col-md-2' })
                                                <div class='col-md-10'>
                                                    <input type = 'text' name='par' id='parcaduz' value='' />
                                                    @Html.EditorFor(model => model.subPiece.SubPieceName, new { htmlAttributes = new { @class = 'form-control' } })
                                                    @Html.ValidationMessageFor(model => model.subPiece.SubPieceName, '', new { @class = 'text-danger' })
                                                </div>
                                            </div>
                                            <div class='form-group'>
                                                @Html.Label('Takım Ömrü: ', new { @class = 'control-label col-md-2' })
                                                <div class='col-md-10'>
                                                    @Html.EditorFor(model => model.subPiece.ToolLife, new { htmlAttributes = new { @class = 'form-control' } })
                                                </div>
                                            </div>
                                            <div class='form-group'>
                                                <div class='col-md-offset-2 col-md-10'>
                                                    <input type = 'submit' value='Güncelle' class='btn btn-default' />
                                                </div>
                                            </div>
                                        </div>
                                    }
                                </div>
                            </div>
                        </div>
                    </div>
                    @* parca sil modal *@
                    <div class='modal fade' id='parcasil' tabindex='-1' role='dialog' aria-labelledby='exampleModalLabel' aria-hidden='true'>
                        <div class='modal-dialog' role='document'>
                            <div class='modal-content' style='width:600px'>
                                <div class='modal-header'>
                                    <h5 class='modal-title' id='exampleModalLabel'>Parçayı Sil</h5>
                                    <button type = 'button' class='close' data-dismiss='modal' aria-label='Close'>
                                        <span aria-hidden='true'>&times;</span>
                                    </button>
                                </div>
                                <div class='modal-body'>
                                    <dl class='dl-horizontal'>
                                        <dt style = 'width:290px;' >
                                            @Html.Label('Parçayı silmek istediğinize emin misiniz?')
                                        </ dt >
                                    </ dl >< hr />
                                    @using(Html.BeginForm('ParcaSil', 'Home', FormMethod.Post))
                                    {
                                        @Html.AntiForgeryToken()
                                        <input type = 'text' name='parcaid' id='parc' value='' />
                                        <div class='form-actions no-color'>
                                            <input type = 'submit' value='Sil' class='btn btn-default' />
                                        </div>
                                    }
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class='modal fade' id='exampleModal' tabindex='-1' role='dialog' aria-labelledby='exampleModalLabel' aria-hidden='true'>
                        <div class='modal-dialog' role='document'>
                            <div class='modal-content' style='width:600px'>
                                <div class='modal-header'>
                                    <h5 class='modal-title' id='exampleModalLabel'>Alt Parça Ekle</h5>
                                    <button type = 'button' class='close' data-dismiss='modal' aria-label='Close'>
                                        <span aria-hidden='true'>&times;</span>
                                    </button>
                                </div>
                                <div class='modal-body'>
                                    @using(Html.BeginForm('Create', 'Home', FormMethod.Post))
                                    {
                                        @Html.AntiForgeryToken()
                                        <div class='form-horizontal' style='padding-top:10px'>
                                            @Html.ValidationSummary(true, '', new { @class = 'text-danger' })
                                            <div class='form-group'>
                                                @*<label class='control-label col-md-2'>Tipi:</label>*@
                                                <div class='col-md-10'>
                                                    <div class='input-group'>
                                                        <div class='input-group-append'>
                                                            <span class='input-group-text'>
                                                                <i class='la la-navicon'></i>
                                                            </span>
                                                        </div>
                                                        @Html.DropDownList('SelectType', new List<SelectListItem> {
                                                                                new SelectListItem{Text = 'Parça Sayısına Göre', Value = '0'},
                                                                                new SelectListItem{Text = 'Zamana Göre', Value = '1'},
                                                                            }, new { @id = 'SelectType', @class = 'form-control' })
                                                    </div>
                                                </div>
                                            </div>
                                            <div class='form-group'>
                                                @*<label class='control-label col-md-2'>Parça Adı:</label>*@
                                                <div class='col-md-10'>
                                                    <div class='input-group'>
                                                        <div class='input-group-append'>
                                                            <span class='input-group-text'>
                                                                <i class='la la-cube'></i>
                                                            </span>
                                                        </div>
                                                        @Html.EditorFor(model => model.subPiece.SubPieceName, new { htmlAttributes = new { @class = 'form-control', @id = 'subpiecename', onblur = 'Kontrol()', placeholder = 'Parça Adı' } })
                                                    </div>
                                                    @Html.ValidationMessageFor(model => model.subPiece.SubPieceName, '', new { @class = 'text-danger' })
                                                </div>
                                            </div>
                                            <div class='form-group'>
                                                @*<label id = 'totalcountlbl' class='control-label col-md-2'>Takım Ömrü:</label>*@
                                                @* @Html.LabelFor(model => model.TotalCount, htmlAttributes: new { @class = 'control-label col-md-2', @id = 'totalcountlbl' })*@
                                                @*<label id = 'datepickerlbl' class='control-label col-md-2'>Takım Ömrü:</label>*@
                                                <div class='col-md-10'>
                                                    <div class='input-group'>
                                                        <div id = 'totalcountlbl' class='input-group-append'>
                                                            <span class='input-group-text'>
                                                                <i class='la la-heart-o'></i>
                                                            </span>
                                                        </div>
                                                        @Html.EditorFor(model => model.subPiece.ToolLife, new { htmlAttributes = new { @class = 'form-control', @id = 'totalcount', placeholder = 'Takım Ömrü' } })
                                                        @Html.ValidationMessageFor(model => model.subPiece.ToolLife, '', new { @class = 'text-danger' })
                                                    </div>
                                                    <div class='input-group date'>
                                                        <div id = 'datepickerlbl' class='input-group-append'>
                                                            <span class='input-group-text'>
                                                                <i class='la la-tachometer'></i>
                                                            </span>
                                                        </div>
                                                        <input type = 'text' class='form-control startdatee' name='startdatee' placeholder='gün/ay/yıl' readonly id='kt_datetimepicker_2' />
                                                        <div class='input-group-append' id='tarihicon1'>
                                                            <span class='input-group-text'>
                                                                <i class='la la-calendar'></i>
                                                            </span>
                                                        </div>
                                                        <input type = 'text' class='form-control enddatee' name='enddatee' placeholder='gün/ay/yıl' readonly id='kt_datetimepicker_2' />
                                                        <div class='input-group-append' id='tarihicon2'>
                                                            <span class='input-group-text'>
                                                                <i class='la la-calendar'></i>
                                                            </span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class='form-group'>
                                                @*<label class='control-label col-md-2'>Takımın Adı:</label>*@
                                                @* @Html.LabelFor(model => model..PieceID, 'PieceID', htmlAttributes: new { @class = 'control-label col-md-2' })*@
                                                <div class='col-md-10'>
                                                    <div class='input-group'>
                                                        <div class='input-group-append'>
                                                            <span class='input-group-text'>
                                                                <i class='la la-navicon'></i>
                                                            </span>
                                                        </div>
                                                        @Html.DropDownList('FKPieceID', null, htmlAttributes: new { @class = 'form-control' })
                                                        @Html.ValidationMessageFor(model => model.subPiece.FKPieceID, '', new { @class = 'text-danger' })
                                                    </div>
                                                </div>
                                            </div>
                                            <div class='form-group'>
                                                <div class='col-md-offset-2 col-md-10 modal-footer'>
                                                    <input type = 'submit' value='Kapat' onclick='Kaydett()' class='btn btn-default' data-dismiss='modal' />
                                                    <input type = 'submit' value='Ekle' @* onclick = 'Kaydet()' *@ class='btn btn-default' />
                                                </div>
                                            </div>
                                        </div>
                                    }
                                </div>
                            </div>
                        </div>
                    </div>
                    @* Takım Not Ekle modal*@
                    <div class='modal fade' id='takimanotekle' tabindex='-1' role='dialog' aria-labelledby='exampleModalLabel' aria-hidden='true'>
                        <div class='modal-dialog' role='document'>
                            <div class='modal-content' style='width:600px'>
                                <div class='modal-header'>
                                    <h5 class='modal-title' id='exampleModalLabel'>Not Ekle</h5>
                                    <button type = 'button' class='close' data-dismiss='modal' aria-label='Close'>
                                        <span aria-hidden='true'>&times;</span>
                                    </button>
                                </div>
                                <div class='modal-body'>
                                    @using(Html.BeginForm('CreateNoteTakim', 'Notes', FormMethod.Post))
                                    {
                                        @Html.AntiForgeryToken()
                                        <div class='form-horizontal'>
                                            @Html.ValidationSummary(true, '', new { @class = 'text-danger' })
                                            <div class='form-group'>
                                                <label class='control-label col-md-2'></label>
                                                <div class='col-md-10'>
                                                    <div class='input-group'>
                                                        <div class='input-group-append'>
                                                            <span class='input-group-text'>
                                                                <i class='la la-commenting'></i>
                                                            </span>
                                                        </div>
                                                        @Html.EditorFor(model => model.note.NoteHeader, new { htmlAttributes = new { @class = 'form-control', placeholder = 'Not Başlık' } })
                                                        @Html.ValidationMessageFor(model => model.note.NoteHeader, '', new { @class = 'text-danger' })
                                                    </div>
                                                </div>
                                            </div>
                                            <div class='form-group'>
                                                <label class='control-label col-md-2'></label>
                                                <div class='col-md-10'>
                                                    <div class='input-group'>
                                                        <div class='input-group-append'>
                                                            <span class='input-group-text'>
                                                                <i class='la la-pencil-square'></i>
                                                            </span>
                                                        </div>
                                                        @Html.EditorFor(model => model.note.NoteDescription, new { htmlAttributes = new { @class = 'form-control', placeholder = 'Notunuzu giriniz' } })
                                                        @Html.ValidationMessageFor(model => model.note.NoteDescription, '', new { @class = 'text-danger' })
                                                    </div>
                                                </div>
                                            </div>
                                            @*<div class='form-group'>
                                                    @Html.LabelFor(model => model.note.CreatedDate, htmlAttributes: new { @class = 'control-label col-md-2' })
                                                    <div class='col-md-10'>
                                                        @Html.EditorFor(model => model.note.CreatedDate, new { htmlAttributes = new { @class = 'form-control' } })
                                                        @Html.ValidationMessageFor(model => model.note.CreatedDate, '', new { @class = 'text-danger' })
                                                    </div>
                                                </div>*@
                                            @*<div class='form-group'>
                                                    @Html.LabelFor(model => model.note.FkPieceID, 'FkPieceID', htmlAttributes: new { @class = 'control-label col-md-2' })
                                                    <div class='col-md-10'>
                                                        @Html.DropDownListFor(model=>model.note.FkPieceID, (SelectList) ViewBag.FkPieceIDd, htmlAttributes: new { @class = 'form-control' })
                                                        @Html.ValidationMessageFor(model => model.note.FkPieceID, '', new { @class = 'text-danger' })
                                                    </div>
                                                </div>*@
                                            @*<div class='form-group'>
                                                    @Html.LabelFor(model => model.note.FkUserID, 'FkUserID', htmlAttributes: new { @class = 'control-label col-md-2' })
                                                    <div class='col-md-10'>
                                                        @Html.DropDownList('FkUserID', null, htmlAttributes: new { @class = 'form-control' })
                                                        @Html.ValidationMessageFor(model => model.note.FkUserID, '', new { @class = 'text-danger' })
                                                    </div>
                                                </div>*@
                                            <input type = 'text' name='nottakimid' id='notetakim' value='' />
                                            <input type = 'text' name='typetakim' id='typetakim' value='False' />
                                            <div class='form-group'>
                                                <div class='col-md-offset-7 col-md-10'>
                                                    <input type = 'submit' value='Notu Ekle' class='btn btn-default' />
                                                </div>
                                            </div>
                                        </div>
                                    }
                                </div>
                            </div>
                        </div>
                    </div>
                    @* Parça Not Ekle modal*@
                    <div class='modal fade' id='parcayanotekle' tabindex='-1' role='dialog' aria-labelledby='exampleModalLabel' aria-hidden='true'>
                        <div class='modal-dialog' role='document'>
                            <div class='modal-content' style='width:600px'>
                                <div class='modal-header'>
                                    <h5 class='modal-title' id='exampleModalLabel'>Parçaya Not Ekle</h5>
                                    <button type = 'button' class='close' data-dismiss='modal' aria-label='Close'>
                                        <span aria-hidden='true'>&times;</span>
                                    </button>
                                </div>
                                <div class='modal-body'>
                                    @using(Html.BeginForm('CreateNoteParca', 'Notes', FormMethod.Post))
                                    {
                                        @Html.AntiForgeryToken()
                                        <div class='form-horizontal'>
                                            @Html.ValidationSummary(true, '', new { @class = 'text-danger' })
                                            <div class='form-group'>
                                                <label class='control-label col-md-2'></label>
                                                <div class='col-md-10'>
                                                    <div class='input-group'>
                                                        <div class='input-group-append'>
                                                            <span class='input-group-text'>
                                                                <i class='la la-commenting'></i>
                                                            </span>
                                                        </div>
                                                        @Html.EditorFor(model => model.note.NoteHeader, new { htmlAttributes = new { @class = 'form-control', placeholder = 'Not Başlık' } })
                                                        @Html.ValidationMessageFor(model => model.note.NoteHeader, '', new { @class = 'text-danger' })
                                                    </div>
                                                </div>
                                            </div>
                                            <div class='form-group'>
                                                <label class='control-label col-md-2'></label>
                                                <div class='col-md-10'>
                                                    <div class='input-group'>
                                                        <div class='input-group-append'>
                                                            <span class='input-group-text'>
                                                                <i class='la la-pencil-square'></i>
                                                            </span>
                                                        </div>
                                                        <div class='editor-field'>
                                                            @Html.TextAreaFor(model => model.note.NoteDescription, new { htmlAttributes = new { @class = 'form-control', placeholder = 'Notunuzu giriniz', @id = 'comment' } })
                                                            @Html.ValidationMessageFor(model => model.note.NoteDescription, '', new { @class = 'text-danger' })
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            @*<div class='form-group'>
                                                    @Html.LabelFor(model => model.note.CreatedDate, htmlAttributes: new { @class = 'control-label col-md-2' })
                                                    <div class='col-md-10'>
                                                        @Html.EditorFor(model => model.note.CreatedDate, new { htmlAttributes = new { @class = 'form-control' } })
                                                        @Html.ValidationMessageFor(model => model.note.CreatedDate, '', new { @class = 'text-danger' })
                                                    </div>
                                                </div>*@
                                            @*<div class='form-group'>
                                                    @Html.LabelFor(model => model.note.FkPieceID, 'FkPieceID', htmlAttributes: new { @class = 'control-label col-md-2' })
                                                    <div class='col-md-10'>
                                                        @Html.DropDownListFor(model=>model.note.FkPieceID, (SelectList) ViewBag.FkPieceIDd, htmlAttributes: new { @class = 'form-control' })
                                                        @Html.ValidationMessageFor(model => model.note.FkPieceID, '', new { @class = 'text-danger' })
                                                    </div>
                                                </div>*@
                                            @*<div class='form-group'>
                                                    @Html.LabelFor(model => model.note.FkUserID, 'FkUserID', htmlAttributes: new { @class = 'control-label col-md-2' })
                                                    <div class='col-md-10'>
                                                        @Html.DropDownList('FkUserID', null, htmlAttributes: new { @class = 'form-control' })
                                                        @Html.ValidationMessageFor(model => model.note.FkUserID, '', new { @class = 'text-danger' })
                                                    </div>
                                                </div>*@
                                            <input type = 'text' name='notparcaid' id='noteparca' value='' />
                                            <input type = 'text' name='typeparca' id='typeparca' value='True' />
                                            <div class='form-group'>
                                                <div class='col-md-offset-7 col-md-10'>
                                                    <input type = 'submit' value='Notu Ekle' class='btn btn-default' />
                                                </div>
                                            </div>
                                        </div>
                                    }
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class='kt-portlet__body'>
                        <!--begin: Search Form -->
                        <div class='kt-form kt-form--label-right kt-margin-t-20 kt-margin-b-10'>
                            <div class='row align-items-center'>
                                <div class='col-xl-8 order-2 order-xl-1'>
                                    <div class='row align-items-center'>
                                        <div class='col-md-4 kt-margin-b-20-tablet-and-mobile'>
                                            <div class='kt-input-icon kt-input-icon--left'>
                                                <input type = 'text' class='form-control' placeholder='Arama...' id='generalSearch'>
                                                <span class='kt-input-icon__icon kt-input-icon__icon--left'>
                                                    <span><i class='la la-search'></i></span>
                                                </span>
                                            </div>
                                        </div>
                                        <div class='col-md-8 kt-margin-b-20-tablet-and-mobile'>
                                            <div class='kt-form__group kt-form__group--inline'>
                                                <div class='kt-form__label'>
                                                    <label>Tarih:</label>
                                                </div>
                                                <div class='input-daterange input-group' id='kt_datepicker_5' data-date-format='dd.mm.yyyy'>
                                                    <input type = 'text' class='form-control datepickers' style='width:500px' id='ilk' placeholder='gün/ay/yıl' name='start' />
                                                    <span class='input-group-text'>
                                                        <i class='la la-calendar'></i>
                                                    </span>
                                                    <input style = 'width:40px; margin-left:18px' type='text' id='son' class='form-control datepickers' placeholder='gün/ay/yıl' name='end' />
                                                    <span class='input-group-text'>
                                                        <i class='la la-calendar'></i>
                                                    </span>
                                                </div>
                                            </div>
                                        </div>
                                        @*<div class='col-md-4 kt-margin-b-20-tablet-and-mobile'>
                                                <div class='kt-input-icon kt-input-icon--left'>
                                                    <input type = 'text' class='form-control' placeholder='Arama...' id='generalSearchh'>
                                                    <span class='kt-input-icon__icon kt-input-icon__icon--left'>
                                                        <span><i class='la la-search'></i></span>
                                                    </span>
                                                </div>
                                            </div>*@
                                        @*<div class='col-md-4 kt-margin-b-20-tablet-and-mobile'>
                                                <div class='kt-form__group kt-form__group--inline'>
                                                    <div class='dropdown dropdown-inline'>
                                                        <button type = 'button' class='btn btn-brand btn-sm' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>
                                                            <i class='la la-gear'></i>
                                                        </button>
                                                        <div class='dropdown-menu dropdown-menu-right'>
                                                            <ul class='kt-nav'>
                                                                <li class='kt-nav__item'>
                                                                    <a href = '@Url.Action('Export', 'Piece')' class='kt-nav__link'>
                                                                        <i class='kt-nav__link-icon la la-file-excel-o'></i>
                                                                        <span class='kt-nav__link-text'>Takımı Dışa Aktar</span>
                                                                    </a>
                                                                </li>
                                                                <li class='kt-nav__item'>
                                                                    <a href = '@Url.Action('ExportAll', 'SubPiece')' class='kt-nav__link'>
                                                                        <i class='kt-nav__link-icon la la-file-excel-o'></i>
                                                                        <span class='kt-nav__link-text'>Tüm Parçaları Dışa Aktar</span>
                                                                    </a>
                                                                </li>
                                                            </ul>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>*@
                                        <div class='col-md-4 kt-margin-b-20-tablet-and-mobile'>
                                            <div class='kt-form__group kt-form__group--inline'>
                                                <div class='kt-form__control'>
                                                    @* @Html.DropDownListFor(m => m.PieceId, (SelectList) ViewBag.Piece, 'Lütfen Seçiniz', new { @class = 'dropdown form-control', @id = 'kt_form_status' })*@
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!--end: Search Form -->
                    </div>
                    <div class='kt-portlet'>
                        <div class='kt-portlet__body'>
                            <ul class='nav nav-tabs' role='tablist'>
                                <li class='nav-item'>
                                    <a class='nav-link active' data-toggle='tab' href='#' data-target='#kt_tabs_1_1'>Parça Sayısına Göre</a>
                                </li>
                                <li class='nav-item'>
                                    <a class='nav-link' data-toggle='tab' href='#kt_tabs_1_3'>Zamana Göre</a>
                                </li>
                                <li class='nav-item'>
                                    <a class='nav-link' style='color:red;' data-toggle='tab' href='#kt_tabs_1_4'>Ömrü Tükenmeye Yakın Olan</a>
                                </li>
                            </ul>
                            <div class='tab-content'>
                                <div class='tab-pane active' id='kt_tabs_1_1' role='tabpanel'>
                                    <div class='kt-form kt-form--label-align-right kt-margin-t-10 kt-margin-b-30'>
                                        <div class='row'>
                                            @*<div class='col-lg-12'>
                                                    <button class='btn btn-secondary' type='button' id='datatable_asc'>A-Z ye sırala</button>
                                                    <button class='btn btn-secondary' type='button' id='datatable_desc'>Z-A ya sırala</button>
                                                </div>*@
                                        </div>
                                    </div>
                                    <div class='kt-portlet__body kt-portlet__body--fit'>
                                        <!--begin: Datatable -->
                                        <div class='datatable childd' id='child_data_ajax'>
                                        </div>
                                        <!--end: Datatable -->
                                    </div>
                                </div>
                                <div class='tab-pane' id='kt_tabs_1_3' role='tabpanel'>
                                    <div class='kt-portlet__body kt-portlet__body--fit'>
                                        <!--begin: Datatable -->
                                        <div class='datatabletime time' id='child_time'>
                                        </div>
                                        <!--end: Datatable -->
                                    </div>
                                </div>
                                <div class='tab-pane' id='kt_tabs_1_4' role='tabpanel'>
                                    <div class='kt-portlet__body kt-portlet__body--fit'>
                                        <!--begin: Datatable -->
                                        <div class='data childs' id='child'>
                                        </div>
                                        <!--end: Datatable -->
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class='kt-footer  kt-grid__item kt-grid kt-grid--desktop kt-grid--ver-desktop' id='kt_footer' style='margin-top:19%;'>
                    <div class='kt-container  kt-container--fluid '>
                        <div class='kt-footer__copyright'>
                            2020&nbsp;&copy;&nbsp;@*<a href = 'http://keenthemes.com/metronic' target='_blank' class='kt-link'>* @Tezmaksan@*</a>*@
                        </div>
                    </div>
                </div>
            </div>
            <!-- end::Content -->
        </div>
    </div>
    @section Scripts
{
        <script type='text/javascript'>
            $(function () {
        // Declare a proxy to reference the hub.
        var notifications = $.connection.myHub;
        //debugger;
        // Create a function that the hub can call to broadcast messages.
        notifications.client.updatenotification = function() {
            NotificationList()
                };
                // Start the connection.
                $.connection.hub.start().done(function() {
            console.log('connection started')
                    //notifications.onconn();
                    NotificationList();
        }).fail(function(e) {
            alert(e);
        });
    });
    function NotificationList() {
        var tbl = $('#notificationyediliste');
                $.ajax({
        url: '/Notification/NotificationList',
                    contentType: 'application/html ; charset:utf-8',
                    type: 'GET',
                    dataType: 'html',
                    success: function(result) {
                console.log(result);
                var a2 = JSON.parse(result);
                tbl.empty();
                var i = 1;
                        $.each(a2, function(key, value) {
                    tbl.append('<a href='/Notification/Details/' + value.NotificationID + '' class='kt-notification__item'>' + ' <div class='kt-notification__item-icon'>' + '<i class='flaticon2-box-1 kt-font-brand'></i>' + '</div>' + '<div class='kt-notification__item-details'>' + ' <div class='kt-notification__item-title' style='font-family:Poppins, Helvetica, sans-serif;'>' + value.Notification_Description + '</div>' + '<div class='kt-notification__item-time'>' + value.Notification_Date + '</div>' + '</div>' + '</a>');
                    i = i + 1;
                });
            }
        });
    }
        </script>
        <script type='text/javascript'>
            $(function () {
        // Declare a proxy to reference the hub.
        var notifications = $.connection.myHub;
        //debugger;
        // Create a function that the hub can call to broadcast messages.
        notifications.client.updatenotification = function() {
            NotificationTumList()
                };
                // Start the connection.
                $.connection.hub.start().done(function() {
            console.log('connection started')
                    //notifications.onconn();
                    NotificationTumList();
        }).fail(function(e) {
            alert(e);
        });
    });
    function NotificationTumList() {
        var tbl = $('#notificationtumliste');
                $.ajax({
        url: '/Notification/NotificationTumList',
                    contentType: 'application/html ; charset:utf-8',
                    type: 'GET',
                    dataType: 'html',
                    success: function(result) {
                console.log(result);
                var a2 = JSON.parse(result);
                tbl.empty();
                var i = 1;
                        $.each(a2, function(key, value) {
                    tbl.append('<a href='/Notification/Details/' + value.NotificationID + '' class='kt-notification__item'>' + ' <div class='kt-notification__item-icon'>' + '<i class='flaticon2-box-1 kt-font-brand'></i>' + '</div>' + '<div class='kt-notification__item-details'>' + ' <div class='kt-notification__item-title' style='font-family:Poppins, Helvetica, sans-serif;'>' + value.Notification_Description + '</div>' + '<div class='kt-notification__item-time'>' + value.Notification_Date + '</div>' + '</div>' + '</a>');
                    i = i + 1;
                });
            }
        });
    }
        </script>
        <script type='text/javascript'>
            $(function () {
        // Declare a proxy to reference the hub.
        var notifications = $.connection.myHub;
        //debugger;
        // Create a function that the hub can call to broadcast messages.
        notifications.client.updatenotification = function() {
            NotificationOtuzList()
                };
                // Start the connection.
                $.connection.hub.start().done(function() {
            console.log('connection started')
                    //notifications.onconn();
                    NotificationOtuzList();
        }).fail(function(e) {
            alert(e);
        });
    });
    function NotificationOtuzList() {
        var tbl = $('#notificationotuzliste');
                $.ajax({
        url: '/Notification/NotificationOtuzList',
                    contentType: 'application/html ; charset:utf-8',
                    type: 'GET',
                    dataType: 'html',
                    success: function(result) {
                console.log(result);
                var a2 = JSON.parse(result);
                tbl.empty();
                var i = 1;
                        $.each(a2, function(key, value) {
                    tbl.append('<a href='/Notification/Details/' + value.NotificationID + '' class='kt-notification__item'>' + ' <div class='kt-notification__item-icon'>' + '<i class='flaticon2-box-1 kt-font-brand'></i>' + '</div>' + '<div class='kt-notification__item-details'>' + ' <div class='kt-notification__item-title' style='font-family:Poppins, Helvetica, sans-serif;'>' + value.Notification_Description + '</div>' + '<div class='kt-notification__item-time'>' + value.Notification_Date + '</div>' + '</div>' + '</a>');
                    i = i + 1;
                });
            }
        });
    }
        </script>
        <script src='https://cdn.onesignal.com/sdks/OneSignalSDK.js' async=''></script>
        <script>
            var OneSignal = window.OneSignal || [];
    OneSignal.push(function () {
        OneSignal.init({
        appId: 'd6d2fce9-81ab-4567-ac68-6ef75ba7b203',
                });
    });
        </script>
        <style>
            .tooltip-inner
    {
        min - width: 250px; /* the minimum width */
    color: #595d6e;
                line - height: 25px;
        white - space: pre - line;
        border - radius: 8px;
        background - color: wheat;
        font - family: Poppins, Helvetica, sans - serif;
        text - decoration: underline;
    }
            /* Tooltip */
            .progress-bar + .tooltip > .tooltip-inner
    {
        background - color: #73AD21;
                color: #FFFFFF;
                border: 1px solid green;
    padding: 15px;
        font - size: 20px;
    }
        </style>
        <style>
            #chartdiv {
                width: 100%;
    height: 300px;
}
        </style>
        <script src = 'https://www.amcharts.com/lib/4/core.js' ></ script >
        < script src='https://www.amcharts.com/lib/4/charts.js'></script>
        <script src = 'https://www.amcharts.com/lib/4/themes/animated.js' ></ script >
        < script >
            $(function () {
                $('#SelectType').on('change', function () {
    if (parseInt($('#SelectType').val()) == 0)
    {
                        $('.startdatee').hide();
                        $('.enddatee').hide();
                        $('#bolme').hide();
                        $('#countlbl').show();
                        $('#counttxt').show();
                        $('#datepickerlbl').hide();
                        $('#counttimelbl').hide();
                        $('#totalcount').show();
                        $('#totalcountlbl').show();
                        $('#tarihicon1').hide();
                        $('#tarihicon2').hide();
    }
    else
    {
                        $('.startdatee').show();
                        $('.enddatee').show();
                        $('#bolme').show();
                        $('#datepickerlbl').show();
                        $('#datepickerlbl').show();
                        $('#countlbl').hide();
                        $('#counttxt').hide();
                        $('#counttimelbl').hide();
                        $('#totalcount').hide();
                        $('#totalcountlbl').hide();
                        $('#tarihicon1').show();
                        $('#tarihicon2').show();
    }
});
                $('#SelectType').trigger('change');
            });
        </script>
        <script>
            function Kontrol()
{
    var start = $('.startdatee').val();
    var end = $('.enddatee').val();
    var startDay = new Date(start);
    var endDay = new Date(end);
    if (startDay > endDay)
    {
        alert('başlangıç tarihi bitiş tarihinden büyük olamaz');
                    $('.startdatee').val('');
                    $('.enddatee').val('');
    }
}
function Kaydet()
{
    //document.getElementById('counttxt').disabled = false;
    var start = $('.startdatee').val();
    var end = $('.enddatee').val();
    var startDay = new Date(start);
    var endDay = new Date(end);
    var millisecondsPerDay = 1000 * 60 * 60 * 24;
    var millisBetween = endDay.getTime() - startDay.getTime();
    var days = (millisBetween / millisecondsPerDay);
    var hours = days * 24;
                // Round down.
                $('#totalcount').val(hours);
    //var now = new Date()
    //var nowtostart = now.getTime() - startDay.getTime();
    //var day = (nowtostart / millisecondsPerDay);
    //var hour = day * 24;
    //$('#counttxt').val(hour.toFixed(0));
}
        </script>
    }
    <!-- end::Page -->
    <!-- begin::Scrolltop -->
    <!-- end::Scrolltop -->
    <!-- end::Sticky Toolbar -->
    <!-- begin::Demo Panel -->
    <!-- end::Demo Panel -->
    <!--Begin::Chat-->
    <!--ENd::Chat-->
    <!-- begin::Global Config(global config for global JS sciprts) -->
    <style>
        .multiselect {
            width: 200px;
        }
        .selectBox {
            position: relative;
        }
            .selectBox select
{
    width: 100%;
    font-weight: bold
}
        .overSelect {
            position: absolute;
            left: 0;
            right: 0;
            top: 0;
            bottom: 0;
        }
        #checkboxes {
            display: none;
            border: 1px #dadada solid;
        }
            #checkboxes label {
                display: block;
            }
    </style>
    <script>
        var expanded = false;
function showCheckboxes()
{
    var checkboxes = document.getElementById('checkboxes');
    if (!expanded)
    {
        checkboxes.style.display = 'block';
        expanded = true;
    }
    else
    {
        checkboxes.style.display = 'none';
        expanded = false;
    }
}
    </script>
    <script>
        var KTAppOptions = {
            'colors': {
                'state': {
                    'brand': '#5d78ff',
                    'dark': '#282a3c',
                    'light': '#ffffff',
                    'primary': '#5867dd',
                    'success': '#34bfa3',
                    'info': '#36a3f7',
                    'warning': '#ffb822',
                    'danger': '#fd3995'
                },
                'base': {
                    'label': [
                        '#c5cbe3',
                        '#a1a8c3',
                        '#3d4465',
                        '#3e4466'
                    ],
                    'shape': [
                        '#f0f3ff',
                        '#d9dffa',
                        '#afb4d4',
                        '#646c9a'
                    ]
                }
            }
        };
    </script>
    @*<script type = 'text/javascript' >
            $(document).ready(function () {
    KTDatatablee();
});
        </script>*@
    <!--end::Page Scripts -->
            ";
            var response = new HttpResponseMessage();
            response.Content = new StringContent(responseString);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }


    }
}
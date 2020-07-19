using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
//using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain;
using AWSServerless_CoreAPI_v5.Domain.Services;
using AWSServerless_CoreAPI_v5.Extensions;
using AWSServerless_CoreAPI_v5.Resource;
using AWSServerless_CoreAPI_v5.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication16.Domain;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        
        private readonly IAuthenticationService authenticationService;
        private readonly TakimOmruDBContext db;
        public LoginController(IAuthenticationService authenticationService, TakimOmruDBContext _db)
        {
            this.authenticationService = authenticationService;
            db = _db;
        }

        [HttpPost]
        public IActionResult Accesstoken(LoginResource loginResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
            else
            {
                AccessTokenResponse accessTokenResponse = authenticationService.CreateAccessToken(loginResource.Username, loginResource.Password);

                if (accessTokenResponse.Success)
                {
                    return Ok(accessTokenResponse.accesstoken);
                }
                else
                {
                    return BadRequest(accessTokenResponse.Message);
                }
            }
        }


        [HttpPost]
        public IActionResult AddUser([FromBody]User user)
        {
            
            db.User.Add(new User()
            {

                UserName = user.UserName,
                Password = user.Password,
                FirstName = user.FirstName,
                LastName = user.LastName

            });

            db.SaveChanges();
            return Ok("Kayıt Başarıyla eklendi");
        }

    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WebApplication16.Domain;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Controllers
{
    [ApiController]
    public class AwsController : ControllerBase
    {

        private readonly TakimOmruDBContext db;
        private readonly IAwsS3HelperService _awsS3Service;
        public AwsController(TakimOmruDBContext _db, IAwsS3HelperService awsS3Service)
        {
            db = _db;
            _awsS3Service = awsS3Service;
        }

        [HttpPost]
        [Route("api/prod/upload")]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm]User user)
        {
            if (file.Length == 0)
            {
                return BadRequest("please provide valid file");
            }
            var fileName = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName
                .TrimStart().ToString();
            var folderName = Request.Form.ContainsKey("folder") ? Request.Form["folder"].ToString() : null;
            bool status;
            using (var fileStream = file.OpenReadStream())
                
            using (var ms = new MemoryStream())
            {
                await fileStream.CopyToAsync(ms);
                status = await _awsS3Service.UploadFileAsync(ms, fileName, folderName);
                db.User.Add(new User()
                {
                    FirstName=user.FirstName,
                    LastName=user.LastName,
                    Password=user.Password,
                    Company=user.Company,
                    Email=user.Email,
                    UserName=user.UserName,
                    CompanyLogo = "https://takimomrubucketyeni.s3.eu-west-2.amazonaws.com/Images/" + fileName
                });
                db.SaveChanges();
            }
            return status ? Ok("success")
                          : StatusCode((int)HttpStatusCode.InternalServerError, $"error uploading {fileName}");
        }
        /// <summary>
        /// endpoint for retrieving content from s3 bucket title directory
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Route("api/prod/get/{folder}")]
        public async Task<IActionResult> Get(string fileName, string folder)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folder))
            {
                return BadRequest("please provide valid file or valid folder name");
            }
            var response = await _awsS3Service.ReadFileAsync(fileName, folder);
            if (response.FileStream == null)
            {
                return NotFound();
            }
            return File(response.FileStream, response.ContentType);
        }
        /// <summary>
        /// endpoint for removing files from s3 bucket
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Route("api/prod/remove")]
        public async Task<IActionResult> Remove(string fileName, string folder)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folder))
            {
                return BadRequest("please provide valid file and/or valid folder name");
            }
            ;
            if (await _awsS3Service.RemoveFileAsync(fileName, folder))
            {
                return NoContent();
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, $"error removing {fileName}");
        }
    }
    
}
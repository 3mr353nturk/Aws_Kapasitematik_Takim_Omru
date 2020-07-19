using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class UserModel
    {

        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string CompanyLogo { get; set; }
        public string Email { get; set; }

    }
}
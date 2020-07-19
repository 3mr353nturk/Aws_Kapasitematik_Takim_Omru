using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string CompanyLogo { get; set; }
        public string Email { get; set; }
    }
}

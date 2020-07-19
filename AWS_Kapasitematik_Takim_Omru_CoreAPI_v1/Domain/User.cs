using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication16.Domain
{
    public partial class User
    {
        public User()
        {
            Piece = new HashSet<Piece>();
            Notification = new HashSet<Notification>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string CompanyLogo { get; set; }
        public string Email { get; set; }

        public ICollection<Piece> Piece { get; set; }
        public ICollection<Notification> Notification { get; set; }
    }
}

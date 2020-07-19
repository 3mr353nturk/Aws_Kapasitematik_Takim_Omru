using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class PieceModel
    {
        public int PieceId { get; set; }
        public string PieceName { get; set; }
        public int? FkUserID { get; set; }
        public string createdDateString => CreatedDate.ToString("dd/MM/yyyy");
        public DateTime CreatedDate { get; set; }
        public int? Adet { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class DetailModel
    {
        public int DetailID { get; set; }
        public int? PieceCount { get; set; }
        
        public string CreatedDate { get; set; }
        public int? FkSubPieceID { get; set; }
        public int? FKUserID { get; set; }
    }
}
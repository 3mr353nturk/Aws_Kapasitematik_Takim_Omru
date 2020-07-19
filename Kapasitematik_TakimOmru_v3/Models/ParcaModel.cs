using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class ParcaModel
    {
        public int SubPieceID { get; set; }
        public string SubPieceName { get; set; }
        public int? ToolLife { get; set; }
        public int? FkPieceID { get; set; }
        public int? FkuserID { get; set; }
        public string CreatedDate { get; set; }
    }
}
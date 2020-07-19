using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class Details
    {
        public int DetailID { get; set; }
        public int? PieceCount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? FkSubPieceID { get; set; }
        public int? FKUserID { get; set; }
        public string SubPieceName { get; set; }
        public int? ToolLife { get; set; }

    }
}
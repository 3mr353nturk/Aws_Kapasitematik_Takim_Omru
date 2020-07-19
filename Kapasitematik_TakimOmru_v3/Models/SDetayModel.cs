using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class SDetayModel
    {
        public int DetailID { get; set; }
        public string SubPieceName { get; set; }
        public int? ToolLife { get; set; }
        public int? PieceCount { get; set; }
        public string CreatedDate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class Model
    {
        public int SubPieceID { get; set; }
        public int FKSubPieceID { get; set; }
        public string SubPieceName { get; set; }
        public int ToolLife { get; set; }
        public int Now { get; set; }
        public int FKPieceID { get; set; }
        public int FKUserID { get; set; }
        public bool Type { get; set; }
    }
}
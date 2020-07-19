using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class SubPiece
    {

        public int SubPieceID { get; set; }
        public string SubPieceName { get; set; }
        public int? ToolLife { get; set; }
        public int? Now { get; set; }
        public bool? Type { get; set; }
        public int? FkPieceID { get; set; }
        public int? FkuserID { get; set; }

        public Piece FkPiece { get; set; }
        public UserModel Fkuser { get; set; }
        public ICollection<Details> Detail { get; set; }
    }
}
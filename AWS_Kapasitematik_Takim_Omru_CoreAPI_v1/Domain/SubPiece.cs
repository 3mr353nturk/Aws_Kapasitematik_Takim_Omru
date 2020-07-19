
using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain;
using System;
using System.Collections.Generic;

namespace WebApplication16.Domain
{
    public partial class SubPiece
    {
        public SubPiece()
        {
            Detail = new HashSet<Detail>();
        }

        public int SubPieceID { get; set; }
        public string SubPieceName { get; set; }
        public int? ToolLife { get; set; }
        public int? Now { get; set; }
        public bool? Type { get; set; }
        public int? FkPieceID { get; set; }
        public int? FkuserID { get; set; }

        public Piece FkPiece { get; set; }
        public User Fkuser { get; set; }
        public ICollection<Detail> Detail { get; set; }
    }
}

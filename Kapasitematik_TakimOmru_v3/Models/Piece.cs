using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public partial class Piece
    {
        public Piece()
        {
            SubPiece = new HashSet<SubPiece>();
        }

        public int PieceId { get; set; }
        public string PieceName { get; set; }
        public int? FkUserID { get; set; }
        public int? Adet { get; set; }

        public UserModel FkUser { get; set; }
        public ICollection<SubPiece> SubPiece { get; set; }
    }
}
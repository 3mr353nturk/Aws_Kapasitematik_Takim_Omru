using System;
using System.Collections.Generic;

namespace WebApplication16.Domain
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
        public DateTime? CreatedDate { get; set; }
        public int? Adet { get; set; }

        public User FkUser { get; set; }
        public ICollection<SubPiece> SubPiece { get; set; }
    }
}

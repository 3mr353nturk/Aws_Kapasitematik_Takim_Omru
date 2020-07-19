using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class ThreeModel
    {
       public IEnumerable<Kapasitematik_TakimOmru_v3.Piece> pieces { get; set; }
       public IEnumerable<Kapasitematik_TakimOmru_v3.SubPiece> subpieces { get; set; }
       public IEnumerable<Kapasitematik_TakimOmru_v3.Notification> notification { get; set; }
       public IEnumerable<Kapasitematik_TakimOmru_v3.Note> notes { get; set; }
       public Kapasitematik_TakimOmru_v3.SubPiece subPiece { get; set; }
       public Kapasitematik_TakimOmru_v3.Piece piece { get; set; }
       public Kapasitematik_TakimOmru_v3.Note note { get; set; }


        public ThreeModel()
        {
            subPiece = new Kapasitematik_TakimOmru_v3.SubPiece();
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class AltKategoriModel
    {
        public int SubPieceId { get; set; }
        public string subPieceName { get; set; }
        public int toolLife { get; set; }
        public int pieceID { get; set; }
        public int pieceCount { get; set; }
        public int kalan { get; set; }
        public int subpiece { get; set; }
       
        public string createdDateString => createdDate.ToString("dd/MM/yyyy");

        public DateTime createdDate { get; set; }
    }
}
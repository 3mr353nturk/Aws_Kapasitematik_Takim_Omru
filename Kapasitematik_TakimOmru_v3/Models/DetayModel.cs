using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class DetayModel
    {
        public string subPieceName { get; set; }
        public int toolLife { get; set; }
        public int pieceCount { get; set; }
        public string createdDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain
{
    public class TakimModul
    {
        
        public int PieceID { get; set; }
        public string PieceName { get; set; }
        public int? FKUserID { get; set; }
        public int? Adet { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}

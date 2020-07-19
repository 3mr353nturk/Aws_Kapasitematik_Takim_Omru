using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain
{
    public class JoinModel
    {
        public int SubPieceId { get; set; }
        public string subPieceName { get; set; }
        public int? toolLife { get; set; }
        public int pieceID { get; set; }
        public int? pieceCount { get; set; }
        public DateTime? createdDate { get; set; }
    }
}

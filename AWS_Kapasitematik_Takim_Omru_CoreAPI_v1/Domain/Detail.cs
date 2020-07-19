using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication16.Domain;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain
{
    public partial class Detail
    {
        public int DetailId { get; set; }
        public int? PieceCount { get; set; }
        public DateTime? CreatedDate { get; set; }
        
        public int? FkSubPieceID { get; set; }
        public int? FkUserID { get; set; }

        public SubPiece FkSubPiece { get; set; }
    }
}

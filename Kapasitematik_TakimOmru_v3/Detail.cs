//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kapasitematik_TakimOmru_v3
{
    using System;
    using System.Collections.Generic;
    
    public partial class Detail
    {
        public int DetailID { get; set; }
        public Nullable<int> PieceCount { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> FKSubPieceID { get; set; }
        public Nullable<int> FKUserID { get; set; }
    
        public virtual SubPiece SubPiece { get; set; }
        public virtual User User { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication16.Domain;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain
{
    public partial class Note
    {
        public int NoteId { get; set; }
        public string NoteHeader { get; set; }
        public string NoteDescription { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? FkpieceId { get; set; }
        public int? FksubpieceId { get; set; }
        public int? FkuserId { get; set; }
        public bool Type { get; set; }

        public User Fkuser { get; set; }
        public Piece Fkpiece { get; set; }
        public SubPiece Fksubpiece { get; set; }
    }
}

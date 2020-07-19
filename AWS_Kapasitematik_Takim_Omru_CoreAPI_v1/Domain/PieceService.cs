using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication16.Domain;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain
{
    public class PieceService
    {
        //private readonly TakimOmruDBContext db;
        //private readonly IHubContext<SignalRHub> hubContext;
        //public PieceService(TakimOmruDBContext _db,IHubContext<SignalRHub> _hubContext)
        //{
        //    db = _db;
        //    hubContext = _hubContext;
        //}

        //public IQueryable<Piece> GetList()
        //{
        //    return db.Piece.AsQueryable();
        //}

        //public async Task SavePiece(Piece piece)
        //{
        //    await db.Piece.AddAsync(piece);
        //    await db.SaveChangesAsync();
        //    await hubContext.Clients.All.SendAsync("PieceList", GetList());
        //}
    }
}

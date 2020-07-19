using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication16.Domain;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Hubs
{
    public class SignalRHub : Hub
    {

        //public static void SendPiece()
        //{
        //    IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
        //    context.Clients.All.updateMessage();
        //}
        //private readonly PieceService pieceService;
        //public SignalRHub(PieceService _pieceService)
        //{
        //    pieceService = _pieceService;
        //}
        //public async Task GetPieceList()
        //{
        //    await Clients.All.SendAsync("PieceList", pieceService.GetList());
        //}




        //private readonly TakimOmruDBContext db;
        //public SignalRHub(TakimOmruDBContext _db)
        //{
        //    db = _db;
        //}
        //public async Task BroadcastFromClient(Piece piece)
        //{
        //    try
        //    {
        //        Piece p = new Piece()
        //        {
        //            PieceName = piece.PieceName,
        //        };

        //        db.Piece.Add(p);
        //        await db.SaveChangesAsync();

        //        await Clients.All.SendAsync(
        //            "Piece",
        //            new
        //            {
        //                piecename = p.PieceName,
        //            }
        //            );
        //    }
        //    catch (Exception ex)
        //    {

        //        await Clients.Caller.SendAsync("HubError", new { error = ex.Message });
        //    }
        //}
        //public async Task SubPieceBroadcast(SubPiece piece)
        //{
        //    try
        //    {
        //        SubPiece p = new SubPiece()
        //        {
        //            SubPieceName = piece.SubPieceName,
        //            ToolLife = piece.ToolLife,
        //            Type = piece.Type,
        //        };

        //        db.SubPiece.Add(p);
        //        await db.SaveChangesAsync();

        //        await Clients.All.SendAsync(
        //            "SubPiece",
        //            new
        //            {
        //                SubPieceName = p.SubPieceName,
        //                ToolLife = p.ToolLife,
        //                Type = p.Type,
        //            }
        //            );
        //    }
        //    catch (Exception ex)
        //    {

        //        await Clients.Caller.SendAsync("HubError", new { error = ex.Message });
        //    }
        //}
        //public async Task Detail(AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain.Detail detail)
        //{
        //    try
        //    {
        //        AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain.Detail p = new AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain.Detail()
        //        {
        //            PieceCount = detail.PieceCount,
        //            CreatedDate = detail.CreatedDate,
        //            FkSubPieceID = detail.FkSubPieceID,
        //            FkUserID = detail.FkUserID,
        //        };

        //        db.Detail.Add(p);
        //        await db.SaveChangesAsync();

        //        await Clients.All.SendAsync(
        //            "Detail",
        //            new
        //            {
        //                PieceCount = p.PieceCount,
        //                CreatedDate = p.CreatedDate,
        //                FkSubPieceID = p.FkSubPieceID,
        //                FkUserID = p.FkUserID,
        //            }
        //            );
        //    }
        //    catch (Exception ex)
        //    {

        //        await Clients.Caller.SendAsync("HubError", new { error = ex.Message });
        //    }
        //}
    }
}

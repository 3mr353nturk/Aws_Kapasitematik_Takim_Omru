using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class RepositoryGridPiece
    {
        SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BaglantiProvider"].ConnectionString);
        public List<PieceModels> GridList(int sessionId,int machineId)
        {
            var piece = new List<PieceModels>();
            using (var cmd = new SqlCommand($@"SELECT [PieceID], 
 [PieceName], [CreatedDate], [Adet] FROM [PimsunDB].[dbo].[MachineListTbl] INNER JOIN [dbo].[Piece] ON [MachineID]=[PieceID] where [FKUserId]={sessionId} AND [MachineID]={machineId}", con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                var dependency = new SqlDependency(cmd);
                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                DataSet ds = new DataSet();
                da.Fill(ds);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    piece.Add(item: new PieceModels
                    {
                        PieceId = int.Parse(ds.Tables[0].Rows[i][0].ToString()),
                        PieceName = ds.Tables[0].Rows[i][1].ToString(),
                        CreatedDate = Convert.ToDateTime(ds.Tables[0].Rows[i][2]).ToString("dd/MM/yyyy"),
                        Adet = Convert.ToInt32(ds.Tables[0].Rows[i][3]),


                    });
                }
            }
            return piece;
        }
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e) //this will be called when any changes occur in db table.  
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MyHub.SendGridPiece();
            }
        }
    }
}
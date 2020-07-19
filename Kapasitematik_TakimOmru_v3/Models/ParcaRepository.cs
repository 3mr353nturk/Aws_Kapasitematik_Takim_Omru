using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class ParcaRepository
    {
        SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BaglantiProvider"].ConnectionString);
        public List<ParcaModel> SubPieceList(int sessionId)
        {
            var subpiece = new List<ParcaModel>();
            using (var cmd = new SqlCommand($@"SELECT [SubPieceID], 
 [SubPieceName], [ToolLife], [CreatedDate] FROM [dbo].[SubPiece] INNER JOIN [dbo].[Detail] ON [SubPieceID]=[FKSubPieceID] where [dbo].[SubPiece].[FKUserId]={sessionId}", con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                var dependency = new SqlDependency(cmd);
                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                DataSet ds = new DataSet();
                da.Fill(ds);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    subpiece.Add(item: new ParcaModel
                    {
                        SubPieceID = int.Parse(ds.Tables[0].Rows[i][0].ToString()),
                        SubPieceName = ds.Tables[0].Rows[i][1].ToString(),
                        ToolLife = Convert.ToInt32(ds.Tables[0].Rows[i][2]),
                        CreatedDate = Convert.ToDateTime(ds.Tables[0].Rows[i][3]).ToString("dd/MM/yyyy")


                    });
                }
            }
            return subpiece;
        }
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e) //this will be called when any changes occur in db table.  
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MyHub.SendSubPiece();
            }
        }
    }
}
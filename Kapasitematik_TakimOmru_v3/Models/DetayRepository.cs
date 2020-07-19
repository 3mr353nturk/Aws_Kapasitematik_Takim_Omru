﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class DetayRepository
    {
        SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BaglantiProvider"].ConnectionString);
        public List<SDetayModel> DetayListe(int sessionId, int SubPieceId)
        {
            var detail = new List<SDetayModel>();
            using (var cmd = new SqlCommand($@"SELECT [DetailID], [SubPieceName], [ToolLife], [PieceCount],  
 [CreatedDate] FROM [dbo].[Detail] INNER JOIN [dbo].[SubPiece] ON [FKSubPieceID]=[SubPieceID] where [FKSubPieceID]={SubPieceId} and [dbo].[Detail].[FKUserId]={sessionId}", con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                var dependency = new SqlDependency(cmd);
                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                DataSet ds = new DataSet();

                da.Fill(ds);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    detail.Add(item: new SDetayModel
                    {
                        DetailID = int.Parse(ds.Tables[0].Rows[i][0].ToString()),
                        SubPieceName = ds.Tables[0].Rows[i][1].ToString(),
                        ToolLife = Convert.ToInt32(ds.Tables[0].Rows[i][2]),
                        PieceCount = Convert.ToInt32(ds.Tables[0].Rows[i][3]),
                        CreatedDate = Convert.ToDateTime(ds.Tables[0].Rows[i][4]).ToString("dd/MM/yyyy"),
                       
                        
                        


                    });
                }
            }
            return detail;
        }
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e) //this will be called when any changes occur in db table.  
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MyHub.SendGridDetay();
            }
        }
    }
}
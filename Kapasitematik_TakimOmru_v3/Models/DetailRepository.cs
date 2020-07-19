using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class DetailRepository
    {
        SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BaglantiProvider"].ConnectionString);
        public List<DetailModel> DetailList(int sessionId)
        {
            var detail = new List<DetailModel>();
            using (var cmd = new SqlCommand($@"SELECT [DetailID], 
 [CreatedDate], [PieceCount] FROM [dbo].[Detail] where [FKUserId]={sessionId}", con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                var dependency = new SqlDependency(cmd);
                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                DataSet ds = new DataSet();
                
                da.Fill(ds);
                
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    detail.Add(item: new DetailModel
                    {
                        DetailID = int.Parse(ds.Tables[0].Rows[i][0].ToString()),
                        CreatedDate =Convert.ToDateTime( ds.Tables[0].Rows[i][1]).ToString("dd/MM/yyyy"),
                        PieceCount = Convert.ToInt32(ds.Tables[0].Rows[i][2]),


                    });
                }
            }
            return detail;
        }
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e) //this will be called when any changes occur in db table.  
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MyHub.SendDetail();
            }
        }
    }
}
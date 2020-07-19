using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class NotificationOtuzRepository
    {
        SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BaglantiProvider"].ConnectionString);
        public List<NotificationModel> NotificationOtuzList(int sessionId)
        {
            var notification = new List<NotificationModel>();
            using (var cmd = new SqlCommand($@"SELECT [NotificationID], 
 [Notification_Description], [Notification_Date] FROM [dbo].[Notification] where [FKUserId]={sessionId} AND [Notification_Date]>DATEADD(DAY,-30,GETDATE())", con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                var dependency = new SqlDependency(cmd);
                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                DataSet ds = new DataSet();
                da.Fill(ds);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    notification.Add(item: new NotificationModel
                    {
                        NotificationID = int.Parse(ds.Tables[0].Rows[i][0].ToString()),
                        Notification_Description = ds.Tables[0].Rows[i][1].ToString(),
                        Notification_Date = ds.Tables[0].Rows[i][2].ToString(),


                    });
                }
            }
            return notification;
        }
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e) //this will be called when any changes occur in db table.  
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MyHub.SendNotification();
            }
        }
    }
}
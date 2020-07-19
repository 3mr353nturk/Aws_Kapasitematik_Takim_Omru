using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class MachineRepository
    {

        SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BaglantiProvider"].ConnectionString);
        public List<MachineModels> MachineList()
        {
            var machine = new List<MachineModels>();
            using (var cmd = new SqlCommand($@"SELECT [MachineID], 
 [MachineName], [MachineModel] FROM [PimsunDB].[dbo].[MachineListTbl]", con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                var dependency = new SqlDependency(cmd);
                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                DataSet ds = new DataSet();
                da.Fill(ds);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    machine.Add(item: new MachineModels
                    {
                        MachineID = int.Parse(ds.Tables[0].Rows[i][0].ToString()),
                        MachineName = ds.Tables[0].Rows[i][1].ToString(),
                        MachineModel=ds.Tables[0].Rows[i][2].ToString()


                    });
                }
            }
            return machine;
        }
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e) //this will be called when any changes occur in db table.  
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MyHub.SendGridMachine();
            }
        }
    }
}
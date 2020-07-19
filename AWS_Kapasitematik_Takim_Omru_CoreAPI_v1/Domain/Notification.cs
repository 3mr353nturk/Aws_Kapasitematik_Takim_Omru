using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication16.Domain;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public string Notification_Description { get; set; }
        public int? Count { get; set; }
        public DateTime? Notification_Date { get; set; }
        public int? FkuserId { get; set; }

        public User Fkuser { get; set; }
    }
}

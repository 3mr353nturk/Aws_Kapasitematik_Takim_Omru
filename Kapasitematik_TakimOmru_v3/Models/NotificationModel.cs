using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class NotificationModel
    {
        public int NotificationID { get; set; }
        public string Notification_Description { get; set; }
        public string Notification_Date { get; set; }
        public int? FKUserId { get; set; }
    }
}
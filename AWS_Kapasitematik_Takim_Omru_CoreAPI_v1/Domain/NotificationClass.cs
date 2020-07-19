using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication16.Domain;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain
{
    public partial class NotificationClass
    {
        public int NotificationId { get; set; }
        public string NotificationDescription { get; set; }
        public DateTime? NotificationDate { get; set; }
        public int? FkuserId { get; set; }

        public User Fkuser { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class ProfilModel
    {
        public IEnumerable<Kapasitematik_TakimOmru_v3.User> users { get; set; }
        public IEnumerable<Kapasitematik_TakimOmru_v3.Note> notes { get; set; }
        public Kapasitematik_TakimOmru_v3.User user { get; set; }
        public Kapasitematik_TakimOmru_v3.Note note { get; set; }
    }
}
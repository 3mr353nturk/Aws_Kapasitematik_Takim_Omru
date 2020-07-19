
//using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication16.Domain;

namespace AWSServerless_CoreAPI_v5.Repositories
{
    public class BaseRepository
    {
        protected readonly TakimOmruDBContext context;
        public BaseRepository(TakimOmruDBContext context)
        {
            this.context = context;
        }
    }
}

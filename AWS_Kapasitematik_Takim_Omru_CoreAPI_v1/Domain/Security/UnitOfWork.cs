//using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication16.Domain;

namespace AWSServerless_CoreAPI_v5.Domain.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TakimOmruDBContext context;
        public UnitOfWork(TakimOmruDBContext context)
        {
            this.context = context;
        }
        public void Complete()
        {
            this.context.SaveChanges();
        }

        public async Task CompleteAsync()
        {
            await this.context.SaveChangesAsync();
        }
    }
}

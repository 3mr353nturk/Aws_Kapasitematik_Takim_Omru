using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerless_CoreAPI_v5.Domain.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
        void Complete();
    }
}

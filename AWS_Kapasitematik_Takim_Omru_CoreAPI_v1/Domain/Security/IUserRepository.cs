
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication16.Domain;

namespace AWSServerless_CoreAPI_v5.Repositories
{
    public interface IUserRepository
    {
        User FindById(int userId);
        User FindByUsernameandPassword(string username, string password);
    }
}

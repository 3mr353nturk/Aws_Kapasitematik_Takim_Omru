
using AWSServerless_CoreAPI_v5.Security;
//using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication16.Domain;

namespace AWSServerless_CoreAPI_v5.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly TokenOptions tokenOptions;
        public UserRepository(TakimOmruDBContext context, IOptions<TokenOptions> tokenOptions) : base(context)
        {
            this.tokenOptions = tokenOptions.Value;
        }

        public User FindById(int userId)
        {
            return context.User.Find(userId);
        }

        public User FindByUsernameandPassword(string username, string password)
        {
            return context.User.Where(u => u.UserName == username && u.Password == password).FirstOrDefault();
        }
    }
}

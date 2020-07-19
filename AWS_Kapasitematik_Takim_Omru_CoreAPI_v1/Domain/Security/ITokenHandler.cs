using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication16.Domain;

namespace AWSServerless_CoreAPI_v5.Security
{
    public interface ITokenHandler
    {
        AccessToken CreateAccessToken(User user);
    }
}

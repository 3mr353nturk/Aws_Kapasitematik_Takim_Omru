using AWSServerless_CoreAPI_v5.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerless_CoreAPI_v5.Domain.Services
{
    public interface IAuthenticationService
    {
        AccessTokenResponse CreateAccessToken(string username, string password);
    }
}

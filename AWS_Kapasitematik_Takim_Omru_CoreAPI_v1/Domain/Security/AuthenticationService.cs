using AWSServerless_CoreAPI_v5.Domain.Services;
using AWSServerless_CoreAPI_v5.Responses;
using AWSServerless_CoreAPI_v5.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerless_CoreAPI_v5.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService userService;

        private readonly ITokenHandler tokenHandler;

        public AuthenticationService(IUserService userService, ITokenHandler tokenHandler)
        {
            this.userService = userService;
            this.tokenHandler = tokenHandler;
        }
        public AccessTokenResponse CreateAccessToken(string username, string password)
        {
            UserResponse userResponse = userService.FindUsernameAndPassword(username, password);

            if (userResponse.Success)
            {
                AccessToken accessToken = tokenHandler.CreateAccessToken(userResponse.user);
                return new AccessTokenResponse(accessToken);
            }
            else
            {
                return new AccessTokenResponse(userResponse.Message);
            }
        }
    }
}

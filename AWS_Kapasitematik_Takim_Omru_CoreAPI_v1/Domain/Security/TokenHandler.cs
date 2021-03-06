﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WebApplication16.Domain;

namespace AWSServerless_CoreAPI_v5.Security
{
    public class TokenHandler : ITokenHandler
    {
        private readonly TokenOptions tokenOptions;
        public TokenHandler(IOptions<TokenOptions> tokenOptions)
        {
            this.tokenOptions = tokenOptions.Value;
        }
        public AccessToken CreateAccessToken(User user)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(tokenOptions.AccessTokenExpiration);

            var securityKey = SignHandler.GetSecurityKey(tokenOptions.SecurityKey);

            SigningCredentials signinCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken JwtSecurityToken = new JwtSecurityToken(
                issuer: tokenOptions.Issuer,
                audience: tokenOptions.Audience,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaim(user),
                signingCredentials: signinCredentials
        );

            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(JwtSecurityToken);
            AccessToken accessToken = new AccessToken();
            accessToken.Token = token;
            accessToken.Expiration = accessTokenExpiration;

            return accessToken;
        }

        private IEnumerable<Claim> GetClaim(User user)
        {

            var claims = new List<Claim>
                {

                    new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                    new Claim(ClaimTypes.GivenName,$"{user.FirstName}"),
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim("Password",user.Password),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                };
            return claims;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerless_CoreAPI_v5.Security
{
    public class TokenOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public long AccessTokenExpiration { get; set; }
        public string SecurityKey { get; set; }
    }
}

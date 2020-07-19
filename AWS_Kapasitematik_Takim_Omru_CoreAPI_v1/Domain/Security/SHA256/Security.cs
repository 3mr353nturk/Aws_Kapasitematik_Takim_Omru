using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain.Security.SHA256
{
    public class Security
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private const string Key = "cut-the-night-with-the-light";
        public Security(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string Encrypt(string input)
        {
            var protector = _dataProtectionProvider.CreateProtector(Key);
            return protector.Protect(input);
        }

        public string Decrypt(string input)
        {
            var protector = _dataProtectionProvider.CreateProtector(Key);
            return protector.Unprotect(input);
        }
    }
}

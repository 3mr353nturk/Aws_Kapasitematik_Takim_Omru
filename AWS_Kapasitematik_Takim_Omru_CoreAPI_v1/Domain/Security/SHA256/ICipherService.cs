using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain.Security.SHA256
{
    public interface ICipherService
    {
        string Encrypt(string cipherText);
        string Decrypt(string cipherText);
    }
}

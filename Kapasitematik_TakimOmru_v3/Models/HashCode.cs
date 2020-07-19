using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Kapasitematik_TakimOmru_v3.Models
{
    public class HashCode
    {
        public string PassHass(string data)
        {
            SHA256 sha = SHA256.Create();
            byte[] hashdata = sha.ComputeHash(Encoding.Default.GetBytes(data));
            StringBuilder returnvalue = new StringBuilder();

            for (int i = 0; i < hashdata.Length; i++)
            {
                returnvalue.Append(hashdata[i].ToString());
            }
            return returnvalue.ToString();
        }
    }
}
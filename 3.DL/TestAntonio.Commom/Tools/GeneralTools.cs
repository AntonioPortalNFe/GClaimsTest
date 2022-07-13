using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TestAntonio.Commom.Extensions;

namespace TestAntonio.Commom.Tools
{
    public static class GeneralTools
    {
        public static string GenerateHash(long ts, string pulicKey, string privateKey)
        {            

            return MD5Hash(ts.ToString() + privateKey + pulicKey );
        }

        public static string MD5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                return string.Concat(Array.ConvertAll(hash, h => h.ToString("x2")));
            }
        }
    }
}

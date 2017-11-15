using System;
using System.Security.Cryptography;

namespace FastRsync.Tests
{
    class Utils
    {
        public static string GetMd5(byte[] data)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                return Convert.ToBase64String(md5Hash.ComputeHash(data));
            }
        }
    }
}

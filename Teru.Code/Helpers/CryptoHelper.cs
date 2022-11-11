using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Teru.Code.Helpers
{
    public class CryptoHelper
    {
        public static string GetSHA256(Stream stream)
        {
            string result = string.Empty;
            byte[] by = null;

            stream.Position = 0;
            var s = System.Security.Cryptography.SHA256.Create();
            by = s.ComputeHash(stream);

            result = BitConverter.ToString(by).Replace("-", "").ToLower();
            return result;
        }
    }
}

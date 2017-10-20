using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BasicLoginServer.CustomLibraries
{
    public class GenerateUniqueValues
    {
        public static string ReturnUniqueEightDigitNumber()
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
            return String.Format("{0:D8}", random);
        }
    }
}
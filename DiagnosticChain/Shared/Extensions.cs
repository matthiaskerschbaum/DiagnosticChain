using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Shared
{
    public static class Extensions
    {
        public static string AsString(this RSAParameters parameter)
        {
            //return parameter.Modulus
            //    + parameter.Exponent
            //    + parameter.D
            //    + parameter.P
            //    + parameter.DP
            //    + parameter.DQ
            //    + parameter.

            return parameter.GetHashCode().ToString();
        }
    }
}

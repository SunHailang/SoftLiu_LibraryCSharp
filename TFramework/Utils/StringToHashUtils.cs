using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFramework.Utils
{
    public static class StringToHashUtils
    {

        public static int GetHashValue(string str)
        {
            if (string.IsNullOrEmpty(str)) return 0;
            int value = 0;
            for (int i = 0; i < str.Length; i++)
            {
                value = 31 * value + str[i];
            }
            return value;
        }

        public static void TestHello()
        {

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZNStatChess.Types
{
    static class ZNHelper
    {
        public static string DeleteWhitespacesFromString(string input)
        {
            int length = input.Length;
            StringBuilder outputBuilder = new StringBuilder(length);
            char ch;

            for (int i = 0; i < length; i++)
            {
                ch = input[i];
                if (!Char.IsWhiteSpace(ch))
                {
                    outputBuilder.Append(ch);
                }
            }

            return outputBuilder.ToString();
        }
    }
}

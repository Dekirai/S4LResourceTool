using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S4LResourceTool
{
    internal static class ByteFormat
    {
        public static string ToByteString(this int value)
        {
            string text = ((int)((float)value / 1024f)).ToString().Reverse<char>().Aggregate("", (string current, char c) => current + c.ToString());
            int num = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (num++ == 3)
                {
                    text = text.Insert(i, ",");
                    num = 0;
                }
            }
            return text.Reverse<char>().Aggregate("", (string current, char c) => current + c.ToString()) + " KB";
        }
    }
}

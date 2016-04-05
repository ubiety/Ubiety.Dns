using System.Text;

namespace Ubiety.Dns.Extensions
{
    public static class StringExtensions
    {
        public static byte[] WriteName(this string source)
        {
            if (!source.EndsWith("."))
            {
                source += ".";
            }

            if (source == ".")
            {
                return new byte[1];
            }

            var name = new StringBuilder();
            name.Append('\0');
            for (int i = 0, j = 0; i < source.Length; i++, j++)
            {
                name.Append(source[i]);
                if (source[i] == '.')
                {
                    name[i - j] = (char)(j & 0xff);
                    j = -1;
                }
            }

            name[name.Length - 1] = '\0';

            return Encoding.ASCII.GetBytes(name.ToString());
         }
    }
}


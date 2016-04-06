//
//  Copyright 2016  Dieter Lunn
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

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


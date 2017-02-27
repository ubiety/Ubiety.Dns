//
//  Copyright 2017 Dieter Lunn
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

using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Ubiety.Dns
{
    public static class DnsHelpers
    {
        public static byte[] CanonicaliseDnsName(string name, bool lowerCase)
        {
            if (!name.EndsWith("."))
            {
                name += ".";
            }

            if (name.Equals("."))
            {
                return new byte[1];
            }

            var newName = new StringBuilder();

            newName.Append('\0');

            for (int i = 0, j = 0; i < name.Length; i++, j++)
            {
                newName.Append(lowerCase ? char.ToLower(name[i]) : name[i]);

                if (name[i] != '.') continue;
                newName[i - j] = (char) (j & 0xff);
                j = -1;
            }

            newName[newName.Length - 1] = '\0';

            return Encoding.ASCII.GetBytes(newName.ToString());
        }

        public static IPAddressCollection DnsServerAddresses()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var networkInterface in networkInterfaces)
            {
                if ((networkInterface.OperationalStatus != OperationalStatus.Up) || (networkInterface.Speed <= 0) ||
                    (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                    (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel)) continue;
                var candidate = networkInterface.GetIPProperties().DnsAddresses;
                var found = false;
                foreach (var address in candidate)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    return candidate;
                }
            }

            return null;
        }
    }
}
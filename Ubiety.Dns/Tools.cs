using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Ubiety.Dns
{
    public class Tools
    {
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
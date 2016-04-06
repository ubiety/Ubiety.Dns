using System;
using System.Net;
using System.Net.NetworkInformation;
using Ubiety.Dns;
using Ubiety.Dns.Enums;

namespace DnsTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Interfaces: ");
			foreach (NetworkInterface info in NetworkInterface.GetAllNetworkInterfaces()) {
				Console.WriteLine (info.Name + " - " + info.OperationalStatus);
			}

			Console.WriteLine ();

			Console.WriteLine ("Dns Servers: ");
			foreach (IPEndPoint dns in Resolver.GetDnsServers()) {
				Console.WriteLine (dns);
			}

            Console.WriteLine();

            var resolver = new Resolver();
            Response response = resolver.Query(args[0], QueryType.A);

            Console.WriteLine("A Record for " + args[0] + ":");
            if (!string.IsNullOrEmpty(response.Error))
            {
                Console.WriteLine(response.Error);
            }

            foreach (var record in response.RecordsRR)
            {
                Console.WriteLine(record);
            }
		}
	}
}

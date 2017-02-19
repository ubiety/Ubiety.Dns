using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Ubiety.Dns.Enums;
using Ubiety.Dns.Query;

namespace DnsTest
{
    internal static class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Interfaces: ");
            foreach (var info in NetworkInterface.GetAllNetworkInterfaces())
            {
                Console.WriteLine(info.Name + " - " + info.OperationalStatus);
            }

            Console.WriteLine();

            var request = new DnsQueryRequest();
            var response = request.Resolve(args[0], QueryType.A, QueryClass.IN, ProtocolType.Tcp);

            Console.WriteLine($"A Record for {args[0]}:");

            foreach (var record in response.Answers)
            {
                Console.WriteLine(record);
            }

            Console.WriteLine();

            Console.WriteLine($"SRV Records for {args[0]}:");

            var srv = request.Resolve($"_xmpp-client._tcp.{args[0]}", QueryType.SRV, QueryClass.IN, ProtocolType.Tcp);

            foreach (var record in srv.Answers)
            {
                Console.WriteLine(record);
            }

            Console.ReadLine();
        }
    }
}
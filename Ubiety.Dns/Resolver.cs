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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using Ubiety.Dns.Enums;

namespace Ubiety.Dns
{
    /// <summary>
    /// Retrieves information from a DNS server based on the type of data requested.
    /// </summary>
    public class Resolver
    {
        private readonly List<IPEndPoint> _dnsServers;
        private readonly Dictionary<string, Response> _responseCache;
        private int _retries;
        private ushort _unique;

        /// <summary>
        /// Creates a new Resolver class.
        /// </summary>
        /// <param name="dnsServers">DNS servers to search</param>
        public Resolver(IEnumerable<IPEndPoint> dnsServers)
        {
            _responseCache = new Dictionary<string, Response>();
            _dnsServers = new List<IPEndPoint>();
            _dnsServers.AddRange(dnsServers);
            _unique = (ushort) new Random().Next();
            _retries = 3;
            Timeout = 1;
            Recursion = true;
            UseCache = true;
            TransportType = TransportType.Udp;
        }

        /// <summary>
        /// Creates a new Resolver class with default system DNS servers
        /// </summary>
        public Resolver() : this(GetDnsServers())
        {
        }

        /// <summary>
        /// Default DNS port
        /// </summary>
        public static int DefaultPort => 53;

        /// <summary>
        /// Library version
        /// </summary>
        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// DNS lookup timeout in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Number of times to retry DNS query
        /// </summary>
        public int Retries
        {
            get { return _retries; }

            set
            {
                if (value > 1)
                    _retries = value;
            }
        }

        /// <summary>
        /// Query domain recursively
        /// </summary>
        public bool Recursion { get; set; }

        /// <summary>
        /// Type of transport to use. Either TCP or UDP.
        /// </summary>
        public TransportType TransportType { get; set; }

        /// <summary>
        /// DNS servers to search
        /// </summary>
        public IPEndPoint[] DnsServers
        {
            get { return _dnsServers.ToArray(); }

            set
            {
                _dnsServers.Clear();
                _dnsServers.AddRange(value);
            }
        }

        /// <summary>
        /// Cache DNS results
        /// </summary>
        public bool UseCache { get; set; }

        /// <summary>
        /// Get default DNS servers from host system
        /// </summary>
        /// <returns>DNS server addresses from host</returns>
        public static IEnumerable<IPEndPoint> GetDnsServers()
        {
            var list = new List<IPEndPoint>();

            var adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var entry in from n in adapters
                where n.OperationalStatus == OperationalStatus.Up
                select n.GetIPProperties()
                into props
                from address in props.DnsAddresses
                select new IPEndPoint(address, DefaultPort)
                into entry
                where !list.Contains(entry)
                select entry)
            {
                list.Add(entry);
            }

            return list.ToArray();
        }

        private Response UdpRequest(Request request)
        {
            var responseMessage = new byte[512];

            for (var i = 0; i < _retries; i++)
            {
                foreach (var t in _dnsServers)
                {
                    using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                    {
                        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, Timeout * 1000);

                        try
                        {
                            socket.SendTo(request.Data, t);
                            var received = socket.Receive(responseMessage);
                            var data = new byte[received];
                            Array.Copy(responseMessage, data, received);
                            var response = new Response(t, data);
                            AddToCache(response);

                            return response;
                        }
                        finally
                        {
                            _unique++;
                        }
                    }
                }
            }

            var timeout = new Response {Error = "Timeout Error"};
            return timeout;
        }

        private Response TcpRequest(Request request)
        {
            for (var i = 0; i < _retries; i++)
            {
                foreach (var dnsServer in _dnsServers)
                {
                    using (var client = new TcpClient { ReceiveTimeout = Timeout * 1000 })
                    {
                        try
                        {
                            var result = client.BeginConnect(dnsServer.Address, dnsServer.Port, null, null);
                            var success = result.AsyncWaitHandle.WaitOne(Timeout * 1000, true);

                            if (!success || !client.Connected)
                            {
                                continue;
                            }

                            using (var stream = new BufferedStream(client.GetStream()))
                            {
                                var data = request.Data;
                                stream.WriteByte((byte)((data.Length >> 8) & 0xff));
                                stream.WriteByte((byte)(data.Length & 0xff));
                                stream.Write(data, 0, data.Length);
                                stream.Flush();

                                var transferResponse = new Response();
                                var soa = 0;
                                var messageSize = 0;

                                while (true)
                                {
                                    var length = stream.ReadByte() << 8 | stream.ReadByte();

                                    if (length <= 0)
                                    {
                                        throw new SocketException();
                                    }

                                    messageSize += length;

                                    var incomingData = new byte[length];
                                    stream.Read(incomingData, 0, length);
                                    var response = new Response(dnsServer, incomingData);

                                    if (response.Header.RCode != ResponseCode.NoError)
                                    {
                                        return response;
                                    }

                                    if (response.Questions[0].QType != QueryType.AXFR)
                                    {
                                        AddToCache(response);
                                        return response;
                                    }

                                    if (transferResponse.Questions.Count == 0)
                                    {
                                        transferResponse.Questions.AddRange(response.Questions);
                                    }
                                    transferResponse.Answers.AddRange(response.Answers);
                                    transferResponse.Authorities.AddRange(response.Authorities);
                                    transferResponse.Additionals.AddRange(response.Additionals);

                                    if (response.Answers[0].Type == DnsType.SOA)
                                    {
                                        soa++;
                                    }

                                    if (soa != 2) continue;
                                    transferResponse.Header.QuestionCount = (ushort)transferResponse.Questions.Count;
                                    transferResponse.Header.AnswerCount = (ushort)transferResponse.Answers.Count;
                                    transferResponse.Header.AuthorityCount = (ushort)transferResponse.Authorities.Count;
                                    transferResponse.Header.AdditionalsCount = (ushort)transferResponse.Additionals.Count;
                                    transferResponse.MessageSize = messageSize;
                                    return transferResponse;
                                }
                            }
                        }
                        finally
                        {
                            _unique++;
                        }
                    }
                }
            }

            var timeout = new Response {Error = "Timeout Error"};
            return timeout;
        }

        private Response GetResponse(Request request)
        {
            request.Header.Id = _unique;
            request.Header.RD = Recursion;

            switch (TransportType)
            {
                case TransportType.Udp:
                    return UdpRequest(request);
                case TransportType.Tcp:
                    return TcpRequest(request);
                case TransportType.All:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var response = new Response {Error = "Unknown Transport Type"};
            return response;
        }

        /// <summary>
        /// Query DNS server for information
        /// </summary>
        /// <param name="name">Domain name to lookup</param>
        /// <param name="qtype">Type of information requested</param>
        /// <param name="qclass">Class of information</param>
        /// <returns>DNS records applying to searched domain</returns>
        public Response Query(string name, QueryType qtype, QueryClass qclass)
        {
            var question = new Question(name, qtype, qclass);
            var response = SearchCache(question);
            if (response != null)
            {
                return response;
            }

            var request = new Request();
            request.AddQuestion(question);
            return GetResponse(request);
        }

        /// <summary>
        /// Query DNS server for information with class set to Internet
        /// </summary>
        /// <param name="name">Domain name to lookup</param>
        /// <param name="qtype">Type of information requested</param>
        /// <returns>DNS records applying to searched domain</returns>
        public Response Query(string name, QueryType qtype)
        {
            var question = new Question(name, qtype, QueryClass.IN);
            var response = SearchCache(question);
            if (response != null)
            {
                return response;
            }

            var request = new Request();
            request.AddQuestion(question);
            return GetResponse(request);
        }

        #region Cache Methods

        /// <summary>
        /// Clears internal cache
        /// </summary>
        public void ClearCache()
        {
            lock (_responseCache)
            {
                _responseCache.Clear();
            }
        }

        private Response SearchCache(Question question)
        {
            if (!UseCache)
                return null;

            var key = question.QClass + "-" + question.QType + "-" + question.QName;

            Response response;

            lock (_responseCache)
            {
                if (!_responseCache.ContainsKey(key))
                    return null;

                response = _responseCache[key];
            }

            var timeLived = (int) ((DateTime.Now.Ticks - response.Timestamp.Ticks)/TimeSpan.TicksPerSecond);

            foreach (var rr in response.AllResourceRecords)
            {
                rr.TimeLived = timeLived;
                if (rr.TTL == 0)
                    return null;
            }

            return response;
        }

        private void AddToCache(Response response)
        {
            if (!UseCache)
                return;

            if (response.Questions.Count == 0)
                return;

            if (response.Header.RCode != ResponseCode.NoError)
                return;

            var question = response.Questions[0];

            var key = question.QClass + "-" + question.QType + "-" + question.QName;

            lock (_responseCache)
            {
                if (_responseCache.ContainsKey(key))
                    _responseCache.Remove(key);

                _responseCache.Add(key, response);
            }
        }

        #endregion
    }
}
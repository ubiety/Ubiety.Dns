using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using Ubiety.Dns.Enums;
using System.Net.Sockets;
using System.IO;

namespace Ubiety.Dns
{
	public class Resolver
	{
		ushort _unique;
		int _retries;
		readonly List<IPEndPoint> _dnsServers;
		readonly Dictionary<string, Response> _responseCache;

		public string Version
		{
			get 
			{
				return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public int Timeout 
		{
			get;
			set;
		}

		public int Retries 
		{
			get 
			{
				return _retries;
			}

			set 
			{
				if (value > 1)
					_retries = value;
			}
		}

		public bool Recursion 
		{
			get;
			set;
		}

		public TransportType TransportType 
		{
			get;
			set;
		}

		public IPEndPoint[] DnsServers 
		{
			get 
			{
				return _dnsServers.ToArray ();
			}

			set 
			{
				_dnsServers.Clear ();
				_dnsServers.AddRange (value);
			}
		}

		public bool UseCache 
		{
			get;
			set;
		}

		public const int DefaultPort = 53;

		public Resolver (IPEndPoint[] dnsServers)
		{
			_responseCache = new Dictionary<string, Response> ();
			_dnsServers = new List<IPEndPoint> ();
			_dnsServers.AddRange (dnsServers);
			_unique = (ushort)(new Random ()).Next ();
			_retries = 3;
			Timeout = 1;
			Recursion = true;
			UseCache = true;
			TransportType = TransportType.Udp;
		}

		public Resolver() : this(GetDnsServers()) 
		{
		}

		public static IPEndPoint[] GetDnsServers() 
		{
			var list = new List<IPEndPoint> ();

			NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces ();
			foreach (NetworkInterface n in adapters) 
			{
				if (n.OperationalStatus == OperationalStatus.Up) 
				{
					IPInterfaceProperties props = n.GetIPProperties ();
					foreach (IPAddress address in props.DnsAddresses) 
					{
						var entry = new IPEndPoint (address, DefaultPort);
						if (!list.Contains (entry))
							list.Add (entry);
					}
				}
			}

			return list.ToArray ();
		}

        #region Cache Methods

		public void ClearCache() 
		{
			_responseCache.Clear ();
		}

        Response SearchCache(Question question)
        {
            if (!UseCache)
                return null;

            string key = question.QClass + "-" + question.QType + "-" + question.QName;

            Response response;

            lock (_responseCache)
            {
                if (!_responseCache.ContainsKey(key))
                    return null;

                response = _responseCache[key];
            }

            int timeLived = (int)((DateTime.Now.Ticks - response.Timestamp.Ticks) / TimeSpan.TicksPerSecond);

            foreach (RR rr in response.RecordsRR)
            {
                rr.TimeLived = timeLived;
                if (rr.TTL == 0)
                    return null;
            }

            return response;
        }

        void AddToCache(Response response)
        {
            if (!UseCache)
                return;

            if (response.Questions.Count == 0)
                return;

            if (response.Header.RCode != ResponseCode.NoError)
                return;

            Question question = response.Questions[0];

            string key = question.QClass + "-" + question.QType + "-" + question.QName;

            lock (_responseCache)
            {
                if (_responseCache.ContainsKey(key))
                    _responseCache.Remove(key);

                _responseCache.Add(key, response);
            }
        }

        #endregion

        Response UdpRequest(Request request)
        {
            var responseMessage = new byte[512];

            for (int i = 0; i < _retries; i++)
            {
                for (int j = 0; j < _dnsServers.Count; j++) 
                {
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, Timeout * 1000);

                    try
                    {
                        socket.SendTo(request.Data, _dnsServers[j]);
                        int received = socket.Receive(responseMessage);
                        var data = new byte[received];
                        Array.Copy(responseMessage, data, received);
                        var response = new Response(_dnsServers[j], data);
                        AddToCache(response);

                        return response;
                    }
                    catch (SocketException)
                    {
                        continue;
                    }
                    finally
                    {
                        _unique++;
                        socket.Close();
                    }
                }
            }

            var timeout = new Response();
            timeout.Error = "Timeout Error";
            return timeout;
        }

        Response TcpRequest(Request request)
        {
            for (int i = 0; i < _retries; i++)
            {
                for (int j = 0; j < _dnsServers.Count; j++) 
                {
                    var client = new TcpClient();
                    client.ReceiveTimeout = Timeout * 1000;

                    try
                    {
                        IAsyncResult result = client.BeginConnect(_dnsServers[j].Address, _dnsServers[j].Port, null, null);
                        bool success = result.AsyncWaitHandle.WaitOne(Timeout*1000, true);

                        if (!success || !client.Connected) 
                        {
                            client.Close();
                            continue;
                        }

                        var stream = new BufferedStream(client.GetStream());

                        byte[] data = request.Data;
                        stream.WriteByte((byte)((data.Length >> 8) & 0xff));
                        stream.WriteByte((byte)(data.Length & 0xff));
                        stream.Write(data, 0, data.Length);
                        stream.Flush();

                        var transferResponse = new Response();
                        int soa = 0;
                        int messageSize = 0;

                        while (true) 
                        {
                            int length = stream.ReadByte() << 8 | stream.ReadByte();

                            if (length <= 0) 
                            {
                                client.Close();
                                throw new SocketException();
                            }

                            messageSize += length;

                            var incomingData = new byte[length];
                            stream.Read(incomingData, 0, length);
                            var response = new Response(_dnsServers[j], incomingData);

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

                            if (response.Answers[0].Type == DnsType.SOA) {
                                soa++;
                            }

                            if (soa == 2) {
                                transferResponse.Header.qdCount = (ushort)transferResponse.Questions.Count;
                                transferResponse.Header.anCount = (ushort)transferResponse.Answers.Count;
                                transferResponse.Header.nsCount = (ushort)transferResponse.Authorities.Count;
                                transferResponse.Header.arCount = (ushort)transferResponse.Additionals.Count;
                                transferResponse.MessageSize = messageSize;
                                return transferResponse;
                            }
                        }
                    }
                    catch (SocketException)
                    {
                        continue;
                    }
                    finally
                    {
                        _unique++;
                        client.Close();
                    }
                }
            }

            var timeout = new Response();
            timeout.Error = "Timeout Error";
            return timeout;
        }

        Response GetResponse(Request request)
        {
            request.Header.Id = _unique;
            request.Header.RD = Recursion;

            if (TransportType == TransportType.Udp)
            {
                return UdpRequest(request);
            }

            if (TransportType == TransportType.Tcp)
            {
                return TcpRequest(request);
            }

            var response = new Response();
            response.Error = "Unknown Transport Type";
            return response;
        }

        public Response Query(string name, QueryType qtype, QueryClass qclass)
        {
            var question = new Question(name, qtype, qclass);
            Response response = SearchCache(question);
            if (response != null)
            {
                return response;
            }

            var request = new Request();
            request.AddQuestion(question);
            return GetResponse(request);
        }

        public Response Query(string name, QueryType qtype)
        {
            var question = new Question(name, qtype, QueryClass.IN);
            Response response = SearchCache(question);
            if (response != null)
            {
                return response;
            }

            var request = new Request();
            request.AddQuestion(question);
            return GetResponse(request);
        }
	}
}


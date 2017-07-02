using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Hydrozoa_CLI 
{
	public class TCPClient : ITCPClient
	{
		protected string Host { get; }
		protected Int32 Port { get; }

		public TCPClient() 
		{
			Host = ConnectionTarget.Host;
			Port = ConnectionTarget.Port;
		}

		public async Task<IList<byte>> ExchangeData(Byte[] data, Int32 buffer_size = 1024, Int32 max_retry = 3, Int32 delta = 10) 
		{
			for (int i = 0; i < max_retry; i++) {
				using (TcpClient client = new TcpClient()) {
					await client.ConnectAsync(Host, Port);
					using (NetworkStream stream = client.GetStream()) {
						try {
							if (!stream.CanWrite) { throw new HostUnreachableException(); } else {
								stream.Write(data, 0, data.Length);
								if (!stream.CanRead) { throw new HostUnreachableException(); } else {
									List<byte> answer = new List<byte>();
									byte[] buffer = new byte[buffer_size];
									while (stream.DataAvailable) {
										stream.Read(buffer, 0, buffer.Length);
										answer.AddRange(buffer);
									}
									return answer;
								}
							}
						} catch(Exception ex) {
							BasicOutputs.Error(ex);
							await Task.Delay(delta);
							delta *= 2;
						}
					}
				}
			}
			return null;
		}
	}

	public class HostUnreachableException : Exception
	{
	    public HostUnreachableException() { }

	    public HostUnreachableException(string message) : base(message) { }

	    public HostUnreachableException(string message, Exception inner) : base(message, inner) { }
	}
}
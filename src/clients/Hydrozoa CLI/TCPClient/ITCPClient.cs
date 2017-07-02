using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Hydrozoa_CLI 
{
	public interface ITCPClient 
	{
		Task<IList<byte>> ExchangeData(Byte[] data, Int32 buffer_size = 1024, Int32 max_retry = 3, Int32 delta = 10);
	}
}
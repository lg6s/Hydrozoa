using System;
using System.Collections.Generic;

namespace Hydrozoa_CLI 
{
	public interface IHydrozoaCmd 
	{
		void ls(IList<string> data);

		void defj(IList<string> data);

		void mkj(IList<string> data);

		void ssj(IList<string> data);

		void rmj(IList<string> data);

		void rmt(IList<string> data);

		void gnt();

		void help();
	}
}
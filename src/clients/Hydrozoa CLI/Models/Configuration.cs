using System;
using System.Collections.Generic;

namespace Hydrozoa_CLI
{
	public static class ConnectionTarget {
		public static string Host { get; set; }
		public static Int32 Port { get; set; }
	}

	public static class AppSettings {
		public static string BaseDir { get; set; }
		public static IList<string> HelpTXT { get; set; }
	}
}
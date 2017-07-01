using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrozoa_CLI
{
	using static Console;
	public class Bash 
	{
		protected readonly IHydrozoaCmd _HC;

		public Bash(IHydrozoaCmd HC) 
		{
			_HC = HC;
		}

		public void Process() 
		{
			IList<string> data;
			while(true) {
				data = BasicInteractions.Request().Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
				switch(data[0]) {
					case "ls":
						_HC.ls(data); 
						break;
					case "defj": 
						_HC.defj(data);
						break;
					case "mkj":
						_HC.mkj(data); 
						break;
					case "ssj": 
						_HC.ssj(data);
						break;
					case "rmj": 
						_HC.rmj(data);
						break;
					case "gnt": 
						_HC.gnt();
						break;
					case "rmt": 
						_HC.rmt(data);
						break;
					case "help":
						_HC.help();
						break;
                    case "\\\\":
                        return;
					default:
						if (data[0] != string.Empty) 
						{ BasicOutputs.Error("unknown command; check for typos or type help"); }
						break;
				}
			}
		}
	}

	public static class BasicInteractions 
    {
        public static string Request(ref string data, string prmpt = null) 
        {
            Write(string.Concat(prmpt, string.IsNullOrEmpty(prmpt) ? "" : " ", ">> "));
            data = ReadLine();
            return data;
        }

        public static string Request(string prmpt = null) 
        {
            Write(string.Concat(prmpt, string.IsNullOrEmpty(prmpt) ? "" : " ", ">> "));
            return ReadLine();
        }

        public static bool ConfirmationRequest(string prmpt) 
        {
            do {
                Write(string.Concat(prmpt, " [Y/n] >> "));
                switch (ReadLine()) {
                    case "Y": return true;
                    case "n": return false;
                    default: 
                        BasicOutputs.Error("Please choose from the given options..."); 
                        break;
                }
            } while (true);
        }
    }

    public static class BasicOutputs 
    {
#region StatusOutput
        public static void Error(string err) 
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine(err);
            ResetColor();
        }

        public static void Error(IList<string> err) 
        {
        	ForegroundColor = ConsoleColor.Red;
        	Output(err);
        	ResetColor();
        }

        public static void Error(Exception ex) 
        {
            ForegroundColor = ConsoleColor.Red;
            if (ex.InnerException != null) { WriteLine(ex.InnerException.Message); }
            WriteLine(ex.Message);
            ResetColor();
        }
#endregion

        public static void Output(IList<string> ls) 
        {
            foreach (string n in ls) { WriteLine(n); }
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Hydrozoa_CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
        	string BaseDir = @"/home/mxar/Documents/Projects/Hydrozoa/src/clients/Hydrozoa CLI/"; //AppDomain.CurrentDomain.BaseDirectory;
        	AppSettings.BaseDir = BaseDir;

        	string path = string.Concat(BaseDir, "resources/greetings.txt");
	    	if (File.Exists(path)) {
	    		BasicOutputs.Output(File.ReadLines(path).ToList<string>());
	    	} else { throw new FileNotFoundException(); }

	    	path = string.Concat(BaseDir, "resources/target_node.json");
	    	if (File.Exists(path)) {
	    		JObject config = JObject.Parse(File.ReadAllText(path));
	    		ConnectionTarget.Host = (string)config["AccessNode"]["Host"];
	    		ConnectionTarget.Port = (Int32)config["AccessNode"]["Port"];
	    	} else { throw new FileNotFoundException(); }

	    	path = string.Concat(BaseDir, "resources/hlp.txt"); 
	    	if (File.Exists(path)) {
	    		AppSettings.HelpTXT = File.ReadLines(path).ToList<string>();
	    	} else { throw new FileNotFoundException(); }

        	Bash B = new Bash((IHydrozoaCmd)(new HydrozoaCmd()));
        	B.Process();
	    }
    }
}
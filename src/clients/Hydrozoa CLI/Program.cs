using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Hydrozoa_CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
        	string BaseDir = @"/home/mxar/Documents/Projects/Hydrozoa/src/clients/Hydrozoa CLI/"; //AppDomain.CurrentDomain.BaseDirectory;
        	Greetings(string.Concat(BaseDir, "resources/greetings.txt"));
	    }

	    internal static void Greetings(string path) {
	    	if (File.Exists(path)) {
	    		BasicOutputs.Output(File.ReadLines(path).ToList<string>());
	    	} else { throw new FileNotFoundException(); }
	    }
    }
}
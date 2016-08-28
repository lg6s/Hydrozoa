using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace WAMS.Services
{
    public static class LogManagement
    {
        private static string LogDirectory { get; set; }
        private static Timer FlushPeriod { get; set; }
        private static ILogger _logger { get; set; }
        public static void Setup(ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            _logger = loggerFactory.CreateLogger("WAMS.LogManagement");

            string ph = "----------------------";
            for (int i = 0; i < 3; i++) { _logger.LogInformation(ph); }
            _logger.LogInformation("Application start went succesfully");
            _logger.LogInformation(">>>> Application : {0}", env.ApplicationName);
            _logger.LogInformation(">>>> Enviroment : {0}", env.EnvironmentName);
            _logger.LogInformation(">>>> root-path : {0}", env.ContentRootPath);
            for (int i = 0; i < 3; i++) { _logger.LogInformation(ph); }

            LogDirectory = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "Logs\\");

            FlushPeriod = new Timer(864000000);
            FlushPeriod.Elapsed += FlushLogsEvent;
            FlushPeriod.AutoReset = true;
            FlushPeriod.Enabled = true;

        }

        private static async void FlushLogsEvent(object sender, ElapsedEventArgs e) { await FlushLogs(LogDirectory); }

        private static async Task FlushLogs(string TempDirectory)
        {
            try{
                if (Directory.Exists(TempDirectory)) {
                    List<string> Files = Directory.GetFiles(TempDirectory).ToList();
                    List<string> SubDirectories = Directory.GetDirectories(TempDirectory).ToList();

#pragma warning disable 4014
                    if (Files.Any()){ Files.ForEach(e => File.Delete(e)); }
                    if (SubDirectories.Any()) { Parallel.ForEach(SubDirectories, e => FlushLogs(e)); }
#pragma warning restore 4014
                }
            } catch (IOException ex) { _logger.LogCritical(ex.Message); }
            await Task.FromResult(0);
        }
    }
}

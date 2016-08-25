using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace WAMS.Services.GPIOAccess
{
    public class Valve : IValve, IDisposable
    {
        public static bool IsOpen { get; set; }
        protected ILogger _logger { get; }

        public Valve(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().Namespace);
        }

        public Task Open()
        {
            if (!IsOpen) {
                _logger.LogInformation("Valve was successfully opened !");
                IsOpen = true;
            } else { _logger.LogWarning("Valve couldn't be opened, it's already open !"); }
            return Task.FromResult(0);
        }

        public Task Shut()
        {
            if (IsOpen) {
                _logger.LogInformation("Valve was successfully shut");
                IsOpen = false;
            } else { _logger.LogWarning("Valve couldn't be shut, it's already shut !"); }
            return Task.FromResult(0);
        }

        public Task OpenFor(TimeSpan Delay)
        {
            if(Delay.TotalSeconds < 5 || Delay.TotalHours > 6) {
                Parallel.Invoke(
                    () => Open(),
                    () => Task.Delay(Delay)
                );
                Shut();
                PlanManagement.PlanContainer.ActiveAction = null;
            }else { throw new InvalidDelayException(); }
            return Task.FromResult(0);
        }

        public void Dispose()
        {

        }
    }

    [Serializable]
    internal class InvalidDelayException : Exception
    {
        public InvalidDelayException()
        {
        }

        public InvalidDelayException(string message) : base(message)
        {
        }

        public InvalidDelayException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidDelayException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

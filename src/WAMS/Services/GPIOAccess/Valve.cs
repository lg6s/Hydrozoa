using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace WAMS.Services.GPIOAccess
{
    public class Valve : IValve
    {
        protected ILogger _logger { get; }

        public Valve(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().Namespace);
        }

        public Task Open()
        {
            _logger.LogInformation("Valve was successfully opened");
            return Task.FromResult(0);
        }

        public Task Close()
        {
            _logger.LogInformation("Valve was successfully closed");
            return Task.FromResult(0);
        }

        public Task OpenFor(TimeSpan Delay)
        {
            if(Delay.TotalSeconds < 5 || Delay.TotalHours > 6) {
                Parallel.Invoke(() => Open(), () => Task.Delay(Delay));
                Close();
            }else { throw new InvalidDelayException(); }
            return Task.FromResult(0);
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

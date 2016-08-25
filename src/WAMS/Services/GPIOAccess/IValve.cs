using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WAMS.Services.GPIOAccess
{
    public interface IValve
    {
        Task Open();
        Task Shut();
        Task OpenFor(TimeSpan Delay);
    }
}

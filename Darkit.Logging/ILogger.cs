using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Darkit.Logging
{
    public interface ILogger
    {
        void Record(string level, string message);
        void Flush();
    }
}

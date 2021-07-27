using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Darkit.Logging.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Log.Init();
                Log.Info("info: {0}", 12354);
                Log.Warn("warn: {0}", 32141235);
            }
            finally
            {
                Log.Quit();
            }
        }
    }
}

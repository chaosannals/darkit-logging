using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;

namespace Darkit.Logging.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            string payload = Encoding.UTF8.GetString(new byte[5000].Select((j, i) => (byte)((i % 26) + 65)).ToArray());
            try
            {
                Log.Init();

                new Thread(() =>
                {
                    for(int i = 0; i <= 1000; ++i)
                    {
                        Log.Info("info: {0} {1}", i, payload);
                        Thread.Sleep(100);
                    }
                }).Start();

                new Thread(() =>
                {
                    for (int i = 0; i <= 1000; ++i)
                    {
                        Log.Warn("warn: {0} {1}", i, payload);
                        Thread.Sleep(100);
                    }
                }).Start();
                
                for(int i =0; i <= 1000; ++i)
                {
                    Log.Info("tick {0}", i);
                    Console.WriteLine("tick: {0}", i);
                    Thread.Sleep(200);
                }
            }
            finally
            {
                Log.Quit();
            }
        }
    }
}

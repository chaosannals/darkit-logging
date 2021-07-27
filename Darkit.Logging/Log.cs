using System;
using System.Collections.Generic;
using System.Timers;
using System.Text;

namespace Darkit.Logging
{
    public static class Log
    {
        public static Timer Ticker { get; private set; }
        public static ILogger Logger { get; private set; }
        public static Dictionary<string, ILogger> Loggers { get; private set; }

        public static void Init(ILogger defaultLogger=null)
        {
            Ticker = new Timer();
            Ticker.Interval = 2000;
            Ticker.Elapsed += (s, args) =>
            {
                try { Flush(); }
                catch (Exception e)
                {
                    Logger.Record("danger", e.Message + e.StackTrace);
                }
            };
            Logger = defaultLogger ?? new DefaultLogger();
            Loggers = new Dictionary<string, ILogger>();
            Ticker.Start();
        }

        public static void Quit()
        {
            Ticker.Stop();
            Flush();
        }

        public static void Info(string message, params object[] args)
        {
            Record("info", message, args);
        }

        public static void Warn(string message, params object[] args)
        {
            Record("warn", message, args);
        }

        public static void Error(string message, params object[] args)
        {
            Record("error", message, args);
        }

        public static void Record(string level, string message, params object[] args)
        {
            string text = string.Format(message, args);
            lock (Loggers)
            {
                if (Loggers.ContainsKey(level))
                {
                    Loggers[level].Record(level, text);
                }
                else
                {
                    Logger.Record(level, text);
                }
            }
        }

        public static void Flush()
        {
            try
            {
                lock (Loggers)
                {
                    foreach (KeyValuePair<string, ILogger> pair in Loggers)
                    {
                        pair.Value.Flush();
                    }
                }
            }
            finally
            {
                lock(Logger)
                {
                    Logger.Flush();
                }
            }
        }
    }
}

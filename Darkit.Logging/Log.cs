using System;
using System.Collections.Generic;
using System.Timers;
using System.Text;

namespace Darkit.Logging
{
    /// <summary>
    /// 日志
    /// </summary>
    public static class Log
    {
        public static Timer Ticker { get; private set; }
        public static ILogger Logger { get; private set; }
        public static Dictionary<string, ILogger> Loggers { get; private set; }

        /// <summary>
        /// 初始化日志。
        /// </summary>
        /// <param name="defaultLogger"></param>
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

        /// <summary>
        /// 停止日志采集，并落盘。
        /// </summary>
        public static void Quit()
        {
            Ticker.Stop();
            Flush();
        }

        /// <summary>
        /// 信息日志。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Info(string message, params object[] args)
        {
            Record("info", message, args);
        }

        /// <summary>
        /// 警告日志。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Warn(string message, params object[] args)
        {
            Record("warn", message, args);
        }

        /// <summary>
        /// 错误日志。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Error(string message, params object[] args)
        {
            Record("error", message, args);
        }

        /// <summary>
        /// 写入日志。
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
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

        /// <summary>
        /// 日志落盘。
        /// </summary>
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

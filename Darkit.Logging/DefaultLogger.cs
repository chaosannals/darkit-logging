using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Darkit.Logging
{
    /// <summary>
    /// 默认日志器
    /// </summary>
    public class DefaultLogger : ILogger
    {
        public int Size { get; private set; }
        public string Folder { get; private set; }
        public string Suffix { get; private set; }
        public List<string> Contents { get; private set; }
        public ILogCleaner Cleaner { get; private set; }
        public TimeSpan CleanSpan { get; private set; }
        public DateTime CleanTime { get; private set; }

        /// <summary>
        /// 初始化。
        /// </summary>
        /// <param name="suffix"></param>
        /// <param name="folder"></param>
        /// <param name="size"></param>
        /// <param name="cleaner"></param>
        public DefaultLogger(string suffix = "log", string folder=null, int size = 2000000, ILogCleaner cleaner=null, TimeSpan? cleanSpan=null)
        {
            Size = size;
            Suffix = suffix;
            Folder = Folder ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
            Contents = new List<string>();
            Cleaner = cleaner ?? new DefaultLogCleaner(Folder, Suffix);
            CleanSpan = cleanSpan ?? TimeSpan.FromDays(1);
            CleanTime = DateTime.Now;
        }

        /// <summary>
        /// 写入日志到内存。
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Record(string level, string message)
        {
            lock (Contents)
            {
                string date = DateTime.Now.ToString("F");
                Contents.Add(string.Format("{0:S} [{1:S}] - {2:S}\r\n", level.ToUpper(), date, message));
            }
        }

        /// <summary>
        /// 落盘刷新。
        /// </summary>
        public void Flush()
        {
            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }

            lock (Contents)
            {
                if (Contents.Count == 0) return;
                string date = DateTime.Now.ToString("yyyyMMdd");
                string name = string.Format("{0:S}.{1}", date, Suffix);
                string path = Path.Combine(Folder, name);

                // 大于 2M 的文件先搬移
                FileInfo info = new FileInfo(path);
                if (info.Exists && info.Length > Size)
                {
                    string time = DateTime.Now.ToString("HHmmss");
                    info.MoveTo(Path.Combine(Folder, string.Format("{0:S}-{1:S}.{2}", date, time, Suffix)));
                }

                // 写入日志
                using (FileStream stream = File.Open(path, FileMode.OpenOrCreate | FileMode.Append))
                {
                    foreach (string text in Contents)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(text);
                        stream.Write(data, 0, data.Length);
                    }
                    Contents.Clear();
                }

                // 日志清理
                if (CleanTime <= DateTime.Now)
                {
                    Cleaner.Clear();
                    CleanTime = DateTime.Now + CleanSpan;
                }
            }
        }
    }
}

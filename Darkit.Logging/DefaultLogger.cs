using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Darkit.Logging
{
    public class DefaultLogger : ILogger
    {
        public int Size { get; private set; }
        public string Folder { get; private set; }
        public string Suffix { get; private set; }
        public List<string> Contents { get; private set; }

        public DefaultLogger(string suffix = "log", string folder=null, int size = 2000000)
        {
            Size = size;
            Suffix = suffix;
            Folder = Folder ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
            Contents = new List<string>();
        }

        public void Record(string level, string message)
        {
            lock (Contents)
            {
                string date = DateTime.Now.ToString("F");
                Contents.Add(string.Format("{0:S} [{1:S}] - {2:S}\r\n", level.ToUpper(), date, message));
            }
        }

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
            }
        }
    }
}

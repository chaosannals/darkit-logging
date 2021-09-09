using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Darkit.Logging
{
    /// <summary>
    /// 日志清理器
    /// </summary>
    public class DefaultLogCleaner : ILogCleaner
    {
        public string Folder { get; private set; }
        public string Suffix { get; private set; }
        public TimeSpan KeepSpan { get; private set; }
        public DefaultLogCleaner(string folder, string suffix="log", TimeSpan? keepSpan=null)
        {
            Folder = folder;
            Suffix = suffix;
            KeepSpan = keepSpan ?? TimeSpan.FromDays(6);
        }

        /// <summary>
        /// 清理日志。
        /// </summary>
        public void Clear()
        {
            if (Directory.Exists(Folder))
            {
                DateTime limit = DateTime.Now - KeepSpan;
                foreach(string fn in Directory.GetFiles(Folder, $"*.{Suffix}"))
                {
                    if (File.GetLastWriteTime(fn) < limit)
                    {
                        File.Delete(fn);
                    }
                }
            }
        }
    }
}

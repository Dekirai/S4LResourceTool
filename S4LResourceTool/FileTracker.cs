using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace S4LResourceTool
{
    internal class FileTracker : IEnumerable<KeyValuePair<string, TrackData>>, IEnumerable
    {
        public event EventHandler<KeyValuePair<string, TrackData>> OnChange;

        public FileTracker()
        {
            this._fileWatcher = new Dictionary<string, TrackData>();
            Task.Run(delegate ()
            {
                for (; ; )
                {
                    foreach (KeyValuePair<string, TrackData> e in this._fileWatcher)
                    {
                        string key = e.Key;
                        TrackData value = e.Value;
                        DateTime lastWriteTime = File.GetLastWriteTime(key);
                        if (!(value.LastModified == lastWriteTime))
                        {
                            this.OnChange(this, e);
                            value.LastModified = lastWriteTime;
                        }
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        public void Add(string path, TrackData data)
        {
            this._fileWatcher.Add(path, data);
        }

        public IEnumerator<KeyValuePair<string, TrackData>> GetEnumerator()
        {
            return this._fileWatcher.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._fileWatcher.GetEnumerator();
        }

        // Token: 0x04000018 RID: 24
        private readonly Dictionary<string, TrackData> _fileWatcher;
    }
}

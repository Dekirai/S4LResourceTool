using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoNetsphere.Resource;

namespace S4LResourceTool
{
    public class TrackData
    {
        public S4ZipEntry Entry { get; set; }
        public DateTime LastModified { get; set; }
        public TrackData(S4ZipEntry entry, DateTime lastModified)
        {
            Entry = entry;
            LastModified = lastModified;
        }
    }
}

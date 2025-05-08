using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoNetsphere.Resource;

namespace S4LResourceTool
{
    internal class FileWalker
    {
        private readonly S4Zip _zipFile;
        private readonly List<object> _selectedObjects;
        private readonly string _path;

        public FileWalker(S4Zip zip, List<object> selectedObjects, string treePath)
        {
            _zipFile = zip;
            _selectedObjects = selectedObjects;
            _path = treePath;
        }

        public List<FileData> GenerateList()
        {
            var list = new List<FileData>();
            foreach (var tag in _selectedObjects)
            {
                list.AddRange(GenerateList(tag, _path));
            }
            return list;
        }

        public IEnumerable<FileData> GenerateList(object tag, string currentPath)
        {
            if (tag is S4ZipEntry s4ZipEntry)
            {
                yield return GenerateList(s4ZipEntry);
            }
            else if (tag is string text)
            {
                foreach (var fileData in GenerateList(text))
                    yield return fileData;
            }

            yield break;
        }

        public IEnumerable<FileData> GenerateList(string path)
        {
            foreach (var file in _zipFile.GetFiles(path, true))
            {
                var extension = Path.GetExtension(file);
                bool isDirectory = extension != null && extension.Length == 0;
                bool entryMissing = _zipFile.Values.All(x => x.FullName != file);

                if (isDirectory && entryMissing)
                {
                    foreach (var fd in GenerateList(path + "/" + file))
                        yield return fd;
                }
                else
                {
                    var entry = _zipFile.Values.FirstOrDefault(x => x.FullName == file);
                    if (entry != null)
                        yield return GenerateList(entry);
                }
            }

            yield break;
        }

        public FileData GenerateList(S4ZipEntry entry)
        {
            return new FileData
            {
                Path = entry.FullName.Substring(_path.Length == 0 ? 0 : _path.Length + 1),
                Entry = entry
            };
        }

        public class FileData
        {
            public string Path { get; set; }
            public S4ZipEntry Entry { get; set; }
        }
    }
}

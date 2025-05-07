using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoNetsphere.Resource;

namespace S4LResourceTool
{
    internal static class ResourceToolExtensions
    {
        public static string GetFolderName(this S4ZipEntry entry, bool fullPath = false)
        {
            return entry.FullName.GetFolderName(fullPath);
        }

        public static string GetFolderName(this string path, bool fullPath = false)
        {
            if (path.Count((char x) => x == '/') == 0)
            {
                return "";
            }
            string text = path.Substring(0, path.LastIndexOf('/'));
            if (!fullPath && text.IndexOf('/') != -1)
            {
                text = text.Substring(text.LastIndexOf('/') + 1);
            }
            return text;
        }

        public static IEnumerable<string> GetFolders(this S4Zip zip)
        {
            List<string> list = new List<string>();
            foreach (S4ZipEntry entry in zip.Values)
            {
                string folderName = entry.GetFolderName(true);
                if (!list.Contains(folderName))
                {
                    list.Add(folderName);
                }
            }
            return (from x in list
                    orderby x
                    select x).ToList<string>();
        }
        public static IEnumerable<string> GetFiles(this S4Zip zip, string path, bool includeFolders = false)
        {
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            foreach (S4ZipEntry s4ZipEntry in zip.Values)
            {
                string folderName = s4ZipEntry.GetFolderName(true);
                if (folderName.StartsWith(path))
                {
                    int num = s4ZipEntry.FullName.Substring(path.Length + 1).Count((char x) => x == '/');
                    string text = folderName.Substring(path.Length);
                    if (text.Length > 0 && text.First<char>() == '/')
                    {
                        text = text.Substring(1);
                    }
                    if (text.IndexOf('/') != -1)
                    {
                        text = text.Substring(0, text.IndexOf('/'));
                    }
                    if (num <= 0 || includeFolders)
                    {
                        List<string> list3 = (num > 0) ? list2 : list;
                        string item = (num > 0) ? text : s4ZipEntry.FullName;
                        if (!list3.Contains(item))
                        {
                            list3.Add(item);
                        }
                    }
                }
            }
            list = (from x in list
                    orderby x
                    select x).ToList<string>();
            list2 = (from x in list2
                     orderby x
                     select x).ToList<string>();
            list2.AddRange(list);
            return list2;
        }
        public static IEnumerable<List<T>> GetChunks<T>(this List<T> list, int chunkCount)
        {
            int chunkSize = list.Count / chunkCount;
            for (int i = 0; i < list.Count; i += chunkSize)
            {
                yield return list.GetRange(i, Math.Min(chunkSize, list.Count - i));
            }
            yield break;
        }
    }
}

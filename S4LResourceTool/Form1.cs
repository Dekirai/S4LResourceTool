using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NeoNetsphere.Resource;
using Org.BouncyCastle.Crypto.Tls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace S4LResourceTool
{
    public partial class Form1 : Form
    {
        public static string _currentPath = "";
        public static S4Zip _zipFile = S4Zip.OpenZip("E:\\Games\\S4 Legacy\\resource.s4hd");
        public static List<string> _extraFolders = new List<string>();
        public static List<string> _deletedItems = new List<string>();
        private static readonly Dictionary<string, ExtensionType> _extensions = new Dictionary<string, ExtensionType>
    {
        {
            ".dds",
            ExtensionType.Image
        },
        {
            ".tga",
            ExtensionType.Image
        },
        {
            ".jpg",
            ExtensionType.Image
        },
        {
            ".png",
            ExtensionType.Image
        },
        {
            ".bmp",
            ExtensionType.Image
        },
        {
            ".txt",
            ExtensionType.Text
        },
        {
            ".x7",
            ExtensionType.Text
        },
        {
            ".xml",
            ExtensionType.Text
        },
        {
            ".ini",
            ExtensionType.Text
        }
    };


        FileTracker _fileTracker = new FileTracker();
        public Form1()
        {
            InitializeComponent();
            //_fileTracker.OnChange += delegate (object sender, KeyValuePair<string, TrackData> kv)
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PopulateView();
        }

        private void PopulateView()
        {
            var zipFile = S4Zip.OpenZip("E:\\Games\\S4 Legacy\\resource.s4hd");
            this.listView1.Items.Clear();
            if (this.searchBox.Text.Length <= 2 || this.searchBox.Text == "Search for an item...")
            {
                this.listView1.Items.Add(new ListViewItem("..")
                {
                    ImageKey = "folder"
                });
                List<string> list = zipFile.GetFiles(_currentPath, true).ToList<string>();
                List<string> list2 = new List<string>();
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    string text = list[i];
                    string extension = Path.GetExtension(text);
                    if (extension == null || extension.Length != 0)
                    {
                        break;
                    }
                    i--;
                    list.RemoveAt(0);
                    list2.Add(text);
                    if (list.Count == 0)
                    {
                        break;
                    }
                }
                foreach (string text2 in _extraFolders)
                {
                    if (text2.StartsWith(_currentPath) && text2.Length > _currentPath.Length && text2.Substring((_currentPath.Length == 0) ? 0 : (_currentPath.Length + 1)).IndexOf('/') == -1)
                    {
                        list2.Add(text2);
                    }
                }
                list2 = (from x in list2
                         orderby x
                         select x).ToList<string>();
                list.InsertRange(0, list2);
                using (List<string>.Enumerator enumerator = list.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        string item = enumerator.Current;
                        string extension2 = Path.GetExtension(item);
                        if (extension2 != null && extension2.Length == 0)
                        {
                            string text3 = item;
                            int num = text3.LastIndexOf('/');
                            if (num != -1)
                            {
                                text3 = text3.Substring(num + 1);
                            }
                            string str = (_currentPath.Length > 0) ? (_currentPath + "/") : _currentPath;
                            this.listView1.Items.Add(new ListViewItem(text3)
                            {
                                ImageKey = "folder",
                                Tag = str + text3
                            });
                        }
                        else
                        {
                            S4ZipEntry s4ZipEntry = zipFile.Values.FirstOrDefault((S4ZipEntry x) => x.FullName == item);
                            if (s4ZipEntry != null)
                            {
                                ListViewItem listViewItem = new ListViewItem(s4ZipEntry.Name)
                                {
                                    ImageKey = Path.GetExtension(s4ZipEntry.Name),
                                    Tag = s4ZipEntry
                                };
                                DateTime lastWriteTime = File.GetLastWriteTime(s4ZipEntry.FileName);
                                listViewItem.SubItems.Add(lastWriteTime.ToShortDateString() + " " + lastWriteTime.ToShortTimeString());
                                listViewItem.SubItems.Add(s4ZipEntry.FileName.Substring(s4ZipEntry.FileName.LastIndexOf('\\') + 1));
                                //listViewItem.SubItems.Add(s4ZipEntry.Length.ToByteString());
                                //listViewItem.ForeColor = (s4ZipEntry.Modified ? Color.Red : Color.Black);
                                this.listView1.Items.Add(listViewItem);
                            }
                        }
                    }
                    return;
                }
            }
            string search = this.searchBox.Text;
            foreach (S4ZipEntry s4ZipEntry2 in (from x in zipFile.Values
                                                where x.Name.Contains(search)
                                                select x).ToList<S4ZipEntry>())
            {
                ListViewItem listViewItem2 = new ListViewItem(s4ZipEntry2.FullName)
                {
                    ImageKey = Path.GetExtension(s4ZipEntry2.Name),
                    Tag = s4ZipEntry2
                };
                DateTime lastWriteTime2 = File.GetLastWriteTime(s4ZipEntry2.FileName);
                listViewItem2.SubItems.Add(lastWriteTime2.ToShortDateString() + " " + lastWriteTime2.ToShortTimeString());
                listViewItem2.SubItems.Add(s4ZipEntry2.FileName.Substring(s4ZipEntry2.FileName.LastIndexOf('\\') + 1));
                this.listView1.Items.Add(listViewItem2);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem focusedItem = this.listView1.FocusedItem;
            if (e.Button == MouseButtons.Left && focusedItem.Bounds.Contains(e.Location))
            {
                if (focusedItem.Tag is S4ZipEntry)
                {
                    this.OnCtxOpen(null, null);
                    return;
                }
                string currentPath = _currentPath;
                string text = ((string)focusedItem.Tag) ?? focusedItem.Text;
                string currentPath2;
                if (!(text == ".."))
                {
                    currentPath2 = text;
                }
                else
                {
                    currentPath2 = ((_currentPath.Count((char x) => x == '/') > 0) ? _currentPath.Substring(0, _currentPath.LastIndexOf('/')) : "");
                }
                _currentPath = currentPath2;
                if (currentPath != _currentPath)
                {
                    this.PopulateView();
                }
            }
        }

        private void OnCtxOpen(object sender, EventArgs e)
        {
            foreach (object obj in this.listView1.SelectedItems)
            {
                ListViewItem listViewItem = (ListViewItem)obj;
                object tag = listViewItem.Tag;
                S4ZipEntry entry = tag as S4ZipEntry;
                if (entry != null && !(entry.Name != listViewItem.Text))
                {
                    string key = _fileTracker.FirstOrDefault((KeyValuePair<string, TrackData> x) => x.Value.Entry == entry).Key;
                    string text;
                    if (key != null)
                    {
                        text = key;
                    }
                    else
                    {
                        text = Path.GetTempFileName().Replace(".tmp", "") + Path.GetExtension(entry.FullName);
                        File.WriteAllBytes(text, entry.GetData());
                        this._fileTracker.Add(text, new TrackData(entry, File.GetLastWriteTime(text)));
                    }
                    Process.Start(text);
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count != 1)
            {
                return;
            }
            string b = "";
            string location = this.listView1.SelectedItems[0].SubItems[0].Text ?? "";
            S4ZipEntry s4ZipEntry = _zipFile.Values.FirstOrDefault((S4ZipEntry x) => x.Name == location);
            if (s4ZipEntry == null)
            {
                return;
            }
            if (location == b)
            {
                return;
            }
            try
            {
                string extension = Path.GetExtension(s4ZipEntry.Name);
                ExtensionType extensionType = ExtensionType.Binary;
                foreach (KeyValuePair<string, ExtensionType> keyValuePair in _extensions)
                {
                    if (extension == keyValuePair.Key)
                    {
                        extensionType = keyValuePair.Value;
                    }
                }
                try
                {
                    //this.imageDisplay.Visible = false;
                    //this.imageDisplay.Image = null;
                    //this.textDisplay.Enabled = false;
                    //this.textDisplay.Visible = false;
                }
                catch
                {
                }
                switch (extensionType)
                {
                    case ExtensionType.Binary:
                    case ExtensionType.Text:
                        {
                            string @string = Encoding.UTF8.GetString(s4ZipEntry.GetData());
                            this.textDisplay.Enabled = true;
                            this.textDisplay.Visible = true;
                            this.textDisplay.Text = @string;
                            break;
                        }
                    //case ExtensionType.Image:
                    //    {
                    //        byte[] data = s4ZipEntry.GetData();
                    //        if (data != null)
                    //        {
                    //            Image image = Preview.LoadImage(data, extension);
                    //            this.imageDisplay.Enabled = true;
                    //            this.imageDisplay.Visible = true;
                    //            this.imageDisplay.Image = image;
                    //            if (image != null)
                    //            {
                    //                this.imageDisplay.SizeMode = ((image.Width > this.imageDisplay.Width || image.Height > this.imageDisplay.Height) ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage);
                    //            }
                    //        }
                    //        break;
                    //    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                b = location;
            }
            catch
            {
            }
        }
    }
}

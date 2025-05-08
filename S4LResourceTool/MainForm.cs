using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NeoNetsphere.Resource;
using Org.BouncyCastle.Crypto.Tls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace S4LResourceTool
{

    public partial class MainForm : Form
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

        public static string _currentPath = "";
        private FileTracker _fileTracker;
        public static S4Zip _zipFile;
        public static List<string> _extraFolders = new List<string>();
        public static List<string> _deletedItems = new List<string>();
        private Dictionary<string, ExtensionType> _extensions;
        private bool _confirmReplacements = true;

        public MainForm()
        {
            InitializeComponent();
            MainForm.SetWindowTheme(tree.Handle, "explorer", null);
            MainForm.SetWindowTheme(listView1.Handle, "explorer", null);
            tree.ImageList = new ImageList();
            listView1.SmallImageList = new ImageList();
            using (Icon folderIcon = NativeMethods.GetFolderIcon(true))
            {
                tree.ImageList.Images.Add("folder", folderIcon.ToBitmap());
                listView1.SmallImageList.Images.Add("folder", folderIcon.ToBitmap());
            }
            Open();
            tree.AfterSelect += new TreeViewEventHandler(OnFolderChange);
        }

        private void Open()
        {
            _zipFile = S4Zip.OpenZip("E:\\Games\\S4 Legacy\\resource.s4hd");
            if (_zipFile == null)
            {
                MessageBox.Show("Failed to open S4Zip");
                return;
            }
            _currentPath = "";
            _extraFolders = new List<string>();
            _deletedItems = new List<string>();
            _extensions = new Dictionary<string, ExtensionType>
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
            _fileTracker = new FileTracker();
            _fileTracker.OnChange += delegate (object sender, KeyValuePair<string, TrackData> kv)
            {
                base.Invoke(new Action(delegate ()
                {
                    string key = kv.Key;
                    S4ZipEntry entry = kv.Value.Entry;
                    if (MessageBox.Show("Do you want to apply your changes?", entry.FullName, MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        return;
                    }
                    entry.SetData(File.ReadAllBytes(key));
                    foreach (object obj in listView1.Items)
                    {
                        ListViewItem listViewItem = (ListViewItem)obj;
                        if (!(listViewItem.SubItems[0].Text != entry.Name))
                        {
                            listViewItem.ForeColor = Color.Red;
                        }
                    }
                    listView1_SelectedIndexChanged(null, null);
                }));
            };
            UpdateTree();
            foreach (KeyValuePair<string, S4ZipEntry> keyValuePair in _zipFile)
            {
                string extension = Path.GetExtension(keyValuePair.Value.Name);
                Dictionary<string, ExtensionType> extensions = _extensions;
                string text = extension;
                if (text == null)
                {
                    throw new ArgumentNullException();
                }
                if (!extensions.ContainsKey(text))
                {
                    _extensions.Add(extension, ExtensionType.Binary);
                }
                if (!listView1.SmallImageList.Images.ContainsKey(extension))
                {
                    using (Icon fileIcon = NativeMethods.GetFileIcon(extension, true))
                    {
                        listView1.SmallImageList.Images.Add(extension, fileIcon.ToBitmap());
                    }
                }
            }
            PopulateView();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateTree();
            PopulateView();
        }

        private void UpdateTree()
        {
            tree.BeginUpdate();
            tree.Nodes.Clear();
            tree.Nodes.Add(GetTreeStructure());
            tree.EndUpdate();
        }

        private TreeNode GetTreeStructure()
        {
            TreeNode treeNode = new TreeNode
            {
                Text = "_resources"
            };
            List<string> list = _zipFile.GetFolders().ToList<string>();
            list.AddRange(_extraFolders);
            foreach (string text in (from x in list
                                     orderby x
                                     select x).ToList<string>())
            {
                TreeNode treeNode2 = treeNode;
                string[] array = text.Split(new char[]
                {
                    '/'
                });
                string[] array2 = array;
                Array.Resize<string>(ref array2, array2.Length - 1);
                foreach (string text2 in array2)
                {
                    bool flag = false;
                    foreach (object obj in treeNode2.Nodes)
                    {
                        TreeNode treeNode3 = (TreeNode)obj;
                        if (!(treeNode3.Text != text2))
                        {
                            treeNode2 = treeNode3;
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        treeNode2 = treeNode2.Nodes[treeNode2.Nodes.Add(new TreeNode(text2))];
                    }
                }
                treeNode2.Nodes.Add(new TreeNode(array.Last<string>()));
            }
            return treeNode;
        }

        private void PopulateView()
        {
            listView1.Items.Clear();
            if (searchBox.Text.Length <= 2 || searchBox.Text == "Search for an item...")
            {
                listView1.Items.Add(new ListViewItem("..")
                {
                    ImageKey = "folder"
                });
                List<string> list = _zipFile.GetFiles(_currentPath, true).ToList<string>();
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
                            listView1.Items.Add(new ListViewItem(text3)
                            {
                                ImageKey = "folder",
                                Tag = str + text3
                            });
                        }
                        else
                        {
                            S4ZipEntry s4ZipEntry = _zipFile.Values.FirstOrDefault((S4ZipEntry x) => x.FullName == item);
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
                                listViewItem.SubItems.Add(s4ZipEntry.Length.ToByteString());
                                //listViewItem.ForeColor = Color.Red;
                                listView1.Items.Add(listViewItem);
                            }
                        }
                    }
                    return;
                }
            }
            string search = searchBox.Text;
            foreach (S4ZipEntry s4ZipEntry2 in (from x in _zipFile.Values
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
                listViewItem2.SubItems.Add(s4ZipEntry2.Length.ToByteString());
                //listViewItem2.ForeColor = Color.Red;
                listView1.Items.Add(listViewItem2);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem focusedItem = this.listView1.FocusedItem;
            if (e.Button == MouseButtons.Left && focusedItem.Bounds.Contains(e.Location))
            {
                if (focusedItem.Tag is S4ZipEntry)
                {
                    OnCtxOpen(null, null);
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
                    PopulateView();
                }
            }
        }

        private void OnCtxOpen(object sender, EventArgs e)
        {
            foreach (object obj in listView1.SelectedItems)
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
                        _fileTracker.Add(text, new TrackData(entry, File.GetLastWriteTime(text)));
                    }
                    Process.Start(text);
                }
            }
        }

        [HandleProcessCorruptedStateExceptions]
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1)
            {
                return;
            }
            string b = "";
            string location = listView1.SelectedItems[0].SubItems[0].Text ?? "";
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
                    textDisplay.Enabled = false;
                    textDisplay.Visible = false;
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
                            textDisplay.Enabled = true;
                            textDisplay.Visible = true;
                            textDisplay.Text = @string;
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

        private void tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _currentPath = this.tree.SelectedNode.FullPath.Replace("_resources", "").Replace('\\', '/');
            if (_currentPath.Length > 0 && _currentPath.First<char>() == '/')
            {
                _currentPath = _currentPath.Substring(1);
            }
            PopulateView();
        }

        private void bt_Save_Click(object sender, EventArgs e)
        { 
            _zipFile.Save();
            foreach (string path in _deletedItems)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            PopulateView();
        }

        private void bt_SaveFile_Click(object sender, EventArgs e)
        {
            string output = "";
            if (this.listView1.SelectedItems.Count > 1 || this.listView1.SelectedItems[0].Tag is string)
            {
                using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
                {
                    Description = "Select your S4 League directory."
                })
                {
                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                    {
                        output = folderBrowserDialog.SelectedPath + "/";
                        goto IL_155;
                    }
                    return;
                }
            }
            S4ZipEntry s4ZipEntry = this.listView1.SelectedItems[0].Tag as S4ZipEntry;
            if (s4ZipEntry != null)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.FileName = s4ZipEntry.Name;
                    string extension = Path.GetExtension(s4ZipEntry.Name);
                    if (extension != null)
                    {
                        saveFileDialog.Filter = extension.Substring(1).ToUpper() + " Files|*" + extension;
                    }
                    if (saveFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(saveFileDialog.FileName))
                    {
                        output = saveFileDialog.FileName;
                        File.WriteAllBytes(output, s4ZipEntry.GetData());
                        return;
                    }
                    return;
                }
            }
        IL_155:
            List<object> list = new List<object>();
            foreach (object obj in this.listView1.SelectedItems)
            {
                ListViewItem listViewItem = (ListViewItem)obj;
                list.Add(listViewItem.Tag);
            }
            FileWalker fileWalker = new FileWalker(_zipFile, list, _currentPath);
            List<FileWalker.FileData> data = fileWalker.GenerateList();
            Task.Run(delegate ()
            {
                int i;
                int j;
                for (i = 0; i < data.Count; i = j)
                {
                    try
                    {
                        FileWalker.FileData item = data[i];
                        string path = output + item.Path;
                        string folderName = path.GetFolderName(true);
                        if (!Directory.Exists(folderName))
                        {
                            Directory.CreateDirectory(folderName);
                        }
                        if (File.Exists(item.Entry.FileName))
                        {
                            File.WriteAllBytes(path, item.Entry.GetData());
                        }
                    }
                    catch
                    {
                    }
                    j = i + 1;
                }
            });
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                string text = listView1.FocusedItem.Tag as string;
                if (text == null || !(text == ".."))
                {
                    listViewItemCtx.Items.Clear();
                    if (listView1.SelectedItems.Count == 1)
                    {
                        if (listView1.SelectedItems[0].Tag is string)
                        {
                            listViewItemCtx.Items.Add(new ToolStripMenuItem("Save", null, new EventHandler(OnCtxSave)));
                            listViewItemCtx.Items.Add(new ToolStripMenuItem("Delete", null, new EventHandler(OnCtxDelete)));
                        }
                        else
                        {
                            listViewItemCtx.Items.Add(new ToolStripMenuItem("Open", null, new EventHandler(OnCtxOpen)));
                            listViewItemCtx.Items.Add(new ToolStripMenuItem("Save", null, new EventHandler(OnCtxSave)));
                            listViewItemCtx.Items.Add(new ToolStripMenuItem("Replace", null, new EventHandler(OnCtxReplace)));
                            listViewItemCtx.Items.Add(new ToolStripMenuItem("Delete", null, new EventHandler(OnCtxDelete)));
                        }
                    }
                    if (listView1.SelectedItems.Count > 1)
                    {
                        listViewItemCtx.Items.Add(new ToolStripMenuItem("Open", null, new EventHandler(OnCtxOpen)));
                        listViewItemCtx.Items.Add(new ToolStripMenuItem("Save", null, new EventHandler(OnCtxSave)));
                        listViewItemCtx.Items.Add(new ToolStripMenuItem("Delete", null, new EventHandler(OnCtxDelete)));
                    }
                    listViewItemCtx.Show(Cursor.Position);
                    return;
                }
            }
        }

        private void OnCtxSave(object sender, EventArgs e)
        {
            string output = "";
            if (this.listView1.SelectedItems.Count > 1 || this.listView1.SelectedItems[0].Tag is string)
            {
                using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
                {
                    Description = "Select your S4 League directory."
                })
                {
                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                    {
                        output = folderBrowserDialog.SelectedPath + "/";
                        goto IL_155;
                    }
                    return;
                }
            }
            S4ZipEntry s4ZipEntry = this.listView1.SelectedItems[0].Tag as S4ZipEntry;
            if (s4ZipEntry != null)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.FileName = s4ZipEntry.Name;
                    string extension = Path.GetExtension(s4ZipEntry.Name);
                    if (extension != null)
                    {
                        saveFileDialog.Filter = extension.Substring(1).ToUpper() + " Files|*" + extension;
                    }
                    if (saveFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(saveFileDialog.FileName))
                    {
                        output = saveFileDialog.FileName;
                        File.WriteAllBytes(output, s4ZipEntry.GetData());
                        return;
                    }
                    return;
                }
            }
        IL_155:
            List<object> list = new List<object>();
            foreach (object obj in this.listView1.SelectedItems)
            {
                ListViewItem listViewItem = (ListViewItem)obj;
                list.Add(listViewItem.Tag);
            }
            FileWalker fileWalker = new FileWalker(_zipFile, list, _currentPath);
            List<FileWalker.FileData> data = fileWalker.GenerateList();
            Task.Run(delegate ()
            {
                int i;
                int j;
                for (i = 0; i < data.Count; i = j)
                {
                    try
                    {
                        FileWalker.FileData item = data[i];
                        string path = output + item.Path;
                        string folderName = path.GetFolderName(true);
                        if (!Directory.Exists(folderName))
                        {
                            Directory.CreateDirectory(folderName);
                        }
                        if (File.Exists(item.Entry.FileName))
                        {
                            File.WriteAllBytes(path, item.Entry.GetData());
                        }
                    }
                    catch
                    {
                    }
                    j = i + 1;
                }
            });
        }

        private void OnCtxDelete(object sender, EventArgs e)
        {
            string arg = (this.listView1.SelectedItems.Count == 1) ? "item" : "items";
            if (MessageBox.Show(string.Format("Are you sure you want to delete {0} {1}?", this.listView1.SelectedItems.Count, arg), "Delete", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            List<object> list = new List<object>();
            foreach (object obj in this.listView1.SelectedItems)
            {
                ListViewItem listViewItem = (ListViewItem)obj;
                list.Add(listViewItem.Tag);
            }
            FileWalker fileWalker = new FileWalker(_zipFile, list, _currentPath);
            List<FileWalker.FileData> data = fileWalker.GenerateList();
            Task.Run(delegate ()
            {
                foreach (FileWalker.FileData fileData in data)
                {
                    _deletedItems.Add(fileData.Entry.FileName);
                    fileData.Entry.Remove(false);
                }
                Invoke(new Action(PopulateView));
            });
        }

        private void OnCtxReplace(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1)
            {
                return;
            }
            S4ZipEntry s4ZipEntry = (S4ZipEntry)listView1.SelectedItems[0].Tag;
            string extension = Path.GetExtension(s4ZipEntry.Name);
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = extension.Substring(1).ToUpper() + " Files|*" + extension;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    s4ZipEntry.SetData(File.ReadAllBytes(openFileDialog.FileName));
                }
            }
            UpdateTree();
            PopulateView();
        }

        private void OnFolderChange(object sender, EventArgs e)
        {
            _currentPath = tree.SelectedNode.FullPath.Replace("_resources", "").Replace('\\', '/');
            if (_currentPath.Length > 0 && _currentPath.First<char>() == '/')
            {
                _currentPath = _currentPath.Substring(1);
            }
            this.PopulateView();
        }
    }
}

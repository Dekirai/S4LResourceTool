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
using S4LResourceTool.Properties;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

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
            //ApplyDarkMode();
            MainForm.SetWindowTheme(tree.Handle, "explorer", null);
            MainForm.SetWindowTheme(listView1.Handle, "explorer", null);
            tree.ImageList = new ImageList();
            listView1.SmallImageList = new ImageList();
            using (Icon folderIcon = NativeMethods.GetFolderIcon(true))
            {
                tree.ImageList.Images.Add("folder", folderIcon.ToBitmap());
                listView1.SmallImageList.Images.Add("folder", folderIcon.ToBitmap());
            }
            if (Settings.Default.ClientPath.Length < 5)
            {
                string output = "";
                using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
                {
                    Description = "Select your S4 League directory."
                })
                {
                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                    {
                        output = folderBrowserDialog.SelectedPath + "/";
                        Settings.Default.ClientPath = output;
                        Settings.Default.Save();
                    }
                }
            }
            Open();
            tree.AfterSelect += new TreeViewEventHandler(OnFolderChange);
        }

        private void Open()
        {
            _zipFile = S4Zip.OpenZip(Settings.Default.ClientPath + "resource.s4hd");
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
                            listViewItem.ForeColor = System.Drawing.Color.Red;
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
            string output = "";
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "Select your S4 League directory."
            })
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    output = folderBrowserDialog.SelectedPath + "/";
                    Settings.Default.ClientPath = output;
                    Settings.Default.Save();
                }
            }
            Open();
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
                listView1.Items.Add(listViewItem2);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem focusedItem = listView1.FocusedItem;
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
                            imageDisplay.Enabled = false;
                            imageDisplay.Visible = false;
                            textDisplay.Text = @string;
                            break;
                        }
                    case ExtensionType.Image:
                        {
                            // hide text
                            textDisplay.Visible = false;

                            // dispose old image
                            if (imageDisplay.Image != null)
                            {
                                imageDisplay.Image.Dispose();
                                imageDisplay.Image = null;
                            }

                            byte[] imgData = s4ZipEntry.GetData();
                            string ext = Path.GetExtension(s4ZipEntry.Name).ToLowerInvariant();

                            if (ext == ".dds" || ext == ".tga")
                            {
                                imageDisplay.Image = PfimImageLoader.Load(imgData, ext);
                            }
                            else
                            {
                                // old‐style using‐statement
                                using (var ms = new MemoryStream(imgData))
                                {
                                    imageDisplay.Image = Image.FromStream(ms);
                                }
                            }

                            imageDisplay.SizeMode = PictureBoxSizeMode.Zoom;
                            imageDisplay.Visible = true;
                            break;
                        }
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
            _currentPath = tree.SelectedNode.FullPath.Replace("_resources", "").Replace('\\', '/');
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
            if (listView1.SelectedItems.Count > 1 || listView1.SelectedItems[0].Tag is string)
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
            S4ZipEntry s4ZipEntry = listView1.SelectedItems[0].Tag as S4ZipEntry;
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
            foreach (object obj in listView1.SelectedItems)
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
            if (listView1.SelectedItems.Count == 1 && listView1.SelectedItems[0].Tag is S4ZipEntry singleEntry)
            {
                using (var dlg = new SaveFileDialog())
                {
                    dlg.FileName = singleEntry.Name;
                    var ext = Path.GetExtension(singleEntry.Name);
                    if (!string.IsNullOrEmpty(ext))
                        dlg.Filter = ext.Substring(1).ToUpper() + " Files|*" + ext;

                    if (dlg.ShowDialog() == DialogResult.OK &&
                        !string.IsNullOrWhiteSpace(dlg.FileName))
                    {
                        File.WriteAllBytes(dlg.FileName, singleEntry.GetData());
                    }
                }
                return;
            }

            string outputDir;
            using (var dlg = new FolderBrowserDialog { Description = "Select a directory to save the files." })
            {
                if (dlg.ShowDialog() != DialogResult.OK ||
                    string.IsNullOrWhiteSpace(dlg.SelectedPath))
                    return;

                outputDir = dlg.SelectedPath;
            }

            var tags = listView1.SelectedItems
                                 .Cast<ListViewItem>()
                                 .Select(item => item.Tag)
                                 .ToList<object>();

            var walker = new FileWalker(_zipFile, tags, _currentPath);
            var allFiles = walker.GenerateList();
            if (allFiles.Count == 0) return;

            progressBarSave.Invoke((Action)(() =>
            {
                progressBarSave.Minimum = 0;
                progressBarSave.Maximum = allFiles.Count;
                progressBarSave.Value = 0;
                progressBarSave.Visible = true;
                label1.Visible = true;
            }));

            Task.Run(() =>
            {
                for (int i = 0; i < allFiles.Count; i++)
                {
                    var fileData = allFiles[i];
                    try
                    {
                        string relative = fileData.Path.Replace('/', Path.DirectorySeparatorChar);
                        string outPath = Path.Combine(outputDir, relative);

                        string folder = Path.GetDirectoryName(outPath);
                        if (!Directory.Exists(folder))
                            Directory.CreateDirectory(folder);

                        File.WriteAllBytes(outPath, fileData.Entry.GetData());
                    }
                    catch
                    {
                        // No
                    }

                    progressBarSave.Invoke((Action)(() =>
                    {
                        progressBarSave.Value = i + 1;
                    }));
                }
                progressBarSave.Invoke((Action)(() =>
                {
                    progressBarSave.Visible = false;
                    label1.Visible = false;
                }));
            });
        }

        private void OnCtxDelete(object sender, EventArgs e)
        {
            string arg = (listView1.SelectedItems.Count == 1) ? "item" : "items";
            if (MessageBox.Show(string.Format("Are you sure you want to delete {0} {1}?", listView1.SelectedItems.Count, arg), "Delete", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            List<object> list = new List<object>();
            foreach (object obj in listView1.SelectedItems)
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
            PopulateView();
        }

        private void searchBox_Enter(object sender, EventArgs e)
        {
            searchBox.Text = "";
            searchBox.ForeColor = System.Drawing.Color.Black;
        }

        private void searchBox_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(searchBox.Text))
            {
                return;
            }
            searchBox.Text = "Search for an item...";
            searchBox.ForeColor = System.Drawing.Color.Silver;
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            PopulateView();
        }

        private void IterateDroppedFiles(string prefix, IEnumerable<string> paths)
        {
            foreach (var fullPath in paths)
            {
                if (Directory.Exists(fullPath))
                {
                    var dirName = Path.GetFileName(fullPath);
                    var newPrefix = prefix + dirName + "/";
                    var childEntries = Directory.GetFileSystemEntries(fullPath);
                    IterateDroppedFiles(newPrefix, childEntries);
                }
                else
                {
                    var fileName = Path.GetFileName(fullPath);
                    var key = prefix + fileName;

                    var existing = _zipFile.Values
                                           .FirstOrDefault(x => x.FullName == key);

                    if (existing != null)
                    {
                        // replace?
                        if (!_confirmReplacements ||
                            MessageBox.Show(
                                $"Are you sure you want to replace {key}?",
                                "Replace data",
                                MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            existing.SetData(File.ReadAllBytes(fullPath));
                        }
                    }
                    else
                    {
                        var entry = _zipFile.CreateEntry(key,
                                                         File.ReadAllBytes(fullPath));
                        var folder = entry.GetFolderName(true);
                        if (_extraFolders.Contains(folder))
                            _extraFolders.Remove(folder);
                    }
                }
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            var dropped = ((string[])e.Data.GetData(DataFormats.FileDrop))
                              .ToList();
            IterateDroppedFiles(
                prefix: (_currentPath.Length == 0) ? "" : (_currentPath + "/"),
                paths: dropped);

            PopulateView();
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }
            e.Effect = DragDropEffects.Copy;
        }

        private void bt_FindUnsed_Click(object sender, EventArgs e)
        {
            string resourceDir = _zipFile.ResourcePath;
            if (!Directory.Exists(resourceDir))
            {
                MessageBox.Show(
                    $"Resource folder not found:\n{resourceDir}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var usedChecksums = new HashSet<string>(
                _zipFile.Values.Select(ent => ent.Checksum.ToString("x")),
                StringComparer.OrdinalIgnoreCase);

            var onDiskFiles = Directory.GetFiles(resourceDir);
            var unused = onDiskFiles
                .Where(path => !usedChecksums.Contains(Path.GetFileName(path)))
                .ToList();

            int count = unused.Count;
            if (count == 0)
            {
                MessageBox.Show(
                    "No unused resources found.",
                    "Find unused resources",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            long totalBytes = unused.Sum(path => new FileInfo(path).Length);
            string sizeText = FormatSize(totalBytes);

            var result = MessageBox.Show(
                $"Found {count} unused resource file{(count > 1 ? "s" : "")} " +
                $"totaling {sizeText}.\n\n" +
                "Do you want to permanently delete them from disk?",
                "Delete Unused Resources",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;

            foreach (var file in unused)
                File.Delete(file);

            MessageBox.Show(
                $"Deleted {count} file{(count > 1 ? "s" : "")}, " +
                $"freeing up {sizeText}.",
                "Done",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private static string FormatSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < suffixes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {suffixes[order]}";
        }

        private void ApplyDarkMode()
        {
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.ForeColor = System.Drawing.Color.White;

            foreach (Control ctrl in this.Controls)
            {
                ApplyDarkStyle(ctrl);
            }

            listView1.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            listView1.ForeColor = System.Drawing.Color.White;
            listView1.BorderStyle = BorderStyle.FixedSingle;

            tree.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            tree.ForeColor = System.Drawing.Color.White;

            textDisplay.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            textDisplay.ForeColor = System.Drawing.Color.White;

            searchBox.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            searchBox.ForeColor = System.Drawing.Color.White;
        }

        private void ApplyDarkStyle(Control ctrl)
        {
            if (ctrl is Button btn)
            {
                btn.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
                btn.ForeColor = System.Drawing.Color.White;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            }

            if (ctrl.HasChildren)
            {
                foreach (Control child in ctrl.Controls)
                {
                    ApplyDarkStyle(child);
                }
            }
        }

        public static Bitmap LoadViaWic(Stream s)
        {
            if (s.CanSeek)
                s.Seek(0, SeekOrigin.Begin);

            var decoder = BitmapDecoder.Create(
                s,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad
            );

            BitmapFrame frame = decoder.Frames[0];
            var converted = new FormatConvertedBitmap(
                frame,
                PixelFormats.Pbgra32,
                null,
                0
            );

            int w = converted.PixelWidth;
            int h = converted.PixelHeight;
            int stride = w * (converted.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[h * stride];
            converted.CopyPixels(pixels, stride, 0);

            var bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            var data = bmp.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly,
                bmp.PixelFormat
            );
            Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            bmp.UnlockBits(data);

            return bmp;
        }

    }
}

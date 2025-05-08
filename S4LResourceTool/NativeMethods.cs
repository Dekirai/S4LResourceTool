using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace S4LResourceTool
{
    internal static class NativeMethods
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref NativeMethods.SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalUnlock(IntPtr hMem);

        public static Icon GetFolderIcon(bool smallSize = true)
        {
            NativeMethods.SHFILEINFO shfileinfo = default(NativeMethods.SHFILEINFO);
            uint num = 272U;
            num += 2U;
            if (smallSize)
            {
                num += 1U;
            }
            else
            {
                num = num;
            }
            NativeMethods.SHGetFileInfo(Environment.GetFolderPath(Environment.SpecialFolder.Windows), 16U, ref shfileinfo, (uint)Marshal.SizeOf<NativeMethods.SHFILEINFO>(shfileinfo), num);
            Icon result = (Icon)Icon.FromHandle(shfileinfo.hIcon).Clone();
            NativeMethods.DestroyIcon(shfileinfo.hIcon);
            return result;
        }

        public static Icon GetFileIcon(string name, bool smallSize = true)
        {
            NativeMethods.SHFILEINFO shfileinfo = default(NativeMethods.SHFILEINFO);
            uint num = 272U;
            if (smallSize)
            {
                num += 1U;
            }
            else
            {
                num = num;
            }
            NativeMethods.SHGetFileInfo(name, 128U, ref shfileinfo, (uint)Marshal.SizeOf<NativeMethods.SHFILEINFO>(shfileinfo), num);
            Icon result = (Icon)Icon.FromHandle(shfileinfo.hIcon).Clone();
            NativeMethods.DestroyIcon(shfileinfo.hIcon);
            return result;
        }

        public const int MAX_PATH = 260;

        public const uint SHGFI_ICON = 256U;

        public const uint SHGFI_USEFILEATTRIBUTES = 16U;

        public const uint SHGFI_SMALLICON = 1U;

        public const uint SHGFI_LARGEICON = 0U;

        public const uint SHGFI_OPENICON = 2U;

        public const uint FILE_ATTRIBUTE_NORMAL = 128U;

        public const uint FILE_ATTRIBUTE_DIRECTORY = 16U;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;

            public int iIcon;

            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
    }
}

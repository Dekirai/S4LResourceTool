using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.ExceptionServices;
using Pfim;

namespace S4LResourceTool
{
    public static class PfimImageLoader
    {
        [HandleProcessCorruptedStateExceptions]
        public static Image Load(byte[] data, string ext)
        {
            try
            {
                ext = (ext ?? "").ToLowerInvariant();
                if (ext == ".dds")
                {
                    return LoadPfimImage(Dds.Create(data, new PfimConfig(32768, 0, true)));
                }
                else if (ext == ".tga")
                {
                    return LoadPfimImage(Targa.Create(data, new PfimConfig(32768, 0, true)));
                }
                else
                {
                    using (var ms = new MemoryStream(data))
                        return Image.FromStream(ms);
                }
            }
            catch
            {
                return null;
            }
        }
        [HandleProcessCorruptedStateExceptions]
        private unsafe static Image LoadPfimImage(IImage img)
        {
            PixelFormat pf;
            switch (img.Format)
            {
                case Pfim.ImageFormat.Rgb24:
                    pf = PixelFormat.Format24bppRgb;
                    break;
                case Pfim.ImageFormat.Rgba32:
                    pf = PixelFormat.Format32bppArgb;
                    break;
                default:
                    throw new NotSupportedException($"Pfim format {img.Format} not supported");
            }

            byte[] buffer = img.Data;
            fixed (byte* p = buffer)
            {
                var bmp = new Bitmap(
                    img.Width,
                    img.Height,
                    img.Stride,
                    pf,
                    (IntPtr)p
                );
                return bmp;
            }
        }
    }
}

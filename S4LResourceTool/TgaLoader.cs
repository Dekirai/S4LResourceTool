using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace S4LResourceTool
{
    public static class TgaLoader
    {
        public static Bitmap LoadTga(Stream stream)
        {
            using (var reader = new BinaryReader(stream, System.Text.Encoding.Default, leaveOpen: true))
            {
                // --- 1) Read header ---
                byte idLength = reader.ReadByte();
                byte colorMapType = reader.ReadByte();
                byte imageType = reader.ReadByte();
                reader.BaseStream.Seek(5, SeekOrigin.Current);
                reader.BaseStream.Seek(4, SeekOrigin.Current);
                int width = reader.ReadInt16();
                int height = reader.ReadInt16();
                byte pixelDepth = reader.ReadByte();
                byte imageDescriptor = reader.ReadByte();

                if (idLength > 0)
                    reader.BaseStream.Seek(idLength, SeekOrigin.Current);

                int bpp = pixelDepth / 8;
                if (bpp != 3 && bpp != 4)
                    throw new NotSupportedException($"Only 24bpp or 32bpp TGAs supported, not {pixelDepth}bpp");

                int pixelCount = width * height;
                byte[] pixels = new byte[pixelCount * bpp];

                if (imageType == 2)
                {
                    reader.Read(pixels, 0, pixels.Length);
                }
                else if (imageType == 10)
                {
                    int ptr = 0;
                    while (ptr < pixels.Length)
                    {
                        byte header = reader.ReadByte();
                        int runLen = (header & 0x7F) + 1;
                        if ((header & 0x80) != 0)
                        {
                            byte[] color = reader.ReadBytes(bpp);
                            for (int i = 0; i < runLen; i++)
                            {
                                Buffer.BlockCopy(color, 0, pixels, ptr, bpp);
                                ptr += bpp;
                            }
                        }
                        else
                        {
                            int bytesToRead = runLen * bpp;
                            reader.Read(pixels, ptr, bytesToRead);
                            ptr += bytesToRead;
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException($"TGA image type {imageType} not supported");
                }

                bool topDown = (imageDescriptor & 0x20) != 0;
                PixelFormat fmt = (bpp == 3)
                    ? PixelFormat.Format24bppRgb
                    : PixelFormat.Format32bppArgb;

                var bmp = new Bitmap(width, height, fmt);
                var data = bmp.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly,
                    fmt);

                int stride = data.Stride;
                IntPtr scan0 = data.Scan0;

                for (int y = 0; y < height; y++)
                {
                    int destY = y;
                    int srcY = topDown ? y : (height - 1 - y);
                    int srcOffset = srcY * width * bpp;
                    IntPtr rowPtr = scan0 + destY * stride;

                    // copy one scanline
                    Marshal.Copy(pixels, srcOffset, rowPtr, width * bpp);
                }

                bmp.UnlockBits(data);
                return bmp;
            }
        }
    }
}

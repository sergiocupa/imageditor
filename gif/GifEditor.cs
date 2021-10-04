

using halba.imageditor.bitmap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;


namespace halba.imageditor.gif
{
    public class GifEditor
    {

        

        public static Image CreateFlashingGif(Bitmap base_image, int interval, float inactive_image_alfa)
        {
            var inactive_image = (inactive_image_alfa < 0.01) ? 
                                 BitmapEditor.SetImageOpacity(base_image, inactive_image_alfa, true) :
                                 BitmapEditor.SetImageOpacity(base_image, inactive_image_alfa, false);

            var imagens = new List<Bitmap>();
            imagens.Add(base_image);
            imagens.Add(inactive_image);

            var gif = CreateGif(imagens, interval, true);
            return gif;
        }



        public static Bitmap CreateGif(List<Bitmap> images, int interval, bool animate_forever)
        {
            MemoryStream gif = new MemoryStream();
            var encoder = new GifEncoder(gif);

            foreach (var i in images)
            {
                Bitmap igif = (Bitmap)PrepareImageToGif(i);
                encoder.AddFrame(igif, frameDelay: new TimeSpan(0, 0, 0, 0, interval));
            }

            var result = new Bitmap(gif);
            return result;
        }


        class ImageTime
        {
            public Bitmap Image { get; set; }
            public int Interval { get; set; }
        }


        public static Image Resize(Bitmap image, int width, int height)
        {
            FrameDimension dimension = new FrameDimension(image.FrameDimensionsList[0]);
            int count = image.GetFrameCount(dimension);

            var imagens = new List<ImageTime>();
            int ixp = 0;
            int ix = 0;
            while (ix < count)
            {
                var delay = BitConverter.ToInt32(image.GetPropertyItem(20736).Value, ixp) * 10;
                image.SelectActiveFrame(dimension, ix);
                Bitmap igif = (Bitmap)PrepareImageToGif(new Bitmap(image, width, height));
                imagens.Add(new ImageTime() { Image = igif, Interval = delay });
                ixp += 4;
                ix++;
            }

            MemoryStream gif = new MemoryStream();
            var encoder = new GifEncoder(gif);
            ix = 0;
            while (ix < count)
            {
                encoder.AddFrame(imagens[ix].Image, frameDelay: new TimeSpan(0, 0, 0, 0, imagens[ix].Interval));
                ix++;
            }

            var result = new Bitmap(gif);
            return result;
        }


        //public static Image CreateGif(List<Bitmap> images, int interval, bool animate_forever)
        //{
        //    Bitmap result = null;
        //    MemoryStream ms = new MemoryStream();

        //    List<Bitmap> iprepareds = new List<Bitmap>();

        //    foreach (var i in images)
        //    {
        //        Bitmap igif = (Bitmap)PrepareImageToGif(i);
        //        iprepareds.Add(igif);
        //    }

        //    if (iprepareds.Count > 0)
        //    {
        //        var gifEncoder = GetEncoder(ImageFormat.Gif);

        //        var enFirst      = new EncoderParameters(1);
        //        var enN          = new EncoderParameters(1);
        //        var enFlush      = new EncoderParameters(1);
        //        enFirst.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
        //        enN.Param[0]     = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionTime);
        //        enFlush.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.Flush);

        //        var pDelay = GetDelayProperty(iprepareds.Count, interval);
        //        var pLoop  = GetLoopProperty();

        //        var first = iprepareds[0];
        //        first.SetPropertyItem(pDelay);

        //        if(animate_forever) first.SetPropertyItem(pLoop);

        //        first.Save(ms, gifEncoder, enFirst);


        //        int ix = 1;
        //        while (ix < iprepareds.Count)
        //        {
        //            first.SaveAdd(iprepareds[ix], enN);
        //            ix++;
        //        }
        //        first.SaveAdd(enFlush);

        //        result = new Bitmap(ms);
        //        first.Dispose();
        //    }
        //    return result;
        //}


        public static void SaveGif(List<Bitmap> images, int interval, bool animate_forever, string file_name)
        {
            var img = CreateGif(images, interval, animate_forever);
            if(img != null)
            {
                img.Save(file_name);
            }
        }


        private static PropertyItem GetDelayProperty(int frame_count, int mili)
        {
            const int   PropertyTagFrameDelay = 0x5100;
            const short PropertyTagTypeLong   = 4;
            const int   UintBytes             = 4;

            var frameDelay   = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem));
            frameDelay.Id    = PropertyTagFrameDelay;
            frameDelay.Type  = PropertyTagTypeLong;
            frameDelay.Len   = frame_count * UintBytes;
            frameDelay.Value = new byte[frame_count * UintBytes];

            var frameDelayBytes = BitConverter.GetBytes((uint)(mili / 10));

            for (int j = 0; j < frame_count; ++j)
            {
                Array.Copy(frameDelayBytes, 0, frameDelay.Value, j * UintBytes, UintBytes);
            }
               
            return frameDelay;
        }
        private static PropertyItem GetLoopProperty()
        {
            const short PropertyTagTypeShort = 3;
            const int   PropertyTagLoopCount = 0x5101;

            var loopPropertyItem = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem));
            loopPropertyItem.Id   = PropertyTagLoopCount;
            loopPropertyItem.Type = PropertyTagTypeShort;
            loopPropertyItem.Len  = 1;
          
            loopPropertyItem.Value = BitConverter.GetBytes((ushort)0); 
            return loopPropertyItem;
        }


        internal static Image PrepareImageToGif(Image original)
        {
            Image result = null;

            int bpp = Image.GetPixelFormatSize(original.PixelFormat);
            if (bpp == 8)
            {
                result = original;
                return result;
            }

            if (bpp < 8)
            {
                result = ConvertPixelFormat(original, PixelFormat.Format8bppIndexed, null);
                return result;
            }

            Color[] palette = GetColors((Bitmap)original, 256);
            result = ConvertPixelFormat(original, PixelFormat.Format8bppIndexed, palette);
            return result;
        }



        private static Color[] GetColors(Bitmap bitmap, int maxColors)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");
            if (maxColors < 0)
                throw new ArgumentOutOfRangeException("maxColors");

            HashSet<int> colors = new HashSet<int>();
            PixelFormat pixelFormat = bitmap.PixelFormat;
            if (Image.GetPixelFormatSize(pixelFormat) <= 8)
                return bitmap.Palette.Entries;

            // 32 bpp source: the performant variant
            if (pixelFormat == PixelFormat.Format32bppRgb ||
                pixelFormat == PixelFormat.Format32bppArgb ||
                pixelFormat == PixelFormat.Format32bppPArgb)
            {
                BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, pixelFormat);
                try
                {
                    unsafe
                    {
                        byte* line = (byte*)data.Scan0;
                        for (int y = 0; y < data.Height; y++)
                        {
                            for (int x = 0; x < data.Width; x++)
                            {
                                int c = ((int*)line)[x];

                                if ((c >> 24) == 0)
                                    c = 0xFFFFFF;
                                if (colors.Contains(c))
                                    continue;

                                colors.Add(c);
                                if (colors.Count == maxColors)
                                    return colors.Select(Color.FromArgb).ToArray();
                            }

                            line += data.Stride;
                        }
                    }
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }
            }
            else
            {

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        int c = bitmap.GetPixel(x, y).ToArgb();
                        if (colors.Contains(c))
                            continue;

                        colors.Add(c);
                        if (colors.Count == maxColors)
                            return colors.Select(Color.FromArgb).ToArray();
                    }
                }
            }

            return colors.Select(Color.FromArgb).ToArray();
        }

        private static Image ConvertPixelFormat(Image image, PixelFormat newPixelFormat, Color[] palette)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            PixelFormat sourcePixelFormat = image.PixelFormat;

            int bpp = Image.GetPixelFormatSize(newPixelFormat);
            if (newPixelFormat == PixelFormat.Format16bppArgb1555 || newPixelFormat == PixelFormat.Format16bppGrayScale)
                throw new NotSupportedException("This pixel format is not supported by GDI+");

            Bitmap result;

            if (bpp > 8)
            {
                result = new Bitmap(image.Width, image.Height, newPixelFormat);
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.DrawImage(image, 0, 0, image.Width, image.Height);
                }

                return result;
            }

            int transparentIndex;
            Bitmap bmp;

            RGBQUAD[] targetPalette = new RGBQUAD[256];
            int colorCount = InitPalette(targetPalette, bpp, (image is Bitmap) ? image.Palette : null, palette, out transparentIndex);
            BITMAPINFO bmi = new BITMAPINFO();
            bmi.icHeader.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            bmi.icHeader.biWidth = image.Width;
            bmi.icHeader.biHeight = image.Height;
            bmi.icHeader.biPlanes = 1;
            bmi.icHeader.biBitCount = (ushort)bpp;
            bmi.icHeader.biCompression = BI_RGB;
            bmi.icHeader.biSizeImage = (uint)(((image.Width + 7) & 0xFFFFFFF8) * image.Height / (8 / bpp));
            bmi.icHeader.biXPelsPerMeter = 0;
            bmi.icHeader.biYPelsPerMeter = 0;
            bmi.icHeader.biClrUsed = (uint)colorCount;
            bmi.icHeader.biClrImportant = (uint)colorCount;
            bmi.icColors = targetPalette;

            bmp = (image as Bitmap) ?? new Bitmap(image);

            IntPtr bits;
            IntPtr hbmResult = CreateDIBSection(IntPtr.Zero, ref bmi, DIB_RGB_COLORS, out bits, IntPtr.Zero, 0);

            IntPtr dcScreen = GetDC(IntPtr.Zero);

            IntPtr hbmSource = bmp.GetHbitmap();
            IntPtr dcSource = CreateCompatibleDC(dcScreen);
            SelectObject(dcSource, hbmSource);

            IntPtr dcTarget = CreateCompatibleDC(dcScreen);
            SelectObject(dcTarget, hbmResult);

            BitBlt(dcTarget, 0, 0, image.Width, image.Height, dcSource, 0, 0, 0x00CC0020 /*TernaryRasterOperations.SRCCOPY*/);

            result = Image.FromHbitmap(hbmResult);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            DeleteDC(dcSource);
            DeleteDC(dcTarget);
            ReleaseDC(IntPtr.Zero, dcScreen);
            DeleteObject(hbmSource);
            DeleteObject(hbmResult);

            ColorPalette resultPalette = result.Palette;
            bool resetPalette = false;

            if (transparentIndex >= 0)
            {
                if (resultPalette.Entries[transparentIndex].A != 0)
                {
                    resultPalette.Entries[transparentIndex] = Color.Transparent;
                    resetPalette = true;
                }

                ToIndexedTransparentByArgb(result, bmp, transparentIndex);
            }

            if (resetPalette) result.Palette = resultPalette;

            if (!ReferenceEquals(bmp, image)) bmp.Dispose();

            return result;
        }
        private static int InitPalette(RGBQUAD[] targetPalette, int bpp, ColorPalette originalPalette, Color[] desiredPalette, out int transparentIndex)
        {
            int maxColors = 1 << bpp;

            Color[] sourcePalette = desiredPalette;

            if (sourcePalette == null && originalPalette != null && originalPalette.Entries.Length > 0 && originalPalette.Entries.Length <= maxColors)
                sourcePalette = originalPalette.Entries;

            if (sourcePalette == null)
            {
                using (Bitmap bmpReference = new Bitmap(1, 1, GetPixelFormat(bpp)))
                {
                    sourcePalette = bmpReference.Palette.Entries;
                }
            }

            transparentIndex = -1;
            bool hasBlack = false;
            int colorCount = Math.Min(maxColors, sourcePalette.Length);
            for (int i = 0; i < colorCount; i++)
            {
                targetPalette[i] = new RGBQUAD(sourcePalette[i]);
                if (transparentIndex == -1 && sourcePalette[i].A == 0)
                    transparentIndex = i;
                if (!hasBlack && (sourcePalette[i].ToArgb() & 0xFFFFFF) == 0)
                    hasBlack = true;
            }

            if (transparentIndex == 0)
            {
                targetPalette[0] = targetPalette[1];
                transparentIndex = 1;
            }
            else if (transparentIndex != -1)
            {
                targetPalette[transparentIndex] = targetPalette[transparentIndex - 1];
            }

            if (colorCount < maxColors && !hasBlack)
                colorCount++;

            return colorCount;
        }

        private unsafe static void ToIndexedTransparentByArgb(Bitmap target, Bitmap source, int transparentIndex)
        {
            int sourceBpp = Image.GetPixelFormatSize(source.PixelFormat);
            int targetBpp = Image.GetPixelFormatSize(target.PixelFormat);

            BitmapData dataTarget = target.LockBits(new Rectangle(Point.Empty, target.Size), ImageLockMode.ReadWrite, target.PixelFormat);
            BitmapData dataSource = source.LockBits(new Rectangle(Point.Empty, source.Size), ImageLockMode.ReadOnly, source.PixelFormat);
            try
            {
                byte* lineSource = (byte*)dataSource.Scan0;
                byte* lineTarget = (byte*)dataTarget.Scan0;
                bool is32Bpp = sourceBpp == 32;

                for (int y = 0; y < dataSource.Height; y++)
                {
                    for (int x = 0; x < dataSource.Width; x++)
                    {
                        if (is32Bpp && ((uint*)lineSource)[x] >> 24 == 0
                            || !is32Bpp && ((ulong*)lineSource)[x] >> 48 == 0UL)
                        {
                            switch (targetBpp)
                            {
                                case 8:
                                    lineTarget[x] = (byte)transparentIndex;
                                    break;
                                case 4:
                                    int pos = x >> 1;
                                    byte nibbles = lineTarget[pos];
                                    if ((x & 1) == 0)
                                    {
                                        nibbles &= 0x0F;
                                        nibbles |= (byte)(transparentIndex << 4);
                                    }
                                    else
                                    {
                                        nibbles &= 0xF0;
                                        nibbles |= (byte)transparentIndex;
                                    }

                                    lineTarget[pos] = nibbles;
                                    break;
                                case 1:
                                    pos = x >> 3;
                                    byte mask = (byte)(128 >> (x & 7));
                                    if (transparentIndex == 0)
                                        lineTarget[pos] &= (byte)~mask;
                                    else
                                        lineTarget[pos] |= mask;
                                    break;
                            }
                        }
                    }

                    lineSource += dataSource.Stride;
                    lineTarget += dataTarget.Stride;
                }
            }
            finally
            {
                target.UnlockBits(dataTarget);
                source.UnlockBits(dataSource);
            }
        }

        private static PixelFormat GetPixelFormat(int bpp)
        {
            switch (bpp)
            {
                case 1:
                    return PixelFormat.Format1bppIndexed;
                case 4:
                    return PixelFormat.Format4bppIndexed;
                case 8:
                    return PixelFormat.Format8bppIndexed;
                case 16:
                    return PixelFormat.Format16bppRgb565;
                case 24:
                    return PixelFormat.Format24bppRgb;
                case 32:
                    return PixelFormat.Format32bppArgb;
                case 48:
                    return PixelFormat.Format48bppRgb;
                case 64:
                    return PixelFormat.Format64bppArgb;
                default:
                    throw new ArgumentOutOfRangeException("bpp");
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }



        private const int BI_RGB = 0;
        private const int DIB_RGB_COLORS = 0;

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BITMAPINFO pbmi, int iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [StructLayout(LayoutKind.Sequential)]
        private struct RGBQUAD
        {
            internal byte rgbBlue;
            internal byte rgbGreen;
            internal byte rgbRed;
            internal byte rgbReserved;

            internal RGBQUAD(Color color)
            {
                rgbRed = color.R;
                rgbGreen = color.G;
                rgbBlue = color.B;
                rgbReserved = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFO
        {
            public BITMAPINFOHEADER icHeader;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public RGBQUAD[] icColors;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFOHEADER
        {
            internal uint biSize;
            internal int biWidth;
            internal int biHeight;
            internal ushort biPlanes;
            internal ushort biBitCount;
            internal uint biCompression;
            internal uint biSizeImage;
            internal int biXPelsPerMeter;
            internal int biYPelsPerMeter;
            internal uint biClrUsed;
            internal uint biClrImportant;
        }
    }
}

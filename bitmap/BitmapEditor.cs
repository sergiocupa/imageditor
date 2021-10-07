

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


namespace halba.imageditor.bitmap
{
    public class BitmapEditor
    {

        



        public static Bitmap ResizeImage(Image source, int width, int height)
        {
            Bitmap brestored = new Bitmap(width, height);
            using (Graphics gimage = Graphics.FromImage(brestored))
            {
                gimage.CompositingQuality = CompositingQuality.HighQuality;
                gimage.InterpolationMode  = InterpolationMode.HighQualityBicubic;
                gimage.DrawImage(source, 0, 0, width, height);
            }
            return brestored;
        }


        public static Bitmap SetImageOpacity(Image image, float opacity, bool fully_transparent)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                if(fully_transparent)
                {
                    gfx.Clear(Color.Transparent);
                }
                else
                {
                    gfx.SmoothingMode     = SmoothingMode.HighQuality;
                    gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    ColorMatrix matrix = new ColorMatrix();
                    matrix.Matrix33 = opacity;
                    ImageAttributes attributes = new ImageAttributes();
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return bmp;
        }

    }
}

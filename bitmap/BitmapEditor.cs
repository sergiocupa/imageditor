

using System.Drawing;
using System.Drawing.Imaging;


namespace halba.imageditor.bitmap
{
    public class BitmapEditor
    {

        public static Bitmap SetImageOpacity(Image image, float opacity, bool fully_transparent)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                if(fully_transparent)
                {
                    SolidBrush blueBrush = new SolidBrush(Color.Transparent);
                    Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                    gfx.FillRectangle(blueBrush, rect);
                }
                else
                {
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

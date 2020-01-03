
extern alias destination;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Example.Common.Helpers
{
    public class FileHelper
    {

        public destination::System.Drawing.Bitmap CreateImageFromString(string strImage)
        {

            string text = strImage;
            //create new image
            destination::System.Drawing.Bitmap bitmap = new destination::System.Drawing.Bitmap(1, 1);
            //Properties string to draw
            destination::System.Drawing.Font font = new destination::System.Drawing.Font("Arial", 25, destination::System.Drawing.FontStyle.Regular, destination::System.Drawing.GraphicsUnit.Pixel);
            destination::System.Drawing.Graphics graphics = destination::System.Drawing.Graphics.FromImage(bitmap);
            //properties new image
            int width = (int)graphics.MeasureString(text, font).Width;
            int height = (int)graphics.MeasureString(text, font).Height;

            //var size = new Size(width, height);

            bitmap = new destination::System.Drawing.Bitmap(bitmap, width, height);
            //add text to image
            graphics = destination::System.Drawing.Graphics.FromImage(bitmap);

            //var ecnijbk=Color.White;

            //System.Drawing.Color color;

            graphics.Clear(destination::System.Drawing.Color.White);
            //graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.DrawString(text, font, new destination::System.Drawing.SolidBrush(destination::System.Drawing.Color.Coral), 0, 0);
            //execute pending graphics
            graphics.Flush();
            //release resources used by graphics
            graphics.Dispose();
            //save the image
            //bitmap.Save("C:\\prueba.png", destination::System.Drawing.Imaging.ImageFormat.Png);

            //do something with image

            return bitmap;
        }
    }
}

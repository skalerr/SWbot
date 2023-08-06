using System.Drawing;
using System.Drawing.Imaging;

namespace SWbot;

public class Utils
{
    public static Bitmap ConvertTo24BppRgb(Bitmap bitmap)
    {
        Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
        using (Graphics g = Graphics.FromImage(newBitmap))
        {
            g.DrawImage(bitmap, 0, 0);
        }
        return newBitmap;
    }
}
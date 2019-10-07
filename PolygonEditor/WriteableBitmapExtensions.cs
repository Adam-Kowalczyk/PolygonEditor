using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PolygonEditor
{
    public static class WriteableBitmapExtensions
    {
        public static void DrawPixel(this WriteableBitmap wb, int x, int y, Color color)
        {
            byte blue = color.B;
            byte green = color.G;
            byte red = color.R;
            byte alpha = color.A;
            byte[] colorData = { blue, green, red, alpha };
            Int32Rect rect = new Int32Rect(x, y, 1, 1);
            wb.WritePixels(rect, colorData, 4, 0);
        }
    }
}

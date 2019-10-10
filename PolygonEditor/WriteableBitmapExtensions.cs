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
            if (x < 0 || x >= wb.PixelWidth) return;
            if (y < 0 || y >= wb.PixelHeight) return;
            byte blue = color.B;
            byte green = color.G;
            byte red = color.R;
            byte alpha = color.A;
            byte[] colorData = { blue, green, red, alpha };
            Int32Rect rect = new Int32Rect(x, y, 1, 1);
            wb.WritePixels(rect, colorData, 4, 0);

        }

        public static void DrawPoint(this WriteableBitmap wb, int x, int y, Color color, int size)
        {
            byte blue = color.B;
            byte green = color.G;
            byte red = color.R;
            byte alpha = color.A;
            byte[] colorData = { blue, green, red, alpha };

            for(int i = x - size; i<= x + size;i++)
            {
                if (i < 0 || i >= wb.PixelWidth) continue;
                for(int j = y - size; j<y + size; j++)
                {
                    if (j < 0 || j >= wb.PixelHeight) continue;
                    Int32Rect rect = new Int32Rect(i, j, 1, 1);
                    wb.WritePixels(rect, colorData, 4, 0);
                }
            }

            
        }

        //public static void DrawLine(this WriteableBitmap wb, int x1, int y1, int x2, int y2, Color color)
        //{
        //    var reverseX = 1;
        //    var reverseY = 1;
        //    if (x1 > x2)
        //        reverseX = -1;
        //    if (y1 > y2)
        //        reverseY = -1;

        //    wb.DrawLineBresenham(reverseX * x1, reverseY * y1, reverseX * x2, reverseY * y2, color, reverseX == -1, reverseY == -1);
        //}

        static void DrawLineBresenham(this WriteableBitmap wb, int x1, int y1, int x2, int y2, Color color, bool reversedX = false, bool reversedY = false)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            int d = 2 * dy - dx;
            int incrE = 2 * dy;
            int incrNE = 2 * (dy - dx);
            int x = x1;
            int y = y1;
            wb.DrawPixel(reversedX ? -x : x, reversedY ? -y : y, color);
            while (x < x2)
            {
                if (d < 0)
                {
                    d += incrE;
                    x++;
                }
                else
                {
                    d += incrNE;
                    x++;
                    y++;
                }
                wb.DrawPixel(reversedX ? -x : x, reversedY ? -y : y, color);
            }
        }

        static void DrawLineBresenhamLow(this WriteableBitmap wb, int x1, int y1, int x2, int y2, Color color)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            int yi = 1;
            if (dy < 0)
            {
                yi = -1;
                dy = -dy;
            }
            int incrE = 2 * dy;
            int incrNE = 2 * (dy - dx);
            int d = 2 * dy - dx;
            int x = x1;
            int y = y1;
            while (x < x2)
            {
                if (d < 0)
                {
                    d += incrE;
                    x++;
                }
                else
                {
                    d += incrNE;
                    x++;
                    y = y + yi;
                }
                wb.DrawPixel(x, y, color);
            }
        }

        static void DrawLineBresenhamHigh(this WriteableBitmap wb, int x1, int y1, int x2, int y2, Color color)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            int xi = 1;
            if (dx < 0)
            {
                xi = -1;
                dx = -dx;
            }
            int incrE = 2 * dx;
            int incrNE = 2 * (dx - dy);
            int d = 2 * dx - dy;
            int x = x1;
            int y = y1;
            while (y < y2)
            {
                if (d < 0)
                {
                    d += incrE;
                    y++;
                }
                else
                {
                    d += incrNE;
                    y++;
                    x = x + xi;
                }
                wb.DrawPixel(x, y, color);
            }
        }

        public static void DrawLine(this WriteableBitmap wb, int x1, int y1, int x2, int y2, Color color)
        {
            if(Math.Abs(y2-y1) < Math.Abs(x2-x1))
            {
                if (x1 > x2)
                    wb.DrawLineBresenhamLow(x2, y2, x1, y1, color);
                else
                    wb.DrawLineBresenhamLow(x1, y1, x2, y2, color);
            }
            else
            {
                if (y1 > y2)
                    wb.DrawLineBresenhamHigh(x2, y2, x1, y1, color);
                else
                    wb.DrawLineBresenhamHigh(x1, y1, x2, y2, color);
            }
        }
    }
}

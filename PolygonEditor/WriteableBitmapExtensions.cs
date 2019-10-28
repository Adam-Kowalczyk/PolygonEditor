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
        public static bool Antialiasing = false;
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

        public static void DrawLine(this WriteableBitmap wb, int x1, int y1, int x2, int y2, Color color)
        {
                wb.DrawBresenhamLine(x1, y1, x2, y2, color);
        }

        public static void DrawBresenhamLine(this WriteableBitmap wb, int x1, int y1, int x2, int y2, Color color)
        {
            //Source: https://pl.wikipedia.org/wiki/Algorytm_Bresenhama
            // zmienne pomocnicze
            int d, dx, dy, ai, bi, xi, yi;
            int x = x1, y = y1;
            // ustalenie kierunku rysowania
            if (x1 < x2)
            {
                xi = 1;
                dx = x2 - x1;
            }
            else
            {
                xi = -1;
                dx = x1 - x2;
            }
            // ustalenie kierunku rysowania
            if (y1 < y2)
            {
                yi = 1;
                dy = y2 - y1;
            }
            else
            {
                yi = -1;
                dy = y1 - y2;
            }
            // pierwszy piksel
            wb.DrawPixel(x, y, color);
            // oś wiodąca OX
            if (dx > dy)
            {
                ai = (dy - dx) * 2;
                bi = dy * 2;
                d = bi - dx;
                // pętla po kolejnych x
                while (x != x2)
                {
                    // test współczynnika
                    if (d >= 0)
                    {
                        x += xi;
                        y += yi;
                        d += ai;
                    }
                    else
                    {
                        d += bi;
                        x += xi;
                    }
                    wb.DrawPixel(x, y, color);
                }
            }
            // oś wiodąca OY
            else
            {
                ai = (dx - dy) * 2;
                bi = dx * 2;
                d = bi - dy;
                // pętla po kolejnych y
                while (y != y2)
                {
                    // test współczynnika
                    if (d >= 0)
                    {
                        x += xi;
                        y += yi;
                        d += ai;
                    }
                    else
                    {
                        d += bi;
                        y += yi;
                    }
                    wb.DrawPixel(x, y, color);
                }
            }
        }


        public static void DrawRelationBox(this WriteableBitmap wb, int x, int y, Relation rel, int number)
        {
            if (rel == Relation.NONE) return;
            Color borderCol = Color.FromRgb(0, 0, 0);
            //wb.DrawLine(x, y, x + 10, y, borderCol);
            //wb.DrawLine(x, y, x, y + 8, borderCol);
            //wb.DrawLine(x + 10, y, x + 10, y + 8, borderCol);
            //wb.DrawLine(x, y + 8, x + 10, y + 8, borderCol);

            if(rel == Relation.EQUALS)
            {
                wb.DrawLine(x, y, x + 5, y, borderCol);
                wb.DrawLine(x, y + 1, x + 5, y + 1, borderCol);

                wb.DrawLine(x, y + 4, x + 5, y + 4, borderCol);
                wb.DrawLine(x, y + 5, x + 5, y + 5, borderCol);
            }

            if(rel == Relation.PARALLEL)
            {
                wb.DrawLine(x, y, x, y + 7, borderCol);
                wb.DrawLine(x + 1, y, x + 1, y + 7, borderCol);

                wb.DrawLine(x + 4, y, x + 4, y + 7, borderCol);
                wb.DrawLine(x + 5, y, x + 5, y + 7, borderCol);
            }

            wb.DrawDigit(x + 8, y, number, borderCol);
        }

        public static void DrawDigit(this WriteableBitmap wb, int x, int y, int number, Color col)
        {
            if(number == -1)
            {
                wb.DrawPixel(x, y + 1, col);
                wb.DrawPixel(x + 1, y, col);
                wb.DrawPixel(x + 2, y, col);
                wb.DrawPixel(x + 3, y, col);
                wb.DrawPixel(x + 4, y + 1, col);
                wb.DrawPixel(x + 4, y + 2, col);
                wb.DrawPixel(x + 3, y + 3, col);
                wb.DrawPixel(x + 2, y + 4, col);
                wb.DrawPixel(x + 2, y + 6, col);
            }
            else if(number == 1)
            {
                wb.DrawLine(x + 1, y, x + 1, y + 7, col);
                wb.DrawPixel(x, y + 1, col);
                wb.DrawPixel(x, y + 7, col);
                wb.DrawPixel(x + 2, y + 7, col);


            }
            else if( number == 2)
            {
                wb.DrawPixel(x, y + 1, col);
                wb.DrawPixel(x + 1, y, col);
                wb.DrawPixel(x + 2, y, col);
                wb.DrawPixel(x + 3, y, col);
                wb.DrawPixel(x + 4, y + 1, col);
                wb.DrawPixel(x + 3, y + 2, col);
                wb.DrawPixel(x + 2, y + 3, col);
                wb.DrawPixel(x + 1, y + 4, col);
                wb.DrawPixel(x, y + 5, col);
                wb.DrawPixel(x, y + 6, col);

                wb.DrawLine(x, y + 6, x + 4, y + 6, col);
            }
            else if(number == 3)
            {
                wb.DrawLine(x, y, x + 3, y, col);
                wb.DrawLine(x, y + 3, x + 3, y + 3, col);
                wb.DrawLine(x, y + 6, x + 3, y + 6, col);
                wb.DrawPixel(x + 4, y + 1, col);
                wb.DrawPixel(x + 4, y + 2, col);
                wb.DrawPixel(x + 4, y + 4, col);
                wb.DrawPixel(x + 4, y + 5, col);



            }
            else if(number == 4)
            {
                wb.DrawLine(x, y, x, y + 2, col);
                wb.DrawLine(x + 4, y, x + 4, y + 2, col);
                wb.DrawLine(x + 1, y + 3, x + 3, y + 3, col);
                wb.DrawLine(x + 4, y + 4, x + 4, y + 6, col);
            }
            else if(number == 5)
            {
                wb.DrawLine(x + 1, y, x + 4, y, col);
                wb.DrawLine(x, y + 1, x, y + 3, col);
                wb.DrawLine(x + 1, y + 3, x + 3, y + 3, col);
                wb.DrawPixel(x + 4, y + 4, col);
                wb.DrawPixel(x + 4, y + 5, col);
                wb.DrawLine(x, y + 6, x + 3, y + 6, col);
            }
        }
    }
}

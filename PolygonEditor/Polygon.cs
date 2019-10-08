using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PolygonEditor
{
    public class Polygon : Observable
    {
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name == value) return;
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        string name;

        public Rect Boundries { get; set; }

        public ObservableCollection<Point> Points
        {
            get
            {
                return points ?? (points = new ObservableCollection<Point>());
            }
        }
        ObservableCollection<Point> points;

        public WriteableBitmap Bitmap
        {
            get
            {
                return bitmap;
            }
            set
            {
                bitmap = value;
                OnPropertyChanged(nameof(Bitmap));
            }
        }
        WriteableBitmap bitmap;

        public void RenderBitmap()
        {
            if (Points.Count < 3) return;
            WriteableBitmap wb = new WriteableBitmap((int)Boundries.Width, (int)Boundries.Height, 96, 96, PixelFormats.Bgra32, null);
            for(int i = 0; i<Points.Count;i++)
            {
                if(i<Points.Count - 1)
                {
                    wb.DrawLine((int)Points[i].X, (int)Points[i].Y, (int)Points[i + 1].X, (int)Points[i + 1].Y, Color.FromArgb(255, 0, 255, 0));
                }
                else
                {
                    wb.DrawLine((int)Points[i].X, (int)Points[i].Y, (int)Points[0].X, (int)Points[0].Y, Color.FromArgb(255, 0, 255, 0));

                }
            }
            Bitmap = wb;
        }
    }

    //int x = 200;
    //int y = 200;
    //int nr = 72;
    //double length = 100;
    //double del = 2 * Math.PI / nr;
    //for (int i =0; i< nr; i++)
    //{
    //    int x2 = (int)(x + Math.Sin(del * i) * length);
    //    int y2 = (int)(y - Math.Cos(del * i) * length);
    //    wb.DrawLine(x, y, x2, y2, Color.FromRgb(255, 0, 0));
    //}
}

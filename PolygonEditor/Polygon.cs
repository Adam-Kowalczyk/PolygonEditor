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
            WriteableBitmap wb = new WriteableBitmap((int)Boundries.Width, (int)Boundries.Height, 96, 96, PixelFormats.Bgra32, null);
            wb.DrawPixel(100, 100, Color.FromArgb(255, 255, 0, 0));
            Bitmap = wb;
        }
    }
}

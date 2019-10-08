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

        public WriteableBitmap EditingBitmap
        {
            get
            {
                return editingBitmap;
            }
            set
            {
                editingBitmap = value;
                OnPropertyChanged(nameof(EditingBitmap));
            }
        }
        WriteableBitmap editingBitmap;

        public Color Color
        {
            get
            {
                return color;
                
            }
            set
            {
                if (color == value) return;
                color = value;
                OnPropertyChanged(nameof(Color));
            }
        }
        Color color = Color.FromArgb(255, 255, 0, 0);

        public void RenderBitmap(bool isCreating = false)
        {
            if (Points.Count < 2) return;
            WriteableBitmap wb = new WriteableBitmap((int)Boundries.Width, (int)Boundries.Height, 96, 96, PixelFormats.Bgra32, null);
            for(int i = 0; i<Points.Count;i++)
            {
                if(i<Points.Count - 1)
                {
                    wb.DrawLine((int)Points[i].X, (int)Points[i].Y, (int)Points[i + 1].X, (int)Points[i + 1].Y, Color);
                }
                else if(i == Points.Count - 1)
                {
                    if (!isCreating)
                    {
                        wb.DrawLine((int)Points[i].X, (int)Points[i].Y, (int)Points[0].X, (int)Points[0].Y, Color);
                    }
                }
            }
            Bitmap = wb;
        }

        public void RenderEditingBitmap(Point? mousePosition)
        {
            if(mousePosition == null)
            {
                Bitmap = null;
                return; 
            }
            WriteableBitmap wb = new WriteableBitmap((int)Boundries.Width, (int)Boundries.Height, 96, 96, PixelFormats.Bgra32, null);
            var mouseP = mousePosition.Value;

            wb.DrawLine((int)Points[Points.Count - 1].X, (int)Points[Points.Count - 1].Y, (int)mouseP.X, (int)mouseP.Y, Color);

            EditingBitmap = wb;
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

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

        public ObservableCollection<DragablePoint> Points
        {
            get
            {
                return points ?? (points = new ObservableCollection<DragablePoint>());
            }
        }
        ObservableCollection<DragablePoint> points;

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
        
        public Rect Area
        {
            get
            {
                if (Points.Count == 0) return new Rect();
                var top = Points.Min(x => x.Y);
                var bottom = Points.Max(x => x.Y);
                var left = Points.Min(x => x.X);
                var right = Points.Max(x => x.X);
                return new Rect(new Point(left, top), new Point(right, bottom));
            }
        }

        public bool IsHit(Point mousePosition)
        {
            var mrg = 5;
            if(mousePosition.X < Area.Right + mrg && mousePosition.X > Area.Left - mrg
                && mousePosition.Y > Area.Top - mrg && mousePosition.Y < Area.Bottom + mrg)
            {
                return true;
            }
            return false;
        }

        public void Move(Point finalPosition, Point startingPosition)
        {
            var xDiff = (int)(finalPosition.X - startingPosition.X);
            var yDiff = (int)(finalPosition.Y - startingPosition.Y);
            foreach(var p in Points)
            {
                p.X += xDiff;
                p.Y += yDiff;
            }

        }

        Int32Rect Cleaner
        {
            get
            {
                return !cleaner.IsEmpty ? cleaner : cleaner = new Int32Rect(0, 0, (int)Boundries.Width, (int)Boundries.Height);
            }
        }
        Int32Rect cleaner = new Int32Rect();


        public void RenderBitmap(bool isCreating = false, Point? mousePosition = null, Point? mouseStartingPosition = null)
        {
            if (Points.Count == 0) return;
            if(Bitmap == null)
                bitmap = new WriteableBitmap((int)Boundries.Width, (int)Boundries.Height, 96, 96, PixelFormats.Bgra32, null);

            int xDiff = 0;
            int yDiff = 0;

            if(mouseStartingPosition.HasValue && mousePosition.HasValue)
            {
                xDiff = (int)(mousePosition.Value.X - mouseStartingPosition.Value.X);
                yDiff = (int)(mousePosition.Value.Y - mouseStartingPosition.Value.Y);
            }

            byte[] pixels1d = new byte[(int)Boundries.Width * (int)Boundries.Height * 4];
            int stride = 4 * (int)Boundries.Width;
            bitmap.WritePixels(Cleaner, pixels1d, stride, 0);


            for (int i = 0; i<Points.Count;i++)
            {
                bitmap.DrawPoint((int)Points[i].X + xDiff, (int)Points[i].Y + yDiff, Color, 3);
                if(i<Points.Count - 1)
                {
                    
                    bitmap.DrawLine((int)Points[i].X + xDiff, (int)Points[i].Y + yDiff, (int)Points[i + 1].X + xDiff, (int)Points[i + 1].Y + yDiff, Color);
                }
                else if(i == Points.Count - 1)
                {
                    if (!isCreating)
                    {
                        bitmap.DrawLine((int)Points[i].X + xDiff, (int)Points[i].Y + yDiff, (int)Points[0].X + xDiff, (int)Points[0].Y + yDiff, Color);
                    }
                    else
                    {
                        if(mousePosition!= null)
                        {
                            var mouseP = mousePosition.Value;

                            bitmap.DrawLine((int)Points[Points.Count - 1].X, (int)Points[Points.Count - 1].Y, (int)mouseP.X, (int)mouseP.Y, Color);
                            bitmap.DrawPoint((int)mouseP.X, (int)mouseP.Y, Color, 3);
                        }
                    }
                }
            }
            Bitmap = bitmap;
        }

    }

}

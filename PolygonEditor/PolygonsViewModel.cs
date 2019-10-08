using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonEditor
{
    public class PolygonsViewModel:Observable
    {
        public ObservableCollection<Polygon> Polygons
        {
            get
            {
                return polygons ?? (polygons = new ObservableCollection<Polygon>());
            }
        }
        ObservableCollection<Polygon> polygons;

        public int BitmapWidth
        {
            get
            {
                return bitmapWidth;
            }
            set
            {
                if (bitmapWidth == value) return;
                bitmapWidth = value;
                OnPropertyChanged(nameof(BitmapWidth));
            }
        }
        int bitmapWidth = 640;

        public int BitmapHeight
        {
            get
            {
                return bitmapHeight;
            }
            set
            {
                if (bitmapHeight == value) return;
                bitmapHeight = value;
                OnPropertyChanged(nameof(BitmapWidth));
            }
        }
        int bitmapHeight = 480;
    }
}

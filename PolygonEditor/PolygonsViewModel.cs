using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

        public bool IsCreating
        {
            get
            {
                return isCreating;
            }
            set
            {
                if (isCreating == value) return;
                isCreating = value;
                OnPropertyChanged(nameof(IsCreating));
            }
        }
        bool isCreating;

        public Polygon SelectedPolygon
        {
            get
            {
                return selectedPolygon;
            }
            set
            {
                if (selectedPolygon == value) return;
                selectedPolygon = value;
                OnPropertyChanged(nameof(SelectedPolygon));

            }
        }
        Polygon selectedPolygon;

        public void CreateNewPolygon(object arg)
        {
            IsCreating = true;
            var poly = new Polygon();          
            poly.Boundries = new Rect(0, 0, BitmapWidth, BitmapHeight);
            Polygons.Add(poly);
            SelectedPolygon = poly;
        }

        public ICommand CreateNewPolygonCommand
        {
            get
            {
                return createNewPolygonCommand ?? (createNewPolygonCommand = new CustomCommand((x) => CreateNewPolygon(x)));
            }
        }
        ICommand createNewPolygonCommand;

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

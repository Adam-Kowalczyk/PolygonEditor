using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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

        public bool IsMoving
        {
            get
            {
                return isMoving;
            }
            set
            {
                if (isMoving == value) return;
                isMoving = value;
                OnPropertyChanged(nameof(IsMoving));
            }
        }
        bool isMoving;

        public bool IsPointDraged
        {
            get
            {
                return isPointDraged;
            }
            set
            {
                if (isPointDraged == value) return;
                isPointDraged = value;
                OnPropertyChanged(nameof(isPointDraged));
            }
        }
        bool isPointDraged;

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
            if (IsCreating) return;
            Random rnd = new Random();
            byte[] colors = new byte[3];
            rnd.NextBytes(colors);

            IsCreating = true;

            var poly = new Polygon();
            poly.Color = Color.FromRgb(colors[0],colors[1], colors[2]);
            poly.Name = "Poly" + Polygons.Count.ToString();
            AddPolygon(poly);
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

        public void AddPolygon(Polygon poly, int zIndex = -1)
        {
            poly.Boundries = new Rect(0, 0, BitmapWidth, BitmapHeight);
            if(zIndex >= 0)
            {
                Polygons.Insert(zIndex, poly);
            }
            else
            {
                Polygons.Add(poly);
            }
            
        }
    }
}

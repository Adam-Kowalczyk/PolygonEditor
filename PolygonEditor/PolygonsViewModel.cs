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

        public bool IsPointDragged
        {
            get
            {
                return isPointDragged;
            }
            set
            {
                if (isPointDragged == value) return;
                isPointDragged = value;
                OnPropertyChanged(nameof(IsPointDragged));
            }
        }
        bool isPointDragged;

        public bool IsLineDragged
        {
            get
            {
                return isLineDragged;
            }
            set
            {
                if (isLineDragged == value) return;
                isLineDragged = value;
                OnPropertyChanged(nameof(IsLineDragged));
            }
        }
        bool isLineDragged;

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

        public void DeletePoint(object arg)
        {
            var point = arg as DragablePoint;
            if (point == null) return;
            if (SelectedPolygon == null) return;

            if (SelectedPolygon.Points.Count <= 3)
            {
                var toDel = SelectedPolygon;
                Polygons.Remove(toDel);
                SelectedPolygon = null;
            }
            else
            {
                SelectedPolygon.DeletePoint(point);
                SelectedPolygon.RenderBitmap();
            }
        }

        public ICommand DeletePointCommand
        {
            get
            {
                return deletePointCommand ?? (deletePointCommand = new CustomCommand((x) => DeletePoint(x)));
            }
        }
        ICommand deletePointCommand;

        public void AddMiddlePoint(object arg)
        {
            var line = arg as Line;
            if (line == null) return;
            if (SelectedPolygon == null) return;
            var index = SelectedPolygon.Lines.IndexOf(line);
            var toAdd = new DragablePoint((line.First.X + line.Second.X) / 2, (line.First.Y + line.Second.Y) / 2);
            SelectedPolygon.AddPoint(toAdd, index);
            SelectedPolygon.RenderBitmap();
        }

        public ICommand AddMiddlePointCommand
        {
            get
            {
                return addMiddlePointCommand ?? (addMiddlePointCommand = new CustomCommand((x) => AddMiddlePoint(x)));
            }
        }
        ICommand addMiddlePointCommand;

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

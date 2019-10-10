using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolygonEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var vm = new PolygonsViewModel();
            DataContext = vm;
            
        }


        private void DrawArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as PolygonsViewModel;
            var drawArea = sender as ItemsControl;
            var pos = e.GetPosition(drawArea);
            int x = (int)(pos.X / drawArea.ActualWidth * vm.BitmapWidth);
            int y = (int)(pos.Y / drawArea.ActualHeight * vm.BitmapHeight);
            if (vm.IsCreating)
            {
                if (vm.SelectedPolygon.Points.Count > 3)
                {
                    var dist = PointsHelpers.Distance(vm.SelectedPolygon.Points[0].X, vm.SelectedPolygon.Points[0].Y, x, y);
                    Debug.WriteLine("Distance: " + dist.ToString());
                }

                if (vm.SelectedPolygon.Points.Count >= 3 && PointsHelpers.Distance(vm.SelectedPolygon.Points[0].X, vm.SelectedPolygon.Points[0].Y, x, y) <= 15)
                {
                    vm.IsCreating = false;
                    vm.SelectedPolygon.RenderBitmap();
                }
                else
                {
                    vm.SelectedPolygon.Points.Add(new DragablePoint(x, y));
                    vm.SelectedPolygon.RenderBitmap(true);
                }
            }
            else
            {
                Polygon clicked = null;
                foreach(var poly in vm.Polygons.Reverse())
                {
                    if(poly.IsHit(new Point(x,y)))
                    {
                        clicked = poly;
                        break;
                    }
                }
                vm.SelectedPolygon = clicked;
                if(clicked == null)
                    Debug.WriteLine("Selected: null");
                else
                    Debug.WriteLine("Selected:" + clicked.Name);
                if (vm.SelectedPolygon != null)
                {

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        foreach (var pnt in vm.SelectedPolygon.Points)
                        {
                            if (pnt.IsHit(x, y, 6))
                            {
                                selectedPoint = pnt;
                                dragStartPoint = new Point(x, y);
                                vm.IsPointDraged = true;
                                return;
                            }
                        }


                        dragStartPoint = new Point(x, y);
                        vm.IsMoving = true;
                    }
                }

            }
        }


        Point? dragStartPoint = null;

        DragablePoint selectedPoint = null;


        private void DrawArea_MouseMove(object sender, MouseEventArgs e)
        {
            var vm = DataContext as PolygonsViewModel;
            var drawArea = sender as ItemsControl;
            var pos = e.GetPosition(drawArea);
            int x = (int)(pos.X / drawArea.ActualWidth * vm.BitmapWidth);
            int y = (int)(pos.Y / drawArea.ActualHeight * vm.BitmapHeight);
            if (vm.IsCreating)
            {
                if(vm.SelectedPolygon.Points.Count>0)
                    vm.SelectedPolygon.RenderBitmap(vm.IsCreating, new Point(x,y));
            }
            else if(vm.SelectedPolygon != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if(dragStartPoint.HasValue)
                    {
                        var xDiff = (int)(x - dragStartPoint.Value.X);
                        var yDiff = (int)(y - dragStartPoint.Value.Y);
                        if (vm.IsMoving)
                        {
                            var area = vm.SelectedPolygon.Area;
                            if (area.Left + xDiff < 0)
                            {
                                x = (int)(-area.Left + dragStartPoint.Value.X);
                            }
                            if (area.Right + xDiff >= vm.SelectedPolygon.Boundries.Width)
                            {
                                x = (int)(vm.SelectedPolygon.Boundries.Width - area.Right + dragStartPoint.Value.X);
                            }
                            if (area.Top + yDiff < 0)
                            {
                                y = (int)(-area.Top + dragStartPoint.Value.Y);
                            }
                            if (area.Bottom + yDiff >= vm.SelectedPolygon.Boundries.Height)
                            {
                                y = (int)(vm.SelectedPolygon.Boundries.Height - area.Bottom + dragStartPoint.Value.Y);
                            }
                            vm.SelectedPolygon.RenderBitmap(mousePosition: new Point(x, y), mouseStartingPosition: dragStartPoint);
                        }
                        else if(vm.IsPointDraged)
                        {
                            if(selectedPoint != null)
                            {
                                selectedPoint.X = x;
                                selectedPoint.Y = y;
                            }
                            vm.SelectedPolygon.RenderBitmap();
                        }
                        
                    }
                }
            }
        }

        private void DrawArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as PolygonsViewModel;
            var drawArea = sender as ItemsControl;
            var pos = e.GetPosition(drawArea);
            int x = (int)(pos.X / drawArea.ActualWidth * vm.BitmapWidth);
            int y = (int)(pos.Y / drawArea.ActualHeight * vm.BitmapHeight);
            if (vm.IsCreating)
            {

            }
            else if (vm.SelectedPolygon != null)
            {
                if (dragStartPoint.HasValue)
                {
                    var xDiff = (int)(x - dragStartPoint.Value.X);
                    var yDiff = (int)(y - dragStartPoint.Value.Y);
                    if (vm.IsMoving)
                    {
                        var area = vm.SelectedPolygon.Area;
                        if (area.Left + xDiff < 0)
                        {
                            x = (int)(-area.Left + dragStartPoint.Value.X);
                        }
                        if (area.Right + xDiff >= vm.SelectedPolygon.Boundries.Width)
                        {
                            x = (int)(vm.SelectedPolygon.Boundries.Width - area.Right + dragStartPoint.Value.X);
                        }
                        if (area.Top + yDiff < 0)
                        {
                            y = (int)(-area.Top + dragStartPoint.Value.Y);
                        }
                        if (area.Bottom + yDiff >= vm.SelectedPolygon.Boundries.Height)
                        {
                            y = (int)(vm.SelectedPolygon.Boundries.Height - area.Bottom + dragStartPoint.Value.Y);
                        }
                        vm.SelectedPolygon.Move(new Point(x, y), dragStartPoint.Value);
                        vm.IsMoving = false;
                       
                    }
                    else if (vm.IsPointDraged)
                    {
                        selectedPoint = null;
                        vm.IsPointDraged = false;
                    }

                    vm.SelectedPolygon.RenderBitmap();
                    dragStartPoint = null;
                }
            }
        }
    }
}

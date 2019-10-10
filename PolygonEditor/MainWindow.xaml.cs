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


        private void DrawArea_LeftMouseDown(object sender, MouseButtonEventArgs e)
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
                    vm.SelectedPolygon.AddPoint(new DragablePoint(x, y));
                    vm.SelectedPolygon.RenderBitmap(true);
                }
            }
            else
            {
                Polygon clicked = null;
                foreach (var poly in vm.Polygons.Reverse())
                {
                    if (poly.IsHit(new Point(x, y)))
                    {
                        clicked = poly;
                        break;
                    }
                }
                vm.SelectedPolygon = clicked;
                if (clicked == null)
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
                                vm.IsPointDragged = true;
                                return;
                            }
                        }

                        for (int i = 0; i < vm.SelectedPolygon.Points.Count; i++)
                        {
                            if (i == vm.SelectedPolygon.Points.Count - 1)
                            {
                                if (PointsHelpers.DistanceToLine(vm.SelectedPolygon.Points[i].X, vm.SelectedPolygon.Points[i].Y,
                                    vm.SelectedPolygon.Points[0].X, vm.SelectedPolygon.Points[0].Y, x, y) <= 6)
                                {
                                    dragStartPoint = new Point(x, y);
                                    selectedLine = (vm.SelectedPolygon.Points[i], vm.SelectedPolygon.Points[0]);
                                    vm.IsLineDragged = true;
                                    return;
                                }
                            }
                            else
                            {
                                if (PointsHelpers.DistanceToLine(vm.SelectedPolygon.Points[i].X, vm.SelectedPolygon.Points[i].Y,
                                        vm.SelectedPolygon.Points[i + 1].X, vm.SelectedPolygon.Points[i + 1].Y, x, y) <= 6)
                                {
                                    dragStartPoint = new Point(x, y);
                                    selectedLine = (vm.SelectedPolygon.Points[i], vm.SelectedPolygon.Points[i + 1]);
                                    vm.IsLineDragged = true;
                                    return;
                                }
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

        (DragablePoint, DragablePoint) selectedLine = (null, null);


        private void DrawArea_MouseMove(object sender, MouseEventArgs e)
        {
            var vm = DataContext as PolygonsViewModel;
            var drawArea = sender as ItemsControl;
            var pos = e.GetPosition(drawArea);
            int x = (int)(pos.X / drawArea.ActualWidth * vm.BitmapWidth);
            int y = (int)(pos.Y / drawArea.ActualHeight * vm.BitmapHeight);
            if (vm.IsCreating)
            {
                if (vm.SelectedPolygon.Points.Count > 0)
                    vm.SelectedPolygon.RenderBitmap(vm.IsCreating, new Point(x, y));
            }
            else if (vm.SelectedPolygon != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (dragStartPoint.HasValue)
                    {
                        var xDiff = (int)(x - dragStartPoint.Value.X);
                        var yDiff = (int)(y - dragStartPoint.Value.Y);

                        if (vm.IsMoving)
                        {
                            double xOff = x - dragStartPoint.Value.X;
                            double yOff = y - dragStartPoint.Value.Y;
                            var area = vm.SelectedPolygon.Area;
                            if (area.Left + xDiff < 0)
                            {
                                xOff = -area.Left;
                            }
                            if (area.Right + xDiff >= vm.SelectedPolygon.Boundries.Width)
                            {
                                xOff = vm.SelectedPolygon.Boundries.Width - area.Right;
                            }
                            if (area.Top + yDiff < 0)
                            {
                                yOff = -area.Top;
                            }
                            if (area.Bottom + yDiff >= vm.SelectedPolygon.Boundries.Height)
                            {
                                yOff = vm.SelectedPolygon.Boundries.Height - area.Bottom;
                            }
                            vm.SelectedPolygon.SetOffestToAllPoints((int)xOff, (int)yOff);
                            vm.SelectedPolygon.RenderBitmap(mousePosition: dragStartPoint, mouseStartingPosition: dragStartPoint);
                        }
                        else if (vm.IsPointDragged)
                        {
                            if (selectedPoint != null)
                            {
                                selectedPoint.X = x;
                                selectedPoint.Y = y;
                            }
                            vm.SelectedPolygon.RenderBitmap();
                        }
                        else if (vm.IsLineDragged)
                        {
                            if (selectedLine.Item1 != null && selectedLine.Item2 != null)
                            {
                                selectedLine.Item1.XOffset = xDiff;
                                selectedLine.Item1.YOffset = yDiff;
                                selectedLine.Item2.XOffset = xDiff;
                                selectedLine.Item2.YOffset = yDiff;
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
                    else if (vm.IsPointDragged)
                    {
                        selectedPoint = null;
                        vm.IsPointDragged = false;
                    }
                    else if(vm.IsLineDragged)
                    {
                        if (selectedLine.Item1 != null && selectedLine.Item2 != null)
                        {
                            Debug.WriteLine($"xDiff: {xDiff} yDiff: {yDiff}");
                            selectedLine.Item1.X += xDiff;
                            selectedLine.Item1.Y += yDiff;
                            selectedLine.Item2.X += xDiff;
                            selectedLine.Item2.Y += yDiff;
                        }
                        selectedLine = (null, null);
                        vm.IsLineDragged = false;
                    }
                    vm.SelectedPolygon.SetOffestToAllPoints(0, 0);
                    vm.SelectedPolygon.RenderBitmap();
                    dragStartPoint = null;
                }
            }
        }
    }
}

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
                        if (!vm.IsAddingRelation)
                        {
                            foreach (var pnt in vm.SelectedPolygon.Points)
                            {
                                if (pnt.IsHit(x, y, 6))
                                {
                                    if (!pnt.IsBlocked)
                                    {
                                        selectedPoint = pnt;
                                        dragStartPoint = new Point(x, y);
                                        vm.IsPointDragged = true;
                                        return;
                                    }
                                }
                            }
                        }

                        foreach( var line in vm.SelectedPolygon.Lines)
                        {
                            if(line.IsHit(x,y, 6))
                            {
                                if (vm.IsAddingRelation)
                                {
                                    if (line.Relation == Relation.NONE)
                                    {
                                        vm.AddRelationEnd(line);
                                        vm.SelectedPolygon.FixRelations(line.Second);
                                    }
                                    
                                }
                                else
                                {
                                    if (!(line.First.IsBlocked || line.Second.IsBlocked))
                                    {
                                        dragStartPoint = new Point(x, y);
                                        selectedLine = line;
                                        lineInitial = (new DragablePoint(line.First.X, line.First.Y), new DragablePoint(line.Second.X, line.Second.Y));
                                        vm.IsLineDragged = true;
                                        return;
                                    }
                                }
                            }
                        }

                        if (!vm.IsAddingRelation && !vm.SelectedPolygon.IsAnyPointBlocked)
                        {
                            dragStartPoint = new Point(x, y);
                            vm.IsMoving = true;
                        }
                    }
                }

            }
        }


        Point? dragStartPoint = null;

        DragablePoint selectedPoint = null;
        Line selectedLine;
        (DragablePoint, DragablePoint) lineInitial = (null, null);


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
                            vm.SelectedPolygon.SetOffestToAllPoints((int)xOff, (int)yOff);
                            vm.SelectedPolygon.RenderBitmap(mousePosition: dragStartPoint);
                        }
                        else if (vm.IsPointDragged)
                        {
                            if (selectedPoint != null)
                            {
                                selectedPoint.X = x;
                                selectedPoint.Y = y;
                            }
                            vm.SelectedPolygon.FixRelations(selectedPoint);
                            vm.SelectedPolygon.RenderBitmap();
                        }
                        else if (vm.IsLineDragged)
                        {
                            if (lineInitial.Item1 != null && lineInitial.Item2 != null && selectedLine!= null)
                            {

                                selectedLine.First.X = lineInitial.Item1.X + x - (int)dragStartPoint.Value.X;
                                selectedLine.First.Y = lineInitial.Item1.Y + y - (int)dragStartPoint.Value.Y;

                                selectedLine.Second.X = lineInitial.Item2.X + x - (int)dragStartPoint.Value.X;
                                selectedLine.Second.Y = lineInitial.Item2.Y + y - (int)dragStartPoint.Value.Y;

                            }
                            vm.SelectedPolygon.FixRelations(selectedLine);
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
                        if (lineInitial.Item1 != null && lineInitial.Item2 != null)
                        {
                            //Debug.WriteLine($"xDiff: {xDiff} yDiff: {yDiff}");
                            //lineInitial.Item1.X += xDiff;
                            //lineInitial.Item1.Y += yDiff;
                            //lineInitial.Item2.X += xDiff;
                            //lineInitial.Item2.Y += yDiff;
                        }
                        lineInitial = (null, null);
                        selectedLine = null;
                        vm.IsLineDragged = false;
                    }
                    vm.SelectedPolygon.SetOffestToAllPoints(0, 0);
                    vm.SelectedPolygon.RenderBitmap();
                    dragStartPoint = null;
                }
            }
        }

        private void DrawArea_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as PolygonsViewModel;
            var drawArea = sender as ItemsControl;
            var pos = e.GetPosition(drawArea);
            int x = (int)(pos.X / drawArea.ActualWidth * vm.BitmapWidth);
            int y = (int)(pos.Y / drawArea.ActualHeight * vm.BitmapHeight);
            if (vm.IsCreating || vm.IsMoving || vm.IsLineDragged || vm.IsPointDragged || vm.IsAddingRelation) return;

            if (drawArea.ContextMenu != null)
            {
                drawArea.ContextMenu.IsOpen = false;
            }

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

            drawArea.ContextMenu = null;
            if (vm.SelectedPolygon != null)
            {

                foreach (var pnt in vm.SelectedPolygon.Points)
                {
                    if (pnt.IsHit(x, y, 6))
                    {
                        var ctxMenu = new ContextMenu();
                        drawArea.ContextMenu = ctxMenu;
                        var deleteItem = new MenuItem();
                        deleteItem.Header = "Delete point";
                        deleteItem.Command = vm.DeletePointCommand;
                        deleteItem.CommandParameter = pnt;

                        ctxMenu.Items.Add(deleteItem);

                        var blockPoint = new MenuItem();
                        blockPoint.Header = "Change blocked/unblocked";
                        blockPoint.Command = vm.ChangeBlockingStatusCommand;
                        blockPoint.CommandParameter = pnt;

                        ctxMenu.Items.Add(blockPoint);
                        ctxMenu.IsOpen = true;
                        return;
                    }
                }

                foreach (var line in vm.SelectedPolygon.Lines)
                {
                    if (line.IsHit(x, y, 6))
                    {
                        var ctxMenu = new ContextMenu();
                        drawArea.ContextMenu = ctxMenu;
                        var addPoint = new MenuItem();
                        addPoint.Header = "Add middle point";
                        addPoint.Command = vm.AddMiddlePointCommand;
                        addPoint.CommandParameter = line;
                        var addRelation = new MenuItem();
                        addRelation.Header = "Add 'parallel' relation";
                        addRelation.Command = vm.AddParallelRelationCommand;
                        addRelation.CommandParameter = line;
                        addRelation.IsEnabled = line.Relation == Relation.NONE;
                        var addEqualRelation = new MenuItem();
                        addEqualRelation.Header = "Add 'equal' relation";
                        addEqualRelation.Command = vm.AddEqualRelationCommand;
                        addEqualRelation.CommandParameter = line;
                        addEqualRelation.IsEnabled = line.Relation == Relation.NONE;
                        ctxMenu.Items.Add(addPoint);
                        ctxMenu.Items.Add(addRelation);
                        ctxMenu.Items.Add(addEqualRelation);
                        if(line.Relation != Relation.NONE)
                        {
                            var delRelation = new MenuItem();
                            delRelation.Header = "Delete relation";
                            delRelation.Command = vm.DeleteRelationCommand;
                            delRelation.CommandParameter = line;
                            ctxMenu.Items.Add(delRelation);
                        }
                        ctxMenu.IsOpen = true;
                        return;
                    }

                }

                drawArea.ContextMenu = null;
                var contextMenu = new ContextMenu();
                drawArea.ContextMenu = contextMenu;
                var deletePoly = new MenuItem();
                deletePoly.Header = "Delete polygon";
                deletePoly.Command = vm.DeletePolygonCommand;
                deletePoly.CommandParameter = vm.SelectedPolygon;
                contextMenu.Items.Add(deletePoly);
                contextMenu.IsOpen = true;

            }
        }

    }
}

using System;
using System.Collections.Generic;
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
            if (vm.IsCreating)
            {
                var pos = e.GetPosition(drawArea);
                int x = (int)(pos.X / drawArea.ActualWidth * vm.BitmapWidth);
                int y = (int)(pos.Y / drawArea.ActualHeight * vm.BitmapHeight);

                if (vm.SelectedPolygon.Points.Count > 3 && PointsHelpers.Distance(vm.SelectedPolygon.Points[0].X, vm.SelectedPolygon.Points[0].Y, x, y) <= 5)
                {
                    vm.IsCreating = false;
                    vm.SelectedPolygon.RenderBitmap();
                    vm.SelectedPolygon.EditingBitmap = null;
                }
                else
                {
                    vm.SelectedPolygon.Points.Add(new Point(x, y));
                    vm.SelectedPolygon.RenderBitmap(true);
                }
            }
        }

        private void DrawArea_MouseMove(object sender, MouseEventArgs e)
        {
            var vm = DataContext as PolygonsViewModel;
            var drawArea = sender as ItemsControl;
            if (vm.IsCreating)
            {
                var pos = e.GetPosition(drawArea);
                int x = (int)(pos.X / drawArea.ActualWidth * vm.BitmapWidth);
                int y = (int)(pos.Y / drawArea.ActualHeight * vm.BitmapHeight);

                if(vm.SelectedPolygon.Points.Count>0)
                    vm.SelectedPolygon.RenderEditingBitmap(new Point(x,y));
            }
        }
    }
}

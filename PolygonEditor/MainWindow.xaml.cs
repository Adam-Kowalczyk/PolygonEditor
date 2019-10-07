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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as PolygonsViewModel;
            var poly = new Polygon();
            poly.Boundries = new Rect(0, 0, DrawArea.ActualWidth, DrawArea.ActualHeight);
            poly.RenderBitmap();
            vm.Polygons.Add(poly);
        }
    }
}

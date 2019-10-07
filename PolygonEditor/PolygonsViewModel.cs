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


    }
}

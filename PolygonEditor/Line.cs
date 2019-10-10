using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonEditor
{
    public class Line
    {
        public Line(DragablePoint first, DragablePoint second)
        {
            First = first;
            Second = second;
        }
        public DragablePoint First { get; set; }

        public DragablePoint Second { get; set; }
    }
}

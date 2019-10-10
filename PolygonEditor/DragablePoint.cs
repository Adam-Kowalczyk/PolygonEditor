using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolygonEditor
{
    public class DragablePoint
    {
        public DragablePoint(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X
        {
            get; set;
        }

        public int Y
        {
            get; set;
        }

        public int XOffset { get; set; } = 0;

        public int YOffset { get; set; } = 0;

        public Point ToWpfPoint()
        {
            return new Point(X, Y);
        }

        public bool IsHit(int x1, int y1, int margin)
        {
            return PointsHelpers.Distance(X, Y, x1, y1) <= margin;
        }
    }
}

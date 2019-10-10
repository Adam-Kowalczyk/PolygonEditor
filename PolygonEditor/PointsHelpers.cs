using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonEditor
{
    public static class PointsHelpers
    {
        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        public static double DistanceToLine(double x1, double y1, double x2, double y2, double xP, double yP) //http://csharphelper.com/blog/2016/09/find-the-shortest-distance-between-a-point-and-a-line-segment-in-c/
        {
            double dx = x2- x1;
            double dy = y2 - y1;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                dx = xP - x1;
                dy = yP - y1;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            double t = ((xP - x1) * dx + (yP - y1) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                dx = xP - x1;
                dy = yP - y1;
            }
            else if (t > 1)
            {
                dx = xP - x2;
                dy = yP - y2;
            }
            else
            {
                dx = xP - x1 - t * dx;
                dy = yP - y1 - t * dy;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}

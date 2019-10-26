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

        public Relation Relation { get; set; } = Relation.NONE;

        public Line RelatedLine { get; set; }

        public int RelationID = -1;

        public void RemoveRelation()
        {
            RelationID = -1;
            Relation = Relation.NONE;
            if(RelatedLine!= null)
            {
                RelatedLine.RelatedLine = null;
                RelatedLine.Relation = Relation.NONE;
                RelatedLine.RelationID = -1;
            }
            RelatedLine = null;
        }

        public bool IsHit(int x, int y, int margin)
        {
            return PointsHelpers.DistanceToLine(First.X, First.Y,
                                        Second.X, Second.Y, x, y) <= margin;
        }
    }

    public enum Relation
    {
        NONE = 0,
        EQUALS = 1,
        PARALLEL = 2,
    }
}

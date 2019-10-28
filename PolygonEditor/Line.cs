using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public double Angle
        {
            get
            {
                if (Second.X == First.X) return 100;
                var p1 = (double)(Second.Y - First.Y);
                return p1 / (Second.X - First.X);
            }
        }

        public double Length
        {
            get
            {
                return PointsHelpers.Distance(First.X, First.Y, Second.X, Second.Y);
            }
        }

        public void SetLength(double length, bool changeFirst)
        {
            
            var angle = Angle;
            var change = length / Math.Sqrt((1 + angle * angle));
            if (First.X == Second.X)
            {
                if(changeFirst)
                {
                    if(First.Y > Second.Y)
                    {
                        First.Y = Second.Y + (int)length;
                    }
                    else
                    {
                        First.Y = Second.Y - (int)length;
                    }
                }
                else
                {
                    if (First.Y < Second.Y)
                    {
                        Second.Y = First.Y + (int)length;
                    }
                    else
                    {
                        Second.Y = First.Y - (int)length;
                    }
                }
                return;
            }
            if (changeFirst)
            {
                double newX;
                if (First.X < Second.X)
                {
                    newX = Second.X - change;
                }
                else
                {
                    newX = Second.X + change;
                }
                First.X = (int)newX;
                First.Y = (int)(angle * (newX- Second.X) + Second.Y);
            }
            else
            {
                double newX;
                if (Second.X < First.X)
                {
                    newX = First.X - change;
                }
                else
                {
                    newX =First.X + change;
                }
                Second.X = (int)newX;
                Second.Y = (int)(angle * (newX - First.X) + First.Y);
            }
            

        }

        public void SetAngle(double angle, bool changeFirst)
        {
            if (angle < 1 && angle > -1)
            {
                if (changeFirst)
                {
                    First.Y = (int)(angle * (First.X - Second.X) + Second.Y);
                }
                else
                {
                    Second.Y = (int)(angle * (Second.X - First.X) + First.Y);
                }
            }
            else
            {
                if(changeFirst)
                {
                    First.X = Second.X + (int)((First.Y - Second.Y) / angle);
                }
                else
                {
                    Second.X = First.X + (int)((Second.Y - First.Y) / angle);
                }
            }
        }

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

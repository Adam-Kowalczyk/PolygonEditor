using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PolygonEditor
{
    public class Polygon : Observable
    {
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name == value) return;
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        string name;

        public Rect Boundries { get; set; }

        public ObservableCollection<DragablePoint> Points
        {
            get
            {
                return points ?? (points = new ObservableCollection<DragablePoint>());
            }
        }
        ObservableCollection<DragablePoint> points;

        public List<Line> Lines
        {
            get
            {
                return lines ?? (lines = new List<Line>());
            }
        }
        List<Line> lines = null;

        public void AddPoint(DragablePoint point, int index = -1) // index is Line in which we insert point
        {
            if(Points.Count == 0)
            {
                Points.Add(point);
                return;
            }
            if(index < 0)
            {
                if(Lines.Count > 0)
                {
                    Lines.Last().Second = point;
                }
                else
                {
                    Lines.Add(new Line(Points.First(), point));
                }
                Lines.Add(new Line(point, Points.First()));
                Points.Add(point);
                return;
            }
            else
            {
                var oldEnd = Lines[index].Second;
                Lines[index].Second = point;
                Lines.Insert(index + 1, new Line(point, oldEnd));
                Points.Insert(index + 1, point);
            }
        }

        public WriteableBitmap Bitmap
        {
            get
            {
                return bitmap;
            }
            set
            {
                bitmap = value;
                OnPropertyChanged(nameof(Bitmap));
            }
        }
        WriteableBitmap bitmap;

        public Color Color
        {
            get
            {
                return color;
                
            }
            set
            {
                if (color == value) return;
                color = value;
                OnPropertyChanged(nameof(Color));
            }
        }
        Color color = Color.FromArgb(255, 255, 0, 0);
        
        public Rect Area
        {
            get
            {
                if (Points.Count == 0) return new Rect();
                var top = Points.Min(x => x.Y);
                var bottom = Points.Max(x => x.Y);
                var left = Points.Min(x => x.X);
                var right = Points.Max(x => x.X);
                return new Rect(new Point(left, top), new Point(right, bottom));
            }
        }


        public bool IsHit(Point mousePosition)
        {
            var mrg = 5;
            if(mousePosition.X < Area.Right + mrg && mousePosition.X > Area.Left - mrg
                && mousePosition.Y > Area.Top - mrg && mousePosition.Y < Area.Bottom + mrg)
            {
                return true;
            }
            return false;
        }

        public void Move(Point finalPosition, Point startingPosition)
        {
            var xDiff = (int)(finalPosition.X - startingPosition.X);
            var yDiff = (int)(finalPosition.Y - startingPosition.Y);
            foreach(var p in Points)
            {
                p.X += xDiff;
                p.Y += yDiff;
            }

        }

        Int32Rect Cleaner
        {
            get
            {
                return !cleaner.IsEmpty ? cleaner : cleaner = new Int32Rect(0, 0, (int)Boundries.Width, (int)Boundries.Height);
            }
        }
        Int32Rect cleaner = new Int32Rect();

        public void SetOffestToAllPoints(int xDiff, int yDiff)
        {
            foreach(var point in Points)
            {
                point.XOffset = xDiff;
                point.YOffset = yDiff;
            }
        }

        public void RenderBitmap(bool isCreating = false, Point? mousePosition = null)
        {
            if (Points.Count == 0) return;
            if(Bitmap == null)
                bitmap = new WriteableBitmap((int)Boundries.Width, (int)Boundries.Height, 96, 96, PixelFormats.Bgra32, null);

            byte[] pixels1d = new byte[(int)Boundries.Width * (int)Boundries.Height * 4];
            int stride = 4 * (int)Boundries.Width;
            bitmap.WritePixels(Cleaner, pixels1d, stride, 0);


            for (int i = 0; i<Points.Count;i++)
            {
                bitmap.DrawPoint(Points[i].X + Points[i].XOffset, (int)Points[i].Y + Points[i].YOffset, Color, 3);
                if(i<Points.Count - 1)
                {
                    
                    bitmap.DrawLine((int)Points[i].X + Points[i].XOffset, (int)Points[i].Y + Points[i].YOffset, (int)Points[i + 1].X + Points[i + 1].XOffset, (int)Points[i + 1].Y + Points[i + 1].YOffset, Color);
                }
                else if(i == Points.Count - 1)
                {
                    if (!isCreating)
                    {
                        bitmap.DrawLine((int)Points[i].X + Points[i].XOffset, (int)Points[i].Y + Points[i].YOffset, (int)Points[0].X + Points[0].XOffset, (int)Points[0].Y + Points[0].YOffset, Color);
                    }
                    else
                    {
                        if(mousePosition!= null)
                        {
                            var mouseP = mousePosition.Value;

                            bitmap.DrawLine((int)Points[Points.Count - 1].X, (int)Points[Points.Count - 1].Y, (int)mouseP.X, (int)mouseP.Y, Color);
                            bitmap.DrawPoint((int)mouseP.X, (int)mouseP.Y, Color, 3);
                        }
                    }
                }
            }
            if(!isCreating)
            {
                int id = 1;
                foreach(var line in Lines)
                {
                    if(line.Relation!= Relation.NONE)
                    {
                        if(line.RelationID == -1 && line.RelatedLine!= null)
                        {
                            line.RelationID = id;
                            line.RelatedLine.RelationID = id;
                            id++;
                        }

                        bitmap.DrawRelationBox((line.First.X + line.First.XOffset + line.Second.X + line.Second.XOffset) / 2,
                            (line.First.Y + line.First.YOffset + line.Second.Y + line.Second.YOffset) / 2,
                            line.Relation ,line.RelationID);
                        line.RelationID = -1;
                    }
                }
            }
            Bitmap = bitmap;
        }

        public void DeletePoint(DragablePoint point)
        {
            var pos = Points.IndexOf(point);
            if (pos == -1) return;

            var atBeginning = Lines.Where(x => x.First == point).FirstOrDefault();
            var atEnding = Lines.Where(x => x.Second == point).FirstOrDefault();

            if (atBeginning != null && atEnding != null)
            {
                atBeginning.RemoveRelation();
                atEnding.RemoveRelation();
                DragablePoint toJoin = atBeginning.Second;
                atEnding.Second = toJoin;
                Lines.Remove(atBeginning);
            }

            Points.Remove(point);
        }

        public bool FixRelations(Line line)
        {
            var atBeginning = Lines.Where(x => x.First == line.Second).FirstOrDefault();
            var atEnding = Lines.Where(x => x.Second == line.First).FirstOrDefault();

            if (atBeginning == null || atEnding == null) return false;

            if (atBeginning.Relation == Relation.NONE && atEnding.Relation == Relation.NONE) return true;

            if (atBeginning.Relation != Relation.NONE)
            {
                FixRelationFromLine(atBeginning, true);
            }

            if (atEnding.Relation != Relation.NONE)
            {
                FixRelationFromLine(atEnding, false);
            }

            return true;
        }
        public bool FixRelations(DragablePoint point)
        {
            var atBeginning = Lines.Where(x => x.First == point).FirstOrDefault();
            var atEnding = Lines.Where(x => x.Second == point).FirstOrDefault();

            if (atBeginning == null || atEnding == null) return false;

            if (atBeginning.Relation == Relation.NONE && atEnding.Relation == Relation.NONE) return true;

            if(atBeginning.Relation != Relation.NONE)
            {
                FixRelationFromLine(atBeginning, true);
            }

            if(atEnding.Relation != Relation.NONE)
            {
                FixRelationFromLine(atEnding, false);
            }

            return true;
        }

        public bool FixRelationFromLine(Line line, bool fixFirst)
        {
            if (line.Relation == Relation.NONE) return true;

            var next = Lines.Where(x => x.First == line.RelatedLine.Second).FirstOrDefault();
            var prev = Lines.Where(x => x.Second == line.RelatedLine.First).FirstOrDefault();

            if (line.Relation == Relation.EQUALS)
            {
                if (Math.Abs(line.Length - line.RelatedLine.Length) < 3) return true;
                bool changeFirst = fixFirst;
                if (next.Relation == Relation.NONE)
                    changeFirst = false;
                if (prev.Relation == Relation.NONE)
                    changeFirst = true;
                line.RelatedLine.SetLength(line.Length, changeFirst);

                Debug.WriteLine($"[E]Ordered: {line.Length}, Set: {line.RelatedLine.Length}");

                if (changeFirst)
                {
                    if (prev == line) return true;
                    FixRelationFromLine(prev, true);
                } 
                else
                {
                    if (next == line) return true;
                    FixRelationFromLine(next, false);
                }
                
                    
                //FixRelationFromLine(next, true);
                //FixRelationFromLine(prev, false);
            }
            else if(line.Relation == Relation.PARALLEL)
            {
                if (Math.Abs((line.Angle - line.RelatedLine.Angle)/ line.Angle) < 0.05) return true;

                bool changeFirst = fixFirst;
                if (next.Relation == Relation.NONE)
                    changeFirst = false;
                if (prev.Relation == Relation.NONE)
                    changeFirst = true;
                line.RelatedLine.SetAngle(line.Angle, changeFirst);

                Debug.WriteLine($"[P]Ordered: {line.Angle}, Set: {line.RelatedLine.Angle}");

                if (changeFirst)
                {
                    if (prev == line) return true;
                    FixRelationFromLine(prev, true);
                }
                else
                {
                    if (next == line) return true;
                    FixRelationFromLine(next, false);
                }
            }

            return true;
        }

        public void AddRelation(Line target1, Line target2, Relation rel)
        {
            target1.Relation = rel;
            target2.Relation = rel;
            target1.RelatedLine = target2;
            target2.RelatedLine = target1;

        }


    }

}

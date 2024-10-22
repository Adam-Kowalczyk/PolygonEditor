﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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

        public bool IsCreating
        {
            get
            {
                return isCreating;
            }
            set
            {
                if (isCreating == value) return;
                isCreating = value;
                OnPropertyChanged(nameof(IsCreating));
            }
        }
        bool isCreating;

        public bool IsMoving
        {
            get
            {
                return isMoving;
            }
            set
            {
                if (isMoving == value) return;
                isMoving = value;
                OnPropertyChanged(nameof(IsMoving));
            }
        }
        bool isMoving;

        public bool IsPointDragged
        {
            get
            {
                return isPointDragged;
            }
            set
            {
                if (isPointDragged == value) return;
                isPointDragged = value;
                OnPropertyChanged(nameof(IsPointDragged));
            }
        }
        bool isPointDragged;

        public bool IsLineDragged
        {
            get
            {
                return isLineDragged;
            }
            set
            {
                if (isLineDragged == value) return;
                isLineDragged = value;
                OnPropertyChanged(nameof(IsLineDragged));
            }
        }
        bool isLineDragged;

        public Polygon SelectedPolygon
        {
            get
            {
                return selectedPolygon;
            }
            set
            {
                if (selectedPolygon == value) return;
                selectedPolygon = value;
                OnPropertyChanged(nameof(SelectedPolygon));

            }
        }
        Polygon selectedPolygon;

        public void CreateNewPolygon(object arg)
        {
            if (IsCreating) return;
            Random rnd = new Random();
            byte[] colors = new byte[3];
            rnd.NextBytes(colors);

            IsCreating = true;

            var poly = new Polygon();
            poly.Color = Color.FromRgb(colors[0],colors[1], colors[2]);
            poly.Name = "Poly" + Polygons.Count.ToString();
            AddPolygon(poly);
            SelectedPolygon = poly;
        }

        public ICommand CreateNewPolygonCommand
        {
            get
            {
                return createNewPolygonCommand ?? (createNewPolygonCommand = new CustomCommand((x) => CreateNewPolygon(x)));
            }
        }
        ICommand createNewPolygonCommand;

        public void DeletePoint(object arg)
        {
            var point = arg as DragablePoint;
            if (point == null) return;
            if (SelectedPolygon == null) return;

            if (SelectedPolygon.Points.Count <= 3)
            {
                var toDel = SelectedPolygon;
                Polygons.Remove(toDel);
                SelectedPolygon = null;
            }
            else
            {
                SelectedPolygon.DeletePoint(point);
                SelectedPolygon.RenderBitmap();
            }
        }

        public ICommand DeletePointCommand
        {
            get
            {
                return deletePointCommand ?? (deletePointCommand = new CustomCommand((x) => DeletePoint(x)));
            }
        }
        ICommand deletePointCommand;

        public void AddMiddlePoint(object arg)
        {
            var line = arg as Line;
            if (line == null) return;
            if (SelectedPolygon == null) return;
            line.RemoveRelation();
            var index = SelectedPolygon.Lines.IndexOf(line);
            var toAdd = new DragablePoint((line.First.X + line.Second.X) / 2, (line.First.Y + line.Second.Y) / 2);
            SelectedPolygon.AddPoint(toAdd, index);
            SelectedPolygon.RenderBitmap();
        }

        public ICommand AddMiddlePointCommand
        {
            get
            {
                return addMiddlePointCommand ?? (addMiddlePointCommand = new CustomCommand((x) => AddMiddlePoint(x)));
            }
        }
        ICommand addMiddlePointCommand;

        public bool IsAddingRelation { get; set; } = false;
        Relation relation = Relation.NONE;
        Line relatedLine = null;
        public void AddParallelRelation(object arg)
        {
            IsAddingRelation = true;
            var line = arg as Line;
            if (relatedLine != null)
            {
                if (relation != Relation.PARALLEL) return;
                relatedLine.RelatedLine = line;
                line.RelatedLine = relatedLine;
                IsAddingRelation = false;
                relatedLine = null;
                relation = Relation.NONE;
                Debug.WriteLine("Adding old relation. Line:" + line.First.X + " " + line.Second.X);
            }
            else
            {
                if (relation != Relation.NONE) return;
                Debug.WriteLine("Adding new relation. Line:" + line.First.X + " " + line.Second.X);
                IsAddingRelation = true;
                relatedLine = line;
                relation = Relation.PARALLEL;
            }
            line.Relation = Relation.PARALLEL;
            if (SelectedPolygon != null)
                SelectedPolygon.RenderBitmap();
        }

        public void AddRelationEnd(Line lin)
        {
            switch (relation)
            {
                case Relation.NONE:
                    break;
                case Relation.EQUALS:
                    AddEqualRelation(lin);
                    break;
                case Relation.PARALLEL:
                    AddParallelRelation(lin);
                    break;
                default:
                    break;
            }
        }

        public void AddEqualRelation(object arg)
        {
            IsAddingRelation = true;
            var line = arg as Line;
            
            if (relatedLine != null)
            {
                if (relation != Relation.EQUALS) return;
                relatedLine.RelatedLine = line;
                line.RelatedLine = relatedLine;
                IsAddingRelation = false;
                relatedLine = null;
                relation = Relation.NONE;
                Debug.WriteLine("Adding old relation. Line:" + line.First.X + " " + line.Second.X);
            }
            else
            {
                if (relation != Relation.NONE) return;
                Debug.WriteLine("Adding new relation. Line:" + line.First.X + " " + line.Second.X);
                IsAddingRelation = true;
                relatedLine = line;
                relation = Relation.EQUALS;
            }
            line.Relation = Relation.EQUALS;
            if (SelectedPolygon != null)
                SelectedPolygon.RenderBitmap();
        }

        public ICommand AddParallelRelationCommand
        {
            get
            {
                return addParallelRelationCommand ?? (addParallelRelationCommand = new CustomCommand((x) => AddParallelRelation(x)));
            }
        }
        ICommand addParallelRelationCommand;

        public ICommand AddEqualRelationCommand
        {
            get
            {
                return addEqualRelationCommand ?? (addEqualRelationCommand = new CustomCommand((x) => AddEqualRelation(x)));
            }
        }
        ICommand addEqualRelationCommand;

        public int BitmapWidth
        {
            get
            {
                return bitmapWidth;
            }
            set
            {
                if (bitmapWidth == value) return;
                bitmapWidth = value;
                OnPropertyChanged(nameof(BitmapWidth));
            }
        }
        int bitmapWidth = 640;

        public int BitmapHeight
        {
            get
            {
                return bitmapHeight;
            }
            set
            {
                if (bitmapHeight == value) return;
                bitmapHeight = value;
                OnPropertyChanged(nameof(BitmapWidth));
            }
        }
        int bitmapHeight = 480;

        public void AddPolygon(Polygon poly, int zIndex = -1)
        {
            poly.Boundries = new Rect(0, 0, BitmapWidth, BitmapHeight);
            if(zIndex >= 0)
            {
                Polygons.Insert(zIndex, poly);
            }
            else
            {
                Polygons.Add(poly);
            }
            
        }

        public void DeletePolygon(object poly)
        {
            var polygg = poly as Polygon;
            if (polygg == null) return;
            Polygons.Remove(polygg);
            SelectedPolygon = null;

        }

        public ICommand DeletePolygonCommand
        {
            get
            {
                return deletePolygonCommand ?? (deletePolygonCommand = new CustomCommand((x) => DeletePolygon(x)));
            }
        }
        ICommand deletePolygonCommand;

        public void DeleteRelation(object line)
        {
            var line1 = line as Line;
            line1.RemoveRelation();
            if (SelectedPolygon != null)
                SelectedPolygon.RenderBitmap();
        }

        public ICommand DeleteRelationCommand
        {
            get
            {
                return deleteRelationCommand ?? (deleteRelationCommand = new CustomCommand((x) => DeleteRelation(x)));
            }
        }
        ICommand deleteRelationCommand;

        public void CreatePredefinedPolygon(object par)
        {
            var numString = par as string;
            if (numString == null) return;
            int number = int.Parse(numString);
            Random rnd = new Random();
            byte[] colors = new byte[3];
            rnd.NextBytes(colors);

            var poly = new Polygon();
            if (number == 1)
            {
                poly.AddPoint(new DragablePoint(80, 60));
                poly.AddPoint(new DragablePoint(250, 30));
                poly.AddPoint(new DragablePoint(200, 150));
                poly.AddPoint(new DragablePoint(137, 197));
                poly.AddPoint(new DragablePoint(123, 256));
                poly.AddPoint(new DragablePoint(68, 200));
                poly.AddRelation(poly.Lines[0], poly.Lines[2], Relation.PARALLEL);
                poly.AddRelation(poly.Lines[1], poly.Lines[5], Relation.EQUALS);
                poly.FixRelations(poly.Points[0]);
            }
            else if(number == 2)
            {
                poly.AddPoint(new DragablePoint(464, 70));
                poly.AddPoint(new DragablePoint(521, 267));
                poly.AddPoint(new DragablePoint(344, 335));
                poly.AddPoint(new DragablePoint(434, 238));
                poly.AddPoint(new DragablePoint(400, 100));
                poly.AddPoint(new DragablePoint(300, 78));

                poly.AddRelation(poly.Lines[0], poly.Lines[3], Relation.PARALLEL);
                poly.AddRelation(poly.Lines[2], poly.Lines[5], Relation.EQUALS);
                poly.FixRelations(poly.Points[0]);
            }
            else
            {
                return;
            }
            poly.Color = Color.FromRgb(colors[0], colors[1], colors[2]);
            poly.Name = "Poly" + Polygons.Count.ToString();
            poly.Boundries = new Rect(0, 0, BitmapWidth, BitmapHeight);
            poly.RenderBitmap();
            AddPolygon(poly);
        }

        public ICommand CreatePredefinedPolygonCommand
        {
            get
            {
                return createPredefinedPolygonCommand ?? (createPredefinedPolygonCommand = new CustomCommand((x) => CreatePredefinedPolygon(x)));
            }
        }
        ICommand createPredefinedPolygonCommand;

        public void ChangeBlockStatus(object arg)
        {
            var point = arg as DragablePoint;
            if (point == null) return;
            point.IsBlocked = !point.IsBlocked;
            if (SelectedPolygon != null)
                SelectedPolygon.RenderBitmap();
        }

        public ICommand ChangeBlockingStatusCommand
        {
            get
            {
                return changeBlockingStatusCommand ?? (changeBlockingStatusCommand = new CustomCommand((x) => ChangeBlockStatus(x)));
            }
        }
        ICommand changeBlockingStatusCommand;
    }
}

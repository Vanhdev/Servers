using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace System.Windows.Controls
{
    public class DrawingPoint
    {
        public DrawingPoint Next { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public DrawingPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
        public DrawingPoint MoveTo(DrawingPoint point)
        {
            return MoveTo(point.X, point.Y);
        }
        public DrawingPoint MoveTo(double x, double y)
        {
            return Next = new DrawingPoint(x, y);
        }
        public DrawingPoint Offset(double x, double y)
        {
            return Next = new DrawingPoint(X + x, Y + y);
        }
        public PointCollection Points
        {
            get
            {
                var p = this;
                var l = new PointCollection();
                while (p != null)
                {
                    l.Add(new Point(p.X, p.Y));
                    p = p.Next;
                }
                return l;
            }
        }

        public static explicit operator Point(DrawingPoint p)
        {
            return new Point(p.X, p.Y);
        }

    }
    public class DrawingPen : Canvas
    {
        DrawingPoint pen;
        protected virtual void SetValueCore(DependencyProperty dp, object value)
        {
            base.SetValue(dp, value);
            Invalidate();
        }
        public Brush Stroke
        {
            get => (Brush)GetValue(Line.StrokeProperty);
            set => SetValueCore(Line.StrokeProperty, value);
        }
        public Brush Fill
        {
            get => (Brush)GetValue(Rectangle.FillProperty);
            set => SetValueCore(Rectangle.FillProperty, value);
        }
        public double Thickness
        {
            get => (double)GetValue(Line.StrokeThicknessProperty);
            set => SetValueCore(Line.StrokeThicknessProperty, value);
        }
        public PointCollection Points => pen.Points;
        public DrawingPen()
        {
            this.Thickness = 1;
            this.Stroke = Brushes.Black;
            this.VerticalAlignment = VerticalAlignment.Center;

            //this.Width = this.Height = 32;
        }
        public DrawingPoint Start(double x, double y)
        {
            return pen = new DrawingPoint(x, y);
        }
        public DrawingPoint Start(DrawingPoint point)
        {
            return Start(point.X, point.Y);
        }
        public DrawingPen Offset(double x, double y)
        {
            var p = pen;
            while (p != null)
            {
                p.X += x;
                p.Y += y;
                p = p.Next;
            }
            return this;
        }
        public Polyline Polyline()
        {
            return Polyline(this.Stroke, this.Thickness);
        }
        public Polyline Polyline(Brush color, double thickness)
        {
            Children.Add(new Polyline
            {
                Points = pen.Points,
                Stroke = color,
                StrokeThickness = thickness,
            });
            return (Polyline)Children[Children.Count - 1];
        }
        public Polygon Polygon()
        {
            Children.Add(new Polygon
            {
                Points = pen.Points,
                Fill = this.Fill,
            });
            return (Polygon)Children[Children.Count - 1];
        }
        public Polygon Polygon(Brush fill, Brush stroke, double thickness)
        {
            Children.Add(new Shapes.Polygon
            {
                Points = pen.Points,
                Fill = fill,
                Stroke = stroke,
                StrokeThickness = thickness,
            });
            return (Polygon)Children[Children.Count - 1];
        }
        public Polygon Polygon(double opacity)
        {
            var fill = Stroke.Clone();
            fill.Opacity = opacity;

            Children.Add(new Shapes.Polygon
            {
                Points = pen.Points,
                Fill = fill,
                Stroke = Stroke,
                StrokeThickness = Thickness,
            });
            return (Polygon)Children[Children.Count - 1];
        }

        public event Action<int, double> Rotating;
        public event EventHandler AnimationStoped;
        public void BeginRotate(double seconds, double angle, int steps)
        {
            var trans = this.RenderTransform as RotateTransform;
            if (trans == null)
            {
                trans = new RotateTransform();
                this.RenderTransform = trans;
            }
            trans.CenterX = Width / 2;
            trans.CenterY = Height / 2;

            double stepAngle = angle / steps;
            angle += trans.Angle;

            this.BeginTimer(seconds / steps * 1000, () =>
            {
                if (steps-- == 0)
                {
                    trans.Angle = angle;
                    AnimationStoped?.Invoke(this, EventArgs.Empty);
                    return false;
                }

                trans.Angle += stepAngle;
                Rotating?.Invoke(steps, trans.Angle);
                return true;
            });
        }

        public virtual void Invalidate() { }
    }
}

namespace System.Windows.Controls
{

    public static class ImageExtension
    {
        static Dictionary<string, Type> _map;
        static Dictionary<string, Type> Map
        {
            get
            {
                if (_map == null)
                {
                    _map = new Dictionary<string, Type>();
                }
                return _map;
            }
        }
        public static DrawingPen GetImage(this FrameworkElement e, string name)
        {
            Type type;
            if (!Map.TryGetValue(name, out type))
            {
                type = Type.GetType("System.Windows.Controls." + name + "Img");
                if (type == null) { return new DrawingPen(); }

                _map[type.Name] = type;
            }

            return (DrawingPen)Activator.CreateInstance(type);
        }
        public static DrawingPen GetImage(this FrameworkElement e, string name, double width, double height)
        {
            var img = GetImage(e, name);
            img.Width = width;
            img.Height = height;

            return img;
        }
        public static DrawingPen GetImage(this FrameworkElement e, string name, double size)
        {
            return GetImage(e, name, size, size);
        }
    }
    public abstract class MyImage : DrawingPen
    {
        protected abstract void OnPaint(double x0, double y0, double w, double h);
        protected override Size MeasureOverride(Size constraint)
        {
            Children.Clear();
            OnPaint(Width / 2, Height / 2, Width, Height);
            Polyline();
            return base.MeasureOverride(constraint);
        }
        public override void Invalidate()
        {
            base.InvalidateMeasure();
        }
    }
    public class MenuImg : MyImage
    {
        protected override void OnPaint(double x0, double y0, double w, double h)
        {
            double d = 0;
            Start(0, d).Offset(w, 0);
            Polyline();

            Start(0, y0).Offset(w, 0);
            Polyline();

            Start(0, h - d).Offset(w, 0);
        }
    }
    public class FolderImg : DrawingPen
    {
        protected override Size MeasureOverride(Size constraint)
        {
            double w = Height;
            double h = w * 3 / 4;

            double y0 = (Height - h) / 2;
            double x1 = Width - w;
            double d = h / 6;

            Children.Clear();
            Stroke = (Brush)RGB.Parse("#FFE39E");
            Start(0, y0)
                .Offset(0, h)
                .Offset(w, 0)
                .Offset(0, d - h)
                .Offset(-w / 2, 0)
                .Offset(-d, -d);
            Polygon(0.5);

            Start(0, y0 + 2 * d).Offset(w, 0);
            Polyline();

            return base.MeasureOverride(constraint);
        }
    }
    public class FileImg : MyImage
    {
        protected override void OnPaint(double x0, double y0, double w, double h)
        {
            w = h - h / 6;

            var d = w / 3;
            var p0 = new DrawingPoint(w - d, 0);
            var p1 = new DrawingPoint(w, d);

            Start(p0)
                .MoveTo(0, 0)
                .Offset(0, h)
                .Offset(w, 0)
                .MoveTo(p1)
                .MoveTo(p0);
            Polygon(0.3);

            Start(p0)
                .Offset(0, d)
                .Offset(d, 0);
            Polyline();
        }
    }
    public class DownImg : MyImage
    {
        protected override void OnPaint(double x0, double y0, double w, double h)
        {
            y0 -= h / 4;
            Start(0, y0).MoveTo(x0, y0 + h / 2).MoveTo(w, y0);
        }
    }
    public class LeftImg : MyImage
    {
        protected override void OnPaint(double x0, double y0, double w, double h)
        {
            x0 += w / 4;
            Start(x0, 0).MoveTo(x0 - w / 2, y0).MoveTo(x0, h);
        }
    }
    public class RightImg : MyImage
    {
        protected override void OnPaint(double x0, double y0, double w, double h)
        {
            x0 -= w / 4;
            Start(x0, 0).MoveTo(x0 + w / 2, y0).MoveTo(x0, h);
        }
    }
    public class FilterImg : MyImage
    {

        protected override void OnPaint(double x0, double y0, double w, double h)
        {
            var d = w / 8;
            Start(0, 0)
                .MoveTo(x0 - d, y0)
                .Offset(0, y0)
                .Offset(d, 0)
                .Offset(d, -d)
                .Offset(0, -y0 + d)
                .MoveTo(w, 0);
            Polygon(0.5);
        }
    }
}
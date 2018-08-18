using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DarkChronicleClock;

namespace DarkChronicleClockWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Clock clock;

        private DispatcherTimer tmr;

        private ClockControl clockControl;

        public MainWindow()
        {
            InitializeComponent();


            clock = Clock.GetDefaultClock();

            clockControl = new ClockControl(clock, pnl);

            tmr = new DispatcherTimer();
            tmr.Interval = TimeSpan.FromMilliseconds(25);
            tmr.Tick += new EventHandler(timer_Tick);
            tmr.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            float convToAngle = (float)((2 * Math.PI) / 60000f);

            foreach (SubCircle c in clock.SubCircles)
                UpdateSubCircle(convToAngle, c);

            foreach (SubCircle c in clock.OuterSubCircles)
                UpdateSubCircle(convToAngle, c);

            //clock.MinHandle.AnglePosition += 0.04f * 10;
            //clock.HourHandle.AnglePosition += 0.04f;

            clock.MinHandle.AnglePosition = ClockDrawing.ToRad(-90 + ((float)((DateTime.Now - DateTime.Now.Date).TotalMinutes / 60f) * 360));
            clock.HourHandle.AnglePosition = ClockDrawing.ToRad(-90 + ((float)((DateTime.Now - DateTime.Now.Date).TotalHours / 12f) * 360));

            //Rect r = new Rect(0, 0, pnl.ActualWidth > pnl.ActualHeight ? pnl.ActualHeight : pnl.ActualWidth, pnl.ActualWidth > pnl.ActualHeight ? pnl.ActualHeight : pnl.ActualWidth);
            //r.Inflate(-r.Width / 4f, -r.Height / 4f);

            clockControl.Bounds = new Rect(0, 0, pnl.ActualWidth, pnl.ActualHeight) ;
            clockControl.Update();
        }


        private void UpdateSubCircle(float convToAngle, SubCircle c)
        {
            c.AnglePosition += (float)tmr.Interval.TotalMilliseconds * convToAngle;

            if (c.AnglePosition > 2 * Math.PI)
                c.AnglePosition -= (float)(2 * Math.PI);

            foreach (Handle h in c.BigHandles)
            {
                h.AnglePosition -= (float)tmr.Interval.TotalMilliseconds * convToAngle;

                if (h.AnglePosition > 2 * Math.PI)
                    h.AnglePosition -= (float)(2 * Math.PI);
            }

            foreach (Handle h in c.SmallHandles)
            {
                h.AnglePosition += (float)tmr.Interval.TotalMilliseconds * 2 * convToAngle;

                if (h.AnglePosition > 2 * Math.PI)
                    h.AnglePosition -= (float)(2 * Math.PI);
            }
        }


        class ClockControl
        {
            private BorderManager borderManager1;
            private BorderManager borderManager2;

            private MainClockControl mainClock1;
            private MainClockControl mainClock2;

            private Rect bounds;
            public Rect Bounds
            {
                get { return bounds; }
                set { bounds = value; }
            }

            public ClockControl(Clock clock, Canvas pnl)
            {
                borderManager1 = new BorderManager(0) { BorderBrush = new SolidColorBrush(Color.FromArgb(200, Colors.WhiteSmoke.R, Colors.WhiteSmoke.G, Colors.WhiteSmoke.B)) };
                borderManager2 = new BorderManager(0) { BorderBrush = new SolidColorBrush(Color.FromArgb(50, Colors.WhiteSmoke.R, Colors.WhiteSmoke.G, Colors.WhiteSmoke.B)) };


                mainClock1 = new MainClockControl(clock, pnl, bounds, borderManager1);
                mainClock2 = new MainClockControl(clock, pnl, bounds, borderManager2);
            }


            public void Update()
            {
                double w = Math.Min(bounds.Width, bounds.Height);
                Rect frontBounds = new Rect(bounds.Width / 2f - w / 2, bounds.Height / 2f - w / 2, w, w);
                frontBounds.Inflate(-0.25f * frontBounds.Width, -0.25f * frontBounds.Height);
                borderManager1.ScaleSize = frontBounds.Width / 500f;
                



                Rect backgroundBounds = new Rect(0, 0, frontBounds.Width * 4, frontBounds.Height * 4);
                borderManager2.ScaleSize = backgroundBounds.Width / 500f;

                mainClock1.Bounds = frontBounds;
                mainClock1.Update();

                mainClock2.Bounds = backgroundBounds;
                mainClock2.Update();
            }

        }

        class MainClockControl
        {

            private Ellipse main;
            private Ellipse outermain;
            private Ellipse innermain;
            private Ellipse inner;
            private Ellipse innerinner1;
            private Ellipse innerinner2;
            private Ellipse center;

            private Path horizontal;
            private Path vertical;

            private Path hourPath;
            private Line hourLine;
            private Path minPath;
            private Line minLine;


            //private Canvas pnl;

            private Rect bounds;

            public Rect Bounds
            {
                get { return bounds; }
                set { bounds = value; }
            }

            private BorderManager borderManager;

            private List<SubCircleControl> subCircles;
            private List<SubCircleControl> outerSubCircles;

            private Clock clock;

            public MainClockControl(Clock clock, Canvas pnl, Rect bounds, BorderManager borderManager)
            {
                //this.pnl = pnl;
                this.bounds = bounds;
                this.borderManager = borderManager;
                this.clock = clock;


                InitializeMainControls(pnl);
                InitializeSubCircles(pnl);
            }

            private void InitializeSubCircles(Canvas pnl)
            {
                subCircles = new List<SubCircleControl>(clock.SubCircles.Count);
                foreach (var circle in clock.SubCircles)
                    subCircles.Add(new SubCircleControl(circle, this, pnl, Rect.Empty, borderManager));

                outerSubCircles = new List<SubCircleControl>(clock.OuterSubCircles.Count);
                foreach (var circle in clock.OuterSubCircles)
                    outerSubCircles.Add(new SubCircleControl(circle, this, pnl, Rect.Empty, borderManager));
            }

            private void InitializeMainControls(Canvas pnl)
            {
                main = new Ellipse() { Stroke = borderManager.BorderBrush };
                outermain = new Ellipse() { Stroke = borderManager.BorderBrush };
                innermain = new Ellipse() { Stroke = borderManager.BorderBrush };

                inner = new Ellipse() { Stroke = borderManager.BorderBrush };
                innerinner1 = new Ellipse() { Stroke = borderManager.BorderBrush };
                innerinner2 = new Ellipse() { Stroke = borderManager.BorderBrush };

                center = new Ellipse() { Fill = borderManager.BorderBrush };

                horizontal = new Path() { Fill = borderManager.BorderBrush };
                vertical = new Path() { Fill = borderManager.BorderBrush };


                hourPath = new Path() { Fill = borderManager.BorderBrush };
                hourLine = new Line() { Stroke = borderManager.BorderBrush };
                minPath = new Path() { Fill = borderManager.BorderBrush };
                minLine = new Line() { Stroke = borderManager.BorderBrush };

                pnl.Children.Add(main);
                pnl.Children.Add(outermain);
                pnl.Children.Add(innermain);
                pnl.Children.Add(inner);
                pnl.Children.Add(innerinner1);
                pnl.Children.Add(innerinner2);
                pnl.Children.Add(center);
                pnl.Children.Add(horizontal);
                pnl.Children.Add(vertical);
                pnl.Children.Add(hourPath);
                pnl.Children.Add(hourLine);
                pnl.Children.Add(minPath);
                pnl.Children.Add(minLine);
            }

            public void Update()
            {
                double unit = (bounds.Width / 100f) / 2f;
                UpdateMainClock(unit);
                UpdateSubCircles(unit);
            }

            private void UpdateMainClock(double unit)
            {
                Point centerPoint = new Point(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);

                // scale with units 0 - 100

                // main at 94
                Rect mainRect = bounds;
                mainRect.Inflate(-6f * unit, -6f * unit);

                // outermain at 100
                Rect outermainRect = bounds;

                // innermain at 75
                Rect innermainRect = bounds;
                innermainRect.Inflate(-25f * unit, -25f * unit);

                // inner at 40
                Rect innerRect = bounds;
                innerRect.Inflate(-60f * unit, -60f * unit);

                // innerinner1 at 28
                Rect innerinner1Rect = bounds;
                innerinner1Rect.Inflate(-72f * unit, -72f * unit);

                // innerinner2 at 24
                Rect innerinner2Rect = bounds;
                innerinner2Rect.Inflate(-76f * unit, -76f * unit);

                Rect centerRect = bounds;
                centerRect.Inflate(-90f * unit, -90f * unit);

                SetBounds(main, mainRect);
                main.StrokeThickness = borderManager.MediumBorder;

                SetBounds(outermain, outermainRect);
                outermain.StrokeThickness = borderManager.ThinBorder;
                SetBounds(innermain, innermainRect);
                innermain.StrokeThickness = borderManager.ThinBorder;

                SetBounds(inner, innerRect);
                inner.StrokeThickness = borderManager.ThickBorder;
                SetBounds(innerinner1, innerinner1Rect);
                innerinner1.StrokeThickness = borderManager.ThinBorder;
                SetBounds(innerinner2, innerinner2Rect);
                innerinner2.StrokeThickness = borderManager.ThinBorder;

                SetBounds(center, centerRect);


                Point[] horPoints = new Point[] {
                     GetPointAtAngleFromCircle(bounds,outermainRect, ToRad(180+15)),
                     new Point(centerPoint.X, centerPoint.Y - 0.1f * center.Height),
                     GetPointAtAngleFromCircle(bounds,outermainRect, ToRad(360+15)),
                     new Point(centerPoint.X, centerPoint.Y + 0.1f * center.Height),
                 };
                SetPoints(horizontal, horPoints);

                Point[] verPoints = new Point[] {
                    GetPointAtAngleFromCircle(bounds,outermainRect, ToRad(90+15)),
                    new Point(centerPoint.X - 0.1f * center.Width,  centerPoint.Y),
                    GetPointAtAngleFromCircle(bounds,outermainRect, ToRad(270+15)),
                    new Point(centerPoint.X + 0.1f * center.Width, centerPoint.Y),
                };
                SetPoints(vertical, verPoints);


                Rect outerhandlecircleRect = bounds;
                Rect outerhandlecircle2Rect = bounds;
                Rect outerhandlecircle3Rect = bounds;
                outerhandlecircleRect.Inflate(5 * unit, 5 * unit);
                outerhandlecircle2Rect.Inflate(20 * unit, 20 * unit);
                outerhandlecircle3Rect.Inflate(3 * unit, 3 * unit);

                Point[] hourPoints = new Point[] {
                    GetPointAtAngleFromCircle(bounds, outerhandlecircleRect, clock.MinHandle.AnglePosition),
                    GetPointAtAngleFromCircle(bounds, outerhandlecircle3Rect, clock.MinHandle.AnglePosition - ToRad(3)),
                    GetPointAtAngleFromCircle(bounds, outerhandlecircle2Rect, clock.MinHandle.AnglePosition),
                    GetPointAtAngleFromCircle(bounds, outerhandlecircle3Rect, clock.MinHandle.AnglePosition + ToRad(3))                
                };
                SetPoints(hourPath, hourPoints);

                SetLine(hourLine, centerPoint, GetPointAtAngleFromCircle(bounds, outerhandlecircleRect, clock.MinHandle.AnglePosition));
                hourLine.StrokeThickness = borderManager.LessThinBorder;

                outerhandlecircleRect = innermainRect;
                outerhandlecircle2Rect = innermainRect;
                outerhandlecircle3Rect = innermainRect;
                outerhandlecircleRect.Inflate(5 * unit, 5 * unit);
                outerhandlecircle2Rect.Inflate(15 * unit, 15 * unit);
                outerhandlecircle3Rect.Inflate(3 * unit, 3 * unit);

                Point[] minPoints = new Point[] {
                   GetPointAtAngleFromCircle(bounds, outerhandlecircleRect, clock.HourHandle.AnglePosition),
                    GetPointAtAngleFromCircle(bounds, outerhandlecircle3Rect, clock.HourHandle.AnglePosition - ToRad(3)),
                    GetPointAtAngleFromCircle(bounds, outerhandlecircle2Rect, clock.HourHandle.AnglePosition),
                    GetPointAtAngleFromCircle(bounds, outerhandlecircle3Rect, clock.HourHandle.AnglePosition + ToRad(3))

                };
                SetPoints(minPath, minPoints);

                SetLine(minLine, centerPoint, GetPointAtAngleFromCircle(bounds, outerhandlecircleRect, clock.HourHandle.AnglePosition));
                minLine.StrokeThickness = borderManager.LessThinBorder;
            }


            private void UpdateSubCircles(double unit)
            {
                Rect subcirclebounds = bounds;
                subcirclebounds.Inflate(-75f * unit, -75f * unit);

                Rect outerSubcirclebounds = bounds;
                outerSubcirclebounds.Inflate(-75f * unit, -75f * unit);

                if (subcirclebounds != Rect.Empty)
                {
                    for (int i = 0; i < subCircles.Count; i++)
                    {
                        SubCircleControl sc = subCircles[i];
                        SubCircle c = clock.SubCircles[i];

                        Rect r = subcirclebounds;
                        r.Offset(-r.X - r.Width / 2f, -r.Y - r.Height / 2f);

                        Rect outercircle = bounds;
                        outercircle.Inflate(2f * unit, 2f * unit);

                        Point offset = GetPointAtAngleFromCircle(bounds, outercircle, c.AnglePosition);
                        r.Offset(offset.X, offset.Y);

                        sc.Bounds = r;
                        sc.Update();
                    }
                }

                if (outerSubcirclebounds != Rect.Empty)
                {
                    for (int i = 0; i < outerSubCircles.Count; i++)
                    {
                        SubCircleControl sc = outerSubCircles[i];
                        SubCircle c = clock.OuterSubCircles[i];

                        Rect r = outerSubcirclebounds;
                        r.Offset(-r.X - r.Width / 2f, -r.Y - r.Height / 2f);

                        Rect outercircle = bounds;

                        outercircle.Inflate(subcirclebounds.Width / 2, subcirclebounds.Height / 2);


                        Point offset = GetPointAtAngleFromCircle(bounds, outercircle, c.AnglePosition);
                        r.Offset(offset.X, offset.Y);

                        sc.Bounds = r;
                        sc.Update();
                    }
                }
            }
        }

        class BorderManager
        {


            private double scalesize;

            public double ScaleSize
            {
                get { return scalesize; }
                set { scalesize = value; }
            }
            public BorderManager(double scalesize)
            {
                this.scalesize = scalesize;
                BorderBrush = Brushes.Black;
            }

            public double ThickBorder { get { return 16 * scalesize; } }
            public double MediumBorder { get { return 8 * scalesize; } }
            public double ThinBorder { get { return 1 * scalesize; } }
            public double LessThinBorder { get { return 2 * scalesize; } }

            public Brush BorderBrush { get; set; }
        }

        class SubCircleControl
        {

            private Ellipse outer;
            private Ellipse inner;
            private Ellipse center;

            private Rect bounds;

            public Rect Bounds
            {
                get { return bounds; }
                set { bounds = value; }
            }

            //private Canvas pnl;
            private BorderManager borderManager;

            private SubCircle circle;

            private List<HandleControl> smallHandles;
            private List<HandleControl> bigHandles;

            private Line lineToCenterParent;
            private MainClockControl parent;

            public SubCircleControl(SubCircle circle, MainClockControl parent, Canvas pnl, Rect bounds, BorderManager borderManager)
            {
                //this.pnl = pnl;
                this.bounds = bounds;
                this.borderManager = borderManager;
                this.circle = circle;
                this.parent = parent;

                InitializeMainControls(pnl);

                InitializeHandles(pnl);
            }

            private void InitializeHandles(Canvas pnl)
            {
                bigHandles = new List<HandleControl>(circle.BigHandles.Count);
                foreach (var handle in circle.BigHandles)
                    bigHandles.Add(new HandleControl(handle, this, pnl, Rect.Empty, borderManager));

                smallHandles = new List<HandleControl>(circle.SmallHandles.Count);
                foreach (var handle in circle.SmallHandles)
                    smallHandles.Add(new HandleControl(handle, this, pnl, Rect.Empty, borderManager));
            }

            private void InitializeMainControls(Canvas pnl)
            {
                outer = new Ellipse() { Stroke = borderManager.BorderBrush };
                inner = new Ellipse() { Stroke = borderManager.BorderBrush };
                center = new Ellipse() { Fill = borderManager.BorderBrush };
                lineToCenterParent = new Line() { Stroke = borderManager.BorderBrush };

                pnl.Children.Add(outer);
                pnl.Children.Add(inner);
                pnl.Children.Add(center);
                pnl.Children.Add(lineToCenterParent);
            }

            public void Update()
            {
                double unit = (bounds.Width / 100f) / 2f;
                UpdateMainCircle(unit);
                UpdateHandles(unit);
            }

            public void UpdateMainCircle(double unit)
            {
                Rect outerRect = bounds;

                Rect innerRect = bounds;
                innerRect.Inflate(-10f * unit, -10f * unit);

                Rect centerRect = bounds;
                centerRect.Inflate(-90f * unit, -90f * unit);


                SetBounds(outer, outerRect);
                outer.StrokeThickness = borderManager.LessThinBorder;

                SetBounds(inner, innerRect);
                outer.StrokeThickness = borderManager.ThinBorder;

                SetBounds(center, centerRect);

                lineToCenterParent.X1 = centerRect.X + centerRect.Width / 2;
                lineToCenterParent.Y1 = centerRect.Y + centerRect.Height / 2;
                lineToCenterParent.X2 = parent.Bounds.X + parent.Bounds.Width / 2f;
                lineToCenterParent.Y2 = parent.Bounds.Y + parent.Bounds.Height / 2f;
                lineToCenterParent.StrokeThickness = borderManager.ThinBorder;
            }

            public void UpdateHandles(double unit)
            {

                Rect bigHandleBounds = bounds;
                bigHandleBounds.Inflate(-75 * unit, -75 * unit);

                Rect smallHandleBounds = bounds;
                smallHandleBounds.Inflate(-85 * unit, -85 * unit);

                if (bigHandleBounds != Rect.Empty)
                {
                    for (int i = 0; i < circle.BigHandles.Count; i++)
                    {
                        HandleControl hc = bigHandles[i];
                        Handle h = circle.BigHandles[i];

                        Rect r = bigHandleBounds;
                        r.Offset(-r.X - r.Width / 2f, -r.Y - r.Height / 2f);

                        Point offset = GetPointAtAngleFromCircle(bigHandleBounds, bounds, h.AnglePosition);
                        r.Offset(offset.X, offset.Y);

                        hc.Bounds = r;
                        hc.Update();
                    }
                }

                if (smallHandleBounds != Rect.Empty)
                {
                    for (int i = 0; i < circle.SmallHandles.Count; i++)
                    {
                        HandleControl hc = smallHandles[i];
                        Handle h = circle.SmallHandles[i];

                        Rect r = smallHandleBounds;
                        r.Offset(-r.X - r.Width / 2f, -r.Y - r.Height / 2f);

                        Point offset = GetPointAtAngleFromCircle(smallHandleBounds, bounds, h.AnglePosition);
                        r.Offset(offset.X, offset.Y);

                        hc.Bounds = r;
                        hc.Update();
                    }
                }
            }
        }

        class HandleControl
        {
            private Ellipse center;
            private Line lineToCenterParent;

            private Rect bounds;
            public Rect Bounds
            {
                get { return bounds; }
                set { bounds = value; }
            }

            private BorderManager borderManager;
            private Handle handle;
            private SubCircleControl parent;

            public HandleControl(Handle handle, SubCircleControl parent, Canvas pnl, Rect bounds, BorderManager borderManager)
            {
                this.handle = handle;
                this.bounds = bounds;
                this.borderManager = borderManager;
                this.parent = parent;


                InitializeMainControls(pnl);
            }

            private void InitializeMainControls(Canvas pnl)
            {
                center = new Ellipse() { Fill = borderManager.BorderBrush };
                lineToCenterParent = new Line() { Stroke = borderManager.BorderBrush };

                pnl.Children.Add(center);
                pnl.Children.Add(lineToCenterParent);
            }

            public void Update()
            {
                Rect centerRect = bounds;
                SetBounds(center, centerRect);

                lineToCenterParent.X1 = centerRect.X + centerRect.Width / 2;
                lineToCenterParent.Y1 = centerRect.Y + centerRect.Height / 2;
                lineToCenterParent.X2 = parent.Bounds.X + parent.Bounds.Width / 2f;
                lineToCenterParent.Y2 = parent.Bounds.Y + parent.Bounds.Height / 2f;
                lineToCenterParent.StrokeThickness = borderManager.ThinBorder;
            }
        }

  

        private static void SetBounds(Ellipse el, Rect r)
        {
            if (r == Rect.Empty)
                el.Visibility = Visibility.Collapsed;
            else
            {
                el.Visibility = Visibility.Visible;
                //if (!double.IsInfinity(r.Left))
                    el.SetValue(Canvas.LeftProperty, r.Left);
                //if (!double.IsInfinity(r.Top))
                    el.SetValue(Canvas.TopProperty, r.Top);
                //if (!double.IsInfinity(r.Width) && !double.IsInfinity(r.Height))
                //{
                    el.Width = r.Width;
                    el.Height = r.Height;
                //}
            }
        }


        private static void SetPoints(Path p, Point[] points)
        {
            PathGeometry pg = new PathGeometry();

            PathFigure pf = new PathFigure();
            pf.StartPoint = points[0];
            for (int i = 0; i < points.Length - 1; i++)
                pf.Segments.Add(new QuadraticBezierSegment(points[i], points[i + 1], true));

            pf.IsClosed = true;

            pg.Figures.Add(pf);
            p.Data = pg;
        }

        private static void SetLine(Line l, Point p1, Point p2)
        {
            l.X1 = p1.X;
            l.X2 = p2.X;
            l.Y1 = p1.Y;
            l.Y2 = p2.Y;
        }


        private static Point GetPointAtAngleFromCircle(Rect bounds, Rect circle, double angle)
        {
            double radius = circle.Width / 2f;

            double x = bounds.X + bounds.Width / 2f + radius * Math.Cos(angle);
            double y = bounds.Y + bounds.Height / 2f + radius * Math.Sin(angle);
            return new Point(x, y);
        }

        public static double ToRad(float degrees)
        {
            return (double)(degrees * ((2 * Math.PI) / 360f));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

    }
}

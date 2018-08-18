using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DarkChronicleClock
{
    public static class ClockDrawing
    {

        private class ClockTools : IDisposable 
        {

            public Pen ThickBorder { get; set; }
            public Pen MediumBorder { get; set; }
            public Pen LessthinBorder { get; set; }
            public Pen ThinBorder { get; set; }
            public Brush Fill { get; set; }




            public void Dispose()
            {
                ThickBorder.Dispose();
                MediumBorder.Dispose();
                LessthinBorder.Dispose();
                ThinBorder.Dispose();
                Fill.Dispose();
            }
        }

        public static void DrawClock(this Graphics g, Clock clock, RectangleF bounds, Color color)
        {
            float unit = (bounds.Width / 100f) / 2f;

            float scaleSize = bounds.Width / 500f;

            ClockTools tools = new ClockTools()
            {
                ThickBorder = new Pen(color, (16f * scaleSize)),
                MediumBorder = new Pen(color, 8f * scaleSize),

                LessthinBorder = new Pen(color, 2f * scaleSize),
                ThinBorder = new Pen(color, 1f * scaleSize),
                Fill = new SolidBrush(color)
            };

            DrawMain(g, clock, bounds, tools);

            tools.Dispose();            
        }

        private static void DrawHandle(Graphics g, RectangleF circle, ClockTools tools)
        {
            g.FillEllipse(tools.Fill, circle);
        }

        private static void DrawSubCircle(Graphics g, SubCircle circle, RectangleF bounds, ClockTools tools)
        {
            RectangleF outer = bounds;

            float unit = (bounds.Width / 100f) / 2f;

            RectangleF inner = bounds;
            inner.Inflate(-10f * unit, -10f * unit);

            RectangleF center = bounds;
            center.Inflate(-90f * unit, -90f * unit);

            g.DrawEllipse(tools.LessthinBorder, outer);
            g.DrawEllipse(tools.ThinBorder, inner);

            g.FillEllipse(tools.Fill, center);

            RectangleF bigHandleBounds = bounds;
            bigHandleBounds.Inflate(-75 * unit, -75 * unit);

            RectangleF smallHandleBounds = bounds;
            smallHandleBounds.Inflate(-85 * unit, -85 * unit);

            foreach (Handle h in circle.BigHandles)
            {
                RectangleF r = bigHandleBounds;
                r.Offset(-r.X - r.Width / 2f, -r.Y - r.Height / 2f);
                r.Offset(GetPointAtAngleFromCircle(bigHandleBounds, bounds, h.AnglePosition));

                g.DrawLine(tools.ThinBorder, new PointF(center.X + center.Width / 2, center.Y + center.Height / 2),
                                         new PointF(r.X + r.Width / 2f, r.Y + r.Height / 2f));

                DrawHandle(g, r, tools);
            }


            foreach (Handle h in circle.SmallHandles)
            {
                RectangleF r = smallHandleBounds;
                r.Offset(-r.X - r.Width / 2f, -r.Y - r.Height / 2f);

                RectangleF outercircle = bounds;
                outercircle.Inflate(50f * unit, 50f * unit);

                r.Offset(GetPointAtAngleFromCircle(smallHandleBounds, outercircle, h.AnglePosition));

                g.DrawLine(tools.ThinBorder, new PointF(center.X + center.Width / 2, center.Y + center.Height / 2),
                                         new PointF(r.X + r.Width / 2f, r.Y + r.Height / 2f));

                DrawHandle(g, r, tools);
            }
        }

        private static void DrawMain(Graphics g, Clock clock, RectangleF bounds, ClockTools tools)
        {
            float unit = (bounds.Width / 100f) / 2f;

            PointF centerPoint = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);

            // scale with units 0 - 100

            // main at 94
            RectangleF main = bounds;
            main.Inflate(-6f * unit, -6f * unit);

            // outermain at 100
            RectangleF outermain = bounds;

            // innermain at 75
            RectangleF innermain = bounds;
            innermain.Inflate(-25f * unit, -25f * unit);

            // inner at 40
            RectangleF inner = bounds;
            inner.Inflate(-60f * unit, -60f * unit);

            // innerinner1 at 28
            RectangleF innerinner1 = bounds;
            innerinner1.Inflate(-72f * unit, -72f * unit);

            // innerinner2 at 24
            RectangleF innerinner2 = bounds;
            innerinner2.Inflate(-76f * unit, -76f * unit);

            RectangleF center = bounds;
            center.Inflate(-90f * unit, -90f * unit);




            g.DrawEllipse(tools.MediumBorder, main);
            g.DrawEllipse(tools.ThinBorder, outermain);
            g.DrawEllipse(tools.ThinBorder, innermain);

            g.DrawEllipse(tools.ThickBorder, inner);
            g.DrawEllipse(tools.ThinBorder, innerinner1);
            g.DrawEllipse(tools.ThinBorder, innerinner2);

            g.FillEllipse(tools.Fill, center);

            g.FillClosedCurve(tools.Fill, new PointF[] {
                     GetPointAtAngleFromCircle(bounds,outermain, ToRad(180+15)),
                     //GetPointAtAngleFromCircle(bounds,inner, ToRad(180)),
                     new PointF(centerPoint.X, centerPoint.Y - 0.1f * center.Height),
                     GetPointAtAngleFromCircle(bounds,outermain, ToRad(360+15)),
                     //GetPointAtAngleFromCircle(bounds,inner, ToRad(270)),
                     new PointF(centerPoint.X, centerPoint.Y + 0.1f * center.Height),
                 }, System.Drawing.Drawing2D.FillMode.Winding, 0
            );

            g.FillClosedCurve(tools.Fill, new PointF[] {
                     GetPointAtAngleFromCircle(bounds,outermain, ToRad(90+15)),
                     new PointF(centerPoint.X - 0.1f * center.Width,  centerPoint.Y),
                     GetPointAtAngleFromCircle(bounds,outermain, ToRad(270+15)),
                     new PointF(centerPoint.X + 0.1f * center.Width, centerPoint.Y),
                 }, System.Drawing.Drawing2D.FillMode.Winding, 0
          );


            RectangleF subcirclebounds = bounds;
            subcirclebounds.Inflate(-75f * unit, -75f * unit);

            RectangleF outerSubcirclebounds = bounds;
            outerSubcirclebounds.Inflate(-75f * unit, -75f * unit);


            foreach (SubCircle c in clock.SubCircles)
            {
                RectangleF r = subcirclebounds;
                r.Offset(-r.X - r.Width / 2f, -r.Y - r.Height / 2f);

                RectangleF outercircle = bounds;
                outercircle.Inflate(2f * unit, 2f * unit);

                r.Offset(GetPointAtAngleFromCircle(bounds, outercircle, c.AnglePosition));
                DrawSubCircle(g, c, r, tools);


                g.DrawLine(tools.ThinBorder, centerPoint,
                                         new PointF(r.X + r.Width / 2f, r.Y + r.Height / 2f));

            }

            foreach (SubCircle c in clock.OuterSubCircles)
            {
                RectangleF r = subcirclebounds;
                r.Offset(-r.X - r.Width / 2f, -r.Y - r.Height / 2f);

                RectangleF outercircle = bounds;
                outercircle.Inflate(subcirclebounds.Width / 2, subcirclebounds.Height / 2);

                r.Offset(GetPointAtAngleFromCircle(bounds, outercircle, c.AnglePosition));
                DrawSubCircle(g, c, r, tools);


                g.DrawLine(tools.ThinBorder, centerPoint,
                                         new PointF(r.X + r.Width / 2f, r.Y + r.Height / 2f));

            }

            RectangleF outerhandlecircle = bounds;
            RectangleF outerhandlecircle2 = bounds;
            RectangleF outerhandlecircle3 = bounds;
            outerhandlecircle.Inflate(5 * unit, 5 * unit);
            outerhandlecircle2.Inflate(20 * unit, 20 * unit);
            outerhandlecircle3.Inflate(3 * unit, 3 * unit);
            g.DrawLine(tools.LessthinBorder, centerPoint, GetPointAtAngleFromCircle(bounds, outerhandlecircle, clock.MinHandle.AnglePosition));
            g.FillClosedCurve(tools.Fill, new PointF[] {
                GetPointAtAngleFromCircle(bounds, outerhandlecircle, clock.MinHandle.AnglePosition),
                GetPointAtAngleFromCircle(bounds, outerhandlecircle3, clock.MinHandle.AnglePosition - ToRad(3)),
                GetPointAtAngleFromCircle(bounds, outerhandlecircle2, clock.MinHandle.AnglePosition),
                GetPointAtAngleFromCircle(bounds, outerhandlecircle3, clock.MinHandle.AnglePosition + ToRad(3))

            }, System.Drawing.Drawing2D.FillMode.Winding, -0.2f);

            outerhandlecircle = innermain;
            outerhandlecircle2 = innermain;
            outerhandlecircle3 = innermain;
            outerhandlecircle.Inflate(5 * unit, 5 * unit);
            outerhandlecircle2.Inflate(15 * unit, 15 * unit);
            outerhandlecircle3.Inflate(3 * unit, 3 * unit);
            g.DrawLine(tools.LessthinBorder, centerPoint, GetPointAtAngleFromCircle(bounds, outerhandlecircle, clock.HourHandle.AnglePosition));
            g.FillClosedCurve(tools.Fill, new PointF[] {
                GetPointAtAngleFromCircle(bounds, outerhandlecircle, clock.HourHandle.AnglePosition),
                GetPointAtAngleFromCircle(bounds, outerhandlecircle3, clock.HourHandle.AnglePosition - ToRad(3)),
                GetPointAtAngleFromCircle(bounds, outerhandlecircle2, clock.HourHandle.AnglePosition),
                GetPointAtAngleFromCircle(bounds, outerhandlecircle3, clock.HourHandle.AnglePosition + ToRad(3))

            }, System.Drawing.Drawing2D.FillMode.Winding, -0.2f);


        }

        private static PointF GetPointAtAngleFromCircle(RectangleF bounds, RectangleF circle, float angle)
        {
            float radius = circle.Width / 2f;

            float x = bounds.X + bounds.Width / 2f + radius * (float)Math.Cos(angle);
            float y = bounds.Y + bounds.Height / 2f + radius * (float)Math.Sin(angle);
            return new PointF(x, y);
        }

        public static float ToRad(float degrees)
        {
            return (float)(degrees * ((2 * Math.PI) / 360f));
        }

    }
}

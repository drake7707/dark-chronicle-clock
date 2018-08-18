using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DarkChronicleClock
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            DoubleBuffered = true;

            clock = Clock.GetDefaultClock();
        }

        private Clock clock;


        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            this.Close();
        }

        //private Bitmap bmp;

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //if (bmp == null || (bmp != null && (bmp.Width != this.Width || bmp.Height != this.Height)))
            //    bmp = new Bitmap(this.Width, this.Height);

            //using (Graphics g = Graphics.FromImage(bmp))
            //{
                //using (SolidBrush darkenlayer = new SolidBrush(Color.FromArgb(10, Color.Black)))
                //    g.FillRectangle(darkenlayer, new Rectangle(0, 0, bmp.Width, bmp.Height));

              
                Graphics g = e.Graphics;

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


                float w = Math.Min(Width - 1, Height - 1);
                RectangleF bounds = new RectangleF(Width / 2f - w / 2, Height / 2f - w / 2, w, w);
                bounds.Inflate(-0.25f * bounds.Width, -0.25f * bounds.Height);

                RectangleF backgroundBounds = bounds;

                //backgroundBounds.Offset(backgroundBounds.Width / 2, backgroundBounds.Height / 2);
                //g.DrawClock(clock, backgroundBounds, Color.FromArgb(32, Color.Black));

                backgroundBounds = new RectangleF(0, 0, bounds.Width * 4, bounds.Height * 4);

                //backgroundBounds.Offset(-backgroundBounds.Width / 2, -backgroundBounds.Height / 2);
                g.DrawClock(clock, backgroundBounds, Color.FromArgb(32, Color.WhiteSmoke));

                //backgroundBounds = bounds;
                //backgroundBounds.Offset(backgroundBounds.Width / 2, -backgroundBounds.Height / 2);
                //g.DrawClock(clock, backgroundBounds, Color.FromArgb(32, Color.Black));

                //backgroundBounds = bounds;
                //backgroundBounds.Offset(-backgroundBounds.Width / 2, backgroundBounds.Height / 2);
                //g.DrawClock(clock, backgroundBounds, Color.FromArgb(32, Color.Black));

                g.DrawClock(clock, bounds, Color.FromArgb(200, Color.WhiteSmoke));
            //}

            //e.Graphics.DrawImage(bmp, new Point(0,0));
        }

        private void picClock_MouseMove(object sender, MouseEventArgs e)
        {
            this.Text = e.X + " - " + e.Y;

        }

        private void tmr_Tick(object sender, EventArgs e)
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

            Invalidate();
        }

        private void UpdateSubCircle(float convToAngle, SubCircle c)
        {
            c.AnglePosition += tmr.Interval * convToAngle;

            if (c.AnglePosition > 2 * Math.PI)
                c.AnglePosition -= (float)(2 * Math.PI);

            foreach (Handle h in c.BigHandles)
            {
                h.AnglePosition -= tmr.Interval * convToAngle;

                if (h.AnglePosition > 2 * Math.PI)
                    h.AnglePosition -= (float)(2 * Math.PI);
            }

            foreach (Handle h in c.SmallHandles)
            {
                h.AnglePosition += tmr.Interval * 2 * convToAngle;

                if (h.AnglePosition > 2 * Math.PI)
                    h.AnglePosition -= (float)(2 * Math.PI);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            this.Close();
        }
    }
}

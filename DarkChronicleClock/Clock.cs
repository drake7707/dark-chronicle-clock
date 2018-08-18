using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkChronicleClock
{
    public class Clock
    {
        public Clock()
        {
            SubCircles = new List<SubCircle>();
        }

        public List<SubCircle> SubCircles { get; set; }

        public List<SubCircle> OuterSubCircles { get; set; }

        public MainHandle HourHandle { get; set; }

        public MainHandle MinHandle { get; set; }



        public static Clock GetDefaultClock()
        {

            Random rnd = new Random();

            Clock clock = new Clock();
            clock.SubCircles = new List<SubCircle>()  { 
                                    new SubCircle() { AnglePosition = ClockDrawing.ToRad(-90 + 120),
                                                      BigHandles = new List<Handle>() {
                                                            new Handle() { AnglePosition = ClockDrawing.ToRad(rnd.Next(360)) }
                                                      },
                                                      SmallHandles = new List<Handle>() {
                                                            new Handle() { AnglePosition = ClockDrawing.ToRad(rnd.Next(360)) }
                                                      },
                                    },
                                    new SubCircle() { AnglePosition = ClockDrawing.ToRad(-90 + 210),
                                                      BigHandles = new List<Handle>() {
                                                            new Handle() { AnglePosition = ClockDrawing.ToRad(rnd.Next(360)) }
                                                      },
                                                      SmallHandles = new List<Handle>() {
                                                            new Handle() { AnglePosition = ClockDrawing.ToRad(rnd.Next(360)) }
                                                      },
                                    }
                };

            clock.OuterSubCircles = new List<SubCircle>()  { 
                                    new SubCircle() { AnglePosition = ClockDrawing.ToRad(-90 + 330),
                                                      BigHandles = new List<Handle>() {
                                                            new Handle() { AnglePosition = ClockDrawing.ToRad(rnd.Next(360)) }
                                                      },
                                                      SmallHandles = new List<Handle>() {
                                                            new Handle() { AnglePosition = ClockDrawing.ToRad(rnd.Next(360)) }
                                                      },
                                    }

                };

            clock.HourHandle = new MainHandle();
            clock.MinHandle = new MainHandle();

            return clock;
        }
    }
    public class SubCircle
    {
        public float AnglePosition { get; set; }

        public List<Handle> BigHandles { get; set; }

        public List<Handle> SmallHandles { get; set; }
    }

    public class Handle
    {
        public float AnglePosition { get; set; }
    }

    public class MainHandle
    {
        public float AnglePosition { get; set; }
    }
}

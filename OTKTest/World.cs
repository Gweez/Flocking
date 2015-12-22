using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using log4net;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using NewFlocking.Things;
using NewFlocking.Things.LivingThings;
using NewFlocking.Util;

namespace NewFlocking
{
    /***
     * Represents the 3D world. Contains Things.
     * */
    class World
    {
        private ILog log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

        private List<Thing> things;
        private float worldSize;

        private Int64 ticks;
        private DateTime startTime;
        int seconds;

        private Vector3 GRAVITY = new Vector3(0, -9.8f, 0);

        public World()
        {
            log.Debug("Hello World!");

            worldSize = 120;
            ticks = 0;
            startTime = System.DateTime.Now;

            things = new List<Thing>();

            for (int i = 0; i <= 100; i++)
            {
                if (i % 2 == 0)
                {
                    things.Add(new Thing(this, new Vector3(i, 0, 0), Color.Red));
                    things.Add(new Thing(this, new Vector3(0, i, 0), Color.Green));
                    things.Add(new Thing(this, new Vector3(0, 0, i), Color.Blue));
                }
            }

            for (int i = 0; i <= 45; i++)
            {
                things.Add(new Herber(this));
            }
            
            for (int i = 0; i <= 50; i++)
            {
                things.Add(new Boid(this));
            }

            things.Add(new Predator(this));
            things.Add(new Predator(this));
            //things.Add(new Predator(this));

            for (int i = 0; i <= 50; i++)
            {
               things.Add(new FlyingInsect(this));
            }
        }

        public Vector3 Gravity {
            get { return GRAVITY; }
        }

        public void tick(double fps)
        {
            string fn = "World.tick(): ";

            ticks++;

            TimeSpan time;

            foreach (Thing thing in things)
            {
                if (thing.id == 154)
                {
                    time = DateTime.Now.Subtract(startTime);

                    if (time.TotalSeconds >= seconds)
                    {
                        System.Console.WriteLine(
                            fn + "time: " + time.TotalSeconds
                            + " loc: " + thing.location 
                            + " velocity: " + thing.velocity.LengthFast
                        );

                        seconds++;
                    }


                }

                thing.tick(fps);
            }
        }

        public void add(Thing thing)
        {
            things.Add(thing);
        }

        public void drawMe()
        {

            // Generate sky plane
            GL.Disable(EnableCap.Lighting);
            GL.Color3(Color.Aqua);
            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, -1.0, 0.0);
            GL.Vertex3(-7000, 102.0, -7000);
            GL.Vertex3(7000, 102.0, -7000);
            GL.Vertex3(7000, 102.0, 7000);
            GL.Vertex3(-7000, 102.0, 7000);
            GL.End();

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);

            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.CullFace);

            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(200.0f, 200.0f, 200.0f, 1.0f));
            GL.Light(LightName.Light0, LightParameter.Ambient, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));

            // Generate ground plane
            GL.Color3(Color.ForestGreen);
            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, 1.0, 0.0);
            GL.Vertex3(-7000, 0.0, -7000);
            GL.Vertex3(-7000, 0.0, 7000);
            GL.Vertex3(7000, 0.0, 7000);
            GL.Vertex3(7000, 0.0, -7000);
            GL.End();

            Vector3 boidHeight = new Vector3(0, 40, 0);
            Vector3 boidLowHeight = new Vector3(0, 6, 0);
            for( double i = 20; i <= worldSize; i += 20) {
                DrawUtils.drawCircle(i, Color.Black);

                /*GL.PushMatrix();
                GL.Translate(boidHeight);
                DrawUtils.drawCircle(i, Color.Red);
                GL.PopMatrix();*/
            }
            DrawUtils.drawCircle(worldSize, Color.Black);

            foreach (Thing thing in things)
            {
                thing.drawMe();
            }            
        }

        public bool isPaused()
        {
            return false;
        }

        public List<Thing> getAllTheThings()
        {
            return things;
        }

        public float getWorldSize()
        {
            return worldSize;
        }

        public float getHeightAt(Vector3 location)
        {
            return 0;
        }
    }
}

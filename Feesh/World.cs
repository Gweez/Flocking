using System;
using System.Collections.Generic;
using System.Drawing;

using log4net;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using Feesh.Things;
using Feesh.Things.Camera;
using Feesh.Things.LivingThings;
using Feesh.Util;

namespace Feesh
{
    /***
     * Represents the 3D world. Contains Things.
     * */
    class World
    {
        private ILog log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

        private List<Thing> things;
        private Camera _camera;
        private float worldSize;

        private Int64 ticks;
        private DateTime startTime;
        int seconds;

        private Vector3 GRAVITY = new Vector3(0, -9.8f, 0);

        public World()
        {
            log.Debug("Hello World!");

            worldSize = 75;
            ticks = 0;
            startTime = System.DateTime.Now;

            things = new List<Thing>();

            things.Add(new Pillar(this, new Vector3(30, 0, -20)));
            things.Add(new Pillar(this, new Vector3(-40, 0, 50)));
            things.Add(new Pillar(this, new Vector3(-55, 0, -25)));

            for (int i = 0; i <= 100; i++)
            {
                if (i % 5 == 0)
                {
                    things.Add(new Thing(this, new Vector3(0, i, 0), Color.Green));
                }
            }

            for (int i = 0; i <= 175; i++)
            {
                things.Add(new Things.LivingThings.Feesh(this));
            }

            for (int i = 0; i <= 100; i++)
            {
                things.Add(new Sardine(this));
            }

            things.Add(new Shark(this));
        }

        public Vector3 Gravity {
            get { return GRAVITY; }
        }

        public Camera Camera
        {
            get { return _camera; }
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

        public void addCamera(Camera camera)
        {
            _camera = camera;
            things.Add(camera);
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
            GL.Light(LightName.Light0, LightParameter.Ambient, new Vector4(0.5f, 0.5f, 0.5f, 1.0f) );

            // Generate ground plane
            GL.Color3(Color.SandyBrown);
            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, 1.0, 0.0);
            GL.Vertex3(-7000, 0.0, -7000);
            GL.Vertex3(-7000, 0.0, 7000);
            GL.Vertex3(7000, 0.0, 7000);
            GL.Vertex3(7000, 0.0, -7000);
            GL.End();

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

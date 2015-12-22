using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using NewFlocking.Util;

namespace NewFlocking.Things.LivingThings
{
    class FlyingInsect : Boid
    {
        private static int firstFlyingInsectId = -1;

        public FlyingInsect(World aWorld)
            : base(aWorld)
        {
            randomizeLocation(true);

            // restrain to 15 meter area
            _location.X = location.X % 30;
            _location.Y = location.Y % 30;
            _location.Z = location.Z % 12;
        }

        protected override void initialize()
        {
            base.initialize();

            if (firstFlyingInsectId == -1)
            {
                firstFlyingInsectId = id;
            }

            maxHeight = 12f;
            maxAccel = 40f;
            maxSpeed = 6.5f;
            minFlightSpeed = 0.5f;
            minSpeed = 0.5f;

            flockingDistance = .3f;

            sight = 2f;

            flies = true;

            _avoidable = false;

            velocity = new Vector3(velocity.X % maxSpeed, 0, velocity.Y % maxSpeed);
        }

           
        protected override Vector3 calcAccel()
        {
            Vector3 accel;

            float collisionMultiplier = 4f;
            float borderMultiplier = 2f;
            float alignmentMultiplier = 0.2f;
            float cohesionMultiplier = 3.5f;
            float wanderMultiplier = 5f;
            float chillMultiplier = 2f;

            // avoid collisions with flockmates
            accel = collisionAvoidance() * collisionMultiplier;

            // avoid going off the map
            accel += (avoidBorder(30) * borderMultiplier);

            // align with flockmates
            accel += (alignmentMatching() * alignmentMultiplier);

            // try to stay in the group
            accel += (cohesion() * cohesionMultiplier);

            // wander a bit
            accel += (wander() * wanderMultiplier);

            // chill out!
            accel += (chill() * chillMultiplier);

            if (location.Y > maxHeight && accel.Y > 0)
            {
                accel.Y = 0;
            }

            return accel;
        }

        protected override void drawModel()
        {
            if (id == firstFlyingInsectId)
            {
                // sight circle
                DrawUtils.drawCircle(this.sight, Color.Blue);

                // flockingDistance circle
                DrawUtils.drawCircle(flockingDistance, Color.Purple);
            }

            GL.Disable(EnableCap.Lighting);
            GL.Color3(Color.Yellow);
            GL.Begin(BeginMode.LineStrip);
            GL.Vertex3(location.X, location.Y, location.Z + .02f);
            GL.Vertex3(location.X, location.Y, location.Z - .02f);
            GL.End();
            GL.Enable(EnableCap.Lighting);
        }
    }

}

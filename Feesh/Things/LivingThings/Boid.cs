using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Feesh.Util;

namespace Feesh.Things.LivingThings
{
    class Boid : LivingThing
    {
        private static int firstBoidId = -1;

        protected static float minFlightSpeed = 0;

        public Boid(World aWorld) : base(aWorld)
        {
            randomizeLocation(true);
        }

        protected override void initialize()
        {
            base.initialize();

            if( firstBoidId == -1 ) {
                firstBoidId = id;
            }

            maxHeight = 60f;
            maxAccel = 70f;
            maxSpeed = 35.0f;
            minFlightSpeed = 8f;
            minSpeed = 8f;

            flockingDistance = 4f;

            sight = 13f;

            flies = true;

            _avoidable = true;

            // randomize starting velocity
            double xVel = (rand.Next(100 + id) % 5) / 4 * 10;
            if (rand.Next() % 2 == 0)
            {
                xVel *= -1f;
            }
            double zVel = (rand.Next(100 + id) % 5) / 4 * 10;
            if (rand.Next() % 2 == 0)
            {
                zVel *= -1f;
            }

            velocity = new Vector3((float)xVel, 0, (float)zVel);
        }

        protected override Vector3 calcAccel()
        {
            Vector3 accel;

            float collisionMultiplier = 7f;
            float borderMultiplier = 2f;
            float alignmentMultiplier = 1.0f;
            float cohesionMultiplier = 0.8f;
            float wanderMultiplier = 2.5f;
            float chillMultiplier = 1.0f;

            if (id == firstBoidId)
            {
                // Make this dude do his own thang
                /*borderMultiplier = 1.5f;
                alignmentMultiplier = 0.5f;
                cohesionMultiplier = 0.5f;
                wanderMultiplier = 4f;
                chillMultiplier = 0;*/
            }

            // avoid collisions with flockmates
            accel = collisionAvoidance() * collisionMultiplier;

            // avoid going off the map
            accel += (avoidBorder(world.getWorldSize()) * borderMultiplier);

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
            /*
            TODO: Textures
            glTexEnvi(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
            glBindTexture(GL_TEXTURE_2D, texture[INDEX_HERBER_TEX]);
            */

            if (id == firstBoidId)
            {
                // sight circle
                DrawUtils.drawCircle(this.sight, Color.Blue);

                // flockingDistance circle
                DrawUtils.drawCircle(flockingDistance, Color.Purple);


                //TODO: Add to DrawUtils
                GL.Begin(BeginMode.Lines);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, 0, this.sight);
                GL.End();
            }

            GL.Color3(Color.White);

            GL.PushMatrix();
            GL.Scale(new Vector3(.4f, .4f, .4f));

            base.drawModel();

            GL.PopMatrix();
        }
    }
}

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
    class Predator : LivingThing
    {
        private static int firstPredatorId = -1;

        #region Constructors
        public Predator(World aWorld) : base(aWorld)
        {
            randomizeLocation(false);
        }

        public Predator(World aWorld, Vector3 startPoint) : base(aWorld, startPoint) { }
        #endregion

        protected override void initialize()
        {
            base.initialize();

            if (firstPredatorId == -1)
            {
                firstPredatorId = id;
            }

            maxAccel = 70f;
            maxSpeed = 18.0f;
            minSpeed = 0.0f;
            minHeight = 1.5f;

            // TODO: hm...
            flockingDistance = 5f;

            sight = 15f;
            flies = false;

            _avoidable = true;

            // randomize starting velocity
            double xVel = (rand.Next(100 + id) % maxSpeed);
            if (rand.Next() % 2 == 0)
            {
                xVel *= -1f;
            }
            double zVel = (rand.Next(100 + id) % maxSpeed);
            if (rand.Next() % 2 == 0)
            {
                zVel *= -1f;
            }

            velocity = new Vector3((float)xVel, 0, (float)zVel);
        }

        protected override Vector3 calcAccel()
        {
            Vector3 accel;

            float collisionMultiplier = 6f;
            float borderMultiplier = 1.0f;
            float huntMultiplier = 2.0f;
            float alignmentMultiplier = 0.6f;
            float cohesionMultiplier = 0.6f;
            float wanderMultiplier = 4f;
            float chillMultiplier = 0.9f;


            // avoid collisions with flockmates
            accel = collisionAvoidance() * collisionMultiplier;

            if (id == firstPredatorId)
            {
                string foo = "bar";
            }


            // avoid going off the map
            accel += (avoidBorder(world.getWorldSize()) * borderMultiplier);

            accel += (hunt() * huntMultiplier);

            // align with flockmates
            accel += (alignmentMatching() * alignmentMultiplier);

            // try to stay in the group
            accel += (cohesion() * cohesionMultiplier);

            // wander a bit
            accel += (wander() * wanderMultiplier);

            // chill out!
             accel += (chill() * chillMultiplier);


            return accel;
        }

        protected Vector3 hunt()
        {
            Vector3 hunt = new Vector3(0, 0, 0);

            foreach (Thing thing in nearbyThings)
            {
                if( thing.GetType() == typeof(Herber) ) {
                    hunt += (thing.location - this.location);
                }
            }

            return hunt;
        }

        protected override void drawModel()
        {
            /*
            TODO: Textures
            glTexEnvi(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
            glBindTexture(GL_TEXTURE_2D, texture[INDEX_HERBER_TEX]);
            */

            if (id == firstPredatorId)
            {
                // sight circle
                DrawUtils.drawCircle(this.sight, Color.Blue);

                // flockingDistance circle
                DrawUtils.drawCircle(flockingDistance, Color.Purple);
            }

            GL.Color3(Color.Red);
            
            GL.PushMatrix();
            GL.Scale(new Vector3(1f, 1f, 1f));

            base.drawModel();

            GL.PopMatrix();
        }
    }
}

using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using NewFlocking.Util;

namespace NewFlocking.Things.LivingThings
{
    /***
     * The Herber class.
     * 
     * A land-based herbervorous animal that fears predators. Engages in flocking behaviour.
     */
    class Herber : LivingThing
    {
        private static int firstHerberId = -1;

        #region Constructors
        public Herber(World aWorld) : base(aWorld)
        {
            randomizeLocation(false);
        }

        public Herber(World aWorld, Vector3 startPoint) : base(aWorld, startPoint) { }
        #endregion

        protected override void initialize()
        {
            base.initialize();

            if (firstHerberId == -1)
            {
                firstHerberId = id;
            }

            maxAccel = 2.5f;
            maxSpeed = 25.0f;
            minSpeed = 0.0f;

            flockingDistance = 5f;

            sight = 11f;
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
            float alignmentMultiplier = 0.8f;
            float cohesionMultiplier = 0.8f;
            float wanderMultiplier = 2.5f;
            float chillMultiplier = 1.0f;


            // avoid collisions with flockmates
            accel = collisionAvoidance() * collisionMultiplier;

            if (id == firstHerberId)
            {
                string foo = "bar";
            }


            // avoid going off the map
            accel += (avoidBorder() * borderMultiplier);

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

        protected override void drawModel()
        {
            /*
            TODO: Textures
            glTexEnvi(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
            glBindTexture(GL_TEXTURE_2D, texture[INDEX_HERBER_TEX]);
            */

            if (id == firstHerberId)
            {
                // sight circle
                DrawUtils.drawCircle(this.sight, Color.Blue);

                // flockingDistance circle
                DrawUtils.drawCircle(flockingDistance, Color.Purple);
            }

            GL.Color3(Color.Yellow);
            
            GL.PushMatrix();
            GL.Scale(new Vector3(.7f, .7f, .7f));

            base.drawModel();

            GL.PopMatrix();
        }
    }
}

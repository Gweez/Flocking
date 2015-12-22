using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using NewFlocking.Things.StateIndicators;
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
        private const int FEAR_TTL = 175;
        private const float RELAX_SPEED = 2;
        
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

            maxAccel = 50f;
            maxSpeed = 25.0f;
            minSpeed = 0.0f;
            minHeight = 1;

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
            float fleeMultiplier = 7f;
            float alignmentMultiplier = 0.8f;
            float cohesionMultiplier = 0.8f;
            float wanderMultiplier = 2.5f;
            float chillMultiplier = 5.0f;


            // avoid collisions with flockmates
            accel = collisionAvoidance() * collisionMultiplier;

            if (id == firstHerberId)
            {
                string foo = "bar";
            }

            // avoid nasty things
            accel += (flee() * fleeMultiplier);

            // avoid going off the map
            accel += (avoidBorder(world.getWorldSize()) * borderMultiplier);

            // align with flockmates
            accel += (alignmentMatching() * alignmentMultiplier);

            // try to stay in the group
            accel += (cohesion() * cohesionMultiplier);

            if (_fearState == null || !states.Contains(_fearState))
            {
                _fearState = null;

                if (velocity.Length < RELAX_SPEED)
                {
                    // flocking instinct reduced
                    accel *= 0.25f;

                    if (_relaxState == null)
                    {
                        _relaxState = new Relaxed(this.world, this);
                        states.Add(_relaxState);
                    }
                }

                // wander a bit
                accel += (wander() * wanderMultiplier);

                // chill out!
                accel += (chill() * chillMultiplier);
            }
            else if (_relaxState != null)
            {
                states.Remove(_relaxState);
                _relaxState = null; 
            }

            return accel;
        }

        protected Vector3 flee()
        {
            Vector3 flee = new Vector3(0, 0, 0);

            foreach (Thing thing in nearbyThings)
            {
                if (thing.GetType() == typeof(Predator))
                {
                    flee += (this.location - thing.location);

                    if (_fearState == null) {
                        _fearState = new Fear(world, this, FEAR_TTL);
                        states.Add(_fearState);
                    }
                    _fearState.TTL = FEAR_TTL;
                }
            }

            return flee;
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

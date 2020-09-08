﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Feesh.Things;
using Feesh.Things.StateIndicators;

namespace Feesh.Things.LivingThings
{
    abstract class LivingThing : Thing
    {
        #region Internal Members

        private static List<Vector3> livingThingVerts;
        private static List<Vector3> livingThingFaces;

        #endregion

        #region Protected Members
        protected Fear _fearState;
        protected Relaxed _relaxState;

        protected float collisionMultiplier = 1;
        protected float borderMultiplier = 1;
        protected float alignmentMultiplier = 1;
        protected float cohesionMultiplier = 1;
        protected float wanderMultiplier = 1;
        protected float chillMultiplier = 1;
        protected float fleeMultiplier = 1;
        #endregion

        #region Public Properties

        public bool isAfraid
        {
            get
            {
                if (_fearState != null)
                {
                    if (states.Contains(_fearState))
                    {
                        return true;
                    }

                    _fearState = null;
                }

                return false;
            }
        }

        public bool isRelaxed
        {
            get
            {
                if (_relaxState != null)
                {
                    if (states.Contains(_relaxState))
                    {
                        return true;
                    }

                    _relaxState = null;
                }

                return false;
            }
        }

        #endregion

        #region Constructors
        public LivingThing(World aWorld) : base(aWorld) { }
        public LivingThing(World aWorld, Vector3 startPoint) : base(aWorld, startPoint) { }
        #endregion

        protected override void initialize()
        {
            base.initialize();

            stateLocation = new Vector3(0, 1.3f, 0.75f);
            _moves = true;

            if (livingThingVerts == null)
            {
                livingThingVerts = new List<Vector3>();

                livingThingVerts.Add(new Vector3(0, 0, 1.5f));
                livingThingVerts.Add(new Vector3(0, -1f, -1f));
                livingThingVerts.Add(new Vector3(1f, 0, -1f));
                livingThingVerts.Add(new Vector3(0, 1f, -1f));
                livingThingVerts.Add(new Vector3(-1f, -0, -1f));

                livingThingFaces = new List<Vector3>();

                livingThingFaces.Add(new Vector3(0, 4f, 1f));
                livingThingFaces.Add(new Vector3(0, 1f, 2f));
                livingThingFaces.Add(new Vector3(0, 3f, 4f));
                livingThingFaces.Add(new Vector3(0, 2f, 3f));
            }
        }

        protected override void drawModel()
        {
            Vector3 point;

            // draw triangular faces
            for (int idx = 0; idx < 4; idx++)
            {
                drawFace(idx, false);
            }

            // draw square back end
            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, 0.0, -1.0);
            for (int idx = 4; idx > 0; idx--)
            {
                point = (Vector3)livingThingVerts[idx];
                // TODO: Textures
                /*glTexCoord2d(backHerTexCoor[idx - 1][0],
                    backHerTexCoor[idx - 1][1]);*/
                GL.Vertex3(point.X, point.Y, point.Z);
            }

            GL.End();

            // Draws axes of LivingThing model
            
            //GL.Begin(BeginMode.Lines);
            //GL.Vertex3(0, 0, 0);
            //GL.Vertex3(10, 0, 0);
            //GL.End();
            //GL.Begin(BeginMode.Lines);
            //GL.Vertex3(0, 0, 0);
            //GL.Vertex3(0, 10, 0);
            //GL.End();
            //GL.Begin(BeginMode.Lines);
            //GL.Vertex3(0, 0, 0);
            //GL.Vertex3(0, 0, 10);
            //GL.End();
             
        }

        private void drawFace(int faceNum, bool wireFrame)
        {
            Vector3 normal, side1, side2, point;

            if (wireFrame)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }

            Vector3 point1, point2;

            point1 = (Vector3)livingThingVerts[Convert.ToInt32(((Vector3)livingThingFaces[faceNum]).Y)];
            point2 = (Vector3)livingThingVerts[Convert.ToInt32(((Vector3)livingThingFaces[faceNum]).X)];
            Vector3.Subtract(ref point1,
                            ref point2,
                            out side1);

            point1 = (Vector3)livingThingVerts[Convert.ToInt32(((Vector3)livingThingFaces[faceNum]).Y)];
            point2 = (Vector3)livingThingVerts[Convert.ToInt32(((Vector3)livingThingFaces[faceNum]).Z)];
            Vector3.Subtract(ref point1,
                            ref point2,
                            out side2);

            // LEFT/RIGHT CORRECT???
            Vector3.Cross(ref side2, ref side1, out normal);

            normal = Vector3.Normalize(normal);

            GL.Begin(BeginMode.Polygon);

            GL.Normal3(normal.X, normal.Y, normal.Z);

            point = (Vector3)livingThingVerts[Convert.ToInt32(((Vector3)livingThingFaces[faceNum]).X)];
            GL.Vertex3(point);
            point = (Vector3)livingThingVerts[Convert.ToInt32(((Vector3)livingThingFaces[faceNum]).Y)];
            GL.Vertex3(point);
            point = (Vector3)livingThingVerts[Convert.ToInt32(((Vector3)livingThingFaces[faceNum]).Z)];
            GL.Vertex3(point);

            GL.End();
        }

        // TODO: Display Lists
        /*private void createDisplayList()
        {
            int 

            GL.NewList(
        }*/

        protected void mate(LivingThing partner)
        {
            string fn = "LivingThing.mate: ";

            if( this.GetType() == partner.GetType()) {
                 // make new baby thing at same location
            }
            else
            {
                log.Error(fn + "Attempt to mate incompatable types!");
            }
        }


        protected Vector3 wander()
        {
            Vector3 wander = new Vector3(0, 0, 0);

            if (rand.Next(100 + id) % 4 != 0)
            {
                // don't change course
                return wander;
            }

            wander.X = rand.Next(100 + id) % 5;
            if (rand.Next(100 + id) % 4 == 0)
            {
                // reverse X direction
                if (velocity.X > 0)
                {
                    wander.X *= -1;
                }
            }
            else
            {
                // go same Z direction
                if (velocity.X < 0)
                {
                    wander.X *= -1;
                }
            }

            wander.Z = rand.Next(100 + id) % 5;
            if (rand.Next(100 + id) % 4 == 0)
            {
                // reverse Z direction
                if (velocity.Z > 0)
                {
                    wander.Z *= -1;
                }
            }
            else
            {
                // go same Z direction
                if (velocity.Z < 0)
                {
                    wander.Z *= -1;
                }
            }

            if (flies)
            {
                wander.Y = rand.Next(100 + id) % 5;
                if (rand.Next(100 + id) % 4 == 0)
                {
                    // reverse Y direction
                    if (velocity.Y > 0)
                    {
                        wander.Y *= -1;
                    }
                }
                else
                {
                    // go same Y direction
                    if (velocity.Y < 0)
                    {
                        wander.Y *= -1;
                    }
                }
            }

            return wander * wanderMultiplier;
        }

        /// <summary>
        /// If things is going over 1/2 of max velocity function
        /// returns a vector for a decrease to that speed.
        /// </summary>
        protected Vector3 chill()
        {
            Vector3 chill = new Vector3(0, 0, 0);

            if (velocity.LengthFast > maxSpeed * 0.5f)
            {
                chill = velocity / -3f;
            }

            if (flies)
            {
                // slight acceleration middle of flight height;
                float idealHeight = ((flockingDistance * 2) + maxHeight) / 2;
                chill.Y += (idealHeight - location.Y)/2;
            }

            return chill * chillMultiplier;
        }

        /// <summary>
        /// Motivates the thing towards center of map when it strays too far.
        /// </summary>
        /// <returns></returns>
        protected Vector3 avoidBorder(float distance)
        {
            float diff;
            Vector3 avoid = new Vector3(0, 0, 0);
            float worldSize = distance;

            if (location.LengthSquared > Math.Pow(worldSize, 2))
            {
                diff = location.LengthFast - worldSize;

                avoid = location * -1;
                avoid.Normalize();
                avoid = avoid * (float)Math.Pow(diff, 2)/4;
            }

            if (flies)
            {
                float worldHeight = world.getHeightAt(location);
                diff = (worldHeight + flockingDistance * 2) - location.Y;

                if (diff > 0)
                {
                    // we're too close to the ground
                    avoid += new Vector3(0,1,0) * (float)Math.Pow(diff, 2);
                }
            }

            return avoid * borderMultiplier;
        }

        /// <summary>
        /// Runs directly away from the provided predator type
        /// </summary>
        /// <returns></returns>
        protected Vector3 flee(Type predator)
        {
            Vector3 flee = new Vector3(0, 0, 0);

            foreach (Thing thing in nearbyThings)
            {
                if (thing.GetType() == predator)
                {
                    flee += (this.location - thing.location);
                }
            }

            return flee * fleeMultiplier;
        }

        /// <summary>
        /// Flocking Rule #1:
        /// Collision avoidance - Avoid collisions with flockmates and 
        /// other avoidable Things.
        /// </summary>
        /// <returns></returns>
        protected Vector3 collisionAvoidance()
        {
            string fn = "Thing.collisionAvoidance(): ";

            Vector3 avoid = new Vector3(0, 0, 0);
            Vector3 toOther;
            float distance;
            bool react;

            foreach (Thing thing in nearbyThings)
            {
                if (thing != this && thing.avoidable)
                {
                    react = false;

                    if (thing.moves)
                    {
                        toOther = Vector3.Subtract(location, thing.location);

                        distance = toOther.LengthFast;

                        if (distance < flockingDistance)
                        {
                            react = true;
                        }
                    }
                    else
                    {
                        // TODO: make this work better for non-pillars
                        Vector3 loc2d = location;
                        loc2d.Y = 0;

                        toOther = Vector3.Subtract(loc2d, thing.location);

                        distance = toOther.LengthFast;
                        react = true;
                        if (distance < 7) // TODO: hard coded for pillars
                        {
                            distance = 0.001F; // set to low number to increase reaction
                        }
                    }

                    if( react )
                    {
                        // We're too close - move away!
                        // ignore vertical option if ground-based
                        if (!flies && toOther.Y != 0)
                        {
                            toOther.Y = 0;
                        }

                        toOther = Vector3.Multiply(toOther, flockingDistance / distance);
                        avoid = Vector3.Add(avoid, toOther);
                    }

                }
            }

            if (avoid.Y != 0)
            {
                log.Error(fn + "non-zero avoid.Y: " + avoid.Y);
            }

            return avoid * collisionMultiplier;
        }

        /// <summary> 
        /// Flocking Rule #2:
        /// Alignment Matching - Steer towards average velocity of flockmates.
        /// </summary>
        protected Vector3 alignmentMatching()
        {
            const string fn = "Thing.alignmentMatching(): +";

            Vector3 match = new Vector3(0, 0, 0);

            if (visibleFlockMates.Count > 0)
            {
                // get average ideal direction
                match = Vector3.Subtract(avgVisibleFlockMateVel, velocity);
            }

            if (match.Y != 0)
            {
                log.Error(fn + " match.Y:" + match.Y);
            }

            return match * alignmentMultiplier;
        }

        /// <summary>
        /// Flocking Rule #3
        /// Cohesion - steer towards average position of neighbors
        /// </summary>
        protected Vector3 cohesion()
        {
            Vector3 cohesion = new Vector3(0, 0, 0);

            if (visibleFlockMates.Count > 0)
            {
                // get vector towards ideal position
                cohesion = Vector3.Subtract(avgVisibleFlockMatePosition, location);
            }

            return cohesion * cohesionMultiplier;
        }

        #region Unit Tests

        protected override void runTests()
        {
            // TODO: order object types are created should not affect test runs.
            base.runTests();

            testAlignment();

            testAvoidBorder();
        }

        private void testAlignment()
        {
            // SetUp
            Vector3 prevVel = this.velocity;
            Vector3 prevLoc = this.location;
            float prevSight = this.sight;

            string fn = "LivingThing.testAlignment(): ";

            this._location = new Vector3(3, 0, 0);
            this.velocity = new Vector3(3, 0, 0);

            this.sight = 10;

            nearbyThings = new List<Thing>();

            Thing aThing = new Thing();

            aThing.velocity = new Vector3(0, 0, 4);

            nearbyThings.Add(aThing);

            analyzeNearbyThings();

            Vector3 alignmentMatching = this.alignmentMatching();

            if (alignmentMatching != new Vector3(-3, 0, 4))
            {
                log.Error(fn + "Alignment test failed!");
            }

            // TearDown
            nearbyThings = null;

            this.sight = prevSight;
            this._location = prevLoc;
            this.velocity = prevVel;
        }

        private void testAvoidBorder()
        {
            string fn = "LivingThing.testAvoidBorder(): ";

            // SetUp
            Vector3 prevVel = this.velocity;
            Vector3 prevLoc = this.location;
            float prevSight = this.sight;
            bool prevFlies = this.flies;

            this.flies = false;

            this._location = new Vector3(world.getWorldSize() + 1, 0, world.getWorldSize() + 1);

            Vector3 avoid = avoidBorder(world.getWorldSize());

            // TODO: do parallel check
            if (avoid != new Vector3(-1, 0 ,-1))
            {
                log.Error(fn + " test failed!");
            }

            this.flies = true;

            this._location = new Vector3(world.getWorldSize() + 1, flockingDistance * 2 + 1 , world.getWorldSize() + 1);

            avoid = avoidBorder(world.getWorldSize());

            // TODO: do parallel check
            if (avoid != new Vector3(-1, 0, -1))
            {
                log.Error(fn + " test failed!");
            }

            this._location = new Vector3(10, flockingDistance, 10);

            avoid = avoidBorder(world.getWorldSize());

            if ( avoid.Y <= 0 )
            {
                log.Error(fn + " test failed!");
            }

            // TearDown
            nearbyThings = null;

            this.flies = prevFlies;
            this.sight = prevSight;
            this._location = prevLoc;
            this.velocity = prevVel;
        }

        #endregion Unit Tests

    }
}
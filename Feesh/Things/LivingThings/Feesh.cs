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
    class Feesh : LivingThing
    {
        private const int FEAR_TTL = 175;

        private static int firstFeeshId = -1;

        protected static int maxTailRotation = 40;
        protected float tailRotationChange = 8.0f;
        protected float tailRotation;

        protected Color4 bodyColor = Color.SlateGray;
        protected Color4 highlightColor = Color.Blue;

        public Feesh(World aWorld) : base(aWorld)
        {
            randomizeLocation(true);
        }

        protected override void initialize()
        {
            base.initialize();

            setSpeciesProperties();

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

            tailRotation = rand.Next(maxTailRotation);
            if (rand.Next() % 2 == 0) {
                tailRotation *= -1f;
            }

            if (rand.Next() % 2 == 0)
            {
                tailRotationChange *= -1f;
            }

            velocity = new Vector3((float)xVel, 0, (float)zVel);
        }

        protected virtual void setSpeciesProperties()
        {
            if (firstFeeshId == -1)
            {
                firstFeeshId = id;
            }

            maxHeight = 60f;
            maxAccel = 60f;
            maxSpeed = 30.0f;
            minSpeed = 1f;

            flockingDistance = 4.5f;

            detailDistance = 80;

            sight = 13f;

            _size = new Vector3(.4f, 1f, 2f);

            collisionMultiplier = 10f;
            borderMultiplier = 5f;
            alignmentMultiplier = 1.0f;
            cohesionMultiplier = 0.7f;
            wanderMultiplier = 2.5f;
            chillMultiplier = 1.0f;
            fleeMultiplier = 13f;

            flies = true;

            _avoidable = true;
        }

        protected override Vector3 calcAccel()
        {
            Vector3 accel;

            // avoid collisions with flockmates
            accel = collisionAvoidance();

            // avoid going off the map
            accel += avoidBorder(world.getWorldSize());

            // align with flockmates
            accel += alignmentMatching();

            // try to stay in the group
            accel += cohesion();

            // wander a bit
            accel += wander();

            // chill out!
            accel += chill();

            // avoid nasty things
            accel += flee(typeof(Shark));

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

            if (id == firstFeeshId)
            {
                //// sight circle
                //DrawUtils.drawCircle(this.sight, Color.Blue);

                //// flockingDistance circle
                //DrawUtils.drawCircle(flockingDistance, Color.Purple);


                ////TODO: Add to DrawUtils
                //GL.Begin(BeginMode.Lines);
                //GL.Vertex3(0, 0, 0);
                //GL.Vertex3(0, 0, this.sight);
                //GL.End();
            }

            // TODO: only draw details when camera is close enough
            float cameraDistance = Vector3.Subtract(location, world.Camera.location).LengthFast;

            // Front
            GL.Color4(bodyColor);
            GL.Color4(10f, 50f, 80f, 0.0f);

            GL.PushMatrix();
            GL.Scale(new Vector3(.2f, .5f, .5f));

            base.drawModel();

            // Tail
            GL.PushMatrix();
            GL.Color4(highlightColor);

            GL.Rotate(tailRotation, new Vector3(0, 1, 0));
            GL.Translate(new Vector3(0, 0, -4f));
            GL.Scale(new Vector3(.6f, .9f, .9f));

            base.drawModel();

            GL.PopMatrix();

            // Back end
            GL.PushMatrix();

            GL.Color4(bodyColor);
            GL.Rotate(180 + (tailRotation * .75f), new Vector3(0, 1, 0));
            GL.Translate(new Vector3(0, 0, 1.9f));
            GL.Scale(new Vector3(.9f, .9f, 1f));
            base.drawModel();

            GL.PopMatrix();
            GL.End();

            GL.PopMatrix();

            // dorsal -- detailed view only
            if (cameraDistance < detailDistance)
            {
                GL.PushMatrix();
                GL.Color4(highlightColor);

                GL.Rotate(-90, new Vector3(1, 0, 0));
                GL.Translate(new Vector3(0, .4f, .5f));
                GL.Scale(new Vector3(0.1f, 0.2f, 0.1f));
                base.drawModel();

                GL.PopMatrix();
            }

            // prep next tail rotation
            if (Math.Abs(tailRotation) > maxTailRotation)
            {
                tailRotationChange *= -1f;
                if (tailRotation > 0) { 
                    tailRotation = maxTailRotation;
                }
                else {
                    tailRotation = maxTailRotation * -1f;
                }
            }

            float speed = velocity.LengthFast;
            if (speed == 0)
            {
                tailRotation += (tailRotationChange * .001f);
            }
            else
            {
                tailRotation += (tailRotationChange * (velocity.LengthFast / maxSpeed));
            }
        }

        protected override void drawStatic()
        {
            // sight lines
            if (nearbyThings != null)
            {
                foreach (Thing thing in nearbyThings)
                {
                    if (id == firstFeeshId)
                    {
                        GL.Color3(Color.Blue);
                        if (thing.GetType() == typeof(Shark))
                        {
                            GL.Color3(Color.Red);
                        }
                        else if (thing.GetType() == typeof(Pillar))
                        {
                            GL.Color3(Color.White);
                        }

                        GL.Begin(BeginMode.Lines);
                        GL.Vertex3(0, 0, 0);
                        GL.Vertex3(Vector3.Subtract(thing.location, location));
                        GL.End();
                    } /*else if( thing.GetType() == typeof(Shark)) {
                        GL.Color3(Color.Red);
                        GL.Begin(BeginMode.Lines);
                        GL.Vertex3(0, 0, 0);
                        GL.Vertex3(Vector3.Subtract(thing.location, location));
                        GL.End();
                    } else if (thing.GetType() == typeof(Pillar)) { 
                        GL.Color3(Color.White);
                        GL.Begin(BeginMode.Lines);
                        GL.Vertex3(0, 0, 0);
                        GL.Vertex3(Vector3.Subtract(thing.location, location));
                        GL.End();
                    }*/
                }
            }
        }
    }
}

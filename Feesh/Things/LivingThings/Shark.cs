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
    class Shark : LivingThing
    {
        private static int firstSharkId = -1;
        private static int maxTailRotation = 30;

        private float tailRotation;
        private float tailRotationChange;


        public Shark(World aWorld) : base(aWorld)
        {
            randomizeLocation(true);
        }

        protected override void initialize()
        {
            base.initialize();

            if (firstSharkId == -1) {
                firstSharkId = id;
            }

            maxHeight = 60f;
            maxAccel = 75f;
            maxSpeed = 25.0f;
            minSpeed = .25f;

            sight = 16f;

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

            tailRotation = rand.Next(maxTailRotation);
            if (rand.Next() % 2 == 0) {
                tailRotation *= -1f;
            }

            tailRotationChange = 5.0f;
            if (rand.Next() % 2 == 0)
            {
                tailRotationChange *= -1f;
            }

            velocity = new Vector3((float)xVel, 0, (float)zVel);
        }

        protected override Vector3 calcAccel()
        {
            Vector3 accel;

            float collisionMultiplier = 10f;
            float borderMultiplier = 5f;
            float wanderMultiplier = 2.5f;
            float chillMultiplier = 2.0f;


            // avoid collisions with flockmates
            accel = collisionAvoidance() * collisionMultiplier;

            // avoid going off the map
            accel += (avoidBorder(world.getWorldSize()) * borderMultiplier);

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

            if (id == firstSharkId)
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

            // Front
            GL.Color3(Color.Blue);

            GL.PushMatrix();
            GL.Scale(new Vector3(1.1f, .75f, 1.2f));

            base.drawModel();

            // Tail
            GL.PushMatrix();
 
            GL.Rotate(tailRotation, new Vector3(0, 1, 0));
            GL.Translate(new Vector3(0, 0.1f, -4f));
            GL.Scale(new Vector3(.2f, 1.2f, .7f));

            base.drawModel();

            GL.PopMatrix();

            // Back end
            GL.PushMatrix();

            GL.Rotate(180 + (tailRotation * .75f), new Vector3(0, 1, 0));
            GL.Translate(new Vector3(0, 0, 1.9f));
            GL.Scale(new Vector3(.9f, .9f, 1.1f));
            base.drawModel();

            GL.PopMatrix();
            GL.End();

            GL.PopMatrix();

            //dorsal
            GL.PushMatrix();
            GL.Color3(Color.Red);
            GL.Rotate(-90, new Vector3(1, 0, 0));
            GL.Translate(new Vector3(0, 1, 1));
            GL.Scale(new Vector3(0.2f, 0.8f, 0.6f));
            base.drawModel();

            GL.PopMatrix();
            
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
    }
}

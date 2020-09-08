using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Feesh.Things.LivingThings
{
    class Sardine : Feesh
    {
        private static int firstSardineId;

        public Sardine(World aWorld)
            :base(aWorld)
        { }

        protected override void setSpeciesProperties()
        {
            if (firstSardineId == -1)
            {
                firstSardineId = id;
            }

            bodyColor = Color.Blue;
            highlightColor = Color.Blue;
            detailDistance = 30;

            _size = new Vector3(.08f, .15f, .6f);

            maxHeight = 30f;
            maxAccel = 45f;
            maxSpeed = 20.0f;
            minSpeed = 1f;

            flockingDistance = 1.5f;

            sight = 10f;

            collisionMultiplier = 10f;
            borderMultiplier = 5f;
            alignmentMultiplier = 1.0f;
            cohesionMultiplier = 0.75f;
            wanderMultiplier = 1.5f;
            chillMultiplier = 1.0f;
            fleeMultiplier = 5f;

            flies = true;

            _avoidable = true;
        }

        protected override void drawModel()
        {
            GL.PushMatrix();
            GL.Scale(new Vector3(.2f, .15f, .3f));

            base.drawModel();

            GL.PopMatrix();
        }
    }
}

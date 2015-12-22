using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace NewFlocking.Things.StateIndicators
{
    class StateIndicator : Thing
    {
        protected Thing parent;
        protected int ttl = -1;
        protected int age = 0;
        protected Color color = Color.Red;

        public StateIndicator(World aWorld, Thing parent)
            : base(aWorld)
        {
            this.parent = parent;
        }

        public StateIndicator(World aWorld, Thing parent, int ttl)
            : base(aWorld)
        {
            this.parent = parent;
            this.ttl = ttl;
        }

        public int TTL
        {
            get
            {
                return ttl;
            }
            set
            {
                ttl = value;
            }
        }

        public int Age
        {
            get
            {
                return age;
            }
        }

        protected override void drawModel()
        {
            GL.Color3(color);

            base.drawModel();
        }

        public override void tick(double fps)
        {
            if (ttl == 0)
            {
                parent.removeState(this);
            }
            else if (ttl == -1)
            {
                // does not expire
                return;
            }

            ttl--;
            age++;
        }
    }
}

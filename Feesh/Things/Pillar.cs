using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

using Feesh.Util;

namespace Feesh.Things
{
    class Pillar : Thing
    {
        public Pillar(World aWorld, Vector3 location)
            : base(aWorld, location)
        {
        }

        protected override void initialize()
        {
            base.initialize();

            _avoidable = true;
            _size = new Vector3(5, 85, 5);
        }

        protected override void drawModel()
        {
            DrawUtils.drawCylinder(5, 85, Color.Purple);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace NewFlocking.Util
{
    class DrawUtils
    {
        public static void drawCircle(double radius, Color color)
        {
            double x;
            double z;

            GL.Color3(color);
            GL.Begin(BeginMode.LineLoop);
            for (double i = 0; i < 2; i += 0.01)
            {
                x = radius * Math.Cos(i * Math.PI);
                z = radius * Math.Sin(i * Math.PI);
                //System.Console.WriteLine("(" + x + ", " + y + ")");

                GL.Vertex3(x, 0.01, z);
            }
            GL.End();
    
        }
    }
}

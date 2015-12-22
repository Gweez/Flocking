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

        public static Quaternion RotationBetweenVectors(Vector3 start, Vector3 dest){
 	        start.Normalize();
            dest.Normalize();
 
         	float cosTheta = Vector3.Dot(start, dest);

 	        Vector3 rotationAxis; 

 	        if (cosTheta < -1 + 0.001f){
         		// special case when vectors in opposite directions:
                // there is no "ideal" rotation axis
                // So guess one; any will do as long as it's perpendicular to start
 		        rotationAxis = Vector3.Cross(new Vector3(0.0f, 0.0f, 1.0f), start);

 		        if (rotationAxis.Length < 0.01 ) {// bad luck, they were parallel, try again!
                    rotationAxis = Vector3.Cross(new Vector3(1.0f, 0.0f, 0.0f), start);
                }
 
                rotationAxis.Normalize();

                return Quaternion.FromAxisAngle(rotationAxis, 180f);
            }

            rotationAxis =  Vector3.Cross(start, dest);

            float s = (float)Math.Sqrt((double)(1+cosTheta)*2 );

 	        float invs = 1 / s;

            return new Quaternion( rotationAxis.X * invs,
                                   rotationAxis.Y * invs,
                                   rotationAxis.Z * invs,
                                   s * 0.5f);
        }

        /// <summary>
        /// Build a rotation matrix from the specified quaternion.
        /// </summary>
        /// <param name="q">Quaternion to translate.</param>
        /// <returns>A matrix instance.</returns>
        public static Matrix4 CreateFromQuaternion(Quaternion q)
        {
            /*
            Matrix4 result = Matrix4.Identity;

			float X = q.X;
			float Y = q.Y;
			float Z = q.Z;
			float W = q.W;
			
			float xx = X * X;
			float xy = X * Y;
			float xz = X * Z;
			float xw = X * W;
			float yy = Y * Y;
			float yz = Y * Z;
			float yw = Y * W;
			float zz = Z * Z;
			float zw = Z * W;
			
			result.M11 = 1 - 2 * (yy + zz);
			result.M21 = 2 * (xy - zw);
			result.M31 = 2 * (xz + yw);

			result.M12 = 2 * (xy + zw);
			result.M22 = 1 - 2 * (xx + zz);
			result.M32 = 2 * (yz - xw);

			result.M13 = 2 * (xz - yw);
			result.M23 = 2 * (yz + xw);
			result.M33 = 1 - 2 * (xx + yy); */

            // Test with method 2
            Matrix4 m1 = new Matrix4(q.W, q.Z, -1 * q.Y, q.X,
                                     -1 * q.Z, q.W, q.X, q.Y,
                                     q.Y, -1 * q.X, q.W, q.Z,
                                     -1 * q.X, -1 * q.Y, -1 * q.Z, q.W);

            Matrix4 m2 = new Matrix4(q.W, q.Z, -1 * q.Y, -1 * q.X,
                                     -1 * q.Z, q.W, q.X, -1 * q.Y,
                                     q.Y, -1 * q.X, q.W, -1 * q.Z,
                                     q.X, q.Y, q.Z, q.W);

            Matrix4 test = m1 * m2;

            return test;
        }
    }
}

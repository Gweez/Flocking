using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using NewFlocking.Things;

namespace NewFlocking.Things.Camera
{
    class Camera : Thing
    {
        protected Vector3 camForward;
        protected Vector3 camUp;
        protected Vector3 camSide;

        protected double camSpeed;

        protected double camMove;
        protected double camStrafe;
        protected double camRaise;

        public Camera(World aWorld) : base(aWorld, new Vector3(46, 52, 60))
        {
            camUp = new  Vector3(0,1,0);

            pitch = -34;
            yaw = -32;

            baseColor = Color.DarkGray;
        }

        public override void tick(double fps)
        {
            
        }

        public Matrix4 look()
        {
            Vector3 view = Vector3.Add(location, camForward);

            return Matrix4.LookAt(location.X, location.Y, location.Z,
                view.X, view.Y, view.Z,
                camUp.X, camUp.Y, camUp.Z);
        }

        /***********************
         *					   *
         *   CAMERA CONTROLS   *
         *				   	   *
         ***********************/
        public void addPitch( float addPitch ) {
            pitch += addPitch;

            if( pitch > 85.0 ) {
                pitch = 85.0f;
            } else if( pitch < -85.0 ) {
                pitch = -85.0f;
            }
        }

        public void addYaw( float addYaw ) {
            yaw += addYaw;
            if( yaw >= 360 || yaw < -360) {
                yaw = 0;
            }
        }

        public void addMove(bool reverse) {
            if (reverse)
            {
                camMove -= camSpeed;
            }
            else
            {
                camMove += camSpeed;
            }
        }

        public void addStrafe( bool right ) {
            if (right)
            {
                camStrafe += camSpeed;
            }
            else
            {
                camStrafe -= camSpeed;
            }
        }

        public void addRaise( int raise ) {
            camRaise += raise;
        }

        public void resetPitch( ) {
            pitch = 0.0f;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace NewFlocking.Things.Camera
{
    class FlyByCamera : Camera
    {
        public FlyByCamera(World aWorld)
            : base(aWorld)
        {
            camSpeed = 10;
        }

        public override void tick(double fps)
        {
            base.tick(fps);

            float cosYaw, cosPitch;
            double sinYaw, sinPitch;
            double viewPitch, viewYaw;
            double height;
            Vector3 fwd, side, up;

            viewPitch = pitch * Math.PI / 180;
            viewYaw = yaw * Math.PI / 180;

            cosYaw = (float)Math.Cos(viewYaw);
            cosPitch = (float)Math.Cos(viewPitch);

            sinYaw = Math.Sin(viewYaw);
            sinPitch = Math.Sin(viewPitch);

            // forward vector
            camForward.X = (float)sinYaw * cosPitch;
            camForward.Y = (float)sinPitch;
            camForward.Z = cosPitch * -cosYaw;

            if (camMove != 0)
            {
                // TODO: fix keypress issue
                /*if (camMove > 1)
                {
                    camMove = 1;
                }
                else if (camMove < -1)
                {
                    camMove = -1;
                }*/

                Vector3.Multiply(ref camForward, (float)((camMove / fps) * camSpeed), out fwd);

                // THIS USED TO BE A Point Vector addition...bad change?
                _location = Vector3.Add(location, fwd);
                camMove = 0;
            }

            if (camStrafe != 0)
            {
                // TODO: fix keypress issue
                /*if (camStrafe > 1)
                {
                    camStrafe = 1;
                }
                else if (camStrafe < -1)
                {
                    camStrafe = -1;
                }*/

                // LEFT/RIGHT CORRECT???
                camSide = Vector3.Cross(camForward, camUp);
                //camSide = Vector3.Cross(camUp, camForward);

                side = Vector3.Multiply(camSide, (float)(camStrafe / fps * camSpeed));

                // THIS USED TO BE A Point Vector addition...bad change?
                _location = Vector3.Add(_location, side);
                camStrafe = 0;
            }

            if (camRaise != 0)
            {
                // TODO: fix keypress issue (2012: ???)
                if (camRaise > 1)
                {
                    camRaise = 1;
                }
                else if (camRaise < -1)
                {
                    camRaise = -1;
                }

                up = Vector3.Multiply(camUp, (float)(camRaise / fps * camSpeed));

                // THIS USED TO BE A Point Vector addition...bad change?
                _location = Vector3.Add(location, up);
                camRaise = 0;
            }

            // TODO: This should ask the world how high the ground is
            //height = getHeight(camLoc.X / 2.0, camLoc.Y / 2.0);
            height = 0.0;
            if (height < 0.0)
            {
                height = 0.0;
            }

            if (location.Y < height + 1.0)
            {
                _location.Y = (float)height + 1.0f;
            }
            else if (location.Y > 100.0)
            {
                _location.Y = 100.0f;
            }

        }
    }
}

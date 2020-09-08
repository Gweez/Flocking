/************************************************
 * 
 * Main game window and application level code.
 * 
 * Initializes windows, event handlers, controls
 * update and redraw rate, creates world.
 * 
 ************************************************/

using System;
using System.Collections;
using System.Drawing;

using log4net;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;

using System.Windows.Forms;

using NewFlocking.Things.Camera;

namespace NewFlocking
{
    class Game : GameWindow
    {
        private ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Camera userCam;
        private World theWorld;

        private static double fps = 30.0;

        private bool mouseLook = false;
        private int mouseX;
        private int mouseY;

        /// <summary>Creates a 800x600 window with the specified title.</summary>
        public Game()
            : base(1228, 1024, GraphicsMode.Default, "New Flocking")
        {
            VSync = VSyncMode.On;

        }

        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string fn = "OnLoad(): ";

            log.Info(fn + "Loading...");

            MouseState mouse = OpenTK.Input.Mouse.GetState();

            mouseX = mouse.X;
            mouseY = mouse.Y;

            theWorld = new World();

            userCam = new FlyByCamera(theWorld);
            theWorld.add(userCam);

            //GL.ClearColor(1.0f, 1.0f, 1.0f, 0.0f);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
        }

        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 300.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            string fn = "Program.OnUpdateFrame(): ";

            /*****************************
             *  Mouse Handlers
             *****************************/
            MouseState mouse = OpenTK.Input.Mouse.GetState();

            int mouseXChange;
            int mouseYChange;

            mouseXChange = mouseX - mouse.X;
            mouseYChange = mouseY - mouse.Y;
            mouseX = mouse.X;
            mouseY = mouse.Y;

            if (mouseXChange != 0 && mouseLook)
            {
                userCam.addYaw((float)mouseXChange * -0.5f);;
            }

            if (mouseYChange != 0 && mouseLook)
            {
                userCam.addPitch((float)mouseYChange * 0.5f);
            }


            /******************************
             *  Keyboard controls
             ******************************/
            if (Keyboard[Key.Escape])
            {
                Exit();
            }

            if (Keyboard[Key.F])
            {
                mouseLook = !mouseLook;
            }

            if (Keyboard[Key.W])
            {
                userCam.addMove( false );
            }

            if (Keyboard[Key.S])
            {
                userCam.addMove( true );
            }

            if (Keyboard[Key.A])
            {
                userCam.addStrafe(false);
            }

            if (Keyboard[Key.D])
            {
                userCam.addStrafe(true);
            }

            if (Keyboard[Key.Q])
            {
                userCam.addRaise(1);
            }

            if (Keyboard[Key.E])
            {
                userCam.addRaise(-1);
            }

            if (Keyboard[Key.Up])
            {
                userCam.addPitch(1);
            }

            if (Keyboard[Key.Down])
            {
                userCam.addPitch(-1);
            }

            if (Keyboard[Key.Right])
            {
                userCam.addYaw(1.2f);
            }

            if (Keyboard[Key.Left])
            {
                userCam.addYaw(-1.2f);
            }
            
            theWorld.tick(fps);
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = userCam.look();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            theWorld.drawMe();

            SwapBuffers();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // The 'using' idiom guarantees proper resource cleanup.
            // We request 30 UpdateFrame events per second, and unlimited
            // RenderFrame events (as fast as the computer can handle).
            using (Game game = new Game())
            {
                game.Run(fps);
            }
        }
    }
}
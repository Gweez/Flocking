using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using log4net;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using NewFlocking.Things.Camera;

namespace NewFlocking
{
    public partial class MainForm : Form
    {
        private ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Camera userCam;
        private World theWorld;

        private static double fps = 30.0;

        private bool mouseLook = false;
        private int mouseX;
        private int mouseY;

        private bool loaded = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("hello");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string fn = "OnLoad(): ";

            log.Info(fn + "Loading...");

//            mouseX = Mouse.X;
//            mouseY = Mouse.Y;

            theWorld = new World();

            userCam = new FlyByCamera(theWorld);
            theWorld.add(userCam);

            //GL.ClearColor(1.0f, 1.0f, 1.0f, 0.0f);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);

            //GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            //Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 300.0f);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadMatrix(ref projection);

            SetupViewport();

            loaded = true;
        }

        private void SetupViewport()
        {
            int w = mainGLControl.Width;
            int h = mainGLControl.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 300.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) // Play nice
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = userCam.look();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            theWorld.drawMe();

            mainGLControl.SwapBuffers();
        }

        private void mainGLControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //TODO: Exit();
            }

            if (e.KeyCode == Keys.F)
            {
                mouseLook = !mouseLook;
            }

            if (e.KeyCode == Keys.W)
            {
                userCam.addMove(false);
            }

            if (e.KeyCode == Keys.S)
            {
                userCam.addMove(true);
            }

            if (e.KeyCode == Keys.A)
            {
                userCam.addStrafe(false);
            }

            if (e.KeyCode == Keys.D)
            {
                userCam.addStrafe(true);
            }

            if (e.KeyCode == Keys.Q)
            {
                userCam.addRaise(1);
            }

            if (e.KeyCode == Keys.E)
            {
                userCam.addRaise(-1);
            }

            if (e.KeyCode == Keys.Up)
            {
                userCam.addPitch(1);
            }

            if (e.KeyCode == Keys.Down)
            {
                userCam.addPitch(-1);
            }

            if (e.KeyCode == Keys.Right)
            {
                userCam.addYaw(1.2f);
            }

            if (e.KeyCode == Keys.Left)
            {
                userCam.addYaw(-1.2f);
            }
        }
    }
}

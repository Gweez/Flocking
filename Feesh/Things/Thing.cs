using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using log4net;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Feesh.Things.StateIndicators;
using Feesh.Util;

namespace Feesh.Things
{
    /***
     * Base class for things in the world.
     * 
     * Represents an object or animal with a location. 
     */
    class Thing
    {
        #region Internal Members
        protected ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private int _id;

        private static List<Type> testsRun = null;

        protected Vector3 _location;
        protected Vector3 _size;

        protected float maxHeight = 1;
        protected float minHeight = 0;
        protected float maxAccel;
        protected float maxSpeed;
        protected float minSpeed;

        protected float detailDistance = 0;
        protected float sight;
        protected float flockingDistance;
        protected bool _avoidable = false;
        protected bool _moves = false;
        protected bool flies;

        protected float pitch;
        protected float yaw;

        protected Color baseColor;
        protected World world;

        protected static int lastID = 0;
        protected static Random rand;
        protected List<StateIndicator> states;
        protected List<StateIndicator> expiredStates;
        protected Vector3 stateLocation;
        protected List<Thing> nearbyThings;
        protected List<Thing> visibleThings;
        protected List<Thing> visibleFlockMates;
        protected Vector3 avgVisibleFlockMateVel;
        protected Vector3 avgVisibleFlockMatePosition;

        #endregion

        #region Constructors

        public Thing()
        {
            initialize();
        }

        public Thing(World aWorld)
        {
            world = aWorld;

            initialize();
        }

        public Thing(World aWorld, Vector3 startPoint)
        {
            initialize();

            _location = startPoint;
            world = aWorld;
        }

        public Thing(World aWorld, Vector3 startPoint, Color baseColor)
        {
            initialize();

            _location = startPoint;
            world = aWorld;
            this.baseColor = baseColor;
        }

        #endregion

        #region Public Properties

        public int id
        {
            get { return _id; }
        }

        public Vector3 location
        {
            get { return _location; }
        }

        public bool moves
        {
            get { return _moves; }
        }

        public Vector3 size
        {
            get { return _size; }
        }

        public Vector3 velocity
        {
            get;
            set;
        }

        public bool avoidable
        {
            get {return _avoidable;}
        }

        #endregion

        protected virtual void initialize() {
            if (rand == null)
            {
                rand = new Random();
            }

            _id = lastID++;

            stateLocation = new Vector3(0, 0.5f, 0.5f);

            _location = new Vector3(0, 0, 0);
            _size = new Vector3(1, 1, 1);
            baseColor = Color.White;
            velocity = new Vector3(0, 0, 0);
            maxHeight = 100;

            flockingDistance = 2f;

            sight = 0;
            flies = true;

            yaw = 0;
            pitch = 0;

            states = new List<StateIndicator>();
            expiredStates = new List<StateIndicator>();

            if (testsRun == null)
            {
                testsRun = new List<Type>();
            }

            if( !testsRun.Contains(this.GetType()) ) {
                testsRun.Add(this.GetType());
                runTests();
            }
        }

        /// <summary>
        /// Draws the thing.
        /// </summary>
        public virtual void drawMe()
        {
            GL.Color3(baseColor);

            GL.PushMatrix();
            GL.Translate(location);

            drawStatic();

            if (velocity.Length == 0)
            {
                GL.Rotate(yaw, 0.0, 1.0, 0.0);
            }
            else
            {
                // Draws thing velocity
                //GL.Begin(BeginMode.Lines);
                //GL.Vertex3(0, 0, 0);
                //GL.Vertex3(velocity.X, velocity.Y, velocity.Z);
                //GL.End();
                
                Quaternion rotationQuat = DrawUtils.RotationBetweenVectors(new Vector3(0,0,1), new Vector3(velocity));

                Matrix4 rotation = DrawUtils.CreateFromQuaternion(rotationQuat);

                GL.MultMatrix(ref rotation);
            }

            drawModel();

            if (states.Count > 0)
            {
                GL.Translate(stateLocation);

                foreach (StateIndicator si in states)
                {
                    si.drawMe();
                }
            }

            GL.PopMatrix();
            
        }

        /****
         * Draw the thing in its 1x size at (0,0,0), do not resize, rotate, or locate thing.
         */
        protected virtual void drawModel() {
            GL.PushMatrix();
            GL.Scale(0.2, 0.2, 0.2);

            GL.Begin(BeginMode.TriangleFan);
            GL.Vertex3(0.0, 1.0, 0.0);
            GL.Vertex3(0.0, 0.0, 1.0);
            GL.Vertex3(1.0, 0.0, 0.0);
            GL.Vertex3(0.0, 0.0, -1.0);
            GL.Vertex3(-1.0, 0.0, 0.0);
            GL.Vertex3(0.0, 0.0, 1.0);
            GL.End();
            GL.Begin(BeginMode.TriangleFan);
            GL.Vertex3(0.0, -1.0, 0.0);
            GL.Vertex3(0.0, 0.0, 1.0);
            GL.Vertex3(-1.0, 0.0, 0.0);
            GL.Vertex3(0.0, 0.0, -1.0);
            GL.Vertex3(1.0, 0.0, 0.0);
            GL.Vertex3(0.0, 0.0, 1.0);
            GL.End();

            GL.PopMatrix();
        }

        /// <summary>
        /// Used for debugging, draws static elements unimpacted by thing velocity
        /// </summary>
        protected virtual void drawStatic()
        {
            // no default
        }

        /// <summary>
        /// The passage of time, game logic here. 
        /// 
        /// Calculates acceleration, pitch, yaw, velocity, and position of thing.
        /// Override if you don't want that done
        /// </summary>
        /// <param name="fps">the number of frames per second</param>
        public virtual void tick(double fps) {
            string fn = "Thing.tick(): ";

            //System.Consoale.WriteLine(this.GetType().Name + " id: " + id + "  location: " + location);

            Vector3 accel = new Vector3(0,0,0);
            float velMag;

            float height = world.getHeightAt(location); 

            if (!world.isPaused())
            {
                // manage states
                foreach (StateIndicator state in states)
                {
                    state.tick(fps);
                }
                foreach (StateIndicator state in expiredStates)
                {
                    states.Remove(state);
                }
                expiredStates = new List<StateIndicator>();

                if (moves)
                {
                    nearbyThings = world.getAllTheThings();

                    if (sight > 0)
                    {
                        analyzeNearbyThings();
                    }

                    accel = calcAccel();

                    if (accel.Length > maxAccel)
                    {
                        accel *= maxAccel / accel.Length;
                    }
                }

                // TODO: Gravity - not subject to acceleration limit     
                if( !flies ) {
                    if ( location.Y > (height + minHeight))
                    {
                        accel += world.Gravity;
                    }
                    else if (location.Y < minHeight)
                    {
                        accel -= world.Gravity;
                    }
                    else
                    {
                        accel.Y = 0;
                    }
                }

                accel = Vector3.Multiply(accel, (float)(1 / fps));
                velocity = Vector3.Add(velocity, accel);
            }

            velMag = velocity.Length;
            if (velMag > maxSpeed)
            {
                velocity *= maxSpeed / velMag;
                velMag = maxSpeed;
            }
            else if (velMag < minSpeed)
            {
                if (velMag != 0.0)
                {
                    velocity *= (minSpeed / velMag);
                }
                else
                {
                    // TODO: this is a bit jank, should choose min speed
                    // vector that matches current pitch and yaw
                    velocity = new Vector3(minSpeed, velocity.Y, velocity.Z);
                }
                velMag = minSpeed;
            }
            
            // calculate direction
            if (velMag > 0)
            {
                // calculate rotation from ( 0, 0, 1 )
                yaw = (float)(Math.Acos(velocity.Z / velMag) / (Math.PI / 180));
                if (velocity.X < 0.0)
                {
                    yaw *= -1.0f;
                }


                // ??
                pitch = (float)(Math.Asin(velocity.Y / velMag) / (Math.PI / 180));


               /* if (velocity.Y < 0.0)
                {
                    pitch *= -1.0f;
                }*/
            }

            if (!world.isPaused())
            {
                _location = Vector3.Add(location, Vector3.Multiply(velocity, (float)(1.0 / fps)));

                if (location.Y < height)
                {
                    _location.Y = height;
                    if (velocity.Y < 0)
                    {
                        velocity += new Vector3(0, velocity.Y * -1, 0);
                    }
                }

                float worldSizeLimit = world.getWorldSize() + 30;
                if (location.X > worldSizeLimit)
                {
                    log.Error(fn + "Thing way out of place!");
                }
            }
        }

        public void removeState(StateIndicator state)
        {
            expiredStates.Add(state);
        }

        /// <summary>
        /// Filters list of nearby things down to only what is visible. Also populates
        /// list and average velocity & position for visible things of the same type.
        /// </summary>
        protected void analyzeNearbyThings()
        {
            string fn = "Thing.analyzeNearbyThings()";

            visibleThings = new List<Thing>();
            visibleFlockMates = new List<Thing>();
            avgVisibleFlockMatePosition = new Vector3(0, 0, 0);
            avgVisibleFlockMateVel = new Vector3(0, 0, 0);

            Vector3 distance;
            float xDiff;
            float zDiff;
            float yDiff;

            foreach (Thing thing in nearbyThings)
            {
                if (thing.id != id)
                {
                    xDiff = Math.Abs(location.X - thing.location.X) - thing.size.X;
                    zDiff = Math.Abs(location.Z - thing.location.Z) - thing.size.Z;
                    yDiff = Math.Abs(location.Y - thing.location.Y) - thing.size.Y;

                    distance = new Vector3(Math.Max(xDiff, 0), Math.Max(yDiff, 0), Math.Max(zDiff,0));

                    if (distance.LengthFast <= sight)
                    {
                        visibleThings.Add(thing);
                        if (thing.GetType() == this.GetType())
                        {
                            visibleFlockMates.Add(thing);
                            avgVisibleFlockMatePosition += thing.location;
                            avgVisibleFlockMateVel += thing.velocity;
                        }
                    }
                }
            }

            // get average velocity of visible flockmates
            if (visibleFlockMates.Count > 0)
            {
                avgVisibleFlockMatePosition = avgVisibleFlockMatePosition / visibleFlockMates.Count;
                avgVisibleFlockMateVel = avgVisibleFlockMateVel / visibleFlockMates.Count;
            }

            nearbyThings = visibleThings;
        }

        protected virtual Vector3 calcAccel()
        {
            return new Vector3(0, 0, 0);
        }

        protected void randomizeLocation(bool offGround)
        {
            // random starting location
            float worldSize = world.getWorldSize();

            float xLoc = rand.Next(((int)worldSize + id) * 3) % (worldSize * 2);
            xLoc = xLoc - (float)worldSize;

            float zLoc = rand.Next(((int)worldSize + id) * 3) % (worldSize * 2);
            zLoc = zLoc - (float)worldSize;

            float yLoc = 1;
            if (offGround)
            {
                yLoc = rand.Next(((int)worldSize + id) * 3) % maxHeight;
            }

            _location = new Vector3(xLoc, yLoc, zLoc);
        }

        protected virtual void runTests()
        {
            string foo = "bar()";
        }
     }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;
using GameUtilities;

namespace PirateWars
{
    /// <summary>
    /// This is the base class for all other classes in the game
    /// All objects in this game have a few things in common, and this class is designed to encompass all of them and make generating new classes easier
    /// </summary>


    public abstract class Object : GameComponent
    {
        /// <value>Vector2 representing the speed of the boat</value>
        protected Vector2 speed;

        /// <value>Vector2 holding the position of the boat on the screen </value>
        protected Vector2 position;

        /// <value>Vector2 that represents the origin of the texture.  This is the point where anything operation dealing with rotation or collision detection is relative to </value>
        protected Vector2 origin;

        /// <value>float that stores the current angle that the ship is facing relative to the center of the screen.  negative angles are counterclockwise rotations, and positive are clockwise.  Stored in RADIANS not degrees </value>
        protected float angle;

        ///<value>float indicting how fast the ship can turn</value>
        protected float turnSpeed;

        /// <value>filename of the image for the ship</value>
        protected string image;

        /// <value>Texture2D that is used to display the ship on screen</value>
        protected Texture2D texture;

        /// <value>Oriented Bounding Box used for detection collisions between objects</value>
        protected RectangleF Bounding;

        ///<value>creates a rectangle around each object.  Used for collision detection</value>
        public RectangleF BoundingBox
        {
            get
            {
                Bounding.Position = this.position;
                Bounding.RotateAxis(this.angle);
                return Bounding;
            }
        }

        /// <summary>
        /// Default constructor for Object.  Sets all values to 0
        /// </summary>
        public Object() :
            base(null)
        {
            speed = Vector2.Zero;
            position = Vector2.Zero;
            origin = Vector2.Zero;
            angle = 0;
            turnSpeed = 0;
            image = "";
            Bounding = new RectangleF();
        }

        /// <summary>
        /// Takes in an ObjectData sturcture and pulls all data from it, and stores it in the proper variables.  Also takes in a texture to represent this object
        /// </summary>
        /// <param name="d">ObjectData structure.  Loaded from main game through ContentPipeline</param>
        /// <param name="tex">Texture to represent object.  Loaded from main game through ContentPipeline</param>
        public Object(ObjectData d, Texture2D tex)
            : base(null)
        {
            speed = d.speed;
            position = Vector2.Zero;
            texture = tex;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);

            //angle and turnSpeed are stored as degrees in XML file.  Convert to radians
            angle = MathHelper.ToRadians(d.angle);
            turnSpeed = MathHelper.ToRadians(d.turnSpeed);
            Bounding = new RectangleF(position, texture);
        }

        /// <summary>
        /// Constructor for Objects without using ObjectData structure
        /// </summary>
        /// <param name="p">Position to spawn object</param>
        /// <param name="a">Angle to rotate object by at spawn</param>
        /// <param name="tex">Texture to represent that object</param>
        public Object(Vector2 p, float a, Texture2D tex) :base(null)
        {
            position = p;
            angle = a;
            texture = tex;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Bounding = new RectangleF(position, texture);
        }

        #region Accessors

        /// <summary>
        /// Get the speed of the boat
        /// </summary>
        /// <returns>a Vector2 representing the speed of the object</returns>
        public Vector2 Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }

        /// <summary>
        /// get the position of the boat on screen
        /// </summary>
        /// <returns><see cref="position"/></returns>
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        /// <summary>
        /// get the angle that the boat is facing
        /// </summary>
        /// <returns><see cref="angle"/></returns>
        public float Angle
        {
            get
            {
                return angle;
            }
            set
            {
                //take the incoming value and make sure to wrap it so it stays between -Pi/2 and Pi/2
                angle = RectangleF.WrapAngle(value);
            }
        }

        /// <summary>
        /// get the turn speed of the boat
        /// </summary>
        /// <returns><see cref="turnSpeed"/></returns>
        public float TurnSpeed
        {
            get
            {
                return turnSpeed;
            }
            set
            {
                turnSpeed = value;
            }
        }
        /// <summary>
        /// Get the origin of the object in relation to the object's texture.  Is usually the center of the image
        /// </summary>
        /// <returns><see cref="origin"/></returns>
        public Vector2 Origin
        {
            get
            {
                return origin;
            }
        }
        /// <summary>
        /// Get the texture that is displayed on the screen
        /// </summary>
        /// <returns><see cref="texture"/></returns>
        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                texture = value;
            }
        }
        #endregion

        #region Helpers

        /// <summary>
        /// Calculates the angle that an object should face, given its position, its
        /// target's position, its current angle, and its maximum turning speed.
        /// </summary>
        public static float TurnToFace(Vector2 position, Vector2 faceThis,
            float currentAngle, float turnSpeed)
        {
            // consider this diagram:
            //         B 
            //        /|
            //      /  |
            //    /    | y
            //  / o    |
            // A--------
            //     x
            // 
            // where A is the position of the object, B is the position of the target,
            // and "o" is the angle that the object should be facing in order to 
            // point at the target. we need to know what o is. using trig, we know that
            //      tan(theta)       = opposite / adjacent
            //      tan(o)           = y / x
            // if we take the arctan of both sides of this equation...
            //      arctan( tan(o) ) = arctan( y / x )
            //      o                = arctan( y / x )
            // so, we can use x and y to find o, our "desiredAngle."
            // x and y are just the differences in position between the two objects.
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            // we'll use the Atan2 function. Atan will calculates the arc tangent of 
            // y / x for us, and has the added benefit that it will use the signs of x
            // and y to determine what cartesian quadrant to put the result in.
            float desiredAngle = (float)Math.Atan2(y, x);
            // so now we know where we WANT to be facing, and where we ARE facing...
            // if we weren't constrained by turnSpeed, this would be easy: we'd just 
            // return desiredAngle.
            // instead, we have to calculate how much we WANT to turn, and then make
            // sure that's not more than turnSpeed.

            // first, figure out how much we want to turn, using WrapAngle to get our
            // result from -Pi to Pi ( -180 degrees to 180 degrees )
            float difference = RectangleF.WrapAngle(desiredAngle - currentAngle);

            // clamp that between -turnSpeed and turnSpeed.
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            // so, the closest we can get to our target is currentAngle + difference.
            // return that, using WrapAngle again.
            return RectangleF.WrapAngle(currentAngle + difference);
        }//end TurnToFace
        #endregion
    }//end class
}//end namespace

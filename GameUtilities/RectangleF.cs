using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace GameUtilities
{
    /// <summary>
    /// Oriented Bounding Box class using floating point values. Designed to act like the XNA Rectangle class but with floating point values.
    /// Uses Seperating Axis Theroem to detected collisions
    /// For more on OBBs and the SAT:
    /// http://www.metanetsoftware.com/technique/tutorialA.html
    /// http://gamedev.stackexchange.com/questions/43855/how-do-i-get-the-axes-for-sat-collision-detection
    /// http://www.jkh.me/files/tutorials/Separating%20Axis%20Theorem%20for%20Oriented%20Bounding%20Boxes.pdf
    /// </summary>
    public class RectangleF
    {
        /// <value>Vector repesenting the position on the screen</value>
        protected Vector2 position;

        /// <value>Vector representing the dimensions of this bounding box. X is the width, Y is the height</value>
        protected Vector2 dimensions;

        /// <value>Vector representing the half width when the bounding box is not rotated (angle = 0)</value>
        protected Vector2 originalHalfX;

        /// <value>Vector representing the half height when the bounding box is not rotated (angle = 0)</value>
        protected Vector2 originalHalfY;

        /// <value>Vector representing the half width when the bounding box is rotated by an angle in relation to this box's origin (angle != 0)</value>
        protected Vector2 rotatedHalfX;

        /// <value>Vector repesenting the half height when the bounding box is rotated by an angle in relation to this box's origin (angle != 0)</value>
        protected Vector2 rotatedHalfY;

        /// <value>Vector represnting the coordinates of the center of the box (relative to the dimensions of the box)</value>
        protected Vector2 origin;

        /// <value>Vector representing the x-axis of the box at the current angle.  It is the normalized rotated half-width vector</value>
        protected Vector2 xAxis;

        /// <value>Vector representing the y-axis of the box at the current angle.  It is the normalized rotated half-height vector</value>
        protected Vector2 yAxis;
        /// <value>The angle (in radians) that the box is rotated around the box's origin</value>
        float angle;

        /// <summary>
        /// Default Constructor.  Sets all values to 0
        /// </summary>
        public RectangleF()
        {
            position = Vector2.Zero;
            dimensions = Vector2.Zero;
            angle = 0;
            xAxis = rotatedHalfX = originalHalfX = Vector2.Zero;
            yAxis = rotatedHalfY = originalHalfY = Vector2.Zero;
        }

        /// <summary>
        /// Constructs new RectangleF from a vector representing position, and another representing its texture.
        /// </summary>
        /// <param name="p">Vector representing the coordinates where this object is on screen</param>
        /// <param name="t">Texture that this rectangle is drawn around.</param>
        public RectangleF(Vector2 p, Texture2D t)
        {
            position = p;
            dimensions.X = t.Width;
            dimensions.Y = t.Height;
            angle = 0;
            /*
             *  A---------------B
             *  |       |       |
             *  |       |       |
             *  |-------O-------|
             *  |       |       |
             *  |       |       |
             *  D---------------C
             *  
             * This is the bounding box around a non rotated sprite.
             * O is the center of the sprite.  It is also the (X,Y) coordinate that the sprite is drawn on screen.
             * The Vector2 pos is the position on the screen
             * The Vector2 o is the XY coordinate that indicates the center of the sprite in relation to the size of the sprite.
             */
            origin = new Vector2(t.Width / 2, t.Height / 2);
            originalHalfX = new Vector2(dimensions.X / 2, 0);
            originalHalfY = new Vector2(0, dimensions.Y / 2);

            //The internal axes of the rectangle are equal to the halfX and halfY values, but they need to be normalized
            xAxis = originalHalfX;
            yAxis = originalHalfY;
            xAxis.Normalize();      //normalized x-axis
            yAxis.Normalize();      //normalized y-axis

            //initially, the rectangle is not rotated.  So the rotated half lengths equal the original half lenghts
            rotatedHalfX = originalHalfX;
            rotatedHalfY = originalHalfY;
        }
        /// <summary>
        /// Check if two RectangleF's intersect using the Separating Axis Theorem
        /// </summary>
        /// <param name="B">The rectangle that this one is being checked against for collisions</param>
        /// <returns></returns>
        public bool Intersects(RectangleF B)
        {
            /*
             * There are four cases when using the Seperating Axis Theorem with axis aligned bounding boxes
             *      Case 1: there is a seperating line along the first box's X-AXIS
             *      Case 2: there is a seperating line along the first box's Y-AXIS
             *      Case 3: there is a seperating line along the second box's X-AXIS
             *      Case 4: there is a seperating line along the second box's Y-AXIS
             *  If any of these four cases is true, then there is no collision (there exists a seperating axis between the two objects)
             *  So, apply each of the four cases, if any is true, return false.  If the are all false, return true
             *  The general seperating axis theorem algorithm is as follows:
             *      | T • L | > | ( WA*Ax ) • L | + | ( HA*Ay ) • L | + | ( WB*Bx ) • L | + |( HB*By ) • L |
             *      
             *      VARIABLE KEY
             *          T   =   distance between object A and B                 (B.Position - A.Position)
             *          L   =   the axis you are checking along                 (changes for each case)
             *          
             *          WA  =   the halfwidth of object A. Changes when rotated (A.HalfX)
             *          HA  =   the halfheight of object A. "       "    "      (A.HalfY)
             *          Ax  =   the vector describing the x-axis of object A    (Vector2.Normalize(A.HalfX))
             *          Ay  =   the vector describing the y-axis of object A    (Vector2.Normalize(A.HalfY))
             *          
             *          WB  =   the halfwidth of object B. Changes when rotated (B.HalfX)
             *          HB  =   the halfheight of object B. "       "    "      (B.HalfY)
             *          Bx  =   the vector describing the x-axis of object B    (Vector2.Normalize(B.HalfX))
             *          By  =   the vector describing the y-axis of object B    (Vector2.Normalize(B.HalfY))
             * It is important to take the absolute value of each dot product.  Depending on the angle of the OBB, the dot product could be negative or positive.  Allowing negative values can lead to false-positive tests, so everything needs to be kept in a >=0 range.
             * This algorithm was found from the following pdf
             * http://www.jkh.me/files/tutorials/Separating%20Axis%20Theorem%20for%20Oriented%20Bounding%20Boxes.pdf
             */

            //calculate the distance between the objects, and the axes of the objects (normalized half width, and half height)
            //The normalize function passes by reference.  So need to set axes variables equal to the half values and then normalize them
            Vector2 distance = B.Position - this.position;
            Vector2 Ax = this.XAxis;
            Vector2 Ay = this.YAxis;
            Vector2 Bx = B.XAxis;
            Vector2 By = B.YAxis;

            //case 1: check along THIS rectangles X Axis
            float transform = Math.Abs(Vector2.Dot((this.HalfX * Ax), Ax))
                            + Math.Abs(Vector2.Dot((this.HalfY * Ay), Ax))
                            + Math.Abs(Vector2.Dot((B.HalfX * Bx), Ax))
                            + Math.Abs(Vector2.Dot((B.HalfY * By), Ax));
            if (Math.Abs(Vector2.Dot(distance, Ax)) > transform)
                return false;


            //case 2: check along THIs rectangle's Y Axis
            transform = Math.Abs(Vector2.Dot((this.HalfX * Ax), Ay))
                        + Math.Abs(Vector2.Dot((this.HalfY * Ay), Ay))
                        + Math.Abs(Vector2.Dot((B.HalfX * Bx), Ay))
                        + Math.Abs(Vector2.Dot((B.HalfY * By), Ay));
            if (Math.Abs(Vector2.Dot(distance, Ay)) > transform)
                return false;

            //case 3: check along B's X-Axis
            transform = Math.Abs(Vector2.Dot((this.HalfX * Ax), Bx))
                        + Math.Abs(Vector2.Dot((this.HalfY * Ay), Bx))
                        + Math.Abs(Vector2.Dot((B.HalfX * Bx), Bx))
                        + Math.Abs(Vector2.Dot((B.HalfY * By), Bx));

            if (Math.Abs(Vector2.Dot(distance, Bx)) > transform)
                return false;

            //case 4: check along B's Y-Axis
            transform = Math.Abs(Vector2.Dot((this.HalfX * Ax), By))
                        + Math.Abs(Vector2.Dot((this.HalfY * Ay), By))
                        + Math.Abs(Vector2.Dot((B.HalfX * Bx), By))
                        + Math.Abs(Vector2.Dot((B.HalfY * By), By));
            if (Math.Abs(Vector2.Dot(distance, By)) > transform)
                return false;

            //if all cases are true, then there is a collision
            return true;
        }
        /// <summary>
        /// A different collision detection algorithm using very basic axis aligned boxes.
        /// </summary>
        /// <param name="r">The other rectangle that is being checked against</param>
        /// <returns>returns true if the two boxes overlap, false otherwise</returns>
        public bool IntersectsB(RectangleF r)
        {
            /* 
             * (x,y)  (x+w,y)
             * ----------
             * |        |           
             * |        |
             * |        |
             * ----------
             * (x,y+h)(x+w,y+h)
             */
            //if the this retangle's position + its dimension are less than the the other retangle's position, then the two are not intersecting.  Same is true the other way around.
            if (position.X + dimensions.X < r.position.X)
                return false;
            if (r.position.X + r.dimensions.X < this.position.X)
                return false;
            if (this.position.Y + this.dimensions.Y < r.position.Y)
                return false;
            if (r.position.Y + r.dimensions.Y < this.position.Y)
                return false;
            return true;
        }
        /// <summary>
        /// Rotate the half width and length vectors as well as the internal x and y axes by some angle in radians 
        /// </summary>
        /// <param name="a"></param>
        public void RotateAxis(float a)
        {
            /*
             * When an object rotates by an angle a, its bounding box must all rotate. The rotation is done by using a rotation matrix
             * |rotated_x|  =   |original_x||cos(a) -sin(a)| 
             * |rotated_y|      |original_y||sin(a) cos(a) |
             * the following code is just the expanded form of the above matrix
             * 
             * Angle = 0 means that the object is perfectly horizontal. An angle of -pi/2 means that the object is pointing perfectly upward.  The angle measurement is the rotation from the horizontal around the center of the object.
             * 
             * All calculations done through the rotation matrix are accomplished by using the data from when the object was perfectly horizontal
             */
            angle = WrapAngle(a);

            //rotate the original HalfX value by the angle a
            rotatedHalfX = RotateVector(originalHalfX, a);
            
            //rotate the original HalfY value by the angle a
            rotatedHalfY = RotateVector(originalHalfY, a);

            //the internal axes of the box also change as the box is rotated.  They are the normalized half width and length vectors
            xAxis = rotatedHalfX;
            yAxis = rotatedHalfY;
            xAxis.Normalize();
            yAxis.Normalize();
        }
        /// <summary>
        /// Return a string that can be displayed containing all data for this RectangleF.  Used primarily for debugging
        /// </summary>
        /// <returns>String of data</returns>
        public string Print()
        {
            string output;
            output = "POSITION: \t" + position;
            output += "\nDIMENSIONS: \t" + dimensions;
            output += "\n\tO_HX: \t" + originalHalfX + "\t" + (Math.Sqrt((Math.Pow(originalHalfX.X, 2) + Math.Pow(originalHalfX.Y, 2))));
            output += "\n\tO_HY: \t" + originalHalfY + "\t" + (Math.Sqrt((Math.Pow(originalHalfY.X, 2) + Math.Pow(originalHalfY.Y, 2))));
            output += "\n\tR_HX: \t" + this.HalfX + "\t" + (Math.Sqrt((Math.Pow(this.HalfX.X, 2) + Math.Pow(this.HalfX.Y, 2)))); ;
            output += "\n\tR_HY: \t" + this.HalfY + "\t" + (Math.Sqrt((Math.Pow(this.HalfY.X, 2) + Math.Pow(this.HalfY.Y, 2)))); ;
            output += "\n\tXA: \t" + this.XAxis;
            output += "\n\tYA: \t" + this.YAxis;
            output += "\n\tANGLE: \t" + MathHelper.ToDegrees(angle);

            return output;
        }

        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// <param name="radians">the angle to wrap, in radians.</param>
        /// <returns>the input value expressed in radians from -Pi to Pi.</returns>
        /// </summary>
        public static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }//end WrapAngle

        /// <summary>
        /// Access and set the position values of the OBB
        /// </summary>
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
        /// Access the X coordinate of the position vector
        /// </summary>
        public float X
        {
            get
            {
                return position.X;
            }
        }
        /// <summary>
        /// Access the Y coordinate of the position vector
        /// </summary>
        public float Y
        {
            get
            {
                return position.Y;
            }
        }
        /// <summary>
        /// Access the Width of the OBB
        /// </summary>
        public float Width
        {
            get
            {
                return dimensions.X;
            }
        }
        /// <summary>
        /// Access the Height of the OBB
        /// </summary>
        public float Height
        {
            get
            {
                return dimensions.Y;
            }
        }
        /// <summary>
        /// Access the rotated half width vectors
        /// </summary>
        public Vector2 HalfX
        {
            get
            {
                return rotatedHalfX;
            }
        }
        /// <summary>
        /// Access the rotated half length vector
        /// </summary>
        public Vector2 HalfY
        {
            get
            {
                return rotatedHalfY;
            }
        }
        /// <summary>
        /// Access the vector representing the internal x-axis of the OBB
        /// </summary>
        public Vector2 XAxis
        {
            get
            {
                return xAxis;
            }
        }
        /// <summary>
        /// Access the vector reprsenting the internal y-axis of the OBB
        /// </summary>
        public Vector2 YAxis
        {
            get
            {
                return yAxis;
            }
        }

        /// <summary>
        /// Rotate a vector by some angle (in radians)
        /// </summary>
        /// <param name="v">The vector to be rotated</param>
        /// <param name="a">The angle to rotate the vector by</param>
        /// <returns>A new rotated vector</returns>
        public static Vector2 RotateVector(Vector2 v, float a)
        {
            Vector2 r = Vector2.Zero;
            r.X = (float)((v.X * Math.Cos(a)) - (v.Y * Math.Sin(a)));
            r.Y = (float)((v.X * Math.Sin(a)) + (v.Y * Math.Cos(a)));
            return r;
        }
    }//end class
}//end namespace

/*
 * 
 * 
 *                                      |__
                                     |\/
                                     ---
                                     / | [
                              !      | |||
                            _/|     _/|-++'
                        +  +--|    |--|--|_ |-
                     { /|__|  |/\__|  |--- |||__/
                    +---------------___[}-_===_.'____                 /\
                ____`-' ||___-{]_| _[}-  |     |_[___\==--            \/   _
 __..._____--==/___]_|__|_____________________________[___\==--____,------' .7
|                                                                     BB-61/
 \_________________________________________________________________________|*/

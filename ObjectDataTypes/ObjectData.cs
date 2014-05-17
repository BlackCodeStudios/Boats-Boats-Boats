using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ObjectDataTypes
{
    /// <summary>
    /// Holds all data needed to contstruct a new Object in PirateWars.  This class is designed to be serialized and deserialized from an XML document.
    /// </summary>
    public class ObjectData
    {
        /// <value>Vector2 representing the speed of the boat</value>
        public Vector2 speed;
        /// <value>Vector2 that represents the origin of the texture.  This is the point where anything operation dealing with rotation or collision detection is relative to </value>
        public float angle;
        ///<value>float indicting how fast the ship can turn</value>
        public float turnSpeed;
        
        /// <summary>
        /// Construct a new ObjectData instance.  Sets all data equal to 0
        /// </summary>
        public ObjectData()
        {
            speed = Vector2.Zero;
            angle = turnSpeed = 0;
        }
        /// <summary>
        /// Create a string that holds all data from the class so that it can be easily displayed
        /// </summary>
        /// <returns>String with all data</returns>
        public string PrintData() 
        {
            string output = "";
            output += ("Speed: " + speed.X + "\n");
            output += ("Turn Speed: " + turnSpeed + "\n");
            return output;
        }
    }
}

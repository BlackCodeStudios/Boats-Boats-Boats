using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ObjectDataTypes
{
    public class ObjectData
    {
        /// <value>Vector2 representing the speed of the boat</value>
        public Vector2 speed;
        /// <value>Vector2 that represents the origin of the texture.  This is the point where anything operation dealing with rotation or collision detection is relative to </value>
        public float angle;
        ///<value>float indicting how fast the ship can turn</value>
        public float turnSpeed;
        /// <value>filename of the image for the ship</value>
        public string image;
        /// <value>Texture2D that is used to display the ship on screen</value>
        
        //default constructor sets all values to 0
        public ObjectData()
        {
            speed = Vector2.Zero;
            angle = turnSpeed = 0;
            image = "";
        }

        public string PrintData() 
        {
            string output = "";
            output += ("Speed: " + speed.X + "\n");
            output += ("Turn Speed: " + turnSpeed + "\n");
            return output;
        }
    }
}

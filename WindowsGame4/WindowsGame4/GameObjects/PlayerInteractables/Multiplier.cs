using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;

namespace PirateWars
{
    /// <summary>
    /// If the player collides with one of these, the score multiplier in game increase by 1. 
    /// </summary>
    public class Multiplier : PlayerInteractable
    {
        /// <summary>
        /// Default Constructor.  Constructs a new multiplier without using an ObjectData strucutre
        /// </summary>
        /// <param name="p">Vector2 representing position</param>
        /// <param name="d">Vector2 representing the c_direction the multiplier is moving in</param>
        /// <param name="a">Vector2 representing teh rotation of the multiplier about its origin</param>
        /// <param name="tex">Texture to represent the multiplier</param>
        public Multiplier(Vector2 p, Vector2 d, float a, Texture2D tex) : base(p,d,a,tex)
        {
            speed = new Vector2(.25f, .25f);
        }
        
        /// <summary>
        /// Constructs new multiplier from an ObjectData structure and a texture
        /// </summary>
        /// <param name="d">ObjectData structure loaded from an XML file that is loaded from the ContentPipeline</param>
        /// <param name="tex">Texture to represent that image</param>
        public Multiplier(ObjectData d, Texture2D tex) : base(d, tex) { }
        
        /// <summary>
        /// Activate the multiplier's ability.  Since it does nothing to a ship, calling this method does nothing.
        /// </summary>
        /// <param name="s">The ship to act against</param>
        public override void ActivateAbility(Ship s)
        {
            //do nothing.  No ability to call
        }
    }//end Multiplier
}//end namespace
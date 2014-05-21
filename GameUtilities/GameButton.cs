using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameUtilities
{
    /// <summary>
    /// Button class that contains a texture and a position
    /// </summary>
    public class GameButton
    {
        private Texture2D image;
        private Vector2 position;
        /// <summary>
        /// Constructs a new Button using a Texture for that Button and the position it should be on screen
        /// </summary>
        /// <param name="i">Texture that represents this button</param>
        /// <param name="p">Position on the screen where the button is located</param>
        public GameButton(Texture2D i, Vector2 p)
        {
            image = i;
            position = p;
        }
        /// <summary>
        /// Access the texture
        /// </summary>
        /// <returns></returns>
        public Texture2D Texture
        {
            get
            {
                return image;
            }
        }
        public Vector2 Position
        {
            get
            {
                return position;
            }
        }
    }
}
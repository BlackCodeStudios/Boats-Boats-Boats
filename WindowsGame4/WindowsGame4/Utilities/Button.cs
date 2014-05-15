using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace PirateWars
{
    /// <summary>
    /// Button class that handles checking if a mouse has properly clicked on it.
    /// </summary>
    class Button
    {
        private Texture2D image;
        private Vector2 position;
        /// <summary>
        /// Constructs a new Button using a Texture for that Button and the position it should be on screen
        /// </summary>
        /// <param name="i">Texture that represents this button</param>
        /// <param name="p">Position on the screen where the button is located</param>
        public Button(Texture2D i, Vector2 p)
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
        public bool buttonClicked(MouseState mState) 
        {
            if (mState.LeftButton == ButtonState.Pressed)
            {
                if (mState.X >= position.X && mState.X <= position.X + image.Width)
                {
                    if (mState.Y >= position.Y && mState.Y <= position.Y + image.Height)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
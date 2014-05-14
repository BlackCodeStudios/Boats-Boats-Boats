using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace PirateWars
{
    class Button
    {
        private Texture2D image;
        private Vector2 position;
        public Button(Texture2D i, Vector2 p)
        {
            image = i;
            position = p;
        }
        public Texture2D getTexture() 
        {
            return image;
        }
        public Vector2 getPosition() 
        {
            return position;
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
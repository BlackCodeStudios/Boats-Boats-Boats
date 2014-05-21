using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace GameUtilities
{
    public class TextField
    {
        string text;
        Vector2 position;

        public TextField()
        {
            text = "";
            position = Vector2.Zero;
        }

        public TextField(string s, Vector2 p) 
        {
            text = s;
            position = p;
        }

        public string Text
        {
            get
            {
                return text;
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

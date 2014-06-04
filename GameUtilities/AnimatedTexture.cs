using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameUtilities
{
    public class AnimatedTexture
    {
        Texture2D texture;
        Action<TimeSpan, AnimatedTexture> animation;
        Vector2 position;
        float angle;
        TimeSpan time;
        Vector2 origin;
        public AnimatedTexture(Texture2D t, Vector2 p, Action<TimeSpan, AnimatedTexture> a)
        {
            texture = t;
            animation = a;
            position = p;
            angle = 0;
            time = TimeSpan.Zero;
            origin = new Vector2(t.Width / 2, t.Height / 2);
        }

        public AnimatedTexture(Texture2D t, Vector2 p, Action<TimeSpan, AnimatedTexture> a, float theta)
            : this(t, p, a)
        {
            angle = theta;

        }
        public void Update(GameTime gameTime)
        {
            time += gameTime.ElapsedGameTime;
            animation(time,this);
        }

        public Texture2D Texture
        {
            get
            {
                return texture;
            }
        }

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

        public float Angle
        {
            get
            {
                return angle;
            }
            set
            {
                angle = value;
            }
        }

        public Vector2 Origin
        {
            get
            {
                return origin;
            }
        }

        public Action Animation
        {
            get
            {
                return Animation;
            }
        }
    }
}

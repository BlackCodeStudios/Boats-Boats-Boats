using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace GameUtilities
{
    public class MenuButton
    {
        GameButton button;
        Action action;

        public MenuButton(Texture2D t, Vector2 p, Action a)
        {
            button = new GameButton(t, p);
            action = a;
        }

        public MenuButton(GameButton b, Action a)
        {
            button = b;
            action = a;
        }

        public GameButton Button
        {
            get
            {
                return button;
            }
        }

        public Action Action
        {
            get
            {
                return action;
            }
        }
    }
}

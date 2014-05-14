using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;

namespace PirateWars
{
    class Multiplier : PlayerInteractable
    {
        public Multiplier(Vector2 p, Vector2 d, float a, Texture2D tex) : base(p,d,a,tex)
        {
            speed = new Vector2(.25f, .25f);
        }
        public Multiplier(ObjectData d, Texture2D tex) : base(d, tex) { }
        public override void ActivateAbility(Ship s)
        {
            //do nothing.  No ability to call
        }
    }//end Multiplier
}//end namespace
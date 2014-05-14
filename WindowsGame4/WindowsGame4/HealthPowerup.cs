using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;

namespace PirateWars
{
    class HealthPowerup : Multiplier
    {
        float healthBonus;
        public HealthPowerup(Vector2 p, Vector2 d, float a, Texture2D tex)
            : base(p, d, a, tex)
        {
            speed = new Vector2(.3f, .3f);
            direction = new Vector2(0, 1);
            healthBonus = 10;
        }
        public HealthPowerup(ObjectData d, Texture2D tex) : base(d, tex) { }
        public override void ActivateAbility(Ship s)
        {
            s.giveHealth(s.getMaxHealth()/10);  //give back 10% of health
        }
    }
}

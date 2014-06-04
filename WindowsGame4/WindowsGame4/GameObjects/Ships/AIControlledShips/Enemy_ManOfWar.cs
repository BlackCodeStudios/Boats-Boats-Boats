using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;


namespace PirateWars
{
    /// <summary>
    /// Another type of Enemy.  Behaves the same as an Enemy_Brig but with different values.
    /// </summary>
    public class Enemy_ManOfWar : Enemy_Brig
    {
        public Enemy_ManOfWar()
            : base()
        {
            range = 350;
            cannons = 4;
            health = 40;
            damage = 5;
            image = "ManOfWar - Enemy.png";
            maxHealth = health;
            rateOfFire = 2000;
            speed /= 3;
            score = 25;
            turnSpeed = MathHelper.ToRadians(0.2f);
        }

        public Enemy_ManOfWar(EnemyData d, Texture2D tex, Texture2D cBTex) : base(d, tex, cBTex) { }
    }
}

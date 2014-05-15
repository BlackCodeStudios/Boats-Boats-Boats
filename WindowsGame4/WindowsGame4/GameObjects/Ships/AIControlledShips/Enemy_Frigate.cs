using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;


namespace PirateWars
{
    /// <summary>
    /// Another type of Enemy.  Behaves the same as an Enemy_Brig but with different data values.
    /// </summary>
    class Enemy_Frigate : Enemy_Brig
    {
        EnemyData dataEF = new EnemyData();
        public Enemy_Frigate()
            : base()
        {
            range = 300;
            cannons = 3;
            health = 35;
            damage = 5;
            image = "Frigate - Enemy.png";
            maxHealth = health;
            rateOfFire = 2500;
            speed /= 2;
            score = 15;
            turnSpeed = MathHelper.ToRadians(0.5f);
        }

        public Enemy_Frigate(EnemyData d, Texture2D tex, Texture2D cbTex) : base(d, tex, cbTex) { }
    }
}

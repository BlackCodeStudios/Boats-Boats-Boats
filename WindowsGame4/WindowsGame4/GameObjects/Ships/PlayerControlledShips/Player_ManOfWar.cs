using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;

namespace PirateWars
{
    class Player_ManOfWar : Player
    {
        Vector2 baseSpeed;
        public Player_ManOfWar()
            : base()
        {
            image = "ManOfWar";
            speed = new Vector2(3.0f, 3.0f);
            cannons = 6;
            rateOfFire = 550;
            health = 150;
            turnSpeed = MathHelper.ToRadians(1);
            damage = 15;
            maxHealth = health;
            ABILITY_RECHARGE_MAX = 5;
        }
        public Player_ManOfWar(Texture2D tex, Texture2D cBTex)
            :this()
        {
            texture = tex;
            CannonBallTexture = tex;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            baseSpeed = speed;
        }
        public Player_ManOfWar(PlayerData d, Texture2D tex, Texture2D cBTex) 
            : base(d,tex,cBTex)
        {
           
        }
        public override bool ActivateAbility(TimeSpan gameTime)
        {
            if (base.ActivateAbility(gameTime) == false)
                return false;
            speed = Vector2.Zero;
            damageResistance = 1;
            return true;
        }
        public override bool DeactivateAbility(TimeSpan gameTime)
        {
            if (base.DeactivateAbility(gameTime) == false)
                return false;
            damageResistance = 0;
            speed = new Vector2(3.0f, 3.0f);
            return true;
        }
    }
}

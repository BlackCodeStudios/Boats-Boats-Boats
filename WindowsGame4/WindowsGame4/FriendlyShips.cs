using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;


namespace PirateWars
{
    class FriendlyShips : Enemy_Brig
    {
        public FriendlyShips()
            : base()
        {
            speed = Vector2.Zero;
            cannons = 4;
            rateOfFire = 450;
            health = 125;
            turnSpeed = MathHelper.ToRadians(2);
            damage = 15;
            score = 10;
            maxHealth = health;
            state = EnemyState.Firing;
        }

        public FriendlyShips(EnemyData d, Texture2D tex, Texture2D cBTex) : base(d, tex, cBTex) 
        {
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            speed = Vector2.Zero;
        }
        
        public override void UpdateAndMove(GameTime gameTime, Ship enemy)
        {
            //TimeSpan newGameTime = gameTime.TotalGameTime;
            //assume the firing position.
            this.angle = base.TurnToFire(position, enemy.getPosition(), this.angle, turnSpeed);
            Vector2 heading = new Vector2((float)Math.Cos(this.angle), (float)Math.Sin(this.angle));
            if (gameTime.TotalGameTime.TotalMilliseconds - lastFire.TotalMilliseconds > rateOfFire)
            {
                base.Fire();
                lastFire = gameTime.TotalGameTime;
            }
            base.Update(gameTime.TotalGameTime);
        }
    }
}

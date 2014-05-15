using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;


namespace PirateWars
{
    /// <summary>
    /// Friendly Ships behave in the same manner as the Enemy objects because they are AI controlled.  Friendly Ships however do not move and shoot at Enemy ships
    /// </summary>
    public class FriendlyShips : Enemy_Brig
    {
        /// <summary>
        /// Default Constructor for FriendlyShips. Creates a new FriendlyShip with hardcoded values
        /// </summary>
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
        /// <summary>
        /// Construct new FriendlyShip.  Requires EnemyData structure, Texture to represent it and a texture to represent its projectiles.
        /// </summary>
        /// <param name="d">EnemyData structure loaded from the main game from an XML file through the ContentPipeline.</param>
        /// <param name="tex">Texture to represent the ship</param>
        /// <param name="cBTex">Texture to represent the ship's projectiles</param>
        public FriendlyShips(EnemyData d, Texture2D tex, Texture2D cBTex) : base(d, tex, cBTex) 
        {
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            speed = Vector2.Zero;
        }
        
        /// <summary>
        /// Update the rotation of the FriendlyShip towards an Enemy and fire at that Enemy
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        /// <param name="enemy">Enemy data so that the FriendlyShip can aim and fire at it</param>
        public override void UpdateAndMove(TimeSpan gameTime, Ship enemy)
        {
            //assume the firing position.
            this.angle = base.TurnToFire(position, enemy.Position, this.angle, turnSpeed);
            Vector2 heading = new Vector2((float)Math.Cos(this.angle), (float)Math.Sin(this.angle));
            if (gameTime.TotalMilliseconds - lastFire.TotalMilliseconds > rateOfFire)
            {
                base.Fire();
                lastFire = gameTime;
            }
            base.Update(gameTime);
        }
    }
}

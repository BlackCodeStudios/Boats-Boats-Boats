using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;

namespace PirateWars
{
    /// <summary>
    /// Main Enemy class. Extends the ship class and the only difference is the movement mechanics
    /// </summary>
    public class Enemy_Brig : Enemy
    {
        /// <summary>
        /// Defualt constructor for an Enemy Brig.  It is the same as any default enemy constructor
        /// </summary>
        public Enemy_Brig()
            : base()
        {
            range = 250;
            cannons = 2;
            health = 25;
            damage = 5;
            image = "Brig2_1 - Enemy";
            maxHealth = health;
            rateOfFire = 2000;
            speed /= 2;
            score = 10;
            turnSpeed = MathHelper.ToRadians(2);
        }

        /// <summary>
        /// Constructs new Enemy Brig.  This constructor is exactly the same as for the Enemy class.
        /// </summary>
        /// <param name="d">EnemyData structure that holds all initialization data.  Loaded from the main game using the ContentPipeline</param>
        /// <param name="tex">Texture representing this enemy</param>
        /// <param name="cBTex">Texture representing enemy cannon balls</param>
        public Enemy_Brig(EnemyData d, Texture2D tex, Texture2D cBTex) : base(d, tex, cBTex) { }
        
        /// <summary>
        /// Controls moving the enemy ships towards the player, and controls their firing mechanics, as well as moving cannon balls after they have been fired
        /// </summary>
        /// <param name="gameTime">used to restrict the Enemy's rate of fire</param>
        /// <param name="player">Provides player data so that Enemies can respond to events accordingly</param>
        public override void UpdateAndMove(TimeSpan gameTime, Ship player)
        {
            float distanceFromPlayer = Vector2.Distance(position, player.Position);
            //if the enemy is chasing the player, then move the enemy in a heading along that angle
            //otherwise, it is in firing mode, so the enemy should not move and instead orient itself to be parallel with the player
            if (distanceFromPlayer <= range)
            {
                state = EnemyState.Firing;
            }
            if (state == EnemyState.Chasing)
            {
                //calculate the angle towards the player and set the enemy's angle to that angle
                this.angle = TurnToFace(position, player.Position, this.angle, turnSpeed);
                Vector2 heading = new Vector2((float)Math.Cos(this.angle), (float)Math.Sin(this.angle));
                this.position += heading * this.speed;
            }

            //now that the position has been set, check to see if it needs to fire
            if (state == EnemyState.Firing)
            {
                //assume the firing position.
                this.angle = TurnToFire(position, player.Position, this.angle, turnSpeed);
                Vector2 heading = new Vector2((float)Math.Cos(this.angle), (float)Math.Sin(this.angle));
                if (gameTime.TotalMilliseconds - lastFire.TotalMilliseconds > rateOfFire)
                {
                    this.Fire();
                    lastFire = gameTime;
                }
                base.Update(gameTime);
            }//end if firing
        }//end UpdateAndMove


        /// <summary>
        /// This is the same as TurnToFace, but instead it turns the Enemy to be parallel with the player
        /// </summary>
        /// <param name="position">Position of the object that you want to turn</param>
        /// <param name="faceThis">Position of the object that you to turn towards</param>
        /// <param name="currentAngle">angle of the object you want to turn</param>
        /// <param name="turnSpeed">turn speed of the object that you want to turn</param>
        /// <returns>the angle that makes this object parallel with another</returns>
        protected float TurnToFire(Vector2 position, Vector2 faceThis, float currentAngle, float turnSpeed)
        {
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            float desiredAngle = (float)Math.Atan2(y, x) + (float)Math.PI / 2;
            float difference = WrapAngle(desiredAngle - currentAngle);
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            return WrapAngle(currentAngle + difference);
        }//end turn to fire
    }//end class
}//end namepsace

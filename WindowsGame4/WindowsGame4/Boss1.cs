using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectDataTypes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace PirateWars
{
    /// <summary>
    /// Boss type for main game type.  Is a type of enemy and shares many characteristics with Enemy_Brig but overrides functions to give the Boss unique movement and unique projectile movement
    /// </summary>
    class Boss1 : Enemy_Brig
    {
        /// <summary>
        /// Keep track of the state of the Boss.  The Boss has three states: MovingToPosition, Firing, and Fleeing
        /// </summary>
        protected enum BossState
        {
            /// <summary>
            /// The Boss is moving to a predetermined position before it can fire
            /// </summary>
            MovingToPosition,
            
            /// <summary>
            /// The Boss has reached the predetermined position and it is firing
            /// </summary>
            Firing,
            
            /// <summary>
            /// The Boss is fleeing 
            /// </summary>
            Fleeing
        };
        protected BossState bossState;
        protected Vector2 startingPos;
        protected float CANNON_BALL_PERIOD;
        protected float BURST_COUNT;
        protected float TIME_BETWEEN_BURSTS = 200;
        protected TimeSpan lastBurst = TimeSpan.Zero;
        /// <summary>
        /// Construct a new Boss.  Takes an EnemyData structure, a texture to represent it, a texture to represent its projectiles, and the vector holding the coordinates where the spawns
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tex"></param>
        /// <param name="cBTex"></param>
        /// <param name="start"></param>
        public Boss1(EnemyData data, Texture2D tex, Texture2D cBTex, Vector2 start)
            : base(data, tex, cBTex)
        {
            bossState = BossState.MovingToPosition;
            startingPos = start;
            CANNON_BALL_PERIOD = (float)(Math.PI);
            BURST_COUNT = 0;
        }

        /// <summary>
        /// Fire another round of projectiles.  The Boss's projectiles move in a sinusodial motion.
        /// </summary>
        /// <param name="gameTime">provides a snapshot of timing values</param>
        private void Fire(GameTime gameTime)
        {
            /*
             * increment is the space between the cannons (and thus each cannon ball)
             * the spacing is related to the direction that the boat is facing * an increment value (35.0f)
             */
            Vector2 boatDirection = new Vector2((float)(Math.Cos(this.angle)), (float)(Math.Sin(this.angle))); ;

            boatDirection.Normalize();

            Vector2 increment = boatDirection * (texture.Width / cannons);
            for (int i = 1; i <= cannons; i++)
            {
                //direction is perpendicular to the boat and pointing off its left side
                Vector2 direction = new Vector2((float)(Math.Cos(this.angle - MathHelper.PiOver2)), (float)(Math.Sin(this.angle - MathHelper.PiOver2)));
                //Normalize the direction vector (magnitude of 1)
                direction.Normalize();

                //generate the x and y coordinates for each cannon ball on this line.  Shift the x and y coordinates over appropriately
                float x = this.position.X;
                float y = this.position.Y + (float)(5 * Math.Sin(CANNON_BALL_PERIOD * gameTime.TotalGameTime.TotalSeconds));
                //mutliplying the increment by the value of i moves the cannon balls down.  i = 0 starts at the first cannon, and i+1 is the next cannon

                Vector2 pos = new Vector2(x, y);
                pos = pos + ((cannons - i) - (cannons / 2)) * increment;
                //Vector2 posL = this.position + (((cannons - i) - (cannons / 2)) * increment);

                //the position of the cannon ball is its current position plus the normalized direction vector times the speed
                pos += direction * cannonBallVelocity;
                float a = (float)(Math.Atan2(direction.Y, direction.X));
                CBA.Add(new CannonBall(pos, direction, this.CannonBallTexture, this.damage, cannonBallVelocity, (float)Math.Atan2(direction.Y, direction.X)));
            }
        }

        /// <summary>
        /// Override the base Update function.  The Boss's cannon balls are designed to move in a sinusodial motion rather than linearly like all other enemies.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(TimeSpan gameTime)
        {
            //update CannonBall movement
            for (int i = 0; i < CBA.Count; i++)
            {
                //move the cannon balls
                CannonBall c = CBA.ElementAt(i);
                c.setPosition(RotateDirection(c.getDirection(), c.getPosition(), c.getSpeed(), gameTime));
                //Console.WriteLine("CannonBall " + i + ": " + pos);
            }//end for i
            //Console.WriteLine("\n");
        }
        /// <summary>
        /// Rotate the direction of a cannon ball along a sine function
        /// </summary>
        /// <param name="dir">Current direction that the projectile is moving</param>
        /// <param name="pos">Current position that the projectile is at</param>
        /// <param name="speed">The speed of the projectile</param>
        /// <param name="time">Snapshot of timing values to control the sine function</param>
        /// <returns>Vector representing the new position of the projectile</returns>
        protected Vector2 RotateDirection(Vector2 dir, Vector2 pos, Vector2 speed, TimeSpan time)
        { 
            //The projectiles still move linearly along the x-axis, but sinusodially along the y-axis.
            float x = (pos.X + (dir.X * speed.X)) - (this.position.X + this.origin.X);
            float y = ((pos.Y + (dir.Y * speed.Y)) + (5 * (float)Math.Sin((.5f) * MathHelper.TwoPi * time.TotalSeconds))) - (this.position.Y + this.origin.Y);
 
            //increment the position of the projectile and return a new vector with the data
            x += (this.position.X + this.origin.X);
            y += (this.position.Y + this.origin.Y);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Update all projectiles and move the boss
        /// </summary>
        /// <param name="gameTime">Snapshot of time values</param>
        /// <param name="player">Player data so that the boss can respond to events accordingly</param>
        public override void UpdateAndMove(GameTime gameTime, Ship player)
        {
            TimeSpan newGameTime = gameTime.TotalGameTime;
            float distance = Vector2.Distance(position, startingPos);
            if (distance <= 50)
                bossState = BossState.Firing;
            //if the boss is moving to position, just move towards it
            if (bossState == BossState.MovingToPosition)
            {
                //calculate the angle towards the player and set the enemy's angle to that angle
                this.angle = TurnToFace(position, startingPos, this.angle, turnSpeed);
                Vector2 heading = new Vector2((float)Math.Cos(this.angle), (float)Math.Sin(this.angle));
                this.position += heading * this.speed;
            }
            //if the boss is set to fire, start firing projectiles
            else if (bossState == BossState.Firing)
            {
                //assume the firing position.
                this.angle = TurnToFire(position, player.getPosition(), this.angle, turnSpeed);
                Vector2 heading = new Vector2((float)Math.Cos(this.angle), (float)Math.Sin(this.angle));
                
                //the boss fires a burst of 10 projectiles with a break between each projectile in the burst, and a break in between each full burst
                if (newGameTime.TotalMilliseconds - lastFire.TotalMilliseconds > rateOfFire && newGameTime.TotalMilliseconds - lastBurst.TotalMilliseconds > TIME_BETWEEN_BURSTS)
                {
                    lastBurst = newGameTime;
                    this.Fire(gameTime);
                    if (++BURST_COUNT > 9)
                    {
                        BURST_COUNT = 0;
                        lastFire = newGameTime;
                    }
                }
                this.Update(gameTime.TotalGameTime);
            }
        }
    }
}

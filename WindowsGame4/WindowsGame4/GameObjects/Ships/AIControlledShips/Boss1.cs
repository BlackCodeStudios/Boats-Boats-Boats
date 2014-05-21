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
    public class Boss1 : Enemy_Brig
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

        /// <value>Current state of the boss</value>
        protected BossState bossState;

        /// <value>The position that the Boss needs to get to in order to start firing</value>
        private Vector2 startingPos;
        
        /// <value>The period of the sine function governing cannon ball movement for this class</value>
        private float CANNON_BALL_PERIOD;
        
        /// <value>The number of cannon balls fired in this round of firing</value>
        private float burstCount;
        
        /// <value>The total number of cannon balls fired per burst fire</value>
        private float TOTAL_PER_BURST;

        /// <value>The time between each cannon ball in a burst</value>
        private float TIME_BETWEEN_BURSTS = 200;

        /// <value>the time when the most recent cannon ball was fired in a given burst</value>
        private TimeSpan lastBurst = TimeSpan.Zero;
        
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
            burstCount = 0;
            TOTAL_PER_BURST = 10;
        }

        /// <summary>
        /// Fire another round of projectiles.  The Boss's projectiles move in a sinusodial motion.
        /// </summary>
        /// <param name="gameTime">provides a snapshot of timing values</param>
        public override void Fire(TimeSpan gameTime)
        {
            if (CanFire(gameTime) == false)
                return;
            /*
             * increment is the space between the cannons (and thus each cannon ball)
             * the spacing is related to the direction that the boat is facing * an increment value (35.0f)
             */
            Vector2 boatDirection = new Vector2((float)(Math.Cos(this.angle)), (float)(Math.Sin(this.angle))); ;

            boatDirection.Normalize();

            Vector2 increment = this.Direction * (texture.Width / cannons);
            for (int i = 1; i <= cannons; i++)
            {
                //c_direction is perpendicular to the boat and pointing off its left side
                Vector2 c_direction = InitialProjectileDirection();

                //Create cannon ball spawning points.
                float x = this.position.X;
                float y = this.position.Y + (float)(5 * Math.Sin(CANNON_BALL_PERIOD * gameTime.TotalSeconds));
               
                //mutliplying the increment by the value of i moves the cannon balls down.  i = 0 starts at the first cannon, and i+1 is the next cannon
                Vector2 pos = new Vector2(x, y);
                pos = pos + ((cannons - i) - (cannons / 2)) * increment;

                //the position of the cannon ball is its current position plus the normalized c_direction vector times the speed
                pos += c_direction * cannonBallVelocity;
                float a = (float)(Math.Atan2(c_direction.Y, c_direction.X));
                CBA.Add(new CannonBall(pos, c_direction, this.CannonBallTexture, this.damage, cannonBallVelocity, (float)Math.Atan2(c_direction.Y, c_direction.X)));
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
                c.Position = (MoveSinusoidally(c, gameTime));
                c.Angle = (float)(Math.Sin(MathHelper.Pi * gameTime.TotalSeconds));
            }
        }

        /// <summary>
        /// Move an object sinusoidally
        /// </summary>
        /// <param name="c">The cannon ball who's values are being modified</param>
        /// <param name="time">Snapshot of timing values to control the sine function</param>
        /// <returns>Vector representing the new position of the projectile</returns>
        protected Vector2 MoveSinusoidally(CannonBall c, TimeSpan time)
        { 
            //The projectiles still move linearly along the x-axis, but sinusodially along the y-axis.
            float x = (c.Position.X + (c.Direction.X * speed.X));
            float y = ((c.Position.Y + (c.Direction.Y * speed.Y)) + (2.5f * (float)Math.Sin((.5f) * MathHelper.TwoPi * time.TotalSeconds)));
            return new Vector2(x, y);
        }

        /// <summary>
        /// Update all projectiles and move the boss
        /// </summary>
        /// <param name="gameTime">Snapshot of time values</param>
        /// <param name="player">Player data so that the boss can respond to events accordingly</param>
        public override void UpdateAndMove(TimeSpan gameTime, Ship player)
        {
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
                this.angle = TurnToFire(position, player.Position, this.angle, turnSpeed);
                Vector2 heading = new Vector2((float)Math.Cos(this.angle), (float)Math.Sin(this.angle));
                
                //the boss fires a burst of 10 projectiles with a break between each projectile in the burst, and a break in between each full burst
                if (gameTime.TotalMilliseconds - lastFire.TotalMilliseconds > rateOfFire && gameTime.TotalMilliseconds - lastBurst.TotalMilliseconds > TIME_BETWEEN_BURSTS)
                {
                    //update time of last burst
                    lastBurst = gameTime;
                    //fire another salvo of projectiles
                    this.Fire(gameTime);

                    if (++burstCount > TOTAL_PER_BURST)
                    {
                        //Burst is complete, so this round of firing is complete too, reset burst count to 0, and set lastFire to current time
                        burstCount = 0;
                        lastFire = gameTime;
                    }
                }

                //Move cannon balls
                this.Update(gameTime);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;

namespace PirateWars
{
    #region Enemy
    /// <summary>
    /// Abstract class Enemy type.  All enemy types will inherit from this class.  Enemy defines all extra protected variables that are needed that were not defined in Ship as well as all extra functions.
    /// </summary>
    public abstract class Enemy : Ship
    {
        #region Protected Variables
        /// <value>Describes how many pixels away the player can be before the ship is allowed to start firing at them</value>
        protected float range;
        /// <value>The amount added to the player's score upon defeating this enemy</value>
        protected int score;
        /// <summary>
        /// Enemy ships have two states, Chasing and Firing.  Under Chasing, they move towards the player until they are within a certain range.  Once they are within this range, they switch to firing mode.  They no longer move under Firing mode and instead just rotate to aim at the player
        /// </summary>
        protected enum EnemyState
        {
            /// <summary>
            /// the Enemy AI is set to chasing when it is following the player and getting into range to fire at it
            /// </summary>
            Chasing,
            /// <summary>
            /// The Enemy AI is set to Firing when it has gotten within range of the player and then begins shooting at the player
            /// </summary>
            Firing
        };

        /// <value>Sets the initial state of the enemy to chasing the player </value>
        protected EnemyState state = EnemyState.Chasing;
        
        /// <value>Keeps track of when this ship last fired.  Regulates rate of fire in the UpdateAndFire method</value>
        protected TimeSpan lastFire;
        
        #endregion

        /// <summary>
        /// default constructor for enemy class.  shares the same constructor as the ship class
        /// </summary>
        protected Enemy()
            : base()
        {
            range = 250;
            rateOfFire = 1000;
            speed = new Vector2(5, 5);
            turnSpeed = 5.0f * ((float)Math.PI / 180.0f);
            image = "Brig2_1 - Enemy";
            cannons = 2;
            health = 10;
            damage = 5;
            score = 10;
            cannonBallVelocity = new Vector2(4.0f, 4.0f);
            maxHealth = health;
        }

        /// <summary>
        /// Construct new Enemy from EnemyData structure, a texture for the enemy, and a texture for its cannon balls
        /// </summary>
        /// <param name="e">EnemyData structure that holds all initialization data.  Loaded from the main game using the ContentPipeline</param>
        /// <param name="tex">Texture representing this enemy</param>
        /// <param name="cBTex">Texture representing enemy cannon balls</param>
        public Enemy(EnemyData e, Texture2D tex, Texture2D cBTex) 
            : base(e,tex,cBTex)
        {
            range = e.range;
            score = e.score;
            state = EnemyState.Chasing;
        }

        /// <summary>
        /// get the amount of points this ships is worth for killing
        /// </summary>
        /// <returns><see cref="score"/></returns>
        public int getScore()
        {
            return score;
        }
        /// <summary>
        /// Update the enemy's state and move the enemy towards the player
        /// </summary>
        /// <param name="gameTime">Timing snapshot used to regulate rate of fire</param>
        /// <param name="player">The player's game information so that the AI can react accordingly</param>
        public virtual void UpdateAndMove(TimeSpan gameTime, Ship player)
        {
            float distanceFromPlayer = Vector2.Distance(position, player.Position);
            //if the enemy is chasing the player, then move the enemy in a heading along that angle
            //otherwise, it is in firing mode, so the enemy should not move and instead orient itself to be parallel with the player
            if (state == EnemyState.Chasing)
            {
                //calculate the angle towards the player and set the enemy's angle to that angle
                this.angle = TurnToFace(position, player.Position, this.angle, turnSpeed);
                Vector2 heading = new Vector2((float)Math.Cos(this.angle), (float)Math.Sin(this.angle));
                this.position += heading * this.speed;
            }
        }//end update and move
    }//end class Enemy
    #endregion //Enemy
}

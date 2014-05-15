using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;

namespace PirateWars
{
    #region Ship
    /// <summary>
    /// Abstract class holding all the variables necessary for both player and enemy types.  Also contains basic functions for updating the ship which are universal for both player and enemy types
    /// </summary>
    public abstract class Ship : Object
    {
        /// <value>number of cannons per side on the ship (total/2)</value>
        protected int cannons;
        /// <value>double indicating what the delay between cannon shots is in miliseconds</value>
        protected double rateOfFire;
        /// <value>list containing the active cannon balls fired by this ship</value>
        protected List<CannonBall> CBA;
        /// <value>the speed at which this ships cannon balls should move</value>
        protected Vector2 cannonBallVelocity;
        ///<value>the texture for this ships cannon balls</value>
        protected Texture2D CannonBallTexture;
        /// <value>health of the ship</value>
        protected float health;
        ///<value>max health that the ship can have</value>
        protected float maxHealth;
        ///<value>damage that the cannonballs should deal</value>
        protected float damage;
        /// <value>reduces incoming damage.  Values range from 0 to 1, where 0 means no damage resitance and 1 means invincibility</value>
        protected float damageResistance;

        #region Constructors
        /// <summary>
        /// The default constructor for all ships.  All of them share these starting characteristics
        /// </summary>
        public Ship()
        {
            position = Vector2.Zero;
            angle = -(float)(Math.PI / 2.0f);
            origin = Vector2.Zero; //the origin is set the top left initially
            CBA = new List<CannonBall>();
            cannonBallVelocity = new Vector2(7.0f, 7.0f);
            damageResistance = 0;
            maxHealth = health;
        }//end default constructor

        /// <summary>
        /// Construct new Ship from a ShipData structure, a texture to represent the ship, and a texture to represent the ship's projectiles
        /// </summary>
        /// <param name="d">ShipData strucutre. Usually taken from XML file loaded in main game through ContentPipeline</param>
        /// <param name="tex">Texture to represent ship</param>
        /// <param name="cBTex">Texture to represent projectiles</param>
        public Ship(ShipData d, Texture2D tex, Texture2D cBTex)
            : base(d, tex)
        {
            CannonBallTexture = cBTex;
            cannonBallVelocity = d.cannonBallVelocity;
            rateOfFire = d.rateOfFire;
            CBA = new List<CannonBall>();
            damageResistance = d.damageResistance;
            damage = d.damage;
            health = maxHealth = d.health;
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            cannons = d.cannons;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get the health of the ship
        /// </summary>
        /// <returns><see cref="health"/></returns>
        public float Health
        {
            get
            {
                return health;
            }
        }

        /// <summary>
        /// get the max health that the ship can have;
        /// </summary>
        /// <returns><see cref="maxHealth"/></returns>
        public float MaxHealth
        {
            get
            {
                return maxHealth;
            }
        }

        /// <summary>
        /// get the damage that this ship does
        /// </summary>
        /// <returns><see cref="damage"/></returns>
        public float Damage
        {
            get
            {
                return damage;
            }
            set 
            {
                damage = value;
            }
        }

        /// <summary>
        /// get the number of cannons on each side of the ship
        /// </summary>
        /// <returns><see cref="cannons"/></returns>
        public int Cannons
        {
            get
            {
                return cannons;
            }
        }//end getcannons

        /// <summary>
        /// get the delay between cannon shots in miliseconds
        /// </summary>
        /// <returns><see cref="rateOfFire"/></returns>
        public double RateOfFire
        {
            get
            {
                return rateOfFire;
            }
            set 
            {
                rateOfFire = value;
            }
        }//end getROF

        /// <summary>
        /// Get the list of active cannon balls that this ship has fired.  
        /// </summary>
        /// <returns> <see cref="CBA"/> </returns>
        public List<CannonBall> CannonBalls
        {
            get
            {
                return CBA;
            }
        }



        /// <summary>
        /// get the texture for this ships cannon balls
        /// </summary>
        /// <returns><see cref="CannonBallTexture"/></returns>
        public Texture2D CBTexture
        {
            get
            {
                return CannonBallTexture;
            }
        }

        #endregion //Accessors
        
        /// <summary>
        /// Deal damage to a ship. Checks to see what the damage resistance of the ship is and adjusts the damage dealt accordingly
        /// </summary>
        ///<param name="damageTaken">The damage that is dealt to the ship</param>
        public void takeDamage(float damageTaken)
        {
            //if damage resistance is 1, then this will generate health - 0; else it will be health - damagTaken
            health -= (damageTaken - (damageTaken * damageResistance));
        }

        /// <summary>
        /// give the ship more health.  Checks to make sure that the health given is no more than the maximum health allocated
        /// </summary>
        /// <param name="healthGiven">Value of health to give</param>
        public void giveHealth(float healthGiven)
        {
            health = MathHelper.Clamp(health + healthGiven, 0.0f, maxHealth);
        }
        #endregion

        #region Updating
        /// <summary>
        /// Create new cannon balls (cannons * 2) and place them in the <see cref="CBA"/>.  All cannon balls are fired perpindicular to the boat.  Cannon number of cannon balls go off the left side, and the same number go off the right.  This function first creates the c_direction for a left side cannon ball and then inverts it for the right side
        /// Different ships fire different looking cannonBalls, so it is necessary to pass the type of cannonBall into the fire funtion.
        /// </summary>
        public virtual void Fire()
        {
            /*
             * increment is the space between the cannons (and thus each cannon ball)
             * the spacing is related to the c_direction that the boat is facing * an increment value (35.0f)
             */
            Vector2 increment = this.Direction * (texture.Width / cannons);
            for (int i = 1; i <= cannons; i++)
            {
                //get the initial c_direction of the cannon ball.  By default, cannon balls will move linearly along this c_direction vector until they dissappear off screen.
                Vector2 c_direction = InitialProjectileDirection();

                //mutliplying the increment by the value of i moves the cannon balls down.  i = 0 starts at the first cannon, and i+1 is the next cannon
                Vector2 posL = this.position + (((cannons - i) - (cannons / 2)) * increment);
                Vector2 posR = this.position + (((cannons - i) - (cannons / 2)) * increment);

                //the position of the cannon ball is its current position plus the normalized c_direction vector times the speed
                posL += c_direction * cannonBallVelocity;     //off left side
                posR += -c_direction * cannonBallVelocity;    //off right side

                //add Cannon Balls to List of cannon balls
                CBA.Add(new CannonBall(posL, c_direction, this.CannonBallTexture, this.damage, cannonBallVelocity, Object.WrapAngle(this.angle - MathHelper.PiOver2)));      //add left side cannon ball
                CBA.Add(new CannonBall(posR, -c_direction, this.CannonBallTexture, this.damage, cannonBallVelocity, Object.WrapAngle(this.angle + MathHelper.PiOver2)));    //add right side cannon ball
            }
        }

        /// <summary>
        /// Updates the movement of cannonballs.  This way each ship is in charge of updating it's own cannon balls rather than having to loop through each ship somewhere in the main game program
        /// </summary>
        /// <param name="gameTime">provides a snapshot of timing values</param>
        public virtual void Update(TimeSpan gameTime)
        {

            //update CannonBall movement
            for (int i = 0; i < CBA.Count; i++)
            {
                //move the cannon balls
                CannonBall c = CBA.ElementAt(i);
                Vector2 pos = c.Position + (c.Direction * c.Speed);
                c.Position = pos;
            }//end for i
        }//end update
        #endregion

        #region Helpers
        /// <summary>
        /// Calculate the c_direction that the ship is facing.  The vector it returns is normalized
        /// </summary>
        protected Vector2 Direction
        {
            get
            {
                Vector2 d = new Vector2((float)(Math.Cos(this.angle)), (float)(Math.Sin(this.angle)));
                d.Normalize();
                return d;
            }
        }
        /// <summary>
        /// Get the initial c_direction that a cannon ball will travel.  It is perpindicular to the c_direction of the ship
        /// </summary>
        /// <returns>Normalized vector describing the c_direction a projectile will move</returns>
        protected Vector2 InitialProjectileDirection()
        {
            Vector2 d = new Vector2((float)(Math.Cos(this.angle - MathHelper.PiOver2)), (float)(Math.Sin(this.angle - MathHelper.PiOver2)));
            d.Normalize();
            return d;
        }
        #endregion
    }//end ship
}
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
        public float getHealth()
        {
            return health;
        }
        /// <summary>
        /// get the max health that the ship can have;
        /// </summary>
        /// <returns><see cref="maxHealth"/></returns>
        public float getMaxHealth()
        {
            return maxHealth;
        }
        /// <summary>
        /// get the damage that this ship does
        /// </summary>
        /// <returns><see cref="damage"/></returns>
        public float getDamage()
        {
            return damage;
        }
        /// <summary>
        /// get the number of cannons on each side of the ship
        /// </summary>
        /// <returns><see cref="cannons"/></returns>
        public int getCannons()
        {
            return cannons;
        }//end getcannons

        /// <summary>
        /// get the delay between cannon shots in miliseconds
        /// </summary>
        /// <returns><see cref="rateOfFire"/></returns>
        public double getROF()
        {
            return rateOfFire;
        }//end getROF

        /// <summary>
        /// Get the list of active cannon balls that this ship has fired.  
        /// </summary>
        /// <returns> <see cref="CBA"/> </returns>
        public List<CannonBall> getCBA()
        {
            return CBA;
        }



        /// <summary>
        /// get the texture for this ships cannon balls
        /// </summary>
        /// <returns><see cref="CannonBallTexture"/></returns>
        public Texture2D getCannonBallTexture()
        {
            return CannonBallTexture;
        }

        #endregion //Accessors


        #region Mutators

        /// <summary>
        /// change the number of cannons on a side of the boat
        /// </summary>
        /// <param name="newC">the new number of cannons per side</param>
        public void setCannons(int newC)
        {
            cannons = newC;
        }//end setCannons

        /// <summary>
        /// change the delay in between cannon shots (in miliseconds)
        /// </summary>
        /// <param name="newR">the new delay</param>
        public void setROF(double newR)
        {
            rateOfFire = newR;
        }//end setROF




        /// <summary>
        /// change the health of the ship
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
        /// Create new cannon balls (cannons * 2) and place them in the <see cref="CBA"/>.  All cannon balls are fired perpindicular to the boat.  Cannon number of cannon balls go off the left side, and the same number go off the right.  This function first creates the direction for a left side cannon ball and then inverts it for the right side
        /// Different ships fire different looking cannonBalls, so it is necessary to pass the type of cannonBall into the fire funtion.
        /// </summary>
        public virtual void Fire()
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

                //mutliplying the increment by the value of i moves the cannon balls down.  i = 0 starts at the first cannon, and i+1 is the next cannon
                Vector2 posL = this.position + (((cannons - i) - (cannons / 2)) * increment);
                Vector2 posR = this.position + (((cannons - i) - (cannons / 2)) * increment);

                //the position of the cannon ball is its current position plus the normalized direction vector times the speed
                posL += direction * cannonBallVelocity;     //off left side
                posR += -direction * cannonBallVelocity;    //off right side
                //float a = (float)Math.Atan2(direction.Y, direction.X);
                CBA.Add(new CannonBall(posL, direction, this.CannonBallTexture, this.damage, cannonBallVelocity,Object.WrapAngle(this.angle-MathHelper.PiOver2)));
                CBA.Add(new CannonBall(posR, -direction, this.CannonBallTexture, this.damage, cannonBallVelocity,Object.WrapAngle(this.angle+ MathHelper.PiOver2)));

                 /*Console.WriteLine("CB: " + i + ".L" + CBA.ElementAt(CBA.Count-2).getPosition() + "; " + CBA.ElementAt(CBA.Count-2).getAngle() + "; " + CBA.ElementAt(CBA.Count-2).BoundingBox.XAxis + "; " + CBA.ElementAt(CBA.Count-2).BoundingBox.YAxis);
                 Console.WriteLine("CB: " + i + ".R" + CBA.ElementAt(CBA.Count - 1).getPosition() + "; " + CBA.ElementAt(CBA.Count - 1).getAngle() + "; " + CBA.ElementAt(CBA.Count - 1).BoundingBox.XAxis + "; " + CBA.ElementAt(CBA.Count - 1).BoundingBox.YAxis);
             */
            }
        }//end Fire

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
                Vector2 pos = c.getPosition() + (c.getDirection() * c.getSpeed());
                c.setPosition(pos);
                /*Console.WriteLine("CB " + i + ": " + c.getAngle() + "; " + c.BoundingBox.HalfX + "; " + c.BoundingBox.HalfY + "; " + c.BoundingBox.XAxis + "; " + c.BoundingBox.YAxis);
                 */ 
            }//end for i
        }//end update
        #endregion
    }//end ship
    #endregion
}
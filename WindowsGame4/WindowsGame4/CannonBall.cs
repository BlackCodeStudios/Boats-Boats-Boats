using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;
namespace PirateWars
{
    #region CannonBall
    /// <summary>
    /// Main projectile classs 
    /// </summary>
    public class CannonBall : Object
    {
        //CannonBalls are the one exception to objectsWithHealth.  They do not have health, but do have damage
        #region Private Varaibles
        /// <value>stores the value of damage that a cannon ball does when hitting another ship </value>
        private float damage;
        /// <value>is a 2D vector that determines the direction that the cannon ball moves after firing.  This direction is always perpindicular to the direction of the ship it is fired from </value>
        private Vector2 direction;
        #endregion

        /*public member functions*/
        #region Constructors
        /// <summary>
        /// Default constructor for a cannon ball.
        /// </summary>
        ///<params> 
        ///None 
        ///</params>
        public CannonBall()
            : base()
        {
            speed = new Vector2(3.5f, 3.5f);
            image = "CannonBall";
            damage = 10;
            direction = Vector2.Zero;
        }//end default constructor


        /// <summary>
        /// Overloaded constructor for a cannon ball
        /// </summary>
        /// <param name="v">2D vector for the speed of the cannon ball</param>
        /// <param name="dir">2D vector for the direction of the cannon ball.  Should be perpendicular to the direction of the ship it was fired from </param>
        /// <param name="t">Texture that is to be drawn.  Also used to create a bounding rectangle.  Passed to cannon ball class upon intitilization in the Ship.Fire() method</param>
        /// <param name="da">Damage that the cannon ball should deal.  It is passed from the Ship class</param>
        /// <param name="s">Vector representing the speed of the cannon ball</param>
        public CannonBall(Vector2 v, Vector2 dir, Texture2D t, float da, Vector2 s)
            : this()
        {
            //not enough comments # nerd
            position = v;
            direction = dir;
            texture = t;
            damage = da;
            speed = s;
        }
        /// <summary>
        /// Construct new cannon ball.
        /// </summary>
        /// <param name="p">Position of the projectile</param>
        /// <param name="dir">Direction of movement</param>
        /// <param name="t">Texture representing projectile</param>
        /// <param name="da">Damage that projectile deals upon impact</param>
        /// <param name="s">Speed for projectile</param>
        /// <param name="a">Angle that projectile is oriented upon spawn</param>
        public CannonBall(Vector2 p, Vector2 dir, Texture2D t, float da, Vector2 s, float a)
            : this()
        {
            position = p;
            direction = dir;
            texture = t;
            damage = da;
            speed = s;
            angle = a;
            origin = new Vector2(texture.Width/2, texture.Height/2);
            Bounding = new RectangleF(position, texture);
        }
        #endregion

        #region Accessors
        /// <summary>
        /// get the damage this cannon ball does
        /// </summary>
        /// <returns><see cref="damage"/></returns>

        public float getDamage()
        {
            return damage;
        }

        /// <summary>
        /// access the direction that the cannon ball is going
        /// </summary>
        /// <returns>direction</returns>
        public Vector2 getDirection()
        {
            return direction;
        }

        #endregion

        #region Mutators
        
        ///
        /// <summary>
        /// set the damage that the cannon ball does        
        /// </summary>
        ///<param name="newD"> the new value for the damage</param> 

        public void setDamage(float newD)
        {
            damage = newD;
        }

        /// <summary>
        /// change the direction of the cannonball
        /// </summary>
        /// <param name="newD">new direction</param>
        public void setDirection(Vector2 newD)
        {
               direction = newD;
        }
        #endregion
    }
    #endregion
}


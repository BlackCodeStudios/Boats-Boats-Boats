using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;

namespace PirateWars
{

    #region Player
    /// <summary>
    /// Main ship class.  All other ship types are derived from this type.
    /// </summary>
    public class Player_Brig : Player
    {

        /// <summary>
        /// Default Constructor.  Hard coded values for every protected variable in Ship
        /// </summary>
        public Player_Brig()
            : base()
        {
            /*
             * position, angle, origin and CBA have all been initialized by the Ship abstract class constructor
             * There is no need to reinitialize them here
             */
            image = "Brig2_1";
            speed = new Vector2(5.0f, 5.0f);
            cannons = 3;
            rateOfFire = 300;
            health = 100;
            turnSpeed = MathHelper.ToRadians(4.0f);
            damage = 5;
            maxHealth = health;
            shipState = ShipState.AbilityRecharging;
        }
        /// <summary>
        /// Overloaded constructor for ship
        /// </summary>
        /// <param name="tex">texture for the ship</param>
        /// <param name="texCB">texture for the cannon balls</param>
        public Player_Brig(Texture2D tex, Texture2D texCB)
            : this()
        {
            texture = tex;
            CannonBallTexture = tex;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Console.WriteLine("MAXHEALTH: " + maxHealth);
        }//end constructor
        /// <summary>
        /// Construct new Player_Brig from PlayerData strucutre, texture to represent this ship, and a texture to represent the ship's projectile.  Is exactly the same as the constructor from Player so it only calls the base constructor
        /// </summary>
        /// <param name="d">PlayerData structure which is loaded from an XML file through the ContentPipeline of the main game</param>
        /// <param name="tex">Texture to represent the player</param>
        /// <param name="cBTex">Texture to represent the player's projectiles</param>
        public Player_Brig(PlayerData d, Texture2D tex, Texture2D cBTex) 
            : base(d,tex,cBTex){}

        /// <summary>
        /// Override Fire method from Ship.  All this does is prevent the brig type from firing while its ability is activated
        /// </summary>
        public override void Fire()
        {
            if (shipState != ShipState.AbilityActivated)
                base.Fire();
        }
        /// <summary>
        /// Activate the ship's ability.  Calls the base function to perform basic operations and then modifies values specifically for this type
        /// </summary>
        /// <param name="gameTime">provides a snapshot of timing values</param>
        /// <returns>returns true if the ability is succesfully activated, false if otherwise</returns>
        public override bool ActivateAbility(TimeSpan gameTime)
        {
            //if the ability has not recharged then do not activate it
            //call base function to change ship state, ability activate time, and damage reistance
            if (base.ActivateAbility(gameTime) == false)
                return false;
            damage *= 5;
            speed = new Vector2(6.5f, 6.5f);
            return true;
        }
        /// <summary>
        /// Deactivate the ship's ability.  Calls the base function to perform basic operations and then un-does all actions done by ActivateAbility()
        /// </summary>
        /// <param name="gameTime">provides snapshot of timing values</param>
        /// <returns></returns>
        public override bool DeactivateAbility(TimeSpan gameTime)
        {
            if (base.DeactivateAbility(gameTime) == false)
                return false;
            //call the base function which resets damage resistance, ability recharge start time, and ship state
            base.DeactivateAbility(gameTime);
            damage /= 5;
            speed = new Vector2(5.0f, 5.0f);
            return true;
        }
    }//end class
    #endregion
}//end namespace WindowsGame4
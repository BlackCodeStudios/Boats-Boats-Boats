using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;

namespace PirateWars
{
    /// <summary>
    /// Base class for all player controlled ships.  Extends from ship but includes more data necessary for the use of unique Ship abilities.  The class is abstract so it cannot be constructed but constructors are developed here so that the sub-classes may inherit and utilize them.
    /// </summary>
    public abstract class Player : Ship
    {
        /// <summary>
        /// Keep track of ship's state.  Ships have 3 states, AbilityCharged, AbilityRecharging and AbilityActivated
        /// </summary>
        public enum ShipState
        {
            /// <summary>
            /// The player's ability is fully charged
            /// </summary>
            AbilityCharged,

            /// <summary>
            /// The players's ability is charging
            /// </summary>
            AbilityRecharging,
            
            /// <summary>
            /// The player's ability is activated
            /// </summary>
            AbilityActivated
        }

        /// <value>keep track of the ship's current state</value>
        protected ShipState shipState;
        /// <value>time at which the ability was activated</value>
        protected TimeSpan abilityActivateTime;
        ///<value>time at which the ability started to recharge.</value>
        protected TimeSpan abilityRechargeStartTime;
        /// <value>how long the ship's special ability should last in miliseconds</value>
        protected float ABILITY_DURATION;
        /// <value>how long before the ship is allowed to use its ability again</value>
        protected float ABILITY_RECHARGE;
        /// <value>The maximum amount of time before a ship can use its ability again</value>
        protected float ABILITY_RECHARGE_MAX;
        /// <summary>
        /// Default Constructor.  Sets values to default level
        /// </summary>
        public Player()
        {
            abilityActivateTime = abilityRechargeStartTime = TimeSpan.Zero;
            ABILITY_DURATION = 10000;       //10 seconds
            ABILITY_RECHARGE_MAX = 20000;   //20 seconds
            shipState = ShipState.AbilityRecharging;
        }
        /// <summary>
        /// Constructs new Player object from PlayerData structure, a texture to represent it, and a texture to represent its projectiles
        /// </summary>
        /// <param name="d">PlayerData structure which is loaded from an XML file through the ContentPipeline of the main game</param>
        /// <param name="tex">Texture to represent the player</param>
        /// <param name="cBTex">Texture to represent the player's projectiles</param>
        public Player(PlayerData d, Texture2D tex, Texture2D cBTex) 
            : base(d, tex, cBTex) 
        {
            abilityActivateTime = abilityRechargeStartTime = TimeSpan.Zero;
            ABILITY_DURATION = d.ABILITY_DURATION;
            ABILITY_RECHARGE_MAX = d.ABILITY_RECHARGE_MAX;
            shipState = ShipState.AbilityRecharging;
        }

        /// <summary>
        /// get the state of the ship
        /// </summary>
        /// <returns></returns>
        public ShipState getShipState()
        {
            return shipState;
        }
        /// <summary>
        /// get the time at which the user activated the ability
        /// </summary>
        /// <returns><see cref="abilityActivateTime"/></returns>
        public TimeSpan getAbilityActivateTime()
        {
            return abilityActivateTime;
        }
        /// <summary>
        /// get the duration for the ability in milliseconds
        /// </summary>
        /// <returns><see cref="ABILITY_DURATION"/></returns>
        public float getAbilityDuration()
        {
            return ABILITY_DURATION;
        }
        /// <summary>
        /// get the recharge time for the ability in milliseconds
        /// </summary>
        /// <returns><see cref="ABILITY_RECHARGE_MAX"/></returns>
        public float getAbilityRecharge()
        {
            return ABILITY_RECHARGE_MAX;
        }
        /// <summary>
        /// get the time at which the ability started recharging
        /// </summary>
        /// <returns><see cref="abilityRechargeStartTime"/></returns>
        public TimeSpan getAbilityRechargeStartTime()
        {
            return abilityRechargeStartTime;
        }
        /// <summary>
        /// Change the texture used to represent cannon balls.  Some boats have their cannon ball type change when their ability is activated.
        /// </summary>
        /// <param name="cBTex">New texture to represent projectiles</param>
        public void setCannonBallTexture(Texture2D cBTex)
        {
            CannonBallTexture = cBTex;
        }

        /// <summary>
        /// Update necessary player information.  Calls Ship.Update() (handles movement and projectile movement), but also updates ability times and changes player state accordingly.
        /// </summary>
        /// <param name="gameTime">provides a snapshot of timing values</param>
        public override void Update(TimeSpan gameTime)
        {
            //check if the ability has exceeded its duration
            if (shipState == ShipState.AbilityActivated && (gameTime.TotalMilliseconds - abilityActivateTime.TotalMilliseconds > ABILITY_DURATION))
            {
                DeactivateAbility(gameTime);
            }

            //check if the ability has recharged
            else if (shipState == ShipState.AbilityRecharging && ((gameTime.TotalMilliseconds - abilityRechargeStartTime.TotalMilliseconds) >= ABILITY_RECHARGE_MAX))
            {
                shipState = ShipState.AbilityCharged;
                Console.WriteLine("ABILITY CHARGED: " + gameTime + "; " + shipState + "; " + ABILITY_RECHARGE_MAX + "; " + (gameTime.TotalMilliseconds - abilityActivateTime.TotalMilliseconds));
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Activate the player's special ability.  The function is virtual because each ship will have a different power, but this function implements the commonality between all ships (changing ship state, making invicible, and setting abilityActivateTime)
        /// </summary>
        /// <param name="gameTime">TimeSpan with relevant time information to set abilityActivateTime</param>
        /// <returns></returns>
        public virtual bool ActivateAbility(TimeSpan gameTime)
        {
            //if the ability has not recharged then do not activate it
            if (shipState != ShipState.AbilityCharged)
                return false;
            shipState = ShipState.AbilityActivated;
            damageResistance = 1;
            abilityActivateTime = gameTime;
            return true;
            //Console.WriteLine("ABILITY ACTIVATED: " + abilityActivateTime);
        }
        /// <summary>
        /// Deactivate the player's special ability.  The function is virtual because each ship will deactivate differently, bu this function implements the commonality between all ships (changing ship state, making mortal, and setting abilityRechargeStartTime)
        /// </summary>
        /// <param name="gameTime">TimeSpan with relevant time information to set abilityRechargeStartTime</param>
        /// <returns></returns>
        public virtual bool DeactivateAbility(TimeSpan gameTime)
        {
            if (shipState != ShipState.AbilityActivated)
                return false;
            //Console.WriteLine("DEACTIVATING: " + (gameTime));
            shipState = ShipState.AbilityRecharging;
            abilityRechargeStartTime = gameTime;
            damageResistance = 0;
            return true;
        }
        /// <summary>
        /// Reset the player's data to initial conditions.  Only modifies values that may have changed during the game (state, all time values, and health)
        /// </summary>
        public void Reset()
        {
            DeactivateAbility(TimeSpan.Zero);
            abilityActivateTime = TimeSpan.Zero;
            health = maxHealth;
            shipState = ShipState.AbilityRecharging;
        }
    }
}

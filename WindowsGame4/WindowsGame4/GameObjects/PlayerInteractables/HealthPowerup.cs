using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;

namespace PirateWars
{
    /// <summary>
    /// Powerup to provide player with health in game
    /// </summary>
    public class HealthPowerup : Multiplier
    {
        /// <summary>
        /// Construct a new health powerup without using an ObjectData structure
        /// </summary>
        /// <param name="p">Vector representing position on screen</param>
        /// <param name="d">Direction that the powerup is moving in</param>
        /// <param name="a">Angle (in radians) that the powerup is rotated around it's center</param>
        /// <param name="tex">Texture to represent the powerup</param>
        /// <param name="spawn">Time at which the object was spawned</param>
        public HealthPowerup(Vector2 p, Vector2 d, float a, Texture2D tex, TimeSpan spawn)
            : base(p, d, a, tex, spawn)
        {
            speed = new Vector2(.3f, .3f);
            direction = new Vector2(0, 1);
            timeAllowedOnScreen = new TimeSpan(0, 0, 0, 0, 7000);
        }

        /// <summary>
        /// Construct a new health powerup.  Position, direction and angle are all dependent on the main game, but all values from the PlayerInteractableData structure are predetermined.
        /// </summary>
        /// <param name="d">ObjectData structure loaded from an XML file that is loaded from the ContentPipeline</param>
        /// <param name="p">Vector2 representing position</param>
        /// <param name="dir">Vector2 representing the direction the multiplier is moving in</param>
        /// <param name="a">Vector2 representing teh rotation of the multiplier about its origin</param>
        /// <param name="tex">Texture to represent that image</param>
        /// <param name="spawn">Time when the object was spawned</param>
        public HealthPowerup(PlayerInteractableData d, Vector2 p, Vector2 dir, float a, Texture2D tex, TimeSpan spawn) 
            : base(d, p, dir, a, tex,spawn) {/*same constructor as Multiplier*/}

        /// <summary>
        /// Activate the powerup's ability.  In this case, give health
        /// </summary>
        /// <param name="s">The ship receiving health</param>
        public override void ActivateAbility(Ship s)
        {
            s.giveHealth(s.MaxHealth / 10);  //give back 10% of health
        }
    }
}

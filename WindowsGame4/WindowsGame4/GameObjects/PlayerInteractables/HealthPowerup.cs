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
        public HealthPowerup(Vector2 p, Vector2 d, float a, Texture2D tex)
            : base(p, d, a, tex)
        {
            speed = new Vector2(.3f, .3f);
            direction = new Vector2(0, 1);
        }

        /// <summary>
        /// Construct a new health powerup.
        /// </summary>
        /// <param name="d">ObjectData structure loaded from an XML file through the ContentPipeline</param>
        /// <param name="tex">Texture to represent the powerup</param>
        public HealthPowerup(ObjectData d, Texture2D tex) 
            : base(d, tex) {/*same constructor as PlayerInteractable*/}

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

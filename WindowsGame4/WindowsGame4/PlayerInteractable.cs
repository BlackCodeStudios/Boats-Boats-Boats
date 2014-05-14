using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;

namespace PirateWars
{
    /// <summary>
    /// PlayerInteractables is the superclass for all objects that directly interact with the player such as powerups and mutlipliers.  They behave similarily in movement but do different things to the state of the game and the player.
    /// </summary>
    abstract class PlayerInteractable : Object
    {
        protected Vector2 direction;
        /// <summary>
        /// Default constructor for PlayerInteractable.  Uses default from Object and sets all variables unique to PlayerInteractable to 0
        /// </summary>
        public PlayerInteractable()
            : base()
        {
            direction = Vector2.Zero;
        }
        /// <summary>
        /// Construct new PlayerInteractable from an ObjectData structure and a texture to represent this object
        /// </summary>
        /// <param name="d">ObjectData Structure loaded from main game from XML file through ContentPipeline</param>
        /// <param name="tex">Texture to represent this object</param>
        public PlayerInteractable(ObjectData d, Texture2D tex)
            : base(d, tex)
        {
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
        }
        /// <summary>
        /// Construct new PlayerInteractable from 2 Vectors representing position and direction, the angle that the object should be oriented and a Texture to represent that object
        /// </summary>
        /// <param name="p">Vector representing position on screen</param>
        /// <param name="d">Vector representing the direction that the object moves</param>
        /// <param name="a">Angle (in radians) that the object is rotated (relative to its origin)</param>
        /// <param name="tex">Texture to represent that object</param>
        public PlayerInteractable(Vector2 p, Vector2 d, float a, Texture2D tex)
            : base(p, a, tex)
        {
            direction = d;
        }
        /// <summary>
        /// Handles changing position, direction and angle when applicable
        /// </summary>
        /// <param name="player">Player data so that the object can respond to player events accordingly</param>
        public virtual void Update(Ship player)
        {
            float distanceFromPlayer = Vector2.Distance(this.position, player.getPosition());

            if (distanceFromPlayer <= (player.getTexture().Width*1.5))
            {
                this.angle = TurnToFace(position, player.getPosition(), angle, 10);
                WrapAngle(this.angle);
                this.speed *= 2;
            }
            Vector2 heading = new Vector2((float)Math.Cos(this.angle), (float)Math.Sin(this.angle));
            this.position += heading * this.speed;
        }//end update

        public abstract void ActivateAbility(Ship s);
    }
}

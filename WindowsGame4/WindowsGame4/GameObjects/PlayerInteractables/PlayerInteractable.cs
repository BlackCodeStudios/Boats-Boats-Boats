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
    public abstract class PlayerInteractable : Object
    {
        /// <value>Direction that the PlayerInteractable moves along</value>
        protected Vector2 direction;
        
        ///<value>Controlls how long a PlayerInteractable is allowed on the screen (in milliseconds) before it is removed</value>
        protected TimeSpan timeAllowedOnScreen;

        ///<value>The time at whih the PlayerInteractable was spawned</value>
        protected TimeSpan spawnTime;

        /// <value>control whether or not a PlayerInteractable should be displayed or not</value>
        protected bool faded = false;

        /// <value>Once the object has been on the screen for too long, toggle it to be removed</value>
        protected bool timedOut = false;
        
        ///<value>Control the delay between each fade increment</value>
        protected TimeSpan fadeDelay = new TimeSpan(0, 0, 0, 0, 100);

        ///<value>Keep track of the last time the object faded</value>
        protected TimeSpan lastFade = TimeSpan.Zero;

        ///<value>Time at which the object starts blinking to indicate it is about to disappear</value>
        protected TimeSpan startBlink;

        /// <summary>
        /// Default constructor for PlayerInteractable.  Uses default from Object and sets all variables unique to PlayerInteractable to 0
        /// </summary>
        public PlayerInteractable()
            : base()
        {
            //default direction is 0
            direction = Vector2.Zero;
            //allow interactable to be on screen for 5000 ms                                          
            timeAllowedOnScreen = new TimeSpan(0, 0, 0, 0, 5000);
            //set default spawn time to 0             
            spawnTime = TimeSpan.Zero;
            //start blinking when the TimeAllowedOnScreen reaches 55%
            startBlink = new TimeSpan(0,0,0,0,(int)(timeAllowedOnScreen.TotalMilliseconds*0.55f));    
        }

        /// <summary>
        /// Construct new PlayerInteractable from a PlayerInteractableData structure, a texture to represent this object, and the time at which it spawned.
        /// </summary>
        /// <param name="d">PlayerInteractableData strucutre loaded from an XML file through the ContentPipeline</param>
        /// <param name="tex">Textyre to represent the image</param>
        /// <param name="spawn">Time when the PlayerInteractable spawned</param>
        public PlayerInteractable(PlayerInteractableData d, Texture2D tex, TimeSpan spawn)
            :base(d,tex)
        {
            spawnTime = spawn;
            timeAllowedOnScreen = new TimeSpan(0, 0, 0, 0, d.timeAllowedOnScreen);
            startBlink = new TimeSpan(0, 0, 0, 0, (int)(timeAllowedOnScreen.TotalMilliseconds * 0.55f)); 
        }
        
        /// <summary>
        /// Construct new PlayerInteractable from 2 Vectors representing position and c_direction, the angle that the object should be oriented and a Texture to represent that object
        /// </summary>
        /// <param name="p">Vector representing position on screen</param>
        /// <param name="d">Vector representing the c_direction that the object moves</param>
        /// <param name="a">Angle (in radians) that the object is rotated (relative to its origin)</param>
        /// <param name="tex">Texture to represent that object</param>
        /// <param name="spawn">Time when the object was spawned</param>
        public PlayerInteractable(Vector2 p, Vector2 d, float a, Texture2D tex, TimeSpan spawn)
            : base(p, a, tex)
        {
            direction = d;
            spawnTime = spawn;
            timeAllowedOnScreen = new TimeSpan(0, 0, 0, 0, 5000);
            startBlink = new TimeSpan(0, 0, 0, 0, (int)(timeAllowedOnScreen.TotalMilliseconds * .55f));
        }
        
        /// <summary>
        /// Handles changing position, c_direction and angle when applicable
        /// </summary>
        /// <param name="player">Player data so that the object can respond to player events accordingly</param>
        /// <param name="gameTime">Snapshot of timing values from game</param>
        public virtual void Update(Ship player, TimeSpan gameTime)
        {
            float distanceFromPlayer = Vector2.Distance(this.position, player.Position);

            //Update movement
            if (distanceFromPlayer <= player.Texture.Width/2 + this.Texture.Width/2 + 15)
            {
                this.angle = TurnToFace(position, player.Position, angle, 10);
                WrapAngle(this.angle);
                this.speed *= 2;
            }
            Vector2 heading = new Vector2((float)Math.Cos(this.angle), (float)Math.Sin(this.angle));
            this.position += heading * this.speed;

            //Update fade
            //object should start to flash 2 seconds before it is removed (5-3 = 2)
            if (gameTime - spawnTime >= startBlink)
            {
                if (gameTime - lastFade >= fadeDelay)
                {
                    if (faded == true)
                        faded = false;
                    else
                        faded = true;
                    lastFade = gameTime;
                }
            }
            if (gameTime - spawnTime >= timeAllowedOnScreen)
                timedOut = true;
        }//end update

        /// <summary>
        /// Activate the PlayerInteractable's ability (if it has one)
        /// </summary>
        /// <param name="s">The ship that will receive the benefit from the PlayerInteractable</param>
        public abstract void ActivateAbility(Ship s);

        /// <summary>
        /// Access whether or not a PlayerInteractable has spent too much time on screen
        /// </summary>
        public bool TimedOut
        {
            get
            {
                return timedOut;
            }
        }

        /// <summary>
        /// Access whether or not an object is faded out or not
        /// </summary>
        public bool Faded
        {
            get
            {
                return faded;
            }
        }
    }
}

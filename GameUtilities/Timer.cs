using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace GameUtilities
{
    /// <summary>
    /// Keeps track of all timing in game
    /// </summary>
    public class Timer
    {
        TimeSpan time;
        enum TimerState
        {
            Active,
            Paused
        }
        TimerState state;
        /// <summary>
        /// Starts the timer at 0.  Initially the timer is set to Pause, and needs to be activated through the Start function
        /// </summary>
        public Timer()
        {
            time = TimeSpan.Zero;
            state = TimerState.Paused;
        }
        /// <summary>
        /// Increment the timer by the amount of time that has elapsed since the last update.  Only updates if the timer is set to active
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values from main game</param>
        public void Update(GameTime gameTime) 
        {
            //increase the value of time only if the timer is set to Active
            if(state == TimerState.Active)
                time += gameTime.ElapsedGameTime;
        }
        /// <summary>
        /// Start the timer
        /// </summary>
        public void Start()
        {
            state = TimerState.Active;
        }
        /// <summary>
        /// Pause the timer.  This will prevent Update() from incrementing the time
        /// </summary>
        public void Pause()
        {
            state = TimerState.Paused;
        }
        /// <summary>
        /// Reset the timer back to 0
        /// </summary>
        public void Reset()
        {
            time = TimeSpan.Zero;
            state = TimerState.Paused;
        }
        /// <summary>
        /// Access time
        /// </summary>
        public TimeSpan RawTime
        {
            get
            {
                return time;
            }
        }
        /// <summary>
        /// Access display friendly time
        /// </summary>
        public string DisplayTime
        {
            get
            {
                string s = time.ToString(@"mm\:ss\.ff");
                return s;
            }
        }

    }
}

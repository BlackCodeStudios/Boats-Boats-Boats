using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectDataTypes
{
    public class ShipData : ObjectData
    {
        //has all of the variables of ObjectData and...

        /// <value>number of cannons per side on the ship (total/2)</value>
        public int cannons;
        /// <value>double indicating what the delay between cannon shots is in miliseconds</value>
        public double rateOfFire;
        /// <value>the speed at which this ships cannon balls should move</value>
        public Vector2 cannonBallVelocity;
        /// <value>health of the ship</value>
        public float health;
        ///<value>damage that the cannonballs should deal</value>
        public float damage;
        /// <value>reduces incoming damage.  Values range from 0 to 1, where 0 means no damage resitance and 1 means invincibility</value>
        public float damageResistance;
        /// <summary>
        /// Create a string to represent all player data so that it can be displayed in an easy to read format.
        /// </summary>
        /// <returns>returns a string with all ship data</returns>
        public string PrintData()
        {
            string output = base.PrintData();
            output += ("Cannons per side: " + cannons + "\n");
            output += ("Rate of Fire: " + rateOfFire + "\n");
            output += ("Health: " + health + "\n");
            output += ("Damage: " + damage + "\n");
            return output;
        }
    }
}

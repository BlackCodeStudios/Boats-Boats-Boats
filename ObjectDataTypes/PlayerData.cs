using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectDataTypes
{
    /// <summary>
    /// Holds all data needed to contstruct a new Player in PirateWars.  This class is designed to be serialized and deserialized from an XML document.
    /// </summary>
    public class PlayerData : ShipData
    {
        //all of the variables of ShipData plus...
        /// <value>how long the ship's special ability should last in miliseconds</value>
        public float ABILITY_DURATION;
        /// <value>how long before the ship is allowed to use its ability again</value>
        public float ABILITY_RECHARGE;
        /// <value>The maximum amount of time before a ship can use its ability again</value>
        public float ABILITY_RECHARGE_MAX;
    }
}

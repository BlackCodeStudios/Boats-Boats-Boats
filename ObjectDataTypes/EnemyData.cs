using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectDataTypes
{
    /// <summary>
    /// Holds all data needed to contstruct a new Enemy in PirateWars.  This class is designed to be serialized and deserialized from an XML document.
    /// </summary>
    public class EnemyData : ShipData
    {
        //all of the variables of ship data plus...
        public float range;
        public int score;
    }
}

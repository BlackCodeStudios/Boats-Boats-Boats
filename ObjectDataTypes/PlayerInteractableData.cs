using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectDataTypes
{
    public class PlayerInteractableData : ObjectData
    {
        //all of the data from Object plus...
        ///<value>Controlls how long a PlayerInteractable is allowed on the screen (in milliseconds) before it is removed. NOTE: This variable is a TimeSpan in the PlayerInteractable class in the PirateWars namespace.  TimeSpan has problems when being serialized.  Store the float and account for that when creating the TimeSpan from it in the PlayerIneractable class</value>
        public int timeAllowedOnScreen;
    }
}

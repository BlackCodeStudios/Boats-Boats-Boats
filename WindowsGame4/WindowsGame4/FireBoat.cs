using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;


namespace PirateWars
{
    #region FireBoat
    class FireBoat : Enemy
    {
        public FireBoat()
            : base()
        {
            range = 0;
            image = "FireBoat";
            damage = 5;
            speed = new Vector2(4.0f, 4.0f);
            cannons = 0;
            turnSpeed = 10 / (float)Math.PI;
            score = 5;
            health = 10;
        }//end default constructor

        public FireBoat(EnemyData d, Texture2D tex)
            :base(d, tex, null){ }
    }//end class FireBoat
    #endregion
}

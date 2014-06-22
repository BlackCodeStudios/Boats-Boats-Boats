using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;
using GameUtilities;

namespace PirateWars
{
    class Player_Frigate : Player
    {
        PlayerData dataP = new PlayerData();
        /*Player_Frigate inherits from Player_Brig.  Player_Brig is the default choice for players*/
        /*The Frigate is larger than the Brig, has more cannons, more health, but is slower, and has a smaller turn speed*/
        public Player_Frigate ()
        {
            image = "Frigate";
            speed = new Vector2(4.0f, 4.0f);
            cannons = 4;
            rateOfFire = 450;
            health = 125;
            turnSpeed = MathHelper.ToRadians(2);
            damage = 15;
            maxHealth = health;
        }
        public Player_Frigate(Texture2D tex, Texture2D texCB)
            : this()
        {
            texture = tex;
            CannonBallTexture = texCB;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }//end constructor
        
        public Player_Frigate(PlayerData d, Texture2D tex, Texture2D cBTex) 
            : base(d,tex,cBTex)
        {
           
        }
        /// <summary>
        /// The frigate fires like a normal ship except when its ability is activated.  Then it fires off all four sides.
        /// </summary>
        public override void Fire(TimeSpan gameTime)
        {            
            //if the ability is activated shoot in front and behind the ship
            if (shipState == ShipState.AbilityActivated)
            {
                if (CanFire(gameTime) == true)
                {
                    Console.WriteLine("CBA SIZE: " + CBA.Count);
                    CannonBall cannonBallTop = new CannonBall(this.Position, this.Direction, CannonBallTexture, damage, cannonBallVelocity, this.Angle);
                    CannonBall cannonBallBotton = new CannonBall(this.Position, -this.Direction, CannonBallTexture, damage, cannonBallVelocity, -this.Angle);
                    CBA.Add(cannonBallTop);
                    CBA.Add(cannonBallBotton);
                    Console.WriteLine("CBA SIZE: " + CBA.Count);
                }
            }
            //fire off two sides
            base.Fire(gameTime);
        }

        public override bool ActivateAbility(TimeSpan gameTime)
        {
            if (base.ActivateAbility(gameTime) == false)
                return false;
            damage += 5;        //increase damage by 5
            speed /= 2;         //divide speed by 2
            rateOfFire -= 100;  //increase rate of fire
            damageResistance = 1;
            return true;
        }

        public override bool DeactivateAbility(TimeSpan gameTime)
        {
            if (base.DeactivateAbility(gameTime) == false)
                return false;
            //reset stats
            damage -= 5;
            speed *= 2;
            rateOfFire += 100;
            damageResistance = 0;
            return true;
        }
    }
}

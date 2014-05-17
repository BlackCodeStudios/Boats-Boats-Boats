using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjectDataTypes;

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
        public override void Fire()
        {
            //fire off two sides
            base.Fire();
            
            //if the ability is activated shoot in front and behind the ship
            if (shipState == ShipState.AbilityActivated)
            {
                Vector2 increment = RectangleF.RotateVector(this.Direction,(float)Math.PI) * (texture.Height / cannons);
                for (int i = 0; i < cannons; i++)
                {
                    /*
                     * One set will move in the same direction as the ship, the other in the opposite direction.
                     * Same algorithm as in base.Fire() but the direction is just the direction of the player
                     */
                    Vector2 posT = this.position + (((cannons - i) - (cannons / 2)) * increment);
                    Vector2 posB = this.position + (((cannons - i) - (cannons / 2)) * increment);

                    posT += this.Direction * cannonBallVelocity;     //off left side
                    posB += -this.Direction * cannonBallVelocity;    //off right side
                    
                    //add Cannon Balls to List of cannon balls
                    CBA.Add(new CannonBall(posT, this.Direction, this.CannonBallTexture, this.damage, cannonBallVelocity, Object.WrapAngle(this.angle)));      //add left side cannon ball
                    CBA.Add(new CannonBall(posB, -this.Direction, this.CannonBallTexture, this.damage, cannonBallVelocity, Object.WrapAngle(this.angle)));    //add right side cannon ball
                }
            }
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

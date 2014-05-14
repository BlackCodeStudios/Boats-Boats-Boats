using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml.Serialization;
using ObjectDataTypes;

namespace PirateWars
{
    /// <summary>
    /// Spawns the a Player_Brig and an Enemy_Brig in the center of the screen.  The enemy does nothing and takes no damage.  Basic game to check for collisions, and make sure that movement is working correctly.
    /// </summary>
    class TestGame : Microsoft.Xna.Framework.Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Mouse and Keyboard
        MouseState oldMouseState;
        KeyboardState oldKeyState;
        TimeSpan oldKeyPress;
        //Textures
        Texture2D playerBrigTexture;
        Texture2D enemyBrigTexture;
        Texture2D playerCBTexture;
        Texture2D enemyCBTexture;

        Player player;
        Enemy enemy;
        public TestGame()
        {
            /*set graphics properties*/
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;


            //Changes the settings that you just applied
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            spriteBatch = new SpriteBatch(GraphicsDevice);

            oldKeyState = Keyboard.GetState();
            oldMouseState = Mouse.GetState();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            graphics.PreferredBackBufferHeight = (graphics.GraphicsDevice.DisplayMode.Height - (graphics.GraphicsDevice.DisplayMode.Height / 9));
            graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width - (graphics.GraphicsDevice.DisplayMode.Width / 7);
            graphics.ApplyChanges();

            playerBrigTexture = Content.Load<Texture2D>("PlayerImages/Brig2_1");
            enemyBrigTexture = Content.Load<Texture2D>("EnemyImages/Brig2_1 - Enemy");
            enemyCBTexture = Content.Load<Texture2D>("CannonBall_Enemy");
            playerCBTexture = Content.Load<Texture2D>("CannonBall");

            player = new Player_Brig(Content.Load<PlayerData>("ObjectDataFiles/Player_Brig_TestData"), playerBrigTexture, playerCBTexture);
            player.setPosition(new Vector2(290, 537));
            enemy = new Enemy_Brig(Content.Load<EnemyData>("ObjectDataFiles/Enemy_Brig_TestData"), enemyBrigTexture, enemyCBTexture);
            enemy.setPosition(new Vector2(graphics.PreferredBackBufferWidth / 2 - enemyBrigTexture.Width / 2, graphics.PreferredBackBufferHeight / 2 - enemyBrigTexture.Height / 2));
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateKeyboardInput(gameTime);
            player.Update(gameTime.TotalGameTime);
            enemy.Update(gameTime.TotalGameTime);

            CollisionDetection(player, enemy, 0);
            
        }


        private void UpdateKeyboardInput(GameTime time)
        {
            KeyboardState newState = Keyboard.GetState();
            Vector2 newP = player.getPosition();
            float angle = player.getAngle();
            TimeSpan newKeyPress = time.TotalGameTime;

            /*
             * WAD are the movement keys, but only W and S will actually move the ship
             * W: move ship forward in the direction that the ship is pointing 
             * A: rotate the ship to the left (-radians)
             * D: rotate the ship to the right (+radians)
             */
            if (newState.IsKeyDown(Keys.Tab))
            {
                if (player.getShipState() == Player.ShipState.AbilityCharged)
                {
                }
            }
            if (newState.IsKeyDown(Keys.A) || newState.IsKeyDown(Keys.Left))
            {
                angle = player.getAngle() - player.getTurnSpeed();
                /*make sure that it does not move too far off the left screen*/
            }
            if (newState.IsKeyDown(Keys.D) || newState.IsKeyDown(Keys.Right))
            {
                angle = player.getAngle() + player.getTurnSpeed();
            }

            if (newState.IsKeyDown(Keys.W) || newState.IsKeyDown(Keys.Up))
            {
                Vector2 direction = new Vector2((float)(Math.Cos(player.getAngle())), (float)(Math.Sin(player.getAngle())));
                direction.Normalize();
                newP += direction * player.getSpeed();
            }//end move FORWARD
            //check to make sure it stays within bounds, and then update position and angle

            OutOfBounds(ref newP, player.getTexture(), 0, graphics.PreferredBackBufferWidth, 0, graphics.PreferredBackBufferHeight);
            player.setPosition(newP);
            player.setAngle(angle);

            // get the time between the last firing and this firing.  If the delay has been more than 200 miliseconds, fire again
            double delay = newKeyPress.TotalMilliseconds - oldKeyPress.TotalMilliseconds;
            if (newState.IsKeyDown(Keys.Space) && delay > player.getROF())
            {
                player.Fire();
                //reset the value of oldkeypress
                oldKeyPress = newKeyPress;
            }
            //Update keyPress state
            oldKeyState = newState;
        }
       
        private void OutOfBounds(ref Vector2 v, Texture2D t, float lowerXBound, float upperXBound, float lowerYBound, float upperYBound)
        {
            //out of bounds top
            if (v.Y - (t.Height / 2) < lowerYBound)
            {
                v.Y = lowerYBound + t.Height / 2;
            }
            //out of bounds bottom
            else if (v.Y + (t.Height / 2) > upperYBound)
            {
                v.Y = upperYBound - (t.Height / 2);
            }
            //out of bounds left
            else if (v.X - (t.Width / 2) < lowerXBound)
            {
                v.X = lowerXBound + t.Width / 2;
            }
            //out of bounds right
            else if (v.X + (t.Width / 2) > upperXBound)
            {
                v.X = upperXBound - (t.Width / 2);
            }
            else
            {
                return;
            }
        }

        private bool OutOfBounds(CannonBall c)
        {
            if (c.getPosition().X < 0 || c.getPosition().X > graphics.PreferredBackBufferWidth)
                return true;
            else if (c.getPosition().Y < 0 || c.getPosition().Y > graphics.PreferredBackBufferHeight)
                return true;
            else
                return false;
        }//outof bounds cannon



        private void CollisionDetection(Ship s, Ship e, int index)
        {
            //check cannon balls
            for (int j = s.getCBA().Count - 1; j >= 0; j--)
            {
                CannonBall cB = s.getCBA().ElementAt(j);
                if (cB.BoundingBox.Intersects(e.BoundingBox))
                {
                    Console.WriteLine("\nCOLLISION CB "+ j + ": \n" + cB.BoundingBox.Print() + "\nAGAINST: \n" + e.BoundingBox.Print()+"\n");
                    //deal damage to enemy
                    e.takeDamage(cB.getDamage());
                    //remove cannon ball

                    //if the ship is not a player frigate, remove the cannon ball upon contact
                    if (s.GetType() != typeof(Player_Frigate))
                        s.getCBA().Remove(cB);
                    //if it is a player frigate, check to see if it's ability is activated, if not, then remove the cannon ball upon contact.  Else, leave the cannon balls alone
                    else
                    {
                        if (((Player)(s)).getShipState() != Ship.ShipState.AbilityActivated)
                            s.getCBA().Remove(cB);
                    }
                }//end if collision
                //check if the ship should be sunk
            }//end for j

            //if s is the player, then check if it is a brig and has its ability activated
            if (s == player)
            {
                if (player.GetType() == typeof(Player_Brig) && player.getShipState() == Ship.ShipState.AbilityActivated)
                    if (s.BoundingBox.Intersects(e.BoundingBox))
                    {
                        e.takeDamage(s.getDamage());
                    }
            }

            //check if the ship should be sunk
            if (e.getHealth() <= 0)
            {
                if (e.GetType() == typeof(Player_Brig) || e.GetType() == typeof(Player_Frigate) || e.GetType() == typeof(Player_ManOfWar))
                {
                    UnloadContent();
                    EndRun();
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            //base.Draw(gameTime);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            GraphicsDevice.Clear(new Color(28, 107, 160));

            //draw player
            spriteBatch.Draw(enemyCBTexture, player.getPosition(), Color.White);
            spriteBatch.Draw(player.getTexture(), player.getPosition(), null, Color.White, player.getAngle(), player.getOrigin(), 1.0f, SpriteEffects.None, 0.0f);

            //draw Enemy
            spriteBatch.Draw(enemy.getTexture(), enemy.getPosition(), null, Color.White, enemy.getAngle(), enemy.getOrigin(), 1.0f, SpriteEffects.None, 0.0f);

            for (int i = player.getCBA().Count - 1; i >= 0; i--)
            {
                CannonBall c = player.getCBA().ElementAt(i);
                /*if the cannon ball has gone out of bounds, remove it and do not draw, else draw it*/
                if (OutOfBounds(player.getCBA().ElementAt(i)))
                {
                    player.getCBA().RemoveAt(i);
                }
                else
                {
                    spriteBatch.Draw(playerCBTexture, c.getPosition(), null, Color.White, c.getAngle(), c.getOrigin(), 1.0f, SpriteEffects.None, 0.0f);
                }
            }

            /*draw enemy cannon balls*/
            for (int i = enemy.getCBA().Count - 1; i >= 0; i--)
            {
                CannonBall c = enemy.getCBA().ElementAt(i);
                if (OutOfBounds(c))
                {
                    enemy.getCBA().RemoveAt(i);
                }
                else
                    spriteBatch.Draw(enemyCBTexture, c.getPosition(), null, Color.White, c.getAngle(), c.getOrigin(), 1.0f, SpriteEffects.None, 0.0f);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

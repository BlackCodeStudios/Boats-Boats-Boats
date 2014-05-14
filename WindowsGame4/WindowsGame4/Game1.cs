using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
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
    /// This is the main type for your game
    /// </summary>
    #region Game1
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Public Variables
        /// <value>the Ship that the player controls.  It is made static so that the Enemy class can access it for movement purposes</value>
        protected Player player;
        #endregion

        #region Private Variables


        /// <value>initial spawning location for the player</value>
        Vector2 playerStartingPos;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Timer gameTimer;
        #region Textures
        Texture2D playerCBTexture;
        Texture2D enemyCBTexture;
        Texture2D playerBrig;
        Texture2D playerFrigate;
        Texture2D playerManOfWar;
        Texture2D enemyBrig;
        Texture2D enemyFrigate;
        Texture2D enemyFireBoat;
        Texture2D enemyManOfWar;
        Texture2D healthPowerup;
        Texture2D multiplierTexture;
        Texture2D healthBar;
        Texture2D playerCBPowerUpTexture;
        Texture2D BossTexture;
        Texture2D BorderTexture;

        //TEST TEXTURES
        Texture2D LineX;
        Texture2D LineY;
        #endregion

        #region Music and Sounds
        //List<Song> BackgroundSongs;
        //Song mainMenuTheme;
        //Song gameMusic;
        List<Song> BackgroundSongs;
        SoundEffect mainMenuSong;
        SoundEffectInstance mainMenuSongInstance;
        SoundEffect gameSong;
        SoundEffectInstance gameSongInstance;
        SoundEffect gameOverSong;
        SoundEffectInstance gameOverSongInstance;
        #endregion

        #region MouseAndKeyboard
        /// <value>keep track of the last keyboard state</value>
        KeyboardState oldKeyState;
        /// <value>Time at which the keyboard was last updated.  Used to regulate key input so that firing happens on a timing interval even if the key is constantly held down</value>
        TimeSpan oldKeyPress = new TimeSpan(0);
        /// <value>keeps track of the time of last mouse click to place delay between mouse clicks</value>
        TimeSpan oldMouseEvent = new TimeSpan(0);
        /// <value>keep track of the last mouse state</value>
        MouseState oldMouseState;
        #endregion

        #region SpawningVariables
        ///<value>random number generator used for all randomizing aspects</value>
        Random randomGenerator = new Random();
        /// <value>lastSpawn keeps track of the time at which the last spawn occured and is later used to enforce spawning on regular time intervals</value>
        TimeSpan lastSpawn = new TimeSpan(0);
        ///<value>the lowest x-coordinate value that can be assigned when spawning a new enemy</value>
        const int LOWER_SPAWN_X = 30;
        /// <value>the lowest y-coordinate value that can be assigned when spawning a new enemy</value>
        const int LOWER_SPAWN_Y = 30;
        /// <value>The max time interval inbetween spawns in milliseconds</value>
        float SPAWN_TIME_MAX = 4000;
        /// <value>The min time interval in between spawns in milliseconds</value>
        float SPAWN_TIME_MIN = 1500;
        /// <value>the maximum number of enemies that can be on screen at a given point in time</value>
        int SPAWN_NUMBER_THRESHOLD = 10;
        float NextSpawn;
        #endregion

        #region Lists
        /// <value>EnemyList holds all enemies that have been spawned on the map</value>
        List<Enemy> EnemyList;
        /// <value>List that holds all playerInteractables (multipliers and powerups)</value>
        List<PlayerInteractable> playerInteractableList;
        /// <value>List to contain player friendly ships when the user is using the Man Of War and activates its ability</value>
        List<FriendlyShips> friendlyList;
        #endregion

        #region UI
        /// <value>the score that the player has earned during this game session</value>
        int score = 0;
        /// <value>multiplies the worth of each enemy.  Increases when multipliers are collected</value>
        int scoreMultiplier = 1;
        SpriteFont HUDFont;
        Button startButton, returnToMenu, resumeGame;
        Vector2 startButtonPos;
        Texture2D logo;
        SpriteFont logoFont;
        SpriteFont mottoFont;
        Button brigButton, frigateButton, manOfWarButton;
        #endregion //UI

        #region ObjectDataTypes
        PlayerData PLAYER_BRIG_DATA, PLAYER_FRIG_DATA, PLAYER_MOW_DATA;
        EnemyData FIREBOAT_DATA, ENEMY_BRIG_DATA, ENEMY_FRIG_DATA, ENEMY_MOW_DATA, FRIENDLY_SHIPS_DATA;
        EnemyData BOSS_DATA;
        #endregion

        Vector2 BOSS_POSITION;
        bool BOSS_SPAWN_SUCCESS = false;
        bool DISABLE_PLAYER_MOVEMENT = false;

        /// <summary>
        /// The game has several states, but it can only be in one state at a time.
        /// </summary>
        enum GameState
        {
            BootMenu,
            MainMenu,
            ShipSelection,
            Instructions,
            Pause,
            GameOver,
            Loading,
            GameOn
        }
        /// <value>keep track of this game's current state</value>
        GameState gameState;

        #endregion //private variables

        #region HighScore
        /// <summary>
        /// High Scores come with three pieces of information:
        ///     the game the score belongs to
        ///     the high score itself
        ///     and the date that the high score was created
        /// </summary>
        public struct HighScoreData
        {
            /// <summary>
            /// The game type that the score was achieved in
            /// </summary>
            public string GameType;
            /// <summary>
            /// the score that the player achieved
            /// </summary>
            public int HighScore;
            /// <summary>
            /// Date at which high score is achieved
            /// </summary>
            public DateTime date;
        }

        /// <summary>
        /// write to an XML file the high score data for this game type
        /// </summary>
        private void saveHighScoreData()
        {
            HighScoreData highScore = new HighScoreData();
            highScore.GameType = this.ToString();
            highScore.HighScore = score;
            highScore.date = DateTime.Now;
            //if the new score is larger than the current high score, reset the high score; else do nothing.
            if (score > readHighScoreData().HighScore)
            {
                XmlSerializer writer = new XmlSerializer(typeof(HighScoreData));
                StreamWriter file = new StreamWriter("HighScore.xml");

                writer.Serialize(file, highScore);
                file.Close();
            }
        }

        /// <summary>
        /// read from an XML file the high score data for this gametype
        /// </summary>
        /// <returns>The high score for this game type</returns>
        private HighScoreData readHighScoreData()
        {
            StreamReader file = new StreamReader("HighScore.xml");
            XmlSerializer reader = new XmlSerializer(typeof(HighScoreData));
            HighScoreData data = (HighScoreData)reader.Deserialize(file);
            file.Close();
            return data;
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Sets the basics for the graphics, and GUI for this game
        /// </summary>
        public Game1()
        {
            /*set graphics properties*/
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;


            //Changes the settings that you just applied
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            //set default screen to the main menu screen
            gameState = GameState.BootMenu;
            IsMouseVisible = true;

           
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            oldKeyState = Keyboard.GetState();
            oldMouseState = Mouse.GetState();

            gameTimer = new Timer();

            //try opening the high score file.  If it does not open, presume that it has not been created before
            try
            {
                StreamReader file = new StreamReader("HighScore.xml");
                file.Close();
            }
            catch
            {
                //create a new high score where the score is 0 and the date and time is the first time this game was opened
                HighScoreData highScore = new HighScoreData();
                highScore.HighScore = 0;
                highScore.date = DateTime.Now;
                XmlSerializer writer = new XmlSerializer(typeof(HighScoreData));
                StreamWriter file = new StreamWriter("HighScore.xml");
                writer.Serialize(file, highScore);
                file.Close();
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            graphics.PreferredBackBufferHeight = (graphics.GraphicsDevice.DisplayMode.Height - (graphics.GraphicsDevice.DisplayMode.Height / 9));
            graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width - (graphics.GraphicsDevice.DisplayMode.Width / 7);
            graphics.ApplyChanges();

            #region Textures
            playerBrig = Content.Load<Texture2D>("PlayerImages/Brig2_1");
            playerFrigate = Content.Load<Texture2D>("PlayerImages/Frigate");
            playerManOfWar = Content.Load<Texture2D>("PlayerImages/ManOfWar");
            enemyBrig = Content.Load<Texture2D>("EnemyImages/Brig2_1 - Enemy");
            enemyFireBoat = Content.Load<Texture2D>("EnemyImages/FireBoat");
            enemyFrigate = Content.Load<Texture2D>("EnemyImages/Frigate - Enemy");
            enemyManOfWar = Content.Load<Texture2D>("EnemyImages/ManOfWar - Enemy");
            healthPowerup = Content.Load<Texture2D>("HealthPowerup");
            BossTexture = Content.Load<Texture2D>("EnemyImages/Boss");
            #endregion

            BOSS_POSITION = new Vector2(BossTexture.Width / 2, graphics.PreferredBackBufferHeight / 2 - BossTexture.Height / 2);
            LineX = Content.Load<Texture2D>("LineX");
            LineY = Content.Load<Texture2D>("LineY");
            #region Main Menu
            logo = Content.Load<Texture2D>("BeardCoded2");
            logoFont = Content.Load<SpriteFont>("Fonts/LogoFont");
            mottoFont = Content.Load<SpriteFont>("Fonts/MottoFont");
            startButtonPos = new Vector2(300, 300);
            startButton = new Button(Content.Load<Texture2D>("StartButton"), startButtonPos);

            float x1 = (float)graphics.PreferredBackBufferWidth / 6 - playerBrig.Width / 2;
            float x2 = (x1 + ((float)graphics.PreferredBackBufferWidth / 3.0f)) - (playerFrigate.Width / 2);
            float x3 = (x2 + ((float)graphics.PreferredBackBufferWidth / 3.0f)) - (playerManOfWar.Width / 2);

            Vector2 ship1Pos = new Vector2(x1, (graphics.PreferredBackBufferHeight / 2) - playerBrig.Height / 2);
            Vector2 ship2Pos = new Vector2(x2, (graphics.PreferredBackBufferHeight / 2) - playerFrigate.Height / 2);
            Vector2 ship3Pos = new Vector2(x3, (graphics.PreferredBackBufferHeight / 2) - playerManOfWar.Height / 2);
            brigButton = new Button(playerBrig, ship1Pos);
            frigateButton = new Button(playerFrigate, ship2Pos);
            manOfWarButton = new Button(playerManOfWar, ship3Pos);
            returnToMenu = new Button(Content.Load<Texture2D>("ReturnToMenu"), new Vector2(250, 325));
            resumeGame = new Button(Content.Load<Texture2D>("StartButton"), new Vector2(250, 500));
            #endregion

            #region GameMaterial
            // Create a new SpriteBatch, which can be used to draw textures.
            // player = new Player_Brig(Content);
            //player = new Player_Frigate(Content);
            playerStartingPos = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            EnemyList = new List<Enemy>();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            playerInteractableList = new List<PlayerInteractable>();
            friendlyList = new List<FriendlyShips>();

            //load player ship textures
            playerCBTexture = Content.Load<Texture2D>("CannonBall");
            enemyCBTexture = Content.Load<Texture2D>("CannonBall_Enemy");
            playerCBPowerUpTexture = Content.Load<Texture2D>("CannonBall_PowerUp");
            //Load HUD
            HUDFont = Content.Load<SpriteFont>("Fonts/HUDFont");
            healthBar = Content.Load<Texture2D>("healthBar");
            multiplierTexture = Content.Load<Texture2D>("Multiplier");
            #endregion

            #region Time Variables
            #endregion

            //Load object data
            #region ObjectData
            PLAYER_BRIG_DATA = Content.Load<PlayerData>("ObjectDataFiles/Player_BrigData");
            PLAYER_FRIG_DATA = Content.Load<PlayerData>("ObjectDataFiles/Player_FrigateData");
            PLAYER_MOW_DATA = Content.Load<PlayerData>("ObjectDataFiles/Player_ManOfWarData");

            FIREBOAT_DATA = Content.Load<EnemyData>("ObjectDataFiles/FireboatData");
            ENEMY_BRIG_DATA = Content.Load<EnemyData>("ObjectDataFiles/Enemy_BrigData");
            ENEMY_FRIG_DATA = Content.Load<EnemyData>("ObjectDataFiles/Enemy_FrigateData");
            ENEMY_MOW_DATA = Content.Load<EnemyData>("ObjectDataFiles/Enemy_ManOfWarData");

            FRIENDLY_SHIPS_DATA = Content.Load<EnemyData>("ObjectDataFiles/FriendlyShipsData");

            BOSS_DATA = Content.Load<EnemyData>("ObjectDataFiles/BossData");
            #endregion

            #region Sounds
            //load songs from folder and put in list
            BackgroundSongs = new List<Song>();
            //main menu theme
            mainMenuSong = Content.Load<SoundEffect>("Sounds/PirateWarsMainMenu");
            mainMenuSongInstance = mainMenuSong.CreateInstance();
            mainMenuSongInstance.IsLooped = true;
            //game music
            gameSong = Content.Load<SoundEffect>("Sounds/GIOmetryWars");
            gameSongInstance = gameSong.CreateInstance();
            gameSongInstance.IsLooped = true;
            //game over
            gameOverSong = Content.Load<SoundEffect>("Sounds/GameOver");
            gameOverSongInstance = gameOverSong.CreateInstance();
            #endregion

            BorderTexture = Content.Load<Texture2D>("Border");
        }
        #endregion

        #region Unload
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
            base.UnloadContent();
        }
        #endregion

        #region Game Updating
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        #region Update
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (gameState == GameState.GameOver)
            {
                saveHighScoreData();
            }

            /*CHECK WHAT MUSIC SHOULD BE PLAYING*/
            //main menu music
            if (gameState == GameState.MainMenu)
            {
                if (gameOverSongInstance.State == SoundState.Playing)
                    gameOverSongInstance.Stop();
                if (mainMenuSongInstance.State == SoundState.Stopped)
                    mainMenuSongInstance.Play();
            }
            //game music
            else if (gameState == GameState.GameOn)
            {
                if (mainMenuSongInstance.State == SoundState.Playing)
                    mainMenuSongInstance.Stop();
                if (gameSongInstance.State == SoundState.Stopped)
                    gameSongInstance.Play();

            }
            //game over screen music
            else if (gameState == GameState.GameOver)
            {
                if (gameSongInstance.State == SoundState.Playing)
                    gameSongInstance.Stop();
                if (gameOverSongInstance.State == SoundState.Stopped)
                    gameSongInstance.Play();
            }
            //if the game is the BootMenu, MainMenu, Ship Selection or PauseMenu, then the mouse input needs to be updated
            if (gameState == GameState.BootMenu || gameState == GameState.MainMenu || gameState == GameState.ShipSelection || gameState == GameState.Pause || gameState == GameState.GameOver)
            {
                IsMouseVisible = true;
                UpdateMouseInput(gameTime);
            }
            //if game state is GAME ON
            else if (gameState == GameState.GameOn)
            {
                IsMouseVisible = false;
                /*COLLISION DETECTIONS*/
                //player against enemy
                for (int i = EnemyList.Count - 1; i >= 0; i--)
                {
                    CollisionDetection(player, EnemyList.ElementAt(i), i);
                }

                //player against interactables
                for (int i = playerInteractableList.Count - 1; i >= 0; i--)
                {
                    CollisionDetection(player, playerInteractableList.ElementAt(i), i);
                }
                //enemies against player
                for (int i = EnemyList.Count - 1; i >= 0; i--)
                {
                    CollisionDetection(EnemyList.ElementAt(i), player, i);
                }
                //friendlies against enemies
                for (int i = friendlyList.Count - 1; i >= 0; i--)
                {
                    for (int j = EnemyList.Count - 1; j >= 0; j--)
                        CollisionDetection(friendlyList.ElementAt(i), EnemyList.ElementAt(j), j);
                }

                UpdateKeyboardInput(gameTime);

                player.Update(gameTimer.RawTime);

                //update object positions
                foreach (Enemy e in EnemyList)
                {
                    e.UpdateAndMove(gameTime, player);
                }
                foreach (PlayerInteractable p in playerInteractableList)
                {
                    p.Update(player);
                }
                /*check the friendlyList;  if the list has been initialized, and the ship's ability is not activated, then the list should be cleared.  Only applies to Player_ManOfWar.  Otherwise, nothing will happen here*/
                if (friendlyList.Count >= 0 && (player.getShipState() != Ship.ShipState.AbilityActivated))
                    friendlyList.Clear();
                else
                {
                    if (EnemyList.Count != 0 && friendlyList.Count != 0)
                    {
                        for (int i = 0; i < friendlyList.Count(); i++)
                        {
                            //The targets are the first four elements of enemyList.  If there is less than four, then the last one will be attacked by multiple ships
                            int target = i;
                            if (i >= EnemyList.Count - 1)
                                target = EnemyList.Count - 1;
                            friendlyList.ElementAt(i).UpdateAndMove(gameTime, EnemyList.ElementAt(target));
                        }
                    }
                }
                Draw(gameTime);
                Spawn(gameTime);
                gameTimer.Update(gameTime);
                base.Update(gameTime);
            }
        }
        #endregion

        #region Input
        #region KeyboardInput
        /// <summary>
        /// Checks for keyboard input, and interprets that keyboard input.
        /// W: move forward
        /// A: turn left
        /// D: turn right
        /// SPACE: fire
        /// </summary>
        /// <param name="time">GameTime used to limit the rate of fire of the player</param>

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
            if (newState.IsKeyDown(Keys.Escape))
            {
                gameState = GameState.Pause;
                gameTimer.Pause();
            }
            if (newState.IsKeyDown(Keys.Tab))
            {
                KeyboardState doubleCheck = Keyboard.GetState();
                if (player.getShipState() == Ship.ShipState.AbilityCharged)
                {
                    player.ActivateAbility(gameTimer.RawTime);
                    if (player.GetType() == typeof(Player_ManOfWar))
                    {
                        //brute force math where the 8 ships should spawn
                        GenerateFriendlies();
                    }
                }
            }
            if (DISABLE_PLAYER_MOVEMENT == true)
            {
                Vector2 direction = new Vector2((float)(Math.Cos(player.getAngle())), (float)(Math.Sin(player.getAngle())));
                direction.Normalize();
                newP -= direction * player.getSpeed();
                if (player.getPosition().X >= BOSS_POSITION.X + 30)
                    DISABLE_PLAYER_MOVEMENT = false;
            }
            else
            {
                if (newState.IsKeyDown(Keys.A) || newState.IsKeyDown(Keys.Left))
                {
                    angle = player.getAngle() - player.getTurnSpeed();
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
            }
            //check to make sure it stays within bounds, and then update position and angle
            if (BOSS_SPAWN_SUCCESS)
            {
                OutOfBounds(ref newP, player.getTexture(), BOSS_POSITION.X, graphics.PreferredBackBufferWidth, 0, graphics.PreferredBackBufferHeight - 50);
            }
            else
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
        }//end UpdateInput (Keyboard
        #endregion //keyboard

        #region MouseInput
        private void UpdateMouseInput(GameTime gameTime)
        {
            MouseState newState = Mouse.GetState();
            //check if the mouse is clicked, and if it is, see what button it pressed
            if (gameState == GameState.BootMenu)
            {
                if (newState.LeftButton == ButtonState.Pressed)
                {
                    gameState = GameState.MainMenu;
                    oldMouseEvent = gameTime.TotalGameTime;
                }
            }
            if (gameState == GameState.MainMenu)
            {
                //set delay of 300 milliseconds before allowing another mouse click event.  Prevents spaming and menu problems
                if (gameTime.TotalGameTime.TotalMilliseconds - oldMouseEvent.TotalMilliseconds < 200)
                {
                    return;
                }
                //if it clicked the start Button
                if (startButton.buttonClicked(newState))
                {
                    gameState = GameState.ShipSelection;
                    oldMouseState = newState;
                    return;
                }
            }//end MainMenu
            else if (gameState == GameState.ShipSelection)
            {
                //check for Brig Selection
                if (brigButton.buttonClicked(newState))
                {
                    player = new Player_Brig(PLAYER_BRIG_DATA, playerBrig, playerCBTexture);
                    StartGame(gameTime);

                }
                else if (frigateButton.buttonClicked(newState))
                {
                    player = new Player_Frigate(PLAYER_FRIG_DATA, playerFrigate, playerCBTexture);
                    StartGame(gameTime);
                }
                else if (manOfWarButton.buttonClicked(newState))
                {
                    player = new Player_ManOfWar(PLAYER_MOW_DATA, playerManOfWar, playerCBTexture);
                    StartGame(gameTime);
                }
            }//end shipSelection
            else if (gameState == GameState.Pause)
            {
                if (resumeGame.buttonClicked(newState))
                {
                    gameState = GameState.GameOn;
                    gameTimer.Start();
                }
                if (returnToMenu.buttonClicked(newState))
                {
                    gameState = GameState.MainMenu;
                }
            }
            else if (gameState == GameState.GameOver)
            {
                if (returnToMenu.buttonClicked(newState))
                    gameState = GameState.MainMenu;
            }
            oldMouseState = newState;
        }


        private void GenerateFriendlies()
        {
            FriendlyShips e = new FriendlyShips(ENEMY_FRIG_DATA, playerFrigate, playerCBTexture);
            float BUFFER = 25;
            friendlyList = new List<FriendlyShips>();
            //ship 1 (12:00)
            float x = player.getPosition().X;
            float y = player.getPosition().Y - (2 * player.getOrigin().Y) - (2 * e.getOrigin().Y) - BUFFER;
            Vector2 ePos = new Vector2(x, y);
            e.setPosition(ePos);
            friendlyList.Add(e);

            //ship 2 (9 position)
            x = player.getPosition().X - player.getOrigin().X - e.getOrigin().X - BUFFER;
            y = player.getPosition().Y - player.getOrigin().Y;
            FriendlyShips e1 = new FriendlyShips(ENEMY_FRIG_DATA, playerFrigate, playerCBTexture);
            ePos = new Vector2(x, y);
            e1.setPosition(ePos);
            friendlyList.Add(e1);

            //ship 3 (6:00)
            x = player.getPosition().X;
            y = player.getPosition().Y + (2 * player.getOrigin().Y) + (2 * e.getOrigin().Y) + BUFFER;
            ePos = new Vector2(x, y);
            FriendlyShips e2 = new FriendlyShips(ENEMY_FRIG_DATA, playerFrigate, playerCBTexture);
            e2.setPosition(ePos);
            friendlyList.Add(e2);

            //ship 4 (3:00)
            x = player.getPosition().X + player.getOrigin().X + e.getOrigin().X + BUFFER;
            y = player.getPosition().Y - player.getOrigin().Y;
            FriendlyShips e3 = new FriendlyShips(ENEMY_FRIG_DATA, playerFrigate, playerCBTexture);
            ePos = new Vector2(x, y);
            e3.setPosition(ePos);
            friendlyList.Add(e3);
        }


        private void StartGame(GameTime gameTime)
        {
            gameState = GameState.Loading;
            //ready the player
            player.setPosition(playerStartingPos);
            player.Reset();
            //set game constants
            score = 0;
            scoreMultiplier = 1;
            lastSpawn = TimeSpan.Zero;
            //make sure all lists are empty
            EnemyList.Clear();
            player.getCBA().Clear();
            playerInteractableList.Clear();
            //set gameState to GameOn
            gameState = GameState.GameOn;
            BOSS_SPAWN_SUCCESS = false;
            BOSS_READY_TO_SPAWN = false;
            //make sure the timer is set to 0, and then start it
            gameTimer.Reset();
            gameTimer.Start();
            
        }
        #endregion //mouse

        #endregion//input

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            if (gameState == GameState.BootMenu)
            {
                DrawBootMenu(spriteBatch, gameTime);
            }
            else if (gameState == GameState.Loading)
            {
                spriteBatch.DrawString(logoFont, "LOADING " + (gameTimer.DisplayTime), new Vector2(300, 200), Color.Black);
            }

                //check game state, and draw the screen appropriately
            else if (gameState == GameState.MainMenu)
            {
                //draw main menu
                GraphicsDevice.Clear(Color.Black);
                float x = graphics.PreferredBackBufferWidth / 2 - logoFont.MeasureString("BOATS BOATS BOATS").X / 2;
                float y = 25 + logoFont.MeasureString("BOATS BOATS BOATS").Y;
                spriteBatch.DrawString(logoFont, "BOATS BOATS BOATS", new Vector2(x, y), Color.White);
                spriteBatch.Draw(startButton.getTexture(), new Vector2(300, 300), Color.White);
            }
            else if (gameState == GameState.ShipSelection)
            {
                //draw ship selection menu
                DrawShipSelection(spriteBatch, gameTime);
            }
            else if (gameState == GameState.GameOn)
            {
                //draw main game
                DrawGame(spriteBatch, gameTime);
            }
            else if (gameState == GameState.Pause)
            {
                //draw pause menu
                DrawPauseMenu(spriteBatch, gameTime);
            }
            else if (gameState == GameState.GameOver)
            {
                //draw game over screen
                spriteBatch.DrawString(logoFont, "GAME OVER", new Vector2(250, 100), Color.Black);
                spriteBatch.DrawString(mottoFont, "Score: " + score, new Vector2(250, 300), Color.Black);
                spriteBatch.DrawString(mottoFont, "High Score: " + readHighScoreData().HighScore + " " + readHighScoreData().date, new Vector2(400, 300), Color.Black);
                spriteBatch.Draw(returnToMenu.getTexture(), returnToMenu.getPosition(), Color.White);

            }
            spriteBatch.End();
            base.Draw(gameTime);
        }


        /// <summary>
        /// Draws the Boot up Menu for the game
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch which draws everything for the game.  Declared in Draw</param>
        /// <param name="gameTime">Contains timing information for the game</param>
        private void DrawBootMenu(SpriteBatch spriteBatch, GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Draw(logo, new Vector2(graphics.PreferredBackBufferWidth / 2 - logo.Width / 2, graphics.PreferredBackBufferHeight / 2 - logo.Height / 2), Color.White);
            int titleW = graphics.PreferredBackBufferWidth / 2 - (int)logoFont.MeasureString("BlackCode Studios").X / 2;
            int titleH = graphics.PreferredBackBufferHeight / 10;
            spriteBatch.DrawString(logoFont, "BlackCode Studios", new Vector2(titleW, titleH), Color.White);
            spriteBatch.DrawString(mottoFont, "Omnis Erigere Niger Vexillum", new Vector2(200, 600), Color.White);
        }

        private void DrawShipSelection(SpriteBatch spriteBatch, GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            float titleX = graphics.PreferredBackBufferWidth / 2 - logoFont.MeasureString("CHOOSE YOUR SHIP").X / 2;
            spriteBatch.DrawString(logoFont, "CHOOSE YOUR SHIP", new Vector2(titleX, 100), Color.White);

            //draw buttons
            spriteBatch.Draw(brigButton.getTexture(), brigButton.getPosition(), Color.White);
            spriteBatch.Draw(frigateButton.getTexture(), frigateButton.getPosition(), Color.White);
            spriteBatch.Draw(manOfWarButton.getTexture(), manOfWarButton.getPosition(), Color.White);

            //draw ship data
            float brigButtonMid = brigButton.getPosition().X + brigButton.getTexture().Width / 2;
            float frigateButtonMid = frigateButton.getPosition().X + frigateButton.getTexture().Width / 2;
            float mOWButtonMid = manOfWarButton.getPosition().X + manOfWarButton.getTexture().Width / 2;
            //make sure the text is centered around the button
            spriteBatch.DrawString(mottoFont, PLAYER_BRIG_DATA.PrintData() +"\nPOWER: RAMMING SPEED", new Vector2(brigButtonMid - (mottoFont.MeasureString(PLAYER_BRIG_DATA.PrintData()).X / 2), brigButton.getPosition().Y + 30), Color.White);

            spriteBatch.DrawString(mottoFont, PLAYER_FRIG_DATA.PrintData() + "\nPOWER: TANK", new Vector2(frigateButtonMid - (mottoFont.MeasureString(PLAYER_FRIG_DATA.PrintData()).X / 2), frigateButton.getPosition().Y + 30), Color.White);

            spriteBatch.DrawString(mottoFont, PLAYER_MOW_DATA.PrintData() + "\nPOWER: ARMY", new Vector2(mOWButtonMid - (mottoFont.MeasureString(PLAYER_BRIG_DATA.PrintData()).X / 2), brigButton.getPosition().Y + 30), Color.White);

            //draw ship names

            float x = frigateButtonMid - (mottoFont.MeasureString("The Patt Meters").X / 2);
            spriteBatch.DrawString(mottoFont, "The Patt Meters", new Vector2(x, frigateButton.getPosition().Y - 35), Color.White);
            x = brigButtonMid - (mottoFont.MeasureString("The Kimberly").X / 2);
            spriteBatch.DrawString(mottoFont, "The Kimberly", new Vector2(x, brigButton.getPosition().Y - 35), Color.White);
            x = mOWButtonMid - (mottoFont.MeasureString("El Jefe").X / 2);
            spriteBatch.DrawString(mottoFont, "El Jefe", new Vector2(x, manOfWarButton.getPosition().Y - 35), Color.White);


        }

        private void DrawPauseMenu(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(logoFont, "PAUSE", new Vector2(150, 100), Color.Black);
            spriteBatch.Draw(resumeGame.getTexture(), resumeGame.getPosition(), Color.White);
            spriteBatch.Draw(returnToMenu.getTexture(), returnToMenu.getPosition(), Color.White);
        }

        /// <summary>
        /// Draw the main game
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used to draw all textures</param>
        /// <param name="gameTime">Contains timing information for the game</param>
        private void DrawGame(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //need texture, origin, rectangle, color, angle, vector2, sprite effects, float
            //the origin is the center of the ship
            //set background color
            GraphicsDevice.Clear(new Color(28, 107, 160));

            //draw player
            spriteBatch.Draw(enemyCBTexture, player.getPosition(), Color.White);
            spriteBatch.Draw(player.getTexture(), player.getPosition(), null, Color.White, player.getAngle(), player.getOrigin(), 1.0f, SpriteEffects.None, 0.0f);




            //if there are friendlies
            if (player.GetType() == typeof(Player_ManOfWar))
            {
                for (int i = friendlyList.Count - 1; i >= 0; i--)
                {
                    spriteBatch.Draw(friendlyList.ElementAt(i).getTexture(), friendlyList.ElementAt(i).getPosition(), null, Color.White, friendlyList.ElementAt(i).getAngle(), friendlyList.ElementAt(i).getOrigin(), 1.0f, SpriteEffects.None, 0.0f);
                }
            }

            /*draw enemies*/
            for (int i = (EnemyList.Count - 1); i >= 0; i--)
            {
                Enemy e1 = EnemyList.ElementAt(i);
                spriteBatch.Draw(e1.getTexture(), e1.getPosition(), null, Color.White, e1.getAngle(), e1.getOrigin(), 1.0f, SpriteEffects.None, 0.0f);

                //if it is a boss enemy, draw a health bar
                if (e1.GetType() == typeof(Boss1))
                {
                    int x = (int)(e1.getPosition().X - e1.getTexture().Width / 2);
                    int y = (int)(e1.getPosition().Y - e1.getTexture().Height / 2);
                    RectangleF Bounding = e1.BoundingBox;

                    int healthBarL = (int)(healthBar.Width * (double)((e1.getHealth() / e1.getMaxHealth())));
                    spriteBatch.Draw(healthBar, new Rectangle(x, y, healthBarL / 2, 15), Color.Red);
                    //draw bounding box
                    spriteBatch.Draw(BorderTexture, new Rectangle((int)e1.BoundingBox.X, (int)e1.BoundingBox.Y, (int)e1.BoundingBox.Width, (int)e1.BoundingBox.Height), Color.White);
                }
            }

            //draw player cannon balls
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
                    if (player.GetType() == typeof(Player_Frigate) && player.getShipState() == Ship.ShipState.AbilityActivated)
                        spriteBatch.Draw(playerCBPowerUpTexture, c.getPosition(), null, Color.White, c.getAngle(), c.getOrigin(), 1.0f, SpriteEffects.None, 0.0f);
                    else
                        spriteBatch.Draw(playerCBTexture, c.getPosition(), null, Color.White, c.getAngle(), c.getOrigin(), 1.0f, SpriteEffects.None, 0.0f);
                }
            }

            //draw enemy cannon balls
            foreach (Enemy e in EnemyList)
            {
                for (int i = e.getCBA().Count - 1; i >= 0; i--)
                {
                    CannonBall c = e.getCBA().ElementAt(i);
                    if (OutOfBounds(c))
                    {
                        e.getCBA().RemoveAt(i);
                    }
                    else
                        spriteBatch.Draw(enemyCBTexture, c.getPosition(), null, Color.White, c.getAngle(), c.getOrigin(), 1.0f, SpriteEffects.None, 0.0f);
                }
            }
            //draw friendly ships
            foreach (FriendlyShips fs in friendlyList)
            {
                for (int i = fs.getCBA().Count - 1; i >= 0; i--)
                {
                    if (OutOfBounds(fs.getCBA().ElementAt(i)))
                    {
                        fs.getCBA().Remove(fs.getCBA().ElementAt(i));
                    }
                    else
                        spriteBatch.Draw(playerCBTexture, fs.getCBA().ElementAt(i).getPosition(), Color.White);
                }
            }
            
            //draw player interactables
            for (int i = playerInteractableList.Count - 1; i >= 0; i--)
            {
                if (OutOfBounds(playerInteractableList.ElementAt(i)))
                    playerInteractableList.RemoveAt(i);
                else
                    spriteBatch.Draw(playerInteractableList.ElementAt(i).getTexture(), playerInteractableList.ElementAt(i).getPosition(), Color.White);
            }
            
            /*Draw HUD*/
            //draw score
            spriteBatch.DrawString(HUDFont, "Score: " + score + "\nx" + scoreMultiplier, new Vector2(50, 50), Color.Black);
            Color healthBarC = new Color();
            healthBarC = Color.Green;

            //if the player's ability is activated, then they are invincible.  Turn the health bar to gold to indicate this
            if (player.getShipState() == Ship.ShipState.AbilityActivated)
            {
                healthBarC = Color.Gold;
            }

            //draw health bar
            spriteBatch.Draw(healthBar, new Rectangle(this.Window.ClientBounds.Width / 2 - healthBar.Width / 2,
             30, (int)(healthBar.Width * ((double)player.getHealth() / player.getMaxHealth())), 44), healthBarC);

            /*
             * draw ability duration bar underneath the health bar
             * If the ability is depleting, draw gameTimer.RawTime - abilityActivateTime / AbilityDuration
             */
            if (player.getShipState() == Ship.ShipState.AbilityActivated)
            {
                //width of the ability activated bar
                int width = (int)(healthBar.Width * (1 - (gameTimer.RawTime.TotalMilliseconds - player.getAbilityActivateTime().TotalMilliseconds) / player.getAbilityDuration()));

                spriteBatch.Draw(healthBar, new Rectangle(this.Window.ClientBounds.Width / 2 - healthBar.Width / 2,
             75, width, 25), Color.Red);
            }

            //draw ability recharging
            else if (player.getShipState() != Ship.ShipState.AbilityActivated)
            {
                int width = (int)(MathHelper.Clamp((float)(healthBar.Width * (((gameTimer.RawTime.TotalMilliseconds - player.getAbilityRechargeStartTime().TotalMilliseconds) / player.getAbilityRecharge()))), 0, (int)healthBar.Width));
                spriteBatch.Draw(healthBar, new Rectangle(this.Window.ClientBounds.Width / 2 - healthBar.Width / 2,
             75, width, 25), Color.Red);
            }
            
            //draw time
            spriteBatch.DrawString(HUDFont, "Time: " + (gameTimer.DisplayTime), new Vector2(graphics.PreferredBackBufferWidth - 200, 50), Color.Black);
        }
        #endregion


        bool BOSS_READY_TO_SPAWN = false;
        #region Spawn
        /// <summary>
        /// Creates new enemies and adds them to <see cref="EnemyList"/>.  Only spawns new enemies every 3 seconds.
        /// </summary>
        /// <param name="gameTime">snapshot of time values used to limit enemy spawning to happen every 3 seconds</param>
        private void Spawn(GameTime gameTime)
        {
            TimeSpan currentTime = gameTimer.RawTime;
            int scorePerMinute = (int)((score * 1000) / currentTime.TotalMilliseconds);

            //subtract from NextSpawn the time since the last update
            NextSpawn -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            int numberToSpawn = (int)Math.Floor(Math.Log(1 + currentTime.TotalSeconds) + Math.Log((1 + scorePerMinute), 5));

            if (numberToSpawn >= SPAWN_NUMBER_THRESHOLD && BOSS_SPAWN_SUCCESS == false)
            {
                //check to see if the boss can successfully spawn.  If it can't at least set BOSS_READ_TO_SPAWN to true so other enemies stop spawnin
                BOSS_SPAWN_SUCCESS = SpawnBoss();
                BOSS_READY_TO_SPAWN = true;
            }
            //if the boss has spawned successfully, it should be the only thing in the list.  When EnemyList.Count == 0, then the boss is dead.  Set BOSS_SPAWNED_SUCCESS to false, and restart spawning smaller enemies
            if (BOSS_SPAWN_SUCCESS == true)
            {
                if (EnemyList.Count == 0)
                {
                    BOSS_SPAWN_SUCCESS = false;
                    ResetSpawnTime(numberToSpawn, currentTime);
                    BOSS_READY_TO_SPAWN = false;
                }
            }
            else if (NextSpawn <= 0 && EnemyList.Count < 20 && BOSS_READY_TO_SPAWN == false)
            {
                /*
                 * Lograthmic spawn:
                 *      The number of enemies that spawns durig any given wave is the sum of the natural log of game time + the natural log of their score per second
                 *      Ensures that the number is dynamic based on player ability but has a baseline for each game
                 */
                Console.WriteLine(currentTime + " total spawning: " + numberToSpawn + "    " + Math.Log(1 + currentTime.TotalSeconds) + "; " + Math.Log((1 + scorePerMinute), 5));
                numberToSpawn = (int)(MathHelper.Clamp(numberToSpawn, 0, 10));

                lastSpawn = gameTimer.RawTime;
                for (int i = 0; i < numberToSpawn; i++)
                {
                    //create new enemy
                    int spawnType = randomGenerator.Next() % 100;
                    Enemy e = new Enemy_Brig();
                    if (spawnType >= 0 && spawnType < 50)
                    {
                        e = new FireBoat(FIREBOAT_DATA, enemyFireBoat);
                    }
                    else if (spawnType >= 50 && spawnType < 75)
                    {
                        e = new Enemy_Brig(ENEMY_BRIG_DATA, enemyBrig, enemyCBTexture);
                    }
                    else if (spawnType >= 75 && spawnType < 90)
                    {
                        e = new Enemy_Frigate(ENEMY_FRIG_DATA, enemyFrigate, enemyCBTexture);
                    }
                    else
                    {
                        e = new Enemy_ManOfWar(ENEMY_MOW_DATA, enemyManOfWar, enemyCBTexture);
                    }
                    /*create random spawn points within the bounds of the screen.*/
                    int posX, posY;

                    /* There are four regions around the edge of the map (left, right, top, bottom)
                     * randomly choose where to spawn the  enemy so that they spawn anywhere within one of those four regions
                     */
                    int region = randomGenerator.Next(0, 3);
                    switch (region)
                    {
                        //spawn left.  X value should be between the lowest X bound and 50, and y can be from lower bound to buffer height
                        case 0:
                            posX = randomGenerator.Next(LOWER_SPAWN_X, 50);
                            posY = randomGenerator.Next(LOWER_SPAWN_Y, graphics.PreferredBackBufferHeight);
                            break;
                        //spawn top.  Y value is trapped between lower Y bound and 50, and x can be from lower X to buffer width
                        case 1:
                            posX = randomGenerator.Next(LOWER_SPAWN_X, graphics.PreferredBackBufferWidth);
                            posY = randomGenerator.Next(LOWER_SPAWN_Y, 50);
                            break;
                        //spawn right.  X is trapped between buffer width - 50, and buffer width.  Y can be between lower bound and buffer height 
                        case 2:
                            posX = randomGenerator.Next(graphics.PreferredBackBufferWidth - 50, graphics.PreferredBackBufferWidth);
                            posY = randomGenerator.Next(LOWER_SPAWN_Y, graphics.PreferredBackBufferHeight);
                            break;
                        //spawn bottom.  Y is trapped between buffer height - 50 and buffer height.  X can be between lower bound and buffer width
                        default:
                            posX = randomGenerator.Next(LOWER_SPAWN_X, graphics.PreferredBackBufferWidth);
                            posY = randomGenerator.Next(graphics.PreferredBackBufferHeight - 50, graphics.PreferredBackBufferHeight);
                            break;
                    }
                    e.setPosition(new Vector2(posX, posY));

                    //set the ship's angle such that it spans pointing towards the player
                    float angle = Object.TurnToFace(e.getPosition(), player.getPosition(), e.getAngle(), MathHelper.TwoPi);
                    e.setAngle(angle);
                    EnemyList.Add(e);

                    //reset spawn time
                    ResetSpawnTime(numberToSpawn, currentTime);
                }
            }
        }
        #endregion

        private void ResetSpawnTime(int spawnNumber, TimeSpan timeSpan)
        {
            if (timeSpan.TotalSeconds <= 8)
                NextSpawn = SPAWN_TIME_MAX;
            else if (spawnNumber >= SPAWN_NUMBER_THRESHOLD / 2)
                NextSpawn = SPAWN_TIME_MAX;
            else
                NextSpawn = (float)randomGenerator.Next((int)SPAWN_TIME_MIN, (int)SPAWN_TIME_MAX);
        }
        private bool SpawnBoss()
        {
            if (EnemyList.Count == 0)
            {
                Boss1 b = new Boss1(BOSS_DATA, BossTexture, enemyCBTexture, BOSS_POSITION);
                float posX, posY;
                posX = LOWER_SPAWN_X;
                posY = LOWER_SPAWN_Y;
                b.setPosition(new Vector2(posX, posY));
                EnemyList.Add(b);
                Console.WriteLine("SPAWNING BOSS");
                return true;
            }
            if (BOSS_SPAWN_SUCCESS == true)
                return true;
            return false;
        }

        #endregion //game updating

        #region Out of Bounds Check
        /// <summary>
        /// This function checks to see if a vector will be out of the GUI bounds, and if it is out of bounds, will set the vector so that it comes back in bounds
        /// </summary>
        /// <param name="v">the vector taken in to see if it is out of bounds.  Passed by reference.</param>
        /// <param name="t">the texture that belongs to that vector.  Since all objects are drawn with the origin at their middle, it is necesary to know what the width and height of the object are, otherwise, half the object will be allowed to leave the screen</param>
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
        }//end OutOfBounds (vector2, texture)

        /// <summary>
        /// This function checks to see if a CannonBall is out of bounds.  If it is, return true; else return false
        /// </summary>
        /// <param name="c">the cannon ball to be checked for out of boundsness.</param>
        private bool OutOfBounds(CannonBall c)
        {
            if (c.getPosition().X < 0 || c.getPosition().X > graphics.PreferredBackBufferWidth)
                return true;
            else if (c.getPosition().Y < 0 || c.getPosition().Y > graphics.PreferredBackBufferHeight)
                return true;
            else
                return false;
        }//outof bounds cannon

        private bool OutOfBounds(PlayerInteractable m)
        {
            if (m.getPosition().X < 0 || m.getPosition().X > graphics.PreferredBackBufferWidth)
                return true;
            else if (m.getPosition().Y < 0 || m.getPosition().Y > graphics.PreferredBackBufferHeight)
                return true;
            else
                return false;
        }
        #endregion

        private void DropPowerup(Enemy e)
        {
            int r = randomGenerator.Next(0, 100);
            if (r >= 0 && r < 10)
            {
                playerInteractableList.Add(new HealthPowerup(e.getPosition(), new Vector2(0, -1), (float)(Math.PI / 2), healthPowerup));
            }

        }
        #region Collision Detection

        /// <summary>
        /// Collision detection between two ships.  Checks the cannon ball array of the first ship against the second ship to see if the first ship has hit the second.  If it has, damage is dealt, and this function checks to see if the second ship has lost all its health and if it has removes the ship.  Works for both player vs enemy and enemy vs player and handles all special cases
        /// </summary>
        /// <param name="s">The ship dealing damage</param>
        /// <param name="e">The ship having damage dealt against it</param>
        /// <param name="index">Only needs to be used if the function is being called from a loop iterating through a list.  it allows the use of RemoveAt(O(n)) over Remove((O(n))</param>
        
        private void CollisionDetection(Ship s, Ship e, int index)
        {
            //check cannon balls
            for (int j = s.getCBA().Count - 1; j >= 0; j--)
            {
                CannonBall cB = s.getCBA().ElementAt(j);
                if (cB.BoundingBox.Intersects(e.BoundingBox))
                {
                    Console.WriteLine("CB COLLISION: " + "at " + cB.getPosition() + "; HX: " + cB.BoundingBox.HalfX + "; HY: " + cB.BoundingBox.HalfY + "; XA: " + cB.BoundingBox.XAxis + "; YA: " + cB.BoundingBox.YAxis);
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

                        //check if it has collided with a boss, and if it did, force the player backwards
                        if (e.GetType() == typeof(Boss1))
                        {
                            DISABLE_PLAYER_MOVEMENT = true;
                        }
                    }
            }

            //if the ship is a fireboat, check if it has collided with the other ship.  If it has, then do damage and remove it from the list and return
            if (s.GetType() == typeof(FireBoat))
            {
                if (s.BoundingBox.Intersects(e.BoundingBox))
                {
                    Console.WriteLine("FB COLLISION: " + "at " + s.getPosition() + "; HX: " + s.BoundingBox.HalfX + "; HY: " + s.BoundingBox.HalfY + "; XA: " + s.BoundingBox.XAxis + "; YA: " + s.BoundingBox.YAxis);
                    e.takeDamage(s.getDamage());
                    EnemyList.RemoveAt(index);
                    return;
                }
            }

            //check if the ship should be sunk
            if (e.getHealth() <= 0)
            {
                if (e.GetType() == typeof(Player_Brig) || e.GetType() == typeof(Player_Frigate) || e.GetType() == typeof(Player_ManOfWar))
                {
                    gameState = GameState.GameOver;
                }
                else
                {
                    //add score
                    score += (((Enemy)(e)).getScore() * scoreMultiplier);

                    //drop multipliers
                    int numberOfMultis = ((Enemy)(e)).getScore() / 5;
                    for (int m = 0; m < numberOfMultis; m++)
                    {
                        Multiplier mp = new Multiplier(e.getPosition(), e.getPosition(), (float)(randomGenerator.Next(360)) * (float)(Math.PI / 180), multiplierTexture);
                        playerInteractableList.Add(mp);
                    }
                    //determine if this ship will drop a powerup
                    DropPowerup((Enemy)e);
                    //remove from list
                    EnemyList.RemoveAt(index);
                }
            }
        }

        private void CollisionDetection(Player s, PlayerInteractable p, int index)
        {
            if (s.BoundingBox.Intersects(p.BoundingBox))
            {
                if (p.GetType() == typeof(Multiplier))
                    scoreMultiplier++;
                else
                    p.ActivateAbility(s);
                playerInteractableList.RemoveAt(index);
            }
        }
        #endregion
    }
    #endregion
}
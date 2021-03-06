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
using GameUtilities;

namespace PirateWars
{

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    #region Game1
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Private Variables

        #region Player Values
        /// <value>initial spawning location for the player</value>
        Vector2 playerStartingPos;
        /// <value>the Ship that the player controls.  It is made static so that the Enemy class can access it for movement purposes</value>
        protected Player player;
        #endregion

        #region Graphics and Timer
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Timer gameTimer;
        #endregion

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

        #region Menus
        Menu mainMenu;
        Menu bootMenu;
        Menu pauseMenu;
        Menu shipSelectionMenu;
        #endregion

        #region MouseAndKeyboard
        InputManager inputManager;
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
        int SPAWN_NUMBER_THRESHOLD = 9;
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
        GameButton startButton, returnToMenu, resumeGame;
        Vector2 startButtonPos;
        Texture2D logo;
        SpriteFont logoFont;
        SpriteFont mottoFont;
        GameButton brigButton, frigateButton, manOfWarButton;
        #endregion //UI

        #region ObjectDataTypes
        PlayerData PLAYER_BRIG_DATA, PLAYER_FRIG_DATA, PLAYER_MOW_DATA;
        EnemyData FIREBOAT_DATA, ENEMY_BRIG_DATA, ENEMY_FRIG_DATA, ENEMY_MOW_DATA, FRIENDLY_SHIPS_DATA;
        EnemyData BOSS_DATA;
        PlayerInteractableData HEALTH_POWER_DATA, MULTIPLIER_DATA;
        #endregion

        #region Boss Values
        Vector2 BOSS_POSITION;
        bool BOSS_SPAWN_SUCCESS = false;
        bool BOSS_READY_TO_SPAWN = false;
        #endregion

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

            inputManager = new InputManager();

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

            IsFixedTimeStep = true;

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

        #region ChangeMenuFunctions
        private void ToMainMenu() { gameState = GameState.MainMenu; }               //change to MainMenu
        private void ToShipSelection() { gameState = GameState.ShipSelection; }     //change to ShipSelection
        private void ToGameOn() { gameState = GameState.GameOn; }                   //Change to GameOn
        private void ToGameOver() { gameState = GameState.GameOver; }               //change to GameOver
        private void ToPause() { gameState = GameState.Pause; }                     //change to pause
        private void LoadPlayerBrig()
        {
            player = new Player_Brig(PLAYER_BRIG_DATA, playerBrig, playerCBTexture);
            ToGameOn();
        }
        private void LoadPlayerFrig() 
        { 
            player = new Player_Frigate(PLAYER_FRIG_DATA, playerFrigate, playerCBTexture);
            ToGameOn();
        }
        private void LoadPlayerMOW() 
        { 
            player = new Player_ManOfWar(PLAYER_MOW_DATA, playerManOfWar, playerCBTexture);
            ToGameOn();
        }
        #endregion

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

            #region Main Menu
            logo = Content.Load<Texture2D>("BeardCoded2_1");
            logoFont = Content.Load<SpriteFont>("Fonts/LogoFont");
            mottoFont = Content.Load<SpriteFont>("Fonts/MottoFont");

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
            PLAYER_BRIG_DATA = Content.Load<PlayerData>("ObjectDataFiles/Player/Player_BrigData");
            PLAYER_FRIG_DATA = Content.Load<PlayerData>("ObjectDataFiles/Player/Player_FrigateData");
            PLAYER_MOW_DATA = Content.Load<PlayerData>("ObjectDataFiles/Player/Player_ManOfWarData");

            FIREBOAT_DATA = Content.Load<EnemyData>("ObjectDataFiles/AI/FireboatData");
            ENEMY_BRIG_DATA = Content.Load<EnemyData>("ObjectDataFiles/AI/Enemy_BrigData");
            ENEMY_FRIG_DATA = Content.Load<EnemyData>("ObjectDataFiles/AI/Enemy_FrigateData");
            ENEMY_MOW_DATA = Content.Load<EnemyData>("ObjectDataFiles/AI/Enemy_ManOfWarData");

            FRIENDLY_SHIPS_DATA = Content.Load<EnemyData>("ObjectDataFiles/AI/FriendlyShipsData");

            BOSS_DATA = Content.Load<EnemyData>("ObjectDataFiles/AI/BossData");

            HEALTH_POWER_DATA = Content.Load<PlayerInteractableData>("ObjectDataFiles/PlayerInteractables/HealthPowerupData");
            MULTIPLIER_DATA = Content.Load<PlayerInteractableData>("ObjectDataFiles/PlayerInteractables/MultiplierData");
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

            #region Menus

            //General buttons
            startButtonPos = new Vector2(300, 300);
            startButton = new GameButton(Content.Load<Texture2D>("StartButton"), startButtonPos);
            returnToMenu = new GameButton(Content.Load<Texture2D>("ReturnToMenu"), new Vector2(250, 325));
            resumeGame = new GameButton(Content.Load<Texture2D>("StartButton"), new Vector2(250, 500));

            //Boot Menu
            TextField motto = new TextField(("Omnis Erigere Niger Vexillum"), new Vector2(200, 600));
            GameButton logoB = new GameButton(logo, new Vector2(graphics.PreferredBackBufferWidth / 2 - logo.Width / 2, graphics.PreferredBackBufferHeight / 2 - logo.Height / 2));
            bootMenu = new Menu(motto, this);
            bootMenu.AddMenuButton(logoB, ToMainMenu);

            //MainMenu
            TextField menuTitle = new TextField(("BOATS BOATS BOATS"), CenterText("BOATS BOATS BOATS", logoFont));
            mainMenu = new Menu(menuTitle, this);
            mainMenu.AddMenuButton(startButton, ToShipSelection);

            //PauseMenu
            TextField pauseTitle = new TextField("PAUSE", CenterText("PAUSE", logoFont));
            pauseMenu = new Menu(pauseTitle, this);
            pauseMenu.AddMenuButton(resumeGame, ToGameOn);
            pauseMenu.AddMenuButton(returnToMenu, ToMainMenu);

            //ShipSelectionMenu
            float x1 = (float)graphics.PreferredBackBufferWidth / 6 - playerBrig.Width / 2;
            float x2 = (x1 + ((float)graphics.PreferredBackBufferWidth / 3.0f)) - (playerFrigate.Width / 2);
            float x3 = (x2 + ((float)graphics.PreferredBackBufferWidth / 3.0f)) - (playerManOfWar.Width / 2);

            Vector2 ship1Pos = new Vector2(x1, (graphics.PreferredBackBufferHeight / 2) - playerBrig.Height / 2);
            Vector2 ship2Pos = new Vector2(x2, (graphics.PreferredBackBufferHeight / 2) - playerFrigate.Height / 2);
            Vector2 ship3Pos = new Vector2(x3, (graphics.PreferredBackBufferHeight / 2) - playerManOfWar.Height / 2);
            brigButton = new GameButton(playerBrig, ship1Pos);
            frigateButton = new GameButton(playerFrigate, ship2Pos);
            manOfWarButton = new GameButton(playerManOfWar, ship3Pos);

            TextField shipSelectionTitle = new TextField("CHOOSE YOUR SHIP", CenterText("CHOOSE YOUR SHIP", logoFont));
            shipSelectionMenu = new Menu(shipSelectionTitle, this);
            shipSelectionMenu.AddMenuButton(brigButton, LoadPlayerBrig);
            shipSelectionMenu.AddMenuButton(frigateButton, LoadPlayerFrig);
            shipSelectionMenu.AddMenuButton(manOfWarButton, LoadPlayerMOW);
            shipSelectionMenu.AddMenuText(new TextField(PLAYER_BRIG_DATA.PrintData(), new Vector2(brigButton.Position.X, brigButton.Position.Y + brigButton.Texture.Height + 20)));
            shipSelectionMenu.AddMenuText(new TextField(PLAYER_FRIG_DATA.PrintData(), new Vector2(frigateButton.Position.X, frigateButton.Position.Y + brigButton.Texture.Height + 20)));
            shipSelectionMenu.AddMenuText(new TextField(PLAYER_MOW_DATA.PrintData(), new Vector2(manOfWarButton.Position.X, manOfWarButton.Position.Y + brigButton.Texture.Height + 20)));


            #endregion
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
            //update the appropriate menu
            inputManager.Update(gameTime);
            if (gameState == GameState.BootMenu)
                bootMenu.Update(gameTime, inputManager);
            else if (gameState == GameState.MainMenu)
                mainMenu.Update(gameTime, inputManager);
            else if (gameState == GameState.Pause)
                pauseMenu.Update(gameTime, inputManager);
            else if (gameState == GameState.ShipSelection)
                shipSelectionMenu.Update(gameTime, inputManager);

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
                    gameOverSongInstance.Play();
            }

            //if game state is GAME ON, do all game updating
            if (gameState == GameState.GameOn)
            {
                if (gameTimer.RawTime == TimeSpan.Zero)
                    StartGame();
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
                    e.UpdateAndMove(gameTimer.RawTime, player);
                }

                for (int i = playerInteractableList.Count - 1; i >= 0; i--)
                {
                    //update all PlayerInteractables
                    playerInteractableList.ElementAt(i).Update(player, gameTimer.RawTime);
                    //if one has timed out, remove it from the list
                    if (playerInteractableList.ElementAt(i).TimedOut == true)
                    {
                        playerInteractableList.RemoveAt(i);
                    }
                }
                /*check the friendlyList;  if the list has been initialized, and the ship's ability is not activated, then the list should be cleared.  Only applies to Player_ManOfWar.  Otherwise, nothing will happen here*/
                if (friendlyList.Count >= 0 && (player.getShipState() != Player.ShipState.AbilityActivated))
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
                            friendlyList.ElementAt(i).UpdateAndMove(gameTimer.RawTime, EnemyList.ElementAt(target));
                        }
                    }
                }
                Spawn(gameTime);
                gameTimer.Update(gameTime);
                base.Update(gameTime);
            }
            else
            {
                IsMouseVisible = true;
            }
        }
        #endregion


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
            //KeyboardState newState = Keyboard.GetState();
            Vector2 newP = player.Position;
            float angle = player.Angle;
            TimeSpan newKeyPress = time.TotalGameTime;
            /*
             * WAD are the movement keys, but only W will actually move the ship
             * W: move ship forward in the direction that the ship is pointing 
             * A: rotate the ship to the left (-radians)
             * D: rotate the ship to the right (+radians)
             */
            if (inputManager.KeyIsDown(Keys.Escape))
            {
                gameState = GameState.Pause;
                gameTimer.Pause();
            }

            if (inputManager.KeyIsDown(Keys.Tab))
            {
                if (player.getShipState() == Player.ShipState.AbilityCharged)
                {
                    player.ActivateAbility(gameTimer.RawTime);
                    if (player.GetType() == typeof(Player_ManOfWar))
                    {
                        //brute force math where the 4 ships should spawn
                        GenerateFriendlies();
                    }
                }
            }

            if (inputManager.KeyIsDown(Keys.A) || inputManager.KeyIsDown(Keys.Left))
            {
                angle = player.Angle - player.TurnSpeed;
            }

            if (inputManager.KeyIsDown(Keys.D) || inputManager.KeyIsDown(Keys.Right))
            {
                angle = player.Angle + player.TurnSpeed;
            }

            if (inputManager.KeyIsDown(Keys.W) || inputManager.KeyIsDown(Keys.Up))
            {
                Vector2 direction = new Vector2((float)(Math.Cos(player.Angle)), (float)(Math.Sin(player.Angle)));
                direction.Normalize();
                newP += direction * player.Speed;
            }

            //check to make sure it stays within bounds, and then update position and angle
            if (BOSS_SPAWN_SUCCESS)
            {
                OutOfBounds(ref newP, player.Texture, BOSS_POSITION.X, graphics.PreferredBackBufferWidth, 0, graphics.PreferredBackBufferHeight - 50);
            }
            else
                OutOfBounds(ref newP, player.Texture, 0, graphics.PreferredBackBufferWidth, 0, graphics.PreferredBackBufferHeight);
            player.Position = newP;
            player.Angle = angle;

            //place a delay on the space bar firing
            if (inputManager.KeyIsDown(Keys.Space))
                player.Fire(gameTimer.RawTime);
        }//end UpdateInput (Keyboard
        #endregion //keyboard

        private void GenerateFriendlies()
        {
            FriendlyShips e = new FriendlyShips(ENEMY_FRIG_DATA, playerFrigate, playerCBTexture);
            float BUFFER = 25;
            friendlyList = new List<FriendlyShips>();
            //ship 1 (12:00)
            float x = player.Position.X;
            float y = player.Position.Y - (2 * player.Origin.Y) - (2 * e.Origin.Y) - BUFFER;
            Vector2 ePos = new Vector2(x, y);
            e.Position = ePos;
            friendlyList.Add(e);

            //ship 2 (9 position)
            x = player.Position.X - player.Origin.X - e.Origin.X - BUFFER;
            y = player.Position.Y - player.Origin.Y;
            FriendlyShips e1 = new FriendlyShips(ENEMY_FRIG_DATA, playerFrigate, playerCBTexture);
            ePos = new Vector2(x, y);
            e1.Position = ePos;
            friendlyList.Add(e1);

            //ship 3 (6:00)
            x = player.Position.X;
            y = player.Position.Y + (2 * player.Origin.Y) + (2 * e.Origin.Y) + BUFFER;
            ePos = new Vector2(x, y);
            FriendlyShips e2 = new FriendlyShips(ENEMY_FRIG_DATA, playerFrigate, playerCBTexture);
            e2.Position = ePos;
            friendlyList.Add(e2);

            //ship 4 (3:00)
            x = player.Position.X + player.Origin.X + e.Origin.X + BUFFER;
            y = player.Position.Y - player.Origin.Y;
            FriendlyShips e3 = new FriendlyShips(ENEMY_FRIG_DATA, playerFrigate, playerCBTexture);
            ePos = new Vector2(x, y);
            e3.Position = ePos;
            friendlyList.Add(e3);
        }

        #region Helpers
        /// <summary>
        /// Set all game values to default value.  Called when first starting the game (or restarting the game)
        /// </summary>
        private void StartGame()
        {
            gameState = GameState.Loading;

            //ready the player
            player.Position = playerStartingPos;
            player.Reset();

            //set game constants
            score = 0;
            scoreMultiplier = 1;
            lastSpawn = TimeSpan.Zero;

            //make sure all lists are empty
            EnemyList.Clear();
            player.CannonBalls.Clear();
            playerInteractableList.Clear();

            //set gameState to GameOn
            gameState = GameState.GameOn;

            //set Boss values to false
            BOSS_SPAWN_SUCCESS = false;
            BOSS_READY_TO_SPAWN = false;

            //make sure the timer is set to 0, and then start it
            gameTimer.Reset();
            gameTimer.Start();
        }

        private void DropPowerup(Enemy e)
        {
            int r = randomGenerator.Next(0, 100);
            if (r >= 0 && r < 10)
            {
                playerInteractableList.Add(new HealthPowerup(HEALTH_POWER_DATA, e.Position, new Vector2(0, -1), (float)(Math.PI / 2), healthPowerup, gameTimer.RawTime));
            }

        }

        private Vector2 CenterText(string s, SpriteFont f)
        {
            float x = graphics.PreferredBackBufferWidth / 2 - f.MeasureString(s).X / 2;
            float y = 25 + f.MeasureString(s).Y;
            return new Vector2(x, y);
        }
        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            //check game state and draw screen accordingly
            if (gameState == GameState.BootMenu)
            {
                GraphicsDevice.Clear(Color.Black);
                bootMenu.DrawMenu(spriteBatch, graphics, mottoFont, null);
                //DrawBootMenu(spriteBatch, gameTime);
            }
            else if (gameState == GameState.Loading)
            {
                spriteBatch.DrawString(logoFont, "LOADING " + (gameTimer.DisplayTime), new Vector2(300, 200), Color.Black);
            }
            else if (gameState == GameState.MainMenu)
            {
                //draw main menu
                GraphicsDevice.Clear(Color.Black);
                mainMenu.DrawMenu(spriteBatch, graphics, logoFont, null);
            }
            else if (gameState == GameState.ShipSelection)
            {
                //draw ship selection menu
                shipSelectionMenu.DrawMenu(spriteBatch,graphics,logoFont, mottoFont);
            }
            else if (gameState == GameState.GameOn)
            {
                //draw main game
                DrawGame(spriteBatch, gameTime);
            }
            else if (gameState == GameState.Pause)
            {
                //draw pause menu
                //DrawPauseMenu(spriteBatch, gameTime);
            }
            else if (gameState == GameState.GameOver)
            {
                //draw game over screen
                spriteBatch.DrawString(logoFont, "GAME OVER", new Vector2(250, 100), Color.Black);
                spriteBatch.DrawString(mottoFont, "Score: " + score, new Vector2(250, 300), Color.Black);
                spriteBatch.DrawString(mottoFont, "High Score: " + readHighScoreData().HighScore + " " + readHighScoreData().date, new Vector2(400, 300), Color.Black);
                spriteBatch.Draw(returnToMenu.Texture, returnToMenu.Position, Color.White);

            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Draw the main game
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used to draw all textures</param>
        /// <param name="gameTime">Contains timing information for the game</param>
        private void DrawGame(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //set background color
            GraphicsDevice.Clear(new Color(28, 107, 160));

            //draw player
            spriteBatch.Draw(player.Texture, player.Position, null, Color.White, player.Angle, player.Origin, 1.0f, SpriteEffects.None, 0.0f);

            //if there are friendlies
            if (player.GetType() == typeof(Player_ManOfWar))
            {
                for (int i = friendlyList.Count - 1; i >= 0; i--)
                {
                    spriteBatch.Draw(friendlyList.ElementAt(i).Texture, friendlyList.ElementAt(i).Position, null, Color.White, friendlyList.ElementAt(i).Angle, friendlyList.ElementAt(i).Origin, 1.0f, SpriteEffects.None, 0.0f);
                }
            }

            /*draw enemies*/
            for (int i = (EnemyList.Count - 1); i >= 0; i--)
            {
                Enemy e1 = EnemyList.ElementAt(i);
                spriteBatch.Draw(e1.Texture, e1.Position, null, Color.White, e1.Angle, e1.Origin, 1.0f, SpriteEffects.None, 0.0f);

                //if it is a boss enemy, draw a health bar
                if (e1.GetType() == typeof(Boss1))
                {
                    int x = (int)(e1.Position.X - e1.Texture.Width / 2);
                    int y = (int)(e1.Position.Y - e1.Texture.Height / 2);

                    //give the enemy a health bar that the player can see
                    int healthBarL = (int)((healthBar.Width * .5f) * (double)((e1.Health / e1.MaxHealth)));
                    spriteBatch.Draw(healthBar, new Rectangle(x, y, healthBarL / 2, 15), Color.Red);
                }
            }

            //draw player cannon balls
            for (int i = player.CannonBalls.Count - 1; i >= 0; i--)
            {
                CannonBall c = player.CannonBalls.ElementAt(i);
                /*if the cannon ball has gone out of bounds, remove it and do not draw, else draw it*/
                if (OutOfBounds(player.CannonBalls.ElementAt(i)))
                {
                    player.CannonBalls.RemoveAt(i);
                }
                else
                {
                    if (player.GetType() == typeof(Player_Frigate) && player.getShipState() == Player.ShipState.AbilityActivated)
                        spriteBatch.Draw(playerCBPowerUpTexture, c.Position, null, Color.White, c.Angle, c.Origin, 1.0f, SpriteEffects.None, 0.0f);
                    else
                        spriteBatch.Draw(playerCBTexture, c.Position, null, Color.White, c.Angle, c.Origin, 1.0f, SpriteEffects.None, 0.0f);
                }
            }

            //draw enemy cannon balls
            foreach (Enemy e in EnemyList)
            {
                for (int i = e.CannonBalls.Count - 1; i >= 0; i--)
                {
                    CannonBall c = e.CannonBalls.ElementAt(i);
                    if (OutOfBounds(c))
                    {
                        e.CannonBalls.RemoveAt(i);
                    }
                    else
                        spriteBatch.Draw(enemyCBTexture, c.Position, null, Color.White, c.Angle, c.Origin, 1.0f, SpriteEffects.None, 0.0f);
                }
            }
            //draw friendly cannon balls
            foreach (FriendlyShips fs in friendlyList)
            {
                for (int i = fs.CannonBalls.Count - 1; i >= 0; i--)
                {
                    if (OutOfBounds(fs.CannonBalls.ElementAt(i)))
                    {
                        fs.CannonBalls.Remove(fs.CannonBalls.ElementAt(i));
                    }
                    else
                    {
                        spriteBatch.Draw(playerCBTexture, fs.CannonBalls.ElementAt(i).Position, null, Color.White, fs.CannonBalls.ElementAt(i).Angle, fs.CannonBalls.ElementAt(i).Origin, 1.0f, SpriteEffects.None, 0.0f);
                    }
                }
            }

            //draw player interactables
            for (int i = playerInteractableList.Count - 1; i >= 0; i--)
            {
                if (OutOfBounds(playerInteractableList.ElementAt(i)))
                    playerInteractableList.RemoveAt(i);
                else if (playerInteractableList.ElementAt(i).Faded == false)
                {
                    spriteBatch.Draw(playerInteractableList.ElementAt(i).Texture, playerInteractableList.ElementAt(i).Position, Color.White);
                }
            }

            /*Draw HUD*/
            //draw score
            spriteBatch.DrawString(HUDFont, "Score: " + score + "\nx" + scoreMultiplier, new Vector2(50, 50), Color.Black);
            Color healthBarC = new Color();
            healthBarC = Color.Green;

            //if the player's ability is activated, then they are invincible.  Turn the health bar to gold to indicate this
            if (player.getShipState() == Player.ShipState.AbilityActivated)
            {
                healthBarC = Color.Gold;
            }

            //draw health bar
            spriteBatch.Draw(healthBar, new Rectangle(this.Window.ClientBounds.Width / 2 - healthBar.Width / 2,
             30, (int)(healthBar.Width * ((double)player.Health / player.MaxHealth)), 44), healthBarC);

            /*
             * draw ability duration bar underneath the health bar
             * If the ability is depleting, draw gameTimer.RawTime - abilityActivateTime / AbilityDuration
             */
            if (player.getShipState() == Player.ShipState.AbilityActivated)
            {
                //width of the ability activated bar
                int width = (int)(healthBar.Width * (1 - (gameTimer.RawTime.TotalMilliseconds - player.getAbilityActivateTime().TotalMilliseconds) / player.getAbilityDuration()));

                spriteBatch.Draw(healthBar, new Rectangle(this.Window.ClientBounds.Width / 2 - healthBar.Width / 2,
             75, width, 25), Color.Red);
            }

            //draw ability recharging
            else if (player.getShipState() != Player.ShipState.AbilityActivated)
            {
                int width = (int)(MathHelper.Clamp((float)(healthBar.Width * (((gameTimer.RawTime.TotalMilliseconds - player.getAbilityRechargeStartTime().TotalMilliseconds) / player.getAbilityRecharge()))), 0, (int)healthBar.Width));
                spriteBatch.Draw(healthBar, new Rectangle(this.Window.ClientBounds.Width / 2 - healthBar.Width / 2,
             75, width, 25), Color.Red);
            }

            //draw time
            spriteBatch.DrawString(HUDFont, "Time: " + (gameTimer.DisplayTime), new Vector2(graphics.PreferredBackBufferWidth - 200, 50), Color.Black);
        }
        #endregion

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
                    ResetSpawnTime(numberToSpawn);
                    BOSS_READY_TO_SPAWN = false;
                    //after beating the boss, increase the spawn number threshold so that they can face normal enemies again
                    SPAWN_NUMBER_THRESHOLD += 2;
                }
            }
            //if the boss is not ready to spawn, continue spawning regular enemies enemies
            else if (NextSpawn <= SPAWN_NUMBER_THRESHOLD && BOSS_READY_TO_SPAWN == false)
            {
                /*
                 * Lograthmic spawn:
                 *      The number of enemies that spawns durig any given wave is the sum of the natural log of game time + the natural log of their score per second
                 *      Ensures that the number is dynamic based on player ability but has a baseline for each game
                 */
                numberToSpawn = (int)(MathHelper.Clamp(numberToSpawn, 0, 8));

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
                    e.Position = new Vector2(posX, posY);

                    //set the ship's angle such that it spans pointing towards the player
                    float angle = Object.TurnToFace(e.Position, player.Position, e.Angle, MathHelper.TwoPi);
                    e.Angle = angle;
                    EnemyList.Add(e);

                    //reset spawn time
                    ResetSpawnTime(numberToSpawn);
                }
            }
        }

        private void ResetSpawnTime(int spawnNumber)
        {
            //if it is the first 10 seconds of the game, force the next spawn to happen at SPAWN_TIME_MAX
            if (gameTimer.RawTime.TotalSeconds <= 10)
                NextSpawn = SPAWN_TIME_MAX;
            //otherwise, just pick a spawn time between the min and max times
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
                b.Position = (new Vector2(posX, posY));
                EnemyList.Add(b);
                return true;
            }
            if (BOSS_SPAWN_SUCCESS == true)
                return true;
            return false;
        }
        #endregion

        #endregion //game updating

        #region Out of Bounds Check
        /// <summary>
        /// This function checks to see if a vector will be out of the GUI bounds, and if it is out of bounds, will set the vector so that it comes back in bounds
        /// </summary>
        /// <param name="v">the vector taken in to see if it is out of bounds.  Passed by reference.</param>
        /// <param name="t">the texture that belongs to that vector.  Since all objects are drawn with the origin at their middle, it is necesary to know what the width and height of the object are, otherwise, half the object will be allowed to leave the screen</param>
        /// <param name="lowerXBound">The vector's x value is not allowed to be less than this</param>
        /// <param name="lowerYBound">The vector's y value is not allowed to be less than this</param>
        /// <param name="upperXBound">The vector's x value is not allowed to be greater than this</param>
        /// <param name="upperYBound">The vector's y value is not allowed to be greater than this</param>
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
            if (c.Position.X < 0 || c.Position.X > graphics.PreferredBackBufferWidth)
                return true;
            else if (c.Position.Y < 0 || c.Position.Y > graphics.PreferredBackBufferHeight)
                return true;
            else
                return false;
        }//outof bounds cannon

        private bool OutOfBounds(PlayerInteractable m)
        {
            if (m.Position.X < 0 || m.Position.X > graphics.PreferredBackBufferWidth)
                return true;
            else if (m.Position.Y < 0 || m.Position.Y > graphics.PreferredBackBufferHeight)
                return true;
            else
                return false;
        }
        #endregion

        #region Collision Detection

        /// <summary>
        /// Collision detection between two ships.  Checks the cannon ball array of the first ship against the second ship to see if the first ship has hit the second.  If it has, damage is dealt, and this function checks to see if the second ship has lost all its health and if it has removes the ship.  Works for both player vs enemy and enemy vs player and handles all special cases
        /// </summary>
        /// <param name="s">The ship dealing damage</param>
        /// <param name="e">The ship having damage dealt against it</param>
        /// <param name="index">Only needs to be used if the function is being called from a loop iterating through a list.  it allows the use of RemoveAt() which is O(1) over Remove() which is O(n)</param>

        private void CollisionDetection(Ship s, Ship e, int index)
        {
            //check cannon balls
            for (int j = s.CannonBalls.Count - 1; j >= 0; j--)
            {
                CannonBall cB = s.CannonBalls.ElementAt(j);
                if (cB.BoundingBox.Intersects(e.BoundingBox))
                {
                    //deal damage to enemy
                    e.takeDamage(cB.Damage);
                    //remove cannon ball
                    s.CannonBalls.Remove(cB);
                }//end if collision
                //check if the ship should be sunk
            }//end for j

            //if s is the player, then check if it is a brig and has its ability activated
            if (s == player)
            {
                if (player.GetType() == typeof(Player_Brig) && player.getShipState() == Player.ShipState.AbilityActivated)
                    if (s.BoundingBox.Intersects(e.BoundingBox))
                    {
                        e.takeDamage(s.Damage);
                    }
            }

            //if the ship is a fireboat, check if it has collided with the other ship.  If it has, then do damage and remove it from the list and return
            if (s.GetType() == typeof(FireBoat))
            {
                if (s.BoundingBox.Intersects(e.BoundingBox))
                {
                    EnemyList.RemoveAt(index);
                    return;
                }
            }

            //check if the ship should be sunk
            if (e.Health <= 0)
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
                        Multiplier mp = new Multiplier(MULTIPLIER_DATA, e.Position, e.Position, (float)(randomGenerator.Next(360)) * (float)(Math.PI / 180), multiplierTexture, gameTimer.RawTime);
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
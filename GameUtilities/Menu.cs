using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameUtilities
{
    public class Menu : DrawableGameComponent
    {
        bool active = false;
        private List<MenuButton> menuButtons;
        TextField title;
        List<TextField> bodyText;
        List<Texture2D> textures;
        private int _selectedIndex;
        /// <summary>
        /// Get the Title of this menu
        /// </summary>
        public TextField Title
        {
            get
            {
                return title;
            }
        }

        /// <summary>
        /// Get the number of buttons in the menu
        /// </summary>
        public int ButtonCount
        {
            get { return menuButtons.Count; }
        }

        /// <summary>
        /// Get the number of text field objects in the menu
        /// </summary>
        public int TextFieldCount
        {
            get
            {
                return bodyText.Count;
            }
        }

        /// <summary>
        /// Get the total number of objects (text fields and buttons) in the menu.  Does not include title.
        /// </summary>
        public int TotalObjectCount
        {
            get
            {
                return ButtonCount + TextFieldCount;
            }
        }

        /// <summary>
        /// The index of the button that is currently highlighted.  Used when navigating menus with the keyboard or controller. Currently not functional
        /// </summary>
        public int selectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            protected set
            {
                if (value >= menuButtons.Count || value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _selectedIndex = value;
            }
        }
        /// <summary>
        /// The button that the user is currently highlighted.  Used when navigating menus with the keyboard or controller.
        /// </summary>
        public MenuButton SelectedItem
        {
            get
            {
                return menuButtons[selectedIndex];
            }
        }

        /// <summary>
        /// Get wether or not a menu is active
        /// </summary>
        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }
        /// <summary>
        /// Construct new menu that is only a TextField
        /// </summary>
        /// <param name="t">Title of menu</param>
        /// <param name="g">The game that this menu is associated with</param>
        public Menu(TextField t, Game g) : base(g)
        {
            menuButtons = new List<MenuButton>();
            bodyText = new List<TextField>();
            textures = new List<Texture2D>();
            title = t;
            g.IsMouseVisible = true;
        }

        /// <summary>
        /// Construct new menu that is a title and series of textures.  Extra textures are not buttons and will not check for user input
        /// </summary>
        /// <param name="t">Title of menu</param>
        /// <param name="t2D">List of textures to be drawn</param>
        /// <param name="g">Game that this menu is associated with</param>
        public Menu(TextField t, List<Texture2D> t2D, Game g)
            : this(t, g)
        {
            textures = t2D;
        }
        /// <summary>
        /// Construct new menu without any buttons or extra textures
        /// </summary>
        /// <param name="t">Title of menu</param>
        /// <param name="t2D">Textures to be drawn on screen</param>
        /// <param name="body">List of text fields for the body text</param>
        /// <param name="g">The game this menu is associated with</param>
        public Menu(TextField t, List<Texture2D> t2D, List<TextField> body, Game g)
            : this(t,t2D,g)
        {
            menuButtons = new List<MenuButton>();
            bodyText = body;
        }

        /// <summary>
        /// Construct new menu
        /// </summary>
        /// <param name="t">Title of menu</param>
        /// <param name="t2D">Textures to be drawn on screen</param>
        /// <param name="body">List containing body text fields</param>
        /// <param name="button">List containing buttons for menu</param>
        /// <param name="g">The game this menu is associated with</param>
        public Menu(TextField t, List <Texture2D> t2D, List<TextField> body, List<MenuButton> button, Game g)
            : this(t, t2D, body, g)
        {
            menuButtons = button;
        }

        /// <summary>
        /// Add a new Button to the menu
        /// </summary>
        /// <param name="button">The button you want to add</param>
        /// <param name="action">The action associated with that button</param>
        public virtual void AddMenuButton(GameButton button, Action action)
        {
            menuButtons.Add(new MenuButton(button, action));
            selectedIndex = 0;
        }

        public virtual void AddMenuTexture(Texture2D t) 
        {
            textures.Add(t);
        }

        public virtual void AddMenuText(TextField t)
        {
            bodyText.Add(t);
        }
        /// <summary>
        /// Draw the menu.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch used to draw</param>
        /// <param name="graphics">The graphics device manager associated with the menu (the same one associated with the game)</param>
        /// <param name="titleFont">The font used for the title</param>
        /// <param name="textFont">The font used for the body text</param>
        public void DrawMenu(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, SpriteFont titleFont, SpriteFont textFont)
        {
            //draw title
            spriteBatch.DrawString(titleFont, title.Text, title.Position, Color.White);

            //draw body text (if there is any)
            if (bodyText != null)
            {
                foreach (TextField t in bodyText)
                {
                    spriteBatch.DrawString(textFont, t.Text, t.Position, Color.Black);
                }
            }
            //draw menu buttons (if there are any)
            foreach (MenuButton b in menuButtons)
            {
                spriteBatch.Draw(b.Button.Texture, b.Button.Position, Color.White);
            }
        }

        /// <summary>
        /// Checks for user input and updates according
        /// </summary>
        /// <param name="gameTime">Snapshot of timing variables from game</param>
        /// <param name="i">The input manager used to monitor user input</param>
        public void Update(GameTime gameTime, InputManager i)
        {
            foreach (MenuButton b in menuButtons)
            {
                Console.WriteLine("\t" + b.Button.Position);
                if (i.GameButtonWasClicked(b.Button))
                {
                    Console.WriteLine("Button Clicked!");
                    b.Action();
                }

            }
        }

        ///<summary>
        ///Used to navigate a menu using the keyboard or game controller and do a button action
        ///</summary>
        /*public void Navigate(GamePadState gamePadState, GameTime gameTime)
        {
            if ((gamePadState.ThumbSticks.Left.Y < -0.5
               || gamePadState.DPad.Down == ButtonState.Pressed)
               && selectedIndex < ButtonCount - 1)
            {
                selectedIndex++;
            }
            if ((gamePadState.ThumbSticks.Left.Y > 0.5
                        || gamePadState.DPad.Up == ButtonState.Pressed)
                        && selectedIndex > 0)
            {
                selectedIndex--;
            }
            if (gamePadState.Buttons.A == ButtonState.Pressed)
            {
                SelectedItem.Action(Buttons.A);
            }
            else if (gamePadState.Buttons.B == ButtonState.Pressed)
            {
                SelectedItem.Action(Buttons.B);
            }
            else if (gamePadState.Buttons.X == ButtonState.Pressed)
            {
                SelectedItem.Action(Buttons.X);
            }
            else if (gamePadState.Buttons.Y == ButtonState.Pressed)
            {
                SelectedItem.Action(Buttons.Y);
            }
        }*/
    }
}

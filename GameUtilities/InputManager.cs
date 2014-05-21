using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace GameUtilities
{
    /// <summary>
    /// Handles keyboard and mouse inputs.
    /// Taken from TimGThomas's GitHub page at:
    /// https://github.com/TimGThomas/XnaInputManager
    /// </summary>
    /// Added to this class were all comments, and the interaction between the GameButton class.
    /// Many functions were also renamed to keep the naming similar and decrease confusion
    /// 
    /// It is important to keep the vocabulary of this class clear:
    ///     
    ///     -Down:                   the input for that key/mouse button is currently active/high
    ///     -Up:                     the input for that key/mouse button is currently inactive/low
    ///     -Pressed:                the input is currently down but was previously up
    ///     -Released/Clicked:       the input is currently up but was previously down
    ///     
    /// The main point is that Up/Down/Pressed are checks of the current state ONLY, and Released/Clicked depend on the previous state
    /// The XNA Framework uses these same key terms but in a different manner.  Down and Up are used with keys, and Pressed and Released with mouse buttons.   They only refer to the current state without relation to any previous state.

	public class InputManager : GameComponent
	{
        public enum MouseButtons
        {
            Left,
            Middle,
            Right,
            Extra1,
            Extra2
        }

        #region PrivateVariables
        private KeyboardState _priorKeyboardState;
		private KeyboardState _currentKeyboardState;

		private MouseState _priorMouseState;
		private MouseState _currentMouseState;

		private readonly IDictionary<Keys, TimeSpan> _keyHeldTimes;
		
        private readonly IDictionary<MouseButtons, TimeSpan> _mouseButtonHeldTimes;
		private readonly IDictionary<MouseButtons, Func<MouseState, ButtonState>> _mouseButtonMaps;
        
        private readonly IDictionary<Keys, TimeSpan> lastKeyPress;
        private readonly IDictionary<MouseButtons, TimeSpan> lastButtonPress;
        #endregion

        public InputManager() : base(null)
		{
			_mouseButtonMaps = new Dictionary<MouseButtons, Func<MouseState, ButtonState>>
			{
				{ MouseButtons.Left, s => s.LeftButton },
				{ MouseButtons.Right, s => s.RightButton },
				{ MouseButtons.Middle, s => s.MiddleButton },
				{ MouseButtons.Extra1, s => s.XButton1 },
				{ MouseButtons.Extra2, s => s.XButton2 }
			};

			_keyHeldTimes = new Dictionary<Keys, TimeSpan>();
            lastKeyPress = new Dictionary<Keys, TimeSpan>();
			foreach (var key in Enum.GetValues(typeof(Keys)))
			{
				_keyHeldTimes.Add((Keys)key, TimeSpan.Zero);
                lastKeyPress.Add((Keys)key, TimeSpan.Zero);
                
			}

			_mouseButtonHeldTimes = new Dictionary<MouseButtons, TimeSpan>();
            lastButtonPress = new Dictionary<MouseButtons, TimeSpan>();
			foreach (var mouseButton in Enum.GetValues(typeof(MouseButtons)))
			{
				_mouseButtonHeldTimes.Add((MouseButtons)mouseButton, TimeSpan.Zero);
                lastButtonPress.Add((MouseButtons)mouseButton, TimeSpan.Zero);
			}
		}

        #region Keyboard Functions
        /// <summary>
        /// Check if a key was pressed.
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <returns>True if the key is currently down and it was previously up.  False otherwise</returns>
		public bool KeyWasPressed(Keys key)
		{
			return _currentKeyboardState.IsKeyDown(key) && _priorKeyboardState.IsKeyUp(key);
		}

        /// <summary>
        /// Check if a key is currently down
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if the key is currently down, false otherwise</returns>
        public bool KeyIsDown(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Force a delay between key presses.  Checks first if the time delay has been satisfied, and then if the key is actually down.  The key can be held down, but this function will only register true once during each delay period.
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <param name="gameTime">Snapshot of time values from main game</param>
        /// <param name="delay">The delay to be enforced between key presses</param>
        /// <returns>True if enough time has passed and the key is down, false otherwise</returns>
        public bool KeyIsDown(Keys key, TimeSpan gameTime, float delay)
        {
            if (gameTime.TotalMilliseconds - lastKeyPress[key].TotalMilliseconds < delay)
            {
                return false;
            }
            return KeyIsDown(key);
        }

        /// <summary>
        /// Check if a key was pressed for a certain amount of time
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <param name="timeSpan">Time to compare against</param>
        /// <returns>True if the </returns>
		public bool KeyWasPressedFor(Keys key, TimeSpan timeSpan)
		{
			return GetElapsedHeldTime(key).CompareTo(timeSpan) >= 0;
		}

        /// <summary>
        /// Check if a key was pressed with a group of other keys
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <param name="modifiers">The other group of keys</param>
        /// <returns>True if all keys are pressed, false otherwise</returns>
		public bool KeyWasPressedWithModifiers(Keys key, params Keys[] modifiers)
		{
			return KeyWasPressed(key) && modifiers.All(k => _currentKeyboardState.IsKeyDown(k));
		}

        /// <summary>
        /// Check if a key was released.
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <returns>True if the key is currently up but was previously down, false otherwise</returns>
		public bool KeyWasReleased(Keys key)
		{
			return _currentKeyboardState.IsKeyUp(key) && _priorKeyboardState.IsKeyDown(key);
		}

        /// <summary>
        /// Get the time that a key has been held for
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <returns>How long a key has been held for</returns>
		public TimeSpan GetElapsedHeldTime(Keys key)
		{
			return _keyHeldTimes[key];
		}

        #endregion

        #region Mouse Button Functions
        /// <summary>
        /// Get how long a mouse button has been held for
        /// </summary>
        /// <param name="mouseButton">The mouse button to be checked</param>
        /// <returns>How long a mouse button has been held</returns>
		public TimeSpan GetElapsedHeldTime(MouseButtons mouseButton)
		{
			return _mouseButtonHeldTimes[mouseButton];
		}

        /// <summary>
        /// Check if a mouse button is down
        /// </summary>
        /// <param name="button">The mouse button to be checked</param>
        /// <returns>True if the mouse button is down, false otherwise</returns>
		public bool MouseButtonIsDown(MouseButtons button)
		{
			return _mouseButtonMaps[button](_currentMouseState) == ButtonState.Pressed;
		}
        /// <summary>
        /// Check if a mouse button is up
        /// </summary>
        /// <param name="button">The mouse button to be checked</param>
        /// <returns>True if the mouse button is up, false otherwise</returns>
		public bool MouseButtonIsUp(MouseButtons button)
		{
			return _mouseButtonMaps[button](_currentMouseState) == ButtonState.Released;
		}

        /// <summary>
        /// Check if a mouse button was pressed
        /// </summary>
        /// <param name="button">The mouse button to be checked</param>
        /// <returns>True if the button is currently pressed, but was previously released; false otherwise</returns>
		public bool MouseButtonWasPressed(MouseButtons button)
		{
            if (_mouseButtonMaps[button](_currentMouseState) == ButtonState.Pressed)
            {
                if (_mouseButtonMaps[button](_priorMouseState) == ButtonState.Released)
                    return true;
            }
            return false;
		}

        /// <summary>
        /// Check if a botton was pressed with a group of keys
        /// </summary>
        /// <param name="button">The mouse button to be checked</param>
        /// <param name="modifierKeys">The group of keys to also be checked</param>
        /// <returns>True if the mouse button is pressed and all keys are down, false otherwise</returns>
		public bool ButtonWasPressedWithKeyModifiers(MouseButtons button, params Keys[] modifierKeys)
		{
			return MouseButtonWasClicked(button) && modifierKeys.All(k => _currentKeyboardState.IsKeyDown(k));
		}

        /// <summary>
        /// Check if the mouse button was released
        /// </summary>
        /// <param name="button">The mouse button to be checked</param>
        /// <returns>True if the mouse button is currently up, but was previously down</returns>
		public bool MouseButtonWasClicked(MouseButtons button)
		{
			return
				_mouseButtonMaps[button](_currentMouseState) == ButtonState.Released &&
				_mouseButtonMaps[button](_priorMouseState) == ButtonState.Pressed;
		}

        /// <summary>
        /// Get the distance the mouse wheel scrolled
        /// </summary>
        /// <returns>The difference in wheel scroll values from the prior mouse state and the current state</returns>
		public int GetDistanceScrolled()
		{
			return _currentMouseState.ScrollWheelValue - _priorMouseState.ScrollWheelValue;
		}

        /// <summary>
        /// Check if the mouse is scrolling up
        /// </summary>
        /// <returns>True if the current wheel value is greater than the previous wheel value</returns>
		public bool MouseIsScrollingUp()
		{
			return _currentMouseState.ScrollWheelValue > _priorMouseState.ScrollWheelValue;
		}

        /// <summary>
        /// Check if the mouse is scrolling down
        /// </summary>
        /// <returns>True if the current wheel value is less than the previous wheel value</returns>
		public bool MouseIsScrollingDown()
		{
			return _currentMouseState.ScrollWheelValue < _priorMouseState.ScrollWheelValue;
		}
        #endregion

        /// <summary>
        /// Check if a GameButton was clicked.  Checks the current mouse state's position to verify that the mouse click happened in the area of the button
        /// </summary>
        /// <param name="b">GameButton to be checked</param>
        /// <returns>True if the mouse was clicked and the click happened in the area of the button</returns>
        public bool GameButtonWasClicked(GameButton b)
        {
            if (MouseButtonWasClicked(MouseButtons.Left))
            {
                if (_currentMouseState.X >= b.Position.X && _currentMouseState.X <= b.Position.X + b.Texture.Width)
                {
                    if (_currentMouseState.Y >= b.Position.Y && _currentMouseState.Y <= b.Position.Y + b.Texture.Height)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Initialize a new InputManager.  Sets the previous and current mouse and keyboard states to the current state of the mouse and keyboard.
        /// </summary>
		public override void Initialize()
		{
			_priorKeyboardState = _currentKeyboardState = Keyboard.GetState();
			_priorMouseState = _currentMouseState = Mouse.GetState();
		}

        /// <summary>
        /// Update mouse and keyboard states and timing values.
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values from game</param>
		public override void Update(GameTime gameTime)
		{
			// Keyboard
			_priorKeyboardState = _currentKeyboardState;
			_currentKeyboardState = Keyboard.GetState();

			foreach (var key in Enum.GetValues(typeof(Keys)))
			{
                if (_currentKeyboardState[(Keys)key] == KeyState.Down)
                {   
                    _keyHeldTimes[(Keys)key] = _keyHeldTimes[(Keys)key] + gameTime.ElapsedGameTime;
                }
                else
                    _keyHeldTimes[(Keys)key] = TimeSpan.Zero;
			}

			// Mouse
            _priorMouseState = _currentMouseState;
			_currentMouseState = Mouse.GetState();

			foreach (var mouseButton in Enum.GetValues(typeof(MouseButtons)))
			{
                if (_mouseButtonMaps[(MouseButtons)mouseButton](_currentMouseState) == ButtonState.Pressed)
                {
                    _mouseButtonHeldTimes[(MouseButtons)mouseButton] += gameTime.ElapsedGameTime;
                    lastButtonPress[(MouseButtons)mouseButton] = gameTime.TotalGameTime;
                }
                else
                    _mouseButtonHeldTimes[(MouseButtons)mouseButton] = TimeSpan.Zero;
			}

			base.Update(gameTime);
		}
	}
}
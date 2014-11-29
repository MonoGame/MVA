using System;
using Windows.Phone.UI.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Wumpus
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private DrawState _drawState;

        private MenuScreen _menuScreen;
        private GameScreen _gameScreen;


        public Game1()
        {
            var graphics = new GraphicsDeviceManager(this);
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            Content.RootDirectory = "Content";

            // Handle the back button ourselves.
            HardwareButtons.BackPressed += OnBackButton;
        }

        private void OnBackButton(object sender, BackPressedEventArgs e)
        {
            if (_gameScreen != null)
            {
                // Ask about quitting the game first.
            }

            e.Handled = true;
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

            // By setting the display size on the touch
            // panel to the virtual display size it will
            // automatically scale the touch input.
            TouchPanel.DisplayWidth = 1920;
            TouchPanel.DisplayHeight = 1080;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _drawState = new DrawState(GraphicsDevice);
            _menuScreen = new MenuScreen(Content, _drawState.ScreenBounds);
            _menuScreen.OnStart += () =>
            {
                _gameScreen = new GameScreen(Content, _drawState.ScreenBounds);
                _gameScreen.OnGameOver += () => _gameScreen = null;
            };
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var touchState = TouchPanel.GetState();

            if (_gameScreen != null)
                _gameScreen.Update(gameTime, touchState);
            else
                _menuScreen.Update(gameTime, touchState);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (_gameScreen != null)
                _gameScreen.Draw(_drawState);
            else
                _menuScreen.Draw(_drawState);

            base.Draw(gameTime);
        }
     }
}

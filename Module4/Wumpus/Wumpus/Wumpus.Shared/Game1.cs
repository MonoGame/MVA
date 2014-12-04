#if WINDOWS_PHONE_APP
using Windows.Phone.UI.Input;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Windows.UI.Popups;
using Microsoft.Xna.Framework.Input;

namespace Wumpus
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private DrawState _drawState;

        // TODO: Point them to game state sample.
        private MenuScreen _menuScreen;
        private GameScreen _gameScreen;


        public Game1()
        {
            var graphics = new GraphicsDeviceManager(this);
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            Content.RootDirectory = "Content";

#if WINDOWS_PHONE_APP
            // Handle the back button ourselves.
            HardwareButtons.BackPressed += OnBackButton;
#endif
        }
#if WINDOWS_PHONE_APP
        private void OnBackButton(object sender, BackPressedEventArgs e)
        {
            if (_gameScreen == null)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                var dlg = new MessageDialog("Are you sure you want to quit the game?", "Quit?");
                dlg.Commands.Add(new UICommand("Yes", command => 
                    {
                        _gameScreen.Cleanup();
                        _gameScreen = null;
                    }));
                dlg.Commands.Add(new UICommand("No"));
                dlg.CancelCommandIndex = 1;
                dlg.ShowAsync();
            }
        }
#endif
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
            TouchPanel.DisplayWidth = _drawState.ScreenBounds.Width;
            TouchPanel.DisplayHeight = _drawState.ScreenBounds.Height;

#if !WINDOWS_PHONE_APP
            IsMouseVisible = true;
#endif
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _drawState = new DrawState(GraphicsDevice);

            _menuScreen = new MenuScreen(Content, _drawState.ScreenBounds);
            _menuScreen.OnStart += OnGameStart;
        }

        protected void OnGameStart()
        {
            if (MediaPlayer.GameHasControl)
                MediaPlayer.Stop();

            _gameScreen = new GameScreen(Content, _drawState.ScreenBounds);
            _gameScreen.OnGameOver += () =>
            {
                _gameScreen.Cleanup();
                _gameScreen = null;
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
                _gameScreen.Draw(gameTime, _drawState);
            else
                _menuScreen.Draw(gameTime, _drawState);

            base.Draw(gameTime);
        }
     }
}

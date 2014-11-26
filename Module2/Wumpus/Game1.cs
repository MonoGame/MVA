using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;

namespace Wumpus
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private Map _map;
        private SpriteFont _hudFont;

        private Texture2D _wallNorthOpen;
        private Texture2D _wallNorthSolid;
        private Texture2D _wallEastOpen;
        private Texture2D _wallEastSolid;
        private Texture2D _wallSouthOpen;
        private Texture2D _wallSouthSolid;
        private Texture2D _wallWestOpen;
        private Texture2D _wallWestSolid;

        private Texture2D _solidWhite;

        private TouchLocation _lastTouch;

        private Rectangle _buttonNorth;
        private Rectangle _buttonEast;
        private Rectangle _buttonSouth;
        private Rectangle _buttonWest;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            Content.RootDirectory = "Content";
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

            var bounds = GraphicsDevice.Viewport.Bounds;

            var buttonWidth = bounds.Width / 4;
            var buttonHeight = bounds.Height / 4;
            _buttonNorth = new Rectangle(buttonWidth, 0, buttonWidth * 2, buttonHeight);
            _buttonEast = new Rectangle(bounds.Right - buttonHeight, buttonHeight, buttonHeight, buttonHeight * 2);
            _buttonSouth = new Rectangle(buttonWidth, bounds.Bottom - buttonHeight, buttonWidth * 2, buttonHeight);
            _buttonWest = new Rectangle(0, buttonHeight, buttonHeight, buttonHeight * 2);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _hudFont = Content.Load<SpriteFont>("Segoe24");

            _wallNorthOpen = Content.Load<Texture2D>("wall-north-open");
            _wallNorthSolid = Content.Load<Texture2D>("wall-north-solid");
            _wallEastOpen = Content.Load<Texture2D>("wall-east-open");
            _wallEastSolid = Content.Load<Texture2D>("wall-east-solid");
            _wallSouthOpen = Content.Load<Texture2D>("wall-south-open");
            _wallSouthSolid = Content.Load<Texture2D>("wall-south-solid");
            _wallWestOpen = Content.Load<Texture2D>("wall-west-open");
            _wallWestSolid = Content.Load<Texture2D>("wall-west-solid");

            _solidWhite = new Texture2D(GraphicsDevice, 2, 2, false, SurfaceFormat.Color);
            _solidWhite.SetData(new [] { Color.White, Color.White, Color.White, Color.White });

            // Setup the map.
            _map = new Map();
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
            if (touchState.Count > 0)
            {
                var t1 = touchState[0];
                if (t1.Id != _lastTouch.Id && t1.State == TouchLocationState.Pressed)
                {
                    if (_buttonNorth.Contains(t1.Position))
                        _map.MovePlayerNorth();
                    else if (_buttonEast.Contains(t1.Position))
                        _map.MovePlayerEast();
                    else if (_buttonSouth.Contains(t1.Position))
                        _map.MovePlayerSouth();
                    else if (_buttonWest.Contains(t1.Position))
                        _map.MovePlayerWest();
                }

                _lastTouch = t1;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var device = GraphicsDevice;
            device.Clear(Color.Black);

            DrawRoom(gameTime);

            DrawHud(gameTime);
            
            base.Draw(gameTime);
        }

        private void DrawRoom(GameTime gameTime)
        {
            var center = GraphicsDevice.Viewport.Bounds.Center.ToVector2();
            spriteBatch.Begin();

            var wallHalfLength = _wallNorthSolid.Width / 2.0f;
            var wallDepth = _wallNorthSolid.Height;

            var room = _map.PlayerRoom;

            spriteBatch.Draw(room.NorthRoom != -1 ? _wallNorthOpen : _wallNorthSolid, new Vector2(center.X - wallHalfLength, center.Y - wallHalfLength));
            spriteBatch.Draw(room.EastRoom != -1 ? _wallEastOpen : _wallEastSolid, new Vector2(center.X + wallHalfLength - wallDepth, center.Y - wallHalfLength));
            spriteBatch.Draw(room.WestRoom != -1 ? _wallWestOpen : _wallWestSolid, new Vector2(center.X - wallHalfLength, center.Y - wallHalfLength));
            spriteBatch.Draw(room.SouthRoom != -1 ? _wallSouthOpen : _wallSouthSolid, new Vector2(center.X - wallHalfLength, center.Y + wallHalfLength - wallDepth));

            spriteBatch.End();
        }

        private void DrawHud(GameTime gameTime)
        {
            spriteBatch.Begin();

            var room = string.Format("ROOM: {0}", _map.PlayerRoomIndex + 1);
            spriteBatch.DrawString(_hudFont, room, new Vector2(20, 10), Color.White);

            spriteBatch.Draw(_solidWhite, _buttonNorth, Color.Red);
            spriteBatch.Draw(_solidWhite, _buttonEast, Color.Green);
            spriteBatch.Draw(_solidWhite, _buttonSouth, Color.Blue);
            spriteBatch.Draw(_solidWhite, _buttonWest, Color.White);

            spriteBatch.End();
        }
    }
}

using System;
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

        private Texture2D _groundTex;

        private Texture2D _playerTex;

        private Texture2D _solidWhite;

        private TouchLocation _lastTouch;

        private Rectangle _buttonNorth;
        private Rectangle _buttonEast;
        private Rectangle _buttonSouth;
        private Rectangle _buttonWest;
        private Texture2D _buttonNorthTex;
        private Texture2D _buttonEastTex;
        private Texture2D _buttonSouthTex;
        private Texture2D _buttonWestTex;

        private readonly Rectangle _screenBounds;
        private readonly Matrix _screenXform;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            Content.RootDirectory = "Content";

            var screenScale = graphics.PreferredBackBufferHeight / 1080.0f;
            _screenXform = Matrix.CreateScale(screenScale, screenScale, 1.0f);

            _screenBounds = new Rectangle(0, 0,
                (int)Math.Round(graphics.PreferredBackBufferWidth / screenScale),
                (int)Math.Round(graphics.PreferredBackBufferHeight / screenScale));
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

            _groundTex = Content.Load<Texture2D>("ground");

            _playerTex = Content.Load<Texture2D>("player");

            _solidWhite = new Texture2D(GraphicsDevice, 2, 2, false, SurfaceFormat.Color);
            _solidWhite.SetData(new [] { Color.White, Color.White, Color.White, Color.White });

            _buttonNorthTex = Content.Load<Texture2D>("ui/button_north");
            _buttonEastTex = Content.Load<Texture2D>("ui/button_east");
            _buttonSouthTex = Content.Load<Texture2D>("ui/button_south");
            _buttonWestTex = Content.Load<Texture2D>("ui/button_west");

            var bounds = _screenBounds;
            _buttonNorth = new Rectangle(bounds.Center.X - (_buttonNorthTex.Width / 2), 0, _buttonNorthTex.Width, _buttonNorthTex.Height);
            _buttonEast = new Rectangle(bounds.Center.X + (_wallNorthSolid.Width / 2) - _buttonEastTex.Width, bounds.Center.Y - (_buttonNorthTex.Height / 2), _buttonEastTex.Width, _buttonNorthTex.Height);
            _buttonSouth = new Rectangle(bounds.Center.X - (_buttonSouthTex.Width / 2), bounds.Bottom - _buttonSouthTex.Height, _buttonSouthTex.Width, _buttonSouthTex.Height);
            _buttonWest = new Rectangle(bounds.Center.X - (_wallNorthSolid.Width / 2), bounds.Center.Y - (_buttonWestTex.Height / 2), _buttonWestTex.Width, _buttonWestTex.Height);

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
            var screen = _screenBounds;
            var center = screen.Center.ToVector2();

            var room = _map.PlayerRoom;

            // Draw the ground.
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null, null, _screenXform);
            spriteBatch.Draw(_groundTex, Vector2.Zero, screen, Color.White);
            spriteBatch.End();

            var roomHalfWidth = _wallNorthSolid.Width / 2.0f;
            var wallDepth = _wallNorthSolid.Height;
            var roomHalfHeight = _wallEastSolid.Height / 2.0f;

            // Draw the room walls.
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _screenXform);
            spriteBatch.Draw(room.NorthRoom != -1 ? _wallNorthOpen : _wallNorthSolid, new Vector2(center.X - roomHalfWidth, center.Y - roomHalfHeight));
            spriteBatch.Draw(room.EastRoom != -1 ? _wallEastOpen : _wallEastSolid, new Vector2(center.X + roomHalfWidth - wallDepth, center.Y - roomHalfHeight));
            spriteBatch.Draw(room.WestRoom != -1 ? _wallWestOpen : _wallWestSolid, new Vector2(center.X - roomHalfWidth, center.Y - roomHalfHeight));
            spriteBatch.Draw(room.SouthRoom != -1 ? _wallSouthOpen : _wallSouthSolid, new Vector2(center.X - roomHalfWidth, center.Y + roomHalfHeight - wallDepth));
            spriteBatch.End();
        }

        private void DrawHud(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _screenXform);

            var room = _map.PlayerRoom;

            var roomDesc = string.Format("ROOM: {0}", room.Index + 1);
            spriteBatch.DrawString(_hudFont, roomDesc, new Vector2(20, 10), Color.White);

            if (room.NorthRoom != -1)
                spriteBatch.Draw(_buttonNorthTex, _buttonNorth, Color.White);
            if (room.EastRoom != -1)
                spriteBatch.Draw(_buttonEastTex, _buttonEast, Color.White);
            if (room.SouthRoom != -1)
                spriteBatch.Draw(_buttonSouthTex, _buttonSouth, Color.White);
            if (room.WestRoom != -1)
                spriteBatch.Draw(_buttonWestTex, _buttonWest, Color.White);

            spriteBatch.End();
        }
    }
}

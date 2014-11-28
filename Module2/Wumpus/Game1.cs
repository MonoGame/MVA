using System;
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
        private Map _map;
        private SpriteFont _hudFont;

        private float _scrollPos;
        private int _scrollOutRoom = -1;
        private Vector2 _scrollOutEnd;
        private int _scrollInRoom = -1;
        private Vector2 _scrollInStart;

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

        private Button _buttonNorth;
        private Button _buttonEast;
        private Button _buttonSouth;
        private Button _buttonWest;

        private readonly Rectangle _screenBounds;
        private readonly Matrix _screenXform;

        SpriteBatch _spriteBatch;

        public Game1()
        {
            var graphics = new GraphicsDeviceManager(this);
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
            _spriteBatch = new SpriteBatch(GraphicsDevice);

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

            var buttonNorthTex = Content.Load<Texture2D>("ui/button_north");
            var buttonEastTex = Content.Load<Texture2D>("ui/button_east");
            var buttonSouthTex = Content.Load<Texture2D>("ui/button_south");
            var buttonWestTex = Content.Load<Texture2D>("ui/button_west");

            _buttonNorth = new Button(buttonNorthTex, _screenBounds.Center.X - (buttonNorthTex.Width / 2), 0);
            _buttonEast = new Button(buttonEastTex, _screenBounds.Center.X + (_wallNorthSolid.Width / 2) - buttonEastTex.Width, _screenBounds.Center.Y - (buttonNorthTex.Height / 2));
            _buttonSouth = new Button(buttonSouthTex, _screenBounds.Center.X - (buttonSouthTex.Width / 2), _screenBounds.Bottom - buttonSouthTex.Height);
            _buttonWest = new Button(buttonWestTex, _screenBounds.Center.X - (_wallNorthSolid.Width / 2), _screenBounds.Center.Y - (buttonWestTex.Height / 2));

            // Setup the map.
            _map = new Map(DateTime.Now.Millisecond);
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
            if (_scrollOutRoom == -1 && _scrollInRoom == -1)
            { 
                var touchState = TouchPanel.GetState();

                if (_buttonNorth.WasPressed(ref touchState))
                {
                    _map.MovePlayerNorth((curr, next) =>
                    {
                        _scrollPos = 0;
                        _scrollOutRoom = curr;
                        _scrollOutEnd = new Vector2(0, 1080);
                        _scrollInRoom = next;
                        _scrollInStart = new Vector2(0, -1080);
                                             
                    });
                }
                else if (_buttonEast.WasPressed(ref touchState))
                {
                    _map.MovePlayerEast((curr, next) =>
                    {
                        _scrollPos = 0;
                        _scrollOutRoom = curr;
                        _scrollOutEnd = new Vector2(-1920, 0);
                        _scrollInRoom = next;
                        _scrollInStart = new Vector2(1920, 0);
                    });
                }
                else if (_buttonSouth.WasPressed(ref touchState))
                {
                    _map.MovePlayerSouth((curr, next) =>
                    {
                        _scrollPos = 0;
                        _scrollOutRoom = curr;
                        _scrollOutEnd = new Vector2(0, -1080);
                        _scrollInRoom = next;
                        _scrollInStart = new Vector2(0, 1080);                                             
                    });
                }
                else if (_buttonWest.WasPressed(ref touchState))
                {
                    _map.MovePlayerWest((curr, next) =>
                    {
                        _scrollPos = 0;
                        _scrollOutRoom = curr;
                        _scrollOutEnd = new Vector2(1920, 0);
                        _scrollInRoom = next;
                        _scrollInStart = new Vector2(-1920, 0);                                            
                    });
                }
            }

            _scrollPos = MathHelper.Clamp(_scrollPos + (float)gameTime.ElapsedGameTime.TotalSeconds * 2.0f, 0, 1);

            if (_scrollOutRoom != -1)
            {
                if (_scrollPos == 1.0f)
                {
                    _scrollOutRoom = -1;
                    _scrollPos = 0;
                }
            }
            else if (_scrollInRoom != -1)
            {
                if (_scrollPos == 1.0f)
                    _scrollInRoom = -1;
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

            DrawRoom();
            DrawHud();
            
            base.Draw(gameTime);
        }

        private void DrawRoom()
        {
            var screen = _screenBounds;
            var center = screen.Center.ToVector2();
            var room = _map.PlayerRoom;
            var ground = Vector2.Zero;

            if (_scrollOutRoom != -1)
            {
                room = _map[_scrollOutRoom];
                var offset = Vector2.Lerp(Vector2.Zero, _scrollOutEnd, _scrollPos);
                center += offset;
                ground += offset;
            }
            else if (_scrollInRoom != -1)
            {
                room = _map[_scrollInRoom];
                var offset = Vector2.Lerp(_scrollInStart, Vector2.Zero, _scrollPos);
                center += offset;
                ground += offset;
            }            

            // Draw the ground.
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null, null, _screenXform);
            _spriteBatch.Draw(_groundTex, ground, screen, Color.White);
            _spriteBatch.End();

            var roomHalfWidth = _wallNorthSolid.Width / 2.0f;
            var wallDepth = _wallNorthSolid.Height;
            var roomHalfHeight = _wallEastSolid.Height / 2.0f;

            // Draw the room walls.
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _screenXform);
            _spriteBatch.Draw(room.NorthRoom != -1 ? _wallNorthOpen : _wallNorthSolid, new Vector2(center.X - roomHalfWidth, center.Y - roomHalfHeight));
            _spriteBatch.Draw(room.EastRoom != -1 ? _wallEastOpen : _wallEastSolid, new Vector2(center.X + roomHalfWidth - wallDepth, center.Y - roomHalfHeight));
            _spriteBatch.Draw(room.WestRoom != -1 ? _wallWestOpen : _wallWestSolid, new Vector2(center.X - roomHalfWidth, center.Y - roomHalfHeight));
            _spriteBatch.Draw(room.SouthRoom != -1 ? _wallSouthOpen : _wallSouthSolid, new Vector2(center.X - roomHalfWidth, center.Y + roomHalfHeight - wallDepth));
            _spriteBatch.End();
        }

        private void DrawHud()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _screenXform);

            var room = _map.PlayerRoom;

            var roomDesc = string.Format("ROOM: {0}", room.Index + 1);
            _spriteBatch.DrawString(_hudFont, roomDesc, new Vector2(20, 10), Color.White);

            // Only draw the movement buttons if the scene is not animating.
            if (_scrollInRoom == -1 && _scrollOutRoom == -1)
            {
                if (room.NorthRoom != -1)
                    _buttonNorth.Draw(_spriteBatch);
                if (room.EastRoom != -1)
                    _buttonEast.Draw(_spriteBatch);
                if (room.SouthRoom != -1)
                    _buttonSouth.Draw(_spriteBatch);
                if (room.WestRoom != -1)
                    _buttonWest.Draw(_spriteBatch);
            }

            _spriteBatch.End();
        }
    }
}

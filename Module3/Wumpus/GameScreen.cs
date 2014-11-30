using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Wumpus
{
    class GameScreen
    {
        private Map _map;

        private SpriteFont _hudFont;
        private SpriteFont _gameOverFont;

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

        private Texture2D _goopTex;
        private Texture2D _alienTex;

        private Texture2D _trapTex;
        private Texture2D _healthTex;

        private Player _player;

        private Button _buttonNorth;
        private Button _buttonEast;
        private Button _buttonSouth;
        private Button _buttonWest;

        private TimeSpan _gameOverTimer;

        public event Action OnGameOver;
        
        public GameScreen(ContentManager content, Rectangle screenBounds)
        {
            _hudFont = content.Load<SpriteFont>("Segoe24");
            _gameOverFont = content.Load<SpriteFont>("GameOver");

            _wallNorthOpen = content.Load<Texture2D>("wall-north-open");
            _wallNorthSolid = content.Load<Texture2D>("wall-north-solid");
            _wallEastOpen = content.Load<Texture2D>("wall-east-open");
            _wallEastSolid = content.Load<Texture2D>("wall-east-solid");
            _wallSouthOpen = content.Load<Texture2D>("wall-south-open");
            _wallSouthSolid = content.Load<Texture2D>("wall-south-solid");
            _wallWestOpen = content.Load<Texture2D>("wall-west-open");
            _wallWestSolid = content.Load<Texture2D>("wall-west-solid");

            _groundTex = content.Load<Texture2D>("ground");

            _goopTex = content.Load<Texture2D>("alien/goop");
            _alienTex = content.Load<Texture2D>("alien/alien");

            _trapTex = content.Load<Texture2D>("trap");

            _healthTex = content.Load<Texture2D>("ui/health");

            var buttonNorthTex = content.Load<Texture2D>("ui/button_north");
            var buttonEastTex = content.Load<Texture2D>("ui/button_east");
            var buttonSouthTex = content.Load<Texture2D>("ui/button_south");
            var buttonWestTex = content.Load<Texture2D>("ui/button_west");

            _buttonNorth = new Button(buttonNorthTex, screenBounds.Center.X - (buttonNorthTex.Width / 2), 0);
            _buttonEast = new Button(buttonEastTex, screenBounds.Center.X + (_wallNorthSolid.Width / 2) - buttonEastTex.Width, screenBounds.Center.Y - (buttonNorthTex.Height / 2));
            _buttonSouth = new Button(buttonSouthTex, screenBounds.Center.X - (buttonSouthTex.Width / 2), screenBounds.Bottom - buttonSouthTex.Height);
            _buttonWest = new Button(buttonWestTex, screenBounds.Center.X - (_wallNorthSolid.Width / 2), screenBounds.Center.Y - (buttonWestTex.Height / 2));

            // Setup the map.
            _map = new Map(DateTime.Now.Millisecond);

            // Setup the player.
            _player = new Player();            
        }

        public void Update(GameTime gameTime, TouchCollection touchState)
        {
            // If the player is dead handle the game over timer.
            if (_player.IsDead)
            {
                _gameOverTimer += gameTime.ElapsedGameTime;
                if (_gameOverTimer > TimeSpan.FromSeconds(5.0f))
                    OnGameOver();
                return;
            }

            // Process input only if we're not scrolling.
            if (_scrollOutRoom == -1 && _scrollInRoom == -1)
            {
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

            // Keep animating the scroll position.
            _scrollPos = MathHelper.Clamp(_scrollPos + (float)gameTime.ElapsedGameTime.TotalSeconds * 2.0f, 0, 1);

            // Manage the scroll state.
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
                {
                    _scrollInRoom = -1;
                    OnEnterRoom();
                }
            }            
        }

        private void OnEnterRoom()
        {
            var room = _map.PlayerRoom;

            if (room.HasTrap)
            {
                _player.Damage();
                room.HasTrap = false;
            }

            if (_map.AlienRoom == room.Index)
            {
                _player.Kill();
            }
        }

        public void Draw(GameTime gameTime, DrawState state)
        {
            DrawRoom(state);

            if (_player.IsDead)
                DrawGameOver(state);
            else
                DrawHud(gameTime, state);            
        }

        private void DrawRoom(DrawState state)
        {
            var screen = state.ScreenBounds;
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
            state.SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null, null, state.ScreenXform);
            state.SpriteBatch.Draw(_groundTex, ground, screen, Color.White);
            state.SpriteBatch.End();

            var roomHalfWidth = _wallNorthSolid.Width / 2.0f;
            var wallDepth = _wallNorthSolid.Height;
            var roomHalfHeight = _wallEastSolid.Height / 2.0f;

            state.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, state.ScreenXform);

            // Is there an alien near by?
            if (_map.IsAlienNear(room.Index))
            {
                // Randomly place some goop on the ground.
                var random = new Random(room.Index);
                var posX = ((float)random.NextDouble() - 0.5f) * _wallNorthSolid.Width * 0.6f;
                var posY = ((float)random.NextDouble() - 0.5f) * _wallEastSolid.Height * 0.6f;
                var rot = (float)random.NextDouble() * MathHelper.TwoPi;
                var half = new Vector2(_goopTex.Width / 2.0f, _goopTex.Height / 2.0f);

                state.SpriteBatch.Draw(_goopTex, center + new Vector2(posX, posY) - half, null, null, half, rot);
            }
            else if (_map.AlienRoom == room.Index)
            {
                // Draw the alien.
                var half = new Vector2(_alienTex.Width / 2.0f, _alienTex.Height / 2.0f);
                state.SpriteBatch.Draw(_alienTex, center - half, Color.White);
            }

            // Draw the room walls.
            state.SpriteBatch.Draw(room.NorthRoom != -1 ? _wallNorthOpen : _wallNorthSolid, new Vector2(center.X - roomHalfWidth, center.Y - roomHalfHeight));
            state.SpriteBatch.Draw(room.EastRoom != -1 ? _wallEastOpen : _wallEastSolid, new Vector2(center.X + roomHalfWidth - wallDepth, center.Y - roomHalfHeight));
            state.SpriteBatch.Draw(room.WestRoom != -1 ? _wallWestOpen : _wallWestSolid, new Vector2(center.X - roomHalfWidth, center.Y - roomHalfHeight));
            state.SpriteBatch.Draw(room.SouthRoom != -1 ? _wallSouthOpen : _wallSouthSolid, new Vector2(center.X - roomHalfWidth, center.Y + roomHalfHeight - wallDepth));
            state.SpriteBatch.End();
        }

        private void DrawHud(GameTime gameTime, DrawState state)
        {
            state.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, state.ScreenXform);

            var room = _map.PlayerRoom;

            // Only draw the movement buttons if the scene is not animating.
            if (_scrollInRoom == -1 && _scrollOutRoom == -1)
            {
                if (room.NorthRoom != -1)
                    _buttonNorth.Draw(state);
                if (room.EastRoom != -1)
                    _buttonEast.Draw(state);
                if (room.SouthRoom != -1)
                    _buttonSouth.Draw(state);
                if (room.WestRoom != -1)
                    _buttonWest.Draw(state);

                if (room.HasTrap)
                    state.SpriteBatch.Draw(_trapTex, state.ScreenBounds.Center.ToVector2(), Color.White);

                if (_map.IsTrapNear(room.Index))
                    state.SpriteBatch.DrawString(_hudFont, "You hear ticking!", new Vector2(20, 108), Color.White);
            }

            // Draw the health.
            if (_player.Health > 0)
                state.SpriteBatch.Draw(_healthTex, new Vector2(20, 10), Color.White);
            if (_player.Health > 1)
                state.SpriteBatch.Draw(_healthTex, new Vector2(20 + _healthTex.Width, 10), Color.White);
            if (_player.Health > 2)
                state.SpriteBatch.Draw(_healthTex, new Vector2(20 + (_healthTex.Width * 2), 10), Color.White);

            // Draw the room identifier.
            var roomDesc = string.Format("ROOM: {0}", room.Index + 1);
            state.SpriteBatch.DrawString(_hudFont, roomDesc, new Vector2(20, 74), Color.White);

            state.SpriteBatch.End();
        }

        private void DrawGameOver(DrawState state)
        {
            state.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, state.ScreenXform);

            var gameOver = "GAME OVER";
            var size = _gameOverFont.MeasureString(gameOver);

            state.SpriteBatch.DrawString(_gameOverFont, gameOver, state.ScreenBounds.Center.ToVector2() - (size / 2), Color.Red);
            state.SpriteBatch.End();
        }
    }
}

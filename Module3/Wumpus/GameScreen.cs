using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Wumpus
{
    class GameScreen
    {
        private Map _map;

        private static readonly Random _random = new Random();

        private Player _player;

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
        private Vector3? _goopPos;

        private Texture2D _alienTex;

        private Texture2D _trapTex;
        private Texture2D _trapWarnTex;
        private Texture2D _healthTex;

        private Texture2D _flamethrowerTex;
        private Texture2D _flamesTex;

        private Texture2D _gameLossTex;
        private Texture2D _gameWinTex;

        private SoundEffect _footstepsSound;
        private SoundEffect _trapHurtSound;
        private SoundEffectInstance _trapNearSound;
        private SoundEffectInstance _alienNearSound;
        private SoundEffect _deathStingSound;
        private SoundEffect _playerDeathAlienSound;
        private SoundEffect _famethrowerStingSound;
        private SoundEffect _famethrowerSound;
        private SoundEffectInstance _shipAmbienceSound;
        private SoundEffect _alienDeathSound;

        private Button _buttonNorth;
        private Button _buttonEast;
        private Button _buttonSouth;
        private Button _buttonWest;

        private bool _attackMode;
        private TimeSpan _attackTimer;
        private float _attackRot;
        private int _attackRoom;

        private Button _flameThrowerButton;
        private Button _attackButtonNorth;
        private Button _attackButtonEast;
        private Button _attackButtonSouth;
        private Button _attackButtonWest;

        private TimeSpan _roomTimer;

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

            _groundTex = content.Load<Texture2D>("floor");

            _goopTex = content.Load<Texture2D>("alien/goop");
            _alienTex = content.Load<Texture2D>("alien/alien");

            _trapTex = content.Load<Texture2D>("trap");
            _trapWarnTex = content.Load<Texture2D>("ui/beep-beep");

            _healthTex = content.Load<Texture2D>("ui/health");

            _flamethrowerTex = content.Load<Texture2D>("flamethrower");
            _flamesTex = content.Load<Texture2D>("flames");

            _gameLossTex = content.Load<Texture2D>("ui/text_gameover");
            _gameWinTex = content.Load<Texture2D>("ui/text_alienterminated");

            _footstepsSound = content.Load<SoundEffect>("sounds/footsteps");
            _trapHurtSound = content.Load<SoundEffect>("sounds/trap_hurt");
            _trapNearSound = content.Load<SoundEffect>("sounds/trap_near").CreateInstance();
            _alienNearSound = content.Load<SoundEffect>("sounds/alien-near").CreateInstance();
            _deathStingSound = content.Load<SoundEffect>("sounds/death-sting");
            _playerDeathAlienSound = content.Load<SoundEffect>("sounds/player-death-alien");
            _famethrowerStingSound = content.Load<SoundEffect>("sounds/flamethrower-pickup");
            _famethrowerSound = content.Load<SoundEffect>("sounds/flamethrower");
            _shipAmbienceSound = content.Load<SoundEffect>("sounds/ship-ambience").CreateInstance();
            _alienDeathSound = content.Load<SoundEffect>("sounds/alien-death");

            { 
                var buttonNorthTex = content.Load<Texture2D>("ui/button_north");
                var buttonEastTex = content.Load<Texture2D>("ui/button_east");
                var buttonSouthTex = content.Load<Texture2D>("ui/button_south");
                var buttonWestTex = content.Load<Texture2D>("ui/button_west");

                _buttonNorth = new Button(buttonNorthTex, screenBounds.Center.X - (buttonNorthTex.Width / 2), 0);
                _buttonEast = new Button(buttonEastTex, screenBounds.Center.X + (_wallNorthSolid.Width / 2) - buttonEastTex.Width, screenBounds.Center.Y - (buttonNorthTex.Height / 2));
                _buttonSouth = new Button(buttonSouthTex, screenBounds.Center.X - (buttonSouthTex.Width / 2), screenBounds.Bottom - buttonSouthTex.Height);
                _buttonWest = new Button(buttonWestTex, screenBounds.Center.X - (_wallNorthSolid.Width / 2), screenBounds.Center.Y - (buttonWestTex.Height / 2));
            }

            { 
                var flameThrowerBtnTex = content.Load<Texture2D>("ui/flamethrower");
                _flameThrowerButton = new Button(flameThrowerBtnTex, screenBounds.Right - flameThrowerBtnTex.Width, screenBounds.Bottom - flameThrowerBtnTex.Height);

                var buttonNorthTex = content.Load<Texture2D>("ui/attack_north");
                var buttonEastTex = content.Load<Texture2D>("ui/attack_east");
                var buttonSouthTex = content.Load<Texture2D>("ui/attack_south");
                var buttonWestTex = content.Load<Texture2D>("ui/attack_west");

                _attackButtonNorth = new Button(buttonNorthTex, screenBounds.Center.X - (buttonNorthTex.Width / 2), 0);
                _attackButtonEast = new Button(buttonEastTex, screenBounds.Center.X + (_wallNorthSolid.Width / 2) - buttonEastTex.Width, screenBounds.Center.Y - (buttonNorthTex.Height / 2));
                _attackButtonSouth = new Button(buttonSouthTex, screenBounds.Center.X - (buttonSouthTex.Width / 2), screenBounds.Bottom - buttonSouthTex.Height);
                _attackButtonWest = new Button(buttonWestTex, screenBounds.Center.X - (_wallNorthSolid.Width / 2), screenBounds.Center.Y - (buttonWestTex.Height / 2));
            }

            // Setup the player.
            _player = new Player();

            // Setup the map.
            _map = new Map(Environment.TickCount);            
            OnEnterRoom();
        }

        public void Cleanup()
        {
            _shipAmbienceSound.Stop();
            _trapNearSound.Stop();
            _alienNearSound.Stop();
        }

        private void PlayFootsteps()
        {
            var volume = MathHelper.Clamp((float)_random.NextDouble(), 0.4f, 0.8f);
            var pitch = ((float)_random.NextDouble() - 0.5f) * 0.25f;
            _footstepsSound.Play(volume, pitch, 0.0f);            
        }

        private bool ShowHud()
        {
            return _scrollInRoom == -1 &&
                    _scrollOutRoom == -1 &&
                    !_map.PlayerRoom.HasTrap &&
                    _map.PlayerRoom.Index != _map.WeaponRoom &&
                    _attackTimer <= TimeSpan.Zero &&
                    _map.AlienRoom != -1 &&
                    !_player.IsDead;
        }

        public void Update(GameTime gameTime, TouchCollection touchState)
        {
            _roomTimer += gameTime.ElapsedGameTime;
            var room = _map.PlayerRoom;

            if (_scrollInRoom == -1)
            {
                // Are we doing the trap animation?
                if (room.HasTrap)
                {
                    if (_roomTimer > TimeSpan.FromSeconds(1.5f))
                    {
                        _player.Damage();
                        room.HasTrap = false;

                        if (_player.IsDead)
                        {
                            _trapNearSound.Stop();
                            _alienNearSound.Stop();
                            _deathStingSound.Play();
                        }
                    }

                    return;
                }

                if (room.Index == _map.WeaponRoom)
                {
                    if (_roomTimer > TimeSpan.FromSeconds(2.5f))
                    {
                        _player.HasWeapon = true;
                        _map.WeaponRoom = -1;
                    }

                    return;   
                }

                // Do the attack logic.
                if (_attackTimer > TimeSpan.Zero)
                {
                    _attackTimer -= gameTime.ElapsedGameTime;

                    if (_attackTimer < TimeSpan.FromSeconds(3.0f) && _map.AlienRoom == _attackRoom)
                    {
                        // Kill the alien!                        
                        _map.AlienRoom = -1;
                        _roomTimer = TimeSpan.Zero;
                        _alienDeathSound.Play();
                        _alienNearSound.Stop();
                    }

                    if (_attackTimer <= TimeSpan.Zero)
                    {
                        // Remove the weapon from the player and replace 
                        // it somewhere in the level.
                        _player.HasWeapon = false;
                        _map.PlaceWeapon();
                        _attackMode = false;
                    }

                    return;
                }
            }


            // If the player is dead handle the game over timer.
            if (_player.IsDead)
            {
                _trapNearSound.Stop();
                _alienNearSound.Stop();

                if (_roomTimer > TimeSpan.FromSeconds(5.0f))
                    OnGameOver();

                return;
            }

            // If the alien is dead then we win.
            if (_map.AlienRoom == -1)
            {
                _trapNearSound.Stop();
                _alienNearSound.Stop();

                if (_roomTimer > TimeSpan.FromSeconds(5.0f))
                    OnGameOver();        
            }

            if (ShowHud())
            {
                if (_player.HasWeapon && _flameThrowerButton.WasPressed(ref touchState))
                    _attackMode = !_attackMode;

                if (_attackMode)
                {
                    if (_attackButtonNorth.WasPressed(ref touchState))
                    {
                        _attackTimer = TimeSpan.FromSeconds(5.0f);
                        _attackRot = MathHelper.TwoPi * 0.25f;
                        _attackRoom = room.NorthRoom;
                        _famethrowerSound.Play();
                    }
                    else if (_attackButtonEast.WasPressed(ref touchState))
                    {
                        _attackTimer = TimeSpan.FromSeconds(5.0f);
                        _attackRot = MathHelper.TwoPi * 0.5f;
                        _attackRoom = room.EastRoom;
                        _famethrowerSound.Play();
                    }
                    else if (_attackButtonSouth.WasPressed(ref touchState))
                    {
                        _attackTimer = TimeSpan.FromSeconds(5.0f);
                        _attackRot = MathHelper.TwoPi * 0.75f;
                        _attackRoom = room.SouthRoom;
                        _famethrowerSound.Play();
                    }
                    else if (_attackButtonWest.WasPressed(ref touchState))
                    {
                        _attackTimer = TimeSpan.FromSeconds(5.0f);
                        _attackRot = MathHelper.TwoPi;
                        _attackRoom = room.WestRoom;
                        _famethrowerSound.Play();
                    }                    
                }
                else
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
                            PlayFootsteps();
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
                            PlayFootsteps();
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
                            PlayFootsteps();
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
                            PlayFootsteps();
                        });
                    }
                }
            }

            // Keep animating the scroll position.
            _scrollPos = MathHelper.Clamp(_scrollPos + (float)gameTime.ElapsedGameTime.TotalSeconds * 2.0f, 0, 1);

            // Manage the scroll state.
            if (_scrollOutRoom != -1)
            {
                if (_scrollPos >= 1.0f)
                    OnEnteringRoom();
            }
            else if (_scrollInRoom != -1)
            {
                _roomTimer = TimeSpan.Zero;
                if (_scrollPos >= 1.0f)
                    OnEnterRoom();
            }
        }

        private void OnEnteringRoom()
        {
            _scrollOutRoom = -1;
            _scrollPos = 0;

            if (!_map.IsAlienNear(_scrollInRoom))
                _goopPos = null;
            else
            {
                // Pick a random position and rotation for goop
                // texture within the room.
                var random = new Random(_scrollInRoom);
                var posX = ((float)random.NextDouble() - 0.5f) * _wallNorthSolid.Width * 0.6f;
                var posY = ((float)random.NextDouble() - 0.5f) * _wallEastSolid.Height * 0.6f;
                var rot = (float)random.NextDouble() * MathHelper.TwoPi;
                _goopPos = new Vector3(posX, posY, rot);
            }
        }

        private void OnEnterRoom()
        {
            _scrollInRoom = -1;
            _scrollPos = 0;

            _roomTimer = TimeSpan.Zero;

            var room = _map.PlayerRoom;

            if (_shipAmbienceSound.State != SoundState.Playing)
            {
                _shipAmbienceSound.IsLooped = true;
                _shipAmbienceSound.Play();
            }

            if (room.HasTrap)
                _trapHurtSound.Play();

            if (_map.IsTrapNear(room.Index))
            {
                _trapNearSound.IsLooped = true;
                _trapNearSound.Play();
            }
            else
                _trapNearSound.Stop();

            if (_map.WeaponRoom == room.Index)
                _famethrowerStingSound.Play();

            if (!_map.IsAlienNear(room.Index))
                _alienNearSound.Stop();
            else
            {
                _alienNearSound.IsLooped = true;
                _alienNearSound.Play();
            }

            if (_map.AlienRoom == room.Index)
            {
                _player.Kill();
                _playerDeathAlienSound.Play();
                _deathStingSound.Play();
            }
        }

        public void Draw(GameTime gameTime, DrawState state)
        {
            DrawRoom(state);

            if (_player.IsDead && _roomTimer > TimeSpan.FromSeconds(2.0f))
                DrawGameOver(state);
            if (_map.AlienRoom == -1)
                DrawGameWin(state);
            else
                DrawHud(gameTime, state);
        }

        private void DrawRoom(DrawState state)
        {
            var screen = state.ScreenBounds;
            var center = screen.Center.ToVector2();
            var room = _map.PlayerRoom;

            if (_scrollOutRoom != -1)
            {
                room = _map[_scrollOutRoom];
                var offset = Vector2.Lerp(Vector2.Zero, _scrollOutEnd, Helpers.EaseInOut(_scrollPos));
                center += offset;
            }
            else if (_scrollInRoom != -1)
            {
                room = _map[_scrollInRoom];
                var offset = Vector2.Lerp(_scrollInStart, Vector2.Zero, Helpers.EaseInOut(_scrollPos));
                center += offset;
            }

            state.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, state.ScreenXform);

            // Draw the ground.
            state.SpriteBatch.Draw(_groundTex, center - _groundTex.GetHalfSize(), Color.White);

            var roomHalfWidth = _wallNorthSolid.Width / 2.0f;
            var wallDepth = _wallNorthSolid.Height;
            var roomHalfHeight = _wallEastSolid.Height / 2.0f;

            // Do we draw the alien goop?
            if (_goopPos.HasValue)
                state.SpriteBatch.Draw(_goopTex, center + new Vector2(_goopPos.Value.X, _goopPos.Value.Y), null, null, _goopTex.GetHalfSize(), _goopPos.Value.Z);

            // Do we draw the alien.
            if (_map.AlienRoom == room.Index)
                state.SpriteBatch.Draw(_alienTex, center - _alienTex.GetHalfSize(), Color.White);
 
            if (room.HasTrap)
            {
                var frameN = MathHelper.Clamp((int)Math.Floor((_roomTimer.TotalSeconds / 1.2f) * 8), 0, 7);
                var frameWidth = _trapTex.Width / 8;
                var frame = new Rectangle(frameN * frameWidth, 0, frameWidth, _trapTex.Height);
                state.SpriteBatch.Draw(_trapTex, center - new Vector2(frameWidth / 2.0f, _trapTex.Height / 2.0f), frame, Color.White);
            }
            
            if (_map.WeaponRoom == room.Index)
                state.SpriteBatch.Draw(_flamethrowerTex, center - _flamethrowerTex.GetHalfSize(), Color.White);

            if (_attackTimer > TimeSpan.FromSeconds(1.25f))
            {
                var frameN = (int)Math.Floor((_attackTimer.TotalSeconds * 6.0f) % 4.0f);
                var frame = new Rectangle(0, frameN * (_flamesTex.Height / 4), _flamesTex.Width, _flamesTex.Height / 4);
                var offset = new Vector2(frame.Right, frame.Height / 2.0f);
                state.SpriteBatch.Draw(_flamesTex, state.ScreenBounds.Center.ToVector2(),
                    frame, Color.White, _attackRot, offset, 1, SpriteEffects.None, 0.0f);                
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

            if (ShowHud())
            {
                if (room.NorthRoom != -1)
                    (_attackMode ? _attackButtonNorth : _buttonNorth).Draw(state);
                if (room.EastRoom != -1)
                    (_attackMode ? _attackButtonEast : _buttonEast).Draw(state);
                if (room.SouthRoom != -1)
                    (_attackMode ? _attackButtonSouth : _buttonSouth).Draw(state);
                if (room.WestRoom != -1)
                    (_attackMode ? _attackButtonWest : _buttonWest).Draw(state);

                if (_player.HasWeapon)
                    _flameThrowerButton.Draw(state);

                if (_map.IsTrapNear(room.Index))
                {
                    var frameN = (int) Math.Floor(_roomTimer.TotalSeconds % 2.0f);
                    var frame = new Rectangle(frameN * (_trapWarnTex.Width / 2), 0, _trapWarnTex.Width / 2, _trapWarnTex.Height);
                    state.SpriteBatch.Draw(_trapWarnTex, new Vector2(state.ScreenBounds.Left, state.ScreenBounds.Bottom - _trapWarnTex.Height), frame, Color.White);
                }
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
            state.SpriteBatch.Draw(_gameLossTex, 
                state.ScreenBounds.Center.ToVector2() - 
                _gameLossTex.GetHalfSize() +
                new Vector2(0, 325), Color.Red);
            state.SpriteBatch.End();
        }

        private void DrawGameWin(DrawState state)
        {
            state.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, state.ScreenXform);
            state.SpriteBatch.Draw(_gameWinTex, state.ScreenBounds.Center.ToVector2() - _gameWinTex.GetHalfSize(), Color.Red);
            state.SpriteBatch.End();
        }
    }
}

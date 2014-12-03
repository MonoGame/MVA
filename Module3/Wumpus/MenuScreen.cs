using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Wumpus
{
    class MenuScreen
    {
        public event Action OnStart;

        private readonly SpriteFont _menuFont;

        private readonly Button _startButton;

        private readonly Song _song;

        private readonly Texture2D _titleTex;

        public MenuScreen(ContentManager content, Rectangle screenBounds)
        {
            _menuFont = content.Load<SpriteFont>("GameOver");

            _titleTex = content.Load<Texture2D>("ui/title_screen");

            _song = content.Load<Song>("music/Blown Away - Menu");

            var center = screenBounds.Center;
            var startTex = content.Load<Texture2D>("ui/start");

            _startButton = new Button(startTex, center.X - (startTex.Width / 2), center.Y + 200);
        }

        public void Update(GameTime gameTime, TouchCollection touchState)
        {
            if (MediaPlayer.State != MediaState.Playing && MediaPlayer.GameHasControl)
                MediaPlayer.Play(_song);

            if (_startButton.WasPressed(ref touchState))
                OnStart();
        }

        public void Draw(GameTime gameTime, DrawState state)
        {
            state.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, state.ScreenXform);

            var center = state.ScreenBounds.Center;

            // Draw the title.
            state.SpriteBatch.Draw(_titleTex, new Vector2(center.X - (_titleTex.Width / 2.0f), center.Y - (_titleTex.Height / 2.0f)), Color.White);

            // Draw the start button blinking.
            if ((gameTime.TotalGameTime.TotalSeconds % 0.9f) > 0.45f)
                _startButton.Draw(state);

            state.SpriteBatch.End();
        }
    }
}


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

        public MenuScreen(ContentManager content, Rectangle screenBounds)
        {
            _menuFont = content.Load<SpriteFont>("GameOver");

            _song = content.Load<Song>("music/Blown Away - Menu");

            var center = screenBounds.Center;
            _startButton = new Button(_menuFont, "Start", center.X, center.Y + 100);
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
            const string title = "ALIEN HUNTER";
            var titleSize = _menuFont.MeasureString(title);
            state.SpriteBatch.DrawString(_menuFont, title, new Vector2(center.X - (titleSize.X / 2.0f), center.Y - titleSize.Y - 100), Color.White);

            // Draw the buttons.
            _startButton.Draw(state);

            state.SpriteBatch.End();
        }
    }
}

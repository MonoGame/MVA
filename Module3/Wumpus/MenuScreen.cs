using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Wumpus
{
    public delegate void OnStartEvent();

    class MenuScreen
    {
        public event OnStartEvent OnStart;

        private readonly SpriteFont _menuFont;

        private readonly Button _startButton;

        public MenuScreen(ContentManager content, Rectangle screenBounds)
        {
            _menuFont = content.Load<SpriteFont>("GameOver");

            var center = screenBounds.Center;
            _startButton = new Button(_menuFont, "Start", center.X, center.Y + 100);
        }

        public void Update(GameTime gameTime, TouchCollection touchState)
        {
            if (_startButton.WasPressed(ref touchState))
                OnStart();
        }

        public void Draw(DrawState state)
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

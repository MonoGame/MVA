using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Wumpus
{
    class Button
    {
        private Rectangle _location;

        private readonly Texture2D _pressTex;

        private readonly SpriteFont _font;
        private readonly string _text;

        private int _lastTouchId;
        private bool _pressed;

        public Button(SpriteFont font, string text, int posX, int posY)
        {
            _font = font;
            _text = text;
            var size = _font.MeasureString(text);
            _location = new Rectangle(posX - (int)(size.X / 2.0f), posY - (int)(size.Y / 2.0f), (int)size.X, (int)size.Y);
        }

        public Button(Texture2D pressTex, int posX, int posY)
        {
            _pressTex = pressTex;
            _location = new Rectangle(posX, posY, pressTex.Width, pressTex.Height);
        }

        public bool WasPressed(ref TouchCollection touches)
        {
            foreach (var touch in touches)
            {
                if (touch.Id == _lastTouchId)
                    continue;                
                if (touch.State != TouchLocationState.Pressed)
                    continue;

                if (_location.Contains(touch.Position))
                {
                    _lastTouchId = touch.Id;
                    _pressed = true;
                    return true;
                }                    
            }

            _pressed = false;

            return false;
        }

        public void Draw(DrawState state)
        {
            if (_pressTex != null)
                state.SpriteBatch.Draw(_pressTex, _location, _pressed ? Color.DarkGray : Color.White);
            else
                state.SpriteBatch.DrawString(_font, _text, new Vector2(_location.X, _location.Y),  _pressed ? Color.DarkGray : Color.White);
        }
    }
}

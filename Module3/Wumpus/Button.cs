using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Wumpus
{
    class Button
    {
        private Rectangle _location;
        private readonly Texture2D _pressTex;
        private int _lastTouchId;
        private bool _pressed;

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

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_pressTex, _location, _pressed ? Color.DarkGray : Color.White);
        }
    }
}

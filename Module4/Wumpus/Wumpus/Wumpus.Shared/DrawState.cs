using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wumpus
{
    class DrawState
    {
        public readonly SpriteBatch SpriteBatch;
        public readonly Rectangle ScreenBounds;
        public readonly Matrix ScreenXform;

        public DrawState(GraphicsDevice device)
        {
            SpriteBatch = new SpriteBatch(device);

            var screenScale = device.PresentationParameters.BackBufferHeight / 1080.0f;
            ScreenXform = Matrix.CreateScale(screenScale, screenScale, 1.0f);

            ScreenBounds = new Rectangle(0, 0,
                (int)Math.Round(device.PresentationParameters.BackBufferWidth / screenScale),
                (int)Math.Round(device.PresentationParameters.BackBufferHeight / screenScale));
        }
    }
}

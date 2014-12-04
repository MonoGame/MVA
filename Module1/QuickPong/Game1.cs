using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
namespace QuickPong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Vector2 _ballPosition;
        Vector2 _ballDirection;
        Vector2 _paddlePosition;
        Texture2D _singlePixelSprite;
        float _speed = 0.1f;

        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            

            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Make our texture a single white pixel
            _singlePixelSprite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            _singlePixelSprite.SetData<Color>(new[] { Color.White });

            _ballPosition = graphics.GraphicsDevice.Viewport.Bounds.Center.ToVector2();
            _ballDirection = new Vector2(-1, -1);

            _paddlePosition = new Vector2(_ballPosition.X, graphics.GraphicsDevice.Viewport.Bounds.Bottom - 20);

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
            // TODO: Add your update logic here

            base.Update(gameTime);

            //Move paddle - to position of 1st touch point in any
            var touchState = TouchPanel.GetState();
            if (touchState.Count > 0)
            {
                _paddlePosition.X = touchState[0].Position.X - 20;
            }

            //Move ball
            _ballPosition += _ballDirection * (float)(gameTime.ElapsedGameTime.TotalMilliseconds * _speed);

            //bounce
            var bounds = graphics.GraphicsDevice.Viewport.Bounds;
            if (_ballPosition.X < bounds.Left + 10 || _ballPosition.X > bounds.Right - 10)
            {
                _ballDirection.X = -_ballDirection.X;
                _speed *= 1.02f;
            }
            if (_ballPosition.Y < bounds.Top + 10 || 
                (_ballPosition.Y > _paddlePosition.Y && _ballPosition.Y < _paddlePosition.Y + 10 && _ballPosition.X > _paddlePosition.X -20 && _ballPosition.X < _paddlePosition.X +20))
            {
                _ballDirection.Y = -_ballDirection.Y;
                _speed *= 1.02f;
            }

            //Reset
            if (_ballPosition.Y > bounds.Bottom)
            {
                _ballPosition = graphics.GraphicsDevice.Viewport.Bounds.Center.ToVector2();
            }


        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(_singlePixelSprite, _ballPosition, null, Color.Red, 0, Vector2.Zero, new Vector2(10, 10), SpriteEffects.None, 0);
            spriteBatch.Draw(_singlePixelSprite, _paddlePosition, null, Color.Blue, 0, Vector2.Zero, new Vector2(40, 10), SpriteEffects.None, 0);
            spriteBatch.End();
 

            base.Draw(gameTime);
        }
    }
}

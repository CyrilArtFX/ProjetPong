using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PongGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public int screenSizeX;
        public int screenSizeY;
        public float mousePositionY;

        Texture2D ballTexture;
        Vector2 ballPosition;
        public int ballSizeX;
        public int ballSizeY;

        Texture2D leftBarTexture;
        Vector2 leftBarPosition;
        public int leftBarSizeX;
        public int leftBarSizeY;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            screenSizeX = this.GraphicsDevice.Viewport.Width;
            screenSizeY = this.GraphicsDevice.Viewport.Height;

            ballSizeX = 14;
            ballSizeY = 14;
            ballPosition = new Vector2(screenSizeX / 2 - ballSizeX / 2, screenSizeY / 2 - ballSizeY / 2);
            ballTexture = new Texture2D(this.GraphicsDevice, ballSizeX, ballSizeY);
            Color[] ballColorData = new Color[ballSizeX * ballSizeY];
            for (int i = 0; i < ballSizeX * ballSizeY; i++)
                ballColorData[i] = Color.White;
            ballTexture.SetData<Color>(ballColorData);

            leftBarSizeX = 20;
            leftBarSizeY = 150;
            leftBarPosition = new Vector2(screenSizeX / 17 - leftBarSizeX / 2, screenSizeY / 2 - leftBarSizeY / 2);
            leftBarTexture = new Texture2D(this.GraphicsDevice, leftBarSizeX, leftBarSizeY);
            Color[] leftBarColorData = new Color[leftBarSizeX * leftBarSizeY];
            for (int i = 0; i < leftBarSizeX * leftBarSizeY; i++)
                leftBarColorData[i] = Color.White;
            leftBarTexture.SetData<Color>(leftBarColorData);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                /*ballPosition.X += 1;
                if (ballPosition.X > this.GraphicsDevice.Viewport.Width)
                    ballPosition.X = 0;*/
                mousePositionY = Mouse.GetState().Y;
                leftBarPosition.Y = mousePositionY - ballSizeY / 2;
                if (leftBarPosition.Y < 0) leftBarPosition.Y = 0;
                if (leftBarPosition.Y + leftBarSizeY > screenSizeY) leftBarPosition.Y = screenSizeY - leftBarSizeY;


                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(ballTexture, ballPosition, Color.White);
            _spriteBatch.Draw(leftBarTexture, leftBarPosition, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using SharpDX.XInput;
using System;
using System.Collections.Generic;

namespace PongGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public int screenSizeX;
        public int screenSizeY;
        public float mousePositionY;

        public string gameState; //"menu" = base state when launching, or after a game is finish
                                 //"game" = when the ball is moving
                                 //"score" = when a player score a point, showing the score and waiting for the player to click on the screen to continue

        public int leftPlayerScore;
        public int rightPlayerScore;

        Texture2D ballTexture;
        Vector2 ballPosition;
        public float ballSpeed;
        public float effectiveBallSpeedX;
        public float effectiveBallSpeedY;
        public int ballSizeX;
        public int ballSizeY;

        Texture2D leftBarTexture;
        Vector2 leftBarPosition;
        public int leftBarSizeX;
        public int leftBarSizeY;

        Texture2D rightBarTexture;
        Vector2 rightBarPosition;
        public int rightBarSizeX;
        public int rightBarSizeY;
        public int rightBarSpeed;

        SpriteFont fontLarge;
        SpriteFont fontSmall;

        private int frameCounter;
        private bool isFrameCounterLaunched;
        private bool canClick;

        List<SoundEffect> soundEffects;



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            soundEffects = new List<SoundEffect>();
        }

        protected override void Initialize()
        {
            screenSizeX = this.GraphicsDevice.Viewport.Width;
            screenSizeY = this.GraphicsDevice.Viewport.Height;
            
            //Initiate ball's datas
            ballSizeX = 14;
            ballSizeY = 14;
            ballPosition = new Vector2(screenSizeX / 2 - ballSizeX / 2, screenSizeY / 2 - ballSizeY / 2);
            ballSpeed = 2.5f;
            effectiveBallSpeedX = ballSpeed;
            effectiveBallSpeedY = ballSpeed;
            ballTexture = new Texture2D(this.GraphicsDevice, ballSizeX, ballSizeY);
            Color[] ballColorData = new Color[ballSizeX * ballSizeY];
            for (int i = 0; i < ballSizeX * ballSizeY; i++)
                ballColorData[i] = Color.White;
            ballTexture.SetData<Color>(ballColorData);

            //Initiate left bar's datas
            leftBarSizeX = 20;
            leftBarSizeY = 120;
            leftBarPosition = new Vector2(screenSizeX / 17 - leftBarSizeX / 2, screenSizeY / 2 - leftBarSizeY / 2);
            leftBarTexture = new Texture2D(this.GraphicsDevice, leftBarSizeX, leftBarSizeY);
            Color[] leftBarColorData = new Color[leftBarSizeX * leftBarSizeY];
            for (int i = 0; i < leftBarSizeX * leftBarSizeY; i++)
                leftBarColorData[i] = Color.White;
            leftBarTexture.SetData<Color>(leftBarColorData);

            //Initiate right bar's datas
            rightBarSizeX = 20;
            rightBarSizeY = 120;
            rightBarPosition = new Vector2(screenSizeX - (screenSizeX / 17 + rightBarSizeX / 2), screenSizeY / 2 - rightBarSizeY / 2);
            rightBarTexture = new Texture2D(this.GraphicsDevice, rightBarSizeX, rightBarSizeY);
            Color[] rightBarColorData = new Color[rightBarSizeX * rightBarSizeY];
            for (int i = 0; i < rightBarSizeX * rightBarSizeY; i++)
                rightBarColorData[i] = Color.White;
            rightBarTexture.SetData<Color>(rightBarColorData);
            rightBarSpeed = 3;

            //Initiate other datas

            ReturnMenu(false);

            frameCounter = 0;
            isFrameCounterLaunched = false;
            canClick = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            fontLarge = Content.Load<SpriteFont>("PixeledFontLarge");
            fontSmall = Content.Load<SpriteFont>("PixeledFontSmall");

            soundEffects.Add(Content.Load<SoundEffect>("PongSounds/Bar")); // [0]
            soundEffects.Add(Content.Load<SoundEffect>("PongSounds/UpAndDown"));// [1]
            soundEffects.Add(Content.Load<SoundEffect>("PongSounds/Win"));// [2]
            soundEffects.Add(Content.Load<SoundEffect>("PongSounds/Defeat"));// [3]
            soundEffects.Add(Content.Load<SoundEffect>("PongSounds/Menu"));// [4]
            soundEffects.Add(Content.Load<SoundEffect>("PongSounds/Win3"));// [5]
            soundEffects.Add(Content.Load<SoundEffect>("PongSounds/Defeat3"));// [6]
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                if(isFrameCounterLaunched)
                {
                    frameCounter++;
                }
                if(frameCounter >= 80)
                {
                    canClick = true;
                    isFrameCounterLaunched = false;
                    frameCounter = 0;
                }

                //Move the ball
                if(gameState == "game")
                {
                    ballPosition.X += effectiveBallSpeedX;
                    ballPosition.Y += effectiveBallSpeedY;
                }

                //Detect when the ball is out
                if (ballPosition.X + ballSizeX >= screenSizeX) //The ball is at the right edge
                {
                    leftPlayerScore++;
                    if (leftPlayerScore < 3) soundEffects[2].Play();
                    else soundEffects[5].Play();
                    ShowScores();
                }
                if (ballPosition.X <= 0) //The ball is at the left edge
                {
                    rightPlayerScore++;
                    if (rightPlayerScore < 3) soundEffects[3].Play();
                    else soundEffects[6].Play();
                    ShowScores();
                }

                //Make the ball bounce of the bar
                if (ballPosition.X <= leftBarPosition.X + leftBarSizeX && ballPosition.X + ballSizeX >= leftBarPosition.X && ballPosition.Y >= leftBarPosition.Y && ballPosition.Y + ballSizeY <= leftBarPosition.Y + leftBarSizeY)
                {
                    ballSpeed += 0.5f;
                    effectiveBallSpeedX = ballSpeed;
                    effectiveBallSpeedY = effectiveBallSpeedY - (((leftBarPosition.Y + leftBarSizeY) - (ballPosition.Y + ballSizeY)) / 200) * ballSpeed;
                    soundEffects[0].Play();
                }
                if (ballPosition.X + ballSizeX >= rightBarPosition.X && ballPosition.X <= rightBarPosition.X + rightBarSizeX && ballPosition.Y >= rightBarPosition.Y && ballPosition.Y + ballSizeY <= rightBarPosition.Y + rightBarSizeY)
                {
                    ballSpeed += 0.5f;
                    effectiveBallSpeedX = -ballSpeed;
                    effectiveBallSpeedY = effectiveBallSpeedY - (((rightBarPosition.Y + rightBarSizeY) - (ballPosition.Y + ballSizeY)) / 200) * ballSpeed;
                    soundEffects[0].Play();
                }

                //Make the ball bounce of the screen (up and down)
                if (ballPosition.Y + ballSizeY >= screenSizeY)
                {
                    effectiveBallSpeedY = -effectiveBallSpeedY;
                    soundEffects[1].Play();
                }
                if (ballPosition.Y <= 0)
                {
                    effectiveBallSpeedY = -effectiveBallSpeedY;
                    soundEffects[1].Play();
                }

                //Limit the ball speed
                if (ballSpeed >= 19.5f) ballSpeed = 19f;

                //Move the left bar
                mousePositionY = Mouse.GetState().Y;
                if(gameState == "game")
                {
                    leftBarPosition.Y = mousePositionY - leftBarSizeY / 2;
                    if (leftBarPosition.Y < 0) leftBarPosition.Y = 0;
                    if (leftBarPosition.Y + leftBarSizeY > screenSizeY) leftBarPosition.Y = screenSizeY - leftBarSizeY;
                }

                //Move the right bar
                rightBarSpeed = 3 + leftPlayerScore;
                if(rightBarPosition.Y + rightBarSizeY / 2 > ballPosition.Y + ballSizeY / 2)
                {
                    if((rightBarPosition.Y + rightBarSizeY / 2) - (ballPosition.Y + ballSizeY / 2) > rightBarSpeed)
                    {
                        rightBarPosition.Y -= rightBarSpeed;
                    }
                    else
                    {
                        rightBarPosition.Y -= (rightBarPosition.Y + rightBarSizeY / 2) - (ballPosition.Y + ballSizeY / 2);
                    }
                    
                }
                if (rightBarPosition.Y + rightBarSizeY / 2 < ballPosition.Y + ballSizeY / 2)
                {
                    if ((ballPosition.Y + ballSizeY / 2) - (rightBarPosition.Y + rightBarSizeY / 2) > rightBarSpeed)
                    {
                        rightBarPosition.Y += rightBarSpeed;
                    }
                    else
                    {
                        rightBarPosition.Y += (ballPosition.Y + ballSizeY / 2) - (rightBarPosition.Y + rightBarSizeY / 2);
                    }
                }
                if (rightBarPosition.Y < 0) rightBarPosition.Y = 0;
                if (rightBarPosition.Y + rightBarSizeY > screenSizeY) rightBarPosition.Y = screenSizeY - rightBarSizeY;

                if(gameState == "score" || gameState == "menu")
                {
                    ButtonState mouseLeftButtonState = Mouse.GetState().LeftButton;
                    if(mouseLeftButtonState.Equals(ButtonState.Pressed) && canClick)
                    {
                        if((gameState == "score" && leftPlayerScore < 3 && rightPlayerScore < 3) || gameState == "menu")
                        {
                            StartGame();
                        }
                        else
                        {
                            ReturnMenu(true);
                        }
                    }
                }

                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            if(gameState == "game" || gameState == "score")
            {
                _spriteBatch.Draw(ballTexture, ballPosition, Color.White);
                _spriteBatch.Draw(leftBarTexture, leftBarPosition, Color.White);
                _spriteBatch.Draw(rightBarTexture, rightBarPosition, Color.White);
            }
            if(gameState == "score")
            {
                _spriteBatch.DrawString(fontLarge, leftPlayerScore + " - " + rightPlayerScore, new Vector2(0.5f * screenSizeX - 65, 0.08f * screenSizeY), Color.White);
                if(leftPlayerScore >= 3)
                {
                    _spriteBatch.DrawString(fontSmall, "LEFT PLAYER WIN !", new Vector2(0.5f * screenSizeX - 180, 0.6f * screenSizeY), Color.White);
                    _spriteBatch.DrawString(fontSmall, "CLICK TO QUIT", new Vector2(0.5f * screenSizeX - 140, 0.8f * screenSizeY), Color.White);
                }
                else if (rightPlayerScore >= 3)
                {
                    _spriteBatch.DrawString(fontSmall, "RIGHT PLAYER WIN !", new Vector2(0.5f * screenSizeX - 180, 0.6f * screenSizeY), Color.White);
                    _spriteBatch.DrawString(fontSmall, "CLICK TO QUIT", new Vector2(0.5f * screenSizeX - 140, 0.8f * screenSizeY), Color.White);
                }
                else
                {
                    _spriteBatch.DrawString(fontSmall, "CLICK TO CONTINUE", new Vector2(0.5f * screenSizeX - 180, 0.7f * screenSizeY), Color.White);
                }
            }
            if(gameState == "menu")
            {
                _spriteBatch.DrawString(fontSmall, "PONG", new Vector2(0.5f * screenSizeX - 40, 0.2f * screenSizeY), Color.White);
                _spriteBatch.DrawString(fontSmall, "CLICK TO START", new Vector2(0.5f * screenSizeX - 155, 0.6f * screenSizeY), Color.White);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void StartGame()
        {
            ballPosition = new Vector2(screenSizeX / 2 - ballSizeX / 2, screenSizeY / 2 - ballSizeY / 2);
            leftBarPosition = new Vector2(screenSizeX / 17 - leftBarSizeX / 2, screenSizeY / 2 - leftBarSizeY / 2);
            rightBarPosition = new Vector2(screenSizeX - (screenSizeX / 17 + rightBarSizeX / 2), screenSizeY / 2 - rightBarSizeY / 2);
            ballSpeed = 2.5f;
            Random random = new Random();
            effectiveBallSpeedX = (((random.Next(0, 2)) * 2) - 1) * ballSpeed;
            effectiveBallSpeedY = (((random.Next(0, 2)) * 2) - 1) * ballSpeed;
            canClick = false;
            soundEffects[4].Play();
            gameState = "game";
        }

        private void ShowScores()
        {
            gameState = "score";
            ballPosition = new Vector2(screenSizeX / 2 - ballSizeX / 2, screenSizeY / 2 - ballSizeY / 2);
            leftBarPosition = new Vector2(screenSizeX / 17 - leftBarSizeX / 2, screenSizeY / 2 - leftBarSizeY / 2);
            rightBarPosition = new Vector2(screenSizeX - (screenSizeX / 17 + rightBarSizeX / 2), screenSizeY / 2 - rightBarSizeY / 2);
            canClick = true;
        }

        private void ReturnMenu(bool playSound)
        {
            if(playSound) soundEffects[4].Play();
            gameState = "menu";
            leftPlayerScore = 0;
            rightPlayerScore = 0;
            canClick = false;
            isFrameCounterLaunched = true;
        }
    }
}

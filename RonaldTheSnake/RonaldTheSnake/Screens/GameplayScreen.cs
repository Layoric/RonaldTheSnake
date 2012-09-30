using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework.Input;

using RonaldTheSnake;
using RonaldTheSnake.World;

namespace RonaldTheSnake.Screens
{
    class GameplayScreen : GameScreen
    {
        SnakeWorld world;
        MessageBoxScreen msg;

        public GameplayScreen()
        {
            //TransitionOnTime = TimeSpan.FromSeconds(0.0);
            //TransitionOffTime = TimeSpan.FromSeconds(0.0);

            //Accelerometer = new Accelerometer();
            //if (Accelerometer.State == SensorState.Ready)
            //{
            //    Accelerometer.ReadingChanged += (s, e) =>
            //    {
            //        accelState = e;
            //    };
            //    Accelerometer.Start();
            //}
            
        }

        public override void LoadContent()
        {
            //gameplayHelper = new GameplayHelper(ScreenManager.Game.Content, ScreenManager.SpriteBatch,
            //    ScreenManager.Game.GraphicsDevice);

            //gameplayHelper.LoadContent();

            //gameplayHelper.Start();
            //world = new SnakeWorld(ScreenManager);
            //world.Intialize();
            //world.Load(ScreenManager.Game.Content);

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            //gameplayHelper.UnloadContent();

            base.UnloadContent();
        }

        /// <summary>
        /// Runs one frame of update for the game.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsActive)
            {
                //world.Update(gameTime);
                //gameplayHelper.Update(elapsedSeconds);
            }
            if (world.worldReset)
            {
                if (msg == null)
                    msg = new GameOverBox("You died!!");

                ScreenManager.AddScreen(msg, ControllingPlayer);
                world.worldReset = false;
                
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Draw the game world, effects, and HUD
        /// </summary>
        /// <param name="gameTime">The elapsed time since last Draw</param>
        public override void Draw(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            world.Draw(gameTime);
            //gameplayHelper.Draw(elapsedSeconds);
        }        

        /// <summary>
        /// Input helper method provided by GameScreen.  Packages up the various input
        /// values for ease of use.  Here it checks for pausing and handles controlling
        /// the player's tank.
        /// </summary>
        /// <param name="input">The state of the gamepads</param>
        public override void HandleInput(InputState input)
        {
            //Vector3 accelerationInfo = accelState == null ? Vector3.Zero : 
            //    new Vector3((float)accelState.X, (float)accelState.Y, (float)accelState.Z);

            //if (gameplayHelper.HandleInput(input.PauseGame, accelerationInfo, TouchPanel.GetState()))
            //{
            //    FinishCurrentGame();
            //}
            
        }

        private void FinishCurrentGame()
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            ScreenManager.AddScreen(new BackgroundScreen(),null);
            ScreenManager.AddScreen(new MainMenuScreen(),null);
        }
    }    
}

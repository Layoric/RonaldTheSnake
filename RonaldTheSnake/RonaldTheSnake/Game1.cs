using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RonaldTheSnake.Screens;

namespace RonaldTheSnake
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            //Set the Windows Phone screen resolution
#if WIN8
            graphics.PreferredBackBufferWidth = 1376;
            graphics.PreferredBackBufferHeight = 768;
            //graphics.PreferredBackBufferWidth = Window.ClientBounds.Right + 1;
            //graphics.PreferredBackBufferHeight = Window.ClientBounds.Bottom + 1;
#else
            //Default
            graphics.PreferredBackBufferWidth = 1376;
            graphics.PreferredBackBufferHeight = 768;
#endif

            //graphics.IsFullScreen = false;
            
            
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 60.0f);
            IsFixedTimeStep = true;

            //Create a new instance of the Screen Manager
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            AddInitialScreens();
        }

        private void AddInitialScreens()
        {
            screenManager.screens.Clear();
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
        }

    }
}

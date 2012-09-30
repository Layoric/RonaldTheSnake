//-----------------------------------------------------------------------------
// ScreenManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;
using System.Threading;

namespace RonaldTheSnake.Screens
{
    /// <summary>
    /// The screen manager is a component which manages one or more GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        #region Fields

        //Debug fields
        //public DebugSystem debugSystem;
        Vector2 debugPos = new Vector2(100, 100);
        Stopwatch stopwatch = new Stopwatch();
        //End debug fields

        public List<GameScreen> screens = new List<GameScreen>();
        List<GameScreen> screensToUpdate = new List<GameScreen>();
        List<GameScreen> tempScreensList = new List<GameScreen>();

        public InputState input = new InputState();

        SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D blankTexture;

        public Thread LoadingThread;
        public delegate void LoadGameAsyncDelegate(string fileName);

        bool isInitialized;

        bool traceEnabled;

        #endregion

        #region Temp Fields

        Viewport FBB_viewport;
        Rectangle viewPortRect;

        #endregion

        #region Properties


        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public bool SaveScreenRequired
        {
            get;
            set;
        }

        public Texture2D BlankTexture
        {
            get { return blankTexture; }
        }

        /// <summary>
        /// A default font shared by all the screens. This saves
        /// each screen having to bother loading their own local copy.
        /// </summary>
        public SpriteFont Font
        {
            get { return font; }
        }


        /// <summary>
        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled
        {
            get { return traceEnabled; }
            set { traceEnabled = value; }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager(Game game)
            : base(game)
        {
            // we must set EnabledGestures before we can query for them, but
            // we don't assume the game wants to read them.
            TouchPanel.EnabledGestures = GestureType.None;
            //adManager = new AdManager(game, "test_client");
            //adManager.TestMode = true;
            //game.Components.Add(adManager);
        }


        /// <summary>
        /// Initializes the screen manager component.
        /// </summary>
        public override void Initialize()
        {
            // initialize the debug system with the game and the name of the font 
            // we want to use for the debugging
            //debugSystem = DebugSystem.Initialize(this.Game, "menufont");


            //// register a new command that lets us move a sprite on the screen
            //debugSystem.DebugCommandUI.RegisterCommand(
            //    "pos",              // Name of command
            //    "set position",     // Description of command
            //    PosCommand          // Command execution delegate
            //    );

            // we use flick and tap in order to bring down the command prompt and
            // open up the keyboard input panel, respectively.
            //TouchPanel.EnabledGestures = GestureType.Flick | GestureType.Tap;

            base.Initialize();

            isInitialized = true;
        }

        ///// <summary>
        ///// This method is called from DebugCommandHost when the user types the 'pos'
        ///// command into the command prompt. This is registered with the command prompt
        ///// through the DebugCommandUI.RegisterCommand method we called in Initialize.
        ///// </summary>
        //void PosCommand(IDebugCommandHost host, string command, IList<string> arguments)
        //{
        //    // if we got two arguments from the command
        //    if (arguments.Count == 2)
        //    {
        //        // process text "pos xPos yPos" by parsing our two arguments
        //        debugPos.X = Single.Parse(arguments[0], CultureInfo.InvariantCulture);
        //        debugPos.Y = Single.Parse(arguments[1], CultureInfo.InvariantCulture);
        //    }
        //    else
        //    {
        //        // if we didn't get two arguments, we echo the current position of the cat
        //        host.Echo(String.Format("Pos={0},{1}", debugPos.X, debugPos.Y));
        //    }
        //}


        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = content.Load<SpriteFont>("menufont");
            blankTexture = content.Load<Texture2D>("blank");
            //myAd = adManager.CreateAd("Image300_50", new Rectangle(0, 20, GraphicsDevice.Viewport.Bounds.Width, 120), RotationMode.Manual, false);


            // Tell each of the screens to load their content.
            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }

            base.LoadContent();
        }


        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // tell the TimeRuler that we're starting a new frame. you always want
            // to call this at the start of Update
            //debugSystem.TimeRuler.StartFrame();

            // Start measuring time for "Update".
            //debugSystem.TimeRuler.BeginMark("Update", Color.Black);

            //Input here
            // Read the keyboard and gamepad.
            //Debug input should be removed when debug is not needed.
            input.Update();
            //HandleTouchInput();

            // Simulate game update by doing a busy loop for 1ms
            stopwatch.Reset();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < 1) ;

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            screensToUpdate.Clear();

            foreach (GameScreen screen in screens)
                screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];
                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(input);

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            // Print debug trace?
            if (traceEnabled)
                TraceScreens();

            base.Update(gameTime);

            // Stop measuring time for "Update".
            //debugSystem.TimeRuler.EndMark("Update");
        }

        void HandleTouchInput()
        {

        }

        public void LoadGame(string fileName)
        {

        }

        public void LoadGameAsync(string fileName)
        {

        }


        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (GameScreen screen in screens)
                screenNames.Add(screen.GetType().Name);

            Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
        }


        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            //debugSystem.TimeRuler.BeginMark("Draw", Color.Yellow);
            foreach (GameScreen screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
            spriteBatch.Begin();



            // Show usage.
            string message =
                "A Button, A key: Show/Hide FPS Counter\n" +
                "B Button, B key: Show/Hide Time Ruler\n" +
                "X Button, X key: Show/Hide Time Ruler Log\n" +
                "Tab key, flick down: Open debug command UI\n" +
                "Tab key, flick up: Close debug command UI\n" +
                "Tap: Show keyboard input panel";

            Vector2 size = font.MeasureString(message);
            //Layout layout = new Layout(GraphicsDevice.Viewport);

            float margin = font.LineSpacing;
            Rectangle rc = new Rectangle(0, 0,
                                    (int)(size.X + margin),
                                    (int)(size.Y + margin));

            spriteBatch.End();

            // Draw other components.
            base.Draw(gameTime);

            // Stop measuring time for "Draw".
            //debugSystem.TimeRuler.EndMark("Draw");
        }


        #endregion

        #region Public Methods


        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (isInitialized)
            {
                screen.LoadContent();
            }

            screens.Add(screen);

            // update the TouchPanel to respond to gestures this screen is interested in
            TouchPanel.EnabledGestures = screen.EnabledGestures;
        }


        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {

            // If we have a graphics device, tell the screen to unload content.
            if (isInitialized)
            {
                screen.UnloadContent();
            }

            screens.Remove(screen);
            screensToUpdate.Remove(screen);

            // if there is a screen still in the manager, update TouchPanel
            // to respond to gestures that screen is interested in.
            if (screens.Count > 0)
            {
                TouchPanel.EnabledGestures = screens[screens.Count - 1].EnabledGestures;
            }
        }


        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }


        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(float alpha)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(blankTexture, GraphicsDevice.Viewport.Bounds, Color.Black * alpha);

            spriteBatch.End();
        }

        /// <summary>
        /// Informs the screen manager to serialize its state to disk.
        /// </summary>
        public void Deactivate()
        {
#if !WINDOWS_PHONE
            return;
#else
                // Save the document
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // if our screen manager directory already exists, delete the contents
                if (storage.DirectoryExists("ScreenManager"))
                {
                    DeleteState(storage);
                }

                // otherwise just create the directory
                else
                {
                    storage.CreateDirectory("ScreenManager");
                }

                // create a file we'll use to store the list of screens in the stack
                using (IsolatedStorageFileStream stream = storage.CreateFile("ScreenManager\\ScreenList.dat"))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        // write out the full name of all the types in our stack so we can
                        // recreate them if needed.
                        foreach (GameScreen screen in screens)
                        {
                            if (screen.IsSerializable)
                            {
                                writer.Write(screen.GetType().AssemblyQualifiedName);
                            }
                        }
                    }
                }

                // now we create a new file stream for each screen so it can save its state
                // if it needs to. we name each file "ScreenX.dat" where X is the index of
                // the screen in the stack, to ensure the files are uniquely named
                int screenIndex = 0;
                foreach (GameScreen screen in screens)
                {
                    if (screen.IsSerializable)
                    {
                        string fileName = string.Format("ScreenManager\\Screen{0}.dat", screenIndex);

                        // open up the stream and let the screen serialize whatever state it wants
                        using (IsolatedStorageFileStream stream = storage.CreateFile(fileName))
                        {
                            screen.Serialize(stream);
                        }

                        screenIndex++;
                    }
                }

                this.AddScreen(new SettingsMessageBox("Paused"), PlayerIndex.One);
            }
#endif
        }

        public bool Activate(bool instancePreserved)
        {
#if !WINDOWS_PHONE
            return false;
#else
            if (!instancePreserved)
            {
                // open up isolated storage
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    // see if our saved state directory exists
                    if (storage.DirectoryExists("ScreenManager"))
                    {
                        try
                        {
                            // see if we have a screen list
                            if (storage.FileExists("ScreenManager\\ScreenList.dat"))
                            {
                                // load the list of screen types
                                using (IsolatedStorageFileStream stream = storage.OpenFile("ScreenManager\\ScreenList.dat", FileMode.Open, FileAccess.Read))
                                {
                                    using (BinaryReader reader = new BinaryReader(stream))
                                    {
                                        if (reader.BaseStream.Length == 0)
                                            throw new Exception();
                                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                                        {
                                            // read a line from our file
                                            string line = reader.ReadString();

                                            // if it isn't blank, we can create a screen from it
                                            if (!string.IsNullOrEmpty(line))
                                            {
                                                Type screenType = Type.GetType(line);
                                                GameScreen screen = Activator.CreateInstance(screenType) as GameScreen;
                                                AddScreen(screen, PlayerIndex.One);
                                            }
                                        }
                                    }
                                }
                            }

                            // next we give each screen a chance to deserialize from the disk
                            for (int i = 0; i < screens.Count; i++)
                            {
                                string filename = string.Format("ScreenManager\\Screen{0}.dat", i);
                                using (IsolatedStorageFileStream stream = storage.OpenFile(filename, FileMode.Open, FileAccess.Read))
                                {
                                    screens[i].Deserialize(stream);
                                }
                            }

                            return true;
                        }
                        catch (Exception)
                        {
                            // if an exception was thrown while reading, odds are we cannot recover
                            // from the saved state, so we will delete it so the game can correctly
                            // launch.
                            DeleteState(storage);
                        }
                    }
                }
            }

            return true;
#endif
        }

        #endregion

        /// <summary>
        /// Deletes the saved state files from isolated storage.
        /// </summary>
        //private void DeleteState(IsolatedStorageFile storage)
        //{
        //    // get all of the files in the directory and delete them
        //    string[] files = storage.GetFileNames("ScreenManager\\*");
        //    foreach (string file in files)
        //    {
        //        storage.DeleteFile(Path.Combine("ScreenManager", file));
        //    }
        //}


    }
}

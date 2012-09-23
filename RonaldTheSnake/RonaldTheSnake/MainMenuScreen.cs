using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RonaldTheSnake;
using Microsoft.Xna.Framework;

namespace RonaldTheSnake
{
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        GameplayScreen gamePlayScreen;
        //LoadGameScreen loadGameScreen;
        MenuEntry resumeGameMenuEntry;
        string resumeFilename = string.Empty;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Ronald the Snake!")
        {
            bool displayResume = false;
            //SaveAndLoad sl = new SaveAndLoad();
            //if (sl.SaveGames.Count > 0)
            //{
            //    displayResume = true;
            //    SaveGame sg = sl.LastSaveGame();
            //    resumeFilename = sg.FileName;
            //}
            // Create our menu entries.
            if (displayResume)
                resumeGameMenuEntry = new MenuEntry("Resume Game");
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            MenuEntry loadGameMenuEntry = new MenuEntry("Load Game");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");

            // Hook up menu event handlers.
            if (displayResume)
                resumeGameMenuEntry.Selected += ResumeGameMenuEntrySelected;
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            loadGameMenuEntry.Selected += LoadGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;

            // Add entries to the menu.
            if (displayResume)
                MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(playGameMenuEntry);

            MenuEntries.Add(loadGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
        }

        void ResumeGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (gamePlayScreen == null)
                gamePlayScreen = new GameplayScreen();
            gamePlayScreen.WorldFileName = resumeFilename;
            gamePlayScreen.LoadWorld = true;

            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               gamePlayScreen);
        }

        void LoadGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //if (loadGameScreen == null)
            //    loadGameScreen = new LoadGameScreen();

            //ScreenManager.AddScreen(loadGameScreen, e.PlayerIndex);
        }

        //protected void AsyncLoadCallBack(object sender, AsyncCompletedEventArgs e)
        //{

        //}


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (gamePlayScreen == null)
                gamePlayScreen = new GameplayScreen();

            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               gamePlayScreen);
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }



        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}

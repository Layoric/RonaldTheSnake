using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RonaldTheSnake;
using Microsoft.Xna.Framework;

namespace RonaldTheSnake.Screens
{
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        
        //LoadGameScreen loadGameScreen;
        MenuEntry resumeGameMenuEntry;
        PlayModeMenuScreen playModeMenuScreen;
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
            MenuEntry playGameMenuEntry = new MenuEntry("Play");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry quitEntry = new MenuEntry("Quit");

            // Hook up menu event handlers.
            if (displayResume)
                resumeGameMenuEntry.Selected += ResumeGameMenuEntrySelected;
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            quitEntry.Selected += quitEntry_Selected;

            // Add entries to the menu.
            if (displayResume)
                MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(quitEntry);
        }

        void quitEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        void ResumeGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //if (gamePlayScreen == null)
            //    gamePlayScreen = new GameplayScreen();
            //gamePlayScreen.WorldFileName = resumeFilename;
            //gamePlayScreen.LoadWorld = true;

            //LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
            //                   gamePlayScreen);
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (playModeMenuScreen == null)
                playModeMenuScreen = new PlayModeMenuScreen();

            ScreenManager.AddScreen(playModeMenuScreen, e.PlayerIndex);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RonaldTheSnake;
using Microsoft.Xna.Framework;
using RonaldTheSnake.World;

namespace RonaldTheSnake.Screens
{
    class PlayModeMenuScreen : MenuScreen
    {
        MenuEntry playArcade;
        MenuEntry playPuzzle;
        MenuEntry playTimed;
        PuzzleModeMenuScreen puzzleScreen;

        SnakeWorld gamePlayScreen;

        public PlayModeMenuScreen(): base("Game Mode")
        {
            //Init
            playArcade = new MenuEntry("Arcade");
            playPuzzle = new MenuEntry("Puzzle");
            playTimed = new MenuEntry("Timed");

            //Wire up
            playArcade.Selected += playArcade_Selected;
            playPuzzle.Selected += playPuzzle_Selected;
            playTimed.Selected += playTimed_Selected;

            //add
            MenuEntries.Add(playArcade);
            MenuEntries.Add(playPuzzle);
            MenuEntries.Add(playTimed);

        }

        void playTimed_Selected(object sender, PlayerIndexEventArgs e)
        {
            if (gamePlayScreen == null)
                gamePlayScreen = new SnakeWorld("test1");

            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               gamePlayScreen);
        }

        void playPuzzle_Selected(object sender, PlayerIndexEventArgs e)
        {
            puzzleScreen = new PuzzleModeMenuScreen();
            ScreenManager.AddScreen(puzzleScreen, e.PlayerIndex);
        }

        void playArcade_Selected(object sender, PlayerIndexEventArgs e)
        {
            if (gamePlayScreen == null)
                gamePlayScreen = new SnakeWorld("test1");

            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               gamePlayScreen);
        }
    }

}
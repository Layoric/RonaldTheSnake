using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RonaldTheSnake.World;
using RonandTheSnake.CustomDataTypes;

namespace RonaldTheSnake.Screens
{
    class LevelCompleteBox : MessageBoxScreen
    {
        SnakeWorld gamePlayScreen;

        public LevelCompleteBox(string message)
            : base(message)
        {
            MenuEntry cancelENtry = new MenuEntry("Cancel");
            MenuEntry OkEntry = new MenuEntry("Confirm");

            Accepted += LevelCompleteBox_Accepted;
            Cancelled += LevelCompleteBox_Cancelled;
        }

        void LevelCompleteBox_Cancelled(object sender, PlayerIndexEventArgs e)
        {
            foreach (var screen in ScreenManager.GetScreens())
            {
                screen.ExitScreen();
            }

            ScreenManager.AddScreen(new BackgroundScreen(), null);
            ScreenManager.AddScreen(new MainMenuScreen(), null);
        }

        void LevelCompleteBox_Accepted(object sender, PlayerIndexEventArgs e)
        {
            string currentLevelName = string.Empty;
            List<LevelMenu> allLevels = new List<LevelMenu>(SnakeHelper.GetAllLevels(ScreenManager.Game.Content));

            foreach (var screen in ScreenManager.GetScreens())
            {
                if (screen is SnakeWorld)
                {
                    currentLevelName = (screen as SnakeWorld).TiledMapName;
                }
                screen.ExitScreen();
            }

            LevelMenu nextLevel = null;
            LevelMenu currentLevel = allLevels.Where(x => x.MapFileName == currentLevelName).FirstOrDefault();
            int currentIndex = allLevels.IndexOf(currentLevel);
            if (currentIndex < allLevels.Count - 1)
                nextLevel = allLevels[currentIndex + 1];

            gamePlayScreen = new SnakeWorld(nextLevel.MapFileName);

            ScreenManager.AddScreen(gamePlayScreen, ControllingPlayer);
        }
    }
}

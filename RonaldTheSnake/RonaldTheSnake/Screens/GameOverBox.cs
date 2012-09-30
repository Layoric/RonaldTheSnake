using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RonaldTheSnake.World;

namespace RonaldTheSnake.Screens
{
    class GameOverBox : MessageBoxScreen
    {
        SnakeWorld gamePlayScreen;

        public GameOverBox(string message) : base(message)
        {
            MenuEntry cancelENtry = new MenuEntry("Cancel");
            MenuEntry OkEntry = new MenuEntry("Confirm");

            

            Accepted += GameOverBox_Accepted;
            Cancelled += GameOverBox_Cancelled;
        }

        void GameOverBox_Cancelled(object sender, PlayerIndexEventArgs e)
        {
            foreach (var screen in ScreenManager.GetScreens())
            {
                screen.ExitScreen();
            }

            ScreenManager.AddScreen(new BackgroundScreen(), null);
            ScreenManager.AddScreen(new MainMenuScreen(), null);
        }

        void GameOverBox_Accepted(object sender, PlayerIndexEventArgs e)
        {
            string currentLevelName = string.Empty;
            foreach (var screen in ScreenManager.GetScreens())
            {
                if (screen is SnakeWorld)
                {
                    currentLevelName = (screen as SnakeWorld).TiledMapName;
                }
                screen.ExitScreen();
            }

            if (gamePlayScreen == null)
                gamePlayScreen = new SnakeWorld(currentLevelName);

            ScreenManager.AddScreen(gamePlayScreen,ControllingPlayer);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RonaldTheSnake
{
    class GameOverBox : MessageBoxScreen
    {
        GameplayScreen gamePlayScreen;

        public GameOverBox(string message) : base(message)
        {
            Accepted += new EventHandler<PlayerIndexEventArgs>(GameOverBox_Accepted);
            Cancelled += new EventHandler<PlayerIndexEventArgs>(GameOverBox_Cancelled);
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
            foreach (var screen in ScreenManager.GetScreens())
            {
                screen.ExitScreen();
            }

            if (gamePlayScreen == null)
                gamePlayScreen = new GameplayScreen();

            ScreenManager.AddScreen(gamePlayScreen,ControllingPlayer);
        }
    }
}

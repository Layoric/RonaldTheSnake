using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RonaldTheSnake.World;
using System.Xml;
using Microsoft.Xna.Framework.Content;
using RonandTheSnake.CustomDataTypes;
using Microsoft.Xna.Framework.Graphics;

namespace RonaldTheSnake.Screens
{
    class PuzzleModeMenuScreen : LevelChoiceMenuScreen
    {
        List<MenuEntry> allLevels = new List<MenuEntry>();
        LevelMenu[] allLevelMenus;
        SnakeWorld gamePlayScreen;

        public PuzzleModeMenuScreen() : base("Puzzle Mode")
        {
            
        }

        public override void LoadContent()
        {
            allLevelMenus = SnakeHelper.GetAllLevels(ScreenManager.Game.Content);

            foreach (LevelMenu level in allLevelMenus)
            {
                LevelMenuEntry menuLevel = new LevelMenuEntry(level.MapFileName,level.MenuName);
                menuLevel.Preview = ScreenManager.Game.Content.Load<Texture2D>(level.MenuImageFile);
                menuLevel.Source = menuLevel.Preview.Bounds;
                menuLevel.Selected += menuLevel_Selected;
                MenuEntries.Add(menuLevel);
            }
            base.LoadContent();
        }

        void menuLevel_Selected(object sender, PlayerIndexEventArgs e)
        {
            string fileName = "";
            if (sender.GetType() == typeof(LevelMenuEntry))
            {
                fileName = (sender as LevelMenuEntry).FileName;
            }
            gamePlayScreen = new SnakeWorld(fileName);
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               gamePlayScreen);
        }
    }
}

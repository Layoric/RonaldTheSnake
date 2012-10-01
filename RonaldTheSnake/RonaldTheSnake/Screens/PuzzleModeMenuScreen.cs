using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RonaldTheSnake.World;
using System.Xml;
using Microsoft.Xna.Framework.Content;
using RonandTheSnake.CustomDataTypes;

namespace RonaldTheSnake.Screens
{
    class PuzzleModeMenuScreen : MenuScreen
    {
        List<MenuEntry> allLevels = new List<MenuEntry>();
        LevelMenu[] allLevelMenus;
        SnakeWorld gamePlayScreen;

        public PuzzleModeMenuScreen() : base("Puzzle Mode")
        {
            
        }

        public override void LoadContent()
        {
            allLevelMenus = ScreenManager.Game.Content.Load<LevelMenu[]>("Levels");

            foreach (LevelMenu level in allLevelMenus)
            {
                LevelMenuEntry menuLevel = new LevelMenuEntry(level.MenuName, level.MapFileName);
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

    class LevelMenuEntry : MenuEntry
    {
        public string FileName { get; set; }

        public LevelMenuEntry(string menuText,string fileName) : base(menuText)
        {
            FileName = fileName;
        }
    }
}

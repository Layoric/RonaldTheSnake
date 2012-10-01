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

namespace RonandTheSnake.CustomDataTypes
{
    public class LevelMenu
    {
        public string MapFileName { get; set; }
        public string MenuName { get; set; }
        public string MenuImageFile { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RonaldTheSnake.SnakeObjects
{
    public class SnakeFood
    {
        public int PointsValue = 0;

        public Point Position { get; set; }
        public Texture2D Texture { get; set; }

        public int RemainingTime { get; set; }

        public SnakeFood(int pointVal)
        {
            PointsValue = pointVal;
        }
    }
}

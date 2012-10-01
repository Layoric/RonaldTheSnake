using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RonaldTheSnake.SnakeObjects
{
    public class SnakeFood
    {
        public int PointsValue = 0;

        public Point Position { get; set; }

        public SnakeFood(int pointVal)
        {
            PointsValue = pointVal;
        }
    }
}

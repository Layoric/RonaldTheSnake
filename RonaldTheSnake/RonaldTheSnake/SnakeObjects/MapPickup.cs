using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RonaldTheSnake.SnakeObjects
{
    public class MapPickup
    {
        public int PointsValue = 0;

        public Point Position { get; set; }

        public MapPickup(int pointVal)
        {
            PointsValue = pointVal;
        }
    }
}

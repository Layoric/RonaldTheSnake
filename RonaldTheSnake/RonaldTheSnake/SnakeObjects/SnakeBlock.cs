using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RonaldTheSnake.SnakeObjects
{
    public class SnakeBlock
    {
        
        public SnakeDirection Direction { get; set; }
        public SnakeDirection PreviousDirection { get; set; }
        public int SnakeBodyIndex { get; set; }
        SnakeBody Body { get; set; }

        public Point Position { get; set; }

        public SnakeBlock()
        {
            
        }
    }
}

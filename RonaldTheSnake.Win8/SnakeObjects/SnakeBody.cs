using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RonaldTheSnake.SnakeObjects
{
    public class SnakeBody
    {
        public List<SnakeBlock> Blocks { get; set; }
        
        public Texture2D Texture { get; set; }
        public Rectangle Source { get; set; }

        public SnakeBody()
        {
            Blocks = new List<SnakeBlock>();            
        }

        

    }

    public class SnakeHead
    {
        public Texture2D Texture { get; set; }

        public Rectangle Source { get; set; }
        public Point Position { get; set; }

        public SnakeHead(Point pos)
        {
            this.Position = pos;
        }
    }
}

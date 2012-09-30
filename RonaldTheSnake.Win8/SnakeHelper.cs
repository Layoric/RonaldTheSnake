using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using RonaldTheSnake.SnakeObjects;
using Microsoft.Xna.Framework.Graphics;

namespace RonaldTheSnake
{
    public class SnakeHelper
    {
        public static Point GetNewBlockPoint(SnakePlayer player)
        {
            Point result = new Point();
            SnakeDirection dir = player.Body.Blocks.Count > 0 ? player.Body.Blocks.Last().Direction : player.Direction;
            Point lastPos = player.Body.Blocks.Count > 0 ? player.Body.Blocks.Last().Position : player.Head.Position;
            switch (dir)
            {
                case SnakeDirection.Up:
                    result = new Point(lastPos.X, lastPos.Y + 1);
                    break;
                case SnakeDirection.Right:
                    result = new Point(lastPos.X - 1, lastPos.Y);
                    break;
                case SnakeDirection.Down:
                    result = new Point(lastPos.X, lastPos.Y - 1);
                    break;
                case SnakeDirection.Left:
                    result = new Point(lastPos.X + 1, lastPos.Y);
                    break;
                default:
                    break;
            }

            return result;
        }

        public static Vector2 MapToScreen(Point point, Texture2D texture)
        {
            Vector2 result = new Vector2((point.X * 32) + (texture.Bounds.Width / 2), (point.Y * 32) + (texture.Bounds.Height / 2));
            return result;
        }

    }
}

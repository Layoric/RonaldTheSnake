using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using RonaldTheSnake.SnakeObjects;
using Microsoft.Xna.Framework.Graphics;
using RonaldTheSnake.Screens;

namespace RonaldTheSnake
{
    public class SnakeHelper
    {
        static GameOverBox gameOver;
        static GameOverBox arcadeTimeUp;
        public static Point offset;
        static bool init = false;

        public static void Init(GraphicsDevice device)
        {
            if (!init)
            {
                offset = new Point((device.DisplayMode.Width - 1024) / 2,
                                (device.DisplayMode.Height - 576) / 4);
            }
        }

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

        public static Vector2 MapToScreen(Point point, Texture2D texture, GraphicsDevice device)
        {
            if (!init)
            {
                offset = new Point((device.DisplayMode.Width - 1024) / 2,
                                (device.DisplayMode.Height - 576) / 4);
            }
            Vector2 result = new Vector2((point.X * 32) + (texture.Bounds.Width / 2) + offset.X, 
                (point.Y * 32) + (texture.Bounds.Height / 2) + offset.Y);
            return result;
        }

        public static void ShowGameOver(ScreenManager screenManager, PlayerIndex? playerIndex)
        {
            if(gameOver == null)
                gameOver = new GameOverBox("You died!!");

            screenManager.AddScreen(gameOver, playerIndex);     
        }

        public static void ShowArcadeTimeUp(ScreenManager screenManager, PlayerIndex? playerIndex, int score)
        {
            arcadeTimeUp = new GameOverBox("Time up!" + Environment.NewLine + "Score: " + score.ToString());

            screenManager.AddScreen(arcadeTimeUp, playerIndex);
        }

    }
}

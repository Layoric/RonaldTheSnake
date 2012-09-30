using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using RonaldTheSnake.SnakeObjects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RonaldTheSnake.Templates
{
    public class SnakeDefaultTemplate : ISnakeTemplate
    {
        private ContentManager Content;
        private ScreenManager ScreenManager;
        public SnakeDefaultTemplate(ScreenManager screenManager)
        {
            ScreenManager = screenManager;
            Content = screenManager.Game.Content;
        }

        public SnakePlayer BuildSnake()
        {
            SnakePlayer player = new SnakePlayer(ScreenManager);
            player.Speed = 50f;
            player.Direction = SnakeDirection.Down;
            player.PreviousDirection = SnakeDirection.Down;
            player.Name = "Default";
            player.Head = new SnakeHead(new Point(5, 5));
            player.Head.Texture = Content.Load<Texture2D>("snakehead");
            player.Head.Source = player.Head.Texture.Bounds;
            player.Body = new SnakeBody();
            player.Body.Texture = Content.Load<Texture2D>("snakeblock");
            player.Body.Source = player.Body.Texture.Bounds;
            
            for (int i = 0; i < 5; i++)
            {
                player.AddSnakeBlock();
            }

            return player;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RonaldTheSnake.Screens;

namespace RonaldTheSnake.SnakeObjects
{
    public class SnakePlayer
    {
        public SnakeHead Head { get; set; }
        public float Speed { get; set; }
        public SnakeBody Body { get; set; }
        public string Name { get; set; }
        private ScreenManager ScreenManager;
        private SpriteBatch SpriteBatch;
        double elapsedGameTime = 0.0f;
        public int Score = 0;

        public SnakePlayer(ScreenManager screenManager)
        {
            ScreenManager = screenManager;
            SpriteBatch = screenManager.SpriteBatch;
        }

        public SnakeDirection Direction { get; set; }
        public SnakeDirection PreviousDirection { get; set; }

        public void Update(GameTime gameTime)
        {
            elapsedGameTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedGameTime > (10000 / Speed))
            {
                UpdateHead();
                UpdateBody();
                elapsedGameTime = 0f;
            }
        }

        public void AddSnakeBlock()
        {
            SnakeBlock block = new SnakeBlock();
            block.SnakeBodyIndex = Body.Blocks.Count - 1;
            block.Position = SnakeHelper.GetNewBlockPoint(this);
            block.Direction = Body.Blocks.Count > 0 ? Body.Blocks.Last().Direction : Direction;
            Body.Blocks.Add(block);
        }

        public void Draw(GameTime gameTime)
        {
            DrawHead();
            Vector2 origin = new Vector2(Body.Texture.Width / 2, Body.Texture.Height / 2);
            foreach (var block in Body.Blocks)
            {
                SpriteBatch.Draw(this.Body.Texture, SnakeHelper.MapToScreen(block.Position, this.Body.Texture, ScreenManager.GraphicsDevice),
                        Body.Source,
                        Color.White,
                        MathHelper.ToRadians(0f),
                        origin,
                        1.0f,
                        SpriteEffects.None,
                        0.0f);
            }
        }

        private void DrawHead()
        {
            Vector2 origin = new Vector2(Head.Texture.Width / 2, Head.Texture.Height / 2);
            switch (Direction)
            {
                case SnakeDirection.Up:
                    SpriteBatch.Draw(Head.Texture, SnakeHelper.MapToScreen(Head.Position, Head.Texture, ScreenManager.GraphicsDevice),
                        Head.Source,
                        Color.White,
                        MathHelper.ToRadians(0f),
                        origin,
                        1.0f,
                        SpriteEffects.None,
                        0.0f);
                    break;
                case SnakeDirection.Right:
                    SpriteBatch.Draw(Head.Texture, SnakeHelper.MapToScreen(Head.Position, Head.Texture, ScreenManager.GraphicsDevice), 
                        Head.Source, 
                        Color.White, 
                        MathHelper.ToRadians(90f),
                        origin,
                        1.0f,
                        SpriteEffects.None,
                        0.0f);
                    break;
                case SnakeDirection.Down:
                    SpriteBatch.Draw(Head.Texture, SnakeHelper.MapToScreen(Head.Position, Head.Texture, ScreenManager.GraphicsDevice),
                        Head.Source,
                        Color.White,
                        MathHelper.ToRadians(180f),
                        origin,
                        1.0f,
                        SpriteEffects.None,
                        0.0f);
                    break;
                case SnakeDirection.Left:
                    SpriteBatch.Draw(Head.Texture, SnakeHelper.MapToScreen(Head.Position, Head.Texture, ScreenManager.GraphicsDevice),
                        Head.Source,
                        Color.White,
                        MathHelper.ToRadians(270f),
                        origin,
                        1.0f,
                        SpriteEffects.None,
                        0.0f);
                    break;
                default:
                    break;
            }
        }

        private void UpdateHead()
        {
            PreviousDirection = Direction;
            switch (Direction)
            {
                case SnakeDirection.Up:
                    Head.Position = new Point(Head.Position.X,Head.Position.Y - 1);
                    break;
                case SnakeDirection.Right:
                    Head.Position = new Point(Head.Position.X + 1,Head.Position.Y);
                    break;
                case SnakeDirection.Down:
                    Head.Position = new Point(Head.Position.X,Head.Position.Y + 1);
                    break;
                case SnakeDirection.Left:
                    Head.Position = new Point(Head.Position.X - 1, Head.Position.Y);
                    break;
                default:
                    break;
            }
        }

        private void UpdateBody()
        {
            for (int i = Body.Blocks.Count - 1; i >= 0; --i)
            {
                SnakeBlock block = Body.Blocks[i];
                block.PreviousDirection = Direction;
                
                switch (block.Direction)
                {
                    case SnakeDirection.Up:
                        block.Position = new Point(block.Position.X, block.Position.Y - 1);
                        break;
                    case SnakeDirection.Right:
                        block.Position = new Point(block.Position.X + 1, block.Position.Y);
                        break;
                    case SnakeDirection.Down:
                        block.Position = new Point(block.Position.X, block.Position.Y + 1);
                        break;
                    case SnakeDirection.Left:
                        block.Position = new Point(block.Position.X - 1, block.Position.Y);
                        break;
                    default:
                        break;
                }
                if (i > 0)
                {
                    block.Direction = Body.Blocks[i - 1].Direction;
                }
                else
                {
                    block.Direction = PreviousDirection;
                }
            }
        }
    }
}

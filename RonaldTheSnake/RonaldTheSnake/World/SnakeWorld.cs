using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RonaldTheSnake.SnakeObjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using RonaldTheSnake.Factories;
using RonaldTheSnake.Templates;
using Microsoft.Xna.Framework.Graphics;

namespace RonaldTheSnake.World
{
    public class SnakeWorld
    {
        public SnakeLevel CurrentLevel { get; set; }
        public List<SnakePlayer> Players = new List<SnakePlayer>();
        const int worldSize = 64;
        GridMap map = new GridMap(worldSize);
        Random randomDropTime = new Random();
        double timeForNextDrop = 0;
        double elapsedTimed = 0;
        SpriteFont gameFont;
        public bool worldReset = false;
        
         

        ScreenManager ScreenManager;

        public SnakeWorld(ScreenManager screenManager)
        {
            this.ScreenManager = screenManager;
        }

        public void Intialize()
        {
            SnakeFactory.RegisterTemplate("default", new SnakeDefaultTemplate(ScreenManager));
            
        }

        public void Load(ContentManager content)
        {
            Players.Add(SnakeFactory.CreateFromTemplate("default"));
            gameFont = ScreenManager.Game.Content.Load<SpriteFont>("gamefont");
            
        }

        public void Update(GameTime gameTime)
        {
            if (timeForNextDrop == 0)
            {
                timeForNextDrop = randomDropTime.Next(1, 2000);
            }
            foreach(var player in Players)
            {
                player.Update(gameTime);
                if (player.Head.Position.Y > worldSize - 1 ||
                    player.Head.Position.X > worldSize - 1 ||
                    player.Head.Position.Y < 0 ||
                    player.Head.Position.X < 0)
                {

                    
                    worldReset = true;
                }
                else
                {
                    MapCell currentCell = map.Cells[player.Head.Position.X][player.Head.Position.Y];
                    if (currentCell.ContainsPickup)
                    {
                        player.Score += currentCell.Pickup.PointsValue;
                        player.AddSnakeBlock();
                        currentCell.Pickup = null;
                    }
                }
            }

            elapsedTimed += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTimed > timeForNextDrop)
            {
                elapsedTimed = 0;
                timeForNextDrop = 0;
                Random randX = new Random(DateTime.Now.Millisecond);
                Random randY = new Random(DateTime.Now.Millisecond + 5);
                map.Cells[randX.Next(0, worldSize - 1)][randY.Next(0, worldSize - 1)].Pickup = new MapPickup(50);
            }


        }

        public void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.DrawString(gameFont, Players[0].Score.ToString(), new Vector2(12, 12), Color.White);

            Vector2 origin = new Vector2(Players[0].Body.Texture.Width / 2, Players[0].Body.Texture.Height / 2);
            int x = 0;
            
            foreach (var cell in map.Cells)
            {
                
                int y = 0;
                foreach (var otherCell in cell)
                {
                    
                    if (otherCell.ContainsPickup)
                    {
                        ScreenManager.SpriteBatch.Draw(Players[0].Body.Texture, SnakeHelper.MapToScreen(new Point(x,y)),
                        Players[0].Body.Source,
                        Color.White,
                        MathHelper.ToRadians(180f),
                        origin,
                        1.0f,
                        SpriteEffects.None,
                        0.0f);
                    }
                    y++;
                }
                x++;
            }

            foreach (var player in Players)
            {
                player.Draw(gameTime);
            }
            ScreenManager.SpriteBatch.End();
        }
    }

    public class SnakeLevel
    {
        public SnakeMap CurrentMap { get; set; }
    }

    public class SnakeMap
    {
        //Tiles
        //Basically Tiled data.
    }

    
}

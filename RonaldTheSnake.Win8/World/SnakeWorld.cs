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
using FuncWorks.XNA.XTiled;

namespace RonaldTheSnake.World
{
    public class SnakeWorld
    {
        public SnakeLevel CurrentLevel { get; set; }
        public List<SnakePlayer> Players = new List<SnakePlayer>();
        Map tiledMap = new Map();
        GridMap map = new GridMap(40,22);
        Random randomDropTime = new Random();
        double timeForNextDrop = 0;
        double elapsedTimed = 0;
        SpriteFont gameFont;
        public bool worldReset = false;
        int foodCount = 0;
        int foodCap = 4;
        MapPickup pickUp;
        List<MapCell> cellsInUse = new List<MapCell>();
         

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
            tiledMap = content.Load<Map>("test1");
            int tileYIndex = tiledMap.TileLayers["Environment"].Tiles[0].Count();
            int tileXIndex = tiledMap.TileLayers["Environment"].Tiles.Count();
            for (int y = 0; y < tileYIndex; y++)
            {
                
                for (int x = 0; x < tileXIndex; x++)
                {
                    if (tiledMap.TileLayers["Environment"].Tiles[x][y] != null)
                    {
                        int sourceId = tiledMap.TileLayers["Environment"].Tiles[x][y].SourceID;
                        if (tiledMap.SourceTiles[sourceId].Properties.Keys.Contains("Col"))
                        {
                            cellsInUse.Add(map.Cells[y * map.Width + x]);
                        }
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (timeForNextDrop == 0)
            {
                timeForNextDrop = randomDropTime.Next(1, 3000);
            }
            foreach(var player in Players)
            {
                player.Update(gameTime);
                if (player.Head.Position.Y > map.Height - 1 ||
                    player.Head.Position.X > map.Width - 1 ||
                    player.Head.Position.Y < 0 ||
                    player.Head.Position.X < 0)
                {

                    
                    worldReset = true;
                }
                else
                {
                    MapCell currentCell = map.Cells[player.Head.Position.Y * map.Width + player.Head.Position.X];
                    Tile currentTile = null;
                    int currentX = player.Head.Position.X;
                    int currentY = player.Head.Position.Y;
                    if(tiledMap.TileLayers["Environment"].Tiles[currentX][currentY] != null)
                     currentTile = tiledMap.SourceTiles[tiledMap.TileLayers["Environment"].Tiles[currentX][currentY].SourceID];

                    if (currentTile != null && currentTile.Properties.Count > 0 && currentTile.Properties.Keys.Contains("Col"))
                    {
                        worldReset = true;
                    }
                    if (currentCell.ContainsPickup)
                    {
                        player.Score += currentCell.Pickup.PointsValue;
                        player.AddSnakeBlock();
                        player.Speed += 5;
                        currentCell.Pickup = null;
                        currentCell.ContainsPickup = false;
                        foodCount--;
                    }

                    foreach (var block in player.Body.Blocks)
                    {
                        if (block.Position == player.Head.Position)
                        {
                            worldReset = true;
                        }
                    }
                }
            }

            GenerateSnakeFood(gameTime);


        }

        private void GenerateSnakeFood(GameTime gameTime)
        {
            elapsedTimed += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTimed > timeForNextDrop && foodCount < foodCap)
            {
                elapsedTimed = 0;
                timeForNextDrop = 0;
                foodCount++;
                Random randX = new Random(DateTime.Now.Millisecond);
                Random randY = new Random(DateTime.Now.Millisecond + 5);
                int xPos,yPos;
                
                xPos = randX.Next(0, map.Width - 1);
                yPos = randY.Next(0, map.Height - 1);
                if (!cellsInUse.Contains(map.Cells[yPos * map.Width + xPos]))
                {
                    pickUp = new MapPickup(50);
                    pickUp.Position = new Point(xPos, yPos);
                    map.Cells[yPos * map.Width + xPos].Pickup = pickUp;
                }
                else
                {
                    foodCount--;
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            

            Vector2 origin = new Vector2(Players[0].Body.Texture.Width / 2, Players[0].Body.Texture.Height / 2);
            int x = 0;
            tiledMap.Draw(ScreenManager.SpriteBatch, new Rectangle(0, 0, 1280, 720));

            foreach (var cell in map.Cells)
            {
                
                
                if (cell.ContainsPickup)
                {
                    int cellY = x / map.Width;
                    int cellX = x % map.Width ;

                    ScreenManager.SpriteBatch.Draw(Players[0].Body.Texture, SnakeHelper.MapToScreen(new Point(cellX, cellY), 
                        Players[0].Body.Texture),
                    Players[0].Body.Source,
                    Color.White,
                    MathHelper.ToRadians(180f),
                    origin,
                    1.0f,
                    SpriteEffects.None,
                    0.0f);
                }
                    
                
                x++;
            }

            foreach (var player in Players)
            {
                player.Draw(gameTime);
                //ScreenManager.SpriteBatch.DrawString(gameFont, "X: " + player.Head.Position.X + Environment.NewLine + "Y: "
                //    + player.Head.Position.Y, new Vector2(640, 600), Color.White);
            }

            //if (foodCount == 1)
            //{
            //    ScreenManager.SpriteBatch.DrawString(gameFont, "foodX: " + pickUp.Position.X + Environment.NewLine + "foodY: "
            //        + pickUp.Position.Y, new Vector2(640, 150), Color.Red);
            //}

            ScreenManager.SpriteBatch.DrawString(gameFont, Players[0].Score.ToString(), new Vector2(12, 12), Color.White);
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

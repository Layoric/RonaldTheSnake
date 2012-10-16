using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RonaldTheSnake.SnakeObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FuncWorks.XNA.XTiled;
using RonaldTheSnake.Factories;

namespace RonaldTheSnake.World
{
    public class GridMap
    {
        List<MapCell> grid = new List<MapCell>();
        

        List<MapCell> cellsInUse = new List<MapCell>();
        Random randomDropTime = new Random();
        List<SnakeFood> mapDrops = new List<SnakeFood>();

        SnakeFood pickUp;

        double foodElapsedTime = 0;

        public int FoodCap { get; set; }
        public int FoodCount { get; set; }

        public int Size { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public GridMap(SnakeLevel currentLevel)
        {
            FoodCap = 4;
            FoodCount = 0;
            Width = currentLevel.TiledMap.Width;
            Height = currentLevel.TiledMap.Height;
            grid.Capacity = Width * Height;
            for (int x = 0; x < Width; x++)
            {
                MapCell xCell = new MapCell();
                grid.Add(xCell);
                for (int y = 0; y < Height; y++)
                {
                    MapCell yCell = new MapCell();
                    grid.Add(yCell);
                }
                
            }

            InitCollisionCells(currentLevel.TiledMap);
            
        }

        public void LoadContent()
        {

        }

        public void Update(GameTime gameTime)
        {
            GenerateSnakeFood(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }

        private void GenerateSnakeFood(GameTime gameTime)
        {
            
            if (foodElapsedTime < 0)
            {
                if (FoodCount < FoodCap)
                {
                    foodElapsedTime = 0;

                    Random randX = new Random(DateTime.Now.Millisecond);
                    Random randY = new Random(DateTime.Now.Millisecond + 5);
                    int xPos, yPos;

                    xPos = randX.Next(0, Width - 1);
                    yPos = randY.Next(0, Height - 1);
                    if (!cellsInUse.Contains(Cells[yPos * Width + xPos]))
                    {
                        FoodCount++;
                        pickUp = SnakeFoodFactory.CreateFromTemplate("cherry");
                        pickUp.Position = new Point(xPos, yPos);
                        //pickUp.Texture = 
                        mapDrops.Add(pickUp);
                        Cells[yPos * Width + xPos].Pickup = pickUp;
                        foodElapsedTime = randomDropTime.Next(1, 3000);
                    }
                }

            }
            else 
            {
                foodElapsedTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
                
            }
        }

        private void InitCollisionCells(Map tiledMap)
        {
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
                            cellsInUse.Add(this.Cells[y * this.Width + x]);
                        }
                    }
                }
            }
        }

        public List<MapCell> Cells
        {
            get
            {
                return grid;
            }
        }
    }

    public class MapCell
    {
        private SnakeFood pickup;
        public SnakeFood Pickup 
        {
            get
            {
                if (ContainsPickup == false)
                    return null;

                return pickup;
            }
            set
            {
                if (value != null)
                    ContainsPickup = true;
                else
                    ContainsPickup = false;

                pickup = value;

            }
        }

        public bool ContainsPickup { get; set; }
    }
}

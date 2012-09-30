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
using RonaldTheSnake.Screens;
using Microsoft.Xna.Framework.Input;
using System.Xml.Serialization;
using System.IO;

namespace RonaldTheSnake.World
{
    public class SnakeWorld : GameScreen
    {
        Point offset;
        public SnakeLevel CurrentLevel { get; set; }
        bool gameOverVisible = false;
        public List<SnakePlayer> Players = new List<SnakePlayer>();
        public string TiledMapName { get; set; }
        Map tiledMap = new Map();
        GridMap map;
        Random randomDropTime = new Random();
        double timeForNextDrop = 0;
        double elapsedTimed = 0;
        double screenElapsedTime = 0;
        SpriteFont gameFont;
        public bool worldReset = false;
        int foodCount = 0;
        int foodCap = 4;
        MapPickup pickUp;
        List<MapCell> cellsInUse = new List<MapCell>();
         

        public SnakeWorld(string levelName)
        {
            TiledMapName = levelName;
        }

        public void Intialize()
        {
            SnakeFactory.RegisterTemplate("default", new SnakeDefaultTemplate(ScreenManager));
            offset = new Point((ScreenManager.Game.GraphicsDevice.DisplayMode.Width - 1024) / 2,
                                (ScreenManager.Game.GraphicsDevice.DisplayMode.Height - 576) / 4);

            Players.Clear();
            //SnakeLevel levelSerialized = new SnakeLevel();
            //levelSerialized.IsCollectLimited = true;
            //levelSerialized.IsFoodSequenced = true;
            //levelSerialized.IsScoreLimited = true;
            //levelSerialized.IsTimeLimited = true;
            //levelSerialized.ScoreLimit = 10000;
            //levelSerialized.TiledMapFileName = "test2";
            //levelSerialized.TimeLimit = 60;
            //levelSerialized.LevelType = SnakeLeveType.Timed;


            //XmlSerializer serializer = new XmlSerializer(typeof(SnakeLevel));
            //using (FileStream fs = new FileStream("levelFileTest.xml", FileMode.CreateNew))
            //{
            //    serializer.Serialize(fs, levelSerialized);
            //}
            
            //serializer.Serialize();
        }

        public override void LoadContent()
        {
            Intialize();
            ContentManager content = ScreenManager.Game.Content;
            Players.Add(SnakeFactory.CreateFromTemplate("default"));
            gameFont = ScreenManager.Game.Content.Load<SpriteFont>("gamefont");
            tiledMap = content.Load<Map>(TiledMapName);
            SnakeHelper.Init(ScreenManager.GraphicsDevice);
            tiledMap.Offset = SnakeHelper.offset;
            map = new GridMap(tiledMap.Width, tiledMap.Height);
            CurrentLevel = new SnakeLevel(tiledMap);
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

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (IsActive)
            {
                TrackElapsedTime(gameTime);

                foreach (var player in Players)
                {
                    UpdatePlayer(gameTime, player);
                }

                if (worldReset && !gameOverVisible)
                {
                    SnakeHelper.ShowGameOver(ScreenManager, ControllingPlayer);

                    gameOverVisible = true;
                    worldReset = false;
                }

                if (CurrentLevel.RemainingTime < 0)
                {
                    SnakeHelper.ShowArcadeTimeUp(ScreenManager, ControllingPlayer, Players[0].Score);
                }

                GenerateSnakeFood(gameTime);

            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private void TrackElapsedTime(GameTime gameTime)
        {
            elapsedTimed += gameTime.ElapsedGameTime.TotalMilliseconds;
            CurrentLevel.RemainingTime -= gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void UpdatePlayer(GameTime gameTime, SnakePlayer player)
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
                if (tiledMap.TileLayers["Environment"].Tiles[currentX][currentY] != null)
                    currentTile = tiledMap.SourceTiles[tiledMap.TileLayers["Environment"].Tiles[currentX][currentY].SourceID];

                if (currentTile != null && currentTile.Properties.Count > 0 && currentTile.Properties.Keys.Contains("Col"))
                {
                    worldReset = true;
                }
                if (currentCell.ContainsPickup)
                {
                    PickupFood(player, currentCell);
                }

                if (HasPlayerHitSelf(player))
                {
                    worldReset = true;
                }
            }
        }

        private void PickupFood(SnakePlayer player, MapCell currentCell)
        {
            player.Score += currentCell.Pickup.PointsValue;
            player.AddSnakeBlock();
            player.Speed += 5;
            currentCell.Pickup = null;
            currentCell.ContainsPickup = false;
            foodCount--;
        }

        private bool HasPlayerHitSelf(SnakePlayer player)
        {
            foreach (var block in player.Body.Blocks)
            {
                if (block.Position == player.Head.Position)
                {
                    return true;
                }
            }
            return false;
        }

        private void GenerateSnakeFood(GameTime gameTime)
        {
            if (timeForNextDrop == 0)
            {
                timeForNextDrop = randomDropTime.Next(1, 3000);
            }
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

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();


            Vector2 origin = new Vector2(Players[0].Body.Texture.Width / 2, Players[0].Body.Texture.Height / 2);
            int x = 0;
            
            tiledMap.Draw(ScreenManager.SpriteBatch, new Rectangle(0, 0, 1024, 576));

            foreach (var cell in map.Cells)
            {


                if (cell.ContainsPickup)
                {
                    int cellY = x / map.Width;
                    int cellX = x % map.Width;

                    ScreenManager.SpriteBatch.Draw(Players[0].Body.Texture, SnakeHelper.MapToScreen(new Point(cellX, cellY),
                        Players[0].Body.Texture, ScreenManager.GraphicsDevice),
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
            ScreenManager.SpriteBatch.DrawString(gameFont, ((int)CurrentLevel.RemainingTime).ToString(), new Vector2(640, 20), Color.Teal);
            ScreenManager.SpriteBatch.End();
        }

        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            if (input.IsNewKeyPress(Keys.Escape, ControllingPlayer, out playerIndex))
            {
                var bgScreen = new BackgroundScreen();
                var mmScreen = new MainMenuScreen();
                LoadingScreen.Load(ScreenManager, true, ControllingPlayer, bgScreen, mmScreen);
            }

            if (input.IsNewKeyPress(Keys.Left, ControllingPlayer, out playerIndex))
            {
                if (Players[(int)playerIndex].Body.Blocks.First().Direction != SnakeDirection.Right)
                    Players[(int)playerIndex].Direction = SnakeDirection.Left;
            }

            if (input.IsNewKeyPress(Keys.Up, ControllingPlayer, out playerIndex))
            {
                if (Players[(int)playerIndex].Body.Blocks.First().Direction != SnakeDirection.Down)
                    Players[(int)playerIndex].Direction = SnakeDirection.Up;
            }

            if (input.IsNewKeyPress(Keys.Right, ControllingPlayer, out playerIndex))
            {
                if (Players[(int)playerIndex].Body.Blocks.First().Direction != SnakeDirection.Left)
                    Players[(int)playerIndex].Direction = SnakeDirection.Right;
            }

            if (input.IsNewKeyPress(Keys.Down, ControllingPlayer, out playerIndex))
            {
                if (Players[(int)playerIndex].Body.Blocks.First().Direction != SnakeDirection.Up)
                    Players[(int)playerIndex].Direction = SnakeDirection.Down;
            }
        }

    }

    public class SnakeLevel
    {

        public SnakeLevel(Map map)
        {
            LevelType = (SnakeLevelType)Enum.Parse(typeof(SnakeLevelType), map.Properties["LevelType"].Value);
            IsTimeLimited = bool.Parse(map.Properties["IsTimeLimited"].Value);
            TimeLimit = int.Parse(map.Properties["TimeLimit"].Value);
            RemainingTime = TimeLimit;
        }

        public bool IsTimeLimited { get; set; }
        public bool IsCollectLimited { get; set; }
        public bool IsScoreLimited { get;set; }

        public bool IsFoodSequenced { get; set; }

        public int? TimeLimit { get; set; }
        public int? ScoreLimit { get; set; }

        public double? RemainingTime { get; set; }

        public SnakeLevelType LevelType { get; set; }
         
    }    
}

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
        public List<SnakePlayer> Players = new List<SnakePlayer>();
        public string TiledMapName { get; set; }
        Map tiledMap = new Map();
        GridMap map;
        
        double elapsedTimed = 0;
        SpriteFont gameFont;
        public bool worldReset = false;
         

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
            map = new GridMap(tiledMap);
            CurrentLevel = new SnakeLevel(tiledMap);
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

                if (worldReset)
                {
                    SnakeHelper.ShowGameOver(ScreenManager, ControllingPlayer);

                    worldReset = false;
                }

                if (CurrentLevel.RemainingTime < 0)
                {
                    SnakeHelper.ShowArcadeTimeUp(ScreenManager, ControllingPlayer, Players[0].Score);
                }

                map.Update(gameTime);

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
            map.FoodCount--;
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
            if (CurrentLevel.IsTimeLimited)
            {
                ScreenManager.SpriteBatch.DrawString(gameFont, ((int)CurrentLevel.RemainingTime).ToString(), new Vector2(640, 20), Color.Teal);
            }
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

            switch (LevelType)
            {
                case SnakeLevelType.Arcade:
                    break;
                case SnakeLevelType.Puzzle:
                    IsCollectLimited = bool.Parse(map.Properties["IsCollectLimited"].Value);
                    IsScoreLimited = bool.Parse(map.Properties["IsScoreLimited"].Value);
                    IsTimeLimited = bool.Parse(map.Properties["IsTimeLimited"].Value);
                    FoodRequired = IsCollectLimited ? (int?)int.Parse(map.Properties["FoodRequired"].Value) : null;
                    ScoreRequired = IsScoreLimited ? (int?)int.Parse(map.Properties["ScoreRequired"].Value) : null;
                    TimeLimit = IsTimeLimited ? (int?)int.Parse(map.Properties["TimeLimit"].Value) : null;              
                    break;
                case SnakeLevelType.Timed:
                    TimeLimit = int.Parse(map.Properties["TimeLimit"].Value);
                    RemainingTime = TimeLimit;
                    break;
                default:
                    break;
            }

            
        }

        public bool IsTimeLimited { get; set; }
        public bool IsCollectLimited { get; set; }
        public bool IsScoreLimited { get;set; }

        public bool IsFoodSequenced { get; set; }

        public int? TimeLimit { get; set; }
        public int? ScoreRequired { get; set; }
        public int? FoodRequired { get; set; }

        public double? RemainingTime { get; set; }

        public SnakeLevelType LevelType { get; set; }
         
    }    
}

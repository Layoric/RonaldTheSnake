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
using RonaldTheSnake.Templates.FoodTemplates;

namespace RonaldTheSnake.World
{
    public class SnakeWorld : GameScreen
    {
        public SnakeLevel CurrentLevel { get; set; }
        public List<SnakePlayer> Players = new List<SnakePlayer>();
        public string TiledMapName { get; set; }
        Map tiledMap = new Map();
        bool refreshHitCells = true;
        double refreshHitCellTimeLimit = 0;
        GridMap map;
        
        double elapsedTimed = 0;
        SpriteFont gameFont;
        SpriteFont scoreFont;
        public bool worldReset = false;
        public bool levelComplete = false;
        public Texture2D gameBackground;

        public Texture2D cherryFood;
        public Texture2D bananaFood;
        public Texture2D appleFood;
        public Texture2D strawberryFood;
         

        public SnakeWorld(string levelName)
        {
            TiledMapName = levelName;
        }

        public void Intialize()
        {
            SnakeFactory.RegisterTemplate("default", new SnakeDefaultTemplate(ScreenManager));

            SnakeFoodFactory.RegisterTemplate("cherry", new CherryFoodTemplate(ScreenManager));
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
            scoreFont = ScreenManager.Game.Content.Load<SpriteFont>("scorefont");
            gameBackground = ScreenManager.Game.Content.Load<Texture2D>("gamebackground");
            cherryFood = ScreenManager.Game.Content.Load<Texture2D>("cherry");
            bananaFood = ScreenManager.Game.Content.Load<Texture2D>("bananas");
            tiledMap = content.Load<Map>(TiledMapName);
            
            SnakeHelper.Init(ScreenManager.GraphicsDevice, tiledMap);
            tiledMap.Offset = SnakeHelper.offset;
            CurrentLevel = new SnakeLevel(tiledMap);

            //GridMap needs to be last due to reliance on Map and SnakeLevel
            map = new GridMap(CurrentLevel);
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

                CheckTimeRemaining();
                CheckDisplayEndScreen();
                CheckCollectedRemaining();

                map.Update(gameTime);
                
                //base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            }   
        }

        private void CheckCollectedRemaining()
        {
            if (CurrentLevel.IsCollectLimited)
            {
                if (CurrentLevel.FoodRequired <= Players[0].TotalFoodCollected && !levelComplete)
                {
                    levelComplete = true;
                }
            }
        }

        private void CheckDisplayEndScreen()
        {
            if (worldReset)
            {
                SnakeHelper.ShowGameOver(ScreenManager, ControllingPlayer);

                worldReset = false;
            }

            if (levelComplete)
            {
                SnakeHelper.ShowLevelComplete(ScreenManager, ControllingPlayer, Players[0].Score);
                levelComplete = false;
            }
        }

        private void CheckTimeRemaining()
        {
            if (CurrentLevel.RemainingTime < 0)
            {
                SnakeHelper.ShowArcadeTimeUp(ScreenManager, ControllingPlayer, Players[0].Score);
            }
        }

        private void TrackElapsedTime(GameTime gameTime)
        {
            elapsedTimed += gameTime.ElapsedGameTime.TotalMilliseconds;
            CurrentLevel.RemainingTime -= gameTime.ElapsedGameTime.TotalSeconds;
            if (refreshHitCells)
            {
                refreshHitCellTimeLimit -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (refreshHitCellTimeLimit < 0)
                {
                    refreshHitCells = false;
                    refreshHitCellTimeLimit = 1500;
                }
            }
        }

        private void UpdatePlayer(GameTime gameTime, SnakePlayer player)
        {
            player.Update(gameTime);
            if (player.Head.Position.Y > map.Height - 1 ||
                player.Head.Position.X > map.Width - 1 ||
                player.Head.Position.Y < 0 ||
                player.Head.Position.X < 0)
            {

                switch (player.Direction)
                {
                    case SnakeDirection.Up:
                        player.Head.Position = new Point(player.Head.Position.X, map.Height - 1);
                        break;
                    case SnakeDirection.Right:
                        player.Head.Position = new Point(0, player.Head.Position.Y);
                        break;
                    case SnakeDirection.Down:
                        player.Head.Position = new Point(player.Head.Position.X, 0);
                        break;
                    case SnakeDirection.Left:
                        player.Head.Position = new Point(map.Width - 1, player.Head.Position.Y);
                        break;
                    default:
                        break;
                }
                //worldReset = true;
            }
            
                MapCell currentCell = map.Cells[player.Head.Position.Y * map.Width + player.Head.Position.X];
                Tile currentTile = null;
                int currentX = player.Head.Position.X;
                int currentY = player.Head.Position.Y;
                if (currentX > 0 && currentX < tiledMap.Width - 1 && currentY > 0 && currentY < tiledMap.Height)
                {
                    if (tiledMap.TileLayers["Environment"].Tiles[currentX][currentY] != null)
                        currentTile = tiledMap.SourceTiles[tiledMap.TileLayers["Environment"].Tiles[currentX][currentY].SourceID];
                }
                else
                {
                    currentTile = null;
                }

                if (currentTile != null && currentTile.Properties.Count > 0 && currentTile.Properties.Keys.Contains("Col"))
                {
                    PlayerEnvironmentCollision(player);
                }

                if (currentTile != null && currentTile.Properties.Count > 0 && currentTile.Properties.Keys.Contains("Wall"))
                {
                    if(!worldReset)
                        worldReset = true;
                }
                if (currentCell != null && currentCell.ContainsPickup)
                {
                    PickupFood(player, currentCell);
                }

                if (HasPlayerHitSelf(player))
                {
                    if (!worldReset)
                        worldReset = true;
                }
            
        }

        private void Update(SnakeHead head)
        {
            
        }

        private void PlayerEnvironmentCollision(SnakePlayer player)
        {
            if (player.Body.Blocks.Count > 4)
            {
                if (!refreshHitCells)
                {
                    int limit = player.Body.Blocks.Count - 5;
                    for (int i = player.Body.Blocks.Count - 1; i > limit; i--)
                    {
                        player.Body.Blocks.RemoveAt(i);
                        player.Score -= 40;
                    }
                    refreshHitCells = true;
                }
            }
            else
            {
                if (!worldReset)
                    worldReset = true;
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
            player.TotalFoodCollected++;
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
            ScreenManager.SpriteBatch.Draw(gameBackground, Vector2.Zero, Color.White);
            tiledMap.Draw(ScreenManager.SpriteBatch, tiledMap.Bounds);

            foreach (var cell in map.Cells)
            {


                if (cell.ContainsPickup)
                {
                    int cellY = x / map.Width;
                    int cellX = x % map.Width;

                    ScreenManager.SpriteBatch.Draw(cell.Pickup.Texture, SnakeHelper.MapToScreen(cell.Pickup.Position,
                        cell.Pickup.Texture, ScreenManager.GraphicsDevice),
                    cell.Pickup.Texture.Bounds,
                    Color.White,
                    MathHelper.ToRadians(0f),
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

            ScreenManager.SpriteBatch.DrawString(scoreFont, Players[0].Score.ToString(), new Vector2(
                ((ScreenManager.GraphicsDevice.DisplayMode.Width + 1024) / 2) - 50
                , 15), Color.White);

            if (CurrentLevel.IsCollectLimited)
            {
                ScreenManager.SpriteBatch.DrawString(scoreFont, Players[0].TotalFoodCollected.ToString() + "/" + 
                    CurrentLevel.FoodRequired,
                    new Vector2(((ScreenManager.GraphicsDevice.DisplayMode.Width + 1024) / 2) - 250
                    , 15), Color.White);
            }

            if (CurrentLevel.IsTimeLimited)
            {
                ScreenManager.SpriteBatch.DrawString(scoreFont, ((int)CurrentLevel.RemainingTime).ToString(), 
                    new Vector2(640, 15), Color.White);
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

       
}

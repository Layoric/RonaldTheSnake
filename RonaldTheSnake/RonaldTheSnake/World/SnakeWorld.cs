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
        float sunSpeed = 0.0001f;
        
        double elapsedTimed = 0;
        SpriteFont gameFont;
        SpriteFont scoreFont;
        public bool worldReset = false;
        public bool levelComplete = false;
        public Texture2D gameBackground;
        public Texture2D midGround;

        public Texture2D cherryFood;
        public Texture2D bananaFood;
        public Texture2D appleFood;
        public Texture2D strawberryFood;

        #region 2D God rays fields

        RenderTarget2D scene;

        double elapsedTime = 0;

        PostProcessingManager ppm;
        CrepuscularRays rays;
        float rayPosMovement = 0.0f;

        List<string> backgroundAssets = new List<string>();
        List<float> bgSpeeds = new List<float>();

        List<Vector2> bgPos = new List<Vector2>();
        List<Vector2> bgPos2 = new List<Vector2>();
        List<Vector2> bgPos2Base = new List<Vector2>();

        int bgCnt;

        ParticleSystem particleSys;

        #endregion         

        public SnakeWorld(string levelName)
        {
            TiledMapName = levelName;
        }

        public void Intialize()
        {
            SnakeFactory.RegisterTemplate("default", new SnakeDefaultTemplate(ScreenManager));

            SnakeFoodFactory.RegisterTemplate("cherry", new CherryFoodTemplate(ScreenManager));
            Players.Clear();

            particleSys = new ParticleSystem(ScreenManager.Game.Content, ScreenManager.SpriteBatch);
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
            gameBackground = ScreenManager.Game.Content.Load<Texture2D>("sky_bg");
            //midGround = ScreenManager.Game.Content.Load<Texture2D>("midground_grasshill");
            cherryFood = ScreenManager.Game.Content.Load<Texture2D>("cherry");
            bananaFood = ScreenManager.Game.Content.Load<Texture2D>("bananas");

            tiledMap = content.Load<Map>(TiledMapName);
            
            SnakeHelper.Init(ScreenManager.GraphicsDevice, tiledMap);
            tiledMap.Offset = SnakeHelper.offset;
            CurrentLevel = new SnakeLevel(tiledMap);

            //GridMap needs to be last due to reliance on Map and SnakeLevel
            map = new GridMap(CurrentLevel);

            ppm = new PostProcessingManager(ScreenManager.Game);

            rays = new CrepuscularRays(ScreenManager.Game, Vector2.One * .5f, "flare4", 0.35f, .97f, .97f, .3f, .20f);

            ppm.AddEffect(rays);

            ScreenManager.Game.Services.AddService(ScreenManager.SpriteBatch.GetType(), ScreenManager.SpriteBatch);

            UpdateLightSource();

            scene = new RenderTarget2D(ScreenManager.Game.GraphicsDevice, 
                ScreenManager.Game.GraphicsDevice.Viewport.Width,
                ScreenManager.Game.GraphicsDevice.Viewport.Height, 
                false, 
                SurfaceFormat.Color, 
                DepthFormat.None);

            AddBackground("cloud1", 0.5f);
            AddBackground("midground_grasshill", 0.0f);
            //AddBackground("midground_fog", 0.0f);

        }

        private void LoadAllBackgroundAssets()
        {
            foreach (var backgroundAsset in backgroundAssets)
            {
                ScreenManager.Game.Content.Load<Texture2D>(backgroundAsset);
            }
        }

        public virtual void AddBackground(string bgAsset, float speed)
        {
            backgroundAssets.Add(bgAsset);
            bgSpeeds.Add(speed);
            bgPos.Add(Vector2.Zero);
            bgCnt++;

            bgPos2.Add(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, 0));
            bgPos2Base.Add(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, 0));
        }

        public override void UnloadContent()
        {
            ScreenManager.Game.Services.RemoveService(typeof(SpriteBatch));
            base.UnloadContent();
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

                particleSys.Update((float)gameTime.ElapsedGameTime.Milliseconds);
                
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
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
            particleSys.CreatePlayerExplosion(SnakeHelper.MapToScreen(
                player.Head.Position,
                ScreenManager.Game.Content.Load<Texture2D>("smoke"),
                ScreenManager.Game.GraphicsDevice));
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
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            ContentManager content = ScreenManager.Game.Content;
            GraphicsDevice device = ScreenManager.GraphicsDevice;
            elapsedTime = 1000 / gameTime.ElapsedGameTime.TotalMilliseconds;

            

            UpdateLightSource();

            //spriteBatch.Begin();

            //ScreenManager.SpriteBatch.Draw(gameBackground,
            //    new Rectangle(0, 0, ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth
            //    , ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight)
            //    , gameBackground.Bounds, Color.White, 0.0f,
            //    Vector2.Zero, SpriteEffects.None, 0.0f
            //    );

            //spriteBatch.End();

            ScreenManager.GraphicsDevice.SetRenderTarget(scene);
            ScreenManager.GraphicsDevice.Clear(Color.White);


            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            LightRaysBackgroundFirstPass(spriteBatch, content, device);

            spriteBatch.End();

            device.SetRenderTarget(null);

            // Apply the post processing maanger (just the rays in this one)
            ppm.Draw(gameTime, scene);

            // Now blend that source with the scene..
            device.SetRenderTarget(scene);
            // Draw the scene in color now
            device.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            //Draw sky background
            ScreenManager.SpriteBatch.Draw(gameBackground,
                new Rectangle(0, 0, ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth
                , ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight)
                , gameBackground.Bounds, Color.White, 0.0f,
                Vector2.Zero, SpriteEffects.None, 0.0f
                );

            LightRaysBackgroundDraw(spriteBatch, content, device);

            spriteBatch.End();
            ScreenManager.GraphicsDevice.SetRenderTarget(null);
            device.Clear(Color.Black);

            //Draw final scene
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            spriteBatch.Draw(ppm.Scene, new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height), Color.White);
            spriteBatch.Draw(scene, new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height), Color.White);

            spriteBatch.End();

            //Update background positions
            for (int b = 0; b < bgCnt; b++)
            {
                bgPos[b] -= new Vector2(bgSpeeds[b], 0);
                bgPos2[b] -= new Vector2(bgSpeeds[b], 0);

                if (bgPos[b].X < -bgPos2Base[b].X)
                    bgPos[b] = new Vector2(bgPos2[b].X + bgPos2Base[b].X, 0);

                if (bgPos2[b].X < -bgPos2Base[b].X)
                    bgPos2[b] = new Vector2(bgPos[b].X + bgPos2Base[b].X, 0);
            }

            spriteBatch.Begin();

            Vector2 origin = new Vector2(Players[0].Body.Texture.Width / 2, Players[0].Body.Texture.Height / 2);
            int x = 0;

            tiledMap.Draw(ScreenManager.SpriteBatch, tiledMap.Bounds);


            particleSys.Draw();
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
                ((ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth + 1024) / 2) - 50
                , 15), Color.White);

            if (CurrentLevel.IsCollectLimited)
            {
                ScreenManager.SpriteBatch.DrawString(scoreFont, Players[0].TotalFoodCollected.ToString() + "/" +
                    CurrentLevel.FoodRequired,
                    new Vector2(((ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth + 1024) / 2) - 250
                    , 15), Color.White);
            }

            if (CurrentLevel.IsTimeLimited)
            {
                ScreenManager.SpriteBatch.DrawString(scoreFont, ((int)CurrentLevel.RemainingTime).ToString(),
                    new Vector2(640, 15), Color.White);
            }


            ScreenManager.SpriteBatch.End();
        }

        private void LightRaysBackgroundDraw(SpriteBatch spriteBatch, ContentManager content, GraphicsDevice device)
        {
            for (int bg = 0; bg < bgCnt; bg++)
            {
                spriteBatch.Draw(content.Load<Texture2D>(backgroundAssets[bg]),
                    new Rectangle((int)bgPos[bg].X,
                        (int)bgPos[bg].Y,
                        ScreenManager.GraphicsDevice.Viewport.Width,
                        device.Viewport.Height),
                    new Rectangle(0, 0,
                        content.Load<Texture2D>(backgroundAssets[bg]).Width,
                        content.Load<Texture2D>(backgroundAssets[bg]).Height),
                rays.RenderColor);
                spriteBatch.Draw(content.Load<Texture2D>(backgroundAssets[bg]),
                    new Rectangle((int)bgPos2[bg].X,
                        (int)bgPos[bg].Y,
                        ScreenManager.GraphicsDevice.Viewport.Width,
                        device.Viewport.Height),
                    new Rectangle(0, 0,
                        content.Load<Texture2D>(backgroundAssets[bg]).Width,
                        content.Load<Texture2D>(backgroundAssets[bg]).Height),
                    rays.RenderColor);
            }
        }

        private void LightRaysBackgroundFirstPass(SpriteBatch spriteBatch, ContentManager content, GraphicsDevice device)
        {
            for (int bg = 0; bg < bgCnt; bg++)
            {
                spriteBatch.Draw(content.Load<Texture2D>(backgroundAssets[bg]),
                    new Rectangle((int)bgPos[bg].X,
                        (int)bgPos[bg].Y,
                        ScreenManager.GraphicsDevice.Viewport.Width,
                        device.Viewport.Height),
                    new Rectangle(0, 0,
                        content.Load<Texture2D>(backgroundAssets[bg]).Width,
                        content.Load<Texture2D>(backgroundAssets[bg]).Height),
                Color.Black);
                spriteBatch.Draw(content.Load<Texture2D>(backgroundAssets[bg]),
                    new Rectangle((int)bgPos2[bg].X,
                        (int)bgPos[bg].Y,
                        ScreenManager.GraphicsDevice.Viewport.Width,
                        device.Viewport.Height),
                    new Rectangle(0, 0,
                        content.Load<Texture2D>(backgroundAssets[bg]).Width,
                        content.Load<Texture2D>(backgroundAssets[bg]).Height),
                    Color.Black);
            }
        }

        private void UpdateLightSource()
        {
            ContentManager content = ScreenManager.Game.Content;
            if (rayPosMovement <= 1.0f)
                rayPosMovement = rayPosMovement + sunSpeed;
            else
                rayPosMovement = 0.0f;

            float y = ((rayPosMovement * rayPosMovement) - rayPosMovement);
            rays.lightSource = new Vector2(rayPosMovement, (y * 4) + 1);

            
            rays.lightTexture = content.Load<Texture2D>("flare4");

            rays.RenderColor = new Color((y * 8) * -1, (y * 6) * -1, (y * 6) * -1);

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

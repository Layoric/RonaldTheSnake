using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RonaldTheSnake.Screens
{
    public class BackgroundEnvironmentScreen : GameScreen, IDisposable
    {

        ContentManager content;
        Texture2D sky;
        Texture2D sun;

        RenderTarget2D scene;

        double elapsedTime = 0;

        float sunSpeed = 0.001f;

        bool showMoon = true;
        bool toggleMoonSun = false;

        PostProcessingManager ppm;
        CrepuscularRays rays;
        float rayPosMovement = 0.0f;

        List<string> backgroundAssets = new List<string>();
        List<float> bgSpeeds = new List<float>();

        List<Vector2> bgPos = new List<Vector2>();
        List<Vector2> bgPos2 = new List<Vector2>();
        List<Vector2> bgPos2Base = new List<Vector2>();

        int bgCnt;

        public BackgroundEnvironmentScreen()
        {

        }

        public override void LoadContent()
        {
            if (content == null)
                content = ScreenManager.Game.Content;

            sky = content.Load<Texture2D>("sky_bg");


            ppm = new PostProcessingManager(ScreenManager.Game);

            rays = new CrepuscularRays(ScreenManager.Game, Vector2.One * .5f, "flare4", 0.35f, .87f, .97f, .3f, .20f);

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
        }

        private void UpdateRaysSource(string assetName)
        {
            rays.lightTexture = content.Load<Texture2D>(assetName);
        }

        private void LoadAllBackgroundAssets()
        {
            foreach (var backgroundAsset in backgroundAssets)
            {
                ScreenManager.Game.Content.Load<Texture2D>(backgroundAsset);
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


            //rays.lightTexture = content.Load<Texture2D>("flare4");
            if (showMoon) // eg, next cycle will be moon
            {
                rays.RenderColor = new Color((y * 8) * -1, (y * 6) * -1, (y * 6) * -1);
                rays.Decay = 0.97f;
                rays.Weight = 0.3f;
            }
            else
            {
                rays.RenderColor = new Color((y * 2) * -1, (y * 2) * -1, (y * 4) * -1);
                rays.Decay = 0.8f;
                rays.Weight = 0.4f;
            }

            if (y > -0.01f)
            {
                if (toggleMoonSun)
                {
                    if (showMoon)
                    {
                        UpdateRaysSource("moon");
                        showMoon = false;
                    }
                    else
                    {
                        UpdateRaysSource("flare4");
                        showMoon = true;
                    }
                }
                toggleMoonSun = false;
            }

            if (y < -0.24f)
                toggleMoonSun = true;

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

        private void UpdateBackgroundPositions()
        {
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

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            UpdateBackgroundPositions();

            base.Update(gameTime, otherScreenHasFocus, false);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            ContentManager content = ScreenManager.Game.Content;
            GraphicsDevice device = ScreenManager.GraphicsDevice;
            elapsedTime = 1000 / gameTime.ElapsedGameTime.TotalMilliseconds;

            UpdateLightSource();

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
            ScreenManager.SpriteBatch.Draw(sky,
                new Rectangle(0, 0, ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth
                , ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight)
                , sky.Bounds, Color.White, 0.0f,
                Vector2.Zero, SpriteEffects.None, 0.0f
                );

            LightRaysBackgroundDraw(spriteBatch, content, device);

            spriteBatch.End();
            ScreenManager.GraphicsDevice.SetRenderTarget(null);
            device.Clear(Color.Black);

            //Draw final scene
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            spriteBatch.Draw(ppm.Scene, device.Viewport.Bounds, Color.White);
            spriteBatch.Draw(scene, device.Viewport.Bounds, Color.White);

            spriteBatch.End();

            //base.Draw(gameTime);
        }

        public void Dispose()
        {
            content.Dispose();
            sky.Dispose();
        }
    }
}

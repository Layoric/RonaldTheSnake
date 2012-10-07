using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RonaldTheSnake.Screens
{
    abstract class LevelChoiceMenuScreen : MenuScreen
    {

        int previewWidth = 128;
        int previewHeight = 128;
        int spacerWidth = 16;
        int spacerBase = 16;

        public LevelChoiceMenuScreen(string title) : base(title)
        {

        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                LevelMenuEntry menuEntry = MenuEntries[i] as LevelMenuEntry;

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime);
                
            }

            spriteBatch.End();
        }

        protected override void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            

            int totalWidth = ScreenManager.GraphicsDevice.Viewport.Width;
            int totalHeight = ScreenManager.GraphicsDevice.Viewport.Height;
            
            spacerWidth = (int)(spacerBase * ScreenManager.GraphicsDevice.Viewport.AspectRatio);
            Vector2 startPos = new Vector2(32f, 32f);
            int numberOfPreviewY = (totalHeight / (previewHeight + spacerBase)) - 1;
            int numberOfPreviewX = (totalWidth / (previewWidth + spacerWidth)) - 1;
            int maxNumberOfMenuEntries = (numberOfPreviewX + 1) * (numberOfPreviewY + 1);
            
            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(32f,32f);
            // update each menu entry's location in turn
            int row = 0;
            int col = 0;
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntry menuEntry = MenuEntries[i];



                position.X = ((col * (spacerWidth + previewWidth)) + startPos.X) * ((transitionOffset + 1));
                position.Y = ((row * (spacerBase + previewHeight)) + startPos.Y) * ((transitionOffset + 1));
                if ((i + 1) * (spacerWidth + previewWidth) < ((totalWidth) * (row + 1)) && (col < numberOfPreviewX))
                {
                    col++;
                }
                else
                {
                    //position.Y += previewHeight;
                    row++;
                    col = 0;
                }

                
                

                //int xStart = i % numberOfPreviewY;
                //int yStart = i / numberOfPreviewY;
                ////xStart++;
                ////yStart++;
                //position.Y = yStart * ((previewWidth + spacerWidth * 2) * ((transitionOffset + 1))) + startPos.Y;
                //position.X = xStart * ((previewHeight + spacerHeight * 2) * ((transitionOffset + 1))) + startPos.X;
                //position.X += spacerWidth * xStart;
                ////if (ScreenState == ScreenState.TransitionOn)
                ////    position.X -= transitionOffset * 256;
                ////else
                ////    position.X += transitionOffset * 512;
                if (i < maxNumberOfMenuEntries)
                {
                    menuEntry.Position = position;
                }
                else
                {
                    menuEntry.IsVisible = false;
                }

                // each entry is to be centered horizontally
            }
        }
    }
}

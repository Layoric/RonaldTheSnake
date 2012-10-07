using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RonaldTheSnake.Screens
{
    class LevelMenuEntry : MenuEntry
    {
        public string Title { get; set; }
        public string SubText { get; set; }
        public Texture2D Preview { get; set; }
        public Vector2 PreviewPosition = new Vector2();

        public string FileName { get; set; }

        public Rectangle Source { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }        

        public LevelMenuEntry(string fileName,string title) : base(title)
        {
            this.Title = title;
            this.FileName = fileName;
        }

        public override void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            //Update the image position of the preview before drawing
            UpdatePreviewPosition();
            Color color = isSelected ? Color.Yellow : Color.White;
            color *= screen.TransitionAlpha;
            if(IsVisible)
            screen.ScreenManager.SpriteBatch.Draw(Preview, PreviewPosition, Source, color);
            base.Draw(screen, isSelected, gameTime);
        }

        private void UpdatePreviewPosition()
        {
            PreviewPosition = new Vector2(Position.X, Position.Y + 16);
        }

    }
}

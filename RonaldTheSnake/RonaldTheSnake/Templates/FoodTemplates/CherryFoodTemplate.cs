using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RonaldTheSnake.Screens;
using Microsoft.Xna.Framework.Content;
using RonaldTheSnake.SnakeObjects;
using Microsoft.Xna.Framework.Graphics;

namespace RonaldTheSnake.Templates.FoodTemplates
{
    public class CherryFoodTemplate : ISnakeFoodTemplate
    {
        ScreenManager ScreenManager;
        ContentManager Content;

        public CherryFoodTemplate(ScreenManager screenManager)
        {
            this.ScreenManager = screenManager;
            this.Content = screenManager.Game.Content;
        }

        public SnakeObjects.SnakeFood GenerateFood()
        {
            SnakeFood result = new SnakeFood(75);
            result.RemainingTime = 5;
            result.Texture = Content.Load<Texture2D>("cherry");
            return result;
        }
    }
}

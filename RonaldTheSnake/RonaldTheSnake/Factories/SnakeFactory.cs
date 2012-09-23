using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RonaldTheSnake.SnakeObjects;
using RonaldTheSnake.Templates;

namespace RonaldTheSnake.Factories
{
    public class SnakeFactory
    {
        private static Dictionary<string, ISnakeTemplate> snakeTemplates;
        public static Dictionary<string, ISnakeTemplate> SnakeTemplates 
        {
            get
            {
                if (snakeTemplates == null)
                    snakeTemplates = new Dictionary<string, ISnakeTemplate>();

                return snakeTemplates;
            }
        }

        public static SnakePlayer CreateFromTemplate(string name)
        {
            if (SnakeTemplates.Keys.Contains(name))
            {
                SnakePlayer player;
                player = SnakeTemplates[name].BuildSnake();
                return player;
            }
            else
            {
                throw new NotSupportedException();
            }
            
        }

        public static void RegisterTemplate(string name, ISnakeTemplate template)
        {
            if (!SnakeTemplates.ContainsKey(name))
            {
                SnakeTemplates.Add(name, template);
            }
        }

    }
}

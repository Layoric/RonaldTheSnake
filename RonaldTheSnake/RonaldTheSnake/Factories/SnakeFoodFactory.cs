using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RonaldTheSnake.Templates;
using RonaldTheSnake.SnakeObjects;

namespace RonaldTheSnake.Factories
{
    public class SnakeFoodFactory
    {
        private static Dictionary<string, ISnakeFoodTemplate> snakeFoodTemplates;
        public static Dictionary<string, ISnakeFoodTemplate> SnakeFoodTemplates
        {
            get
            {
                if (snakeFoodTemplates == null)
                    snakeFoodTemplates = new Dictionary<string, ISnakeFoodTemplate>();

                return snakeFoodTemplates;
            }
        }

        public static SnakeFood CreateFromTemplate(string templateName)
        {
            if (SnakeFoodTemplates.ContainsKey(templateName))
            {
                SnakeFood food;
                food = SnakeFoodTemplates[templateName].GenerateFood();
                return food;
            }
            else
            {
                throw new NotSupportedException("Template not supported or missing: " + templateName);
            }
        }

        public static void RegisterTemplate(string name, ISnakeFoodTemplate template)
        {
            if (!SnakeFoodTemplates.ContainsKey(name))
            {
                SnakeFoodTemplates.Add(name, template);
            }
        }
    }
}

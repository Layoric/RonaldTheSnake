using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RonaldTheSnake.SnakeObjects;

namespace RonaldTheSnake.Templates
{
    public interface ISnakeFoodTemplate
    {
        SnakeFood GenerateFood();
    }
}

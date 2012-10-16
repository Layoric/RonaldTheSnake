
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RonaldTheSnake
{
    public enum SnakeDirection
    {
        Up = 0,
        Right,
        Down,
        Left
    }

    public enum SnakeLevelType
    {
        Arcade,
        Puzzle,
        Timed
    }

    public enum SnakeFoodType
    {
        Cherry = 0,
        Banana,
        Apple,
        Strawberry
    }
}

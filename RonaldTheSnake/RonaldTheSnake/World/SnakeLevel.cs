﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;

namespace RonaldTheSnake.World
{
    public class SnakeLevel
    {
        public Map TiledMap { get; set; }

        public SnakeLevel(Map map)
        {
            TiledMap = map;

            LevelType = (SnakeLevelType)Enum.Parse(typeof(SnakeLevelType), map.Properties["LevelType"].Value);
            switch (LevelType)
            {
                case SnakeLevelType.Arcade:
                    break;
                case SnakeLevelType.Puzzle:
                    IsCollectLimited = map.Properties.ContainsKey("IsCollectLimited") ? bool.Parse(map.Properties["IsCollectLimited"].Value) : false;
                    IsScoreLimited = map.Properties.ContainsKey("IsScoreLimited") ? bool.Parse(map.Properties["IsScoreLimited"].Value) : false;
                    IsTimeLimited = map.Properties.ContainsKey("IsTimeLimited") ? bool.Parse(map.Properties["IsTimeLimited"].Value) : false;
                    FoodVanish = map.Properties.ContainsKey("FoodVanish") ? bool.Parse(map.Properties["FoodVanish"].Value) : false;
                    FoodAddsSpeed = map.Properties.ContainsKey("FoodAddsSpeed") ? bool.Parse(map.Properties["FoodAddsSpeed"].Value) : false;

                    
                    FoodRequired = IsCollectLimited ? (int?)int.Parse(map.Properties["FoodRequired"].Value) : null;
                    ScoreRequired = IsScoreLimited ? (int?)int.Parse(map.Properties["ScoreRequired"].Value) : null;
                    TimeLimit = IsTimeLimited ? (int?)int.Parse(map.Properties["TimeLimit"].Value) : null;
                    RemainingTime = TimeLimit;
                    break;
                case SnakeLevelType.Timed:
                    IsTimeLimited = bool.Parse(map.Properties["IsTimeLimited"].Value);
                    TimeLimit = int.Parse(map.Properties["TimeLimit"].Value);
                    RemainingTime = TimeLimit;
                    break;
                default:
                    break;
            }


        }

        public bool IsTimeLimited { get; set; }
        public bool IsCollectLimited { get; set; }
        public bool IsScoreLimited { get; set; }
        public bool FoodVanish { get; set; }
        public bool FoodAddsSpeed { get; set; }
        

        public int? FoodTimeLimit { get; set; }

        public bool IsFoodSequenced { get; set; }

        public int? TimeLimit { get; set; }
        public int? ScoreRequired { get; set; }
        public int? FoodRequired { get; set; }
        public int? FoodSpeedInc { get; set; }

        public double? RemainingTime { get; set; }

        public SnakeLevelType LevelType { get; set; }

    } 
}
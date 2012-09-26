using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RonaldTheSnake.SnakeObjects;

namespace RonaldTheSnake.World
{
    public class GridMap
    {
        List<MapCell> grid = new List<MapCell>();

        public int Size { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public GridMap(int width, int height)
        {

            Width = width;
            Height = height;
            grid.Capacity = width * height;
            for (int x = 0; x < width; x++)
            {
                MapCell xCell = new MapCell();
                grid.Add(xCell);
                for (int y = 0; y < height; y++)
                {
                    MapCell yCell = new MapCell();
                    grid.Add(yCell);
                }
                
            }
            
        }

        public List<MapCell> Cells
        {
            get
            {
                return grid;
            }
        }


    }

    public class MapCell
    {
        private MapPickup pickup;
        public MapPickup Pickup 
        {
            get
            {
                if (ContainsPickup == false)
                    return null;

                return pickup;
            }
            set
            {
                if (value != null)
                    ContainsPickup = true;
                else
                    ContainsPickup = false;

                pickup = value;

            }
        }

        public bool ContainsPickup { get; set; }
    }
}

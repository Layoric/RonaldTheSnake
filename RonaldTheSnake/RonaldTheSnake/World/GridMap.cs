using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RonaldTheSnake.SnakeObjects;

namespace RonaldTheSnake.World
{
    public class GridMap
    {
        List<List<MapCell>> grid = new List<List<MapCell>>();

        public GridMap(int size)
        {
            grid.Capacity = size;
            for (int x = 0; x < size; x++)
            {
                List<MapCell> xCell = new List<MapCell>();
                xCell.Capacity = size;
                for (int y = 0; y < size; y++)
                {
                    MapCell yCell = new MapCell();
                    xCell.Add(yCell);
                }
                grid.Add(xCell);
            }
            
        }

        public List<List<MapCell>> Cells
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

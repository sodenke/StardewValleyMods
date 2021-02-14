using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace NoSoilDecayOnFarm
{
    public class SaveTiles
    {
        public List<Vector2> tiles = new List<Vector2>();

        public string location { get; set; } = "Farm";

        public SaveTiles()
        {
        }

        public SaveTiles(string location, List<Vector2> tiles)
        {
            this.location = location;
            this.tiles = tiles;
        }
    }
}

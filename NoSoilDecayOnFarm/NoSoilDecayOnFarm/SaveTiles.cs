using Microsoft.Xna.Framework;
using Netcode;
using System.Collections.Generic;

namespace NoSoilDecayOnFarm
{
    public class SaveTiles
    {
        public string location { get; set; }
        public List<Vector2> tiles = new List<Vector2>();

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

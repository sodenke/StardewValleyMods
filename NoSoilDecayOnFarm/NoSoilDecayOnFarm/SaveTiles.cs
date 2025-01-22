using Microsoft.Xna.Framework;
using Netcode;
using System.Collections.Generic;

namespace NoSoilDecayOnFarm
{
    public class SaveTiles
    {
        public string location { get; set; }
        public List<Vector2> tiles = new List<Vector2>();
        public Dictionary<Vector2, string> fertilizer { get; set; }

        public SaveTiles()
        {

        }

        public SaveTiles(string location, List<Vector2> tiles, Dictionary<Vector2, string> fertilizer)
        {
            this.location = location;
            this.tiles = tiles;
            this.fertilizer = fertilizer;
        }
    }
}

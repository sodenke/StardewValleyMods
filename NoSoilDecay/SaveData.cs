using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using System.Collections.Generic;

namespace NoSoilDecayOnFarm
{
    public class SaveData
    {
        public List<SaveTiles> data { get; set; } = new List<SaveTiles>();

        public SaveData()
        {
        }

        public SaveData(Dictionary<GameLocation, List<Vector2>> data)
        {
            this.data = new List<SaveTiles>();
            using (Dictionary<GameLocation, List<Vector2>>.Enumerator enumerator = data.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<GameLocation, List<Vector2>> current = enumerator.Current;
                    this.data.Add(new SaveTiles(current.Key.name, current.Value));
                }
            }
        }
    }
}

using StardewValley;
using System.Collections.Generic;

namespace NoSoilDecayOnFarm
{
    public class SaveData
    {
        public List<SaveTiles> data { get; set; } = new List<SaveTiles>();

        public SaveData()
        {
            data = new List<SaveTiles>();
        }

        public SaveData(Dictionary<GameLocation, HoeDirtPlusFertilizer> hoeDirtWithFertilzerCache)
        {
            this.data = new List<SaveTiles>();
            foreach (var hd in hoeDirtWithFertilzerCache)
            {
                data.Add(new SaveTiles(hd.Key.Name, hd.Value.hoedirtLocation, hd.Value.hoeDirt_fertilizer));
            }
        }

    }
}

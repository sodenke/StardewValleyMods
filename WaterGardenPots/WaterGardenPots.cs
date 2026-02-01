using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace WaterGardenPots
{
    public class WaterGardenPots : Mod
    {
        private static Config config;

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<Config>();
            helper.Events.GameLoop.DayStarted += (s, e) => waterGardenPots();
        }

        private void waterGardenPots()
        {
            if (Game1.IsMasterGame)
            {
                foreach (GameLocation l in getAllLocationsAndBuidlings())
                {
                    if (l is GameLocation location)
                    {
                        //Console.WriteLine("Location: " + location.Name);
                        Dictionary<Vector2, StardewValley.Object>.ValueCollection.Enumerator enumerator2 = location.objects.Values.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            if(enumerator2.Current is IndoorPot indoorPot)
                            {
                                //Console.WriteLine("Found indoor pot at " + location.Name + " coords: " + indoorPot.TileLocation.X + " " + indoorPot.TileLocation.Y);
                                bool isInSprinklerRange = IsInSprinklerRange(location, indoorPot.TileLocation);
                                //Console.WriteLine("Is in sprinkler range: " + isInSprinklerRange);
                                if (isInSprinklerRange)
                                {
                                    indoorPot.Water();
                                }
                            }
                        }

                        // should remove this but leaving for now - covered by another mod
                        foreach (Building building in location.buildings)
                        {
                            if (building is PetBowl bowl)
                            {
                                //Console.WriteLine("Found pet bowl.");
                                var locationOfBowl = Utility.PointToVector2(bowl.GetPetSpot());
                                //Console.WriteLine("Found pet bowl at " + locationOfBowl.X + " " + locationOfBowl.Y);
                                bool isInSprinklerRange = IsInSprinklerRange(location, Utility.PointToVector2(bowl.GetPetSpot()));
                                //Console.WriteLine("Is in sprinkler range: " + isInSprinklerRange);
                                if (isInSprinklerRange)
                                {
                                    //Console.WriteLine("Watered pet bowl");
                                    bowl.watered.Value = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool IsInSprinklerRange(GameLocation l, Vector2 v, bool print = false)
        {
            var locations = new List<Vector2>();
            foreach (SObject o in l.objects.Values.Where(obj => obj.Name.Contains("Sprinkler")))
            {
                List<Vector2> list = GetSprinklerTiles(o);
                foreach (var vector in list)
                {
                    locations.Add(vector);
                    if(print)
                    {
                        Console.WriteLine(vector.X + " " + vector.Y);
                    }
                }
            }

            if (locations.Contains(v))
            {
                return true;
            }
            return false;
        }

        public List<Vector2> GetSprinklerTiles(SObject o)
        {
            int modifiedRadiusForSprinkler = o.GetModifiedRadiusForSprinkler() + config.AddRange;
            if (modifiedRadiusForSprinkler == 0)
            {
                return Utility.getAdjacentTileLocations(o.TileLocation);
            }

            if (modifiedRadiusForSprinkler > 0)
            {
                List<Vector2> list = new List<Vector2>();
                for (int i = (int)o.TileLocation.X - modifiedRadiusForSprinkler; (float)i <= o.TileLocation.X + (float)modifiedRadiusForSprinkler; i++)
                {
                    for (int j = (int)o.TileLocation.Y - modifiedRadiusForSprinkler; (float)j <= o.TileLocation.Y + (float)modifiedRadiusForSprinkler; j++)
                    {
                        list.Add(new Vector2(i, j));
                    }
                }

                return list;
            }

            return new List<Vector2>();
        }

        private IEnumerable<GameLocation> getAllLocationsAndBuidlings()
        {
            foreach (GameLocation location in Game1.locations)
            {
                yield return location;
                if (location.IsBuildableLocation())
                {
                    foreach (Building building in location.buildings)
                        if (building.indoors.Value != null)
                            yield return building.indoors.Value;
                }
            }
        }
    }
}

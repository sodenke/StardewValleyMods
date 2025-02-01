using StardewModdingAPI;
using StardewValley.Buildings;
using StardewValley;
using Microsoft.Xna.Framework;
using SObject = StardewValley.Object;

namespace WaterPetBowl
{
    public class WaterPetBowl : Mod
    {
        public override void Entry(IModHelper helper)
        {
            //config = Helper.ReadConfig<Config>();
            helper.Events.GameLoop.DayStarted += (s, e) => waterPetBowls();
            // helper.Events.GameLoop.GameLaunched += (s, e) => addConfig();
        }

        private void waterPetBowls()
        {
            if (Game1.IsMasterGame)
            {
                foreach (GameLocation l in getAllLocationsAndBuidlings())
                {
                    if (l is GameLocation location)
                    {
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
                List<Vector2> list = o.GetSprinklerTiles();
                foreach (var vector in list)
                {
                    locations.Add(vector);
                    if (print)
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

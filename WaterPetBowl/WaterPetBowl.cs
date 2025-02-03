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
            helper.Events.GameLoop.DayStarted += (s, e) => waterPetBowls();
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
                                var coordsOfBowl = new List<Vector2>();
                                var locationOfBowl = Utility.PointToVector2(bowl.GetPetSpot());

                                // add all 4 tiles coords of the pet bowl building
                                coordsOfBowl.Add(locationOfBowl);
                                coordsOfBowl.Add(new Vector2(locationOfBowl.X, locationOfBowl.Y - 1));
                                coordsOfBowl.Add(new Vector2(locationOfBowl.X + 1, locationOfBowl.Y));
                                coordsOfBowl.Add(new Vector2(locationOfBowl.X + 1, locationOfBowl.Y - 1));
 
                                bool isInSprinklerRange = IsInSprinklerRange(location, coordsOfBowl, false);
                                //Console.WriteLine("Is in sprinkler range: " + isInSprinklerRange);
                                if (isInSprinklerRange)
                                {
                                    bowl.watered.Value = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool IsInSprinklerRange(GameLocation l, List<Vector2> coords, bool print = false)
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

            foreach (var coord in coords)
            {
                if (locations.Contains(coord))
                {
                    return true;
                }
            }
            return false;
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

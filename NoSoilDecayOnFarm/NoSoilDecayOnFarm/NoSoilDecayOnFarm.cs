using StardewValley;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using StardewValley.Buildings;
using GenericModConfigMenu;
using System;
using Netcode;

namespace NoSoilDecayOnFarm
{
    public class NoSoilDecayOnFarm : Mod
    {
        private static SaveData savedata;
        private static Config config;

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<Config>();
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += (s,e) => restoreHoeDirt();
            helper.Events.GameLoop.DayEnding += (s, e) => saveHoeDirt();
            helper.Events.GameLoop.GameLaunched += (s, e) => addConfig();
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Game1.IsMasterGame)
            {
                savedata = Helper.Data.ReadSaveData<SaveData>("nsd.save");
                if (savedata == null)
                    savedata = new SaveData();
                restoreHoeDirt();
            }
        }

        private void restoreHoeDirt()
        {
            if (Game1.IsMasterGame)
            {
                foreach (SaveTiles st in savedata.data)
                {
                    foreach (GameLocation l in getAllLocationsAndBuidlings().Where(lb => lb.Name == st.location))
                    {
                        if (config.farmonly && !(l is Farm || l.IsGreenhouse || l.IsBuildableLocation()))
                            continue;

                        foreach (Vector2 v in st.tiles)
                        {
                            if (!l.terrainFeatures.ContainsKey(v) || !(l.terrainFeatures[v] is HoeDirt))
                            {
                                bool isInSprinklerRange = IsInSprinklerRange(l, v);
                                var newHoeDirt = (Game1.isRaining || isInSprinklerRange) ? new HoeDirt(1) : new HoeDirt(0);
                                NetString oldFertilizer = new NetString();
                                if (st.fertilizer != null && st.fertilizer.ContainsKey(v) && st.fertilizer[v] != null && config.reapplyfertilzer)
                                {
                                    oldFertilizer = new NetString(st.fertilizer[v]);
                                    newHoeDirt.plant(oldFertilizer.Value, Game1.player, true);
                                }

                                l.terrainFeatures.Remove(v);
                                l.terrainFeatures.Add(v, newHoeDirt);

                                if (l.objects.Keys.Contains(v) && l.objects[v] is SObject o && (o.Name.Equals("Weeds") || o.Name.Equals("Stone") || o.Name.Equals("Twig")))
                                    l.objects.Remove(v);
                            }
                        }
                    }
                }
            }
        }

        private bool IsInSprinklerRange(GameLocation l, Vector2 v)
        {
            var locations = new List<Vector2>();
            foreach (SObject o in l.objects.Values.Where(obj => obj.Name.Contains("Sprinkler")))
            { 
                List<Vector2> list = o.GetSprinklerTiles();
                foreach (var vector in list)
                {
                    locations.Add(vector);
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

        private void saveHoeDirt()
        {
            if (Game1.IsMasterGame)
            {
                var hoeDirtCache = new Dictionary<GameLocation, HoeDirtPlusFertilizer>();
                foreach (GameLocation location in getAllLocationsAndBuidlings())
                {
                    if (location is GameLocation l)
                    {
                        if (config.farmonly && !(l is Farm || l.IsGreenhouse || l.IsBuildableLocation()))
                            continue;

                        if (!hoeDirtCache.ContainsKey(location))
                            hoeDirtCache.Add(location, new HoeDirtPlusFertilizer());

                        var hd = new HoeDirtPlusFertilizer();
                        foreach (var coords in location.terrainFeatures.Keys)
                        {
                            var tempTile = location.terrainFeatures[coords];
                            if (tempTile is HoeDirt)
                            {
                                hd.hoedirtLocation.Add(coords);
                                if(((HoeDirt)tempTile).fertilizer.Value != null)
                                {
                                    hd.hoeDirt_fertilizer.Add(coords, ((HoeDirt)tempTile).fertilizer.Value);
                                }
                            }
                        }
                        hoeDirtCache[location] = hd;
                    }
                }

                savedata = new SaveData(hoeDirtCache);
                Helper.Data.WriteSaveData("nsd.save", savedata);
            }
        }

        private void addConfig()
        {
            if (!Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
                return;

            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            api.Register(ModManifest, () => config = new Config(), () => Helper.WriteConfig<Config>(config));
            api.AddBoolOption(ModManifest, () => config.farmonly, (value) => config.farmonly = value, () => "Only on Farm-Locations");
            api.AddBoolOption(ModManifest, () => config.reapplyfertilzer, (value) => config.farmonly = value, () => "Re-Apply Fertilizer");
        }
    }
    
}

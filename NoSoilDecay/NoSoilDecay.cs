using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace NoSoilDecayOnFarm
{
    public class NoSoilDecayOnFarm : Mod
    {
        private static SaveData savedata;
        private static Config config;

        public override void Entry(IModHelper helper)
        {
            NoSoilDecayOnFarm.config = (Config)helper.ReadConfig<Config>();
            helper.Events.GameLoop.SaveLoaded += new EventHandler<SaveLoadedEventArgs>(this.OnSaveLoaded);
            helper.Events.GameLoop.DayStarted += (EventHandler<DayStartedEventArgs>)((s, e) => this.restoreHoeDirt());
            helper.Events.GameLoop.DayEnding += (EventHandler<DayEndingEventArgs>)((s, e) => this.saveHoeDirt());
            helper.Events.GameLoop.GameLaunched += (EventHandler<GameLaunchedEventArgs>)((s, e) => this.addConfig());
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Game1.IsMasterGame)
                return;
            NoSoilDecayOnFarm.savedata = (SaveData)this.Helper.Data.ReadSaveData<SaveData>("nsd.save");
            if (NoSoilDecayOnFarm.savedata == null)
                NoSoilDecayOnFarm.savedata = new SaveData();
            this.restoreHoeDirt();
        }

        private void restoreHoeDirt()
        {
            if (!Game1.IsMasterGame)
                return;
            foreach (SaveTiles saveTiles in NoSoilDecayOnFarm.savedata.data)
            {
                SaveTiles st = saveTiles;
                using (IEnumerator<GameLocation> enumerator1 = this.getAllLocationsAndBuidlings().Where<GameLocation>((Func<GameLocation, bool>)(lb => lb.name == st.location)).GetEnumerator())
                {
                    while (((IEnumerator)enumerator1).MoveNext())
                    {
                        GameLocation current = enumerator1.Current;
                        if (!NoSoilDecayOnFarm.config.farmonly || (current is Farm || current.IsGreenhouse || current is BuildableGameLocation))
                        {
                            foreach (Vector2 tile in st.tiles)
                            {
                                if (!((NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>)current.terrainFeatures).ContainsKey(tile) || !(((NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>)current.terrainFeatures)[tile] is HoeDirt))
                                {
                                    ((NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>)current.terrainFeatures).Remove(tile);
                                    ((NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>)current.terrainFeatures).Add(tile, Game1.isRaining != null ? (TerrainFeature)new HoeDirt(1, (GameLocation)null) : (TerrainFeature)new HoeDirt(0, (GameLocation)null));
                                    int num;
                                    if (((IEnumerable<Vector2>)(object)((OverlaidDictionary)current.objects).Keys).Contains<Vector2>(tile))
                                    {
                                        Object @object = ((OverlaidDictionary)current.objects)[tile];
                                        if (@object != null)
                                        {
                                            num = ((Item)@object).Name.Equals("Weeds") || ((Item)@object).Name.Equals("Stone") ? 1 : (((Item)@object).Name.Equals("Twig") ? 1 : 0);
                                            goto label_13;
                                        }
                                    }
                                    num = 0;
                                label_13:
                                    if (num != 0)
                                        ((OverlaidDictionary)current.objects).Remove(tile);
                                }
                            }
                            using (IEnumerator<Object> enumerator2 = ((IEnumerable<Object>)(object)((OverlaidDictionary)current.objects).Values).Where<Object>((Func<Object, bool>)(obj => obj.name.Contains("Sprinkler"))).GetEnumerator())
                            {
                                while (((IEnumerator)enumerator2).MoveNext())
                                    enumerator2.Current.DayUpdate(current);
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<GameLocation> getAllLocationsAndBuidlings()
        {
            List<GameLocation> locations = new List<GameLocation>();
            locations.Add((StardewValley.GameLocation)Game1.getFarm());
            return locations;
        }


        private void saveHoeDirt()
        {
            if (!Game1.IsMasterGame)
                return;
            Dictionary<GameLocation, List<Vector2>> data = new Dictionary<GameLocation, List<Vector2>>();
            using (IEnumerator<GameLocation> enumerator = this.getAllLocationsAndBuidlings().GetEnumerator())
            {
                while (((IEnumerator)enumerator).MoveNext())
                {
                    GameLocation location = enumerator.Current;
                    GameLocation gameLocation = null;
                    int num;
                    if (location != null)
                    {
                        gameLocation = location;
                        num = 1;
                    }
                    else
                        num = 0;
                    if (num != 0 && (!NoSoilDecayOnFarm.config.farmonly || (gameLocation is Farm || gameLocation.IsGreenhouse || gameLocation is BuildableGameLocation)))
                    {
                        if (!data.ContainsKey(location))
                            data.Add(location, new List<Vector2>());
                        data[location] = ((IEnumerable<Vector2>)(object)((NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>)location.terrainFeatures).Keys).Where<Vector2>((Func<Vector2, bool>)(t => ((NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>)location.terrainFeatures)[t] is HoeDirt)).ToList<Vector2>();
                    }
                }
            }
            NoSoilDecayOnFarm.savedata = new SaveData(data);
            this.Helper.Data.WriteSaveData<SaveData>("nsd.save", NoSoilDecayOnFarm.savedata);
        }

        private void addConfig()
        {
            if (!this.Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
                return;
            IGMCMAPI api = (IGMCMAPI)this.Helper.ModRegistry.GetApi<IGMCMAPI>("spacechase0.GenericModConfigMenu");
            api.RegisterModConfig(this.ModManifest, (Action)(() => NoSoilDecayOnFarm.config = new Config()), (Action)(() => this.Helper.WriteConfig<Config>(NoSoilDecayOnFarm.config)));
            api.RegisterSimpleOption(this.ModManifest, "Only on Farm-Locations", "", (Func<bool>)(() => NoSoilDecayOnFarm.config.farmonly), (Action<bool>)(value => NoSoilDecayOnFarm.config.farmonly = value));
        }
    }
}

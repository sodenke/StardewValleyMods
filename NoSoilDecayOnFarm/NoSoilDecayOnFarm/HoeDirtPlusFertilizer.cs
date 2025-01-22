using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Netcode;

namespace NoSoilDecayOnFarm
{
    public class HoeDirtPlusFertilizer
    {
        public List<Vector2> hoedirtLocation = new List<Vector2>();
        public Dictionary<Vector2, string> hoeDirt_fertilizer = new Dictionary<Vector2, string>();
    }
}

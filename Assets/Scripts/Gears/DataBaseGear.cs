using System.Collections.Generic;
using System.Linq;
using _ScriptableObject;
using UnityEngine;

namespace Gears
{
    [CreateAssetMenu(fileName = "DataBase_Gear", menuName = "Scriptable Object/DataBase/Gear")]
    public class DataBaseGear : ScriptableObject
    {
        [SerializeField] private List<GearSO> gears;

        public List<GearSO> CommonGear => gears.Where(_gear => _gear.Rarity.Affixes == 1).ToList();
        public List<GearSO> MagicGear => gears.Where(_gear => _gear.Rarity.Affixes == 2).ToList();
        public List<GearSO> RareGear => gears.Where(_gear => _gear.Rarity.Affixes == 3).ToList();
        public List<GearSO> LegendGear => gears.Where(_gear => _gear.Rarity.Affixes == 4).ToList();

        private const int weightTotal = 20;
        private const int weightMagic = 10;
        private const int weightRare = 5;
        private const int weightLegendary = 2;

        private List<GearSO> GetRandomRarityList(int _random1to20)
        {
            if (_random1to20 < weightLegendary) return LegendGear;
            if (_random1to20 < weightRare) return RareGear;
            if (_random1to20 < weightMagic) return MagicGear;
            return CommonGear;
        }
        public GearSO GetRandom()
        {
            /*
            List<GearSO> Rarity = GetRandomRarityList(Random.Range(0, weightTotal));
            return Rarity[Random.Range(0, Rarity.Count)];
            */
            return gears[Random.Range(0, gears.Count)];
        }
    }
}
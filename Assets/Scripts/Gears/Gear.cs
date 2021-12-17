using System;
using System.Collections.Generic;
using _Instances;
using Stats;

namespace Gears
{
    [Serializable]
    public class Gear
    {
        public GearSO GearSO { get; private set; }
        public List<Affix> Affixes { get; private set; }
        public int Stage { get; private set; }

        public BattleStats GetStats()
        {
            BattleStats ret = new BattleStats(0);
            Affixes.ForEach(affix => ret += affix.affix.GenerateBS(affix.value));
            ret += GearSO.MainStat.affix.GenerateBS(GearSO.MainStat.value);
            return ret;
        }
        
        /// <summary>
        /// called on Gear Random Creation
        /// </summary>
        /// <param name="lvl"> the stage in the game (1 2 or 3)</param>
        public void CreateGear(int lvl)
        {
            GearSO = DataBase.Gear.GetRandom();
            Affixes = new List<Affix>();
            Stage = lvl;

            List<AffixSO> nonAffixes = new List<AffixSO>();
            nonAffixes.AddRange(GearSO.NonAffixs);
            for (int i = 0; i < GearSO.Rarity.Affixes; i++)
            {
                if (DataBase.Affix.Affixes.Count <= nonAffixes.Count) break;
                AffixSO affix = DataBase.Affix.GetRandomBut(nonAffixes);
                int value = affix.getValue(Math.Min(lvl,affix.Tier.Length-1));
                Affixes.Add(new Affix(affix, value));
                nonAffixes.Add(affix);
            }
        }

        public Gear CraftNewGear(Dictionary<AffixSO, int> _gearStats)
        {
            GearSO = DataBase.Gear.GetRandom();
            Affixes = new List<Affix>();
            Stage = KeepBetweenScene.Stage;

            foreach (KeyValuePair<AffixSO,int> _pair in _gearStats)
            {
                int value = _pair.Key.getValue(Math.Min(_pair.Key.Tier.Length, Math.Max(0, _pair.Value - 1)));
                Affixes.Add(new Affix(_pair.Key, value));
            }

            return this;
        }
        
        public void CreateGearObject(Gear _gear)
        {
            GearSO = _gear.GearSO;
            Affixes = _gear.Affixes;
            Stage = _gear.Stage;
        }

        public override string ToString()
        {
            return $"{GearSO.Name}, Affix 1 = {Affixes[0].ToString()}";
        }

        public void SetAffixes(List<Affix> newAffixes)
        {
            Affixes = newAffixes;
        }
    }
}
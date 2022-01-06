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

        public Gear() {}
        public Gear(Gear gear)
        {
            GearSO = gear.GearSO;
            Affixes = gear.Affixes;
            Stage = gear.Stage;
        }

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
        /// <param name="_stage"> the stage in the game (1 2 or 3)</param>
        public void CreateGear()
        {
            GearSO = DataBase.Gear.GetRandom();
            Affixes = new List<Affix>();
            Stage = KeepBetweenScene.Stage;

            List<AffixSO> nonAffixes = new List<AffixSO>();
            nonAffixes.AddRange(GearSO.NonAffixs);
            for (int i = 0; i < GearSO.Rarity.Affixes; i++)
            {
                if (DataBase.Affix.Affixes.Count <= nonAffixes.Count) break;
                AffixSO affix = DataBase.Affix.GetRandomBut(nonAffixes);
                int value = affix.getValue(Stage);
                if (value == 0)
                {
                    i--;
                    nonAffixes.Add(affix);
                    continue;
                }
                Affixes.Add(new Affix(affix, value, Stage + 1));
                nonAffixes.Add(affix);
            }
        }

        public Gear CraftNewGear(Dictionary<AffixSO, int> _gearStats)
        {
            GearSO = DataBase.Gear.GetRandom();
            Affixes = new List<Affix>();
            Stage = KeepBetweenScene.Stage;

            foreach (AffixSO _affix in _gearStats.Keys)
            {
                int value = _affix.getValueOfTier(_gearStats[_affix]);
                Affixes.Add(new Affix(_affix, value, _gearStats[_affix]));
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
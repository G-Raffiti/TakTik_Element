using System;
using System.Collections.Generic;
using _Instances;
using Stats;

namespace Gears
{
    [Serializable]
    public class Gear
    {
        public GearSo GearSo { get; private set; }
        public List<Affix> Affixes { get; private set; }
        public int Stage { get; private set; }

        public Gear() {}
        public Gear(Gear _gear)
        {
            GearSo = _gear.GearSo;
            Affixes = _gear.Affixes;
            Stage = _gear.Stage;
        }

        public BattleStats GetStats()
        {
            BattleStats _ret = new BattleStats(0);
            Affixes.ForEach(_affix => _ret += _affix.affix.GenerateBs(_affix.value));
            _ret += GearSo.MainStat.affix.GenerateBs(GearSo.MainStat.value);
            return _ret;
        }
        
        /// <summary>
        /// called on Gear Random Creation
        /// </summary>
        /// <param name="_stage"> the stage in the game (1 2 or 3)</param>
        public void CreateGear()
        {
            GearSo = DataBase.Gear.GetRandom();
            Affixes = new List<Affix>();
            Stage = BattleStage.Stage;

            List<AffixSo> _nonAffixes = new List<AffixSo>();
            _nonAffixes.AddRange(GearSo.NonAffixs);
            for (int _i = 0; _i < GearSo.Rarity.Affixes; _i++)
            {
                if (DataBase.Affix.Affixes.Count <= _nonAffixes.Count) break;
                AffixSo _affix = DataBase.Affix.GetRandomBut(_nonAffixes);
                int _value = _affix.GetValue(Stage);
                if (_value == 0)
                {
                    _i--;
                    _nonAffixes.Add(_affix);
                    continue;
                }
                Affixes.Add(new Affix(_affix, _value, Stage + 1));
                _nonAffixes.Add(_affix);
            }
        }

        public Gear CraftNewGear(Dictionary<AffixSo, int> _gearStats)
        {
            GearSo = DataBase.Gear.GetRandom();
            Affixes = new List<Affix>();
            Stage = BattleStage.Stage;

            foreach (AffixSo _affix in _gearStats.Keys)
            {
                int _value = _affix.GetValueOfTier(_gearStats[_affix]);
                Affixes.Add(new Affix(_affix, _value, _gearStats[_affix]));
            }

            return this;
        }
        
        public void CreateGearObject(Gear _gear)
        {
            GearSo = _gear.GearSo;
            Affixes = _gear.Affixes;
            Stage = _gear.Stage;
        }

        public override string ToString()
        {
            return $"{GearSo.Name}, Affix 1 = {Affixes[0].ToString()}";
        }

        public void SetAffixes(List<Affix> _newAffixes)
        {
            Affixes = _newAffixes;
        }
    }
}
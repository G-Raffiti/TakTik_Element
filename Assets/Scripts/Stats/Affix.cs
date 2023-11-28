using System;
using DataBases;
using UnityEngine;

namespace Stats
{
    [Serializable]
    public struct Affix
    {
        public AffixSo affix;
        public float value;
        public int min;
        public int max;
        public int tier;

        public Affix(AffixSo _affix, float _value, int _tier)
        {
            affix = _affix;
            value = _value;
            tier = _tier;
            min = affix.Tier[Math.Max(0, _tier - 1)];
            max = affix.Tier[Math.Max(1, Math.Min(_tier, affix.Tier.Length -1))];
        }

        public override string ToString()
        {
            return value >= 0 ? $"+ {(int) value} {affix.Name} " : $"- {(int) value} {affix.Name} ";
        }

        public string ValueToString(int _value)
        {
            return $"{_value} {affix.Icon}";
        }
        
        public string TierRangeToString()
        {
            return $"t{tier} ({min} - {max})";
        }
    }
}
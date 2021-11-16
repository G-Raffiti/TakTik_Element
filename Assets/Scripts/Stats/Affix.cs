using System;

namespace Stats
{
    [Serializable]
    public struct Affix
    {
        public AffixSO affix;
        public float value;

        public EAffix Type => affix.Type;

        public Affix(AffixSO _affix, float _value)
        {
            affix = _affix;
            value = _value;
        }

        public override string ToString()
        {
            return value > 0 ? $"+ {(int) value} {affix.Name} " : $"- {(int) value} {affix.Name} ";
        }

        public string Value(int _value)
        {
            return $"{_value} {affix.Icon(affix.Type)}";
        }

        public int getTier()
        {
            int tier = 0;
            foreach (int _tier in affix.Tier)
            {
                if (value >= _tier)
                    tier += 1;
                else break;
            }

            return tier;
        }
    }
}
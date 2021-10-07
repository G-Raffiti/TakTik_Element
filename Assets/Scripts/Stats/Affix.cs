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
    }
}
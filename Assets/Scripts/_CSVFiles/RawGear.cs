using System;
using System.Collections.Generic;
using _Instances;
using _ScriptableObject;
using DataBases;
using Gears;
using Skills.ScriptableObject_GridEffect;
using Stats;
using UnityEngine;

namespace _CSVFiles
{
    public struct RawGear
    {
        public readonly Rarity Rarity;
        public readonly EGear Type;
        public readonly Sprite Icon;
        public readonly string Name;
        public Affix MainStat;
        public readonly SkillGridEffect SpecialEffect;
        public readonly List<AffixSO> NonAffix;

        public RawGear(Dictionary<string, object> _csvGear)
        {
            Rarity = UnityEngine.Resources.Load<Rarity>(
                $"ScriptableObject/Rarities/Rarity_{_csvGear["Rarity"]}");
            Enum.TryParse(_csvGear["Type"].ToString(), out Type);
            Icon = UnityEngine.Resources.Load<Sprite>(
                $"Sprite/2000_Icons/{_csvGear["Icon"]}");
            Name = _csvGear["Name"].ToString();
            Enum.TryParse(_csvGear[$"Implicit"].ToString(), out EAffix _mainStat);
            AffixSO _affix = DataBase.Affix.AllAffixes.Find(_a => _a.Type == _mainStat);
            float.TryParse(_csvGear["ImplicitValue"].ToString(), out float _value);
            MainStat = new Affix(_affix, _value, 1);
            SpecialEffect = UnityEngine.Resources.Load<SkillGridEffect>(
                $"ScriptableObject/SkillEffects/GridEffect_{_csvGear["SpecialEffect"]}");
            NonAffix = new List<AffixSO>();
            for (int _i = 0; _i < 11; _i++)
            {
                if(_csvGear[$"Non{_i}"].ToString() == String.Empty) continue;
                Enum.TryParse(_csvGear[$"Non{_i}"].ToString(), out EAffix _nonAffix);
                AffixSO _toAdd = DataBase.Affix.AllAffixes.Find(_a => _a.Type == _nonAffix);
                if (!NonAffix.Contains(_toAdd))
                    NonAffix.Add(_toAdd);
            }
        }
    }
}
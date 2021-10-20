using System;
using System.Collections.Generic;
using _Instances;
using _ScriptableObject;
using Skills.ScriptableObject_GridEffect;
using Stats;
using UnityEngine;

namespace Gears
{
    public struct rawGear
    {
        public Rarity Rarity;
        public EGear Type;
        public Sprite Icon;
        public string Name;
        public Affix MainStat;
        public SkillGridEffect SpecialEffect;
        public List<AffixSO> NonAffix;

        public rawGear(Dictionary<string, object> CSVGear)
        {
            Rarity = UnityEngine.Resources.Load<Rarity>(
                $"ScriptableObject/Rarities/Rarity_{CSVGear["Rarity"]}");
            Enum.TryParse(CSVGear["Type"].ToString(), out Type);
            Icon = UnityEngine.Resources.Load<Sprite>(
                $"Sprite/2000_Icons/{CSVGear["Icon"]}");
            Name = CSVGear["Name"].ToString();
            Enum.TryParse(CSVGear[$"Implicit"].ToString(), out EAffix mainStat);
            AffixSO affix = DataBase.Affix.AllAffixes.Find(a => a.Type == mainStat);
            float.TryParse(CSVGear["ImplicitValue"].ToString(), out float value);
            MainStat = new Affix(affix, value);
            SpecialEffect = UnityEngine.Resources.Load<SkillGridEffect>(
                $"ScriptableObject/SkillEffects/GridEffect_{CSVGear["SpecialEffect"]}");
            NonAffix = new List<AffixSO>();
            for (int i = 0; i < DataBase.Affix.Affixes.Count; i++)
            {
                if(CSVGear[$"Non{i}"].ToString() == String.Empty) continue;
                Enum.TryParse(CSVGear[$"Non{i}"].ToString(), out EAffix nonAffix);
                NonAffix.Add(DataBase.Affix.AllAffixes.Find(a => a.Type == nonAffix));
            }
        }
    }
}
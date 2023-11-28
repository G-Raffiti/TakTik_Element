using System;
using System.Collections.Generic;
using _ScriptableObject;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Skills.ScriptableObject_GridEffect;
using StatusEffect;
using Units;
using UnityEngine;

namespace _CSVFiles
{
    public struct RawSkill
    {
        public readonly string Name;
        public readonly EArchetype Archetype;
        public readonly Element Element;
        public readonly int Cost;
        public readonly SkillEffect Effect1;
        public readonly SkillEffect Effect2;
        public readonly SkillEffect Effect3;
        public readonly SkillGridEffect GridEffect;
        public readonly StatusSO Status;
        public readonly EZone RangeType;
        public readonly int RangeValue;
        public readonly EZone ZoneType;
        public readonly int Radius;
        public readonly bool NeedView;
        public readonly bool NeedTarget;
        public readonly EAffect Affect;
        public readonly bool CanBeModified;
        public readonly int Power;
        public readonly bool Consumable;
        public readonly Sprite Icon;

        public RawSkill(bool _no)
        {
            Name = "";
            Archetype = EArchetype.None;
            Element = Element.None();
            Cost = 0;
            Effect1 = null;
            Effect2 = null;
            Effect3 = null;
            GridEffect = null;
            Status = null;
            RangeType = EZone.Basic;
            RangeValue = 0;
            ZoneType = EZone.Basic;
            Radius = 0;
            NeedView = true;
            NeedTarget = false;
            Affect = EAffect.All;
            CanBeModified = true;
            Power = 0;
            Consumable = false;
            Icon = null;
        }

        public RawSkill(Dictionary<string, object> _csvSkill)
        {
            this = new RawSkill(false);
            Name = _csvSkill["Name"].ToString();
            Enum.TryParse(_csvSkill["Archetype"].ToString(), out Archetype);
            Element = UnityEngine.Resources.Load<Element>(
                $"ScriptableObject/Elements/Element_{_csvSkill["Element"]}");
            int.TryParse(_csvSkill["Cost"].ToString(), out Cost);
            Effect1 = UnityEngine.Resources.Load<SkillEffect>(
                $"ScriptableObject/SkillEffects/SkillEffect_{_csvSkill["Effect1"]}");
            Effect2 = UnityEngine.Resources.Load<SkillEffect>(
                $"ScriptableObject/SkillEffects/SkillEffect_{_csvSkill["Effect2"]}");
            Effect3 = UnityEngine.Resources.Load<SkillEffect>(
                $"ScriptableObject/SkillEffects/SkillEffect_{_csvSkill["Effect3"]}");
            GridEffect = UnityEngine.Resources.Load<SkillGridEffect>(
                    $"ScriptableObject/SkillEffects/GridEffect_{_csvSkill["GridEffect"]}");
            Status = UnityEngine.Resources.Load<StatusSO>(
                $"ScriptableObject/StatusEffect/Status_{_csvSkill["Status"]}");
            Enum.TryParse(_csvSkill["RangeType"].ToString(), out RangeType);
            int.TryParse(_csvSkill["RangeValue"].ToString(), out RangeValue);
            Enum.TryParse(_csvSkill["ZoneType"].ToString(), out ZoneType);
            int.TryParse(_csvSkill["Radius"].ToString(), out Radius);
            bool.TryParse(_csvSkill["NeedView"].ToString(), out NeedView);
            bool.TryParse(_csvSkill["NeedTarget"].ToString(), out NeedTarget);
            Enum.TryParse(_csvSkill["Affect"].ToString(), out Affect);
            bool.TryParse(_csvSkill["CanBeModified"].ToString(), out CanBeModified);
            int.TryParse(_csvSkill["Power"].ToString(), out Power);
            bool.TryParse(_csvSkill["Consumable"].ToString(), out Consumable);
            Icon = UnityEngine.Resources.Load<Sprite>(
                $"Sprite/2000_Icons/All_Skill/{_csvSkill["Icon"]}");
        }

    }
}
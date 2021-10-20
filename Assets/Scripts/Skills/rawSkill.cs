using System;
using System.Collections.Generic;
using _Instances;
using _ScriptableObject;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Skills.ScriptableObject_GridEffect;
using Stats;
using StatusEffect;
using Units;
using UnityEngine;
using Resources = UnityEngine.Resources;

namespace Skills
{
    public struct rawSkill
    {
        public string Name;
        public EArchetype Archetype;
        public Element Element;
        public int Cost;
        public SkillEffect Effect1;
        public SkillEffect Effect2;
        public SkillEffect Effect3;
        public SkillGridEffect GridEffect;
        public StatusSO Status;
        public EZone RangeType;
        public int RangeValue;
        public EZone ZoneType;
        public int Radius;
        public bool NeedView;
        public bool NeedTarget;
        public EAffect Affect;
        public bool CanBeModified;
        public int Power;
        public bool Consumable;
        public Sprite Icon;

        public rawSkill(bool no)
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

        public rawSkill(Dictionary<string, object> CSVSkill)
        {
            this = new rawSkill(false);
            Name = CSVSkill["Name"].ToString();
            Enum.TryParse(CSVSkill["Archetype"].ToString(), out Archetype);
            Element = UnityEngine.Resources.Load<Element>(
                $"ScriptableObject/Elements/Element_{CSVSkill["Element"]}");
            int.TryParse(CSVSkill["Cost"].ToString(), out Cost);
            Effect1 = UnityEngine.Resources.Load<SkillEffect>(
                $"ScriptableObject/SkillEffects/SkillEffect_{CSVSkill["Effect1"]}");
            Effect2 = UnityEngine.Resources.Load<SkillEffect>(
                $"ScriptableObject/SkillEffects/SkillEffect_{CSVSkill["Effect2"]}");
            Effect3 = UnityEngine.Resources.Load<SkillEffect>(
                $"ScriptableObject/SkillEffects/SkillEffect_{CSVSkill["Effect3"]}");
            GridEffect = UnityEngine.Resources.Load<SkillGridEffect>(
                    $"ScriptableObject/SkillEffects/GridEffect_{CSVSkill["GridEffect"]}");
            Status = UnityEngine.Resources.Load<StatusSO>(
                $"ScriptableObject/StatusEffect/Status_{CSVSkill["Status"]}");
            Enum.TryParse(CSVSkill["RangeType"].ToString(), out RangeType);
            int.TryParse(CSVSkill["RangeValue"].ToString(), out RangeValue);
            Enum.TryParse(CSVSkill["ZoneType"].ToString(), out ZoneType);
            int.TryParse(CSVSkill["Radius"].ToString(), out Radius);
            bool.TryParse(CSVSkill["NeedView"].ToString(), out NeedView);
            bool.TryParse(CSVSkill["NeedTarget"].ToString(), out NeedTarget);
            Enum.TryParse(CSVSkill["Affect"].ToString(), out Affect);
            bool.TryParse(CSVSkill["CanBeModified"].ToString(), out CanBeModified);
            int.TryParse(CSVSkill["Power"].ToString(), out Power);
            bool.TryParse(CSVSkill["Consumable"].ToString(), out Consumable);
            Icon = UnityEngine.Resources.Load<Sprite>(
                $"Sprite/2000_Icons/All_Skill/{CSVSkill["Icon"]}");
        }

    }
}
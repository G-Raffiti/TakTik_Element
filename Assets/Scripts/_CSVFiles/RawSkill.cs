using System;
using System.Collections.Generic;
using _ScriptableObject;
using Buffs;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Skills.ScriptableObject_GridEffect;
using Units;
using UnityEngine;

namespace _CSVFiles
{
    public struct RawSkill
    {
        public string Name;
        public EArchetype Archetype;
        public Element Element;
        public int Cost;
        public SkillEffect Effect1;
        public SkillEffect Effect2;
        public SkillEffect Effect3;
        public SkillGridEffect GridEffect;
        public StatusSo Status;
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
        public MonsterSo Monster;

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
            Monster = null;
        }

        public RawSkill(Dictionary<string, object> _cvsSkill)
        {
            this = new RawSkill(false);
            Name = _cvsSkill["Name"].ToString();
            Enum.TryParse(_cvsSkill["Archetype"].ToString(), out Archetype);
            Element = UnityEngine.Resources.Load<Element>(
                $"ScriptableObject/Elements/Element_{_cvsSkill["Element"]}");
            int.TryParse(_cvsSkill["Cost"].ToString(), out Cost);
            Effect1 = UnityEngine.Resources.Load<SkillEffect>(
                $"ScriptableObject/SkillEffects/SkillEffect_{_cvsSkill["Effect1"]}");
            Effect2 = UnityEngine.Resources.Load<SkillEffect>(
                $"ScriptableObject/SkillEffects/SkillEffect_{_cvsSkill["Effect2"]}");
            Effect3 = UnityEngine.Resources.Load<SkillEffect>(
                $"ScriptableObject/SkillEffects/SkillEffect_{_cvsSkill["Effect3"]}");
            GridEffect = UnityEngine.Resources.Load<SkillGridEffect>(
                    $"ScriptableObject/SkillEffects/GridEffect_{_cvsSkill["GridEffect"]}");
            Status = UnityEngine.Resources.Load<StatusSo>(
                $"ScriptableObject/StatusEffect/Status_{_cvsSkill["Status"]}");
            Enum.TryParse(_cvsSkill["RangeType"].ToString(), out RangeType);
            int.TryParse(_cvsSkill["RangeValue"].ToString(), out RangeValue);
            Enum.TryParse(_cvsSkill["ZoneType"].ToString(), out ZoneType);
            int.TryParse(_cvsSkill["Radius"].ToString(), out Radius);
            bool.TryParse(_cvsSkill["NeedView"].ToString(), out NeedView);
            bool.TryParse(_cvsSkill["NeedTarget"].ToString(), out NeedTarget);
            Enum.TryParse(_cvsSkill["Affect"].ToString(), out Affect);
            bool.TryParse(_cvsSkill["CanBeModified"].ToString(), out CanBeModified);
            int.TryParse(_cvsSkill["Power"].ToString(), out Power);
            bool.TryParse(_cvsSkill["Consumable"].ToString(), out Consumable);
            Icon = UnityEngine.Resources.Load<Sprite>(
                $"Sprite/2000_Icons/All_Skill/{_cvsSkill["Icon"]}");
            bool.TryParse(_cvsSkill["BaseSkill"].ToString(), out bool _basic);
            if (!_basic)
                Monster = UnityEngine.Resources.Load<MonsterSo>(
                $"ScriptableObject/Monsters/{_cvsSkill["MonsterType"]}_{Archetype}_{Element.Type}_{_cvsSkill["MonsterName"]}");
        }

    }
}
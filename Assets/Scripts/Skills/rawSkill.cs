using System;
using System.Collections.Generic;
using _Instances;
using _ScriptableObject;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Skills.ScriptableObject_GridEffect;
using Stats;
using UnityEngine;

namespace Skills
{
    public struct rawSkill
    {
        public string Name;
        public Element Element;
        public int Cost;
        public SkillEffect Effect1;
        public SkillEffect Effect2;
        public SkillEffect Effect3;
        public SkillGridEffect GridEffect;
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
            Element = Element.None();
            Cost = 0;
            Effect1 = null;
            Effect2 = null;
            Effect3 = null;
            GridEffect = null;
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
            if (Enum.TryParse(CSVSkill["Element"].ToString(), out EElement ele))
                Element = DataBase.Skill.GetElement(ele);
            int.TryParse(CSVSkill["Cost"].ToString(), out Cost);
            Effect1 = null;
            Effect2 = null;
            Effect3 = null;
            GridEffect = null;
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
            Icon = null;
        }
    }
}
using System.Collections.Generic;
using _CSVFiles;
using _ScriptableObject;
using Buffs;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Skills.ScriptableObject_GridEffect;
using Stats;
using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace Skills
{
    [CreateAssetMenu(fileName = "Skill_", menuName = "Scriptable Object/New Skill")]
    public class SkillSo : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private Element element;
        [SerializeField] private Sprite icon;
        
        [SerializeField] private List<SkillEffect> effects;
        [SerializeField] private SkillGridEffect gridEffect;
        [SerializeField] private EAffect affect;
        
        [FormerlySerializedAs("range")] [SerializeField] private GridRange gridRange;
        [SerializeField] private int power;
        [SerializeField] private List<StatusSo> statusEffects;

        [SerializeField] private bool consumable;
        [SerializeField] private int cost;
        [SerializeField] private EArchetype archetype;
        [SerializeField] private MonsterSo monster;

        public string Name => name;
        public Element Element => element;
        public Sprite Icon => icon;
        public ESkill Type => effects[0].Type;
        public List<SkillEffect> Effects => effects;
        public SkillGridEffect GridEffect => gridEffect;
        public EAffect Affect => affect;
        public GridRange GridRange => gridRange;
        public int Power => power;
        public List<StatusSo> StatusEffects => statusEffects;
        public bool Consumable => consumable;
        public int Cost => cost;
        public EArchetype Archetype => archetype;
        public MonsterSo Monster => monster;

        public void SetData(RawSkill _rawSkill)
        {
            name = _rawSkill.Name;
            element = _rawSkill.Element;
            affect = _rawSkill.Affect;
            gridRange = new GridRange
            {
                rangeValue = _rawSkill.RangeValue,
                canBeModified = _rawSkill.CanBeModified,
                needTarget = _rawSkill.NeedTarget,
                needView = _rawSkill.NeedView,
                radius = _rawSkill.Radius,
                rangeType = _rawSkill.RangeType,
                zoneType = _rawSkill.ZoneType,
            };
            power = _rawSkill.Power;
            consumable = _rawSkill.Consumable;
            cost = _rawSkill.Cost;

            archetype = _rawSkill.Archetype;
            effects = new List<SkillEffect>();
            if (_rawSkill.Effect1 != null) effects.Add(_rawSkill.Effect1);
            if (_rawSkill.Effect2 != null) effects.Add(_rawSkill.Effect2);
            if (_rawSkill.Effect3 != null) effects.Add(_rawSkill.Effect3);

            if (_rawSkill.GridEffect != null) gridEffect = _rawSkill.GridEffect;

            statusEffects = new List<StatusSo>();
            if (_rawSkill.Status != null) statusEffects.Add(_rawSkill.Status);

            icon = _rawSkill.Icon;
            monster = _rawSkill.Monster;
        }
    }
}
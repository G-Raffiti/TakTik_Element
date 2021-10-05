using System.Collections.Generic;
using _ScriptableObject;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Skills.ScriptableObject_GridEffect;
using Stats;
using StatusEffect;
using UnityEngine;

namespace Skills
{
    [CreateAssetMenu(fileName = "Skill_", menuName = "Scriptable Object/New Skill")]
    public class SkillSO : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private Element element;
        [SerializeField] private Sprite icon;
        
        [SerializeField] private List<SkillEffect> effects;
        [SerializeField] private SkillGridEffect gridEffect;
        [SerializeField] private EAffect affect;
        
        [SerializeField] private Range range;
        [SerializeField] private Power power;
        [SerializeField] private List<StatusSO> statusEffects;

        [SerializeField] private bool consumable;
        [SerializeField] private int cost;

        public string Name => name;
        public Element Element => element;
        public Sprite Icon => icon;
        public ESkill Type => effects[0].Type;
        public List<SkillEffect> Effects => effects;
        public SkillGridEffect GridEffect => gridEffect;
        public EAffect Affect => affect;
        public Range Range => range;
        public Power Power => power;
        public List<StatusSO> StatusEffects => statusEffects;
        public bool Consumable => consumable;
        public int Cost => cost;
    }
}
﻿using System.Collections.Generic;
using _ScriptableObject;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Skills.ScriptableObject_GridEffect;
using Skills.ScriptableObject_RelicEffect;
using Stats;
using StatusEffect;
using UnityEngine;

namespace Skills
{
    [CreateAssetMenu(fileName = "Relic_", menuName = "Scriptable Object/New Relic")]
    public class RelicSO : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private Element element;
        [SerializeField] private Sprite icon;

        [SerializeField] private List<RelicEffect> relicEffects;
        [SerializeField] private SkillEffect effect;
        [SerializeField] private SkillGridEffect gridEffect;
        [SerializeField] private EAffect affect;
        
        [SerializeField] private Range range;
        [SerializeField] private Power power;
        [SerializeField] private List<StatusSO> statusEffects;

        public string Name => name;
        public Element Element => element;
        public Sprite Icon => icon;
        public List<RelicEffect> RelicEffects => relicEffects;
        public SkillEffect Effect => effect;
        public SkillGridEffect GridEffect => gridEffect;
        public EAffect Affect => affect;
        public Range Range => range;
        public Power Power => power;
        public List<StatusSO> StatusEffects => statusEffects;
    }
}
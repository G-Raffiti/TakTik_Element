using System.Collections.Generic;
using _ScriptableObject;
using Buffs;
using Relics.ScriptableObject_RelicEffect;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Skills.ScriptableObject_GridEffect;
using Stats;
using UnityEngine;

namespace Relics
{
    [CreateAssetMenu(fileName = "Relic_", menuName = "Scriptable Object/New Relic")]
    public class RelicSO : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private Element element;
        [SerializeField] private Sprite icon;

        [SerializeField] private List<RelicEffect> relicEffects;
        [SerializeField] private float effectFactor;
        [SerializeField] private SkillEffect effect;
        [SerializeField] private SkillGridEffect gridEffect;
        [SerializeField] private EAffect affect;

        [SerializeField] private BattleStats battleStats;
        [SerializeField] private int cost;
        [SerializeField] private List<StatusSO> statusEffects;

        [SerializeField] private string description;
        [SerializeField] private string flavour;

        public string Name => name;
        public Element Element => element;
        public Sprite Icon => icon;
        public List<RelicEffect> RelicEffects => relicEffects;
        public SkillEffect Effect => effect;
        public SkillGridEffect GridEffect => gridEffect;
        public EAffect Affect => affect;
        public float EffectFactor => effectFactor;
        public BattleStats BattleStats => battleStats;

        public int Cost => cost;
        public List<StatusSO> StatusEffects => statusEffects;

        public string Description => description;
        public string Flavour => flavour;
    }
}
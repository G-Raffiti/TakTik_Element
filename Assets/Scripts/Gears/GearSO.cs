using System.Collections.Generic;
using _ScriptableObject;
using Skills.ScriptableObject_GridEffect;
using Stats;
using UnityEngine;

namespace Gears
{
    public enum EGear {OneHand, TwoHand, Body, Boots, Helm, Neckless}
    
    [CreateAssetMenu(fileName = "Gear_", menuName = "Scriptable Object/New Gear")]
    public class GearSO : ScriptableObject
    {
        [SerializeField] private Rarity rarity;
        public Rarity Rarity => rarity;
        [SerializeField] private EGear type;
        public EGear Type => type;
        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
        [SerializeField] private new string name;
        public string Name => name;
        [SerializeField] private Affix mainStat;
        public Affix MainStat => mainStat;
        [SerializeField] private SkillGridEffect specialEffect;
        public SkillGridEffect SpecialEffect => specialEffect;

        /// <summary>
        /// Affixes that can't be generate on this type of Item
        /// </summary>
        [SerializeField] private List<AffixSO> nonAffixs;
        /// <summary>
        /// Affixes that can't be generate on this type of Item
        /// </summary>
        public List<AffixSO> NonAffixs => nonAffixs;
    }
}
using System.Collections.Generic;
using System.Linq;
using _Instances;
using UnityEngine;

namespace Stats
{
    public enum EAffix
    {
        HP,
        AP,
        MP,
        Speed,
        Shield,
        Dodge,
        
        Dext,
        Strength,
        Intel,

        Affinity,
        Fire,
        Water,
        Nature,
        
        BasicPower,
        Focus,
        Power,
        SkillPower,
        SpellPower,
        
        Range,
        Zone,
    }

    [CreateAssetMenu(fileName = "DataBase_Affix", menuName = "Scriptable Object/DataBase/Affix")]
    public class DataBaseAffix : ScriptableObject
    {
        [SerializeField] private List<AffixSO> affixes;
        
        public List<AffixSO> AllAffixes => affixes;

        public Dictionary<EAffix, AffixSO> Affixes => GetAffixes();
        
        private Dictionary<EAffix, AffixSO> GetAffixes()
        {
            Dictionary<EAffix, AffixSO> ret = new Dictionary<EAffix, AffixSO>();
            affixes.ForEach(_affix => ret.Add(_affix.Type, _affix));
            return ret;
        }

        public AffixSO GetRandomBut(IEnumerable<AffixSO> _nonAffixes)
        {
            List<AffixSO> aff = new List<AffixSO>();
            aff.AddRange(affixes.Except(_nonAffixes));
            return aff[Random.Range(0, aff.Count)];
        }
    }
}
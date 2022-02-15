using System.Collections.Generic;
using System.Linq;
using _Extension;
using _Instances;
using Stats;
using UnityEngine;

namespace DataBases
{
    public enum EAffix
    {
        HP,
        AP,
        MP,
        Speed,
        Shield,

        Fire,
        Water,
        Nature,
        
        Power,
        Focus,
        
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
            List<AffixSO> aff = new List<AffixSO>(affixes.Except(_nonAffixes));
            List<AffixSO> weigthedAffixes = new List<AffixSO>();
            foreach (AffixSO _affix in aff)
            {
                for (int i = 0; i < _affix.Rarity + BattleStage.Stage; i++)
                {
                    weigthedAffixes.Add(_affix);
                }
            }

            return weigthedAffixes.GetRandom();
        }
    }
}
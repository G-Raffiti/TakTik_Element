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
        Hp,
        AP,
        Mp,
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
        [SerializeField] private List<AffixSo> affixes;
        
        public List<AffixSo> AllAffixes => affixes;

        public Dictionary<EAffix, AffixSo> Affixes => GetAffixes();
        
        private Dictionary<EAffix, AffixSo> GetAffixes()
        {
            Dictionary<EAffix, AffixSo> _ret = new Dictionary<EAffix, AffixSo>();
            affixes.ForEach(_affix => _ret.Add(_affix.Type, _affix));
            return _ret;
        }

        public AffixSo GetRandomBut(IEnumerable<AffixSo> _nonAffixes)
        {
            List<AffixSo> _aff = new List<AffixSo>(affixes.Except(_nonAffixes));
            List<AffixSo> _weigthedAffixes = new List<AffixSo>();
            foreach (AffixSo _affix in _aff)
            {
                for (int _i = 0; _i < _affix.Rarity + BattleStage.Stage; _i++)
                {
                    _weigthedAffixes.Add(_affix);
                }
            }

            return _weigthedAffixes.GetRandom();
        }
    }
}
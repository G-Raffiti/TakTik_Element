using System.Collections.Generic;
using _Extension;
using Relics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DataBases
{
    [CreateAssetMenu(fileName = "DataBase_Relic", menuName = "Scriptable Object/DataBase/Relic")]
    public class DataBaseRelic : ScriptableObject
    {
        [SerializeField] private List<RelicSo> allRelics;
        public List<RelicSo> AllRelics => allRelics;

        public RelicSo GetRandom() => AllRelics.GetRandom();
        
    }
}
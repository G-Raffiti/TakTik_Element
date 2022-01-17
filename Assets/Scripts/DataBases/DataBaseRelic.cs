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
        [SerializeField] private List<RelicSO> allRelics;
        public List<RelicSO> AllRelics => allRelics;

        public RelicSO GetRandom() => AllRelics.GetRandom();
        
    }
}
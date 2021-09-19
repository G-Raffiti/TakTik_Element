﻿using System.Collections.Generic;
using Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Skills
{
    [CreateAssetMenu(fileName = "DataBase_Relic", menuName = "Scriptable Object/DataBase/Relic")]
    public class DataBaseRelic : ScriptableObject
    {
        [SerializeField] private List<RelicSO> allRelics;
        public List<RelicSO> AllRelics => allRelics;

        public RelicSO GetRandom()
        {
            return AllRelics[Random.Range(0, AllRelics.Count)];
        }
    }
}
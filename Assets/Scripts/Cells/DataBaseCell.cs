using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StatusEffect;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cells
{
    [CreateAssetMenu(fileName = "DataBase_Cell", menuName = "Scriptable Object/DataBase/Cell")]
    public class DataBaseCell : ScriptableObject
    {
        [SerializeField] private List<CellSO> AllCells;

        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private GameObject gridObjectPrefab;
        [SerializeField] private StatusSO corruptionSO;

        public StatusSO CorruptionSO => corruptionSO;

        public GameObject GridObjectPrefab => gridObjectPrefab;
        public GameObject TilePrefab => tilePrefab;
    }
}
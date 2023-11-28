using System.Collections.Generic;
using Buffs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cells
{
    [CreateAssetMenu(fileName = "DataBase_Cell", menuName = "Scriptable Object/DataBase/Cell")]
    public class DataBaseCell : ScriptableObject
    {
        [FormerlySerializedAs("AllCells")]
        [SerializeField] private List<CellSo> allCells;

        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private GameObject gridObjectPrefab;
        [FormerlySerializedAs("corruptionSO")]
        [SerializeField] private StatusSo corruptionSo;

        public StatusSo CorruptionSo => corruptionSo;

        public GameObject GridObjectPrefab => gridObjectPrefab;
        public GameObject TilePrefab => tilePrefab;
    }
}
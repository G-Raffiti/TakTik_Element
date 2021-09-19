using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cells
{
    [CreateAssetMenu(fileName = "DataBase_Cell", menuName = "Scriptable Object/DataBase/Cell")]
    public class DataBaseCell : ScriptableObject
    {
        [SerializeField] private List<GameObject> AllCells;
        public Dictionary<ETile, GameObject> Cells => SetDico();
        
        
        [SerializeField] private GameObject gridObjectPrefab;
        public GameObject GridObjectPrefab => gridObjectPrefab;

        private Dictionary<ETile, GameObject> SetDico()
        {
            Dictionary<ETile, GameObject> ret = new Dictionary<ETile, GameObject>();
            foreach (GameObject _cell in AllCells)
            {
                if (_cell.GetComponent<Cell>() == null) continue;
                Cell _Cell = _cell.GetComponent<Cell>();
                ret.Add(_Cell.CellType, _cell);
            }

            return ret;
        }
    }
}
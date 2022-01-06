using System.Collections.Generic;
using Cells;
using UnityEngine;

namespace GridObjects
{
    [CreateAssetMenu(fileName = "GridObject_Tree", menuName = "Scriptable Object/Grid Objects/Tree")]
    public class Tree : GridObjectSO
    {
        [SerializeField] private List<Sprite> trees;
        public override Sprite Image => trees[Random.Range(0, trees.Count)];

        public override List<Cell> GetZoneOfInteraction(Cell location)
        {
            return new List<Cell>();
        }
    }
}
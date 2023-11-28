using System.Collections.Generic;
using _Extension;
using Cells;
using UnityEngine;

namespace GridObjects
{
    [CreateAssetMenu(fileName = "GridObject_Tree", menuName = "Scriptable Object/Grid Objects/Tree")]
    public class Tree : GridObjectSo
    {
        [SerializeField] private List<Sprite> trees;
        public override Sprite Image => trees.GetRandom();

        public override List<Cell> GetZoneOfInteraction(Cell _location)
        {
            return new List<Cell>();
        }
    }
}
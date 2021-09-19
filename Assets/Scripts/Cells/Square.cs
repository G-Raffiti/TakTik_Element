using System.Collections.Generic;
using StatusEffect;
using UnityEngine;

namespace Cells
{
    /// <summary>
    /// Implementation of square cell.
    /// </summary>
    public abstract class Square : Cell
    {
        protected List<Cell> neighbours = null;
        protected static readonly Vector2[] Directions =
        {
        new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1)
        };

        public override int GetDistance(Cell other)
        {
            return (int)(Mathf.Abs(OffsetCoord.x - other.OffsetCoord.x) + Mathf.Abs(OffsetCoord.y - other.OffsetCoord.y));
        }//Distance is given using Manhattan Norm.
        
        public override List<Cell> GetNeighbours(List<Cell> cells)
        {
            if (neighbours == null)
            {
                neighbours = new List<Cell>(4);
                foreach (Vector2 _direction in Directions)
                {
                    Cell _neighbour = cells.Find(c => c.OffsetCoord == OffsetCoord + _direction);
                    if (_neighbour == null) continue;

                    neighbours.Add(_neighbour);
                }
            }

            return neighbours;
        }
        
        //Each square cell has four neighbors, which positions on grid relative to the cell are stored in _directions constant.
        //It is totally possible to implement squares that have eight neighbours, it would require modification of GetDistance function though.
        public override void CopyFields(Cell newCell)
        {
            newCell.OffsetCoord = OffsetCoord;
        }
    }
}
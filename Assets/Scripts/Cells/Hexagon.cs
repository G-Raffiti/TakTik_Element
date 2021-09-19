using System.Collections.Generic;
using UnityEngine;

namespace Cells
{
    /// <summary>
    /// Implementation of hexagonal cell.
    /// </summary>
    public abstract class Hexagon : Cell
    {
        List<Cell> neighbours = null;
        /// <summary>
        /// HexGrids comes in four types regarding the layout. 
        /// This distinction is necessary to convert cube coordinates to offset and vice versa.
        /// </summary>
        [HideInInspector]
        public HexGridType hexGridType;

        /// <summary>
        /// Converts offset coordinates into cube coordinates.
        /// Cube coordinates is another system of coordinates that makes calculation on hex grids easier.
        /// </summary>
        protected Vector3 CubeCoord
        {
            get
            {
                Vector3 _ret = new Vector3();
                switch (hexGridType)
                {
                    case HexGridType.OddQ:
                        {
                            _ret.x = OffsetCoord.x;
                            _ret.z = OffsetCoord.y - (OffsetCoord.x + (Mathf.Abs(OffsetCoord.x) % 2)) / 2;
                            _ret.y = -_ret.x - _ret.z;
                            break;
                        }
                    case HexGridType.EvenQ:
                        {
                            _ret.x = OffsetCoord.x;
                            _ret.z = OffsetCoord.y - (OffsetCoord.x - (Mathf.Abs(OffsetCoord.x) % 2)) / 2;
                            _ret.y = -_ret.x - _ret.z;
                            break;
                        }
                }
                return _ret;
            }
        }

        /// <summary>
        /// Converts cube coordinates back to offset coordinates.
        /// </summary>
        /// <param name="cubeCoords">Cube coordinates to convert.</param>
        /// <returns>Offset coordinates corresponding to given cube coordinates.</returns>
        protected Vector2 CubeToOffsetCoords(Vector3 cubeCoords)
        {
            Vector2 _ret = new Vector2();

            switch (hexGridType)
            {
                case HexGridType.OddQ:
                    {
                        _ret.x = cubeCoords.x;
                        _ret.y = cubeCoords.z + (cubeCoords.x + (Mathf.Abs(cubeCoords.x) % 2)) / 2;
                        break;
                    }
                case HexGridType.EvenQ:
                    {
                        _ret.x = cubeCoords.x;
                        _ret.y = cubeCoords.z + (cubeCoords.x - (Mathf.Abs(cubeCoords.x) % 2)) / 2;
                        break;
                    }
            }
            return _ret;
        }

        protected static readonly Vector3[] Directions =  {
        new Vector3(+1, -1, 0), new Vector3(+1, 0, -1), new Vector3(0, +1, -1),
        new Vector3(-1, +1, 0), new Vector3(-1, 0, +1), new Vector3(0, -1, +1)};

        public override int GetDistance(Cell other)
        {
            Hexagon _other = other as Hexagon;
            int _distance = (int)(Mathf.Abs(CubeCoord.x - _other.CubeCoord.x) + Mathf.Abs(CubeCoord.y - _other.CubeCoord.y) + Mathf.Abs(CubeCoord.z - _other.CubeCoord.z)) / 2;
            return _distance;
        }//Distance is given using Manhattan Norm.

        public override List<Cell> GetNeighbours(List<Cell> cells)
        {
            if (neighbours == null)
            {
                neighbours = new List<Cell>(6);
                foreach (Vector3 _direction in Directions)
                {
                    Cell _neighbour = cells.Find(c => c.OffsetCoord == CubeToOffsetCoords(CubeCoord + _direction));
                    if (_neighbour == null) continue;
                    neighbours.Add(_neighbour);
                }
            }
            return neighbours;

        }//Each hex cell has six neighbors, which positions on grid relative to the cell are stored in _directions constant.

        public override void CopyFields(Cell newCell)
        {
            newCell.OffsetCoord = OffsetCoord;
            (newCell as Hexagon).hexGridType = hexGridType;
        }
    }

    public enum HexGridType
    {
        EvenQ,
        OddQ,
        EvenR,
        OddR
    };
}
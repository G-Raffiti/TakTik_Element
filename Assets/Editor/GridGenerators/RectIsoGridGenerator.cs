using System.Collections.Generic;
using Cells;
using UnityEditor;
using UnityEngine;

namespace Editor.GridGenerators
{
    /// <summary>
    /// Generates rectangular shaped grid of isos.
    /// </summary>
    [ExecuteInEditMode()]
    public class RectIsoGridGenerator : CellGridGenerator
    {
        public GameObject IsoPrefab;

        public int Width;
        public int Height;

        public override GridInfo GenerateGrid()
        {
            List<Cell> _ret = new List<Cell>();

            if (IsoPrefab.GetComponent<Cell>() == null)
            {
                Debug.LogError("Invalid iso cell prefab provided");
                return null;
            }

            Vector3 _isoSize = IsoPrefab.GetComponent<Cell>().GetCellDimensions();
            
            for (int _i = 0; _i < Width; _i++)
            {
                for (int _j = 0; _j < Height; _j++)
                {
                    GameObject _iso = PrefabUtility.InstantiatePrefab(IsoPrefab) as GameObject;

                    _iso.transform.position = new Vector3(_i * _isoSize.x, _j * _isoSize.y, 0);

                    int _x = _i+_j;
                    int _y = Width - 1 + _j - _i;
                    _iso.GetComponent<Cell>().OffsetCoord = new Vector2(_x, _y);
                    _iso.GetComponent<Cell>().movementCost = 1;
                    _ret.Add(_iso.GetComponent<Cell>());

                    _iso.transform.parent = CellsParent;
                }
            }
            
            for (int _i = 0; _i < Width-1; _i++)
            {
                for (int _j = 0; _j < Height-1; _j++)
                {
                    GameObject _iso = PrefabUtility.InstantiatePrefab(IsoPrefab) as GameObject;

                    int _x = _i + _j + 1;
                    int _y = Width - 1 + _j - _i;

                    _iso.transform.position = new Vector3(_isoSize.x/2 + _i * _isoSize.x, _isoSize.y/2 + _j * _isoSize.y, 0);
                    _iso.GetComponent<Cell>().OffsetCoord = new Vector2(_x, _y);
                    _iso.GetComponent<Cell>().movementCost = 1;
                    _ret.Add(_iso.GetComponent<Cell>());

                    _iso.transform.parent = CellsParent;
                }
            }
            
            
            Vector3 _cellDimensions = IsoPrefab.GetComponent<Cell>().GetCellDimensions();

            GridInfo _gridInfo = new GridInfo();
            _gridInfo.Cells = _ret;
            _gridInfo.Dimensions = new Vector3(_cellDimensions.x * (Width - 1), _cellDimensions.y * (Height - 1), _cellDimensions.z);
            _gridInfo.Center = _gridInfo.Dimensions / 2;

            return _gridInfo;
        }
    }
}
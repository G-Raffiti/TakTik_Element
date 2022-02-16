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
    public class RectIsoGridGenerator : ICellGridGenerator
    {
        public GameObject IsoPrefab;

        public int Width;
        public int Height;

        public override GridInfo GenerateGrid()
        {
            List<Cell> ret = new List<Cell>();

            if (IsoPrefab.GetComponent<Cell>() == null)
            {
                Debug.LogError("Invalid iso cell prefab provided");
                return null;
            }

            Vector3 isoSize = IsoPrefab.GetComponent<Cell>().GetCellDimensions();
            
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    GameObject iso = PrefabUtility.InstantiatePrefab(IsoPrefab) as GameObject;

                    iso.transform.position = new Vector3(i * isoSize.x, j * isoSize.y, 0);

                    int x = i+j;
                    int y = Width - 1 + j - i;
                    iso.GetComponent<Cell>().OffsetCoord = new Vector2(x, y);
                    iso.GetComponent<Cell>().MovementCost = 1;
                    ret.Add(iso.GetComponent<Cell>());

                    iso.transform.parent = CellsParent;
                }
            }
            
            for (int i = 0; i < Width-1; i++)
            {
                for (int j = 0; j < Height-1; j++)
                {
                    GameObject iso = PrefabUtility.InstantiatePrefab(IsoPrefab) as GameObject;

                    int x = i + j + 1;
                    int y = Width - 1 + j - i;

                    iso.transform.position = new Vector3(isoSize.x/2 + i * isoSize.x, isoSize.y/2 + j * isoSize.y, 0);
                    iso.GetComponent<Cell>().OffsetCoord = new Vector2(x, y);
                    iso.GetComponent<Cell>().MovementCost = 1;
                    ret.Add(iso.GetComponent<Cell>());

                    iso.transform.parent = CellsParent;
                }
            }
            
            
            Vector3 cellDimensions = IsoPrefab.GetComponent<Cell>().GetCellDimensions();

            GridInfo gridInfo = new GridInfo();
            gridInfo.Cells = ret;
            gridInfo.Dimensions = new Vector3(cellDimensions.x * (Width - 1), cellDimensions.y * (Height - 1), cellDimensions.z);
            gridInfo.Center = gridInfo.Dimensions / 2;

            return gridInfo;
        }
    }
}
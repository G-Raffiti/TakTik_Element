using UnityEngine;
using System.Collections.Generic;
using Cells;
using UnityEditor;

namespace TbsFramework.EditorUtils.GridGenerators
{
    /// <summary>
    /// Generates rectangular shaped grid of squares.
    /// </summary>
    [ExecuteInEditMode()]
    public class RectangularSquareGridGenerator : ICellGridGenerator
    {
        public GameObject SquarePrefab;

        public int Width;
        public int Height;

        public override GridInfo GenerateGrid()
        {
            List<Cell> ret = new List<Cell>();

            if (SquarePrefab.GetComponent<Square>() == null)
            {
                Debug.LogError("Invalid square cell prefab provided");
                return null;
            }

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    GameObject square = PrefabUtility.InstantiatePrefab(SquarePrefab) as GameObject;
                    Vector3 squareSize = square.GetComponent<Cell>().GetCellDimensions();

                    square.transform.position = new Vector3(i * squareSize.x, j * squareSize.y, 0);
                    square.GetComponent<Cell>().OffsetCoord = new Vector2(i, j);
                    square.GetComponent<Cell>().movementCost = 1;
                    ret.Add(square.GetComponent<Cell>());

                    square.transform.parent = CellsParent;
                }
            }
            Vector3 cellDimensions = SquarePrefab.GetComponent<Cell>().GetCellDimensions();

            GridInfo gridInfo = new GridInfo();
            gridInfo.Cells = ret;
            gridInfo.Dimensions = new Vector3(cellDimensions.x * (Width - 1), cellDimensions.y * (Height - 1), cellDimensions.z);
            gridInfo.Center = gridInfo.Dimensions / 2;

            return gridInfo;
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using Cells;
using UnityEditor;

namespace TbsFramework.EditorUtils.GridGenerators
{
    /// <summary>
    /// Generates equilateral shaped grid of hexagons.
    /// </summary>
    [ExecuteInEditMode()]
    public class EquilateralHexGridGenerator : ICellGridGenerator
    {
        public GameObject HexagonPrefab;
        public int SideA;
        public int SideB;

        public override GridInfo GenerateGrid()
        {
            HexGridType hexGridType = SideA % 2 == 0 ? HexGridType.EvenQ : HexGridType.OddQ; ;
            List<Cell> hexagons = new List<Cell>();

            if (HexagonPrefab.GetComponent<Hexagon>() == null)
            {
                Debug.LogError("Invalid hexagon prefab provided");
                return null;
            }

            for (int i = 0; i < SideA; i++)
            {
                for (int j = 0; j < SideB; j++)
                {
                    GameObject hexagon = PrefabUtility.InstantiatePrefab(HexagonPrefab) as GameObject;

                    Vector3 hexSize = hexagon.GetComponent<Cell>().GetCellDimensions();

                    hexagon.transform.position = new Vector3((i * hexSize.x * 0.75f), (i * hexSize.y * 0.5f) + (j * hexSize.y), 0);
                    hexagon.GetComponent<Hexagon>().OffsetCoord = new Vector2(SideA - i - 1, SideB - j - 1 - (i / 2));
                    hexagon.GetComponent<Hexagon>().hexGridType = hexGridType;
                    hexagon.GetComponent<Hexagon>().movementCost = 1;
                    hexagons.Add(hexagon.GetComponent<Cell>());

                    hexagon.transform.parent = CellsParent;
                }
            }
            Vector3 hexDimensions = HexagonPrefab.GetComponent<Cell>().GetCellDimensions();
            float hexSide = hexDimensions.x / 2;

            GridInfo gridInfo = new GridInfo();
            gridInfo.Cells = hexagons;
            float dimX = hexSide * Mathf.Sqrt(3) * (SideA - 1) * Mathf.Sqrt(3) / 2;
            gridInfo.Dimensions = new Vector3(dimX, hexDimensions.y * (SideB - 1) + (dimX) * Mathf.Sqrt(3) / 4, hexDimensions.z);
            gridInfo.Center = gridInfo.Dimensions / 2;

            return gridInfo;
        }
    }
}
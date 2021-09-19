using System.Collections.Generic;
using Cells;
using UnityEditor;
using UnityEngine;

namespace TbsFramework.EditorUtils.GridGenerators
{
    /// <summary>
    /// Generates hexagonal shaped grid of hexagons.
    /// </summary>
    [ExecuteInEditMode()]
    class HexagonalHexGridGenerator : ICellGridGenerator
    {
        #pragma warning disable 0649
        public GameObject HexagonPrefab;
        public int Radius;
        #pragma warning restore 0649

        public override GridInfo GenerateGrid()
        {
            List<Cell> hexagons = new List<Cell>();

            if (HexagonPrefab.GetComponent<Hexagon>() == null)
            {
                Debug.LogError("Invalid hexagon prefab provided");
                return null;
            }

            for (int i = 0; i < Radius; i++)
            {
                for (int j = 0; j < (Radius * 2) - i - 1; j++)
                {
                    GameObject hexagon = PrefabUtility.InstantiatePrefab(HexagonPrefab) as GameObject;
                    Vector3 hexSize = hexagon.GetComponent<Cell>().GetCellDimensions();

                    hexagon.transform.position = new Vector3((i * hexSize.x * 0.75f), (i * hexSize.y * 0.5f) + (j * hexSize.y), 0);
                    hexagon.GetComponent<Hexagon>().OffsetCoord = new Vector2(i, Radius - j - 1 - (i / 2));
                    hexagon.GetComponent<Hexagon>().hexGridType = HexGridType.OddQ;
                    hexagon.GetComponent<Hexagon>().movementCost = 1;
                    hexagons.Add(hexagon.GetComponent<Cell>());

                    hexagon.transform.parent = CellsParent;

                    if (i == 0) continue;

                    GameObject hexagon2 = PrefabUtility.InstantiatePrefab(HexagonPrefab) as GameObject;
                    hexagon2.transform.position = new Vector3((-i * hexSize.x * 0.75f), (i * hexSize.y * 0.5f) + (j * hexSize.y), 0);
                    hexagon2.GetComponent<Hexagon>().OffsetCoord = new Vector2(-i, Radius - j - 1 - (i / 2));
                    hexagon2.GetComponent<Hexagon>().hexGridType = HexGridType.OddQ;
                    hexagon2.GetComponent<Hexagon>().movementCost = 1;
                    hexagons.Add(hexagon2.GetComponent<Cell>());

                    hexagon2.transform.parent = CellsParent;
                }
            }
            Vector3 hexDimensions = HexagonPrefab.GetComponent<Cell>().GetCellDimensions();

            GridInfo gridInfo = new GridInfo();
            gridInfo.Cells = hexagons;
            gridInfo.Dimensions = new Vector3(hexDimensions.x * (Radius * 2) - 2, hexDimensions.y * ((Radius * 2) - 2), hexDimensions.z);
            gridInfo.Center = new Vector3(0, gridInfo.Dimensions.y / 2, 0);

            return gridInfo;
        }

    }
}
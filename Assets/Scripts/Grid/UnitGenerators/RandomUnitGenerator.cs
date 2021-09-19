using System.Collections.Generic;
using System.Linq;
using Cells;
using Units;
using UnityEngine;

namespace Grid.UnitGenerators
{
    class RandomUnitGenerator : MonoBehaviour, IUnitGenerator
    {
        private System.Random rnd = new System.Random();

        #pragma warning disable 0649
        public Transform unitsParent;

        public GameObject unitPrefab;
        public int numberOfPlayers;
        public int unitsPerPlayer;
        #pragma warning restore 0649

        /// <summary>
        /// Method spawns UnitPerPlayer number of UnitPrefabs in random positions.
        /// Each player gets equal number of units.
        /// </summary>
        public List<Unit> SpawnUnits(List<Cell> cells)
        {
            unitsParent = GameObject.Find("Units").transform;
            List<Unit> _ret = new List<Unit>();

            List<Cell> _freeCells = cells.FindAll(h => h.GetComponent<Cell>().isTaken == false);
            _freeCells = _freeCells.OrderBy(h => rnd.Next()).ToList();

            for (int _i = 0; _i < numberOfPlayers; _i++)
            {
                for (int _j = 0; _j < unitsPerPlayer; _j++)
                {
                    Cell _cell = _freeCells.ElementAt(0);
                    _freeCells.RemoveAt(0);
                    _cell.GetComponent<Cell>().isTaken = true;

                    GameObject _unit = Instantiate(unitPrefab);
                    _unit.transform.position = _cell.transform.position + new Vector3(0, 0, 0);
                    _unit.GetComponent<Unit>().playerNumber = _i;
                    _unit.GetComponent<Unit>().Cell = _cell.GetComponent<Cell>();
                    _unit.GetComponent<Unit>().Initialize();
                    _unit.transform.parent = unitsParent;


                    _ret.Add(_unit.GetComponent<Unit>());
                }
            }
            return _ret;
        }
    }
}

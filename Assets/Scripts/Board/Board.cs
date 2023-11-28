using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _Instances;
using EndConditions;
using GridObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Cells
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private VoidEvent onBoardLoaded;
        [SerializeField] private DataBase dataBase;
        /// <summary>
        /// BoardSO to Save or to test
        /// </summary>
        [SerializeField] private BoardSo boardTest;
        private List<SavedCell> savedCells = new List<SavedCell>();
        [SerializeField] private Image background;
        [SerializeField] private Camera mainCamera;
        
        /// <summary>
        /// return the Conditions that will ends the Battle and a Bool "isWin ?"
        /// </summary>
        public EndConditionSo EndCondition { get; private set; }

        
        /// <summary>
        /// return a Board Stage 1 = 1Loot, 3Fight, 1Boss, Stage 2 = 3Fight, 1Boss, Stage 3 = 1Loot, 2Fight, 1Boss.
        /// </summary>
        /// <returns></returns>
        private static BoardSo Randomize(EConditionType _state)
        {
            switch (_state)
            {
                case EConditionType.LootBox:
                    return DataBase.Board.LootBoxBoards[Random.Range(0, DataBase.Board.LootBoxBoards.Count)];
                case EConditionType.Boss:
                    return DataBase.Board.BossBattleBoards[Random.Range(0, DataBase.Board.BossBattleBoards.Count)];
                case EConditionType.Last:
                    return DataBase.Board.LastBattleBoards[Random.Range(0, DataBase.Board.LastBattleBoards.Count)];
                default:
                    return DataBase.Board.DeathBattleBoards[Random.Range(0, DataBase.Board.DeathBattleBoards.Count)];
            }
        }

        /// <summary>
        /// Save The SceneBoard in a BoardSO
        /// </summary>
        [ContextMenu("Save Board")]
        public void SaveBoard()
        {
            dataBase.InstantiateDataBases();
            savedCells = new List<SavedCell>();

            List<GridObject> _gridObjects = new List<GridObject>();
            foreach (Transform _child in GameObject.Find("Objects").transform)
            {
                if(_child.gameObject.GetComponent<GridObject>() != null)
                    _gridObjects.Add(_child.gameObject.GetComponent<GridObject>());
            }

            foreach (GridObject _gridObject in _gridObjects)
            {
                if (_gridObject.Cell == null)
                {
                    Debug.LogError("no Cell");
                }
                else _gridObject.Cell.Take(_gridObject);
            }
            
            foreach (Transform _child in transform)
            {
                Cell _cell = _child.gameObject.GetComponent<Cell>();
                if (_cell.CellSo == null)
                {
                    Debug.LogWarning($"cell ({_cell.OffsetCoord.x}/{_cell.OffsetCoord.y}) missing CellSO");
                }
                if (_cell != null)
                {
                    savedCells.Add(new SavedCell(_child.gameObject.GetComponent<TileIsometric>()));
                }
            }
            
            boardTest.SaveBoard(savedCells, background.sprite, mainCamera);
            Debug.Log($"Board SO saved in {boardTest.name}");
        }
        
        /// <summary>
        /// method Called at the beginning of each Battle to choose and create the Board
        /// </summary>
        public void LoadBoard(EConditionType _state)
        {
            LoadBoard(Randomize(_state));
        }
        
        /// <summary>
        /// Instantiate all the Cells and GridObjects from a Saved BoardSO
        /// </summary>
        private void LoadBoard(BoardSo _data)
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            while (GameObject.Find("Objects").transform.childCount > 0)
            {
                DestroyImmediate(GameObject.Find("Objects").transform.GetChild(0).gameObject);
            }

            //dataBase.InstantiateDataBases();
            background.sprite = null;
            
            foreach (SavedCell _savedCell in _data.Cells)
            {

                GameObject _instance = GameObject.Instantiate(DataBase.Cell.TilePrefab);
                _instance.transform.SetParent(transform);
                _instance.transform.position = new Vector3(_savedCell.position[0],_savedCell.position[1],_savedCell.position[2]);
                TileIsometric _cell = _instance.GetComponent<TileIsometric>();
                if (_cell != null)
                {
                    _cell.SetCellSo(_savedCell.type);
                    _cell.OffsetCoord = new Vector2(_savedCell.offsetCoord[0], _savedCell.offsetCoord[1]);
                    _cell.isSpawnPlace = _savedCell.isSpawn;
                    _cell.Initialize();
                }

                if (_savedCell.gridObject == null) continue;
                GameObject _gridObject = Instantiate(DataBase.Cell.GridObjectPrefab) as GameObject;
                _gridObject.transform.SetParent(GameObject.Find("Objects").transform);
                _gridObject.transform.position = new Vector3(_savedCell.position[0],_savedCell.position[1],_savedCell.position[2]);
                _gridObject.GetComponent<GridObject>().GridObjectSo = _savedCell.gridObject;
                
                _cell.Take(_gridObject.GetComponent<GridObject>());
            }

            background.sprite = _data.Background;

            mainCamera.gameObject.transform.position = new Vector3(_data.Camera.x, _data.Camera.y, -15f);
            mainCamera.orthographicSize = _data.Camera.size;

            EndCondition = _data.EndCondition;

            onBoardLoaded.Raise();
        }
        
        /// <summary>
        /// Only Used in Editor
        /// </summary>
        [ContextMenu("Load Test Board")]
        public void LoadTest()
        {
            LoadBoard(boardTest);
        }
    }
}
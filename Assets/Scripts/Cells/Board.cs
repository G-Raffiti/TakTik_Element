using System.Collections.Generic;
using _Instances;
using EndConditions;
using GridObjects;
using UnityEditor;
using UnityEngine;

namespace Cells
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private DataBase dataBase;
        /// <summary>
        /// BoardSO to Save or to test
        /// </summary>
        [SerializeField] private BoardSO boardTest;
        private List<SavedCell> SavedCells = new List<SavedCell>();
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private Camera mainCamera;
        
        /// <summary>
        /// return the Conditions that will ends the Battle and a Bool "isWin ?"
        /// </summary>
        public EndConditionSO EndCondition { get; private set; }

        
        /// <summary>
        /// return a Board Stage 1 = 1Loot, 3Fight, 1Boss, Stage 2 = 3Fight, 1Boss, Stage 3 = 1Loot, 2Fight, 1Boss.
        /// </summary>
        /// <returns></returns>
        private static BoardSO randomize()
        {
            if (KeepBetweenScene.Stage == 0 || KeepBetweenScene.Stage == 3 && KeepBetweenScene.BattleBeforeBoss == 0)
                return DataBase.Board.LootBoxBoards[Random.Range(0, DataBase.Board.LootBoxBoards.Count)];
            
            if (KeepBetweenScene.BattleBeforeBoss <= 0)
                return DataBase.Board.BossBattleBoards[Random.Range(0, DataBase.Board.BossBattleBoards.Count)];
            
            return DataBase.Board.DeathBattleBoards[Random.Range(0, DataBase.Board.DeathBattleBoards.Count)];
        }

        /// <summary>
        /// Save The SceneBoard in a BoardSO
        /// </summary>
        [ContextMenu("Save Board")]
        public void SaveBoard()
        {
            dataBase.InstantiateDataBases();
            SavedCells = new List<SavedCell>();

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
                Cell _Cell = _child.gameObject.GetComponent<Cell>();
                if (_Cell != null)
                {
                    SavedCells.Add(new SavedCell(_child.gameObject.GetComponent<TileIsometric>()));
                }
            }
            
            boardTest.SaveBoard(SavedCells, background.sprite, mainCamera);
            Debug.Log($"Board SO saved in {boardTest.name}");
        }
        
        /// <summary>
        /// method Called at the beginning of each Battle to choose and create the Board
        /// </summary>
        public void LoadBoard()
        {
            LoadBoard(randomize());
        }
        
        /// <summary>
        /// Instantiate all the Cells and GridObjects from a Saved BoardSO
        /// </summary>
        private void LoadBoard(BoardSO _data)
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            while (GameObject.Find("Objects").transform.childCount > 0)
            {
                DestroyImmediate(GameObject.Find("Objects").transform.GetChild(0).gameObject);
            }

            dataBase.InstantiateDataBases();
            background.sprite = null;
            
            foreach (SavedCell _SavedCell in _data.Cells)
            {
                
                GameObject instance = PrefabUtility.InstantiatePrefab(DataBase.Cell.TilePrefab) as GameObject;
                instance.transform.SetParent(transform);
                instance.transform.position = new Vector3(_SavedCell.position[0],_SavedCell.position[1],_SavedCell.position[2]);
                TileIsometric _cell = instance.GetComponent<TileIsometric>();
                if (_cell != null)
                {
                    _cell.CellSO = _SavedCell.type;
                    _cell.OffsetCoord = new Vector2(_SavedCell.offsetCoord[0], _SavedCell.offsetCoord[1]);
                    _cell.IsSpawnPlace = _SavedCell.isSpawn;
                }

                if (_SavedCell.gridObject == null) continue;
                GameObject gridObject = Instantiate(DataBase.Cell.GridObjectPrefab) as GameObject;
                gridObject.transform.SetParent(GameObject.Find("Objects").transform);
                gridObject.transform.position = new Vector3(_SavedCell.position[0],_SavedCell.position[1],_SavedCell.position[2]);
                gridObject.GetComponent<GridObject>().GridObjectSO = _SavedCell.gridObject;
                
                _cell.Take(gridObject.GetComponent<GridObject>());
            }

            background.sprite = _data.Background;

            mainCamera.gameObject.transform.position = new Vector3(_data.Camera.x, _data.Camera.y, -15f);
            mainCamera.orthographicSize = _data.Camera.size;
            background.transform.position = new Vector3(_data.Camera.x, _data.Camera.y, 0);
            

            EndCondition = _data.EndCondition;
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
using System.Collections.Generic;
using _Instances;
using BattleOver;
using GridObjects;
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
        private List<SavedCell> Cells = new List<SavedCell>();
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
            dataBase.Start();
            Cells = new List<SavedCell>();

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
                    Cells.Add(new SavedCell(_child.gameObject.GetComponent<Cell>()));
                }
            }
            
            boardTest.SaveBoard(Cells, background.sprite, mainCamera);
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

            background.sprite = null;
            
            foreach (SavedCell _Cell in _data.Cells)
            {
                GameObject instance = Instantiate(DataBase.Cell.Cells[_Cell.type]) as GameObject;
                instance.transform.SetParent(transform);
                instance.transform.position = new Vector3(_Cell.position[0],_Cell.position[1],_Cell.position[2]);
                Cell _t = instance.GetComponent<Cell>();
                if (_t != null)
                {
                    _t.OffsetCoord = new Vector2(_Cell.offsetCoord[0], _Cell.offsetCoord[1]);
                    _t.isSpawnPlace = _Cell.isSpawn;
                }

                if (_Cell.gridObject == null) continue;
                GameObject gridObject = Instantiate(DataBase.Cell.GridObjectPrefab) as GameObject;
                gridObject.transform.SetParent(GameObject.Find("Objects").transform);
                gridObject.transform.position = new Vector3(_Cell.position[0],_Cell.position[1],_Cell.position[2]);
                gridObject.GetComponent<GridObject>().GridObjectSO = _Cell.gridObject;
                
                _t.Take(gridObject.GetComponent<GridObject>());
            }

            background.sprite = _data.Background;

            mainCamera.gameObject.transform.position = new Vector3(_data.Camera.x, _data.Camera.y, -15f);
            mainCamera.orthographicSize = _data.Camera.x + 1;
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
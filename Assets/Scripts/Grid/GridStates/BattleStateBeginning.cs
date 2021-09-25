using System.Collections.Generic;
using System.Linq;
using Cells;
using Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Grid.GridStates
{
    public class BattleStateBeginning : BattleState
    {
        private List<Cell> setupCells;
        private BattleStateManager stateManager;
        private List<Hero> heroes;
        private List<MonsterSO> monsters;
        private List<GameObject> prefabHeroes;
        private int index = 0;
        
        public BattleStateBeginning(BattleStateManager _stateManager, List<MonsterSO> _monsters) : base(_stateManager)
        {
            stateManager = _stateManager;
            heroes = new List<Hero>();
            setupCells = new List<Cell>();
            prefabHeroes = new List<GameObject>();
            monsters = _monsters;
        }

        public override void OnStateEnter()
        {
            stateManager.Cells.ForEach(c => c.UnMark());
            
            List<Cell> _freeCells = stateManager.Cells.FindAll(c => c.isWalkable && c.Buffs.Count == 0);
            
            List<Cell> _enemiesCells = new List<Cell>();
            while (_enemiesCells.Count < monsters.Count)
            {
                int _cellIndex = Random.Range(0, _freeCells.Count);
                    _enemiesCells.Add(_freeCells[_cellIndex]);
                    _freeCells.Remove(_freeCells[_cellIndex]);
            }

            for (int i = 0; i < monsters.Count; i++)
            {
                monsters[i].Spawn(_enemiesCells[i]);
            }
            
            foreach (Hero _hero in GameObject.Find("Player").GetComponentsInChildren<Hero>())
            {
                if (_hero.isDead) continue;
                _hero.isPlaced = false;
                heroes.Add(_hero);
            }

            List<Cell> _spawnCells = new List<Cell>();
            foreach (Cell _cell in StateManager.Cells)
            {
                if (_cell.isSpawnPlace)
                {
                    _spawnCells.Add(_cell);
                }
            }

            if (_spawnCells.Count == 0)
            {
                while (setupCells.Count < 10)
                {
                    int _cellIndex = Random.Range(0, _freeCells.Count);
                    setupCells.Add(_freeCells[_cellIndex]);
                }
            }
            else setupCells = _spawnCells;

            foreach (Cell _setupCell in setupCells)
            {
                _setupCell.MarkAsReachable();
            }

            for (int i = 0; i < heroes.Count; i++)
            {
                GameObject _pref = Object.Instantiate(heroes[i].Prefab, GameObject.Find("Units").transform);
                heroes[i].Spawn(_pref);
                _pref.transform.position = new Vector3(-1, i);
                _pref.GetComponent<Unit>().InitializeSprite();
                prefabHeroes.Add(_pref);
            }
            
            prefabHeroes[index].GetComponent<Unit>().MarkAsSelected();
        }
        

        public override void OnStateExit()
        {
            for (int i = 0; i < heroes.Count; i++)
            {
                if (!heroes[i].isPlaced)
                {
                    Object.DestroyImmediate(prefabHeroes[i]);
                }
            }
            foreach (Cell _cell in stateManager.Cells)
            {
                _cell.UnMark();
            }
        }

        private void ChangeIndex()
        {
            prefabHeroes[index].GetComponent<Unit>().UnMark();
            index += 1;
            if (index >= prefabHeroes.Count)
                index = 0;
            prefabHeroes[index].GetComponent<Unit>().MarkAsSelected();
        }
        
        public override void OnCellClicked(Cell _cell)
        {
            if (setupCells.Contains(_cell) &! _cell.isTaken)
            {
                prefabHeroes[index].transform.position = _cell.transform.position;

                if (prefabHeroes[index].GetComponent<Unit>().Cell != null)
                {
                    prefabHeroes[index].GetComponent<Unit>().Cell.FreeTheCell();
                }
                else
                {
                    heroes[index].isPlaced = true;
                }

                _cell.Take(prefabHeroes[index].GetComponent<Unit>());
                prefabHeroes[index].GetComponent<Unit>().AutoSortOrder();
            }

            else
            {
                ChangeIndex();
            }
        }

        public override void OnCellSelected(Cell _selectedCell)
        {
            if (setupCells.Contains(_selectedCell))
            {
                _selectedCell.MarkAsPath();
            }
        }

        public override void OnCellDeselected(Cell _unselectedCell)
        {
            if (setupCells.Contains(_unselectedCell))
            {
                _unselectedCell.MarkAsReachable();
            }
        }
    }
}
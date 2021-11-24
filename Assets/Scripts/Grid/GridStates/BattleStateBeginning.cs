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
        private Dictionary<BattleHero, GameObject> prefabHeroes;
        private GameObject sprite;

        private BattleHero actualHero { get; set; }
        
        public BattleStateBeginning(BattleStateManager _stateManager, List<MonsterSO> _monsters) : base(_stateManager)
        {
            stateManager = _stateManager;
            heroes = new List<Hero>();
            setupCells = new List<Cell>();
            prefabHeroes = new Dictionary<BattleHero, GameObject>();
            monsters = _monsters;
        }

        public override void OnStateEnter()
        {
            stateManager.Cells.ForEach(c => c.UnMark());
            
            List<Cell> _freeCells = stateManager.Cells.FindAll(c => c.IsWalkable && c.Buffs.Count == 0);
            
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
                if (_cell.IsSpawnPlace)
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
                _pref.transform.position = new Vector3(-2, 3-i);
                _pref.GetComponent<Unit>().InitializeSprite();
                prefabHeroes.Add(_pref.GetComponent<BattleHero>(), _pref);
            }

            stateManager.OnHeroSelected.EventListeners += ChangeHero;
            sprite = GameObject.Find("Layer/Sprite");
        }
        

        public override void OnStateExit()
        {
            foreach (BattleHero _hero in prefabHeroes.Keys)
            {
                if(!_hero.Hero.isPlaced)
                    GameObject.DestroyImmediate(prefabHeroes[_hero]);
            }
            
            foreach (Cell _cell in stateManager.Cells)
            {
                _cell.UnMark();
            }

            stateManager.OnHeroSelected.EventListeners -= ChangeHero;
        }

        private void ChangeHero(Unit hero)
        {
            foreach (BattleHero _hero in prefabHeroes.Keys)
            {
                _hero.UnMark();
            }

            actualHero = (BattleHero) hero;
            actualHero.MarkAsSelected();
        }

        public override void OnCellClicked(Cell _cell)
        {
            if (!setupCells.Contains(_cell))
            {
                actualHero = null;
                return;
            }
            if (actualHero == null && !_cell.IsTaken) return;
            if (actualHero == null)
            {
                ChangeHero(_cell.CurrentUnit);
                return;
            }

            if (!_cell.IsTaken) // Place Hero on a Free Cell
            {
                actualHero.transform.position = _cell.transform.position;
            }

            else
            {
                if (actualHero.Cell != null) // Swap Cell
                {
                    Cell actualCell = actualHero.Cell;
                    
                    actualCell.FreeTheCell();
                    _cell.CurrentUnit.transform.position = actualCell.transform.position;
                    actualCell.Take(_cell.CurrentUnit);
                    
                    actualHero.transform.position = _cell.transform.position;
                }

                else // Swap Position
                {
                    _cell.FreeTheCell();
                    actualHero.transform.position = _cell.transform.position;
                }
            }
            
            _cell.Take(actualHero);
            actualHero.AutoSortOrder();

            foreach (Cell _setupCell in setupCells)
            {
                _setupCell.MarkAsReachable();
            }

            foreach (BattleHero _hero in prefabHeroes.Keys)
            {
                if (_hero != actualHero)
                {
                    if (_hero.Cell == _cell)
                        _hero.Cell = null;
                }
                if (_hero.Cell == null)
                {
                    int i = heroes.IndexOf(_hero.Hero);
                    prefabHeroes[_hero].transform.position = new Vector3(-2, 3 - i);
                }
            }
            
            actualHero.Hero.isPlaced = true;
            actualHero = null;
        }

        public override void OnCellSelected(Cell _selectedCell)
        {
            if (!setupCells.Contains(_selectedCell)) return;
            _selectedCell.MarkAsPath();

            if (actualHero == null) return;
            
            sprite.transform.position = _selectedCell.transform.position + new Vector3(0, 0.7f);
            sprite.GetComponent<SpriteRenderer>().sprite = actualHero.UnitSprite;
        }

        public override void OnCellDeselected(Cell _unselectedCell)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = null;
            
            if (setupCells.Contains(_unselectedCell))
                _unselectedCell.MarkAsReachable();
            
            if (actualHero != null && _unselectedCell == actualHero.Cell)
                _unselectedCell.MarkAsHighlighted();
        }
    }
}
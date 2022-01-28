using System.Collections.Generic;
using System.Linq;
using Cells;
using Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StateMachine.GridStates
{
    public class BattleStateBeginning : BattleState
    {
        private List<Cell> setupCells;
        private BattleStateManager stateManager;
        private List<Hero> heroes;
        private List<MonsterSO> monsters;
        private Dictionary<BattleHero, GameObject> battleHeroes;
        private GameObject sprite;
        private Vector3 outOfScreen = new Vector3(-100, -100, -100);

        private BattleHero actualHero { get; set; }
        
        public BattleStateBeginning(BattleStateManager _stateManager, List<MonsterSO> _monsters) : base(_stateManager)
        {
            State = EBattleState.Beginning;
            stateManager = _stateManager;
            heroes = new List<Hero>();
            setupCells = new List<Cell>();
            battleHeroes = new Dictionary<BattleHero, GameObject>();
            monsters = _monsters;
        }

        public override void OnStateEnter()
        {
            stateManager.Cells.ForEach(c => c.UnMark());
            
            List<Cell> _freeCells = stateManager.Cells.FindAll(c => c.IsWalkable && c.Buffs.Count == 0 && !c.IsSpawnPlace);
            
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
                    if (!setupCells.Contains(_freeCells[_cellIndex]))
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
                _pref.transform.position = setupCells[i].transform.position;
                _pref.GetComponent<Unit>().InitializeSprite();
                battleHeroes.Add(_pref.GetComponent<BattleHero>(), _pref);
                setupCells[i].Take(_pref.GetComponent<Unit>());
                heroes[i].isPlaced = true;
            }
            sprite = GameObject.Find("Layer/Sprite");
        }
        

        public override void OnStateExit()
        {
            foreach (BattleHero _hero in battleHeroes.Keys)
            {
                if(!_hero.Hero.isPlaced)
                    _hero.StartCoroutine(_hero.OnDestroyed());
            }
            
            foreach (Cell _cell in stateManager.Cells)
            {
                _cell.UnMark();
            }
        }

        private void ChangeHero(Unit hero)
        {
            actualHero = (BattleHero) hero;
        }

        private void PlaceHero(Cell _cell)
        {
            actualHero.transform.position = _cell.transform.position;
            _cell.Take(actualHero);
            actualHero.Hero.isPlaced = true;
            actualHero = null;
        }

        private void CheckCells()
        {
            // Check if 2 Heroes are on the same Cell
            List<IMovable> heroesOnSameCell = StateManager.GetCell
                .GroupBy(z => z.Value)
                .Where(z => z.Count() > 1)
                .SelectMany(z => z)
                .Select(z => z.Key)
                .ToList();
            if (heroesOnSameCell.Count > 1)
            {
                foreach (IMovable _movable in heroesOnSameCell)
                {
                    Debug.Log(((BattleHero) _movable).UnitName);
                }
                heroesOnSameCell[0].Cell.Take(heroesOnSameCell[0]);
                ChangeHero((BattleHero) heroesOnSameCell[1]);
                return;
            }

            foreach (BattleHero _hero in battleHeroes.Keys) 
            {
                if (_hero.Cell == null)
                {
                    ChangeHero(_hero);
                    return;
                }
            }
        }

        public override void OnCellClicked(Cell _cell)
        {
            // Check if something happens
            if (!setupCells.Contains(_cell))
            {
                if (actualHero.Cell != null)
                    actualHero = null;
                CheckCells();
                return;
            }
            if (actualHero == null && !_cell.IsTaken) 
            {
                CheckCells();
                return;
            }
            
            BattleHero nextHero = null;
            if (_cell.CurrentUnit != null)
            {
                nextHero = (BattleHero) _cell.CurrentUnit;
            }
            
            // Set the position of the ActualHero on the Cell
            if (actualHero != null) PlaceHero(_cell);

            if (nextHero != null)
            {
                ChangeHero(nextHero);
                return;
            }
            
            CheckCells();
        }

        public override void OnCellSelected(Cell _selectedCell)
        {
            if (actualHero == null) return;
            sprite.transform.position = _selectedCell.transform.position + new Vector3(0, 0.7f);
            sprite.GetComponent<SpriteRenderer>().sprite = actualHero.UnitSprite;
            
            if (!setupCells.Contains(_selectedCell)) return;
            _selectedCell.MarkAsPath();
        }

        public override void OnCellDeselected(Cell _unselectedCell)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = null;

            foreach (Cell _setupCell in setupCells)
            {
                _setupCell.MarkAsReachable();
            }

            if (actualHero != null)
                actualHero.MarkAsSelected();
        }
    }
}
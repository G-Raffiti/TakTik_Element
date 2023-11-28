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
        private List<MonsterSo> monsters;
        private Dictionary<BattleHero, GameObject> battleHeroes;
        private GameObject sprite;

        private BattleHero ActualHero { get; set; }
        
        public BattleStateBeginning(BattleStateManager _stateManager, List<MonsterSo> _monsters) : base(_stateManager)
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
            stateManager.Cells.ForEach(_c => _c.UnMark());
            
            List<Cell> _freeCells = stateManager.Cells.FindAll(_c => _c.IsWalkable && _c.Buffs.Count == 0 && !_c.isSpawnPlace);
            
            List<Cell> _enemiesCells = new List<Cell>();
            while (_enemiesCells.Count < monsters.Count)
            {
                int _cellIndex = Random.Range(0, _freeCells.Count);
                _enemiesCells.Add(_freeCells[_cellIndex]);
                _freeCells.Remove(_freeCells[_cellIndex]);
            }

            for (int _i = 0; _i < monsters.Count; _i++)
            {
                monsters[_i].Spawn(_enemiesCells[_i]);
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
                    if (!setupCells.Contains(_freeCells[_cellIndex]))
                        setupCells.Add(_freeCells[_cellIndex]);
                }
            }
            else setupCells = _spawnCells;

            foreach (Cell _setupCell in setupCells)
            {
                _setupCell.MarkAsReachable();
            }

            for (int _i = 0; _i < heroes.Count; _i++)
            {
                GameObject _pref = Object.Instantiate(heroes[_i].Prefab, GameObject.Find("Units").transform);
                heroes[_i].Spawn(_pref.GetComponent<BattleHero>());
                _pref.transform.position = setupCells[_i].transform.position;
                _pref.GetComponent<Unit>().InitializeSprite();
                battleHeroes.Add(_pref.GetComponent<BattleHero>(), _pref);
                setupCells[_i].Take(_pref.GetComponent<Unit>());
                heroes[_i].isPlaced = true;
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

        private void ChangeHero(Unit _hero)
        {
            ActualHero = (BattleHero) _hero;
        }

        private void PlaceHero(Cell _cell)
        {
            ActualHero.transform.position = _cell.transform.position;
            _cell.Take(ActualHero);
            ActualHero.Hero.isPlaced = true;
            ActualHero = null;
        }

        private void CheckCells()
        {
            // Check if 2 Heroes are on the same Cell
            List<Movable> _heroesOnSameCell = StateManager.ObjectCells
                .GroupBy(_z => _z.Value)
                .Where(_z => _z.Count() > 1)
                .SelectMany(_z => _z)
                .Select(_z => _z.Key)
                .ToList();
            if (_heroesOnSameCell.Count > 1)
            {
                foreach (Movable _movable in _heroesOnSameCell)
                {
                    Debug.Log(((BattleHero) _movable).unitName);
                }
                _heroesOnSameCell[0].Cell.Take(_heroesOnSameCell[0]);
                ChangeHero((BattleHero) _heroesOnSameCell[1]);
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
                if (ActualHero != null)
                    if (ActualHero.Cell != null)
                        ActualHero = null;
                CheckCells();
                return;
            }
            if (ActualHero == null && !_cell.IsTaken) 
            {
                CheckCells();
                return;
            }
            
            BattleHero _nextHero = null;
            if (_cell.CurrentUnit != null)
            {
                _nextHero = (BattleHero) _cell.CurrentUnit;
            }
            
            // Set the position of the ActualHero on the Cell
            if (ActualHero != null) PlaceHero(_cell);

            if (_nextHero != null)
            {
                ChangeHero(_nextHero);
                return;
            }
            
            CheckCells();
        }

        public override void OnCellSelected(Cell _targetCell)
        {
            if (ActualHero == null) return;
            sprite.transform.position = _targetCell.transform.position + new Vector3(0, 0.7f);
            sprite.GetComponent<SpriteRenderer>().sprite = ActualHero.UnitSprite;
            
            if (!setupCells.Contains(_targetCell)) return;
            _targetCell.MarkAsPath();
        }

        public override void OnCellDeselected(Cell _targetCell)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = null;

            foreach (Cell _setupCell in setupCells)
            {
                _setupCell.MarkAsReachable();
            }
        }
    }
}
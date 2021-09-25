using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using _Instances;
using BattleOver;
using Cells;
using Grid.GridStates;
using Grid.UnitGenerators;
using Players;
using StatusEffect;
using Units;
using UnityEngine;

namespace Grid
{
    [RequireComponent(typeof(Board))]
    public class BattleStateManager : MonoBehaviour
    {

        /// <summary>
        /// UnitAdded event is invoked each time AddUnit method is called.
        /// </summary>
        public event EventHandler<UnitCreatedEventArgs> UnitAdded;

        public InfoEvent TooltipOn;
        public VoidEvent TooltipOff;

        [SerializeField] private BoolEvent onBattleIsOver;
        private EndConditionSO endCondition;

        private BattleState battleState; //The grid delegates some of its behaviours to cellGridState object.
        public BattleState BattleState
        {
            get
            {
                return battleState;
            }
            set
            {
                if (battleState != null)
                    battleState.OnStateExit();
                battleState = value;
                battleState.OnStateEnter();
            }
        }

        public int NumberOfPlayers { get; private set; }
        
        public int CurrentPlayerNumber { get; protected set; }

        /// <summary>
        /// GameObject that holds player objects.
        /// </summary>
        public Transform playersParent;

        public BattleStateManager()
        {
            battleState = new BattleStateBlockInput(this);
        }

        public bool GameFinished { get; private set; }
        public List<Player> Players { get; private set; }
        public List<Cell> Cells { get; protected set; }
        public List<Unit> Units { get; protected set; }

        public void Initialize()
        {
            BattleState = new BattleStateBlockInput(this);
            if (!Equals(instance, this))
            {
                instance = this;
            }
            
            GameFinished = false;
            Players = new List<Player>();
            for (int _i = 0; _i < playersParent.childCount; _i++)
            {
                Player _player = playersParent.GetChild(_i).GetComponent<Player>();
                if (_player != null)
                    Players.Add(_player);
                else
                    Debug.LogError("Invalid object in Players Parent game object");
            }
            NumberOfPlayers = Players.Count;
            CurrentPlayerNumber = Players.Min(p => p.playerNumber);

            InitializeCells();
            
            NextCorruptionTurn = CorruptionTurn;
            StartCoroutine(BattleBeginning());
        }

        /// <summary>
        /// Method Called to register all Cells to Events and create Lists of Cells and GridObjects
        /// </summary>
        public virtual void InitializeCells()
        {
            Board _board = GetComponent<Board>();

            _board.LoadBoard();
            endCondition = _board.EndCondition;
            
            Cells = new List<Cell>();
            for (int _i = 0; _i < transform.childCount; _i++)
            {
                Cell _cell = transform.GetChild(_i).gameObject.GetComponent<Cell>();
                if (_cell != null)
                    Cells.Add(_cell);
                else
                    Debug.LogError("Invalid object in cells parent game object");
            }

            foreach (Cell _cell in Cells)
            {
                _cell.CellClicked += OnCellClicked;
                _cell.CellHighlighted += OnCellHighlighted;
                _cell.CellDehighlighted += OnCellDehighlighted;
                _cell.GetComponent<Cell>().GetNeighbours(Cells);
            }

            GridObjects = new List<GridObjects.GridObject>();
            foreach (Transform _transform in GameObject.Find("Objects").transform)
            {
                GridObjects.Add(_transform.gameObject.GetComponent<GridObjects.GridObject>());
            }
            GridObjects.ForEach(g => g.Initialize());
        }

        public virtual void InitializeUnits()
        {
            Units = new List<Unit>();
            IUnitGenerator _unitGenerator = GetComponent<IUnitGenerator>();
            if (_unitGenerator != null)
            {
                List<Unit> _units = _unitGenerator.SpawnUnits(Cells);
                foreach (Unit _unit in _units)
                {
                    AddUnit(_unit.GetComponent<Transform>());
                }
            }
            else
                Debug.LogError("No IUnitGenerator script attached to cell grid");
            
            Units.Sort((u1, u2) => u1.BattleStats.Speed.CompareTo(u2.BattleStats.Speed));

            for (int i = 0; i < Units.Count; i++)
            {
                Units[i].BattleStats.TurnPoint += TurnCost * 2 + Units[i].BattleStats.Speed + i;
            }
            Units.Reverse();
            PlayingUnit = Units[0];
        }

        protected void OnCellDehighlighted(object sender, EventArgs e)
        {
            BattleState.OnCellDeselected(sender as Cell);
        }

        protected void OnCellHighlighted(object sender, EventArgs e)
        {
            BattleState.OnCellSelected(sender as Cell);
        }

        protected void OnCellClicked(object sender, EventArgs e)
        {
            BattleState.OnCellClicked(sender as Cell);
        }

        /// <summary>
        /// Coroutine Called while a Unit is Dying
        /// </summary>
        private IEnumerator OnUnitDestroyed(object sender, DeathEventArgs e)
        {
            while (((Unit) sender).isDying)
                yield return null;
            Units.Remove((Unit) sender);
            
            if (endCondition.battleIsOver(this))
            {
                BattleState = new BattleStateBlockInput(this);
                onBattleIsOver.Raise(endCondition.WinCondition);
            }
        }

        private void TriggerOnUnitDestroyed(object sender, DeathEventArgs e)
        {
            StartCoroutine(OnUnitDestroyed(sender, e));
        }

        /// <summary>
        /// Adds unit to the game
        /// </summary>
        /// <param name="unit">Unit to add</param>
        public void AddUnit(Transform unit)
        {
            Units.Add(unit.GetComponent<Unit>());
            unit.GetComponent<Unit>().UnitDestroyed += TriggerOnUnitDestroyed;

            UnitAdded?.Invoke(this, new UnitCreatedEventArgs(unit));
        }

        /// <summary>
        /// Method is called once, at the beggining of the game.
        /// </summary>
        public virtual void StartGame()
        {
            List<Hero> heroesPlaced = GameObject.Find("Player").GetComponentsInChildren<Hero>().Where(h => h.isPlaced).ToList();
            if (heroesPlaced.Count <= 0) return;
            
            KeepBetweenScene.StartBattle();
            BattleState.OnStateExit();
            InitializeUnits();
            onStartGame.Raise();
            StartTurn();
        }
        /// <summary>
        /// Method makes turn transitions. It is called by player at the end of his turn.
        /// </summary>
        public virtual void EndTurn()
        {
            if (PlayingUnit is Monster {isPlaying: true}) return;
            
            BattleState = new BattleStateBlockInput(this);
            if (endCondition.battleIsOver(this))
            {
                onBattleIsOver.Raise(endCondition.WinCondition);
            }

            PlayingUnit.OnTurnEnd();
            SortByTurnPoints();
            foreach (Unit _unit in Units)
            {
                if (_unit.transform.position == _unit.Cell.transform.position) continue;
                Cell _closestCell = Cells.OrderBy(h => Math.Abs((h.transform.position - _unit.transform.position).magnitude)).First();
                List<Cell> _path = _unit.FindPathFrom(Cells,_closestCell, _unit.Cell);
                _unit.transform.position = _closestCell.transform.position;
                _unit.Move(_unit.Cell, _path);
            }

            foreach (Unit _unit in Units.Where(_unit => _unit.transform.position == _unit.Cell.transform.position))
            {
                _unit.transform.position = _unit.Cell.transform.position;
            }

            if (Turn >= NextCorruptionTurn)
                Corruption();
            
            StartTurn();
        }


        #region Const Turn Cost;
        private const int TurnCost = 20;
        private const int CorruptionTurn = 3;
        #endregion

        public int NextCorruptionTurn; 
        
        [SerializeField] private UnitEvent onUnitStartTurn;
        [SerializeField] public VoidEvent onStartGame;
        private float TurnCount = 1;
        public int Turn => (int)TurnCount;

        [SerializeField] private StatusSO CorruptionSO;

        public Unit PlayingUnit { get; private set; }

        public static BattleStateManager instance;
        
        public List<GridObjects.GridObject> GridObjects { get; private set; }

        private void Start()
        {
            Initialize();
        }

        [ContextMenu("Beginning")]
        public IEnumerator BattleBeginning()
        {
            yield return new WaitForSeconds(0.2f);
            BattleState = new BattleStateBeginning(this, BattleGenerator.GenerateEnemies(endCondition.Type));
        }

        private void SortByTurnPoints()
        {
            Units.Sort((u1, u2) => u1.BattleStats.TurnPoint.CompareTo(u2.BattleStats.TurnPoint));
            Units.Reverse();
        }

        private void StartTurn()
        {
            BattleState = new BattleStateBlockInput(this);
            TurnCount += 1f / Units.Count;
            foreach (Unit _unit in Units)
            {
                _unit.Buffs.ForEach(b => b.OnStartTurn(_unit));
            }

            foreach (GridObjects.GridObject _gridObject in GridObjects)
            {
                _gridObject.OnStartTurn();
            }
            
            PlayingUnit = Units[0];
            
            Debug.Log($"Player {PlayingUnit.playerNumber}: {PlayingUnit.UnitName} start Turn");

            PlayingUnit.BattleStats.TurnPoint -= TurnCost;
            foreach (Unit _unit in Units.Where(_unit => _unit != PlayingUnit))
            {
                _unit.BattleStats.TurnPoint += _unit.BattleStats.Speed;
            }

            Players.Find(p => p.playerNumber == PlayingUnit.playerNumber).Play(this);
            onUnitStartTurn.Raise(PlayingUnit);
        }

        private void Corruption()
        {
            NextCorruptionTurn += 1;

            if (NextCorruptionTurn <= CorruptionTurn + 1)
            {
                Cells.Where(c => c.Neighbours.Count < 4).ToList().ForEach(CorruptCell);
                return;
            }
            
            Cells.Where(_cell => _cell.Neighbours.Any(_neighbourTile => _neighbourTile.isCorrupted)).ToList().ForEach(CorruptCell);
        }

        private void CorruptCell(Cell _cell)
        {
            _cell.AddBuff(new Buff(_cell, CorruptionSO));
            _cell.isCorrupted = true;
        }

        /// <summary>
        /// Method Called after using a Skill or after that any Unit Fall in an UnderGround Tile.
        /// </summary>
        public void OnSkillUsed()
        {
            // Verification if the Playing Unit is still alive
            if (PlayingUnit != Units[0])
            {
                EndTurn();
                return;
            }

            // This Method Work only during the Player Turn
            if (PlayingUnit.playerNumber != 0) return;
            BattleState = new BattleStateBlockInput(this);
            BattleState = new BattleStateUnitSelected(this, PlayingUnit);
        }

        public void BlockInputs()
        {
            // This Method Work only during the Player Turn
            if (PlayingUnit.playerNumber == 0)
                BattleState = new BattleStateBlockInput(this);
        }

        public void AddCell(Cell _newCell, Cell _toDestroy)
        {
            if (_toDestroy.isTaken)
            {
                if (_toDestroy.CurrentGridObject != null)
                {
                    GridObjects.Remove(_toDestroy.CurrentGridObject);
                }

                if (_toDestroy.CurrentUnit != null)
                {
                    Cell destination = _toDestroy.Neighbours.Where(c => !c.isTaken).ToList()[0];
                    if (destination != null)
                        _toDestroy.CurrentUnit.Move(destination, new List<Cell>() {destination});
                    else _toDestroy.CurrentUnit.OnDestroyed();
                }
            }
            Cells.Add(_newCell);
            Cells.Remove(_toDestroy);
            _newCell.CellClicked += OnCellClicked;
            _newCell.CellHighlighted += OnCellHighlighted;
            _newCell.CellDehighlighted += OnCellDehighlighted;
        }

        public void RemoveGridObject(Cell _toDestroy)
        {
            if (!_toDestroy.isTaken) return;
            if (_toDestroy.CurrentGridObject != null)
            {
                GridObjects.Remove(_toDestroy.CurrentGridObject);
            }
            OnSkillUsed();
        }
    }
}
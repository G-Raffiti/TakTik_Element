using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using _Instances;
using Cells;
using EndConditions;
using Gears;
using GridObjects;
using Players;
using Skills;
using StateMachine.GridStates;
using StateMachine.UnitGenerators;
using StatusEffect;
using Units;
using UnityEngine;
using Void = _EventSystem.CustomEvents.Void;

namespace StateMachine
{
    [RequireComponent(typeof(Board))]
    public class BattleStateManager : MonoBehaviour
    {
        public static BattleStateManager instance;
        private BattleState battleState;                                //The grid delegates some of its behaviours to cellGridState object.
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

        public EndConditionSO endCondition { get; private set; }
        private List<Player> Players { get; set; }
        public List<Cell> Cells { get; private set; }
        public List<Unit> Units { get; private set; }
        public List<Unit> DeadThisTurn { get; private set; }
        public List<GridObject> GridObjects { get; private set; }
        public Unit PlayingUnit { get; private set; }
        
        [Header("Event Listener")]
        [SerializeField] private VoidEvent onStartBattle;
        [SerializeField] private VoidEvent onEndTurn;
        [SerializeField] private CellEvent onGridObjectDestroyed;
        [SerializeField] private VoidEvent onSkillUsed;
        [SerializeField] private VoidEvent onUIEnable;
        [SerializeField] private VoidEvent onMonsterPlay;
        [SerializeField] private VoidEvent onActionDone;
        [SerializeField] private SkillEvent onSkillSelected;
        
        [Header("Event Sender")]
        [SerializeField] private VoidEvent onBattleStarted;
        [SerializeField] private BoolEvent onBattleIsOver;
        [SerializeField] private UnitEvent onUnitStartTurn;
        
        [Header("Event Holder")]
        [SerializeField] private InfoEvent tooltipOn;
        [SerializeField] private VoidEvent tooltipOff;
        [SerializeField] private UnitEvent onHeroSelected;
        
        public InfoEvent TooltipOn => tooltipOn;
        public VoidEvent TooltipOff => tooltipOff;
        public UnitEvent OnHeroSelected => onHeroSelected;


        private const int TurnCost = 20;
        private const int CorruptionTurn = 3;
        public int NextCorruptionTurn { get; private set; }
        private float TurnCount = 1;
        public int Turn => (int)TurnCount;
        public bool GameStarted { get; set; }


        /// <summary>
        /// GameObject that holds player objects.
        /// </summary>
        //public Transform playersParent;
        private void Start()
        {
            DeadThisTurn = new List<Unit>();
            onEndTurn.EventListeners += EndTurn;
            onGridObjectDestroyed.EventListeners += RemoveGridObject;
            onSkillUsed.EventListeners += SkillUsed;
            onUIEnable.EventListeners += BlockInputs;
            onMonsterPlay.EventListeners += BlockInputs;
            onActionDone.EventListeners += ActionDone;
            onStartBattle.EventListeners += StartBattle;
            onSkillSelected.EventListeners += SkillSelected;
            Initialize();
        }

        private void OnDestroy()
        {
            onEndTurn.EventListeners -= EndTurn;
            onGridObjectDestroyed.EventListeners -= RemoveGridObject;
            onSkillUsed.EventListeners -= SkillUsed;
            onUIEnable.EventListeners -= BlockInputs;
            onMonsterPlay.EventListeners -= BlockInputs;
            onActionDone.EventListeners -= ActionDone;
            onStartBattle.EventListeners -= StartBattle;
            onSkillSelected.EventListeners -= SkillSelected;
        }
        
    #region Event Handler
        private void BlockInputs(Void _obj)
        {
            BlockInputs();
        }

        private void SkillUsed(Void _obj)
        {
            SkillUsed();
        }
        
        private void EndTurn(Void item)
        {
            EndTurn();
        }

        private void ActionDone(Void _obj)
        {
            ActionDone();
        }
        
        private void StartBattle(Void _obj)
        {
            StartBattle();
        }
        
        /// <summary>
        /// Link the CellsEvent to the BattleState Deselected
        /// </summary>
        private void OnCellUnselected(object sender, EventArgs e)
        {
            BattleState.OnCellDeselected(sender as Cell);
        }

        /// <summary>
        /// Link the CellsEvent to the BattleState Selected
        /// </summary>
        private void OnCellSelected(object sender, EventArgs e)
        {
            BattleState.OnCellSelected(sender as Cell);
        }

        /// <summary>
        /// Link the CellsEvent to the BattleState Clicked
        /// </summary>
        private void OnCellClicked(object sender, EventArgs e)
        {
            BattleState.OnCellClicked(sender as Cell);
        }

        /// <summary>
        /// Method Called by a DeathEvent to Start the On UnitDestroyed Coroutine
        /// </summary>
        private void TriggerOnUnitDestroyed(object sender, DeathEventArgs e)
        {
            StartCoroutine(UnitDestroyed(sender));
        }
        
    #endregion

    #region Initialisation

        
        /// <summary>
        /// Method Called in Start to Setup the scene
        /// </summary>
        private void Initialize()
        {
            Transform playersParent = GameObject.Find("Players").transform;
            
            BattleState = new BattleStateBlockInput(this);
            if (!Equals(instance, this))
            {
                instance = this;
            }
            
            Players = new List<Player>();
            for (int _i = 0; _i < playersParent.childCount; _i++)
            {
                Player _player = playersParent.GetChild(_i).GetComponent<Player>();
                if (_player != null)
                    Players.Add(_player);
                else
                    Debug.LogError("Invalid object in Players Parent game object");
            }

            InitializeCells();
            
            NextCorruptionTurn = CorruptionTurn;
            StartCoroutine(BattleBeginning());
        }
        private IEnumerator BattleBeginning()
        {
            //TODO : Trouver solution pour ne pas attendre un temps fix
            yield return new WaitForSeconds(0.2f);
            BattleState = new BattleStateBeginning(this, BattleGenerator.GenerateEnemies(endCondition.Type));
        }

        /// <summary>
        /// Method Called to register all Cells to Events and create Lists of Cells and GridObjects
        /// </summary>
        private void InitializeCells()
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
                _cell.CellHighlighted += OnCellSelected;
                _cell.CellDehighlighted += OnCellUnselected;
                _cell.GetComponent<Cell>().GetNeighbours(Cells);
            }

            GridObjects = new List<GridObjects.GridObject>();
            foreach (Transform _transform in GameObject.Find("Objects").transform)
            {
                GridObjects.Add(_transform.gameObject.GetComponent<GridObjects.GridObject>());
            }
            GridObjects.ForEach(g => g.Initialize());
        }

        #endregion

    #region Battle Start

        /// <summary>
        /// Method is called once, at the beggining of the game.
        /// </summary>
        private void StartBattle()
        {
            List<Hero> heroesPlaced = GameObject.Find("Player").GetComponentsInChildren<Hero>().Where(h => h.isPlaced).ToList();
            if (heroesPlaced.Count <= 0) return;
            
            KeepBetweenScene.StartBattle();
            BattleState.OnStateExit();
            InitializeUnits();
            onBattleStarted.Raise();
            GameStarted = true;
            StartTurn();
        }
        
        /// <summary>
        /// Method Called to Snap Units to their Cells and register all Units.
        /// </summary>
        private void InitializeUnits()
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
                Units[i].TurnPoint += TurnCost * 2 + Units[i].BattleStats.Speed + i;
            }
            Units.Reverse();
            PlayingUnit = Units[0];
        }

        #endregion

        /// <summary>
        /// Method makes turn transitions. It is called by player at the end of his turn.
        /// </summary>
        private void EndTurn()
        {
            if (PlayingUnit != null && PlayingUnit is Monster {isPlaying: true}) return;

            BattleState = new BattleStateBlockInput(this);
            
            if (endCondition.battleIsOver(this))
            {
                onBattleIsOver.Raise(endCondition.WinCondition);
            }
            
            Cells.ForEach(c => c.OnEndTurn());
            
            foreach (Cell _cell in Cells.Where(c => c.IsTaken && c.CurrentUnit != null && c.Buffs.Count > 0))
            {
                _cell.Buffs.ForEach(b => b.OnEndTurn(_cell.CurrentUnit));
            }
            
            if (PlayingUnit != null)
            {
                PlayingUnit.OnTurnEnd();
            }
            
            SortByTurnPoints();
            foreach (Unit _unit in Units)
            {
                if (_unit.transform.position == _unit.Cell.transform.position) continue;
                Cell _closestCell = Cells.OrderBy(h => Math.Abs((h.transform.position - _unit.transform.position).magnitude)).First();
                List<Cell> _path = _unit.FindPathFrom(Cells,_closestCell, _unit.Cell);
                _unit.transform.position = _closestCell.transform.position;
                _unit.Move(_unit.Cell, _path);
            }

            foreach (Unit _unit in Units.Where(_unit => _unit.transform.position != _unit.Cell.transform.position))
            {
                _unit.transform.position = _unit.Cell.transform.position;
            }

            if (Turn >= NextCorruptionTurn)
                Corruption();
            
            StartTurn();
        }
        private void StartTurn()
        {
            if (endCondition.battleIsOver(this))
            {
                BattleState = new BattleStateBlockInput(this);
                onBattleIsOver.Raise(endCondition.WinCondition);
            }
            DeadThisTurn = new List<Unit>();
            
            BattleState = new BattleStateBlockInput(this);

            PlayingUnit = Units[0];
            
            TurnCount += 1f / Units.Count;
            foreach (Unit _unit in Units)
            {
                _unit.Buffs.ForEach(b => b.OnStartTurn(_unit));
                _unit.Cell.Buffs.ForEach(b => b.OnStartTurn(_unit));
            }

            foreach (GridObjects.GridObject _gridObject in GridObjects)
            {
                _gridObject.OnStartTurn();
            }

            Debug.Log($"Player {PlayingUnit.playerNumber}: {PlayingUnit.UnitName} start Turn");

            PlayingUnit.TurnPoint -= TurnCost;
            foreach (Unit _unit in Units.Where(_unit => _unit != PlayingUnit))
            {
                _unit.TurnPoint += _unit.BattleStats.Speed;
            }

            Players.Find(p => p.playerNumber == PlayingUnit.playerNumber).Play(this);
            onUnitStartTurn.Raise(PlayingUnit);
        }

        private void SortByTurnPoints()
        {
            Units.Sort((u1, u2) => u1.TurnPoint.CompareTo(u2.TurnPoint));
            Units.Reverse();
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
            _cell.AddBuff(new Buff(_cell, DataBase.Cell.CorruptionSO));
            _cell.isCorrupted = true;
        }
            
        /// <summary>
        /// Method Called to set the State to the last State.
        /// </summary>
        private void ActionDone()
        {
            // Verification if the Playing Unit is still alive
            if (PlayingUnit != Units[0])
            {
                EndTurn();
                return;
            }

            // This Method Work only during the Player Turn
            if (PlayingUnit.playerNumber != 0) return;
            BattleState = new BattleStateUnitSelected(this, PlayingUnit);
        }

        private void SkillSelected(SkillInfo skill)
        {
            BattleState = new BattleStateSkillSelected(this, skill);
        }
        
        /// <summary>
        /// Method Called after using a Skill or after that any Unit Fall in an UnderGround Tile.
        /// </summary>
        private void SkillUsed()
        {
            // Verification if the Playing Unit is still alive
            if (PlayingUnit != Units[0])
            {
                EndTurn();
                return;
            }

            // This Method Work only during the Player Turn
            if (PlayingUnit.playerNumber != 0) return;
            BattleState = new BattleStateUnitSelected(this, PlayingUnit);
        }

        /// <summary>
        /// Method Called to Save Block Inputs (Used when UI is enabled or while AI play)
        /// </summary>
        private void BlockInputs()
        {
            BattleState = new BattleStateBlockInput(this);
        }

        /// <summary>
        /// Add a Cell to the Grid
        /// </summary>
        /// <param name="_newCell"></param>
        /// <param name="_toDestroy"></param>
        public void AddCell(Cell _newCell, Cell _toDestroy)
        {
            if (_toDestroy.IsTaken)
            {
                if (_toDestroy.CurrentGridObject != null)
                {
                    GridObjects.Remove(_toDestroy.CurrentGridObject);
                }

                if (_toDestroy.CurrentUnit != null)
                {
                    Cell destination = _toDestroy.Neighbours.Where(c => !c.IsTaken).ToList()[0];
                    if (destination != null)
                        _toDestroy.CurrentUnit.Move(destination, new List<Cell>() {destination});
                    else StartCoroutine(_toDestroy.CurrentUnit.OnDestroyed());
                }
            }
            Cells.Add(_newCell);
            Cells.Remove(_toDestroy);
            _newCell.CellClicked += OnCellClicked;
            _newCell.CellHighlighted += OnCellSelected;
            _newCell.CellDehighlighted += OnCellUnselected;
        }

        private void RemoveGridObject(Cell _toDestroy)
        {
            BlockInputs();
            if (_toDestroy.CurrentGridObject != null)
            {
                GridObjects.Remove(_toDestroy.CurrentGridObject);
            }
        }
        
        /// <summary>
        /// Coroutine Called when a Unit is Dying
        /// </summary>
        private IEnumerator UnitDestroyed(object sender)
        {
            while (((Unit) sender).isDying)
                yield return null;
           
            Units.Remove((Unit) sender);
            if (sender is Monster _sender)
                DeadThisTurn.Add(_sender);
            
            if ((Unit) sender == PlayingUnit)
            {
                PlayingUnit = null;
                EndTurn();
            }
        }

        /// <summary>
        /// Adds unit to the game
        /// </summary>
        /// <param name="unit">Unit to add</param>
        private void AddUnit(Transform unit)
        {
            Units.Add(unit.GetComponent<Unit>());
            unit.GetComponent<Unit>().UnitDestroyed += TriggerOnUnitDestroyed;
        }
    }
}
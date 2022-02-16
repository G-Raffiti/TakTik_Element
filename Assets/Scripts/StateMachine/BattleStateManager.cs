using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using _Instances;
using Buffs;
using Cells;
using EndConditions;
using GridObjects;
using Players;
using Relics;
using Relics.ScriptableObject_RelicEffect;
using Skills;
using StateMachine.GridStates;
using Units;
using UnityEngine;
using Void = _EventSystem.CustomEvents.Void;

namespace StateMachine
{
    /// <summary>
    /// class <c>BattleStateManager</c> handle the state pattern of the battle scene.
    /// Singleton 
    /// </summary>
    [RequireComponent(typeof(Board))]
    public class BattleStateManager : MonoBehaviour
    {
        ////////////////////// VARIABLES ///////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// <c>instance</c> is the reference to the singleton object
        /// </summary>
        public static BattleStateManager instance;
        /// <summary>
        /// Instance variable <c>battleState</c>. Define the actual <c>BattleState</c>.
        /// The grid delegates some of its behaviours to <c>BattleStateManager</c> object.
        /// </summary>
        private BattleState battleState;
        /// <value>Property <c>BattleState</c> give access to <c>battleState</c>
        /// and manage the state transition.</value>
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
        
        /// <value>Property <c>endCondition</c> define the end condition type of the board.</value>
        public EndConditionSO endCondition { get; private set; }
        /// <value>Property <c>Players</c> list of all <c>Player</c>.</value>
        private List<Player> Players { get; set; }
        /// <value>Property <c>Cells</c> list of all <c>Cell</c>.</value>
        public List<Cell> Cells { get; private set; }
        /// <value>Property <c>Units</c> list of all <c>Unit</c> still alive in the battle.</value>
        public List<Unit> Units { get; private set; }
        /// <value> Property <c>Monsters</c> list of all enemies <c>Unit</c> still alive in the battle. </value>
        public List<Monster> Monsters
        {
            get
            {
                List<Monster> monsters = new List<Monster>();
                foreach (Unit unit in Units.Where(u => u.playerType != EPlayerType.HUMAN)) 
                {
                    if (unit is Monster monster)
                        monsters.Add(monster);
                }

                return monsters;
            }
        }
        /// <value>Property <c>DeadThisTurn</c> list of all <c>Monster</c> dead during this turn.</value>
        public List<Monster> DeadThisTurn { get; private set; }
        /// <value>Property <c>GridObjects</c> list of all <c>GridObject</c> present in the scene.</value>
        public List<GridObject> GridObjects { get; private set; }
        /// <summary>
        /// Instance variable <c>PlayingUnit</c> is the <c>Unit</c> which is currently playing.
        /// </summary>
        public Unit PlayingUnit { get; private set; }


        ////////////////////// EVENTS //////////////////////////////////////////////////////////////////////////////////
        
        [Header("Event Listener")]
        [SerializeField] private VoidEvent onStartBattle;
        [SerializeField] private VoidEvent onEndTurn;
        [SerializeField] private GridObjectEvent onGridObjectDestroyed;
        [SerializeField] private VoidEvent onSkillUsed;
        [SerializeField] private VoidEvent onUIEnable;
        [SerializeField] private VoidEvent onMonsterPlay;
        [SerializeField] private VoidEvent onActionDone;
        [SerializeField] private SkillEvent onSkillSelected;
        [SerializeField] private UnitEvent onUnitMoved;
        [SerializeField] private VoidEvent onBattleEndTrigger;

        [Header("Event Sender")]
        [SerializeField] private VoidEvent onBattleStarted;
        [SerializeField] private BoolEvent onBattleIsOver;
        [SerializeField] private UnitEvent onUnitStartTurn;

        /// <summary> Constant variable <c>TurnCost</c> is the point's amount subtracted to the
        /// <c>PlayingUnit</c>'s turn points. </summary>
        private const int TurnCost = 20;

        /// <summary> Constant variable <c>CorruptionTurn</c> represents the number of turn before corruption
        /// started. </summary>
        private const int CorruptionTurn = 3;

        /// <value> Property <c>NextCorruptionTurn</c> represents the number of turn before next corruption. </value>
        public int NextCorruptionTurn { get; private set; }

        /// <summary> Instance variable <c>TurnCount</c> represents the turn counter. </summary>
        private float TurnCount = 1;

        /// <value> Property <c>Turn</c> represents the number of actual turn. </value>
        public int Turn => (int)TurnCount;

        /// <summary> Instance variable <c>ObjectCells</c> store cells for each movable objects. </summary>
        public Dictionary<IMovable, Cell> ObjectCells { get; private set; } = new Dictionary<IMovable, Cell>();

        
        ////////////////////// METHODS /////////////////////////////////////////////////////////////////////////////////
        
        private void Start()
        {
            DeadThisTurn = new List<Monster>();
            onEndTurn.EventListeners += EndTurn;
            onGridObjectDestroyed.EventListeners += RemoveGridObject;
            onSkillUsed.EventListeners += SkillUsed;
            onUIEnable.EventListeners += SetStateBlockInputs;
            onMonsterPlay.EventListeners += SetStateBlockInputs;
            onActionDone.EventListeners += ActionDone;
            onStartBattle.EventListeners += StartBattle;
            onSkillSelected.EventListeners += SetStateSkillSelected;
            onUnitMoved.EventListeners += UnitMoved;
            onBattleEndTrigger.EventListeners += UnitsEndBattle;
            Initialize();
        }
        
        private void OnDestroy()
        {
            onEndTurn.EventListeners -= EndTurn;
            onGridObjectDestroyed.EventListeners -= RemoveGridObject;
            onSkillUsed.EventListeners -= SkillUsed;
            onUIEnable.EventListeners -= SetStateBlockInputs;
            onMonsterPlay.EventListeners -= SetStateBlockInputs;
            onActionDone.EventListeners -= ActionDone;
            onStartBattle.EventListeners -= StartBattle;
            onSkillSelected.EventListeners -= SetStateSkillSelected;
            onUnitMoved.EventListeners -= UnitMoved;
            onBattleEndTrigger.EventListeners -= UnitsEndBattle;
        }
        
        #region StateManager
        
        /// <summary>
        /// Method <c>SetStateSkillSelected</c> transition to skill selected state.
        /// </summary>
        /// <param name="skill">the selected skill</param>
        private void SetStateSkillSelected(SkillInfo skill)
        {
            BattleState = new BattleStateSkillSelected(this, skill);
        }
        
        /// <summary>
        /// Method <c>SetStateBlockInputs</c> transition to block input state.
        /// </summary>
        private void SetStateBlockInputs()
        {
            BattleState = new BattleStateBlockInput(this);
        }
        
        /// <summary>
        /// Method <c>SetStateUnitSelected</c> transition to unit selected state.
        /// </summary>
        /// <param name="unit">the selected unit</param>
        private void SetStateUnitSelected(Unit unit)
        {
            BattleState = new BattleStateUnitSelected(this, unit);
        }
        
        #endregion

        #region Event Handler
        private void SetStateBlockInputs(Void _obj)
        {
            SetStateBlockInputs();
        }

        private void SkillUsed(Void _obj)
        {
            CheckPlayingUnitAlive();
        }
        
        private void EndTurn(Void item)
        {
            EndTurn();
        }

        private void ActionDone(Void _obj)
        {
            CheckPlayingUnitAlive();
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
        
        /// <summary>
        /// Trigger all Relic effect "On Battle End"
        /// </summary>
        private void UnitsEndBattle(Void _obj)
        {
            //TODO: A mettre dans Unit -> catch event battleisover
            foreach (Unit hero in Units.Where(u => u.playerType == EPlayerType.HUMAN))
            {
                Hero _hero = ((BattleHero)hero).Hero;
                foreach (RelicSO _relicSO in _hero.Relics)
                {
                    foreach (RelicEffect _effect in _relicSO.RelicEffects)
                    {
                        _effect.OnEndFight(_hero, _relicSO);
                    }
                }
            }
        }
        
    #endregion
    
        #region Initialisation

        
        /// <summary>
        /// method <c>Initialize</c> setup the scene.
        /// </summary>
        private void Initialize()
        {
            BattleState = new BattleStateBlockInput(this);
            if (!Equals(instance, this))
            {
                instance = this;
            }

            InitializePlayers();
            InitializeCells();
            InitializeGridObjects();
            
            NextCorruptionTurn = CorruptionTurn;
            BattleState = new BattleStateBeginning(this, BattleGenerator.GenerateEnemies(endCondition.Type));
        }

        /// <summary>
        /// method <c>InitializePlayers</c> retrieve all players in the scene.
        /// </summary>
        private void InitializePlayers()
        {
            Transform playersParent = GameObject.Find("Players").transform;
            
            Players = new List<Player>();
            for (int _i = 0; _i < playersParent.childCount; _i++)
            {
                Player _player = playersParent.GetChild(_i).GetComponent<Player>();
                if (_player != null)
                    Players.Add(_player);
                else
                    Debug.LogError("Invalid object in Players Parent game object");
            }
        }
        
        /// <summary>
        /// Method <c>InitializeCells</c> register all Cells to Events and create Lists of Cells and GridObjects
        /// </summary>
        private void InitializeCells()
        {
            Board _board = GetComponent<Board>();

            _board.LoadBoard(BattleStage.currentState);
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
        }
        
        /// <summary>
        /// method <c>InitializeGridObjects</c> retrieve all gridobjects in the scene.
        /// </summary>
        private void InitializeGridObjects()
        {
            GridObjects = new List<GridObject>();
            foreach (Transform _transform in GameObject.Find("Objects").transform)
            {
                GridObjects.Add(_transform.gameObject.GetComponent<GridObject>());
            }
            GridObjects.ForEach(g => g.Initialize());
        }

        /// <summary>
        /// Method Called to Snap Units to their Cells and register all Units.
        /// </summary>
        private void InitializeUnits()
        {
            Units = GameObject.Find("Units").GetComponentsInChildren<Unit>().ToList();
            foreach (Unit unit in Units)
            {
                unit.UnitDestroyed += TriggerOnUnitDestroyed;
            }

            Units.Sort((u1, u2) => u1.BattleStats.Speed.CompareTo(u2.BattleStats.Speed));

            for (int i = 0; i < Units.Count; i++)
            {
                Units[i].TurnPoint += TurnCost * 2 + Units[i].BattleStats.Speed + i;
            }
            Units.Reverse();
            PlayingUnit = Units[0];

            foreach (Cell _cell in Cells.Where(c => c.GetCurrentIMovable() != null))
            {
                if (!ObjectCells.Keys.Contains(_cell.GetCurrentIMovable()))
                    ObjectCells.Add(_cell.GetCurrentIMovable(), _cell);
            }
        }
        
        #endregion
        
        /// <summary>
        /// Method is called once, at the beginning of the game.
        /// </summary>
        private void StartBattle()
        {
            SetStateBlockInputs();
            InitializeUnits();
            onBattleStarted.Raise();
            StartTurn();
        }

        /// <summary>
        /// Method makes turn transitions. It is called by player at the end of his turn.
        /// </summary>
        private void EndTurn()
        {
            if (instance.Monsters.Where(m => m.isDying).ToList().Count > 0) return;
            if (PlayingUnit != null && PlayingUnit is Monster {isPlaying: true}) return;
            
            BattleState = new BattleStateBlockInput(this);

            if (Check()) return;
            
            Cells.ForEach(c => c.OnEndTurn());

            foreach (Unit _unit in Units)
            {
                if (PlayingUnit != null && _unit == PlayingUnit)
                {
                    PlayingUnit.EndTurn();
                }
                _unit.OnTurnEnds();
            }
            
            SortByTurnPoints();

            if (Turn >= NextCorruptionTurn)
                Corruption();
            
            StartTurn();
        }
        
        /// <summary>
        /// method <c>StartTurn</c> manage the start turn.
        /// Defines the playing Unit and start it's turn.
        /// </summary>
        private void StartTurn()
        {
            BattleState = new BattleStateBlockInput(this);
            if (Check()) return;
            DeadThisTurn = new List<Monster>();

            PlayingUnit = Units[0];
            
            TurnCount += 1f / Units.Count;
            foreach (Unit _unit in Units)
            {
                _unit.OnTurnStarts();
            }

            foreach (GridObject _gridObject in GridObjects)
            {
                _gridObject.OnStartTurn();
            }

            Debug.Log(PlayingUnit.playerType == EPlayerType.HUMAN ? $"Player's Turn {PlayingUnit.ColouredName()}" : $"IA's Turn {PlayingUnit.ColouredName()}", DLogType.Error);

            // TODO: mettre dans Unit
            PlayingUnit.TurnPoint -= TurnCost;
            foreach (Unit _unit in Units.Where(_unit => _unit != PlayingUnit))
            {
                _unit.TurnPoint += _unit.BattleStats.Speed;
            }

            Players.Find(p => p.playerType == PlayingUnit.playerType).Play(this);
            onUnitStartTurn.Raise(PlayingUnit);
        }
        
        private void SortByTurnPoints()
        {
            Units.Sort((u1, u2) => u1.TurnPoint.CompareTo(u2.TurnPoint));
            Units.Reverse();
        }
        
        /// <summary>
        /// method <c>Corruption</c> update the corruption and corrupt the cells.
        /// </summary>
        private void Corruption()
        {
            NextCorruptionTurn += 1;
            
            //TODO: la cellule gère sa propre corruption (send corruption event a toute les cellules)
            if (NextCorruptionTurn <= CorruptionTurn + 1)
            {
                Cells.Where(c => c.Neighbours.Count < 4).ToList().ForEach(CorruptCell);
                return;
            }
            
            Cells.Where(_cell => _cell.Neighbours.Any(_neighbourTile => _neighbourTile.isCorrupted)).ToList().ForEach(CorruptCell);
        }

        /// <summary>
        /// method <c>CorruptCell</c> corrupt the given cell.
        /// </summary>
        /// <param name="_cell">the cell to corrupt</param>
        private void CorruptCell(Cell _cell)
        {
            _cell.AddBuff(new Buff(_cell, DataBase.Cell.CorruptionSO));
            _cell.isCorrupted = true;
        }
            
        /// <summary>
        /// Method <c>CheckPlayingUnitAlive</c> End turn if the playing unit is dead.
        /// </summary>
        private void CheckPlayingUnitAlive()
        {
            // Verification if the Playing Unit is still alive
            if (PlayingUnit != Units[0])
            {
                EndTurn();
                return;
            }

            // This Method Work only during the Player Turn
            if (PlayingUnit.playerType != EPlayerType.HUMAN) return;
            SetStateUnitSelected(PlayingUnit);
        }
        
        /// <summary>
        /// method <c>Check</c> verifies the end condition.
        /// </summary>
        /// <returns>true if the end condition is satisfied, false otherwise</returns>
        public bool Check()
        {
            if (endCondition.battleIsOver(this))
            {
                BattleState = new BattleStateBlockInput(this);
                onBattleIsOver.Raise(endCondition.WinCondition);
                return true;
            }

            return false;
        }

        /// <summary>
        /// method <c>UnitMoved</c> called each time a unit finish a move.
        /// </summary>
        /// <param name="unit">the moved unit</param>
        private void UnitMoved(Unit unit)
        {
            if (unit == PlayingUnit)
                SetStateUnitSelected(PlayingUnit);
        }

        /// <summary>
        /// method <c>RemoveGridObject</c> remove the given object from the list of grid objects.
        /// </summary>
        /// <param name="_toDestroy">the object to destroy</param>
        private void RemoveGridObject(GridObject _toDestroy)
        {
            SetStateBlockInputs();
            GridObjects.Remove(_toDestroy);
        }
        
        /// <summary>
        /// Coroutine Called when a Unit is Dying
        /// </summary>
        private IEnumerator UnitDestroyed(object sender)
        {
            while (((Unit) sender).isDying)
                yield return null;
           
            Units.Remove((Unit) sender);
            ObjectCells.Remove((Unit) sender);

            if (sender is BattleHero)
            {
                Check();
            }

            if (sender is Monster _sender)
                DeadThisTurn.Add(_sender);
            
            if ((Unit) sender == PlayingUnit)
            {
                PlayingUnit = null;
                EndTurn();
            }
        }

        /// <summary>
        /// Method called every cell crossed by a Unit or a Grid Object to Actualise the Dictionnary.
        /// </summary>
        /// <param name="_movable">the object to move</param>
        /// <param name="_cell">the destination cell</param>
        public void OnIMovableMoved(IMovable _movable, Cell _cell)
        {
            if (_cell == null && ObjectCells.Keys.Contains(_movable))
            {
                ObjectCells.Remove(_movable);
                return;
            }
            if (!ObjectCells.Keys.Contains(_movable))
                ObjectCells.Add(_movable, _cell);
            ObjectCells[_movable] = _cell;

            if (BattleState.State == EBattleState.Beginning) return;
            
            if (ObjectCells.Values.Distinct().Count() != ObjectCells.Count)
            {
                Debug.LogError("A cell is occupied by two entities");
            }

            if (ObjectCells.Values.Any(c => c == null))
            {
                foreach (KeyValuePair<IMovable,Cell> _keyValuePair in ObjectCells)
                {
                    if (_keyValuePair.Value == null)
                    {
                        StartCoroutine(_keyValuePair.Key.OnDestroyed());
                    }
                }
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
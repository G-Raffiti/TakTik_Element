using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using _ScriptableObject;
using Cells;
using Gears;
using Grid;
using Grid.GridStates;
using Pathfinding.Algorithms;
using Resources.ToolTip.Scripts;
using Stats;
using StatusEffect;
using TMPro;
using Units.UnitStates;
using UnityEngine;
using UserInterface;

namespace Units
{
    /// <summary>
    /// Base class for all units in the game.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class Unit : IMovable
    {
        
        public string UnitName;
        public override string getName()
        {
            return UnitName;
        }
        
    #region Event Handler
        /// <summary>
        /// UnitSelected event is invoked when user clicks on unit that belongs to him. 
        /// It requires a collider on the unit game object to work.
        /// </summary>
        public event EventHandler UnitSelected;
        /// <summary>
        /// UnitDeselected event is invoked when user click outside of currently selected unit's collider.
        /// It requires a collider on the unit game object to work.
        /// </summary>
        public event EventHandler UnitDeselected;
        /// <summary>
        /// UnitAttacked event is invoked when the unit is attacked.
        /// </summary>
        public event EventHandler<AttackEventArgs> UnitAttacked;
        /// <summary>
        /// UnitDestroyed event is invoked when unit's BattleStats.HP drop below 0.
        /// </summary>
        public event EventHandler<DeathEventArgs> UnitDestroyed;
        /// <summary>
        /// UnitMoved event is invoked when unit moves from one cell to another.
        /// </summary>
        [SerializeField] private UnitEvent onUnitMoved;
        
        protected virtual void OnMouseDown()
        {
            if(cell != null)
                cell.OnMouseDown();
        }
        protected virtual void OnMouseEnter()
        {
            if(cell != null)
                cell.OnMouseEnter();
        }
        protected virtual void OnMouseExit()
        {
            if(cell != null)
                cell.OnMouseExit();
        }

    #endregion

    #region Unit State

        public UnitState UnitState { get; set; }
        public void SetState(UnitState state)
        {
            UnitState.MakeTransition(state);
        }

    #endregion

    #region Unit Stats
        
        /// <summary>
        /// Actual Stats of the Unit, Can be modified from all Sources,
        /// </summary>
        public BattleStats BattleStats;
        
        /// <summary>
        /// The Stats of the Unit before any alteration. 
        /// </summary>
        protected BattleStats baseStats;
        
        /// <summary>
        /// Contain all the Total Stats (max Life, max AP, MP etc...)
        /// </summary>
        protected BattleStats total;
        
        /// <summary>
        /// Propriety that contain all the Total Stats (max Life, max AP, MP etc...)
        /// </summary>
        public BattleStats Total => total;

        /// <summary>
        /// Indicates the player that the unit belongs to. 
        /// Should correspoond with PlayerNumber variable on Player script.
        /// </summary>
        public int playerNumber;

    #endregion

    #region Unit Buffs

        /// <summary>
        /// A list of buffs that are applied to the unit.
        /// </summary>
        public List<Buff> Buffs => buffs;
        
        /// <summary>
        /// A list of buffs that are applied to the unit.
        /// </summary>
        protected List<Buff> buffs = new List<Buff>();
        
        /// <summary>
        /// Public Method to Add a new Buff to the Unit
        /// </summary>
        /// <param name="_buff"></param>
        public void ApplyBuff(Buff _buff)
        {
            bool applied = false;
            
            for (int i = 0; i < buffs.Count; i++)
            {
                if (buffs[i].Effect == _buff.Effect)
                {
                    buffs[i].Undo(this);
                    buffs[i] += _buff;
                    buffs[i].Apply(this);
                    applied = true;
                    break;
                }
            }

            if (!applied)
            {
                buffs.Add(_buff);
                _buff.Apply(this);
            }
        }

    #endregion

    #region IMovable

        public override void Move(Cell destinationCell, List<Cell> path)
        {
            int cost = Movable.Move(this, destinationCell, path).Count;
            if (BattleStateManager.instance.PlayingUnit == this)
                BattleStats.MP -= cost;
        }
        /// <summary>
        /// Dictionary of all Cells and best Path to go to this Cells
        /// </summary>
        protected Dictionary<Cell, List<Cell>> cachedPaths = null;

        /// <summary>
        /// Determines speed of movement animation.
        /// </summary>
        public float movementAnimationSpeed;
        public override float MovementAnimationSpeed => movementAnimationSpeed;
        
        
        private static DijkstraPathfinding pathfinder = new DijkstraPathfinding();
        private static Pathfinding.Algorithms.Pathfinding fallbackPathfinder = new AStarPathfinding();


        public override void AutoSortOrder()
        {
            unitSprite.sortingOrder = 100 - (int)transform.position.y;
        }

        /// <summary>
        /// Coroutine that play the Movement Animation on the Grid
        /// </summary>
        /// <param name="path">List of Cells in Order of the path to take</param>
        /// <returns></returns>
        public override IEnumerator MovementAnimation(List<Cell> path)
        {
            IsMoving = true;
            Color[] _colors = getColors();
            path.Reverse();
            foreach (Cell _cell in path)
            {
                MarkAsMoving();
                Vector3 _destinationPos = new Vector3(_cell.transform.localPosition.x, _cell.transform.localPosition.y,
                    transform.localPosition.z);
                while (transform.localPosition != _destinationPos)
                {
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, _destinationPos,
                        Time.deltaTime * movementAnimationSpeed);
                    yield return 0;
                }
            }

            IsMoving = false;
            MarkBack(_colors);
            OnMoveFinished();
        }
        
        /// <summary>
        /// Method called after movement animation has finished.
        /// </summary>
        protected virtual void OnMoveFinished()
        {
            onUnitMoved.Raise(this);
        }

        ///<summary>
        /// Method indicates if unit is capable of moving to cell given as parameter.
        /// </summary>
        public virtual bool IsCellMovableTo(Cell _cell)
        {
            return _cell.isWalkable;
        }
        /// <summary>
        /// Method indicates if unit is capable of moving through cell given as parameter.
        /// </summary>
        public virtual bool IsCellTraversable(Cell _cell)
        {
            return _cell.isWalkable;
        }
        
        /// <summary>
        /// Method returns all cells that the unit is capable of moving to.
        /// </summary>
        public HashSet<Cell> GetAvailableDestinations(List<Cell> cells)
        {
            cachedPaths = new Dictionary<Cell, List<Cell>>();

            Dictionary<Cell, List<Cell>> _paths = CachePaths(cells);
            foreach (Cell _key in _paths.Keys)
            {
                if (!IsCellMovableTo(_key))
                {
                    continue;
                }
                List<Cell> _path = _paths[_key];

                float _pathCost = _path.Sum(c => c.movementCost);
                if (_pathCost <= BattleStats.MP)
                {
                    cachedPaths.Add(_key, _path);
                }
            }
            return new HashSet<Cell>(cachedPaths.Keys);
        }

        private Dictionary<Cell, List<Cell>> CachePaths(List<Cell> cells)
        {
            Dictionary<Cell, Dictionary<Cell, float>> _edges = GetGraphEdges(cells);
            Dictionary<Cell, List<Cell>> _paths = pathfinder.FindAllPaths(_edges, Cell);
            return _paths;
        }

        public List<Cell> FindPath(List<Cell> cells, Cell destination)
        {
            if (cachedPaths != null && cachedPaths.ContainsKey(destination))
            {
                return cachedPaths[destination];
            }
            else
            {
                return fallbackPathfinder.FindPath(GetGraphEdges(cells), Cell, destination);
            }
        }
        
        public List<Cell> FindPathFrom(List<Cell> cells, Cell origine, Cell destination)
        {
            if (cachedPaths != null && cachedPaths.ContainsKey(destination))
            {
                return cachedPaths[destination];
            }
            else
            {
                return fallbackPathfinder.FindPath(GetGraphEdges(cells), origine, destination);
            }
        }
        
        /// <summary>
        /// Method returns graph representation of cell grid for pathfinding.
        /// </summary>
        protected virtual Dictionary<Cell, Dictionary<Cell, float>> GetGraphEdges(List<Cell> cells)
        {
            Dictionary<Cell, Dictionary<Cell, float>> _ret = new Dictionary<Cell, Dictionary<Cell, float>>();
            foreach (Cell _cell in cells)
            {
                if (IsCellTraversable(_cell) || _cell.Equals(Cell))
                {
                    _ret[_cell] = new Dictionary<Cell, float>();
                    foreach (Cell _neighbour in _cell.GetNeighbours(cells).FindAll(IsCellTraversable))
                    {
                        _ret[_cell][_neighbour] = _neighbour.movementCost;
                    }
                }
            }
            return _ret;
        }

        #endregion
    
    #region Fight Attack / Defence
        /// <summary>
        /// Method indicates if it is possible to attack a unit from given cell.
        /// </summary>
        /// <param name="other">Unit to attack</param>
        /// <param name="sourceCell">Cell to perform an attack from</param>
        /// <returns>Boolean value whether unit can be attacked or not</returns>
        public virtual bool IsUnitAttackable(Unit other, Cell sourceCell)
        {
            return sourceCell.GetDistance(other.Cell) <= BattleStats.Range.RangeValue
                && other.playerNumber != playerNumber
                && BattleStats.AP >= 1;
        }

        /// <summary>
        /// Method performs an attack on given unit.
        /// </summary>
        public void AttackHandler(Unit _unitToAttack)
        {
            if (!IsUnitAttackable(_unitToAttack, Cell))
            {
                return;
            }

            AttackAction _attackAction = DealDamage(_unitToAttack);
            MarkAsAttacking(_unitToAttack);
            _unitToAttack.DefendHandler(this, _attackAction.Damage, Element.None());
            AttackActionPerformed(_attackAction.ActionCost);
        }
        
        /// <summary>
        /// Method for calculating damage and action points cost of attacking given unit
        /// </summary>
        /// <returns></returns>
        protected virtual AttackAction DealDamage(Unit _unitToAttack)
        {
            return new AttackAction(BattleStats.Power.Basic, 1f);
        }
        
        /// <summary>
        /// Method called after unit performed an attack.
        /// </summary>
        /// <param name="actionCost">Action point cost of performed attack</param>
        protected virtual void AttackActionPerformed(float actionCost)
        {
            BattleStats.AP -= actionCost;
        }

        /// <summary>
        /// Handler method for defending against an attack.
        /// </summary>
        /// <param name="aggressor">Unit that performed the attack</param>
        /// <param name="damage">Amount of damge that the attack caused</param>
        /// <param name="element">Element Type of the attack</param>
        public void DefendHandler(Unit aggressor, float damage, Element element)
        {
            Debug.Log($"Damage : {aggressor.UnitName} did {damage} {element.Type} damage to {UnitName}");
            int _damageTaken = Defend(aggressor, damage, element);
            
            if (BattleStats.Shield > 0)
            {
                if (BattleStats.Shield < _damageTaken)
                {
                    _damageTaken -= BattleStats.Shield;
                    BattleStats.Shield = 0;
                }
                else
                {
                    BattleStats.Shield -= _damageTaken;
                    _damageTaken = 0;
                }
            }
                
            BattleStats.HP -= _damageTaken;
            if (BattleStats.HP > total.HP) 
                BattleStats.HP = total.HP;
            
            if(_damageTaken > 0)
                MarkAsDefending(aggressor);
            if(_damageTaken != 0)
                OnHit(aggressor, _damageTaken, element);
            
            DefenceActionPerformed();

            UnitAttacked?.Invoke(this, new AttackEventArgs(aggressor, this, (int)damage));
            if (BattleStats.HP > 0) return;
            StartCoroutine(OnDestroyed());
        }
        
        /// <summary>
        /// Method called after unit performed defence.
        /// </summary>
        protected void DefenceActionPerformed() { }
        
        /// <summary>
        /// Method for calculating actual damage taken by the unit.
        /// </summary>
        /// <param name="aggressor">Unit that performed the attack</param>
        /// <param name="damage">Amount of damage that the attack caused</param>
        /// <param name="element">Element of the damage</param>
        /// <returns>Amount of damage that the unit has taken</returns>        
        protected int Defend(Unit aggressor, float damage, Element element)
        {

            if (BattleStats.Dodge >= 1)
            {
                BattleStats.Dodge -= 1;
                return 0;
            }

            return (int) (damage * (BattleStats.GetDamageTaken(element.Type) / 100f));
        }

        public int DamageTaken(Unit aggressor, float damage, Element element)
        {
            return Defend(aggressor, damage, element);
        }
        
    #endregion

    #region Mark as / Color Change

        public ColorSet colorSet;
        [Header("Unity Infos")]
        [SerializeField] private SpriteRenderer frame;
        [SerializeField] private SpriteRenderer back;
        [SerializeField] protected SpriteRenderer unitSprite;
        [SerializeField] private TextMeshProUGUI info;
        [SerializeField] private TextMeshProUGUI shadow;
        private Animation anim;
        public Sprite UnitSprite => unitSprite.sprite;
        
        private Dictionary<EColor, Color> Colors = new Dictionary<EColor, Color>();
        public bool isDying = false;

        /// <summary>
        /// Called on Battle Setup to the Set Colors and animation to the Unit's sprite
        /// </summary>
        public void InitializeSprite()
        {
            Colors = colorSet.GetColors();
            AutoSortOrder();
            UnMark();
            anim = gameObject.GetComponent<Animation>();
        }

        /// <summary>
        /// Method to get the ActualMarking
        /// </summary>
        public Color[] getColors()
        {
            Color[] _colorSaved = new Color[3];
            _colorSaved[0] = back.color;
            _colorSaved[1] = frame.color;
            _colorSaved[2] = unitSprite.color;

            return _colorSaved;
        }
        
        /// <summary>
        /// Method to set to the Marking
        /// </summary>
        public void MarkBack(Color[] _colors)
        {
            back.color = _colors[0];
            frame.color = _colors[1];
            unitSprite.color = _colors[2];
        }
        
        /// <summary>
        /// Method marks unit as Moving
        /// </summary>
        protected void MarkAsMoving()
        {
            AutoSortOrder();
            back.color = Colors[EColor.none];
            frame.color = Colors[EColor.none];
            unitSprite.color = Colors[EColor.elementFull];
        }
        
        /// <summary>
        /// Method marks unit as current players unit.
        /// </summary>
        public void MarkAsFriendly()
        {
            back.color = Colors[EColor.ally] * Colors[EColor.transparency];
            frame.color = Colors[EColor.none];
            unitSprite.color = Colors[EColor.elementShadow];
        }
        
        /// <summary>
        /// Method mark units to indicate user that the unit is in range and can be attacked.
        /// </summary>
        public void MarkAsReachableEnemy()
        {
            back.color = Colors[EColor.enemy] * Colors[EColor.transparency];
            frame.color = Colors[EColor.enemy];
            unitSprite.color = Colors[EColor.elementShadow];
        }
        
        /// <summary>
        /// Method marks unit as currently selected, to distinguish it from other units.
        /// </summary>
        public void MarkAsSelected()
        {
            back.color = Colors[EColor.ally] * Colors[EColor.transparency];
            frame.color = Colors[EColor.ally];
            unitSprite.color = Colors[EColor.elementFull];
        }
        
        /// <summary>
        /// Method marks unit to indicate user that he can't do anything more with it this turn.
        /// </summary>
        public void MarkAsFinished()
        {
            UnMark();
        }
        
        /// <summary>
        /// Method returns the unit to its base appearance
        /// </summary>
        public void UnMark()
        {
            back.color = Colors[EColor.none];
            frame.color = Colors[EColor.none];
            unitSprite.color = Colors[EColor.elementShadow];
        }
        
        /// <summary>
        /// Method to Show to the player what happened and how much damage was done
        /// </summary>
        protected void OnHit(Unit aggressor, int damage, Element element)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(element.TextColour);
            info.text = damage > 0 ? $"- <color=#{_hexColor}>{damage}</color> HP" : $"+ {-damage} HP";
            shadow.text = damage > 0 ? $"- {damage} HP" : $"+ {-damage} HP";
            anim.PlayQueued("TextFade");
        }

        /// <summary>
        /// Gives visual indication that the unit is under attack.
        /// </summary>
        /// <param name="aggressor">
        /// Unit that is attacking.
        /// </param>
        public void MarkAsDefending(Unit aggressor)
        {
            anim.PlayQueued("Hit");
        }
        
        /// <summary>
        /// Gives visual indication that the unit is attacking.
        /// </summary>
        /// <param name="target">
        /// Unit that is under attack.
        /// </param>
        public void MarkAsAttacking(Unit target)
        {
            anim.PlayQueued("Attack");
        }
        
        /// <summary>
        /// Gives visual indication that the unit is destroyed. It gets called right before the unit game object is
        /// destroyed.
        /// </summary>
        public void MarkAsDestroyed()
        {
            anim.PlayQueued("Death");
        }

    #endregion

        /// <summary>
        /// Method called after object instantiation to initialize fields etc. 
        /// </summary>
        public virtual void Initialize()
        {
            buffs = new List<Buff>();

            UnitState = new UnitStateNormal(this);

            Inventory ??= new Inventory();
        }

        public void OnDestroy()
        {
            if (Cell != null)
            {
                Cell.FreeTheCell();
            }
        }

        /// <summary>
        /// Method is called at the start of each turn.
        /// </summary>
        public virtual void OnTurnStart()
        {
            BattleStats.MP = total.MP;
            BattleStats.AP = total.AP;

            SetState(new UnitStateMarkedAsFriendly(this));
        }
        
        /// <summary>
        /// Method is called at the end of each turn.
        /// </summary>
        public void OnTurnEnd()
        {
            cachedPaths = null;
            buffs.ForEach(b =>
            {
                b.Duration -= 1;
                b.OnEndTurn(this);
            });
            
            List<Buff> _buffs = buffs.Where(_buff => _buff.Duration <= 0).ToList();
            _buffs.ForEach(_buff =>
            {
                _buff.Undo(this);
                buffs.Remove(_buff);
            });
            
            Cell.Buffs.ForEach( b => b.OnEndTurn(this));
            
            SetState(new UnitStateNormal(this));
        }
        
        /// <summary>
        /// Method is called when units HP drops below 1.
        /// </summary>
        public override IEnumerator OnDestroyed()
        {
            isDying = true;
            Cell.isTaken = false;
            MarkAsDestroyed();
            yield return new WaitWhile(IsAnimPlaying);
            UnitDestroyed?.Invoke(this, new DeathEventArgs(this));
            isDying = false;
            yield return new WaitForSeconds(0.1f);
            Destroy(gameObject);
        }

        /// <summary>
        /// return true if the Unit is playing any animation
        /// </summary>
        /// <returns></returns>
        protected bool IsAnimPlaying() => anim.isPlaying;

        /// <summary>
        /// Method is called when unit is selected.
        /// </summary>
        public void OnUnitSelected()
        {
            SetState(new UnitStateMarkedAsSelected(this));
            if (UnitSelected != null)
            {
                UnitSelected.Invoke(this, new EventArgs());
            }
        }
        
        /// <summary>
        /// Method is called when unit is deselected.
        /// </summary>
        public void OnUnitDeselected()
        {
            SetState(new UnitStateMarkedAsFriendly(this));
            if (UnitDeselected != null)
            {
                UnitDeselected.Invoke(this, new EventArgs());
            }
        }

        public virtual void UpdateStats()
        {
            buffs.ForEach(buff => buff.Undo(this));
            
            BattleStats newTotal = baseStats + Inventory.GearStats();
            BattleStats diff = newTotal - Total;
            BattleStats actual = new BattleStats(newTotal)
            {
                Shield = BattleStats.Shield + Math.Max(0, diff.Shield),
                Dodge = BattleStats.Dodge + Math.Max(0, diff.Dodge),
                HP = BattleStats.HP + Math.Max(0, diff.HP),
                AP = BattleStats.AP + Math.Max(0, diff.AP),
                MP = BattleStats.MP + Math.Max(0, diff.MP),
                TurnPoint = BattleStats.TurnPoint,
            };

            BattleStats = new BattleStats(actual);
            total = new BattleStats(newTotal);
            
            buffs.ForEach(buff => buff.Apply(this));
        }
    }
    public class AttackAction
    {
        public readonly int Damage;
        public readonly float ActionCost;

        public AttackAction(int damage, float actionCost)
        {
            Damage = damage;
            ActionCost = actionCost;
        }
    }

    public class MovementEventArgs : EventArgs
    {
        public Cell OriginCell;
        public Cell DestinationCell;
        public List<Cell> Path;

        public MovementEventArgs(Cell sourceCell, Cell destinationCell, List<Cell> path)
        {
            OriginCell = sourceCell;
            DestinationCell = destinationCell;
            Path = path;
        }
    }

    public class AttackEventArgs : EventArgs
    {
        public Unit Attacker;
        public Unit Defender;

        public int Damage;

        public AttackEventArgs(Unit attacker, Unit defender, int damage)
        {
            Attacker = attacker;
            Defender = defender;

            Damage = damage;
        }
    }

    public class DeathEventArgs : EventArgs
    {
        public Unit deadUnit;

        public DeathEventArgs(Unit _deadUnit)
        {
            deadUnit = _deadUnit;
        }
    }
    
    public class UnitCreatedEventArgs : EventArgs
    {
        public Transform Unit;

        public UnitCreatedEventArgs(Transform unit)
        {
            this.Unit = unit;
        }
    }
}
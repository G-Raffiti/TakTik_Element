using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using _ScriptableObject;
using Buffs;
using Cells;
using DataBases;
using Gears;
using Relics;
using Resources.ToolTip.Scripts;
using StateMachine;
using Stats;
using TMPro;
using UnityEngine;
using UserInterface;

namespace Units
{
    /// <summary>
    /// Base class for all units in the game.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class Unit : IMovable, IInfo
    {
        ////////////////////// Unity References ////////////////////////////////////////////////////////////////////////
        [Header("Unity References")]
        [SerializeField] private ColorSet colorSet;
        [SerializeField] protected SpriteRenderer unitSpriteRenderer;
        [SerializeField] private TextMeshProUGUI info;
        public Sprite UnitSprite => unitSpriteRenderer.sprite;
        public override SpriteRenderer MovableSprite => unitSpriteRenderer;
        private Animation anim;
        private LifeBar lifeBar;
        [HideInInspector] public string UnitName;
        public override string getName => UnitName;
        
        
        //////////////////// Events ////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event invoked when the Unit Take Damage or is Healed
        /// </summary>
        public event EventHandler<AttackEventArgs> UnitAttacked;
        /// <summary>
        /// UnitDestroyed event is invoked when unit's BattleStats.HP drop below 0.
        /// </summary>
        public event EventHandler<DeathEventArgs> UnitDestroyed;
        
        [Header("Events Sender")]
        [SerializeField] private UnitEvent onUnitTooltip_ON;
        [SerializeField] private UnitEvent onUnitMoved;
        
        
        /////////////////// Unit Stats /////////////////////////////////////////////////////////////////////////////////
        public abstract Relic Relic { get; }
        
        /// <summary>
        /// it represent the initiative of the unit, this point are used to take a round
        /// </summary>
        [Header("Unit's Stats")]
        public int TurnPoint;
        
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

        /// <summary>
        /// A list of buffs that are applied to the unit.
        /// </summary>
        public List<Buff> Buffs => buffs;
        
        /// <summary>
        /// A list of buffs that are applied to the unit.
        /// </summary>
        protected List<Buff> buffs = new List<Buff>();
        public bool isDying { get; set; }

        
        //////////////////// IMovable //////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Dictionary of all Cells and best Path to go to this Cells
        /// </summary>
        public Dictionary<Cell, List<Cell>> cachedPaths = null;
        
        
        /////////////////// On Mouse Actions ///////////////////////////////////////////////////////////////////////////
        public virtual void OnPointerEnter()
        {
            lifeBar.Show();
        }

        public virtual void OnPointerExit()
        {
            lifeBar.Hide();
        }
        public virtual void OnLeftClick()
        {
        }
        public virtual void OnRightClick()
        {
            onUnitTooltip_ON.Raise(this);
        }


        //////////////////// Buffs /////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Public Method to Add a new Buff to the Unit
        /// </summary>
        /// <param name="_buff"></param>
        public void ApplyBuff(Buff _buff)
        {
            bool applied = false;
            Buff buff = new Buff(_buff);
            buff.onFloor = false;
            
            for (int i = 0; i < buffs.Count; i++)
            {
                if (buffs[i].Effect == buff.Effect)
                {
                    buffs[i].Undo(this);
                    buffs[i] += buff;
                    buffs[i].Apply(this);
                    applied = true;
                    break;
                }
            }

            if (!applied)
            {
                buffs.Add(buff);
                _buff.Apply(this);
            }
        }

        public void RemoveBuff(StatusSO effect)
        {
            if (Buffs.Any(b => b.Effect == effect))
                Buffs.Remove(Buffs.Find(b => b.Effect == effect));
        }
        
        
        /////////////////////// I Movable Overrides ////////////////////////////////////////////////////////////////////
        public override List<Cell> Move(Cell destinationCell, List<Cell> path)
        {
            List<Cell> _path = Movement.Move(this, destinationCell, path);
            int cost = _path.Count;
            if (BattleStateManager.instance.PlayingUnit == this)
                BattleStats.MP -= cost;
            return _path;
        }

        public override IEnumerator Fall(Cell _destination)
        {
            Inventory = new Inventory();
            while (IsMoving)
                yield return null;
            _destination.FallIn();
            if (isDying) yield break;
            StartCoroutine(OnDestroyed());
        }

        /// <summary>
        /// Coroutine that play the Movement Animation on the Grid
        /// </summary>
        /// <param name="path">List of Cells in Order of the path to take</param>
        /// <returns></returns>
        public override IEnumerator MovementAnimation(List<Cell> path)
        {
            lifeBar.HideForced();
            TileIsometric.CellState _state = ((TileIsometric) Cell).State;
            yield return base.MovementAnimation(path);
            MarkBack(_state);
            OnMoveFinished(path.Count);
        }
        
        /// <summary>
        /// Method called after movement animation has finished.
        /// </summary>
        protected virtual void OnMoveFinished(int _cellWalked)
        {
            onUnitMoved.Raise(this);
        }

        ///<summary>
        /// Method indicates if unit is capable of moving to cell given as parameter.
        /// </summary>
        protected virtual bool IsCellMovableTo(Cell _cell)
        {
            return _cell.IsWalkable;
        }
        /// <summary>
        /// Method indicates if unit is capable of moving through cell given as parameter.
        /// </summary>
        protected virtual bool IsCellTraversable(Cell _cell)
        {
            return _cell.IsWalkable;
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

                float _pathCost = _path.Sum(c => c.MovementCost);
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
            if (destination == Cell)
                return new List<Cell>() {Cell};
            if (cachedPaths != null && cachedPaths.ContainsKey(destination))
            {
                return cachedPaths[destination];
            }
            else
            {
                return fallbackPathfinder.FindPath(GetGraphEdges(cells), Cell, destination);
            }
        }
        
        public List<Cell> FindPathFrom(List<Cell> cells, Cell origin, Cell destination)
        {
            if (cachedPaths != null && cachedPaths.ContainsKey(destination))
            {
                return cachedPaths[destination];
            }
            else
            {
                return fallbackPathfinder.FindPath(GetGraphEdges(cells), origin, destination);
            }
        }
        
        /// <summary>
        /// Method returns graph representation of cell grid for pathfinding.
        /// </summary>
        private Dictionary<Cell, Dictionary<Cell, float>> GetGraphEdges(List<Cell> cells)
        {
            Dictionary<Cell, Dictionary<Cell, float>> _ret = new Dictionary<Cell, Dictionary<Cell, float>>();
            foreach (Cell _cell in cells)
            {
                if (IsCellTraversable(_cell) || _cell.Equals(Cell))
                {
                    _ret[_cell] = new Dictionary<Cell, float>();
                    foreach (Cell _neighbour in _cell.GetNeighbours(cells).FindAll(IsCellTraversable))
                    {
                        _ret[_cell][_neighbour] = _neighbour.MovementCost;
                    }
                }
            }
            return _ret;
        }

        public override void AutoSortOrder()
        {
            base.AutoSortOrder();
            lifeBar.AutoSortOrder();
        }
        
        
        ///////////////////// Fight / Damage Handler / Defence /////////////////////////////////////////////////////////
        /// <summary>
        /// Handler method for defending against an attack.
        /// </summary>
        /// <param name="aggressor">Unit that performed the attack</param>
        /// <param name="damage">Amount of damge that the attack caused</param>
        /// <param name="element">Element Type of the attack</param>
        public void DefendHandler(Unit aggressor, float damage, Element element)
        {
            if (isDying) return;
            if (BattleStats.HP <= 0)
            {
                StartCoroutine(OnDestroyed());
                return;
            }

            int _damageTaken = 0;
            if (damage > 0)
                _damageTaken = DamageTaken(damage, element);
            else _damageTaken = BattleStats.GetHealTaken(damage, element.Type);

            OnHit(_damageTaken, element);
            Debug.Log($"Damage : {aggressor.ColouredName()} did {_damageTaken} {element.Name} damage to {ColouredName()} on {Cell.OffsetCoord}");
            
            if (_damageTaken > 0)
            {
                MarkAsDefending(aggressor);
                
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
            }

            BattleStats.HP -= _damageTaken;
            if (BattleStats.HP > total.HP) 
                BattleStats.HP = total.HP;

            UnitAttacked?.Invoke(this, new AttackEventArgs(aggressor, this, _damageTaken));
            if (BattleStats.HP > 0) return;
            
            if (isDying) return;
            StartCoroutine(OnDestroyed());
        }

        /// <summary>
        /// get the Damage Mitigation from Affinity
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public int DamageTaken(float damage, Element element)
        {
            return (int) BattleStats.GetDamageTaken(damage, element.Type);
        }
        
        /// <summary>
        /// Method is called when units HP drops below 1.
        /// </summary>
        public override IEnumerator OnDestroyed()
        {
            isDying = true;
            BattleStats.HP = 0;
            Cell.FreeTheCell();
            MarkAsDestroyed();
            yield return new WaitWhile(() => anim.isPlaying);
            UnitDestroyed?.Invoke(this, new DeathEventArgs(this));
            isDying = false;
            yield return new WaitForSeconds(0.1f);
            if (Cell != null)
            {
                Cell.FreeTheCell();
            }
            Destroy(gameObject);
        }
        

        ////////////////////// Mark As (Color Change and Cells UI interactions) ////////////////////////////////////////
        /// <summary>
        /// Called on Battle Setup to the Set Colors and animation to the Unit's sprite
        /// </summary>
        public void InitializeSprite()
        {
            anim = gameObject.GetComponent<Animation>();
            lifeBar = GetComponent<LifeBar>();
            AutoSortOrder();
            lifeBar.Initialize();
            UnMark();
        }

        /// <summary>
        /// Method to set to the Marking
        /// </summary>
        public void MarkBack(TileIsometric.CellState state)
        {
            ((TileIsometric)Cell).MarkAs(state);
        }
        
        /// <summary>
        /// Method marks unit as currently selected, to distinguish it from other units.
        /// </summary>
        public void MarkAsSelected()
        {
            Cell?.MarkAsHighlighted();
        }

        /// <summary>
        /// Method returns the unit to its base appearance
        /// </summary>
        public void UnMark()
        {
            Cell?.UnMark();
        }
        
        /// <summary>
        /// Method to Show to the player what happened and how much damage was done
        /// </summary>
        private void OnHit(int damage, Element element)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(element.TextColour);
            if (damage == 0) return;
            if (damage > 0)
            {
                info.text = $"- <color=#{_hexColor}>{damage}</color> HP";
            }
            else
            {
                info.text = $"+ {-damage} HP";
            }
            
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
            anim.Play("Hit");
        }

        /// <summary>
        /// Gives visual indication that the unit is destroyed. It gets called right before the unit game object is
        /// destroyed.
        /// </summary>
        public void MarkAsDestroyed()
        {
            anim.PlayQueued("Death");
        }
    
        ////////////////////////// Initialisation & Start/End Turn Methods /////////////////////////////////////////////
        /// <summary>
        /// Method called after object instantiation to initialize fields etc. 
        /// </summary>
        public virtual void Initialize()
        {
            buffs = new List<Buff>();

            Inventory ??= new Inventory();
        }

        /// <summary>
        /// Method is called at the start of THIS unit's Turn
        /// </summary>
        public virtual void StartTurn()
        {
            BattleStats.MP = total.MP;
            BattleStats.AP = total.AP;
        }
        
        /// <summary>
        /// Method is called at the End of THIS unit's Turn
        /// </summary>
        public virtual void EndTurn()
        {
            cachedPaths = null;
        }
        
        /// <summary>
        /// Method Called at the End of ANY OTHER unit's Turn
        /// </summary>
        public void OnTurnEnds()
        {
            buffs.ForEach(b =>
            {
                b.OnEndTurn(this);
            });
            
            List<Buff> _buffs = buffs.Where(_buff => _buff.Duration <= 0).ToList();
            _buffs.ForEach(_buff =>
            {
                _buff.Undo(this);
                buffs.Remove(_buff);
            });
        }

        /// <summary>
        /// Method Called at the Start of EVERY unit's Turn
        /// </summary>
        public void OnTurnStarts()
        {
        }
        
        
        
        ///////////////////////////////// Stats Modifications //////////////////////////////////////////////////////////
        public virtual void UpdateStats()
        {
            buffs.ForEach(buff => buff.Undo(this));
            
            BattleStats newTotal = baseStats + Inventory.GearStats();
            BattleStats diff = newTotal - Total;
            BattleStats actual = new BattleStats(newTotal)
            {
                Shield = BattleStats.Shield + Math.Max(0, diff.Shield),
                HP = BattleStats.HP + Math.Max(0, diff.HP),
                AP = BattleStats.AP + Math.Max(0, diff.AP),
                MP = BattleStats.MP + Math.Max(0, diff.MP),
            };

            BattleStats = new BattleStats(actual);
            total = new BattleStats(newTotal);
            
            buffs.ForEach(buff => buff.Apply(this));
        }
        
        
        ////////////////////////////////// IInfo Overrides / Tooltip ///////////////////////////////////////////////////
        public string GetInfoMain()
        {
            string str = "";
            str += ColouredName();
            if (playerNumber == 0)
            {
                str +=  "\nHero" + "\n";
            }

            else str +=  "\nMonster" + "\n";

            return str;
        }

        public string GetInfoLeft()
        {
            string str = "";
            str += $"<sprite name=AP> <color={colorSet.HexColor(EAffix.AP)}>{(int)Total.AP}</color>    ";
            str += $"<sprite name=MP> <color={colorSet.HexColor(EAffix.MP)}>{(int)Total.MP}</color> \n";
            str += $"<sprite name=HP> <color={colorSet.HexColor(EAffix.HP)}>{BattleStats.HP} </color>/ {Total.HP}    ";
            str += $"<sprite name=Shield> <color={colorSet.HexColor(EAffix.Shield)}>{BattleStats.Shield}</color> \n";
            str += $"<sprite name=Fire> <color={colorSet.HexColor(EAffix.Fire)}>{BattleStats.GetPower(EElement.Fire)}</color>  <sprite name=Water> <color={colorSet.HexColor(EAffix.Water)}>{BattleStats.GetPower(EElement.Water)}</color>  <sprite name=Nature> <color={colorSet.HexColor(EAffix.Nature)}>{BattleStats.GetPower(EElement.Nature)}</color> \n";
            str += $"<sprite name=Speed> <color={colorSet.HexColor(EAffix.Speed)}>{BattleStats.Speed} </color>  ";
            str += $"<sprite name=Focus> <color={colorSet.HexColor(EAffix.Focus)}>{BattleStats.Focus} </color> \n";

            return str;
        }

        public string GetInfoRight()
        {
            string str = "";
            str += BattleStats.Range.ToString(this)+ "\n";
            str += $"<sprite name=TP> <color={colorSet.HexColor(EColor.TurnPoint)}>{TurnPoint} </color> \n";
            return str;
        }

        public virtual string GetInfoDown()
        {
            return Buffs.Aggregate("", (_current, _buff) => _current + (_buff.InfoOnUnit(_buff, this) + "\n"));
        }

        public string ColouredName()
        {
            string hexColour;
            if (playerNumber == 0)
                hexColour = colorSet.HexColor(EColor.ally);
            else 
                hexColour = colorSet.HexColor(EColor.enemy);
            return $"<color={hexColour}>{UnitName}</color>";
        }

        public Sprite GetIcon()
        {
            return UnitSprite;
        }

        public Color GetTeamColor()
        {
            return playerNumber == 0 ? colorSet.GetColors()[EColor.ally] : colorSet.GetColors()[EColor.enemy];
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
}
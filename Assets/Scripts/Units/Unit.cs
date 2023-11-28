using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using _Extension;
using _Instances;
using _LeanTween.Framework;
using _ScriptableObject;
using Buffs;
using Cells;
using DataBases;
using Gears;
using Players;
using Relics;
using Resources.ToolTip.Scripts;
using StateMachine;
using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UserInterface;

namespace Units
{
    /// <summary>
    /// class <c>Unit</c> represents all units in the game.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class Unit : Movable, IInfo
    {
        ////////////////////// Unity References ////////////////////////////////////////////////////////////////////////
        
        [Header("Unity References")]
        [SerializeField] private ColorSet colorSet;
        [SerializeField] protected SpriteRenderer unitSpriteRenderer;
        [SerializeField] private TextMeshProUGUI info;
        public abstract Sprite UnitSprite { get; }
        public override SpriteRenderer MovableSprite => unitSpriteRenderer;
        private LifeBar lifeBar;
        [FormerlySerializedAs("UnitName")]
        [HideInInspector] public string unitName;
        public override string GetName => unitName;
        
        
        //////////////////// Events ////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event invoked when the Unit Take Damage or is Healed
        /// </summary>
        public event EventHandler<AttackEventArgs> UnitAttacked;
        /// <summary>
        /// UnitDestroyed event is invoked when unit's BattleStats.HP drop below 0.
        /// </summary>
        public event EventHandler<DeathEventArgs> UnitDestroyed;
        
        [FormerlySerializedAs("onUnitTooltip_ON")]
        [Header("Events Sender")]
        [SerializeField] private UnitEvent onUnitTooltipOn;
        [SerializeField] private UnitEvent onUnitMoved;
        
        
        /////////////////// Unit Stats /////////////////////////////////////////////////////////////////////////////////
        public abstract Relic Relic { get; }
        
        /// <summary>
        /// it represent the initiative of the unit, this point are used to take a round
        /// </summary>
        [FormerlySerializedAs("TurnPoint")]
        [Header("Unit's Stats")]
        public int turnPoint;
        
        /// <summary>
        /// Actual Stats of the Unit, Can be modified from all Sources,
        /// </summary>
        [FormerlySerializedAs("BattleStats")]
        public BattleStats battleStats;
        
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
        public EPlayerType playerType;

        /// <summary>
        /// A list of buffs that are applied to the unit.
        /// </summary>
        public List<Buff> Buffs => buffs;
        
        /// <summary>
        /// A list of buffs that are applied to the unit.
        /// </summary>
        protected List<Buff> buffs = new List<Buff>();
        public bool IsDying { get; set; }
        public EElement Main { get; private set; }
        public EElement Weakness { get; private set; }
        public EElement Resist { get; private set; }

        
        //////////////////// IMovable //////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Dictionary of all Cells and best Path to go to this Cells
        /// </summary>
        public Dictionary<Cell, List<Cell>> cachedPaths = null;
        
        
        /////////////////// On Mouse Actions ///////////////////////////////////////////////////////////////////////////
        public virtual void OnPointerEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
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
            OnTooltipOn.Raise(this);
        }


        //////////////////// Buffs /////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Public Method to Add a new Buff to the Unit
        /// </summary>
        /// <param name="_buff"></param>
        public void ApplyBuff(Buff _buff)
        {
            bool _applied = false;
            Buff _newBuff = new Buff(_buff);
            _newBuff.onFloor = false;
            
            for (int _i = 0; _i < buffs.Count; _i++)
            {
                if (buffs[_i].Effect == _newBuff.Effect)
                {
                    buffs[_i].Undo(this);
                    buffs[_i] += _newBuff;
                    buffs[_i].Apply(this);
                    _applied = true;
                    break;
                }
            }

            if (!_applied)
            {
                buffs.Add(_newBuff);
                _newBuff.Apply(this);
            }
        }

        public void RemoveBuff(StatusSo _effect)
        {
            if (Buffs.Any(_b => _b.Effect == _effect))
                Buffs.Remove(Buffs.Find(_b => _b.Effect == _effect));
        }
        
        
        /////////////////////// I Movable Overrides ////////////////////////////////////////////////////////////////////
        #region IMovable
        public override List<Cell> Move(Cell _destinationCell, List<Cell> _completePath)
        {
            List<Cell> _path = Movement.Move(this, _destinationCell, _completePath);
            int _cost = _path.Count;
            if (BattleStateManager.instance.PlayingUnit == this)
                battleStats.mp -= _cost;
            return _path;
        }

        public override IEnumerator Fall(Cell _destination)
        {
            inventory = new Inventory();
            while (IsMoving)
                yield return null;
            _destination.FallIn();
            if (IsDying) yield break;
            StartCoroutine(OnDestroyed());
        }

        /// <summary>
        /// Coroutine that play the Movement Animation on the Grid
        /// </summary>
        /// <param name="path">List of Cells in Order of the path to take</param>
        /// <returns></returns>
        public override IEnumerator MovementAnimation(List<Cell> _path)
        {
            lifeBar.HideForced();
            CellState _state = Cell.State;
            yield return base.MovementAnimation(_path);
            MarkBack(_state);
            OnMoveFinished(_path.Count);
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
        public HashSet<Cell> GetAvailableDestinations(List<Cell> _cells)
        {
            cachedPaths = new Dictionary<Cell, List<Cell>>();

            Dictionary<Cell, List<Cell>> _paths = CachePaths(_cells);
            foreach (Cell _key in _paths.Keys)
            {
                if (!IsCellMovableTo(_key))
                {
                    continue;
                }
                List<Cell> _path = _paths[_key];

                float _pathCost = _path.Sum(_c => _c.movementCost);
                if (_pathCost <= battleStats.mp)
                {
                    cachedPaths.Add(_key, _path);
                }
            }
            return new HashSet<Cell>(cachedPaths.Keys);
        }

        private Dictionary<Cell, List<Cell>> CachePaths(List<Cell> _cells)
        {
            Dictionary<Cell, Dictionary<Cell, float>> _edges = GetGraphEdges(_cells);
            Dictionary<Cell, List<Cell>> _paths = pathfinder.FindAllPaths(_edges, Cell);
            return _paths;
        }

        public List<Cell> FindPath(List<Cell> _cells, Cell _destination)
        {
            if (_destination == Cell)
                return new List<Cell>() {Cell};
            if (cachedPaths != null && cachedPaths.ContainsKey(_destination))
            {
                return cachedPaths[_destination];
            }
            else
            {
                return fallbackPathfinder.FindPath(GetGraphEdges(_cells), Cell, _destination);
            }
        }
        
        public List<Cell> FindPathFrom(List<Cell> _cells, Cell _origin, Cell _destination)
        {
            if (cachedPaths != null && cachedPaths.ContainsKey(_destination))
            {
                return cachedPaths[_destination];
            }
            else
            {
                return fallbackPathfinder.FindPath(GetGraphEdges(_cells), _origin, _destination);
            }
        }
        
        /// <summary>
        /// Method returns graph representation of cell grid for pathfinding.
        /// </summary>
        private Dictionary<Cell, Dictionary<Cell, float>> GetGraphEdges(List<Cell> _cells)
        {
            Dictionary<Cell, Dictionary<Cell, float>> _ret = new Dictionary<Cell, Dictionary<Cell, float>>();
            foreach (Cell _cell in _cells)
            {
                if (IsCellTraversable(_cell) || _cell.Equals(Cell))
                {
                    _ret[_cell] = new Dictionary<Cell, float>();
                    foreach (Cell _neighbour in _cell.GetNeighbours(_cells).FindAll(IsCellTraversable))
                    {
                        _ret[_cell][_neighbour] = _neighbour.movementCost;
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
        #endregion

        ///////////////////// Fight / Damage Handler / Defence /////////////////////////////////////////////////////////
        # region Fight & Damage Handler

        private List<string> damageReccorded = new List<string>();

        public float DamageModifier(Element _dmgElement)
        {
            if (_dmgElement.Type == EElement.None) return 1;
            if (_dmgElement.Type == Main) return 1;
            if (_dmgElement.Type == Resist) return 0.75f;
            return 1.5f;
        }
        
        /// <summary>
        /// Handler method for defending against an attack.
        /// </summary>
        /// <param name="aggressor">Unit that performed the attack</param>
        /// <param name="damage">Amount of damge that the attack caused</param>
        /// <param name="element">Element Type of the attack</param>
        public void DefendHandler(Unit _aggressor, float _damage, Element _element)
        {
            if (IsDying) return;
            if (battleStats.hp <= 0)
            {
                StartCoroutine(OnDestroyed());
                return;
            }

            int _damageTaken = 0;
            if (_damage > 0)
                _damageTaken = DamageTaken(_damage, _element);
            else _damageTaken = battleStats.GetHealTaken(_damage, _element.Type);

            Debug.Log($"Damage : {_aggressor.ColouredName()} did {_damageTaken} {_element.Name} damage to {ColouredName()} on {Cell.OffsetCoord}");

            int _shieldDamage = 0;
            if (_damageTaken > 0)
            {
                if (battleStats.shield > 0)
                {
                    if (battleStats.shield < _damageTaken)
                    {
                        _shieldDamage = battleStats.shield;
                        _damageTaken -= battleStats.shield;
                        battleStats.shield = 0;
                    }
                    else
                    {
                        _shieldDamage = _damageTaken;
                        battleStats.shield -= _damageTaken;
                        _damageTaken = 0;
                    }
                }
            }
            RecordHit(_damageTaken, _shieldDamage, _element);

            battleStats.hp -= _damageTaken;
            if (battleStats.hp > total.hp) 
                battleStats.hp = total.hp;

            UnitAttacked?.Invoke(this, new AttackEventArgs(_aggressor, this, _damageTaken));
            if (battleStats.hp > 0) return;
            
            if (IsDying) return;
            StartCoroutine(OnDestroyed());
        }

        /// <summary>
        /// get the Damage Mitigation from Affinity
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public int DamageTaken(float _damage, Element _element)
        {
            return (int) (_damage * DamageModifier(_element));
        }
        
        /// <summary>
        /// Method is called when units HP drops below 1.
        /// </summary>
        public override IEnumerator OnDestroyed()
        {
            IsDying = true;
            UnitAttacked?.Invoke(this, new AttackEventArgs(this, this, battleStats.hp));
            battleStats.hp = 0;
            Cell.FreeTheCell();
            MarkAsDestroyed();
            yield return new WaitUntil(() => LeanTween.tweensRunning <= 0);
            UnitDestroyed?.Invoke(this, new DeathEventArgs(this));
            IsDying = false;
            yield return new WaitForSeconds(0.1f);
            if (Cell != null)
            {
                Cell.FreeTheCell();
            }
            Destroy(gameObject);
        }
        #endregion

        ////////////////////// Mark As (Color Change and Cells UI interactions) ////////////////////////////////////////
        # region UI & Mark As

        private void Update()
        {
            if (isPlayingAnim) return;
            if (damageReccorded.Count > 0)
            {
                StartCoroutine(MarkAsTakingDamage());
            }
        }

        private bool isPlayingAnim;
        private IEnumerator MarkAsTakingDamage()
        {
            isPlayingAnim = true;
            info.text = damageReccorded[0];
            LeanTween.alphaCanvas(info.GetComponent<CanvasGroup>(), 1, 0.3f);
            LeanTween.moveLocal(info.gameObject, new Vector3(0, 50), 1);
            LeanTween.alphaCanvas(info.GetComponent<CanvasGroup>(), 0, 0.1f).setDelay(0.9f);
            LeanTween.moveLocal(info.gameObject, Vector3.zero, 0).setDelay(1f);
            damageReccorded.RemoveAt(0);
            yield return new WaitUntil(() => LeanTween.tweensRunning <= 0);
            isPlayingAnim = false;
        }
        
        /// <summary>
        /// Called on Battle Setup to the Set Colors to the Unit's sprite
        /// </summary>
        public void InitializeSprite()
        {
            lifeBar = GetComponent<LifeBar>();
            AutoSortOrder();
            lifeBar.Initialize();
            UnMark();
            info.text = "";
        }

        /// <summary>
        /// Method to set to the Marking
        /// </summary>
        public void MarkBack(CellState _state)
        {
            ((TileIsometric)Cell).MarkAs(_state);
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
        private void RecordHit(int _hpDamage, int _shieldDamage, Element _element)
        {
            if (_hpDamage == 0 && _shieldDamage == 0) return;
            string _hexColor = ColorUtility.ToHtmlStringRGB(_element.TextColour);
            string _ret = "";
            if (_hpDamage == 0){}
            else if (_hpDamage > 0)
                _ret += $"  - <color=#{_hexColor}>{_hpDamage}</color> <sprite name=HP>";
            else
                _ret += $"  + {-_hpDamage} <sprite name=HP>";
            
            if (_shieldDamage == 0) {}
            else if (_shieldDamage > 0)
                _ret += $"  - <color=#{_hexColor}>{_shieldDamage}</color> <sprite name=Shield>";
            else
                _ret += $"  + {-_shieldDamage} <sprite name=Shield>";

            damageReccorded.Add(_ret);
        }

        /// <summary>
        /// Gives visual indication that the unit is under attack.
        /// </summary>
        /// <param name="aggressor">
        /// Unit that is attacking.
        /// </param>
        public void MarkAsDefending(Unit _aggressor)
        {
        }

        /// <summary>
        /// Gives visual indication that the unit is destroyed. It gets called right before the unit game object is
        /// destroyed.
        /// </summary>
        public void MarkAsDestroyed()
        {
            LeanTween.alpha(gameObject, 0, 1f);
        }
        #endregion
    
        ////////////////////////// Initialisation & Start/End Turn Methods /////////////////////////////////////////////
        # region Start / End Turn
        /// <summary>
        /// Method called after object instantiation to initialize fields etc. 
        /// </summary>
        public virtual void Initialize()
        {
            buffs = new List<Buff>();

            inventory ??= new Inventory();
        }

        /// <summary>
        /// Method is called at the start of THIS unit's Turn
        /// </summary>
        public virtual void StartTurn()
        {
            battleStats.mp = total.mp;
            battleStats.ap = total.ap;
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
            buffs.ForEach(_b =>
            {
                _b.OnEndTurn(this);
            });
            
            List<Buff> _buffs = buffs.Where(_buff => _buff.duration <= 0).ToList();
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
        #endregion

        ///////////////////////////////// Stats Modifications //////////////////////////////////////////////////////////
        # region Stats
        public virtual void UpdateStats()
        {
            buffs.ForEach(_buff => _buff.Undo(this));
            
            BattleStats _newTotal = baseStats + inventory.GearStats();
            BattleStats _diff = _newTotal - Total;
            BattleStats _actual = new BattleStats(_newTotal)
            {
                shield = battleStats.shield + Math.Max(0, _diff.shield),
                hp = battleStats.hp + Math.Max(0, _diff.hp),
                ap = battleStats.ap + Math.Max(0, _diff.ap),
                mp = battleStats.mp + Math.Max(0, _diff.mp),
            };

            battleStats = new BattleStats(_actual);
            total = new BattleStats(_newTotal);
            
            buffs.ForEach(_buff => _buff.Apply(this));
            MainElement();
        }
        
        /// <summary>
        /// Method to set the Main element and Weakness and Resistance
        /// </summary>
        /// <returns></returns>
        public void MainElement()
        {
            Dictionary<EElement, float> _elements = new Dictionary<EElement, float>()
            {
                {EElement.Fire, battleStats.affinity.fire},
                {EElement.Water, battleStats.affinity.water},
                {EElement.Nature, battleStats.affinity.nature},
            };
            Main = _elements.GetKeyOfMaxValue();

            switch (Main)
            {
                case EElement.Fire: 
                    Resist = EElement.Nature;
                    Weakness = EElement.Water;
                    break;
                case EElement.Nature:
                    Resist = EElement.Water;
                    Weakness = EElement.Fire;
                    break;
                case EElement.Water:
                    Resist = EElement.Fire;
                    Weakness = EElement.Nature;
                    break;
                case EElement.None:
                    Resist = EElement.None;
                    Weakness = EElement.None;
                    break;
            }
        }
        #endregion
        
        ////////////////////////////////// IInfo Overrides / Tooltip ///////////////////////////////////////////////////
        public abstract string GetInfoMain();

        public string GetInfoLeft()
        {
            string _str = "";
            _str += $"<sprite name=AP> <color={colorSet.HexColor(EAffix.AP)}>{(int)Total.ap}</color>    ";
            _str += $"<sprite name=MP> <color={colorSet.HexColor(EAffix.Mp)}>{(int)Total.mp}</color> \n";
            _str += $"<sprite name=HP> <color={colorSet.HexColor(EAffix.Hp)}>{battleStats.hp} </color>/ {Total.hp}    ";
            _str += $"<sprite name=Shield> <color={colorSet.HexColor(EAffix.Shield)}>{battleStats.shield}</color> \n";
            _str += $"<sprite name=Fire> <color={colorSet.HexColor(EAffix.Fire)}>{battleStats.GetPower(EElement.Fire)}</color>  <sprite name=Water> <color={colorSet.HexColor(EAffix.Water)}>{battleStats.GetPower(EElement.Water)}</color>  <sprite name=Nature> <color={colorSet.HexColor(EAffix.Nature)}>{battleStats.GetPower(EElement.Nature)}</color> \n";
            _str += $"<sprite name=Speed> <color={colorSet.HexColor(EAffix.Speed)}>{battleStats.speed} </color>  ";
            _str += $"<sprite name=Focus> <color={colorSet.HexColor(EAffix.Focus)}>{battleStats.focus} </color> \n";

            return _str;
        }

        public string GetInfoRight()
        {
            return battleStats.gridRange.ToStringForUnit();
        }

        public string GetInfoDown()
        {
            return $"Element:{DataBase.Element.Elements[Main].Name} {DataBase.Element.Elements[Main].Symbol}";
        }

        public string GetElements()
        {
            return $"Weakness:{DataBase.Element.Elements[Weakness].Name} {DataBase.Element.Elements[Weakness].Symbol} Resistance:{DataBase.Element.Elements[Resist].Name} {DataBase.Element.Elements[Resist].Symbol}";
        }

        public string ColouredName()
        {
            string _hexColour;
            if (playerType == EPlayerType.Human)
                _hexColour = colorSet.HexColor(EColor.Ally);
            else 
                _hexColour = colorSet.HexColor(EColor.Enemy);
            return $"<color={_hexColour}>{unitName}</color>";
        }

        public Sprite GetIcon()
        {
            return UnitSprite;
        }

        public Color GetTeamColor()
        {
            return playerType == EPlayerType.Human ? colorSet.GetColors()[EColor.Ally] : colorSet.GetColors()[EColor.Enemy];
        }

        public abstract UnitEvent OnTooltipOn { get; }
    }

    public class AttackEventArgs : EventArgs
    {
        public Unit Attacker;
        public Unit Defender;

        public int Damage;

        public AttackEventArgs(Unit _attacker, Unit _defender, int _damage)
        {
            Attacker = _attacker;
            Defender = _defender;

            Damage = _damage;
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
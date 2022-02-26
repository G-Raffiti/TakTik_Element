using System;
using System.Collections.Generic;
using System.Linq;
using _Instances;
using _Pathfinding.DataStructs;
using Buffs;
using DataBases;
using GridObjects;
using StateMachine;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Cells
{
    /// <summary>
    /// Class representing a single field (cell) on the grid.
    /// </summary>
    public abstract class Cell : MonoBehaviour, IGraphNode, IEquatable<Cell>
    {
        ///////////////////// Object References in Unity ///////////////////////////////////////////////////////////////
        [Header("Unity Infos")]
        [SerializeField] private SpriteRenderer frame;
        [SerializeField] private SpriteRenderer fade;
        [SerializeField] public SpriteRenderer element;
        [SerializeField] private SpriteRenderer effect;
        [SerializeField] public SpriteRenderer background;        
        [SerializeField] private ColorSet colorSet;
        [SerializeField] private Sprite selectFrame;
        [SerializeField] private Sprite fullFrame;
        public Sprite full { get; set; }
        private Dictionary<EColor, Color> Colors = new Dictionary<EColor, Color>();
        private CellState state;
        public CellState State => state;
        
        /// <summary>
        /// instance variable <c>CellSO</c> contain all the base information and sprites of the Cell
        /// </summary>
        [SerializeField] protected CellSO cellSO;

        public CellSO CellSO => cellSO;
        
        
        ////////////////////////// Coordinate in the grid / Neighbours /////////////////////////////////////////////////
        /// <summary>
        /// instance variable <c>_directions</c> Offset Directions to get Neighbours
        /// </summary>
        private static readonly Vector2[] _directions =
        {
            new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1)
        };
        
        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private Vector2 offsetCoord;
        /// <summary>
        /// property <c>OffsetCoord</c> return the Coordinate on the Cell
        /// </summary>
        public Vector2 OffsetCoord { get { return offsetCoord; } set { offsetCoord = value; } }

        /// <summary>
        /// property <c>Neighbours</c> Return a List of the adjacent Cells
        /// </summary>
        public List<Cell> Neighbours { get; private set; }

        /// <summary>
        /// Instance variable <c>IsSpawnPlace</c> bool that return if the Cell is a Forced SpawnPlace for BattleHeroes.
        /// </summary>
        public bool IsSpawnPlace;
        
        

        ////////////////////// Type of Cell ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// property <c>IsTaken</c> return true if something is occupying the cell.
        /// </summary>
        public bool IsTaken => GetCurrentIMovable() != null;

        /// <summary>
        /// property <c>IsUnderground</c> return true if you can fall in it.
        /// </summary>
        public bool IsUnderGround { get; set; }

        /// <summary>
        /// property <c>IsWalkable</c> return true if the Tile is NOT Taken and NOT underground
        /// </summary>
        public bool IsWalkable => !IsTaken && !IsUnderGround;

        /// <summary>
        /// Cost of moving through the cell.
        /// </summary>
        [HideInInspector]
        public float MovementCost = 1;

        ////////////////////////// Current I Movable ///////////////////////////////////////////////////////////////////
        /// <summary>
        /// property <c>CurrentUnit</c> Reference of the Current Unit on the Cell
        /// </summary>
        public Unit CurrentUnit { get; private set; }
        
        /// <summary>
        /// property <c>CurrentGridObject</c> Reference of the Current GridObject on the Cell
        /// </summary>
        public GridObject CurrentGridObject { get; private set; }
        
        /// <summary>
        /// method <c>GetCurrentIMovable</c> Return the Unit or the GridObject on the Cell
        /// </summary>
        public IMovable GetCurrentIMovable()
        {
            if (CurrentUnit != null) return CurrentUnit;
            if (CurrentGridObject != null) return CurrentGridObject;
            return null;
        }
        
        /////////////////////////////////// Buffs //////////////////////////////////////////////////////////////////////
        /// <summary>
        /// property <c>isCorrupted</c> return true if the Cell has the Buff Corruption applied to it.
        /// </summary>
        public bool isCorrupted { get; set; }

        /// <summary>
        /// property <c>Buffs</c> return the List of all Buff applied to the Cell
        /// </summary>
        public List<Buff> Buffs { get; set; }

        //////////////////////// EVENTS ////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CellClicked event is invoked when user clicks on the cell. 
        /// It requires a collider on the cell game object to work.
        /// </summary>
        public event EventHandler CellClicked;
        /// <summary>
        /// CellHighlighed event is invoked when cursor enters the cell's collider. 
        /// It requires a collider on the cell game object to work.
        /// </summary>
        public event EventHandler CellHighlighted;
        /// <summary>
        /// CellDehighlighted event is invoked when cursor exits the cell's collider. 
        /// It requires a collider on the cell game object to work.
        /// </summary>
        public event EventHandler CellDehighlighted;
        
        ////////////////////////////// Methods related to Movements ////////////////////////////////////////////////////
        #region Movement
        /// <summary>
        /// Method that empty the Fields CurrentUnit and CurrentGridObject turn isTaken to false
        /// </summary>
        public void FreeTheCell()
        {
            if (CurrentUnit != null)
            {
                Buffs.ForEach(b =>
                {
                    b.Undo(CurrentUnit);
                });
            }
            CurrentUnit = null;
            CurrentGridObject = null;
        }

        /// <summary>
        /// Method that put the IMovable as the Current Unit or GridObject turn isTaken to true
        /// </summary>
        public void Take(IMovable _movable)
        {
            if (_movable.Cell != null)
                _movable.Cell.FreeTheCell();
            
            if (IsUnderGround)
            {
                _movable.StartCoroutine(_movable.Fall(this));
                return;
            }

            if (_movable is Unit _unit)
            {
                CurrentUnit = _unit;
                Buffs.ForEach(b =>
                {
                    b.Effect.OnUnitTakeCell(b, CurrentUnit);
                });
            }
            
            else if (_movable is GridObject _gridObject)
                CurrentGridObject = _gridObject;
            
            BattleStateManager.instance.OnIMovableMoved(_movable, this);
        }
        
        /// <summary>
        /// Method Called on Check to be sure IMovable are on the Good Cells.
        /// </summary>
        public void ForceTake(IMovable _movable)
        {
            if (_movable.Cell != null)
                _movable.Cell.FreeTheCell();

            if (_movable is Unit _unit)
            {
                CurrentUnit = _unit;
            }
            
            else if (_movable is GridObject _gridObject)
                CurrentGridObject = _gridObject;
            
            if (SceneManager.GetActiveScene () != SceneManager.GetSceneByName("BattleScene")) return;

            if (IsUnderGround)
            {
                _movable.StartCoroutine(_movable.Fall(this));
                return;
            }

            BattleStateManager.instance.OnIMovableMoved(_movable, this);
        }

        /// <summary>
        /// Method Called when something is on top of this Cell if it is Underground
        /// </summary>
        public void FallIn()
        {
            if (!IsUnderGround) return;
            FreeTheCell();
            background.sprite = full;
            IsUnderGround = false;
        }
        #endregion

        /////////////////////////// Grid Related Methods ///////////////////////////////////////////////////////////////
        #region Grid Related Methods
        /// <summary>
        /// Method returns distance to a cell that is given as parameter. 
        /// </summary>
        public int GetDistance(Cell other)
        {
            return (int)(Mathf.Abs(OffsetCoord.x - other.OffsetCoord.x) + Mathf.Abs(OffsetCoord.y - other.OffsetCoord.y));
        }//Distance is given using Manhattan Norm.

        /// <summary>
        /// Method returns cells adjacent to current cell, from list of cells given as parameter.
        /// </summary>
        public List<Cell> GetNeighbours(List<Cell> cells)
        {
            if (Neighbours == null)
            {
                Neighbours = new List<Cell>(4);
                foreach (Vector2 direction in _directions)
                {
                    Cell neighbour = cells.Find(c => c.OffsetCoord == OffsetCoord + direction);
                    if (neighbour == null) continue;

                    Neighbours.Add(neighbour);
                }
            }

            return Neighbours;
        }
        
        /// <summary>
        /// Method returns cell's physical dimensions It is used in grid generators.
        /// </summary>
        public Vector3 GetCellDimensions()
        {
            return new Vector3(2, 1.154f, 0);
        }
        #endregion

        ////////////////////////////// Initialize Methods //////////////////////////////////////////////////////////////
        #region Initialize
        /// <summary>
        /// method <c>SetCellSO</c> called on <code>Board.Load()</code> to set the Type of Cell
        /// </summary>
        public void SetCellSO(CellSO _savedCellType)
        {
            cellSO = _savedCellType;
        }
        
        /// <summary>
        /// Method Called after the Board is Loaded
        /// </summary>
        public void Initialize()
        {
            isCorrupted = false;
            Buffs = new List<Buff>();
            cellSO.Spawn(this);
            Colors = colorSet.GetColors();
            AutoSort();
            if(cellSO.BasicBuff.Effect != null)
                AddBuff(new Buff(cellSO.BasicBuff));
        }
        
        private void AutoSort()
        {
            int position = (int) (transform.position.y / (GetCellDimensions().y/2f));
            frame.sortingOrder = 300 - position;
            fade.sortingOrder = 200 - position;
            element.sortingOrder = 500 - position;
            effect.sortingOrder = 100 - position;
            background.sortingOrder = 0 - position;
        }
        #endregion
        
        /////////////////////////// On Mouse Unity Event Handler ///////////////////////////////////////////////////////
        #region OnMouse Event Handler
        public virtual void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            CellHighlighted?.Invoke(this, new EventArgs());
            if (CurrentGridObject != null)
            {
                CurrentGridObject.OnPointerEnter();
            }
            if (CurrentUnit != null)
                CurrentUnit.OnPointerEnter();
        }

        public virtual void OnMouseExit()
        {
            
            if (CurrentGridObject != null)
            {
                CurrentGridObject.OnPointerExit();
            }
            if (CurrentUnit != null)
                CurrentUnit.OnPointerExit();
            if (EventSystem.current.IsPointerOverGameObject()) return;
            CellDehighlighted?.Invoke(this, new EventArgs());
        }

        public virtual void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            CellClicked?.Invoke(this, new EventArgs());
            if(CurrentUnit != null)
                CurrentUnit.OnLeftClick();
        }
        private void OnMouseOver()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                OnMouseExit();
                return;
            }
            if (Input.GetMouseButtonDown(1))
            {
                if(CurrentUnit != null)
                    CurrentUnit.OnRightClick();
            }
        }
        #endregion
        
        /////////////////////// Mark As Methods ////////////////////////////////////////////////////////////////////////
        # region Mark As
        /// <summary>method <c>MarkAs</c> marks the cell to the given state</summary>
        public void MarkAs(CellState _state)
        {
            frame.color = _state.frameColor;
            fade.color = _state.HighlightColor;
            element.color = _state.ElementColor;
            frame.sprite = _state.frame;
        }
        /// <summary>
        ///  Method marks the cell to give user an indication that selected unit can reach it.
        /// </summary>
        public void MarkAsReachable()
        {
            state = new CellState(selectFrame, Colors[EColor.reachable],
                Colors[EColor.reachable] * Colors[EColor.transparency]);
            MarkAs(state);
        }
        
        /// <summary>
        ///  Method marks the cell to give user an indication that selected unit can't reach it but it is in the range.
        /// </summary>
        public void MarkAsUnReachable()
        {
            state = new CellState(null, Colors[EColor.unMark],
                Colors[EColor.reachable] * Colors[EColor.transparency]);
            MarkAs(state);
        }
        
        /// <summary>
        /// Method marks the cell as a part of a path.
        /// </summary>
        public void MarkAsPath()
        {
            state = new CellState(selectFrame, Colors[EColor.path],
                Colors[EColor.path] * Colors[EColor.transparency]);
            MarkAs(state);
        }
        
        /// <summary>
        /// Method marks the cell as highlighted. It gets called when the mouse is over the cell.
        /// </summary>
        public void MarkAsHighlighted()
        {
            try
            {
                state = new CellState(fullFrame, Colors[EColor.highlighted],
                    Colors[EColor.highlighted] * Colors[EColor.transparency]);
                MarkAs(state);
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log(e.StackTrace);
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
            }
            
        }
        /// <summary>
        /// Method returns the cell to its base appearance.
        /// </summary>
        public void UnMark()
        {
            state = new CellState(null, Colors[EColor.unMark],
                Colors[EColor.none]);
            MarkAs(state);
        }

        /// <summary>
        /// Method marks the Cell as Interactable. It get Called when a Unit is in range or if an Object can be used.
        /// </summary>
        public void MarkAsInteractable()
        {
            state = new CellState(selectFrame, Colors[EColor.usable],
                Colors[EColor.usable] * Colors[EColor.transparency]);
            MarkAs(state);
        }

        /// <summary>
        /// Method marks the Cell with the Enemies Color
        /// </summary>
        public void MarkAsEnemyCell()
        {
            state = new CellState(selectFrame, Colors[EColor.enemy],
                Colors[EColor.enemy] * Colors[EColor.transparency]);
            MarkAs(state);
        }
        #endregion

        ///////////////////////////// Buffs ////////////////////////////////////////////////////////////////////////////
        #region Buffs
        /// <summary>
        /// Method <c>AddBuff</c> Called to Add a Buff to the Cell
        /// </summary>
        public void AddBuff(Buff _buff)
        {
            if (_buff == null) return;
            Buff buff = new Buff(_buff);
            buff.onFloor = true;
            
            bool applied = false;
            
            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].Effect == buff.Effect)
                {
                    Buffs[i] += buff;
                    applied = true;
                    break;
                }
            }

            if (!applied)
            {
                Buffs.Add(buff);
                effect.sprite = buff.Effect.OnFloorSprite;
            }
        }

        /// <summary>
        /// method <c>OnEndTurn</c> Apply all Buffs effect to the <c>CurrentUnit</c>
        /// </summary>
        public void OnEndTurn()
        {
            Buffs.ForEach(b =>
            {
                b.OnEndTurn(CurrentUnit);
            });

            List<Buff> newList = new List<Buff>(Buffs.Where(b => b.Effect != cellSO.BasicBuff.Effect && b.Effect != DataBase.Cell.CorruptionSO));

            foreach (Buff _buff in newList.Where(_buff => _buff.Duration <= 0))
            {
                if (CurrentUnit != null) _buff.Undo(CurrentUnit);
                Buffs.Remove(_buff);
            }

            effect.sprite = Buffs.Count <= 0 
                ? null 
                : Buffs[Buffs.Count - 1].Effect.OnFloorSprite;
        }
        #endregion
        
        ///////////////////////// IEqualable / IGraphNode //////////////////////////////////////////////////////////////
        # region IEqualable / IGraphNode
        public virtual bool Equals(Cell other) => 
            OffsetCoord.x == other.OffsetCoord.x && OffsetCoord.y == other.OffsetCoord.y;

        public override bool Equals(object other)
        {
            if (!(other is Cell))
                return false;

            return Equals(other as Cell);
        }
        
        public int GetDistance(IGraphNode other) =>GetDistance(other as Cell);
        
        public override int GetHashCode()
        {
            int _hash = 23;

            _hash = (_hash * 37) + (int)OffsetCoord.x;
            _hash = (_hash * 37) + (int)OffsetCoord.y;
            return _hash;
        }
        #endregion
    }
}
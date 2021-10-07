using System;
using System.Collections.Generic;
using GridObjects;
using Pathfinding.DataStructs;
using StatusEffect;
using Units;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cells
{
    /// <summary>
    /// Class representing a single field (cell) on the grid.
    /// </summary>
    public abstract class Cell : MonoBehaviour, IGraphNode, IEquatable<Cell>
    {
        [SerializeField]
        private Vector2 offsetCoord;
        /// <summary>
        /// Position of the cell on the grid.
        /// </summary>
        public Vector2 OffsetCoord { get { return offsetCoord; } set { offsetCoord = value; } }


        /// <summary>
        /// Indicates if something is occupying the cell.
        /// </summary>
        public virtual bool IsTaken => GetCurrentIMovable() != null;

        /// <summary>
        /// Indicate if you can fall in it.
        /// </summary>
        public abstract bool IsUnderGround { get; set; }

        /// <summary>
        /// Indicate if the Tile is NOT Taken and NOT underground
        /// </summary>
        public virtual bool IsWalkable => !IsTaken && !IsUnderGround;
        
        /// <summary>
        /// Cost of moving through the cell.
        /// </summary>
        [HideInInspector]
        public float MovementCost = 1;

        /// <summary>
        /// Return the Unit on it
        /// </summary>
        public Unit CurrentUnit { get; private set; }
        
        /// <summary>
        /// Return the GridObject on it
        /// </summary>
        public GridObject CurrentGridObject { get; private set; }
        
        /// <summary>
        /// Return the Unit or the GridObject on it
        /// </summary>
        public IMovable GetCurrentIMovable()
        {
            if (CurrentUnit != null) return CurrentUnit;
            if (CurrentGridObject != null) return CurrentGridObject;
            return null;
        }
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

        protected internal virtual void OnMouseEnter()
        {
            CellHighlighted?.Invoke(this, new EventArgs());
        }
        protected internal virtual void OnMouseExit()
        {
            CellDehighlighted?.Invoke(this, new EventArgs());
        }

        internal void OnMouseDown()
        {
            CellClicked?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Method returns distance to a cell that is given as parameter. 
        /// </summary>
        public abstract int GetDistance(Cell other);

        /// <summary>
        /// Method returns cells adjacent to current cell, from list of cells given as parameter.
        /// </summary>
        public abstract List<Cell> GetNeighbours(List<Cell> cells);
        /// <summary>
        /// Method returns cell's physical dimensions It is used in grid generators.
        /// </summary>
        public abstract Vector3 GetCellDimensions();

        /// <summary>
        ///  Method marks the cell to give user an indication that selected unit can reach it.
        /// </summary>
        public abstract void MarkAsReachable();
        /// <summary>
        ///  Method marks the cell to give user an indication that selected unit can't reach it but it is in the range.
        /// </summary>
        public abstract void MarkAsUnReachable();
        /// <summary>
        /// Method marks the cell as a part of a path.
        /// </summary>
        public abstract void MarkAsPath();
        /// <summary>
        /// Method marks the cell as highlighted. It gets called when the mouse is over the cell.
        /// </summary>
        public abstract void MarkAsHighlighted();
        /// <summary>
        /// Method returns the cell to its base appearance.
        /// </summary>
        public abstract void UnMark();

        /// <summary>
        /// Method marks the Cell as Interactable. It get Called when a Unit is in range or if an Object can be used.
        /// </summary>
        public abstract void MarkAsInteractable();

        public int GetDistance(IGraphNode other)
        {
            return GetDistance(other as Cell);
        }

        public virtual bool Equals(Cell other)
        {
            return (OffsetCoord.x == other.OffsetCoord.x && OffsetCoord.y == other.OffsetCoord.y);
        }

        public override bool Equals(object other)
        {
            if (!(other is Cell))
                return false;

            return Equals(other as Cell);
        }

        public override int GetHashCode()
        {
            int _hash = 23;

            _hash = (_hash * 37) + (int)OffsetCoord.x;
            _hash = (_hash * 37) + (int)OffsetCoord.y;
            return _hash;
        }

        /// <summary>
        /// Method for cloning field values into a new cell. Used in Tile Painter in Grid Helper
        /// </summary>
        /// <param name="newCell">Cell to copy field values to</param>
        public abstract void CopyFields(Cell newCell);

        /// <summary>
        /// Return a List of the adjacent Cells
        /// </summary>
        public abstract List<Cell> Neighbours { get; set; }

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
            _movable.Cell?.FreeTheCell();
            
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
                    b.OnStartTurn(CurrentUnit);
                    b.OnEndTurn(CurrentUnit);
                    b.Apply(CurrentUnit);
                });
            }
            else if (_movable is GridObject _gridObject)
                CurrentGridObject = _gridObject;
            
            _movable.Cell = this;
        }
        
        /// <summary>
        /// Methode Called to Add a Buff to the Cell
        /// </summary>
        public abstract void AddBuff(Buff _buff);
        
        /// <summary>
        /// Method Called when something is on top of this Cell if it is Underground
        /// </summary>
        public abstract void FallIn();
        
        /// <summary>
        /// return if the Cell has the Buff Corruption applied to it.
        /// </summary>
        public abstract bool isCorrupted { get; set; }

        /// <summary>
        /// return the List of all Buff applied to the Cell
        /// </summary>
        public abstract List<Buff> Buffs { get; set; }

        public abstract CellSO CellSO { get; set; }

        /// <summary>
        /// Bool Used on Enemy Spawn
        /// </summary>
        public bool IsSpawnPlace;

        /// <summary>
        /// Method called to apply effects of Buffs to the Current Unit.
        /// </summary>
        public abstract void OnEndTurn();
    }
}
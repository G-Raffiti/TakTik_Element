using System;
using System.Collections.Generic;
using GridObjects;
using Pathfinding.DataStructs;
using StatusEffect;
using Units;
using UnityEngine;

namespace Cells
{
    /// <summary>
    /// Class representing a single field (cell) on the grid.
    /// </summary>
    public abstract class Cell : MonoBehaviour, IGraphNode, IEquatable<Cell>
    {
        [HideInInspector]
        [SerializeField]
        private Vector2 offsetCoord;
        /// <summary>
        /// Position of the cell on the grid.
        /// </summary>
        public Vector2 OffsetCoord { get { return offsetCoord; } set { offsetCoord = value; } }

        
        /// <summary>
        /// Indicates if something is occupying the cell.
        /// </summary>
        [Header("Cell Parameters")]
        public bool isTaken;

        /// <summary>
        /// Indicate if you can fall in it.
        /// </summary>
        public bool isUnderGround;
        
        /// <summary>
        /// Indicate if the Tile is NOT Taken and NOT underground
        /// </summary>
        public bool isWalkable => !isTaken && !isUnderGround;

        /// <summary>
        /// Cost of moving through the cell.
        /// </summary>
        public float movementCost = 1;

        /// <summary>
        /// Return the Unit on it
        /// </summary>
        public Unit CurrentUnit { get; set; }
        
        /// <summary>
        /// Return the GridObject on it
        /// </summary>
        public GridObject CurrentGridObject { get; set; }
        
        /// <summary>
        /// Return the Unit or the GridObject on it
        /// </summary>
        public IMovable GetCurrentIMovable()
        {
            if (!isTaken) return null;
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
            isTaken = false;
            CurrentUnit = null;
            CurrentGridObject = null;
        }

        /// <summary>
        /// Method that put the IMovable as the Current Unit or GridObject turn isTaken to true
        /// </summary>
        public void Take(IMovable _movable)
        {
            if (isUnderGround)
            {
                _movable.StartCoroutine(_movable.Fall(this));
                return;
            }

            isTaken = true;
            if (_movable is Unit _unit) 
                CurrentUnit = _unit;
            if (_movable is GridObject _gridObject)
            {
                CurrentGridObject = _gridObject;
            }
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
        /// return the Type of Tile it is (Used to recreate Board)
        /// </summary>
        public abstract ETile CellType { get; }
        
        /// <summary>
        /// return the List of all Buff applied to the Cell
        /// </summary>
        public abstract List<Buff> Buffs { get; }
        
        /// <summary>
        /// return the Value at witch the Buffs are effective on the Cell
        /// </summary>
        public abstract int Power { get; }

        public bool isSpawnPlace;
    }
}
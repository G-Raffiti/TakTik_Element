﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using _Instances;
using Cells;
using Gears;
using Grid;
using Pathfinding.Algorithms;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GridObjects
{
    [RequireComponent(typeof(Animation))]
    public class GridObject : IMovable
    {
        [SerializeField] private GridObjectSO gridObject;
        [SerializeField] private CellEvent gridObjectDestroyed;
        public GridObjectSO GridObjectSO
        {
            get => gridObject;
            set => gridObject = value;
        }

        private bool isUsed = false;

        /// <summary>
        /// Coroutine that play the Movement Animation on the Grid
        /// </summary>
        /// <param name="path">List of Cells that have to be crossed</param>
        /// <returns></returns>
        public override IEnumerator MovementAnimation(List<Cell> path)
        {
            IsMoving = true;
            path.Reverse();
            foreach (Cell _cell in path)
            {
                Vector3 _destinationPos = new Vector3(_cell.transform.localPosition.x, _cell.transform.localPosition.y,
                    transform.localPosition.z);
                while (transform.localPosition != _destinationPos)
                {
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, _destinationPos,
                        Time.deltaTime * MovementAnimationSpeed);
                    yield return 0;
                }
            }

            IsMoving = false;
        }
        
        [SerializeField] private float moveAnimSpeed;
        public override float MovementAnimationSpeed => moveAnimSpeed;

        public bool IsInteractable =>
            gridObject.GetZoneOfInteraction(Cell).Contains(BattleStateManager.instance.PlayingUnit.Cell) && !isUsed;

        public string Name { get; set; }

        public bool IsInteractableFrom(Cell _cell)
        {
            return GridObjectSO.GetZoneOfInteraction(Cell).Contains(_cell) && !isUsed;
        }

        public void Initialize()
        {
            GetComponent<SpriteRenderer>().sprite = gridObject.Image;
            AutoSortOrder();
            Cell.Take(this);
        }

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

        public override IEnumerator OnDestroyed()
        {
            gridObjectDestroyed.Raise(Cell);
            
            while (IsMoving)
            {
                yield return null;
            }

            GetComponent<Animation>().Play("DestroyGridObject");

            while (GetComponent<Animation>().isPlaying)
            {
                yield return null;
            }
            
            Cell.FreeTheCell();
            Destroy(gameObject);
        }

        public void Interact(Unit _unit)
        {
            gridObject.Interact(_unit, Cell);
            isUsed = true;
        }

        public void OnStartTurn()
        {
            isUsed = false;
        }
        
        public override void AutoSortOrder()
        {
            GetComponent<SpriteRenderer>().sortingOrder = 100 - (int)transform.position.y;
        }
    }
}
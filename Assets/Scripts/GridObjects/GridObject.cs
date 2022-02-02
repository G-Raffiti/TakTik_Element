using System.Collections;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using Cells;
using StateMachine;
using Units;
using UnityEngine;

namespace GridObjects
{
    [RequireComponent(typeof(Animation))]
    public class GridObject : IMovable
    {
        [SerializeField] private GridObjectSO gridObject;
        [SerializeField] private GridObjectEvent gridObjectDestroyed;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private VoidEvent onActionDone;
        private bool isDying;

        public bool IsDying => isDying;

        public GridObjectSO GridObjectSO
        {
            get => gridObject;
            set => gridObject = value;
        }

        private bool isUsed = false;

        public override List<Cell> Move(Cell destinationCell, List<Cell> path)
        {
            if (!GridObjectSO.Movable) return new List<Cell>();
            return base.Move(destinationCell, path);
        }

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
        
        /// <summary>
        /// Check if the unit is in range to activate the GridObject
        /// </summary>
        public bool IsInteractable =>
            gridObject.GetZoneOfInteraction(Cell).Contains(BattleStateManager.instance.PlayingUnit.Cell) && !isUsed;

        public bool IsInteractableFrom(Cell _cell)
        {
            return GridObjectSO.GetZoneOfInteraction(Cell).Contains(_cell) && !isUsed;
        }

        public void Initialize()
        {
            spriteRenderer.sprite = gridObject.Image;
            float halfHeight = spriteRenderer.bounds.size.y/2;
            spriteRenderer.gameObject.transform.localPosition = new Vector3(0, halfHeight-0.4f);
            AutoSortOrder();
            Cell.Take(this);
        }

        protected void OnMouseDown()
        {
            if(Cell != null)
                Cell.OnMouseDown();
        }
        protected void OnMouseEnter()
        {
            if(Cell != null)
                Cell.OnMouseEnter();
            GridObjectSO.ShowAction(BattleStateManager.instance.PlayingUnit, Cell);
        }
        protected void OnMouseExit()
        {
            if(Cell != null)
                Cell.OnMouseExit();
        }

        public override IEnumerator OnDestroyed()
        {
            gridObjectDestroyed.Raise(this);
            Animation anim = GetComponent<Animation>();
            isDying = true;
            yield return new WaitWhile(() => anim.isPlaying);
            Cell.FreeTheCell();
            isDying = false;
            yield return new WaitForSeconds(0.1f);
            Destroy(gameObject);
            onActionDone.Raise();
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

        public override SpriteRenderer MovableSprite => spriteRenderer;
        public override string getName => GridObjectSO.Name;
    }
}
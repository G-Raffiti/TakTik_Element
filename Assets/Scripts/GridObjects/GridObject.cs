using System.Collections;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using Cells;
using StateMachine;
using StateMachine.GridStates;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GridObjects
{
    [RequireComponent(typeof(Animation))]
    public class GridObject : IMovable
    {
        [Header("Unity References")]
        [SerializeField] private GridObjectSO gridObject;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [Header("Event Sender")]
        [SerializeField] private VoidEvent onActionDone;
        [SerializeField] private GridObjectEvent gridObjectDestroyed;
        [SerializeField] private InfoEvent onInfoTooltip_ON;
        [SerializeField] private VoidEvent onInfoTooltip_OFF;
        
        private bool isDying = false;
        private bool isUsed = false;
        public GridObjectSO GridObjectSO
        {
            get => gridObject;
            set => gridObject = value;
        }
        public bool IsDying => isDying;
        public override SpriteRenderer MovableSprite => spriteRenderer;
        public override string getName => GridObjectSO.Name;
        
        
        //////////////////// IMovable //////////////////////////////////////////////////////////////////////////////////
        public override List<Cell> Move(Cell destinationCell, List<Cell> path)
        {
            if (!GridObjectSO.Movable) return new List<Cell>();
            return base.Move(destinationCell, path);
        }
 
        
        //////////////////// Grid Object Interactions //////////////////////////////////////////////////////////////////
        /// <summary>
        /// Check if the unit is in range to activate the GridObject
        /// </summary>
        public bool IsInteractable =>
            gridObject.GetZoneOfInteraction(Cell).Contains(BattleStateManager.instance.PlayingUnit.Cell) && !isUsed;

        /// <summary>
        /// Check from a Given Cell
        /// </summary>
        public bool IsInteractableFrom(Cell _cell)
        {
            return GridObjectSO.GetZoneOfInteraction(Cell).Contains(_cell) && !isUsed;
        }
        
        /// <summary>
        /// Method that Call the the Action in the GridObjectSO
        /// </summary>
        /// <param name="_unit"></param>
        public void Interact(Unit _unit)
        {
            gridObject.Interact(_unit, Cell);
            isUsed = true;
        }


        /////////////////// On Mouse Actions ///////////////////////////////////////////////////////////////////////////
        public void OnPointerEnter()
        {
            if (BattleStateManager.instance.BattleState.State == EBattleState.Unit)
                GridObjectSO.ShowAction(BattleStateManager.instance.PlayingUnit, Cell);
            //if (!EventSystem.current.IsPointerOverGameObject())
            onInfoTooltip_ON.Raise(gridObject);
        }

        public void OnPointerExit()
        {
            //if (!EventSystem.current.IsPointerOverGameObject())
            onInfoTooltip_OFF.Raise();
        }
        

        /////////////////// Initialisation & On Destroy ////////////////////////////////////////////////////////////////
        public void Initialize()
        {
            spriteRenderer.sprite = gridObject.Image;
            float halfHeight = spriteRenderer.bounds.size.y/2;
            spriteRenderer.gameObject.transform.localPosition = new Vector3(0, halfHeight-0.4f);
            AutoSortOrder();
            Cell.Take(this);
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
        
         
        //////////////////// Start Turn ////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Called By State Manager on Every Units Turn
        /// </summary>
        public void OnStartTurn()
        {
            isUsed = false;
        }
    }
}
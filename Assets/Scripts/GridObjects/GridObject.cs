using System.Collections;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using Cells;
using StateMachine;
using StateMachine.GridStates;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace GridObjects
{
    [RequireComponent(typeof(Animation))]
    public class GridObject : Movable
    {
        [Header("Unity References")]
        [SerializeField] private GridObjectSo gridObject;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [Header("Event Sender")]
        [SerializeField] private VoidEvent onActionDone;
        [SerializeField] private GridObjectEvent gridObjectDestroyed;
        [FormerlySerializedAs("onInfoTooltip_ON")]
        [SerializeField] private InfoEvent onInfoTooltipOn;
        [FormerlySerializedAs("onInfoTooltip_OFF")]
        [SerializeField] private VoidEvent onInfoTooltipOff;
        
        private bool isDying = false;
        private bool isUsed = false;
        public GridObjectSo GridObjectSo
        {
            get => gridObject;
            set => gridObject = value;
        }
        public bool IsDying => isDying;
        public override SpriteRenderer MovableSprite => spriteRenderer;
        public override string GetName => GridObjectSo.Name;
        
        
        //////////////////// IMovable //////////////////////////////////////////////////////////////////////////////////
        public override List<Cell> Move(Cell _destinationCell, List<Cell> _completePath)
        {
            if (!GridObjectSo.Movable) return new List<Cell>();
            return base.Move(_destinationCell, _completePath);
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
            return GridObjectSo.GetZoneOfInteraction(Cell).Contains(_cell) && !isUsed;
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
                GridObjectSo.ShowAction(BattleStateManager.instance.PlayingUnit, Cell);
            //if (!EventSystem.current.IsPointerOverGameObject())
            onInfoTooltipOn.Raise(gridObject);
        }

        public void OnPointerExit()
        {
            //if (!EventSystem.current.IsPointerOverGameObject())
            onInfoTooltipOff.Raise();
        }
        

        /////////////////// Initialisation & On Destroy ////////////////////////////////////////////////////////////////
        public void Initialize()
        {
            spriteRenderer.sprite = gridObject.Image;
            float _halfHeight = spriteRenderer.bounds.size.y/2;
            spriteRenderer.gameObject.transform.localPosition = new Vector3(0, _halfHeight-0.4f);
            AutoSortOrder();
            Cell.Take(this);
        }
        
        public override IEnumerator OnDestroyed()
        {
            gridObjectDestroyed.Raise(this);
            Animation _anim = GetComponent<Animation>();
            isDying = true;
            yield return new WaitWhile(() => _anim.isPlaying);
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
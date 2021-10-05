using System;
using System.Collections;
using _EventSystem.CustomEvents;
using _EventSystem.Listeners;
using Grid;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Void = _EventSystem.Void;

namespace Resources.ToolTip.Scripts
{
    public abstract class InfoBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IInfo, IGameEventListener<Void>
    {
        [SerializeField] protected InfoEvent TooltipOn = null;
        [SerializeField] protected VoidEvent TooltipOff = null;
        private bool gameStarted = false;

        /// <summary>
        /// Method Called to generate the Sprite of Info Prefabs
        /// </summary>
        public virtual void DisplayIcon()
        {
            if (gameObject.GetComponent<Image>() != null)
            {
                Image ui = gameObject.GetComponent<Image>();
                ui.sprite = GetIcon();
            }
        }
        
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            BattleStateManager.instance.onStartGame.RegisterListener(this);
        }

        /// <summary>
        /// Name and main Type of Info Object
        /// </summary>
        public abstract string GetInfoMain();
        
        /// <summary>
        /// Info Left and Right are the core Infos
        /// </summary>
        public abstract string GetInfoLeft();
        
        /// <summary>
        /// Info Left and Right are the core Infos
        /// </summary>
        public abstract string GetInfoRight();
        
        /// <summary>
        /// can be use for precision
        /// </summary>
        public abstract string GetInfoDown();
        
        /// <summary>
        /// return the Sprite
        /// </summary>
        public abstract Sprite GetIcon();
        
        /// <summary>
        /// return the name with a color for rarity or element etc...
        /// </summary>
        public abstract string ColouredName();

        /// <summary>
        /// Method Called on Hover an Info Object to Display the Tooltip
        /// can be override to add a condition to not show
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!gameStarted) return;
            TooltipOn.Raise(this);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            TooltipOff.Raise();
        }

        public virtual void OnPointerClick(PointerEventData eventData) { }

        public void OnEventRaised(Void item)
        {
            gameStarted = true;
        }
    }
}

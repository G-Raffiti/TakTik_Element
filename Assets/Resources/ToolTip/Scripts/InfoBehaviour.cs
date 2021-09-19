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

        public void DisplayIcon()
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

        public abstract string GetInfoMain();
        public abstract string GetInfoLeft();
        public abstract string GetInfoRight();
        public abstract string GetInfoDown();
        public abstract Sprite GetIcon();
        public abstract string ColouredName();

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

using System.Collections;
using _EventSystem.CustomEvents;
using StateMachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Resources.ToolTip.Scripts
{
    public abstract class InfoBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IInfo
    {
        /// <summary>
        /// Method Called to generate the Sprite of Info Prefabs
        /// </summary>
        public virtual void DisplayIcon()
        {
            if (gameObject.GetComponent<Image>() != null)
            {
                Image _ui = gameObject.GetComponent<Image>();
                _ui.sprite = GetIcon();
            }
        }
        
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
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
        public abstract void OnPointerEnter(PointerEventData _eventData);
        public abstract void OnPointerExit(PointerEventData _eventData);
        public virtual void OnPointerClick(PointerEventData _eventData){}
    }
}

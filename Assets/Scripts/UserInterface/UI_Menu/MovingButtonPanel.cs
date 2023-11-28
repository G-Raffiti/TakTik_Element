using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UserInterface.UI_Menu
{
    public class MovingButtonPanel : MonoBehaviour, IPointerClickHandler
    {
        [FormerlySerializedAs("MenuPanel")]
        public MovingPanelMenu menuPanel;
        
        public void OnPointerClick(PointerEventData _eventData)
        {
            menuPanel.ShowPanelBtn(transform.GetSiblingIndex());
        }
    }
}
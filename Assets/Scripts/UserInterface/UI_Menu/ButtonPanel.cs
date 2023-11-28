using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UserInterface.UI_Menu
{
    public class ButtonPanel : MonoBehaviour, IPointerClickHandler
    {
        [FormerlySerializedAs("MenuPanel")]
        public MenuPanel menuPanel;
        
        public void OnPointerClick(PointerEventData _eventData)
        {
            menuPanel.Menu(transform.GetSiblingIndex());
        }
    }
}
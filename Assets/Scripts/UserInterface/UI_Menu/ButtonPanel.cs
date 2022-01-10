using UnityEngine;
using UnityEngine.EventSystems;

namespace UserInterface.UI_Menu
{
    public class ButtonPanel : MonoBehaviour, IPointerClickHandler
    {
        public MenuPanel MenuPanel;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            MenuPanel.Menu(transform.GetSiblingIndex());
        }
    }
}
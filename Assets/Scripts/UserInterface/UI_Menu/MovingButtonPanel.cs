using UnityEngine;
using UnityEngine.EventSystems;

namespace UserInterface.UI_Menu
{
    public class MovingButtonPanel : MonoBehaviour, IPointerClickHandler
    {
        public MovingPanelMenu MenuPanel;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            MenuPanel.ShowPanelBtn(transform.GetSiblingIndex());
        }
    }
}
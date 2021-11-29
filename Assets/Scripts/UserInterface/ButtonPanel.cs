using UnityEngine;
using UnityEngine.EventSystems;

namespace UserInterface
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
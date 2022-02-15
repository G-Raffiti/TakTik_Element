using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.UI_Menu
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] private Transform MenuBtns;
        [SerializeField] public Transform Panels;
        [SerializeField] private bool showOnStart = true;
        [SerializeField] private MovingPanelMenu MovingMenu;

        [Header("the alpha Value of the actif Menu Button")]
        [SerializeField] private float aValue = 1;
        private void Awake()
        {
            Close();
        }

        private void Start()
        {
            if (MovingMenu == null)
            {
                for (int i = 0; i < MenuBtns.childCount; i++)
                {
                    if (MenuBtns.GetChild(i).GetComponent<ButtonPanel>() != null) continue;
                
                    MenuBtns.GetChild(i).gameObject.AddComponent<ButtonPanel>();
                    MenuBtns.GetChild(i).GetComponent<ButtonPanel>().MenuPanel = this;
                }
            }
            else
            {
                for (int i = 0; i < MenuBtns.childCount; i++)
                {
                    if (MenuBtns.GetChild(i).GetComponent<MovingButtonPanel>() != null) continue;
                
                    MenuBtns.GetChild(i).gameObject.AddComponent<MovingButtonPanel>();
                    MenuBtns.GetChild(i).GetComponent<MovingButtonPanel>().MenuPanel = MovingMenu;
                }
            }
            Close();
            if(showOnStart)
                Menu(0);
        }

        public void Menu(int index)
        {
            foreach (Transform _btn in MenuBtns)
            {
                Color _c = _btn.GetComponent<Image>().color;
                _c.a = 0.5f;
                _btn.GetComponent<Image>().color = _c;
            }
            foreach (Transform _panel in Panels)
            {
                _panel.gameObject.SetActive(false);
            }
            Panels.GetChild(index).gameObject.SetActive(true);
            
            Color _a = MenuBtns.GetChild(index).GetComponent<Image>().color;
            _a.a = aValue;
            MenuBtns.GetChild(index).GetComponent<Image>().color = _a;
        }

        public void Close()
        {
            foreach (Transform _btn in MenuBtns)
            {
                Color _c = _btn.GetComponent<Image>().color;
                _c.a = 0.5f;
                _btn.GetComponent<Image>().color = _c;
            }
            foreach (Transform _panel in Panels)
            {
                _panel.gameObject.SetActive(false);
            }
        }
    }
}
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UserInterface.UI_Menu
{
    public class MenuPanel : MonoBehaviour
    {
        [FormerlySerializedAs("MenuBtns")]
        [SerializeField] private Transform menuBtns;
        [FormerlySerializedAs("Panels")]
        [SerializeField] public Transform panels;
        [SerializeField] private bool showOnStart = true;
        [FormerlySerializedAs("MovingMenu")]
        [SerializeField] private MovingPanelMenu movingMenu;

        [Header("the alpha Value of the actif Menu Button")]
        [SerializeField] private float aValue = 1;
        private void Awake()
        {
            Close();
        }

        private void Start()
        {
            if (movingMenu == null)
            {
                for (int _i = 0; _i < menuBtns.childCount; _i++)
                {
                    if (menuBtns.GetChild(_i).GetComponent<ButtonPanel>() != null) continue;
                
                    menuBtns.GetChild(_i).gameObject.AddComponent<ButtonPanel>();
                    menuBtns.GetChild(_i).GetComponent<ButtonPanel>().menuPanel = this;
                }
            }
            else
            {
                for (int _i = 0; _i < menuBtns.childCount; _i++)
                {
                    if (menuBtns.GetChild(_i).GetComponent<MovingButtonPanel>() != null) continue;
                
                    menuBtns.GetChild(_i).gameObject.AddComponent<MovingButtonPanel>();
                    menuBtns.GetChild(_i).GetComponent<MovingButtonPanel>().menuPanel = movingMenu;
                }
            }
            Close();
            if(showOnStart)
                Menu(0);
        }

        public void Menu(int _index)
        {
            foreach (Transform _btn in menuBtns)
            {
                Color _c = _btn.GetComponent<Image>().color;
                _c.a = 0.5f;
                _btn.GetComponent<Image>().color = _c;
            }
            foreach (Transform _panel in panels)
            {
                _panel.gameObject.SetActive(false);
            }
            panels.GetChild(_index).gameObject.SetActive(true);
            
            Color _a = menuBtns.GetChild(_index).GetComponent<Image>().color;
            _a.a = aValue;
            menuBtns.GetChild(_index).GetComponent<Image>().color = _a;
        }

        public void Close()
        {
            foreach (Transform _btn in menuBtns)
            {
                Color _c = _btn.GetComponent<Image>().color;
                _c.a = 0.5f;
                _btn.GetComponent<Image>().color = _c;
            }
            foreach (Transform _panel in panels)
            {
                _panel.gameObject.SetActive(false);
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] private Transform MenuBtns;
        [SerializeField] private Transform Panels;

        private void Start()
        {
            for (int i = 0; i < MenuBtns.childCount; i++)
            {
                if (MenuBtns.GetChild(i).GetComponent<ButtonPanel>() != null) continue;
                
                MenuBtns.GetChild(i).gameObject.AddComponent<ButtonPanel>();
                MenuBtns.GetChild(i).GetComponent<ButtonPanel>().MenuPanel = this;
            }
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
            _a.a = 1f;
            MenuBtns.GetChild(index).GetComponent<Image>().color = _a;
        }
    }
}
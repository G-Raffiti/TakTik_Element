using Resources.ToolTip.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class Icon_UI : MonoBehaviour
    {
        [SerializeField] private Image icon;

        public void DisplayIcon(IInfo _info)
        {
            icon.sprite = _info.GetIcon();
        }
    }
}
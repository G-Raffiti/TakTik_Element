using Resources.ToolTip.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.ToolTips
{
    public class BasicTooltip : Tooltip
    {
        [HideInInspector] public IInfo Info;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI main;
        [SerializeField] private TextMeshProUGUI left;
        [SerializeField] private TextMeshProUGUI right;
        [SerializeField] private TextMeshProUGUI down;
        [SerializeField] private LayoutElement Layout;
        [SerializeField] private int widthMax;
        
        protected override void ShowToolTip()
        {
            Layout.enabled = false;
            icon.sprite = Info.GetIcon();
            main.text = Info.GetInfoMain();
            left.text = Info.GetInfoLeft();
            right.text = Info.GetInfoRight();
            down.text = Info.GetInfoDown();
            
            if (left.preferredWidth + right.preferredWidth > widthMax)
                Layout.enabled = true;
        }
    }
}
using Gears;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.ToolTips
{
    public class GearTooltip : Tooltip
    {
        [HideInInspector]
        public GearInfo Gear;

        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private TextMeshProUGUI main;
        [SerializeField] private TextMeshProUGUI statsLeft;
        [SerializeField] private TextMeshProUGUI statsRight;
        [SerializeField] private Image icon;
        [SerializeField] private Image shadowEffect;
        [SerializeField] private Image frame;
        [SerializeField] private Image rarity;
        
        protected override void ShowToolTip()
        {
            nameTxt.text = Gear.Gear.GearSO.Name;
            main.text = Gear.GetInfoMain();
            statsLeft.text = Gear.GetInfoLeft();
            statsRight.text = Gear.GetInfoRight();
            icon.sprite = Gear.GetIcon();
            shadowEffect.sprite = Gear.GetIcon();
            frame.color = Gear.Gear.GearSO.Rarity.TextColour;
            rarity.color = Gear.Gear.GearSO.Rarity.TextColour;
        }
    }
}
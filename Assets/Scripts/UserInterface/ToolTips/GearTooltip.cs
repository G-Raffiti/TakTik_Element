using Gears;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UserInterface.ToolTips
{
    public class GearTooltip : Tooltip
    {
        [FormerlySerializedAs("Gear")]
        [HideInInspector]
        public GearInfo gear;

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
            nameTxt.text = gear.Gear.GearSo.Name;
            main.text = gear.GetInfoMain();
            statsLeft.text = gear.GetInfoLeft();
            statsRight.text = gear.GetInfoRight();
            icon.sprite = gear.GetIcon();
            shadowEffect.sprite = gear.GetIcon();
            frame.color = gear.Gear.GearSo.Rarity.TextColour;
            rarity.color = gear.Gear.GearSo.Rarity.TextColour;
        }
    }
}
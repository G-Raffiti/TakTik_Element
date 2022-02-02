using System.Collections.Generic;
using _EventSystem.CustomEvents;
using DataBases;
using Resources.ToolTip.Scripts;
using StatusEffect;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterface
{
    public class BuffInfo : InfoBehaviour
    {
        public Buff Buff { get; set; }
        public Unit Unit { get; set; }
        
        [Header("References Unity")]
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI Duration;
        [SerializeField] private List<Image> frame;
        [SerializeField] private ColorSet colorSet;
        
        [Header("Tooltip Events")] 
        [SerializeField] private InfoEvent InfoTooltip_ON;
        [SerializeField] private VoidEvent InfoTooltip_OFF;

        public override string GetInfoMain()
        {
            return $"{ColouredName()} {Buff.Duration}<sprite name=Duration>";
        }

        public override string GetInfoLeft()
        {
            if (Unit != null)
                return Buff.InfoOnUnit(Buff, Unit);
            return Buff.InfoBuff();
        }

        public override string GetInfoRight()
        {
            return "";
        }

        public override string GetInfoDown()
        {
            return "";
        }

        public override Sprite GetIcon()
        {
            return Buff.Effect.OnFloorSprite;
        }

        public override string ColouredName()
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Buff.Effect.Element.TextColour);
            return $"<color=#{_hexColor}>{Buff.Effect.Name}</color>";
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            InfoTooltip_ON.Raise(this);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            InfoTooltip_OFF.Raise();
        }

        public override void DisplayIcon()
        {
            icon.sprite = GetIcon();
            frame.ForEach(i => i.color = Buff.Effect.Type == EBuff.Buff
                ? colorSet.GetColors()[EColor.Buff]
                : colorSet.GetColors()[EColor.Debuff]);
            Duration.text = Buff.Effect.IsDefinitive 
                ? $"<sprite name=Infinity>" 
                : $"{Buff.Duration}";
        }
    }
}
using System.Collections.Generic;
using Resources.ToolTip.Scripts;
using StatusEffect;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class BuffInfo : InfoBehaviour
    {
        public Buff Buff { get; set; }
        
        [SerializeField] private Image icon;
        [SerializeField] private List<Image> StatIcons;
        [SerializeField] private TextMeshProUGUI Duration;
        [SerializeField] private TextMeshProUGUI value;
        [SerializeField] private List<Image> frame;


        public override string GetInfoMain()
        {
            return $"{ColouredName()} {Buff.Duration}<sprite name=Duration>";
        }

        public override string GetInfoLeft()
        {
            throw new System.NotImplementedException();
        }

        public override string GetInfoRight()
        {
            throw new System.NotImplementedException();
        }

        public override string GetInfoDown()
        {
            throw new System.NotImplementedException();
        }

        public override Sprite GetIcon()
        {
            throw new System.NotImplementedException();
        }

        public override string ColouredName()
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Buff.Effect.Element.TextColour);
            return $"<color=#{_hexColor}>{Buff.Effect.Name}</color>";
        }
    }
}
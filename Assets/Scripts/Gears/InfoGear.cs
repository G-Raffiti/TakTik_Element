using DataBases;
using Resources.ToolTip.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gears
{
    public class InfoGear : InfoBehaviour
    {
        public Gear Gear { get; set; }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            TooltipOn.Raise(this);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            TooltipOff.Raise();
        }


        public override string GetInfoMain()
        {
            return $"<size=35>{ColouredName()}</size>";
        }

        public override string GetInfoLeft()
        {
            string ret = "Stats:\n";
            ret += Gear.GearSO.MainStat.ToString();
            Gear.Affixes.ForEach(affix => ret += $"\n{affix.ToString()}");
            return ret;
        }

        public override string GetInfoRight()
        {
            string ret = "\n(implicit)";
            Gear.Affixes.ForEach(affix => ret += $"\n{affix.TierRangeToString()}");
            return ret;
        }

        public override string GetInfoDown()
        {
            return Gear.GearSO.SpecialEffect != null ? Gear.GearSO.SpecialEffect.InfoEffect() : "";
        }

        public override Sprite GetIcon()
        {
            return Gear.GearSO.Icon;
        }

        public override string ColouredName()
        {
            return $"<color={ColorSet.HexColor(Gear.GearSO.Rarity.TextColour)}>{Gear.GearSO.Name}</color>\n{Gear.GearSO.Rarity.Name} {Gear.GearSO.Type}";
        }
    }
}
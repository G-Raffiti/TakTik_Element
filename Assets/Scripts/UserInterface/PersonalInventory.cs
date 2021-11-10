using System.Collections.Generic;
using System.Linq;
using Resources.ToolTip.Scripts;
using Stats;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterface
{
    public class PersonalInventory : InfoBehaviour
    {
        [SerializeField] private ColorSet colorSet;
        [SerializeField] private Image icon;
        [SerializeField] private Image health;
        [SerializeField] private List<Transform> slots;
        private BattleStats BattleStats;
        private BattleStats baseStats;
        private BattleStats total;
        
        public Hero Hero { get; private set; }

        public List<Transform> Slots => slots;

        public void Initialize(Hero _hero)
        {
            Hero = _hero;
            
            baseStats = _hero.BattleStats;
            BattleStats = new BattleStats(baseStats + Hero.Inventory.GearStats());
            total = BattleStats;
            BattleStats.HP = _hero.ActualHP;
            
            icon.sprite = Hero.UnitSprite;
            health.GetComponent<Image>().fillAmount = BattleStats.HP / (float)total.HP;
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            TooltipOn.Raise(this);
        }
        
        public override string GetInfoMain()
        {
            return ColouredName();
        }

        
        public override string GetInfoLeft()
        {
            string str = "";
            str += $"<sprite name=AP> <color={colorSet.HexColor(EAffix.AP)}>{(int)total.AP}</color>    ";
            str += $"<sprite name=MP> <color={colorSet.HexColor(EAffix.MP)}>{(int)total.MP}</color> \n";
            str += $"<sprite name=HP> <color={colorSet.HexColor(EAffix.HP)}>{BattleStats.HP} </color>/ {total.HP}    ";
            str += $"<sprite name=Shield> <color={colorSet.HexColor(EAffix.Shield)}>{BattleStats.Shield}</color> \n";
            str += $"<sprite name=Fire> <color={colorSet.HexColor(EAffix.Fire)}>{BattleStats.GetPower(EElement.Fire)}</color>  <sprite name=Water> <color={colorSet.HexColor(EAffix.Water)}>{BattleStats.GetPower(EElement.Water)}</color>  <sprite name=Nature> <color={colorSet.HexColor(EAffix.Nature)}>{BattleStats.GetPower(EElement.Nature)}</color>";

            return str;
        }

        public override string GetInfoRight()
        {
            Unit u = null;
            string str = "";
            str += BattleStats.Range.ToString(u)+ "\n";
            str += $"<sprite name=Speed> <color={colorSet.HexColor(EAffix.Speed)}>{BattleStats.Speed} </color> \n";
            return str;
        }
        
        public override string GetInfoDown()
        {
            return "";
        }

        public override string ColouredName()
        {
            return $"<color={colorSet.HexColor(EColor.ally)}>{Hero.UnitName}</color>";
        }

        public override Sprite GetIcon()
        {
            return icon.sprite;
        }
    }
}
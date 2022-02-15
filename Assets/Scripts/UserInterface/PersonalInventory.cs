using System.Collections.Generic;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using DataBases;
using Gears;
using Relics;
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
        [SerializeField] private Image shield;
        [SerializeField] private List<SlotDragAndDrop> slots;
        [SerializeField] private GameObject GearInfoPrefab;
        private BattleStats BattleStats;
        private BattleStats baseStats;
        private BattleStats total;
        
        [Header("Tooltip Events")] 
        [SerializeField] private UnitEvent UnitTooltip_ON;
        [SerializeField] private VoidEvent UnitTooltip_OFF;

        public Hero Hero { get; private set; }

        public List<SlotDragAndDrop> Slots => slots;

        public void Initialize(Hero _hero)
        {
            Hero = _hero;
            icon.sprite = Hero.UnitSprite;

            UpdateStats();
        }

        public void FillInventory()
        {
            for (int i = 0; i < Hero.Inventory.gears.Count; i++)
            {
                GameObject gearObj = Instantiate(GearInfoPrefab, slots[i].transform);
                gearObj.GetComponent<GearInfo>().Gear = Hero.Inventory.gears[i];
                gearObj.GetComponent<GearInfo>().DisplayIcon();
                slots[i].UpdateMyItem();
                slots[i].UpdateBackgroundState();
            }
        }

        private void UpdateHP(int ActualHP)
        {
            if (health == null) return;
            health.fillAmount = ActualHP / (float)total.HP;
            shield.fillAmount = total.Shield / (float) total.HP;
        }

        public void UpdateStats()
        {
            baseStats = Hero.BattleStats;
            BattleStats = new BattleStats(baseStats + Hero.Inventory.GearStats() + Hero.GetRelic().BattleStats);
            total = BattleStats;
            BattleStats.HP = Hero.ActualHP;
            
            UpdateHP(Hero.ActualHP);
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (Input.GetMouseButton(0)) return;
            UnitTooltip_ON?.Raise(Hero.Unit);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (Input.GetMouseButton(0)) return;
            UnitTooltip_OFF?.Raise();
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
            string str = "";
            str += BattleStats.Range.ToString()+ "\n";
            str += $"<sprite name=Speed> <color={colorSet.HexColor(EAffix.Speed)}>{BattleStats.Speed} </color> \n";
            return str;
        }
        
        public override string GetInfoDown()
        {
            string str = "";
            foreach (RelicSO _heroRelic in Hero.Relics)
            {
                str += _heroRelic.Name + "\n";
            }

            return str; 
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
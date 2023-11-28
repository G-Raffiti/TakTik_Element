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
using UnityEngine.Serialization;
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
        [FormerlySerializedAs("GearInfoPrefab")]
        [SerializeField] private GameObject gearInfoPrefab;
        private BattleStats battleStats;
        private BattleStats baseStats;
        private BattleStats total;
        
        [FormerlySerializedAs("UnitTooltip_ON")]
        [Header("Tooltip Events")] 
        [SerializeField] private UnitEvent unitTooltipOn;
        [FormerlySerializedAs("UnitTooltip_OFF")]
        [SerializeField] private VoidEvent unitTooltipOff;

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
            for (int _i = 0; _i < Hero.Inventory.gears.Count; _i++)
            {
                GameObject _gearObj = Instantiate(gearInfoPrefab, slots[_i].transform);
                _gearObj.GetComponent<GearInfo>().Gear = Hero.Inventory.gears[_i];
                _gearObj.GetComponent<GearInfo>().DisplayIcon();
                slots[_i].UpdateMyItem();
                slots[_i].UpdateBackgroundState();
            }
        }

        private void UpdateHp(int _actualHp)
        {
            if (health == null) return;
            health.fillAmount = _actualHp / (float)total.hp;
            shield.fillAmount = total.shield / (float) total.hp;
        }

        public void UpdateStats()
        {
            baseStats = Hero.BattleStats;
            battleStats = new BattleStats(baseStats + Hero.Inventory.GearStats() + Hero.GetRelic().BattleStats);
            total = battleStats;
            battleStats.hp = Hero.ActualHp;
            UpdateHp(Hero.ActualHp);
            
            icon.color = Hero.isDead ? Color.black : Color.white;
        }
        
        public override void OnPointerEnter(PointerEventData _eventData)
        {
            if (Input.GetMouseButton(0)) return;
            unitTooltipOn?.Raise(Hero.Unit);
        }

        public override void OnPointerExit(PointerEventData _eventData)
        {
            if (Input.GetMouseButton(0)) return;
            unitTooltipOff?.Raise();
        }

        public override string GetInfoMain()
        {
            return ColouredName();
        }

        
        public override string GetInfoLeft()
        {
            string _str = "";
            _str += $"<sprite name=AP> <color={colorSet.HexColor(EAffix.AP)}>{(int)total.ap}</color>    ";
            _str += $"<sprite name=MP> <color={colorSet.HexColor(EAffix.Mp)}>{(int)total.mp}</color> \n";
            _str += $"<sprite name=HP> <color={colorSet.HexColor(EAffix.Hp)}>{battleStats.hp} </color>/ {total.hp}    ";
            _str += $"<sprite name=Shield> <color={colorSet.HexColor(EAffix.Shield)}>{battleStats.shield}</color> \n";
            _str += $"<sprite name=Fire> <color={colorSet.HexColor(EAffix.Fire)}>{battleStats.GetPower(EElement.Fire)}</color>  <sprite name=Water> <color={colorSet.HexColor(EAffix.Water)}>{battleStats.GetPower(EElement.Water)}</color>  <sprite name=Nature> <color={colorSet.HexColor(EAffix.Nature)}>{battleStats.GetPower(EElement.Nature)}</color>";

            return _str;
        }

        public override string GetInfoRight()
        {
            string _str = "";
            _str += battleStats.gridRange.ToString()+ "\n";
            _str += $"<sprite name=Speed> <color={colorSet.HexColor(EAffix.Speed)}>{battleStats.speed} </color> \n";
            return _str;
        }
        
        public override string GetInfoDown()
        {
            string _str = "";
            foreach (RelicSo _heroRelic in Hero.Relics)
            {
                _str += _heroRelic.Name + "\n";
            }

            return _str; 
        }

        public override string ColouredName()
        {
            return $"<color={colorSet.HexColor(EColor.Ally)}>{Hero.UnitName}</color>";
        }

        public override Sprite GetIcon()
        {
            return icon.sprite;
        }
    }
}
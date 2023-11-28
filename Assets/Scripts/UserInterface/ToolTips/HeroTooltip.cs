using System.Collections.Generic;
using _DragAndDropSystem;
using Gears;
using Relics;
using UnityEngine;

namespace UserInterface.ToolTips
{
    public class HeroTooltip : UnitTooltip
    {
        [Header("Hero Tooltip")] [SerializeField]
        private Transform relicHolder;

        [SerializeField] private List<SlotDragAndDrop> inventory;

        protected override void ShowToolTip()
        {
            for (int _i = 0; _i < unit.inventory.gears.Count; _i++)
            {
                GameObject _pref = Instantiate(gearPref.gameObject, inventory[_i].transform);
                _pref.GetComponent<GearInfo>().Gear = unit.inventory.gears[_i];
                _pref.GetComponent<GearInfo>().DisplayIcon();
                inventory[_i].UpdateBackgroundState();
            }

            foreach (RelicSo _relicSo in unit.Relic.RelicEffects)
            {
                GameObject _pref = Instantiate(relicPref.gameObject, relicHolder);
                _pref.GetComponent<RelicInfo>().CreateRelic(_relicSo);
                _pref.GetComponent<RelicInfo>().DisplayIcon();
            }
            base.ShowToolTip();
            statsBlock.SetActive(true);
        }

        public override void HideTooltip()
        {
            while (relicHolder.childCount > 0)
            {
                DestroyImmediate(relicHolder.GetChild(0).gameObject);
            }

            foreach (SlotDragAndDrop _cell in inventory)
            {
                if (_cell.GetItem() == null) continue;
                DestroyImmediate(_cell.GetItem().gameObject);
            }
            base.HideTooltip();
        }
        
        protected override Vector3 LockPosition()
        {
            return new Vector3(lockPosition.x, 540-backgroundRectTransform.rect.height / 2);
        }
        
        protected override Vector3 Position()
        {
            return new Vector3(lockPosition.x, 540-backgroundRectTransform.rect.height / 2);
        }
    }

}
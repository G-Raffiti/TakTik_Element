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
            for (int i = 0; i < Unit.Inventory.gears.Count; i++)
            {
                GameObject pref = Instantiate(gearPref.gameObject, inventory[i].transform);
                pref.GetComponent<GearInfo>().Gear = Unit.Inventory.gears[i];
                pref.GetComponent<GearInfo>().DisplayIcon();
                inventory[i].UpdateBackgroundState();
            }

            foreach (RelicSO _relicSO in Unit.Relic.RelicEffects)
            {
                GameObject pref = Instantiate(relicPref.gameObject, relicHolder);
                pref.GetComponent<RelicInfo>().CreateRelic(_relicSO);
                pref.GetComponent<RelicInfo>().DisplayIcon();
            }
            base.ShowToolTip();
            StatsBlock.SetActive(true);
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
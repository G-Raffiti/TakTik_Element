using _DragAndDropSystem;
using Gears;
using Relics;
using Skills;
using Units;
using UnityEngine;

namespace UserInterface.ToolTips
{
    public class MonsterTooltip : UnitTooltip
    {
        [Header("Monster Infos")]
        [SerializeField] private SlotDragAndDrop GearSlot;
        [SerializeField] private SkillInfo Skill;
        [SerializeField] private RelicInfo RelicHolder;


        protected override void ShowToolTip()
        {
            Skill.skill = ((Monster) Unit).monsterSkill;
            Skill.Unit = Unit;
            Skill.DisplayIcon();
            GameObject _gear = Instantiate(gearPref.gameObject, GearSlot.transform);
            _gear.GetComponent<GearInfo>().Gear = Unit.Inventory.gears[0];
            _gear.GetComponent<GearInfo>().DisplayIcon();
            GearSlot.UpdateBackgroundState();
            if (Unit.Relic.RelicEffects.Count <= 0) return;
            RelicHolder.gameObject.SetActive(true);
            RelicHolder.CreateRelic(Unit.Relic.RelicEffects[0]);
            RelicHolder.DisplayIcon();
            base.ShowToolTip();
            StatsBlock.SetActive(true);
        }

        public override void HideTooltip()
        {
            RelicHolder.gameObject.SetActive(false);
            if (GearSlot.GetItem() != null)
                DestroyImmediate(GearSlot.GetItem().gameObject);
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
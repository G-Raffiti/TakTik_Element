using _DragAndDropSystem;
using Gears;
using Relics;
using Skills;
using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace UserInterface.ToolTips
{
    public class MonsterTooltip : UnitTooltip
    {
        [FormerlySerializedAs("GearSlot")]
        [Header("Monster Infos")]
        [SerializeField] private SlotDragAndDrop gearSlot;
        [FormerlySerializedAs("Skill")]
        [SerializeField] private SkillInfo skill;
        [FormerlySerializedAs("RelicHolder")]
        [SerializeField] private RelicInfo relicHolder;


        protected override void ShowToolTip()
        {
            base.ShowToolTip();
            statsBlock.SetActive(true);
            skill.skill = ((Monster) unit).MonsterSkill;
            skill.unit = unit;
            skill.DisplayIcon();
            GameObject _gear = Instantiate(gearPref.gameObject, gearSlot.transform);
            _gear.GetComponent<GearInfo>().Gear = unit.inventory.gears[0];
            _gear.GetComponent<GearInfo>().DisplayIcon();
            gearSlot.UpdateBackgroundState();
            if (unit.Relic.RelicEffects.Count <= 0) return;
            relicHolder.gameObject.SetActive(true);
            relicHolder.CreateRelic(unit.Relic.RelicEffects[0]);
            relicHolder.DisplayIcon();
        }

        public override void HideTooltip()
        {
            relicHolder.gameObject.SetActive(false);
            if (gearSlot.GetItem() != null)
                DestroyImmediate(gearSlot.GetItem().gameObject);
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
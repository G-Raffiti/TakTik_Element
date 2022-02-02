using _EventSystem.CustomEvents;
using Gears;
using Resources.ToolTip.Scripts;
using Skills;
using Units;
using UnityEngine;

namespace UserInterface.ToolTips
{
    public class TooltipManager : MonoBehaviour
    {
        [SerializeField] private SkillTooltip skillTooltip;
        [SerializeField] private GearTooltip gearTooltip;
        [SerializeField] private UnitTooltip unitTooltip;
        [SerializeField] private BasicTooltip basicTooltip;

        [Header("Event Listener")] 
        [SerializeField] private SkillEvent onSkillTooltip_ON;
        [SerializeField] private VoidEvent onSkillTooltip_OFF;
        [SerializeField] private GearInfoEvent onGearTooltip_ON;
        [SerializeField] private VoidEvent onGearTooltip_OFF;
        [SerializeField] private UnitEvent onUnitTooltip_ON;
        [SerializeField] private VoidEvent onUnitTooltip_OFF;
        [SerializeField] private InfoEvent onTooltip_ON;
        [SerializeField] private VoidEvent onTooltip_OFF;
        
        // Event Handler
        private void EventTrigger_SkillTooltip_ON(SkillInfo _skill)
        {
            if (skillTooltip.LockInPlace) return;
            skillTooltip.Skill = _skill;
            skillTooltip.DisplayInfo();
        }
        private void EventTrigger_SkillTooltip_OFF(Void empty)
        {
            if (skillTooltip.LockInPlace) return;
            skillTooltip.HideTooltip();
        }
        
        private void EventTrigger_GearTooltip_ON(GearInfo _gear)
        {
            if (gearTooltip.LockInPlace) return;
            gearTooltip.Gear = _gear;
            gearTooltip.DisplayInfo();
        }
        private void EventTrigger_GearTooltip_OFF(Void empty)
        {
            if (gearTooltip.LockInPlace) return;
            gearTooltip.HideTooltip();
        }
        private void EventTrigger_UnitTooltip_ON(Unit _unit)
        {
            if (unitTooltip.LockInPlace) return;
            unitTooltip.Unit = _unit;
            unitTooltip.DisplayInfo();
        }
        private void EventTrigger_UnitTooltip_OFF(Void empty)
        {
            if (unitTooltip.LockInPlace) return;
            unitTooltip.HideTooltip();
        }
        
        private void EventTrigger_Tooltip_ON(IInfo _info)
        {
            if (basicTooltip.LockInPlace) return;
            basicTooltip.Info = _info;
            basicTooltip.DisplayInfo();
        }
        private void EventTrigger_Tooltip_OFF(Void empty)
        {
            if (basicTooltip.LockInPlace) return;
            basicTooltip.HideTooltip();
        }
        
        // Add Listener to the Events
        private void Start()
        {
            onSkillTooltip_ON.EventListeners += EventTrigger_SkillTooltip_ON;
            onSkillTooltip_OFF.EventListeners += EventTrigger_SkillTooltip_OFF;
            onGearTooltip_ON.EventListeners += EventTrigger_GearTooltip_ON;
            onGearTooltip_OFF.EventListeners += EventTrigger_GearTooltip_OFF;
            onUnitTooltip_ON.EventListeners += EventTrigger_UnitTooltip_ON;
            onUnitTooltip_OFF.EventListeners += EventTrigger_UnitTooltip_OFF;
            onTooltip_ON.EventListeners += EventTrigger_Tooltip_ON;
            onTooltip_OFF.EventListeners += EventTrigger_Tooltip_OFF;
        }
        
        // Remove Listener on Destroy
        private void OnDestroy()
        {
            onSkillTooltip_ON.EventListeners -= EventTrigger_SkillTooltip_ON;
            onSkillTooltip_OFF.EventListeners -= EventTrigger_SkillTooltip_OFF;
            onGearTooltip_ON.EventListeners -= EventTrigger_GearTooltip_ON;
            onGearTooltip_OFF.EventListeners -= EventTrigger_GearTooltip_OFF;
            onUnitTooltip_ON.EventListeners -= EventTrigger_UnitTooltip_ON;
            onUnitTooltip_OFF.EventListeners -= EventTrigger_UnitTooltip_OFF;
            onTooltip_ON.EventListeners -= EventTrigger_Tooltip_ON;
            onTooltip_OFF.EventListeners -= EventTrigger_Tooltip_OFF;
        }
    }
}
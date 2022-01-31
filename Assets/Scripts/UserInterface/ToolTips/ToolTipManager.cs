using _EventSystem.CustomEvents;
using Gears;
using Skills;
using UnityEngine;

namespace UserInterface.ToolTips
{
    public class ToolTipManager : MonoBehaviour
    {
        [SerializeField] private SkillTooltip skillTooltip;
        [SerializeField] private GearTooltip gearTooltip;

        [Header("Event Listener")] 
        [SerializeField] private SkillEvent onSkillToolTip_ON;
        [SerializeField] private VoidEvent onSkillToolTip_OFF;
        [SerializeField] private GearInfoEvent onGearToolTip_ON;
        [SerializeField] private VoidEvent onGearTooltip_OFF;
        
        // Event Handler
        private void EventTrigger_SkillToolTip_ON(SkillInfo _skill)
        {
            skillTooltip.Skill = _skill;
            skillTooltip.DisplayInfo();
        }
        private void EventTrigger_SkillToolTip_OFF(Void empty)
        {
            skillTooltip.HideTooltip();
        }
        
        private void EventTrigger_GearToolTip_ON(GearInfo _gear)
        {
            gearTooltip.Gear = _gear;
            gearTooltip.DisplayInfo();
        }
        private void EventTrigger_GearToolTip_OFF(Void empty)
        {
            gearTooltip.HideTooltip();
        }
        
        // Add Listener to the Events
        private void Start()
        {
            onSkillToolTip_ON.EventListeners += EventTrigger_SkillToolTip_ON;
            onSkillToolTip_OFF.EventListeners += EventTrigger_SkillToolTip_OFF;
            onGearToolTip_ON.EventListeners += EventTrigger_GearToolTip_ON;
            onGearTooltip_OFF.EventListeners += EventTrigger_GearToolTip_OFF;
        }
        
        // Remove Listener on Destroy
        private void OnDestroy()
        {
            onSkillToolTip_ON.EventListeners -= EventTrigger_SkillToolTip_ON;
            onSkillToolTip_OFF.EventListeners -= EventTrigger_SkillToolTip_OFF;
            onGearToolTip_ON.EventListeners -= EventTrigger_GearToolTip_ON;
            onGearTooltip_OFF.EventListeners -= EventTrigger_GearToolTip_OFF;
        }
    }
}
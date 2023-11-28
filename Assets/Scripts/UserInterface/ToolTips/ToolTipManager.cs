using System.Collections.Generic;
using _EventSystem.CustomEvents;
using Gears;
using Resources.ToolTip.Scripts;
using Skills;
using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace UserInterface.ToolTips
{
    public class TooltipManager : MonoBehaviour
    {
        [SerializeField] private SkillTooltip skillTooltip;
        [SerializeField] private GearTooltip gearTooltip;
        [SerializeField] private UnitTooltip unitTooltip;
        [SerializeField] private BasicTooltip basicTooltip;
        [SerializeField] private HeroTooltip heroTooltip;
        [SerializeField] private MonsterTooltip monsterTooltip;
        [SerializeField] private List<Canvas> tooltipsCanvas;

        [FormerlySerializedAs("ScenePadding")]
        [Header("TO CHANGE IN EVERY SCENE")]
        [Tooltip("Refer to the margin to the edge of the screen where the Tooltips should have")]
        [SerializeField] private Vector2 scenePadding;

        [FormerlySerializedAs("onSkillTooltip_ON")]
        [Header("Event Listener")] 
        [SerializeField] private SkillEvent onSkillTooltipOn;
        [FormerlySerializedAs("onSkillTooltip_OFF")]
        [SerializeField] private VoidEvent onSkillTooltipOff;
        [FormerlySerializedAs("onGearTooltip_ON")]
        [SerializeField] private GearInfoEvent onGearTooltipOn;
        [FormerlySerializedAs("onGearTooltip_OFF")]
        [SerializeField] private VoidEvent onGearTooltipOff;
        [FormerlySerializedAs("onUnitTooltip_ON")]
        [SerializeField] private UnitEvent onUnitTooltipOn;
        [FormerlySerializedAs("onUnitTooltip_OFF")]
        [SerializeField] private VoidEvent onUnitTooltipOff;
        [FormerlySerializedAs("onTooltip_ON")]
        [SerializeField] private InfoEvent onTooltipOn;
        [FormerlySerializedAs("onTooltip_OFF")]
        [SerializeField] private VoidEvent onTooltipOff;
        [FormerlySerializedAs("onHeroTooltip_ON")]
        [SerializeField] private UnitEvent onHeroTooltipOn;
        [FormerlySerializedAs("onHeroTooltip_OFF")]
        [SerializeField] private VoidEvent onHeroTooltipOff;
        [FormerlySerializedAs("onMonsterTooltip_ON")]
        [SerializeField] private UnitEvent onMonsterTooltipOn;
        [FormerlySerializedAs("onMonsterTooltip_OFF")]
        [SerializeField] private VoidEvent onMonsterTooltipOff;
        
        // Event Handler
        private void EventTrigger_SkillTooltip_ON(SkillInfo _skill)
        {
            skillTooltip.HideTooltip();
            skillTooltip.skill = _skill;
            skillTooltip.DisplayInfo();
        }
        private void EventTrigger_SkillTooltip_OFF(Void _empty)
        {
            if (skillTooltip.LockInPlace) return;
            skillTooltip.HideTooltip();
        }
        
        private void EventTrigger_GearTooltip_ON(GearInfo _gear)
        {
            if (gearTooltip.LockInPlace) return;
            gearTooltip.gear = _gear;
            gearTooltip.DisplayInfo();
        }
        private void EventTrigger_GearTooltip_OFF(Void _empty)
        {
            if (gearTooltip.LockInPlace) return;
            gearTooltip.HideTooltip();
        }
        private void EventTrigger_UnitTooltip_ON(Unit _unit)
        {
            if (unitTooltip.LockInPlace) return;
            unitTooltip.unit = _unit;
            unitTooltip.DisplayInfo();
        }
        private void EventTrigger_UnitTooltip_OFF(Void _empty)
        {
            if (unitTooltip.LockInPlace) return;
            unitTooltip.HideTooltip();
        }
        
        private void EventTrigger_HeroTooltip_ON(Unit _unit)
        {
            heroTooltip.HideTooltip();
            monsterTooltip.HideTooltip();
            heroTooltip.unit = _unit;
            heroTooltip.DisplayInfo();
        }
        private void EventTrigger_HeroTooltip_OFF(Void _empty)
        {
            if (heroTooltip.LockInPlace) return;
            heroTooltip.HideTooltip();
        }
        private void EventTrigger_MonsterTooltip_ON(Unit _unit)
        {
            monsterTooltip.HideTooltip();
            heroTooltip.HideTooltip();
            monsterTooltip.unit = _unit;
            monsterTooltip.DisplayInfo();
        }
        private void EventTrigger_MonsterTooltip_OFF(Void _empty)
        {
            if (monsterTooltip.LockInPlace) return;
            monsterTooltip.HideTooltip();
        }
        private void EventTrigger_Tooltip_ON(IInfo _info)
        {
            if (basicTooltip.LockInPlace) return;
            basicTooltip.Info = _info;
            basicTooltip.DisplayInfo();
        }
        private void EventTrigger_Tooltip_OFF(Void _empty)
        {
            if (basicTooltip.LockInPlace) return;
            basicTooltip.HideTooltip();
        }
        
        // Add Listener to the Events
        private void Start()
        {
            foreach (Canvas _canvas in tooltipsCanvas)
            {
                _canvas.gameObject.SetActive(true);
            }
            
            skillTooltip.Padding = scenePadding;
            gearTooltip.Padding = scenePadding;
            unitTooltip.Padding = scenePadding;
            basicTooltip.Padding = new Vector2(10,10);
            heroTooltip.Padding = scenePadding;
            monsterTooltip.Padding = scenePadding;
            
            onSkillTooltipOn.EventListeners += EventTrigger_SkillTooltip_ON;
            onSkillTooltipOff.EventListeners += EventTrigger_SkillTooltip_OFF;
            
            onGearTooltipOn.EventListeners += EventTrigger_GearTooltip_ON;
            onGearTooltipOff.EventListeners += EventTrigger_GearTooltip_OFF;
            
            onUnitTooltipOn.EventListeners += EventTrigger_UnitTooltip_ON;
            onUnitTooltipOff.EventListeners += EventTrigger_UnitTooltip_OFF;
            
            onHeroTooltipOn.EventListeners += EventTrigger_HeroTooltip_ON;
            onHeroTooltipOff.EventListeners += EventTrigger_HeroTooltip_OFF;
            
            onMonsterTooltipOn.EventListeners += EventTrigger_MonsterTooltip_ON;
            onMonsterTooltipOff.EventListeners += EventTrigger_MonsterTooltip_OFF;
            
            onTooltipOn.EventListeners += EventTrigger_Tooltip_ON;
            onTooltipOff.EventListeners += EventTrigger_Tooltip_OFF;
        }
        
        // Remove Listener on Destroy
        private void OnDestroy()
        {
            onSkillTooltipOn.EventListeners -= EventTrigger_SkillTooltip_ON;
            onSkillTooltipOff.EventListeners -= EventTrigger_SkillTooltip_OFF;
            
            onGearTooltipOn.EventListeners -= EventTrigger_GearTooltip_ON;
            onGearTooltipOff.EventListeners -= EventTrigger_GearTooltip_OFF;
            
            onUnitTooltipOn.EventListeners -= EventTrigger_UnitTooltip_ON;
            onUnitTooltipOff.EventListeners -= EventTrigger_UnitTooltip_OFF;
            
            onHeroTooltipOn.EventListeners -= EventTrigger_HeroTooltip_ON;
            onHeroTooltipOff.EventListeners -= EventTrigger_HeroTooltip_OFF;
            
            onMonsterTooltipOn.EventListeners -= EventTrigger_MonsterTooltip_ON;
            onMonsterTooltipOff.EventListeners -= EventTrigger_MonsterTooltip_OFF;
            
            onTooltipOn.EventListeners -= EventTrigger_Tooltip_ON;
            onTooltipOff.EventListeners -= EventTrigger_Tooltip_OFF;
        }
    }
}
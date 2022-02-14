using System.Collections.Generic;
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
        [SerializeField] private HeroTooltip heroTooltip;
        [SerializeField] private MonsterTooltip monsterTooltip;
        [SerializeField] private List<Canvas> tooltipsCanvas;

        [Header("TO CHANGE IN EVERY SCENE")]
        [Tooltip("Refer to the margin to the edge of the screen where the Tooltips should have")]
        [SerializeField] private Vector2 ScenePadding;

        [Header("Event Listener")] 
        [SerializeField] private SkillEvent onSkillTooltip_ON;
        [SerializeField] private VoidEvent onSkillTooltip_OFF;
        [SerializeField] private GearInfoEvent onGearTooltip_ON;
        [SerializeField] private VoidEvent onGearTooltip_OFF;
        [SerializeField] private UnitEvent onUnitTooltip_ON;
        [SerializeField] private VoidEvent onUnitTooltip_OFF;
        [SerializeField] private InfoEvent onTooltip_ON;
        [SerializeField] private VoidEvent onTooltip_OFF;
        [SerializeField] private UnitEvent onHeroTooltip_ON;
        [SerializeField] private VoidEvent onHeroTooltip_OFF;
        [SerializeField] private UnitEvent onMonsterTooltip_ON;
        [SerializeField] private VoidEvent onMonsterTooltip_OFF;
        
        // Event Handler
        private void EventTrigger_SkillTooltip_ON(SkillInfo _skill)
        {
            skillTooltip.HideTooltip();
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
        
        private void EventTrigger_HeroTooltip_ON(Unit _unit)
        {
            heroTooltip.HideTooltip();
            monsterTooltip.HideTooltip();
            heroTooltip.Unit = _unit;
            heroTooltip.DisplayInfo();
        }
        private void EventTrigger_HeroTooltip_OFF(Void empty)
        {
            if (heroTooltip.LockInPlace) return;
            heroTooltip.HideTooltip();
        }
        private void EventTrigger_MonsterTooltip_ON(Unit _unit)
        {
            monsterTooltip.HideTooltip();
            heroTooltip.HideTooltip();
            monsterTooltip.Unit = _unit;
            monsterTooltip.DisplayInfo();
        }
        private void EventTrigger_MonsterTooltip_OFF(Void empty)
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
        private void EventTrigger_Tooltip_OFF(Void empty)
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
            
            skillTooltip.padding = ScenePadding;
            gearTooltip.padding = ScenePadding;
            unitTooltip.padding = ScenePadding;
            basicTooltip.padding = new Vector2(10,10);
            heroTooltip.padding = ScenePadding;
            monsterTooltip.padding = ScenePadding;
            
            onSkillTooltip_ON.EventListeners += EventTrigger_SkillTooltip_ON;
            onSkillTooltip_OFF.EventListeners += EventTrigger_SkillTooltip_OFF;
            
            onGearTooltip_ON.EventListeners += EventTrigger_GearTooltip_ON;
            onGearTooltip_OFF.EventListeners += EventTrigger_GearTooltip_OFF;
            
            onUnitTooltip_ON.EventListeners += EventTrigger_UnitTooltip_ON;
            onUnitTooltip_OFF.EventListeners += EventTrigger_UnitTooltip_OFF;
            
            onHeroTooltip_ON.EventListeners += EventTrigger_HeroTooltip_ON;
            onHeroTooltip_OFF.EventListeners += EventTrigger_HeroTooltip_OFF;
            
            onMonsterTooltip_ON.EventListeners += EventTrigger_MonsterTooltip_ON;
            onMonsterTooltip_OFF.EventListeners += EventTrigger_MonsterTooltip_OFF;
            
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
            
            onHeroTooltip_ON.EventListeners -= EventTrigger_HeroTooltip_ON;
            onHeroTooltip_OFF.EventListeners -= EventTrigger_HeroTooltip_OFF;
            
            onMonsterTooltip_ON.EventListeners -= EventTrigger_MonsterTooltip_ON;
            onMonsterTooltip_OFF.EventListeners -= EventTrigger_MonsterTooltip_OFF;
            
            onTooltip_ON.EventListeners -= EventTrigger_Tooltip_ON;
            onTooltip_OFF.EventListeners -= EventTrigger_Tooltip_OFF;
        }
    }
}
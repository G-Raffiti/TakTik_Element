using System;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using Skills;
using TMPro;
using UnityEngine;
using Void = _EventSystem.CustomEvents.Void;

namespace Shops
{
    public class CampUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtCampPoint;
        [SerializeField] private DragAndDropCell SlotSkillSwap1;
        [SerializeField] private DragAndDropCell SlotSkillSwap2;
        [SerializeField] private DragAndDropCell SlotRelicSwap1;
        [SerializeField] private DragAndDropCell SlotRelicSwap2;
        [SerializeField] private DragAndDropCell SlotForgetSkill;
        
        [Header("Event Sender")]
        [SerializeField] private VoidEvent onSwapSkill;
        [SerializeField] private VoidEvent onForgetSkill;
        [SerializeField] private VoidEvent onCampHeal;
        
        [Header("Event Listener")] 
        [SerializeField] private IntEvent onCampPointUsed;
        
        public SkillInfo SkillSwap1 { get; private set; }
        public SkillInfo SkillSwap2 { get; private set; }
        public SkillInfo SkillForget { get; private set; }
        public RelicInfo RelicSwap1 { get; private set; }
        public RelicInfo RelicSwap2 { get; private set; }
        private void OnEnable()
        {
            onCampPointUsed.EventListeners += actualiseCampPoint;
        }

        private void OnDisable()
        {
            onCampPointUsed.EventListeners -= actualiseCampPoint;
        }

        private void actualiseCampPoint(int campPoint)
        {
            txtCampPoint.text = $"You have {campPoint} Camp Points Left";
            EmptySlots();
        }

        public void ForgetSkillButton()
        {
            if (SlotForgetSkill.GetInfoSkill() == null) return;
            SkillForget = SlotForgetSkill.GetInfoSkill();
            onForgetSkill.Raise();
        }

        public void HealButton()
        {
            onCampHeal.Raise();
        }

        public void EmptySlots()
        {
            SlotSkillSwap1.RemoveItem();
            SlotSkillSwap2.RemoveItem();
            SlotRelicSwap1.RemoveItem();
            SlotRelicSwap2.RemoveItem();
            SlotForgetSkill.RemoveItem();
        }
    }
}
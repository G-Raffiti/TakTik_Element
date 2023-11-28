using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using Skills;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Shops
{
    public class ShopCampMonoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtCampPoint;
        [FormerlySerializedAs("SlotSkillDuplicate")]
        [SerializeField] private SlotDragAndDrop slotSkillDuplicate;
        [FormerlySerializedAs("SlotForgetSkill")]
        [SerializeField] private SlotDragAndDrop slotForgetSkill;
        
        [Header("Event Sender")]
        [SerializeField] private VoidEvent onDuplicateSkill;
        [SerializeField] private VoidEvent onForgetSkill;
        [SerializeField] private VoidEvent onCampHeal;
        
        [Header("Event Listener")] 
        [SerializeField] private IntEvent onCampPointUsed;
        
        public SkillInfo SkillForget { get; private set; }
        public SkillInfo SkillDuplicate { get; private set; }

        private void OnEnable()
        {
            onCampPointUsed.EventListeners += ActualiseCampPoint;
        }

        private void OnDisable()
        {
            onCampPointUsed.EventListeners -= ActualiseCampPoint;
        }

        private void ActualiseCampPoint(int _campPoint)
        {
            txtCampPoint.text = $"You have {_campPoint} Camp Points Left";
            EmptySlots();
        }

        public void ForgetSkillButton()
        {
            if (slotForgetSkill.GetInfoSkill() == null) return;
            SkillForget = slotForgetSkill.GetInfoSkill();
            onForgetSkill.Raise();
        }

        public void DuplicateSkillButton()
        {
            if (slotSkillDuplicate.GetInfoSkill() == null) return;
            SkillDuplicate = slotSkillDuplicate.GetInfoSkill();
            onDuplicateSkill.Raise();
        }

        public void HealButton()
        {
            onCampHeal.Raise();
        }

        public void EmptySlots()
        {
            slotSkillDuplicate.RemoveItem();
            slotForgetSkill.RemoveItem();
        }
    }
}
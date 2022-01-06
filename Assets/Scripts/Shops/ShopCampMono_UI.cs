using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using Skills;
using TMPro;
using UnityEngine;

namespace Shops
{
    public class ShopCampMono_UI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtCampPoint;
        [SerializeField] private DragAndDropCell SlotSkillDuplicate;
        [SerializeField] private DragAndDropCell SlotForgetSkill;
        
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
            onCampPointUsed.EventListeners += actualiseCampPoint;
            actualiseCampPoint(FindObjectOfType<ShopCampMono>().CampPoint);
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

        public void DuplicateSkillButton()
        {
            if (SlotSkillDuplicate.GetInfoSkill() == null) return;
            SkillDuplicate = SlotSkillDuplicate.GetInfoSkill();
            onDuplicateSkill.Raise();
        }

        public void HealButton()
        {
            onCampHeal.Raise();
        }

        public void EmptySlots()
        {
            SlotSkillDuplicate.RemoveItem();
            SlotForgetSkill.RemoveItem();
        }
    }
}
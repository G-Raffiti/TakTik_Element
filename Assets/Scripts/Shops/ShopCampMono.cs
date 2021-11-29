using _EventSystem.CustomEvents;
using _Instances;
using Skills;
using UnityEngine;
using Void = _EventSystem.CustomEvents.Void;

namespace Shops
{
    public class ShopCampMono : MonoBehaviour
    {
        [Header("Event Sender")]
        [SerializeField] private IntEvent onCampPointUsed;
        
        [Header("Event Listener")] 
        [SerializeField] private VoidEvent onDuplicateSkill;
        [SerializeField] private VoidEvent onForgetSkill;
        [SerializeField] private VoidEvent onCampHeal;

        private void Start()
        {
            onDuplicateSkill.EventListeners += DuplicateSkills;
            onForgetSkill.EventListeners += ForgetSkill;
            onCampHeal.EventListeners += HealHeroes;
            
            //TODO : Relic that can modify the value of CampPoint
            CampPoint = 1;
        }

        private void OnDestroy()
        {
            onDuplicateSkill.EventListeners -= DuplicateSkills;
            onForgetSkill.EventListeners -= ForgetSkill;
            onCampHeal.EventListeners -= HealHeroes;
        }

        public int CampPoint { get; private set; }

        void DuplicateSkills(Void empty)
        {
            if (CampPoint < 1) return;
            CampPoint -= 1;
            ShopCampMono_UI _shopCampMonoUI = GameObject.Find("CampFire/Background/CampUI").GetComponent<ShopCampMono_UI>();
            DuplicateSkill(_shopCampMonoUI.SkillDuplicate);
            onCampPointUsed.Raise(CampPoint);
        }

        public void ForgetSkill(Void empty)
        {
            if (CampPoint < 1) return;
            ShopCampMono_UI _shopCampMonoUI = GameObject.Find("CampFire/Background/CampUI").GetComponent<ShopCampMono_UI>();
            if (FindObjectOfType<DeckMono>().RemoveSkill(_shopCampMonoUI.SkillForget.skill.BaseSkill))
            {
                CampPoint -= 1;
                onCampPointUsed.Raise(CampPoint);
            }
        }

        private void HealHeroes(Void empty)
        {
            PlayerData.getInstance().Heroes.ForEach(h => h.HealHP(30));
        }

        private void DuplicateSkill(SkillInfo a)
        {
            a.skill.Deck.Skills.Add(a.skill.BaseSkill);
        }
    }
}
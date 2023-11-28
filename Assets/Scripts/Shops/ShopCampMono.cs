using _EventSystem.CustomEvents;
using _Instances;
using Decks;
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
            //TODO : Relic that can modify the value of CampPoint
            CampPoint = 2;
            
            onDuplicateSkill.EventListeners += DuplicateSkills;
            onForgetSkill.EventListeners += ForgetSkill;
            onCampHeal.EventListeners += HealHeroes;
            
            onCampPointUsed.Raise(CampPoint);
        }

        private void OnDestroy()
        {
            onDuplicateSkill.EventListeners -= DuplicateSkills;
            onForgetSkill.EventListeners -= ForgetSkill;
            onCampHeal.EventListeners -= HealHeroes;
        }

        public int CampPoint { get; private set; }

        void DuplicateSkills(Void _empty)
        {
            if (CampPoint < 1) return;
            CampPoint -= 1;
            ShopCampMonoUI _shopCampMonoUI = GameObject.Find("CampFire/Background/CampUI").GetComponent<ShopCampMonoUI>();
            SkillInfo _duplicate = _shopCampMonoUI.SkillDuplicate;
            _duplicate.skill.Deck.drawPile.Add(_duplicate.skill.BaseSkill);
            onCampPointUsed.Raise(CampPoint);
        }

        public void ForgetSkill(Void _empty)
        {
            if (CampPoint < 1) return;
            ShopCampMonoUI _shopCampMonoUI = GameObject.Find("CampFire/Background/CampUI").GetComponent<ShopCampMonoUI>();
            if (FindObjectOfType<DeckMono>().RemoveSkill(_shopCampMonoUI.SkillForget.skill.BaseSkill))
            {
                CampPoint -= 1;
                onCampPointUsed.Raise(CampPoint);
            }
        }

        private void HealHeroes(Void _empty)
        {
            if (CampPoint < 1) return;
            CampPoint -= 1;
            PlayerData.GetInstance().Heroes.ForEach(_h => _h.HealHp(30));
            onCampPointUsed.Raise(CampPoint);
        }
    }
}
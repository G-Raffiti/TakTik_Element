using System;
using System.Linq;
using _EventSystem.CustomEvents;
using _Instances;
using Skills;
using UnityEngine;
using Void = _EventSystem.CustomEvents.Void;

namespace Shops
{
    public class Camp : MonoBehaviour
    {
        [Header("Event Sender")]
        [SerializeField] private IntEvent onCampPointUsed;
        
        [Header("Event Listener")] 
        [SerializeField] private VoidEvent onSwapSkill;
        [SerializeField] private VoidEvent onSwapRelic;
        [SerializeField] private VoidEvent onForgetSkill;
        [SerializeField] private VoidEvent onCampHeal;

        private void Start()
        {
            onSwapSkill.EventListeners += SwapSkills;
            onSwapRelic.EventListeners += SwapRelics;
            onForgetSkill.EventListeners += ForgetSkill;
            
            //TODO : Relic that can modify the value of CampPoint
            CampPoint = 2;
            onCampPointUsed.Raise(CampPoint);
        }

        private void OnDestroy()
        {
            onSwapSkill.EventListeners -= SwapSkills;
            onSwapRelic.EventListeners -= SwapRelics;
            onForgetSkill.EventListeners -= ForgetSkill;
        }

        public int CampPoint { get; private set; }

        public void SwapSkills(Void empty)
        {
            if (CampPoint < 1) return;
            CampPoint -= 1;
            CampUI _campUI = GameObject.Find("CampFire/Background/CampUI").GetComponent<CampUI>();
            SwapSkill(_campUI.SkillSwap1, _campUI.SkillSwap2);
            onCampPointUsed.Raise(CampPoint);
        }
        
        public void SwapRelics(Void empty)
        {
            if (CampPoint < 1) return;
            CampPoint -= 1;
            CampUI _campUI = GameObject.Find("CampFire/Background/CampUI").GetComponent<CampUI>();
            SwapRelic(_campUI.RelicSwap1, _campUI.RelicSwap2);
            onCampPointUsed.Raise(CampPoint);
        }

        public void ForgetSkill(Void empty)
        {
            if (CampPoint < 1) return;
            CampUI _campUI = GameObject.Find("CampFire/Background/CampUI").GetComponent<CampUI>();
            if (_campUI.SkillForget.Deck.RemoveSkill(_campUI.SkillForget.Skill))
            {
                CampPoint -= 1;
                onCampPointUsed.Raise(CampPoint);
            }
        }

        private void HealHeroes(Void empty)
        {
            PlayerData.getInstance().Heroes.ForEach(h => h.HealHP(30));
        }

        private void SwapRelic(RelicInfo a, RelicInfo b)
        {
            a.Deck.Relics.Add(b.Relic);
            b.Deck.Relics.Add(a.Relic);
            a.Deck.Relics.Remove(a.Relic);
            b.Deck.Relics.Remove(b.Relic);
        }

        public void SwapSkill(SkillInfo a, SkillInfo b)
        {
            a.Deck.AddSkill(b.Skill);
            b.Deck.AddSkill(a.Skill);
            a.Deck.RemoveSkill(a.Skill);
            b.Deck.RemoveSkill(b.Skill);
        }
    }
}
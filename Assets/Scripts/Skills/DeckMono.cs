using System;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _Extension;
using Relics;
using Units;
using UnityEngine;

namespace Skills
{
    public class DeckMono : MonoBehaviour
    {
        [SerializeField] private BoolEvent onEndBattle;
        [SerializeField] public List<SkillSO> DrawPile = new List<SkillSO>();

        public List<SkillSO> HandSkills = new List<SkillSO>();
        public List<SkillSO> DiscardPile = new List<SkillSO>();
        public List<SkillSO> ConsumedSkills = new List<SkillSO>();
        public List<RelicSO> Relics = new List<RelicSO>();
        public Relic Relic = new Relic();

        public static int HAND_SIZE = 5;

        private void Start()
        {
            onEndBattle.EventListeners += OnBattleEndRaised;
        }

        private void OnDestroy()
        {
            onEndBattle.EventListeners -= OnBattleEndRaised;
        }

        public void DrawNewHand()
        {
            if (HandSkills.Count >= HAND_SIZE) return;
            Draw(HAND_SIZE - HandSkills.Count);
        }

        public void ClearHandSkills()
        {
            DiscardPile.AddRange(HandSkills);
            HandSkills = new List<SkillSO>();
        }

        /// <summary>
        /// Shuffle the Deck and the Discard together and change the Actual Skill to be the first of the shuffled list.
        /// </summary>
        public void ShuffleDeck()
        {
            DrawPile.AddRange(DiscardPile);
            DiscardPile = new List<SkillSO>();
            
            DrawPile.Shuffle();
        }

        /// <summary>
        /// Draw n skills.
        /// </summary>
        public void Draw(int n)
        {
            int maxRange = (DrawPile.Count < n) ? DrawPile.Count : n;
            int remainRange = n - maxRange;
            
            HandSkills.AddRange(DrawPile.GetRange(0, maxRange));
            DrawPile.RemoveRange(0, maxRange);

            if (remainRange > 0)
            {
                ShuffleDeck();
                HandSkills.AddRange(DrawPile.GetRange(0, Math.Min(remainRange, DrawPile.Count)));
                DrawPile.RemoveRange(0, Math.Min(remainRange, DrawPile.Count));
            }
        }
        
        
        /// <summary>
        /// Add up all the relics
        /// </summary>
        public void UpdateDeck()
        {
            Relic = Relic.CreateRelic(Relics);
        }
        
        public List<Skill> GetHandSkills(Unit unit)
        {
            Debug.Log("Hand Size :" + HandSkills.Count);
            List<Skill> skills = new List<Skill>();
            
            foreach (SkillSO skill in HandSkills)
            {
                skills.Add(Skill.CreateSkill(skill, this, unit));
            }

            return skills;
        }
        
        /// <summary>
        /// Method Called when a Skill is Used.
        /// </summary>
        public bool UseSkill(Skill skill)
        {
            if (!HandSkills.Contains(skill.BaseSkill)) return false;
            
            if (skill.BaseSkill.Consumable) 
                ConsumedSkills.Add(skill.BaseSkill);
            else DiscardPile.Add(skill.BaseSkill);
            
            HandSkills.Remove(skill.BaseSkill);
            
            return true;
        }

        public void Initialize()
        {
            DrawPile.AddRange(ConsumedSkills);
            ConsumedSkills = new List<SkillSO>();
            DrawPile.AddRange(DiscardPile);
            DiscardPile = new List<SkillSO>();
            DrawPile.AddRange(HandSkills);
            HandSkills = new List<SkillSO>();
            ShuffleDeck();
            UpdateDeck();
        }

        public void InitializeForCamp()
        {
            DrawPile.AddRange(ConsumedSkills);
            ConsumedSkills = new List<SkillSO>();
            DrawPile.AddRange(DiscardPile);
            DiscardPile = new List<SkillSO>();
            DrawPile.AddRange(HandSkills);
            HandSkills = new List<SkillSO>();
            DrawPile.Sort((s, s2) => String.Compare(s.Name, s2.Name, StringComparison.Ordinal));
            DrawPile.Sort((s, s2) => s.Cost.CompareTo(s2.Cost));
        }

        private void OnBattleEndRaised(bool item)
        {
            Initialize();
        }

        public void LearnSkill(SkillSO _monsterSkill, Skill learning)
        {
            HandSkills.Remove(learning.BaseSkill);
            List<SkillSO> _newList = new List<SkillSO> {_monsterSkill};
            _newList.AddRange(HandSkills);
            HandSkills = new List<SkillSO>(_newList);
        }

        /// <summary>
        /// Used in Camp to Swap Skill or in Event to Forget a Skill Completly
        /// </summary>
        /// <param name="_Skill"></param>
        /// <returns></returns>
        public bool RemoveSkill(SkillSO _Skill)
        {
            if (!DrawPile.Contains(_Skill)) return false;
            DrawPile.Remove(_Skill);
            return true;
        }

        public void PrintDebug()
        {
            /*
            Debug.Log("pile:" + Skills.Count + " " +
                      "hand:" + HandSkills.Count + " " +
                      "used:" + UsedSkills.Count);
            */
        }

        public void AddHandSkill(SkillSO _skill)
        {
            HandSkills.Add(_skill);
        }
    }
}
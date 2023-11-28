using System;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _Extension;
using Relics;
using Skills;
using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace Decks
{
    public class DeckMono : MonoBehaviour
    {
        [SerializeField] private BoolEvent onEndBattle;
        [FormerlySerializedAs("DrawPile")]
        [SerializeField] public List<SkillSo> drawPile = new List<SkillSo>();

        [FormerlySerializedAs("HandSkills")]
        public List<SkillSo> handSkills = new List<SkillSo>();
        [FormerlySerializedAs("DiscardPile")]
        public List<SkillSo> discardPile = new List<SkillSo>();
        [FormerlySerializedAs("ConsumedSkills")]
        public List<SkillSo> consumedSkills = new List<SkillSo>();
        [FormerlySerializedAs("Relics")]
        public List<RelicSo> relics = new List<RelicSo>();
        public Relic Relic = new Relic();

        public static int HandSize = 5;

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
            if (handSkills.Count >= HandSize) return;
            Draw(HandSize - handSkills.Count);
        }

        public void ClearHandSkills()
        {
            discardPile.AddRange(handSkills);
            handSkills = new List<SkillSo>();
        }

        /// <summary>
        /// Shuffle the Deck and the Discard together and change the Actual Skill to be the first of the shuffled list.
        /// </summary>
        public void ShuffleDeck()
        {
            drawPile.AddRange(discardPile);
            discardPile = new List<SkillSo>();
            
            drawPile.Shuffle();
        }

        /// <summary>
        /// Draw n skills.
        /// </summary>
        public void Draw(int _n)
        {
            int _maxRange = (drawPile.Count < _n) ? drawPile.Count : _n;
            int _remainRange = _n - _maxRange;
            
            handSkills.AddRange(drawPile.GetRange(0, _maxRange));
            drawPile.RemoveRange(0, _maxRange);

            if (_remainRange > 0)
            {
                ShuffleDeck();
                handSkills.AddRange(drawPile.GetRange(0, Math.Min(_remainRange, drawPile.Count)));
                drawPile.RemoveRange(0, Math.Min(_remainRange, drawPile.Count));
            }
        }
        
        
        /// <summary>
        /// Add up all the relics
        /// </summary>
        public void UpdateDeck()
        {
            Relic = Relic.CreateRelic(relics);
        }
        
        public List<Skill> GetHandSkills(Unit _unit)
        {
            List<Skill> _skills = new List<Skill>();
            
            foreach (SkillSo _skill in handSkills)
            {
                _skills.Add(Skill.CreateSkill(_skill, this, _unit));
            }

            return _skills;
        }
        
        /// <summary>
        /// Method Called when a Skill is Used.
        /// </summary>
        public bool UseSkill(Skill _skill)
        {
            if (!handSkills.Contains(_skill.BaseSkill)) return false;
            
            if (_skill.BaseSkill.Consumable) 
                consumedSkills.Add(_skill.BaseSkill);
            else discardPile.Add(_skill.BaseSkill);

            handSkills.Remove(_skill.BaseSkill);
            
            return true;
        }

        public void Initialize()
        {
            drawPile.AddRange(consumedSkills);
            consumedSkills = new List<SkillSo>();
            drawPile.AddRange(discardPile);
            discardPile = new List<SkillSo>();
            drawPile.AddRange(handSkills);
            handSkills = new List<SkillSo>();
            ShuffleDeck();
            UpdateDeck();
        }

        public void InitializeForCamp()
        {
            drawPile.AddRange(consumedSkills);
            consumedSkills = new List<SkillSo>();
            drawPile.AddRange(discardPile);
            discardPile = new List<SkillSo>();
            drawPile.AddRange(handSkills);
            handSkills = new List<SkillSo>();
            drawPile.Sort((_s, _s2) => String.Compare(_s.Name, _s2.Name, StringComparison.Ordinal));
            drawPile.Sort((_s, _s2) => _s.Cost.CompareTo(_s2.Cost));
        }

        private void OnBattleEndRaised(bool _item)
        {
            Initialize();
        }

        public void LearnSkill(SkillSo _monsterSkill, Skill _learning)
        {
            List<SkillSo> _newList = new List<SkillSo> {_monsterSkill};
            _newList.AddRange(handSkills);
            handSkills = new List<SkillSo>(_newList);
        }

        /// <summary>
        /// Used in Camp to Swap Skill or in Event to Forget a Skill Completly
        /// </summary>
        /// <param name="_skill"></param>
        /// <returns></returns>
        public bool RemoveSkill(SkillSo _skill)
        {
            if (!drawPile.Contains(_skill)) return false;
            drawPile.Remove(_skill);
            return true;
        }

        public void AddHandSkill(SkillSo _skill)
        {
            handSkills.Add(_skill);
        }
    }
}
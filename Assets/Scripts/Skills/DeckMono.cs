using System;
using System.Collections;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _Extension;
using Cells;
using Units;
using UnityEngine;

namespace Skills
{
    public class DeckMono : MonoBehaviour
    {
        [SerializeField] private BoolEvent onEndBattle;
        [SerializeField] public List<SkillSO> Skills = new List<SkillSO>();

        private List<SkillSO> HandSkills = new List<SkillSO>();
        private List<SkillSO> UsedSkills = new List<SkillSO>();
        private List<SkillSO> ConsumedSkills = new List<SkillSO>();
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
            if (HandSkills.Count > 0) return;
            Draw(HAND_SIZE);
        }

        public void ClearHandSkills()
        {
            UsedSkills.AddRange(HandSkills);
            HandSkills = new List<SkillSO>();
        }

        /// <summary>
        /// Shuffle the Deck and the Discard together and change the Actual Skill to be the first of the shuffled list.
        /// </summary>
        public void ShuffleDeck()
        {
            Skills.AddRange(UsedSkills);
            UsedSkills = new List<SkillSO>();
            
            Skills.Shuffle();
        }

        /// <summary>
        /// Draw n skills.
        /// </summary>
        public void Draw(int n)
        {
            HandSkills = new List<SkillSO>();

            int maxRange = (Skills.Count < n) ? Skills.Count : n;
            int remainRange = n - maxRange;
            
            HandSkills.AddRange(Skills.GetRange(0, maxRange));
            Skills.RemoveRange(0, maxRange);

            if (remainRange > 0)
            {
                ShuffleDeck();
                HandSkills.AddRange(Skills.GetRange(0, remainRange));
                Skills.RemoveRange(0, remainRange);
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
            List<Skill> skills = new List<Skill>();
            
            foreach (SkillSO skill in HandSkills)
            {
                skills.Add(Skill.CreateSkill(skill, Relic, unit));
            }

            return skills;
        }
        
    /*    
    #region Change Methode for Relics

        /// <summary>
        /// Public Method for the Relics to change the Element of the Skills
        /// </summary>
        public void ChangeElement(Element _element)
        {
            Element = _element;
        }
        
        /// <summary>
        /// Public Method for Relics to change witch Units can be affected by the Skills
        /// </summary>
        public void ChangeAffect(EAffect _affect)
        {
            Affect = _affect;
        }
        
        /// <summary>
        /// Public Method for Relics to change the Range Type of the Skills
        /// </summary>
        public void ChangeRangeType(EZone _rangeType)
        {
            range.RangeType = _rangeType;
        }
        
        /// <summary>
        /// Public Method for Relics to change the Zone Type of the Skills
        /// </summary>
        public void ChangeZoneType(EZone _zoneType)
        {
            range.ZoneType = _zoneType;
        }
        
        /// <summary> 
        /// Public Method for Relics to change if the Skills of the Deck need View
        /// </summary>
        public void ChangeNeedView(bool _needView)
        {
            range.NeedView = _needView;
        }
        
        /// <summary> 
        /// Public Method for Relics to change the Skill's Cost
        /// </summary>
        public void ChangeCost(int _added)
        {
            Cost += _added;
            if (Cost < 0)
                Cost = 0;
        }

    #endregion
*/
        /// <summary>
        /// Method Called when a Skill is Used.
        /// </summary>
        public bool UseSkill(SkillSO skill)
        {
            if (!HandSkills.Contains(skill)) return false;
            
            if (skill.Consumable) ConsumedSkills.Add(skill);
            else UsedSkills.Add(skill);
            
            HandSkills.Remove(skill);
            
            return true;
        }

        public void Initialize()
        {
            Skills.AddRange(ConsumedSkills);
            ConsumedSkills = new List<SkillSO>();
            Skills.AddRange(UsedSkills);
            UsedSkills = new List<SkillSO>();
            Skills.AddRange(HandSkills);
            HandSkills = new List<SkillSO>();
            ShuffleDeck();
            Draw(HAND_SIZE);
        }
        
        /// <summary>
        /// Method Called on Monster Spawn to initialize the Info Skill they Use.
        /// </summary>
        /// <param name="_skillSO"></param>
        /// <param name="_relicSO"></param>
        public void InitializeForMonster(SkillSO _skillSO, List<RelicSO> _relicSO)
        {
            if (_skillSO != null)
                Skills.Add(_skillSO);
            if (_relicSO != null)
                Relics.AddRange(_relicSO);
        }

        private static IEnumerator HighlightZone(List<Cell> zone)
        {
            foreach (Cell _cell in zone)
            {
                _cell.MarkAsHighlighted();
                yield return new WaitForSeconds(0.05f);
            }
            foreach (Cell _cell in zone)
            {
                _cell.MarkAsHighlighted();
            }
            yield return new WaitForSeconds(0.2f);
            zone.ForEach(c => c.UnMark());
        }

        private void OnBattleEndRaised(bool item)
        {
            Initialize();
            Skills.Sort((s, s2) => String.Compare(s.Name, s2.Name, StringComparison.Ordinal));
            Skills.Sort((s, s2) => s.Cost.CompareTo(s2.Cost));
        }
        
        public void LearnSkill(SkillSO _monsterSkill)
        {
            List<SkillSO> _newList = new List<SkillSO> {_monsterSkill};
            _newList.AddRange(Skills);
            Skills = new List<SkillSO>(_newList);
        }

        /// <summary>
        /// Used in Camp to Swap Skill or in Event to Add a Skill to a Specific Deck
        /// </summary>
        /// <param name="LearnSkill">Skill to Add in the Deck</param>
        public void AddSkill(SkillSO LearnSkill)
        {
            Skills.Add(LearnSkill);
        }
        
        /// <summary>
        /// Used in Camp to Swap Skill or in Event to Forget a Skill Completly
        /// </summary>
        /// <param name="_Skill"></param>
        /// <returns></returns>
        public bool RemoveSkill(SkillSO _Skill)
        {
            if (!Skills.Contains(_Skill)) return false;
            Skills.Remove(_Skill);
            return true;
        }

        public void PrintDebug()
        {
            Debug.Log("pile:" + Skills.Count + " " +
                      "hand:" + HandSkills.Count + " " +
                      "used:" + UsedSkills.Count);
        }
    }
}
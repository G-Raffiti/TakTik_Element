using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using Grid;
using Skills;
using Units;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

namespace UserInterface
{
    /// <summary>
    /// Class that Update the Skills UI on Battle Scene
    /// </summary>
    public class DeckUI : MonoBehaviour
    {
        [SerializeField] private Image frameOfFirstSkill;
        [SerializeField] private SkillInfo FirstSkill;
        private AllDecks allDecks;
        private Dictionary<SkillInfo, int> skills;
        private Unit unit;

        [Header("Event Listener")]
        [SerializeField] private UnitEvent onUnitStartTurn;
        [SerializeField] private VoidEvent onSkillUsed;
        [SerializeField] private VoidEvent onActionDone;

        private IEnumerator Start()
        {
            allDecks = GameObject.Find("Decks").GetComponent<AllDecks>();
            skills = new Dictionary<SkillInfo, int>();
            for (int i = 0; i < transform.childCount; i++)
            {
                skills.Add(transform.GetChild(i).gameObject.GetComponent<SkillInfo>(), transform.childCount - i - 1);
            }
            
            int number = transform.GetSiblingIndex();
            
            yield return new WaitForSeconds(0.1f);
            
            foreach (SkillInfo _skillsKey in skills.Keys)
            {
                _skillsKey.Deck = allDecks.Decks[number];
            }
        }

        private void OnEnable()
        {
            onSkillUsed.EventListeners += onSkillUsedRaised;
            onUnitStartTurn.EventListeners += onUnitStartTurnRaised;
            onActionDone.EventListeners += onSkillUsedRaised;
        }

        private void OnDisable()
        {
            onSkillUsed.EventListeners -= onSkillUsedRaised;
            onUnitStartTurn.EventListeners -= onUnitStartTurnRaised;
            onActionDone.EventListeners -= onSkillUsedRaised;
        }

        public void onUnitStartTurnRaised(Unit item)
        {
            if (item.playerNumber != 0)
            {
                unit = BattleStateManager.instance.Units.Find(Unit => Unit.playerNumber == 0);
            }
            else unit = item;
            
            UpdateDisplay();
        }

        public void onSkillUsedRaised<T>(T param)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            foreach (KeyValuePair<SkillInfo,int> _skill in skills)
            {
                if (_skill.Key.Deck.Skills.Count - 1 < _skill.Value)
                {
                    foreach (Transform _child in _skill.Key.gameObject.transform)
                    {
                        _child.gameObject.SetActive(false);
                    }
                }
                else
                {
                    foreach (Transform _child in _skill.Key.gameObject.transform)
                    {
                        _child.gameObject.SetActive(true);
                    }
                    _skill.Key.UpdateSkill(_skill.Value, unit);
                    _skill.Key.DisplayIcon();
                }
            }

            if (BattleStateManager.instance.PlayingUnit.BattleStats.AP < 1 || FirstSkill.Cost > FirstSkill.Unit.BattleStats.AP)
            {
                frameOfFirstSkill.color = Color.grey;
            }
            else frameOfFirstSkill.color = Color.white;
        }
    }
}
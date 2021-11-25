using System.Collections;
using _EventSystem.CustomEvents;
using Grid;
using Skills;
using Units;
using UnityEngine;

namespace UserInterface
{
    /// <summary>
    /// Class that Update the Skills UI on Battle Scene
    /// </summary>
    public class DeckMonoUI : MonoBehaviour
    {
        private DeckMono deck;
        private Unit unit;

        [SerializeField] private GameObject skillBtn;

        [Header("Event Listener")]
        [SerializeField] private UnitEvent onUnitStartTurn;
        [SerializeField] private VoidEvent onSkillUsed;
        [SerializeField] private VoidEvent onActionDone;

        private IEnumerator Start()
        {
            AllDecksMono allDecks = GameObject.Find("Decks").GetComponent<AllDecksMono>();
            
            yield return new WaitForSeconds(0.1f);
            
            deck = allDecks.Deck;
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
            
            StartCoroutine(DrawAnimation());
        }

        public void onSkillUsedRaised<T>(T param)
        {
            DrawAnimation();
        }

        private void OnSkillUsed()
        {
            
        }

        private IEnumerator DrawAnimation()
        {
            yield return new WaitForSeconds(0.2f);

            foreach (Skill skill in deck.GetHandSkills())
            {
                GameObject skillInfo = GameObject.Instantiate(skillBtn, transform);
                skillInfo.GetComponent<SkillInfo>().skill = skill;
                skillInfo.GetComponent<SkillInfo>().skill = skill;
            }
            /*
            if (BattleStateManager.instance.PlayingUnit.BattleStats.AP < 1 || FirstSkill.Cost > FirstSkill.Unit.BattleStats.AP)
            {
                frameOfFirstSkill.color = Color.grey;
            }
            else frameOfFirstSkill.color = Color.white;
            */
        }
    }
}
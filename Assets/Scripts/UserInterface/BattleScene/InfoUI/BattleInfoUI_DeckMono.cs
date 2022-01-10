using System.Collections.Generic;
using _DragAndDropSystem;
using Decks;
using Skills;
using StateMachine;
using Units;
using UnityEngine;

namespace UserInterface.BattleScene.InfoUI
{
    public class BattleInfoUI_DeckMono : MonoBehaviour
    {
        private DeckMono deck;
        [SerializeField] private DragAndDropCell SlotPrefab;
        [SerializeField] private SkillInfo SkillPrefab;
        [SerializeField] private Transform DrawPile;
        [SerializeField] private Transform DiscardPile;
        [SerializeField] private Transform ConsumedPile;

        private void OnEnable()
        {
            deck = FindObjectOfType<DeckMono>();
            Unit nextHero = BattleStateManager.instance.Units.Find(u => u.playerNumber == 0);

            SetPile(deck.DrawPile, nextHero, DrawPile);
            SetPile(deck.DiscardPile, nextHero, DiscardPile);
            SetPile(deck.ConsumedSkills, nextHero, ConsumedPile);
        }

        private void SetPile(List<SkillSO> Pile, Unit user, Transform holder)
        {
            foreach (SkillSO _skillSO in Pile)
            {
                GameObject SlotsObj = Instantiate(SlotPrefab.gameObject, holder);
                GameObject SkillObj = Instantiate(SkillPrefab.gameObject, SlotsObj.transform);
                SlotsObj.GetComponent<DragAndDropCell>().containType = DragAndDropCell.ContainType.Skill;
                SlotsObj.GetComponent<DragAndDropCell>().cellType = DragAndDropCell.CellType.DropOnly;
                
                SkillObj.GetComponent<SkillInfo>().skill = Skill.CreateSkill(_skillSO, deck, user);
                SkillObj.GetComponent<SkillInfo>().Unit = user;

                SkillObj.GetComponent<SkillInfo>().DisplayIcon();
            }
        }

        private void OnDisable()
        {
            EmptyPile(DrawPile);
            EmptyPile(DiscardPile);
            EmptyPile(ConsumedPile);
        }

        private void EmptyPile(Transform Pile)
        {
            while (Pile.childCount > 2)
            {
                DestroyImmediate(Pile.GetChild(1).gameObject);
            }
        }
    }
}
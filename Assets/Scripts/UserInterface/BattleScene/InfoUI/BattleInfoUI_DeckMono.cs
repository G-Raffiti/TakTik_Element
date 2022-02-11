using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private SlotDragAndDrop SlotPrefab;
        [SerializeField] private SkillInfo SkillPrefab;
        [SerializeField] private Transform DrawPile;
        [SerializeField] private Transform DiscardPile;
        [SerializeField] private Transform ConsumedPile;
        [SerializeField] private BattleHero Hero;
        [SerializeField] private Hero blancHero;
        [SerializeField] private DeckMono deck;


        private void OnEnable()
        {
            Unit nextHero;
            if (deck == null)
            {
                deck = GameObject.Find("DeckMono/Deck1").GetComponent<DeckMono>();
            }
            Debug.Log(BattleStateManager.instance.PlayingUnit != null ? BattleStateManager.instance.PlayingUnit.UnitName : "a pas trouvé");
            if (BattleStateManager.instance.PlayingUnit == null) return;
            if (BattleStateManager.instance.PlayingUnit.playerNumber == 0)
                nextHero = BattleStateManager.instance.PlayingUnit;
            else
            {
                Hero.Spawn(blancHero);
                nextHero = Hero;
            }

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
                SlotsObj.GetComponent<SlotDragAndDrop>().containType = SlotDragAndDrop.ContainType.Skill;
                SlotsObj.GetComponent<SlotDragAndDrop>().cellType = SlotDragAndDrop.CellType.DropOnly;
                
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
using System;
using System.Collections.Generic;
using System.Linq;
using _DragAndDropSystem;
using _Extension;
using Decks;
using Players;
using Skills;
using StateMachine;
using Units;
using UnityEngine;

namespace UserInterface.BattleScene.InfoUI
{
    public class BattleInfoUI_DeckMono : MonoBehaviour
    {
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
            if (BattleStateManager.instance.PlayingUnit == null) return;
            if (BattleStateManager.instance.PlayingUnit.playerType == EPlayerType.HUMAN)
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
            List<SkillSO> _copie = new List<SkillSO>();
            _copie.AddRange(Pile);
            _copie.Sort((s, s1) => String.Compare(s.Name, s1.Name, StringComparison.Ordinal));
            _copie.Sort((s, s1) => s.Cost.CompareTo(s1.Cost));
            foreach (SkillSO _skillSO in _copie)
            {
                GameObject SkillObj = Instantiate(SkillPrefab.gameObject, holder);
                
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
            int i = 0;
            //Array to hold all child obj
            GameObject[] allChildren = new GameObject[Pile.childCount - 2];

            //Find all child obj and store to that array
            foreach (Transform child in Pile)
            {
                if (child.GetComponent<SkillInfo>() == null) continue;
                allChildren[i] = child.gameObject;
                i += 1;
            }

            //Now destroy them
            foreach (GameObject child in allChildren)
            {
                DestroyImmediate(child.gameObject);
            }
            
        }
    }
}
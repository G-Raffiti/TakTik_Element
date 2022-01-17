using System;
using System.Collections.Generic;
using _DragAndDropSystem;
using _Instances;
using Cells;
using Decks;
using Relics;
using Skills;
using StateMachine;
using TMPro;
using Units;
using UnityEngine;
using UserInterface;

namespace Score
{
    public class ScoreSceneUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private List<Cell> BossSpawnPlace;
        [SerializeField] private List<PersonalInventory> Inventories;
        [SerializeField] private List<Transform> RelicHolder;
        [SerializeField] private DragAndDropCell SlotPrefab;
        [SerializeField] private RelicInfo RelicPrefab;
        private DeckMono deck;
        [SerializeField] private SkillInfo SkillPrefab;
        [SerializeField] private Transform DrawPile;
        [SerializeField] private BattleHero hero;
        private bool isWin;
        
        private void Start()
        {
            KeepBetweenScene.EndGame();
            isWin = PlayerData.getInstance().IsVictory;
            title.text = isWin ? "Victory !" : "Game Over !";
            for (int i = 0; i < ScoreHolder.Bosses.Count; i++)
            {
                ScoreHolder.Bosses[i].Spawn(BossSpawnPlace[i]);
            }

            for (int i = 0; i < PlayerData.getInstance().Heroes.Count; i++)
            {
                Inventories[i].Initialize(PlayerData.getInstance().Heroes[i]);
                Inventories[i].FillInventory();
                foreach (RelicSO _relic in PlayerData.getInstance().Heroes[i].Relics)
                {
                    GameObject slotObj = Instantiate(SlotPrefab.gameObject, RelicHolder[i].transform);
                    slotObj.GetComponent<DragAndDropCell>().cellType = DragAndDropCell.CellType.DropOnly;
                    slotObj.GetComponent<DragAndDropCell>().containType = DragAndDropCell.ContainType.Relic;
                    GameObject relicObj = Instantiate(RelicPrefab.gameObject, slotObj.transform);
                    relicObj.GetComponent<RelicInfo>().CreateRelic(_relic);
                    slotObj.GetComponent<DragAndDropCell>().AddItem(relicObj.GetComponent<DragAndDropItem>());
                    relicObj.GetComponent<RelicInfo>().DisplayIcon();
                }
            }
            deck = FindObjectOfType<DeckMono>();

            SetPile(deck.DrawPile, hero, DrawPile);
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

        public void Btn_ExitGame()
        {
            Application.Quit();
        }
    }
}
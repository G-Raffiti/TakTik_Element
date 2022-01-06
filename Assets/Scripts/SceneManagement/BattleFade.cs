using System;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _Instances;
using EndConditions;
using StateMachine;
using UnityEngine;
using Random = UnityEngine.Random;
using Void = _EventSystem.CustomEvents.Void;

namespace SceneManagement
{
    public class BattleFade : MonoBehaviour
    {
        [SerializeField] private ChangeScene SceneManager;
        [SerializeField] private ShopChoice_UI shopChoiceUI;
        [SerializeField] private GameObject nextBattle;
        [SerializeField] private EndRun_UI endRunUI;
        
        [Header("Event Sender")] [SerializeField]
        private VoidEvent onUIEnable;
        
        [Header("Event Listener")]
        [SerializeField] private BoolEvent onBattleIsOver;
        [SerializeField] private VoidEvent onQuitShop;
        [SerializeField] private VoidEvent NextBattle;
        
        private void Start()
        {
            onBattleIsOver.EventListeners += OnBattleEnded;
            onQuitShop.EventListeners += StartNewBattle;
            NextBattle.EventListeners += StartNewBattle;
        }

        private void OnDestroy()
        {
            onBattleIsOver.EventListeners -= OnBattleEnded;
            onQuitShop.EventListeners -= StartNewBattle;
            NextBattle.EventListeners -= StartNewBattle;
        }

        private void StartNewBattle(Void empty)
        {
            KeepBetweenScene.EndBattle();
            SceneManager.LoadScene("BattleScene");
            //Todo: Show this Log on screen (BattleStart)
            //Debug.Log($"Battle: {KeepBetweenScene.BattleNumber} Stage: {KeepBetweenScene.Stage}");
        }

        private void OnBattleEnded(bool isWin)
        {
            onUIEnable.Raise();

            if (BattleStateManager.instance.endCondition.Type == EConditionType.Last || !isWin)
            {
                endRunUI.gameObject.SetActive(true);
                endRunUI.Open(isWin);
                return;
            }
            
            YouWin();
        }

        public void GoToScoreScene()
        {
            SceneManager.LoadScene("ScoreScene");
        }

        private void YouWin()
        {
            switch (BattleStateManager.instance.endCondition.Type)
            {
                case EConditionType.LootBox:
                    nextBattle.SetActive(true);
                    break;
                default:
                    shopChoiceUI.gameObject.SetActive(true);
                    break;
            }
        }

        public void GoToShop(string _shopName)
        {
            SceneManager.LoadScene(_shopName);
        }
    }
}
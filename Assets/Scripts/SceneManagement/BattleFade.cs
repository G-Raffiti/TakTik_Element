using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private GameObject resurectionPool;
        
        [Header("Event Sender")] [SerializeField]
        private VoidEvent onUIEnable;
        
        [Header("Event Listener")]
        [SerializeField] private BoolEvent onBattleIsOver;
        [SerializeField] private VoidEvent onQuitShop;
        [SerializeField] private VoidEvent NextBattle;
        [SerializeField] private VoidEvent goToResurrection;
        
        private void Start()
        {
            onBattleIsOver.EventListeners += OnBattleEnded;
            onQuitShop.EventListeners += StartNewBattle;
            NextBattle.EventListeners += StartNewBattle;
            goToResurrection.EventListeners += GoToResurrection;
        }

        private void OnDestroy()
        {
            onBattleIsOver.EventListeners -= OnBattleEnded;
            onQuitShop.EventListeners -= StartNewBattle;
            NextBattle.EventListeners -= StartNewBattle;
            goToResurrection.EventListeners -= GoToResurrection;
        }

        private void GoToResurrection(Void _obj)
        {
            SceneManager.LoadScene("Reborn");
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
            if (PlayerData.getInstance().Heroes.Any(h => h.isDead) && PlayerData.getInstance().ResurrectionPoints > 0)
            {
                resurectionPool.SetActive(true);
                return;
            }
                
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
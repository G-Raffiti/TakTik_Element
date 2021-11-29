using System;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _EventSystem.Listeners;
using _Instances;
using UnityEngine;
using Random = UnityEngine.Random;
using Void = _EventSystem.CustomEvents.Void;

namespace SceneManagement
{
    public class BattleFade : MonoBehaviour
    {
        [SerializeField] private ChangeScene SceneManager;
        [SerializeField] private List<String> ShopSceneNames;
        
        [Header("Event Listener")]
        [SerializeField] private BoolEvent onBattleEnd;
        [SerializeField] private VoidEvent onQuitShop;
        
        private void Start()
        {
            onBattleEnd.EventListeners += OnBattleEnded;
            onQuitShop.EventListeners += StartNewBattle;
        }

        private void OnDestroy()
        {
            onBattleEnd.EventListeners -= OnBattleEnded;
            onQuitShop.EventListeners -= StartNewBattle;
        }

        private void StartNewBattle(Void empty)
        {
            SceneManager.LoadScene("BattleScene");
        }

        private void OnBattleEnded(bool isWin)
        {
            if (!isWin)
            {
                YouLoose();
            }
            else YouWin();
        }

        private void YouLoose()
        {
            SceneManager.LoadScene("ScoreScene");
        }

        private void YouWin()
        {
            if (KeepBetweenScene.Stage == 1 && KeepBetweenScene.BattleNumber == 0)
            {
                StartNewBattle(new Void());
                return;
            }

            GoToRandomShop();
        }

        private void GoToRandomShop()
        {
            SceneManager.LoadScene(ShopSceneNames[Random.Range(0, ShopSceneNames.Count)]);
        }

    }
}
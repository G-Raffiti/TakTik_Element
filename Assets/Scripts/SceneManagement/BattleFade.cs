using System;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
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
        public bool BattleEnded = false;
        
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
            KeepBetweenScene.EndBattle();
            SceneManager.LoadScene("BattleScene");
            Debug.Log($"Battle: {KeepBetweenScene.BattleNumber} Stage: {KeepBetweenScene.Stage}");
        }

        private void OnBattleEnded(bool isWin)
        {
            if (BattleEnded) return;
            BattleEnded = true;
            
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
            if (KeepBetweenScene.Stage == 0 && KeepBetweenScene.BattleNumber == 0)
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
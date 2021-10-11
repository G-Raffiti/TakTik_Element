using System;
using _EventSystem.CustomEvents;
using _EventSystem.Listeners;
using UnityEngine;

namespace SceneManagement
{
    public class BattleFade : MonoBehaviour
    {
        [SerializeField] private BoolEvent onBattleEnd;
        [SerializeField] private ChangeScene SceneManager;
        
        private void Start()
        {
            onBattleEnd.EventListeners += OnBattleEnded;
        }

        private void OnDestroy()
        {
            onBattleEnd.EventListeners -= OnBattleEnded;
        }

        private void StartNewBattle()
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
            StartNewBattle();
        }

    }
}
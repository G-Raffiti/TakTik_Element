using System;
using _EventSystem.CustomEvents;
using _EventSystem.Listeners;
using UnityEngine;

namespace SceneManagement
{
    public class BattleFade : MonoBehaviour, IGameEventListener<bool>
    {
        [SerializeField] private BoolEvent onBattleEnd;
        [SerializeField] private ChangeScene SceneManager;
        
        private void Start()
        {
            onBattleEnd.RegisterListener(this);
        }

        private void OnDestroy()
        {
            onBattleEnd.UnregisterListener(this);
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
        
        public void OnEventRaised(bool item)
        {
            OnBattleEnded(item);
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
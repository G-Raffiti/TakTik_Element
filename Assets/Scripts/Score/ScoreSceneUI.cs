using System;
using System.Collections.Generic;
using _Instances;
using Cells;
using TMPro;
using Units;
using UnityEngine;

namespace Score
{
    public class ScoreSceneUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private List<Cell> BossSpawnPlace;
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
            
        }
    }
}
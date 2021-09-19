﻿using System.Collections.Generic;
using System.Linq;
using _Instances;
using Grid;
using Units;
using UnityEngine;

namespace BattleOver
{
    [CreateAssetMenu(fileName = "EndCondition_LootBoxes", menuName = "Scriptable Object/End Conditions/LootBoxes")]
    public class LootBoxes : EndConditionSO
    {
        public override bool battleIsOver(BattleStateManager StateManager)
        {
            List<Unit> playerHeroes = StateManager.Units.Where(unit => unit.playerNumber == 0).ToList();
            WinCondition = playerHeroes.Count > 0;
            if (KeepBetweenScene.Stage == 0)
            {
                KeepBetweenScene.NextStage();
            }
            return StateManager.GridObjects.Count == 0 || StateManager.Turn >= 3 || playerHeroes.Count == 0;
        }
    }
}
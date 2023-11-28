using System;
using StateMachine;
using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace UserInterface.BattleScene
{
    public class DeathWaiter : MonoBehaviour    
    {
        [FormerlySerializedAs("Inventory")]
        [SerializeField] private BattleInventoryUI inventory;
        [FormerlySerializedAs("Relic")]
        [SerializeField] private BattleRelicChoiceUI relic;
        [FormerlySerializedAs("CloseInventoryBtn")]
        [SerializeField] private GameObject closeInventoryBtn;
        [FormerlySerializedAs("CloseRelicBtn")]
        [SerializeField] private GameObject closeRelicBtn;

        private bool relicLooted = false;
        private void Update()
        {
            if (BattleStateManager.instance.DeadThisTurn.Count == 0 
                || inventory.gameObject.activeSelf 
                || relic.gameObject.activeSelf) 
                return;

            if (!relicLooted 
                && BattleStateManager.instance.DeadThisTurn[0].Type == EMonster.Boss)
            {
                closeRelicBtn.SetActive(true);
                relic.gameObject.SetActive(true);
                relic.ShowOnKill(BattleStateManager.instance.DeadThisTurn[0]);
                relicLooted = true;
                return;
            }
                
            closeInventoryBtn.SetActive(true);
            inventory.gameObject.SetActive(true);
            inventory.ShowOnKill(BattleStateManager.instance.DeadThisTurn[0]);
        }
    }
}
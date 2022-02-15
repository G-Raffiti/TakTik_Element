using System;
using StateMachine;
using Units;
using UnityEngine;

namespace UserInterface.BattleScene
{
    public class DeathWaiter : MonoBehaviour    
    {
        [SerializeField] private BattleInventory_UI Inventory;
        [SerializeField] private BattleRelicChoice_UI Relic;
        [SerializeField] private GameObject CloseInventoryBtn;
        [SerializeField] private GameObject CloseRelicBtn;

        private bool RelicLooted = false;
        private void Update()
        {
            if (BattleStateManager.instance.DeadThisTurn.Count == 0 
                || Inventory.gameObject.activeSelf 
                || Relic.gameObject.activeSelf) 
                return;

            if (!RelicLooted 
                && BattleStateManager.instance.DeadThisTurn[0].Type == EMonster.Boss)
            {
                CloseRelicBtn.SetActive(true);
                Relic.gameObject.SetActive(true);
                Relic.ShowOnKill(BattleStateManager.instance.DeadThisTurn[0]);
                RelicLooted = true;
                return;
            }
                
            CloseInventoryBtn.SetActive(true);
            Inventory.gameObject.SetActive(true);
            Inventory.ShowOnKill(BattleStateManager.instance.DeadThisTurn[0]);
            
            BattleStateManager.instance.DeadThisTurn.Remove(BattleStateManager.instance.DeadThisTurn[0]);
        }
    }
}
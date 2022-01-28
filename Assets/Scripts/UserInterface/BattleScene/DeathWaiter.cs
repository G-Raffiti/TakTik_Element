using System;
using StateMachine;
using UnityEngine;

namespace UserInterface.BattleScene
{
    public class DeathWaiter : MonoBehaviour    
    {
        [SerializeField] private BattleInventory_UI Inventory;
        [SerializeField] private GameObject CloseBtn;
        
        private void Update()
        {
            if (BattleStateManager.instance.DeadThisTurn.Count == 0 || Inventory.gameObject.activeSelf) return;
            
            CloseBtn.SetActive(true);
            Inventory.gameObject.SetActive(true);
            Inventory.ShowOnKill(BattleStateManager.instance.DeadThisTurn[0]);
            BattleStateManager.instance.DeadThisTurn.Remove(BattleStateManager.instance.DeadThisTurn[0]);
        }
    }
}
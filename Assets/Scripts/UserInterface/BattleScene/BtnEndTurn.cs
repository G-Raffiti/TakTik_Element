using System;
using _EventSystem.CustomEvents;
using UnityEngine;

namespace UserInterface.BattleScene
{
    public class BtnEndTurn : MonoBehaviour
    {
        [Header("Event Listener")]
        [SerializeField] private BoolEvent onBattleEnd;
        
        [SerializeField] private BattleInventory_UI inventory;

        private void OnEnable()
        {
            onBattleEnd.EventListeners += SetInactive;
        }
        
        private void OnDisable()
        {
            onBattleEnd.EventListeners -= SetInactive;
        }

        public void SetInactive(bool winCondition)
        {
            gameObject.SetActive(false);
        }

        private void OnMouseDown()
        {
            if (!inventory.isActiveAndEnabled) return;
        }
    }
}
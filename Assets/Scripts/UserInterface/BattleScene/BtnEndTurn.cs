using System;
using System.Linq;
using _EventSystem.CustomEvents;
using StateMachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterface.BattleScene
{
    public class BtnEndTurn : MonoBehaviour
    {
        [Header("Event Listener")]
        [SerializeField] private BoolEvent onBattleEnd;
        [SerializeField] private BattleInventory_UI inventory;
        [SerializeField] private VoidEvent onEndTurn;

        private void OnEnable()
        {
            onBattleEnd.EventListeners += SetInactive;
        }
        
        private void OnDisable()
        {
            onBattleEnd.EventListeners -= SetInactive;
        }

        private void SetInactive(bool winCondition)
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
        }

        public void OnPointerClick()
        {
            if (inventory.gameObject.activeSelf) return;
            if (BattleStateManager.instance.Monsters.Where(m => m.isDying).ToList().Count > 0) return;
            onEndTurn.Raise();
        }
    }
}
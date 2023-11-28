using System;
using _EventSystem.CustomEvents;
using _LeanTween.Framework;
using _SaveSystem;
using Score;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Menu
{
    public class StartMenu : MonoBehaviour
    {
        [FormerlySerializedAs("SaveSystem")]
        [SerializeField] private SavingLoading saveSystem;
        [SerializeField] private VoidEvent startNewGame;
        [SerializeField] private IntEvent startGame;
        [FormerlySerializedAs("Menu")]
        [SerializeField] private CanvasGroup menu;

        public void LoadGame(int _saveNumber)
        {
            saveSystem.Load(_saveNumber);
            if (PersistentData.Instance.saveNumber != _saveNumber)
            {
                PersistentData.Instance.saveNumber = _saveNumber;
                startNewGame.Raise();
                return;
            }
            startGame.Raise(_saveNumber);
        }

        private void Start()
        {
            LeanTween.alphaCanvas(menu, 1, 2f).setDelay(1f);
        }

        class ButtonCustom : Button
        {
            public override void OnPointerClick(PointerEventData _eventData)
            {
                base.OnPointerClick(_eventData);
            }
        }
    }
}
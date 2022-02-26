using System;
using _EventSystem.CustomEvents;
using _LeanTween.Framework;
using _SaveSystem;
using Score;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menu
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] private SavingLoading SaveSystem;
        [SerializeField] private VoidEvent startNewGame;
        [SerializeField] private IntEvent startGame;
        [SerializeField] private CanvasGroup Menu;

        public void LoadGame(int saveNumber)
        {
            SaveSystem.Load(saveNumber);
            if (PersistentData.Instance.SaveNumber != saveNumber)
            {
                PersistentData.Instance.SaveNumber = saveNumber;
                startNewGame.Raise();
                return;
            }
            startGame.Raise(saveNumber);
        }

        private void Start()
        {
            LeanTween.alphaCanvas(Menu, 1, 2f).setDelay(1f);
        }

        class ButtonCustom : Button
        {
            public override void OnPointerClick(PointerEventData eventData)
            {
                base.OnPointerClick(eventData);
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                throw new NotImplementedException();
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                throw new NotImplementedException();
            }
        }
    }
}
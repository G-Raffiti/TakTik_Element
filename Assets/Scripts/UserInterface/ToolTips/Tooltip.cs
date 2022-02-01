﻿using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.ToolTips
{
    public abstract class Tooltip : MonoBehaviour
    {
        [SerializeField] private Canvas popupCanvas;
        [SerializeField] private GameObject obj;
        [SerializeField] private RectTransform backgroundRectTransform;
        private Vector3 offset;
        private float padding;

        private void Awake()
        {
            if (obj.activeSelf) { HideTooltip(); }
        }

        private void Start()
        {
            offset = new Vector2(10, 10);
            padding = 5;
            HideTooltip();
        }

        private void Update()
        {
            FollowCursor();
        }

        private void FollowCursor()
        {
            if (!obj.activeSelf) { return; }

            Vector3 _newPos = Input.mousePosition + offset;
            _newPos.z = 0f;
            float _rightEdgeToScreenEdgeDistance = Screen.width - (_newPos.x + backgroundRectTransform.rect.width * popupCanvas.scaleFactor) - padding;
            if (_rightEdgeToScreenEdgeDistance < 0)
            {
                _newPos.x = Input.mousePosition.x - backgroundRectTransform.rect.width * popupCanvas.scaleFactor - offset.x;
            }

            #region if the Tooltip place is not on the Left of the mouse 
            /*
        float leftEdgeToScreenEdgeDistance = 0 - (newPos.x - backgroundRectTransform.rect.width * popupCanvas.scaleFactor) + padding;
        if (leftEdgeToScreenEdgeDistance > 0)
        {
            newPos.x += leftEdgeToScreenEdgeDistance;
        }
        */
            #endregion

            float _topEdgeToScreenEdgeDistance = Screen.height - (_newPos.y + backgroundRectTransform.rect.height * popupCanvas.scaleFactor) - padding;
            if (_topEdgeToScreenEdgeDistance < 0)
            {
                _newPos.y += _topEdgeToScreenEdgeDistance;
            }
            backgroundRectTransform.transform.position = _newPos;
        }

        public void DisplayInfo()
        {
            ShowToolTip();
            obj.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(backgroundRectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(backgroundRectTransform);
        }

        public virtual void HideTooltip()
        {
            obj.SetActive(false);
        }
        protected abstract void ShowToolTip();
    }
}

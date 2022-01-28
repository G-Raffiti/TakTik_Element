using System.Collections.Generic;
using System.Text;
using _EventSystem.CustomEvents;
using Resources.ToolTip.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Void = _EventSystem.CustomEvents.Void;

namespace UserInterface
{
    public class Tooltip : MonoBehaviour
    {
        [Header("Event Listener")] 
        [SerializeField] private InfoEvent ToolTipOn;
        [SerializeField] private VoidEvent ToolTipOff;
        
        [Header("References")]
        [SerializeField] private GameObject obj;
        [SerializeField] private List<Image> icon;
        [SerializeField] private TMP_Text mainTxt;
        [SerializeField] private TMP_Text leftTxt;
        [SerializeField] private TMP_Text rightTxt;
        [SerializeField] private TMP_Text downTxt;
        [SerializeField] private RectTransform backgroundRectTransform;

        private Vector3 offset;
        private float padding;
        private Canvas popupCanvas;

        private void Awake()
        {
            popupCanvas = gameObject.GetComponent<Canvas>();
            if (obj.activeSelf) { HideTooltip(); }
        }

        private void Start()
        {
            ToolTipOn.EventListeners += DisplayInfo;
            ToolTipOff.EventListeners += HideTooltip;
            offset = new Vector2(10, 10);
            padding = 5;
            HideTooltip();
        }

        private void OnDestroy()
        {
            ToolTipOn.EventListeners -= DisplayInfo;
            ToolTipOff.EventListeners -= HideTooltip;
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

        public void DisplayInfo(IInfo _info)
        {
            StringBuilder _main = new StringBuilder();
            StringBuilder _left = new StringBuilder();
            StringBuilder _right = new StringBuilder();
            StringBuilder _down = new StringBuilder();

            _main.Append(_info.GetInfoMain());
            _left.Append(_info.GetInfoLeft());
            _right.Append(_info.GetInfoRight());
            _down.Append(_info.GetInfoDown());


            mainTxt.text = _main.ToString();
            leftTxt.text = _left.ToString();
            rightTxt.text = _right.ToString();
            downTxt.text = _down.ToString();

            icon.ForEach(i => i.sprite = _info.GetIcon());

            obj.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(backgroundRectTransform);
        }

        public void HideTooltip(Void empty)
        {
            obj.SetActive(false);
        }
        public void HideTooltip()
        {
            obj.SetActive(false);
        }
    }
}

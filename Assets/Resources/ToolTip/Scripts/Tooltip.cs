using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.ToolTip.Scripts
{
    public class Tooltip : MonoBehaviour
    {
        [SerializeField] private GameObject obj;
        [SerializeField] private Image icon;
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

            icon.sprite = _info.GetIcon();

            obj.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(backgroundRectTransform);
        }

        public void HideTooltip()
        {
            obj.SetActive(false);
        }
    }
}

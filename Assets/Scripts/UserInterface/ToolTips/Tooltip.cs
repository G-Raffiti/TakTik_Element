using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.ToolTips
{
    public abstract class Tooltip : MonoBehaviour
    {
        [SerializeField] protected Canvas popupCanvas;
        [SerializeField] private GameObject obj;
        [SerializeField] protected RectTransform backgroundRectTransform;
        [SerializeField] private bool canLock;
        [SerializeField] protected Vector2 lockPosition;
        protected bool lockInPlace;
        [SerializeField] protected Vector3 offset;
        public Vector2 padding { get; set; }
        public bool LockInPlace => lockInPlace;

        private void Awake()
        {
            if (obj.activeSelf) { HideTooltip(); }
        }

        private void Start()
        {
            HideTooltip();
        }

        private void Update()
        {
            if (!obj.activeSelf) return; 
            
            if (Input.GetMouseButton(1) && canLock)
            {
                lockInPlace = true;
            }

            if (lockInPlace && Input.GetMouseButton(0))
            {
                lockInPlace = false;
                HideTooltip();
                return;
            }

            if (lockInPlace)
            {
                backgroundRectTransform.transform.position = LockPosition();
                return;
            }
            
            backgroundRectTransform.transform.position = Position();
        }

        protected virtual Vector3 Position()
        {
            Vector3 _newPos = Input.mousePosition + offset;
            _newPos.z = 0f;
            float _rightEdgeToScreenEdgeDistance = Screen.width - (_newPos.x + backgroundRectTransform.rect.width * popupCanvas.scaleFactor) - padding.x;
            if (_rightEdgeToScreenEdgeDistance < 0)
            {
                _newPos.x = Input.mousePosition.x - backgroundRectTransform.rect.width * popupCanvas.scaleFactor - offset.x;
            }

            float _topEdgeToScreenEdgeDistance = Screen.height - (_newPos.y + backgroundRectTransform.rect.height * popupCanvas.scaleFactor) - padding.y;
            if (_topEdgeToScreenEdgeDistance < 0)
            {
                _newPos.y += _topEdgeToScreenEdgeDistance;
            }
            return _newPos;
        }

        protected virtual Vector3 LockPosition() => Position();

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

using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UserInterface.UI_Menu
{
    public class MovingPanelMenu : MonoBehaviour
    {
        [FormerlySerializedAs("MovingPanel")]
        [SerializeField] private GameObject movingPanel;
        [FormerlySerializedAs("Menu")]
        [SerializeField] private MenuPanel menu;
        [FormerlySerializedAs("CloseBtn")]
        [SerializeField] private Button closeBtn;
        [SerializeField] private float movementAnimationSpeed;
        [SerializeField] private int offset;
        private int actualPanel = -1;
        private bool isOnScreen;
        private float minWidth;

        private void Start()
        {
            minWidth = movingPanel.GetComponent<RectTransform>().sizeDelta.x +1;
        }

        public void ShowPanelBtn(int _index)
        {
            StartCoroutine(ShowPanel(_index));
        }

        public void HidePanelBtn()
        {
            StartCoroutine(Hide());
        }

        private IEnumerator ShowPanel(int _index)
        {
            if (actualPanel == _index) yield break;
            if (isOnScreen)
            {
                yield return Hide();
            }
            menu.Menu(_index);
            actualPanel = _index;
            yield return new WaitWhile(() =>
                movingPanel.GetComponent<RectTransform>().sizeDelta.x <= minWidth);
            float _width = movingPanel.GetComponent<RectTransform>().sizeDelta.x;
            Vector3 _pos = movingPanel.transform.localPosition;
            Vector3 _destinationPos = new Vector3(_pos.x - _width - offset, _pos.y, _pos.z);
            while (movingPanel.transform.localPosition != _destinationPos)
            {
                movingPanel.transform.localPosition = Vector3.MoveTowards(movingPanel.transform.localPosition, _destinationPos,
                    Time.deltaTime * movementAnimationSpeed);
                yield return 0;
            }
            isOnScreen = true;
            yield return MoveCloseBtn(true);
        }

        private IEnumerator Hide()
        {
            yield return MoveCloseBtn(false);
            float _width = movingPanel.GetComponent<RectTransform>().sizeDelta.x;
            Vector3 _pos = movingPanel.transform.localPosition;
            Vector3 _destinationPos = new Vector3(_pos.x + _width + offset, _pos.y, _pos.z);
            while (movingPanel.transform.localPosition != _destinationPos)
            {
                movingPanel.transform.localPosition = Vector3.MoveTowards(movingPanel.transform.localPosition, _destinationPos,
                    Time.deltaTime * movementAnimationSpeed);
                yield return 0;
            }
            menu.Close();
            isOnScreen = false;
            actualPanel = -1;
        }

        private IEnumerator MoveCloseBtn(bool _show)
        {
            if (closeBtn == null) yield break;
            float _height = closeBtn.GetComponent<RectTransform>().sizeDelta.y;
            if (!_show) _height *= -1;
            Vector3 _pos = closeBtn.transform.localPosition;
            Vector3 _destinationPos = new Vector3(_pos.x, _pos.y - _height, _pos.z);
            while (closeBtn.transform.localPosition != _destinationPos)
            {
                closeBtn.transform.localPosition = Vector3.MoveTowards(closeBtn.transform.localPosition, _destinationPos,
                    Time.deltaTime * movementAnimationSpeed);
                yield return 0;
            }
        }
        
    }
}
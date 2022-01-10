using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.UI_Menu
{
    public class MovingPanelMenu : MonoBehaviour
    {
        [SerializeField] private GameObject MovingPanel;
        [SerializeField] private MenuPanel Menu;
        [SerializeField] private Button CloseBtn;
        [SerializeField] private float movementAnimationSpeed;
        [SerializeField] private int offset;
        private bool isOnScreen;
        private float minWidth;

        private void Start()
        {
            minWidth = MovingPanel.GetComponent<RectTransform>().sizeDelta.x;
        }

        public void ShowPanelBtn(int index)
        {
            StartCoroutine(showPanel(index));
        }

        public void HidePanelBtn()
        {
            StartCoroutine(Hide());
        }

        private IEnumerator showPanel(int index)
        {
            if (isOnScreen)
            {
                yield return Hide();
            }
            Menu.Menu(index);
            yield return new WaitWhile(() =>
                MovingPanel.GetComponent<RectTransform>().sizeDelta.x <= minWidth);
            float width = MovingPanel.GetComponent<RectTransform>().sizeDelta.x;
            Vector3 pos = MovingPanel.transform.localPosition;
            Vector3 _destinationPos = new Vector3(pos.x - width - offset, pos.y, pos.z);
            while (MovingPanel.transform.localPosition != _destinationPos)
            {
                MovingPanel.transform.localPosition = Vector3.MoveTowards(MovingPanel.transform.localPosition, _destinationPos,
                    Time.deltaTime * movementAnimationSpeed);
                yield return 0;
            }
            isOnScreen = true;
            yield return MoveCloseBtn(true);
        }

        private IEnumerator Hide()
        {
            yield return MoveCloseBtn(false);
            float width = MovingPanel.GetComponent<RectTransform>().sizeDelta.x;
            Vector3 pos = MovingPanel.transform.localPosition;
            Vector3 _destinationPos = new Vector3(pos.x + width + offset, pos.y, pos.z);
            while (MovingPanel.transform.localPosition != _destinationPos)
            {
                MovingPanel.transform.localPosition = Vector3.MoveTowards(MovingPanel.transform.localPosition, _destinationPos,
                    Time.deltaTime * movementAnimationSpeed);
                yield return 0;
            }
            Menu.Close();
            yield return new WaitUntil(() =>
                MovingPanel.GetComponent<RectTransform>().sizeDelta.x <= minWidth);
            isOnScreen = false;
        }

        private IEnumerator MoveCloseBtn(bool show)
        {
            if (CloseBtn == null) yield break;
            float height = CloseBtn.GetComponent<RectTransform>().sizeDelta.y;
            if (!show) height *= -1;
            Vector3 pos = CloseBtn.transform.localPosition;
            Vector3 _destinationPos = new Vector3(pos.x, pos.y - height, pos.z);
            while (CloseBtn.transform.localPosition != _destinationPos)
            {
                CloseBtn.transform.localPosition = Vector3.MoveTowards(CloseBtn.transform.localPosition, _destinationPos,
                    Time.deltaTime * movementAnimationSpeed);
                yield return 0;
            }
        }
        
    }
}
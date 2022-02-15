using _LeanTween.Framework;
using Resources.ToolTip.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace UserInterface.ToolTips
{
    public class BasicTooltip : Tooltip
    {
        [HideInInspector] public IInfo Info;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI main;
        [SerializeField] private TextMeshProUGUI left;
        [SerializeField] private TextMeshProUGUI right;
        [SerializeField] private TextMeshProUGUI down;
        [SerializeField] private LayoutElement Layout;
        [SerializeField] private int widthMax;
        private CanvasGroup canvasGroup;

        public void Awake()
        {
            canvasGroup = backgroundRectTransform.GetComponent<CanvasGroup>();
        }

        protected override void Update()
        {
            base.Update();
            if (Input.GetMouseButtonDown(0))
                HideTooltip();
        }

        protected override void ShowToolTip()
        {
            LeanTween.alphaCanvas(canvasGroup, 0, 0);
            LeanTween.alphaCanvas(canvasGroup, 1, 0.1f).delay = 0.4f;
            Layout.enabled = false;
            icon.sprite = Info.GetIcon();
            main.text = Info.GetInfoMain();
            left.text = Info.GetInfoLeft();
            right.text = Info.GetInfoRight();
            down.text = Info.GetInfoDown();
            
            if (left.preferredWidth + right.preferredWidth > widthMax)
                Layout.enabled = true;
        }

        public override void HideTooltip()
        {
            LeanTween.alphaCanvas(canvasGroup, 0, 0.1f);
            base.HideTooltip();
        }
    }
}
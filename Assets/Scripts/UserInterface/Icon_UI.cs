using Grid;
using Resources.ToolTip.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class Icon_UI : MonoBehaviour
    {
        [SerializeField] private Image icon;

        public void DisplayIcon(IInfo _info)
        {
            icon.sprite = _info.GetIcon();
            EnableIcon();
        }

        public void EnableIcon()
        {
            if (BattleStateManager.instance.PlayingUnit.playerNumber == 0 && (int) BattleStateManager.instance.PlayingUnit.BattleStats.AP > 0)
            {
                icon.color = Color.white;
            }
            else icon.color = Color.gray;
        }
    }
}
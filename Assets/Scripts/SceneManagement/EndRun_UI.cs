using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SceneManagement
{
    public class EndRun_UI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI winLoose;
        [SerializeField] private Button goToButton;

        public Button GoToButton => goToButton;

        public void Open(bool winCondition)
        {
            winLoose.text = winCondition ? "Victory !" : "Game Over !";
        }
    }
}
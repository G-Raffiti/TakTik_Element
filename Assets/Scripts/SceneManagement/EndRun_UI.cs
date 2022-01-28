using TMPro;
using UnityEngine;

namespace SceneManagement
{
    public class EndRun_UI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI winLoose;

        public void Open(bool winCondition)
        {
            winLoose.text = winCondition ? "Victory !" : "Game Over !";
        }
    }
}
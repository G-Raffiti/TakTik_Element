using TMPro;
using UnityEngine;

namespace SceneManagement
{
    public class EndRunUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI winLoose;

        public void Open(bool _winCondition)
        {
            winLoose.text = _winCondition ? "Victory !" : "Game Over !";
        }
    }
}
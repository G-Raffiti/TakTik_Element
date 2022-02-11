using UnityEngine;
using UserInterface;

namespace UISetupState
{
    public class GameOptions : MonoBehaviour
    {
        [SerializeField] private UI_Manager Manager;

        public void isLifeBarActive(bool isActive)
        {
            Manager.MenuActiveLifeBar(isActive);
            foreach (LifeBar _bar in FindObjectsOfType<LifeBar>())
            {
                _bar.Activate(isActive);
            }
        }

        public void isCompleteSats(bool isActive)
        {
            Manager.MenuActiveCompleteStats(isActive);
        }
    }
}